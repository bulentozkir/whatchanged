using System.Diagnostics;
using System.Text.Json;
using PCChangeTracker.Core;
using PCChangeTracker.Windows;

namespace PCChangeTracker.App;

/// <summary>
/// Captures snapshots with the current user's default, unelevated security context only. Nothing here requests
/// elevation: machine-wide sources are read with standard permissions, and whatever they cannot read is reported as incomplete.
/// </summary>
public interface ICaptureService
{
    Task<Snapshot> CaptureAsync(IReadOnlySet<Category> enabled, IProgress<string> progress, CancellationToken cancellationToken,
        CollectionScope scope = CollectionScope.Both);
}

public sealed class CaptureService(string dataDirectory) : ICaptureService
{
    public async Task<Snapshot> CaptureAsync(IReadOnlySet<Category> enabled, IProgress<string> progress, CancellationToken cancellationToken,
        CollectionScope scope = CollectionScope.Both)
    {
        ValidateRequest(scope);
        if (CollectorTransport.IsAdministrator)
            throw new CaptureAccessException("Start ChangeTracker normally, not as administrator. Checks run only with standard Windows permissions.");
        cancellationToken.ThrowIfCancellationRequested();
        if (scope == CollectionScope.Both)
        {
            var machine = await CaptureLocalAsync(enabled, CollectionScope.Machine, progress, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var user = await CaptureLocalAsync(enabled, CollectionScope.CurrentUser, progress, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return Combine(user, machine);
        }
        return await CaptureLocalAsync(enabled, scope, progress, cancellationToken);
    }

    internal static void ValidateRequest(CollectionScope scope)
    {
        if (scope is not CollectionScope.CurrentUser and not CollectionScope.Machine and not CollectionScope.Both)
            throw new ArgumentException("Choose current-user or machine-wide collection.");
    }

    internal static Snapshot Combine(Snapshot user, Snapshot machine)
    {
        if (user.Scope != CollectionScope.CurrentUser || user.Elevated || machine.Scope != CollectionScope.Machine || machine.Elevated ||
            user.SchemaVersion != machine.SchemaVersion)
            throw new InvalidDataException("Combined checks require separate unelevated user and machine observations.");
        var results = new List<CollectionResult>();
        foreach (var category in Enum.GetValues<Category>())
        {
            var runs = new[] { user, machine }.Where(snapshot => CollectorCatalog.Supports(category, snapshot.Scope))
                .Select(snapshot => snapshot.Results.Single(result => result.Category == category)).ToArray();
            var keys = runs.Select(run => run.FingerprintKeyId).Where(key => key is not null).Distinct().ToArray();
            var items = runs.SelectMany(run => run.Items).ToArray();
            var status = runs.All(run => run.Status == CollectionStatus.Disabled) ? CollectionStatus.Disabled
                : runs.All(run => run.Status == CollectionStatus.Failed) ? CollectionStatus.Failed
                : runs.All(run => run.Status == CollectionStatus.Success) && keys.Length <= 1 && items.Select(item => item.Key).Distinct(StringComparer.OrdinalIgnoreCase).Count() == items.Length
                    ? CollectionStatus.Success : CollectionStatus.Partial;
            results.Add(new(category, status, runs.Min(run => run.StartedAt), runs.Max(run => run.FinishedAt), items,
                status == CollectionStatus.Success ? null : status == CollectionStatus.Disabled ? "Not selected for collection."
                    : "One or more selected scopes did not provide a complete compatible reading. This category is not compared.")
                { Version = 2, FingerprintKeyId = keys.Length == 1 ? keys[0] : null });
        }
        return new(Guid.NewGuid(), user.StartedAt < machine.StartedAt ? user.StartedAt : machine.StartedAt,
            user.FinishedAt > machine.FinishedAt ? user.FinishedAt : machine.FinishedAt, results)
            { Scope = CollectionScope.Both, SchemaVersion = user.SchemaVersion };
    }

    internal async Task<Snapshot> CaptureLocalAsync(IReadOnlySet<Category> enabled, CollectionScope scope,
        IProgress<string>? progress, CancellationToken cancellationToken)
    {
        var start = DateTimeOffset.UtcNow;
        var results = new List<CollectionResult>();
        foreach (var descriptor in CollectorCatalog.All)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!CollectorCatalog.Supports(descriptor.Category, scope) || !enabled.Contains(descriptor.Category))
            {
                results.Add(new(descriptor.Category, CollectionStatus.Disabled, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, [],
                    CollectorCatalog.Supports(descriptor.Category, scope) ? "Not selected for collection." : "Outside the selected collection scope."));
                continue;
            }
            progress?.Report("Checking " + descriptor.Name.ToLowerInvariant() + "...");
            results.Add(await CollectAsync(descriptor.Category, scope, cancellationToken));
        }
        cancellationToken.ThrowIfCancellationRequested();
        return new(Guid.NewGuid(), start, DateTimeOffset.UtcNow, results) { Scope = scope };
    }

    private async Task<CollectionResult> CollectAsync(Category category, CollectionScope scope, CancellationToken cancellationToken)
    {
        var started = DateTimeOffset.UtcNow;
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(TimeSpan.FromSeconds(25));
        using var process = new Process { StartInfo = WorkerStartInfo(category, scope) };
        try
        {
            if (!process.Start()) throw new InvalidOperationException("Collector could not start.");
            try { process.PriorityClass = ProcessPriorityClass.BelowNormal; } catch (System.ComponentModel.Win32Exception) { }
            var outputTask = CollectorTransport.ReadTextAsync(process.StandardOutput, 16_000_000, deadline.Token);
            var errorTask = CollectorTransport.ReadTextAsync(process.StandardError, 64_000, deadline.Token);
            await process.WaitForExitAsync(deadline.Token);
            var output = await outputTask;
            await errorTask;
            if (process.ExitCode != 0 || output.Length > 16_000_000) throw new InvalidOperationException("Collector output was not usable.");
            var result = JsonSerializer.Deserialize<CollectionResult>(output);
            if (result is null || result.Category != category) throw new InvalidOperationException("Collector output was not valid.");
            return result;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new(category, CollectionStatus.Failed, started, DateTimeOffset.UtcNow, [], "The check exceeded its 25-second limit. No removals were inferred.");
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            return new(category, CollectionStatus.Failed, started, DateTimeOffset.UtcNow, [], "This Windows source could not be checked. No settings were changed.");
        }
        finally
        {
            try { if (process.Id > 0 && !process.HasExited) { process.Kill(true); await process.WaitForExitAsync(CancellationToken.None); } }
            catch (InvalidOperationException) { }
            catch (System.ComponentModel.Win32Exception) { }
        }
    }

    /// <summary>Starts a collector worker directly (never through the shell), so it inherits this process's unelevated token.</summary>
    internal ProcessStartInfo WorkerStartInfo(Category category, CollectionScope scope)
    {
        var executable = Environment.ProcessPath ?? throw new InvalidOperationException("Application path unavailable.");
        var info = new ProcessStartInfo(executable)
        {
            UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true
        };
        if (Path.GetFileNameWithoutExtension(executable).Equals("dotnet", StringComparison.OrdinalIgnoreCase))
            info.ArgumentList.Add(typeof(App).Assembly.Location);
        info.ArgumentList.Add("--collect");
        info.ArgumentList.Add(category.ToString());
        info.ArgumentList.Add("--scope");
        info.ArgumentList.Add(scope.ToString());
        info.ArgumentList.Add("--data-dir");
        info.ArgumentList.Add(dataDirectory);
        return info;
    }
}