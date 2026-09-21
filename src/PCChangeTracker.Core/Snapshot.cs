namespace PCChangeTracker.Core;

public enum Category
{
    Applications, Startup, Services, ScheduledTasks, WindowsUpdates, Drivers,
    DefaultApps, Audio, Network, Protection, Environment
}

public enum CollectionStatus { Success, Partial, Failed, Disabled }
public enum CollectionScope { Legacy, CurrentUser, Machine, Both }
public enum Importance { Info, Review, Important }
public enum TriageGroup { ReviewFirst, Routine, Other }
public enum ChangeKind { Added, Removed, Modified }
public enum SettingsPage { None, Apps, Startup, Services, Tasks, Updates, Devices, Defaults, Sound, Network, Security, Environment }

public static class CollectionScopes
{
    public static string Display(this CollectionScope scope) => scope switch
    {
        CollectionScope.CurrentUser => "Current user",
        CollectionScope.Machine => "Machine-wide",
        CollectionScope.Both => "Current user + Machine-wide",
        _ => "Legacy mixed scope"
    };

    public static string Context(CollectionScope scope, bool elevated) => scope == CollectionScope.Legacy
        ? scope.Display() : $"{scope.Display()} / {(elevated ? scope == CollectionScope.Both ? "Machine administrator access; user standard access" : "Administrator access" : "Standard access")}";
}

public sealed record ConfigurationItem(string Key, string Name, Dictionary<string, string> Fields)
{
    public string? Fingerprint { get; init; }
}

public sealed record CollectionResult(
    Category Category,
    CollectionStatus Status,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    IReadOnlyList<ConfigurationItem> Items,
    string? Diagnostic = null)
{
    public int Version { get; init; } = 1;
    public string? FingerprintKeyId { get; init; }
}

public sealed record Snapshot(
    Guid Id,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    IReadOnlyList<CollectionResult> Results)
{
    public int SchemaVersion { get; init; } = 1;
    public CollectionScope Scope { get; init; } = CollectionScope.Legacy;
    public bool Elevated { get; init; }
}

public sealed record CoverageGap(Category Category, string Reason);

public sealed record ObservedChange(
    string Id, Category Category, string Key, string Name, ChangeKind Kind,
    ConfigurationItem? Before, ConfigurationItem? After,
    Importance Importance, TriageGroup Group, string Why, SettingsPage Settings,
    DateTimeOffset ObservedFrom, DateTimeOffset ObservedTo)
{
    public string Uncertainty => "The cause and exact change time are not known.";
}

public sealed record Comparison(
    Snapshot Before, Snapshot After,
    IReadOnlyList<ObservedChange> Changes,
    IReadOnlyList<CoverageGap> Gaps,
    IReadOnlyList<Category> Unchanged);

public interface ICollector
{
    Category Category { get; }
    CollectionResult Collect(CancellationToken cancellationToken);
}

public static class CategoryNames
{
    public static string Display(this Category category) => category switch
    {
        Category.Applications => "Installed apps",
        Category.Startup => "Startup entries",
        Category.ScheduledTasks => "Scheduled tasks",
        Category.WindowsUpdates => "Windows updates",
        Category.DefaultApps => "Default apps",
        Category.Protection => "Protection settings",
        Category.Environment => "PATH",
        _ => category.ToString()
    };

    public static string Question(this Category category) => category switch
    {
        Category.Startup or Category.Services or Category.ScheduledTasks => "What starts automatically?",
        Category.Protection => "Was protection reduced?",
        Category.Network => "Why is connectivity different?",
        Category.DefaultApps or Category.Audio => "Why do everyday actions behave differently?",
        Category.Environment => "Did my developer setup change?",
        _ => "What software changed?"
    };
}