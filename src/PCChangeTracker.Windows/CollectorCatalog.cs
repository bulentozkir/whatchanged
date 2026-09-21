using PCChangeTracker.Core;

namespace PCChangeTracker.Windows;

public sealed record CollectorDescriptor(Category Category, string Scope, bool Recommended = true)
{
    public string Name => Category.Display();
}

public static class CollectorCatalog
{
    public static IReadOnlyList<CollectorDescriptor> All { get; } =
    [
        new(Category.Applications, "Registered desktop apps, user/machine and 32/64-bit views. Store packages and portable apps are not included."),
        new(Category.Startup, "User/machine Run and RunOnce registrations. No commands or arguments stored; enabled status is not inferred."),
        new(Category.Services, "Windows service registration and startup configuration. Driver services are excluded; no runtime polling."),
        new(Category.ScheduledTasks, "Accessible tasks, enabled state, trigger types, and privately compared action definitions."),
        new(Category.WindowsUpdates, "Local successful Windows Update history only. No online scan or update installation."),
        new(Category.Drivers, "Locally exposed signed-device driver identity, provider, and version. No firmware probing."),
        new(Category.DefaultApps, "Effective HTTP, HTTPS, PDF, image, media, archive, and CSV handlers. Nothing is changed."),
        new(Category.Audio, "Default playback, recording, and communications endpoints. No recording or microphone access."),
        new(Category.Protection, "Windows Firewall profile configuration only. No antivirus assessment or safety verdict."),
        new(Category.Network, "Adapter DNS/DHCP and current-user proxy configuration. No packets, Wi-Fi passwords, or network probes.", false),
        new(Category.Environment, "Persisted user and machine PATH only. No general environment variables or executable scanning.", false)
    ];

    public static bool Supports(Category category, CollectionScope scope) => scope switch
    {
        CollectionScope.Both => Supports(category, CollectionScope.CurrentUser) || Supports(category, CollectionScope.Machine),
        CollectionScope.CurrentUser => category is Category.Applications or Category.Startup or Category.DefaultApps
            or Category.Audio or Category.Network or Category.Environment,
        CollectionScope.Machine => category is Category.Applications or Category.Startup or Category.Services
            or Category.ScheduledTasks or Category.WindowsUpdates or Category.Drivers or Category.Protection
            or Category.Network or Category.Environment,
        _ => false
    };

    public static string Describe(Category category, CollectionScope scope)
    {
        if (!Supports(category, scope)) return "Not collected in this scope.";
        if (scope == CollectionScope.Both) return All.Single(descriptor => descriptor.Category == category).Scope;
        return category switch
        {
            Category.Applications => scope == CollectionScope.CurrentUser
                ? "Current-user desktop app registrations only. Machine registrations, Store packages, and portable apps are excluded."
                : "Machine desktop app registrations, both 32-bit and 64-bit views. User profiles, Store packages, and portable apps are excluded.",
            Category.Startup => scope == CollectionScope.CurrentUser
                ? "Current-user Run and RunOnce registrations only. Launch values are privately compared."
                : "Machine Run and RunOnce registrations only. Launch values are privately compared.",
            Category.Network => scope == CollectionScope.CurrentUser
                ? "Current-user proxy configuration only. No adapter settings or traffic."
                : "Shared adapter DNS/DHCP settings only. No user proxy, packets, or network probes.",
            Category.Environment => scope == CollectionScope.CurrentUser
                ? "Persisted current-user PATH only. No machine PATH or other environment variables."
                : "Persisted machine PATH only. No user PATH or other environment variables.",
            _ => All.Single(descriptor => descriptor.Category == category).Scope
        };
    }

    public static ICollector Create(Category category, string dataDirectory,
        CollectionScope scope = CollectionScope.CurrentUser, byte[]? comparisonKey = null)
    {
        if (scope == CollectionScope.Both || !Supports(category, scope)) throw new ArgumentException("Create collectors for one concrete user or machine scope at a time.");
        return category switch
        {
            Category.Applications or Category.Startup or Category.Services or Category.Environment => new RegistryCollector(category, dataDirectory, scope, comparisonKey),
            Category.DefaultApps => new DefaultAppsCollector(),
            Category.Audio => new AudioCollector(),
            Category.Network => new NetworkCollector(dataDirectory, scope, comparisonKey),
            Category.Protection => new ProtectionCollector(),
            Category.Drivers => new DriverCollector(),
            Category.ScheduledTasks => new TasksCollector(dataDirectory, comparisonKey),
            Category.WindowsUpdates => new UpdatesCollector(),
            _ => throw new ArgumentOutOfRangeException(nameof(category))
        };
    }
}

public abstract class CollectorBase(Category category) : ICollector
{
    public Category Category { get; } = category;
    protected List<ConfigurationItem> Items { get; } = [];
    protected int Errors { get; private set; }
    protected string? KeyId { get; set; }

    public CollectionResult Collect(CancellationToken cancellationToken)
    {
        var start = DateTimeOffset.UtcNow;
        Items.Clear();
        Errors = 0;
        try
        {
            Read(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (Items.Select(item => item.Key).Distinct(StringComparer.OrdinalIgnoreCase).Count() != Items.Count) MarkPartial();
            return new(Category, Errors == 0 ? CollectionStatus.Success : CollectionStatus.Partial,
                start, DateTimeOffset.UtcNow, Items.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase).ToArray(),
                Errors == 0 ? null : "Some entries could not be read. This category will not be compared until both readings are complete.")
            { FingerprintKeyId = KeyId };
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            return new(Category, CollectionStatus.Failed, start, DateTimeOffset.UtcNow, [],
                "Windows did not provide a complete readable result. No settings were changed.");
        }
    }

    protected abstract void Read(CancellationToken cancellationToken);
    protected void MarkPartial() => Errors++;
    protected void Guard(Action action)
    {
        try { action(); }
        catch (OperationCanceledException) { throw; }
        catch (Exception exception) when (exception is not OutOfMemoryException) { MarkPartial(); }
    }
}