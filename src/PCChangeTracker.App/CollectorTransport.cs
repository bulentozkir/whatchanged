using System.Buffers.Binary;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using Microsoft.Win32.SafeHandles;
using PCChangeTracker.Core;
using PCChangeTracker.Windows;

namespace PCChangeTracker.App;

public sealed class CaptureAccessException(string message) : Exception(message);

internal sealed record MachineCaptureRequest(Category[] Categories, byte[] ComparisonKey);

internal static class CollectorTransport
{
    private const string PipePrefix = "PCChangeTracker.capture.";
    private const int MaximumMessageBytes = 16_000_000;

    internal static bool IsAdministrator
    {
        get
        {
            using var identity = WindowsIdentity.GetCurrent();
            return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    internal static async Task<Snapshot> CaptureElevatedAsync(string dataDirectory, IReadOnlySet<Category> enabled, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var executable = Environment.ProcessPath ?? throw new CaptureAccessException("Application path unavailable.");
        if (Path.GetFileNameWithoutExtension(executable).Equals("dotnet", StringComparison.OrdinalIgnoreCase))
            throw new CaptureAccessException("Open the installed application to request administrator access.");
        var pipeName = PipePrefix + Guid.NewGuid().ToString("N");
        using var pipe = CreateServer(pipeName);
        Process? helper = null;
        byte[]? material = null;
        var launch = Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Process.Start(ElevationStartInfo(executable, pipeName, Environment.ProcessId));
        }, CancellationToken.None);
        try
        {
            helper = await launch.WaitAsync(cancellationToken) ?? throw new CaptureAccessException("The administrator check could not start.");
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            deadline.CancelAfter(TimeSpan.FromMinutes(5));
            using (var connectionDeadline = CancellationTokenSource.CreateLinkedTokenSource(deadline.Token))
            {
                connectionDeadline.CancelAfter(TimeSpan.FromSeconds(15));
                await pipe.WaitForConnectionAsync(connectionDeadline.Token);
            }
            if (!GetNamedPipeClientProcessId(pipe.SafePipeHandle, out var clientId) || clientId != helper.Id)
                throw new CaptureAccessException("The administrator helper identity could not be verified.");
            using var key = new ComparisonKey(dataDirectory);
            material = key.Export();
            var categories = enabled.Where(category => CollectorCatalog.Supports(category, CollectionScope.Machine)).Order().ToArray();
            await WriteAsync(pipe, new MachineCaptureRequest(categories, material), deadline.Token);
            var snapshot = await ReadAsync<Snapshot>(pipe, MaximumMessageBytes, deadline.Token);
            if (snapshot.Scope != CollectionScope.Machine || !snapshot.Elevated || snapshot.Results is null ||
                snapshot.Results.Count != Enum.GetValues<Category>().Length ||
                snapshot.Results.Select(result => result.Category).Distinct().Count() != snapshot.Results.Count ||
                snapshot.Results.Any(result => !Enum.IsDefined(result.Category) ||
                    (!categories.Contains(result.Category) && result.Status != CollectionStatus.Disabled)))
                throw new CaptureAccessException("The administrator helper returned an invalid result. Nothing was saved.");
            await helper.WaitForExitAsync(deadline.Token);
            if (helper.ExitCode != 0) throw new CaptureAccessException("The administrator check did not finish. Nothing was saved.");
            return snapshot;
        }
        catch (Win32Exception exception) when (exception.NativeErrorCode == 1223)
        {
            throw new CaptureAccessException("Administrator access was not granted. No elevated check ran. You can still use Check now with standard access.");
        }
        catch (Win32Exception)
        {
            throw new CaptureAccessException("Windows could not start the administrator helper. Device or package policy may restrict elevation. Check now still uses standard access.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new CaptureAccessException("The administrator check timed out. Existing history was preserved.");
        }
        finally
        {
            pipe.Dispose();
            if (material is not null) CryptographicOperations.ZeroMemory(material);
            if (helper is not null)
            {
                try { await helper.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30)); }
                catch (Exception exception) when (exception is TimeoutException or Win32Exception or InvalidOperationException) { }
                helper.Dispose();
            }
            else _ = DisposeLaunchAsync(launch);
        }
    }

    private static async Task DisposeLaunchAsync(Task<Process?> launch)
    {
        try { (await launch)?.Dispose(); }
        catch (Exception exception) when (exception is Win32Exception or InvalidOperationException or OperationCanceledException) { }
    }

    internal static ProcessStartInfo ElevationStartInfo(string executable, string pipeName, int parentId)
    {
        var info = new ProcessStartInfo(executable) { UseShellExecute = true, Verb = "runas", WorkingDirectory = AppContext.BaseDirectory };
        info.ArgumentList.Add("--machine-check");
        info.ArgumentList.Add(pipeName);
        info.ArgumentList.Add(parentId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        return info;
    }

    internal static NamedPipeServerStream CreateServer(string pipeName)
    {
        using var identity = WindowsIdentity.GetCurrent();
        var security = new PipeSecurity();
        security.SetAccessRuleProtection(true, false);
        security.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkSid, null), PipeAccessRights.FullControl, AccessControlType.Deny));
        security.AddAccessRule(new PipeAccessRule(identity.User!, PipeAccessRights.FullControl, AccessControlType.Allow));
        security.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
            PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, AccessControlType.Allow));
        return NamedPipeServerStreamAcl.Create(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous | PipeOptions.FirstPipeInstance, 4096, 4096, security);
    }

    internal static async Task<int> RunHelperAsync(string pipeName, int parentId)
    {
        if (!IsAdministrator || !pipeName.StartsWith(PipePrefix, StringComparison.Ordinal) ||
            !Guid.TryParseExact(pipeName[PipePrefix.Length..], "N", out _) || parentId <= 0) return 2;
        MachineCaptureRequest? request = null;
        using var lifetime = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        try
        {
            using var parent = Process.GetProcessById(parentId);
            if (parent.HasExited || !string.Equals(parent.MainModule?.FileName, Environment.ProcessPath, StringComparison.OrdinalIgnoreCase)) return 2;
            using var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Anonymous);
            await pipe.ConnectAsync(10000, lifetime.Token);
            if (!GetNamedPipeServerProcessId(pipe.SafePipeHandle, out var serverId) || serverId != parentId) return 2;
            request = await ReadAsync<MachineCaptureRequest>(pipe, 4096, lifetime.Token);
            ValidateMachineRequest(request);
            var disconnected = pipe.ReadAsync(new byte[1], lifetime.Token).AsTask();
            var parentExited = parent.WaitForExitAsync(lifetime.Token);
            var capture = new CaptureService("").CaptureLocalAsync(request.Categories.ToHashSet(), CollectionScope.Machine,
                request.ComparisonKey, null, lifetime.Token);
            if (await Task.WhenAny(capture, disconnected, parentExited) != capture) lifetime.Cancel();
            var snapshot = await capture;
            lifetime.Token.ThrowIfCancellationRequested();
            await WriteAsync(pipe, snapshot, lifetime.Token);
            return 0;
        }
        catch (Exception exception) when (exception is not OutOfMemoryException) { return 1; }
        finally
        {
            lifetime.Cancel();
            if (request?.ComparisonKey is not null) CryptographicOperations.ZeroMemory(request.ComparisonKey);
        }
    }

    internal static void ValidateMachineRequest(MachineCaptureRequest request)
    {
        if (request.ComparisonKey is not { Length: 32 } || request.Categories is not { Length: > 0 and <= 11 } ||
            request.Categories.Distinct().Count() != request.Categories.Length ||
            request.Categories.Any(category => !CollectorCatalog.Supports(category, CollectionScope.Machine)))
            throw new InvalidDataException("Invalid machine collection request.");
    }

    internal static async Task WriteAsync<T>(Stream stream, T value, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(value);
        try
        {
            if (payload.Length > MaximumMessageBytes) throw new InvalidDataException("Collector response exceeded its size limit.");
            var header = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(header, payload.Length);
            await stream.WriteAsync(header, cancellationToken);
            await stream.WriteAsync(payload, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
        finally { CryptographicOperations.ZeroMemory(payload); }
    }

    internal static async Task<T> ReadAsync<T>(Stream stream, int limit, CancellationToken cancellationToken)
    {
        var header = new byte[4];
        await stream.ReadExactlyAsync(header, cancellationToken);
        var length = BinaryPrimitives.ReadInt32LittleEndian(header);
        if (length <= 0 || length > limit) throw new InvalidDataException("Collector message exceeded its size limit.");
        var payload = new byte[length];
        try
        {
            await stream.ReadExactlyAsync(payload, cancellationToken);
            return JsonSerializer.Deserialize<T>(payload) ?? throw new InvalidDataException("Collector message was empty.");
        }
        finally { CryptographicOperations.ZeroMemory(payload); }
    }

    internal static async Task<string> ReadTextAsync(StreamReader reader, int limit, CancellationToken cancellationToken)
    {
        var result = new StringBuilder();
        var buffer = new char[8192];
        int count;
        while ((count = await reader.ReadAsync(buffer, cancellationToken)) > 0)
        {
            if (result.Length + count > limit) throw new InvalidDataException("Collector output exceeded its size limit.");
            result.Append(buffer, 0, count);
        }
        return result.ToString();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetNamedPipeClientProcessId(SafePipeHandle pipe, out uint processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetNamedPipeServerProcessId(SafePipeHandle pipe, out uint processId);
}