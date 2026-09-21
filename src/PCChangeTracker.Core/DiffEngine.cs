using System.Security.Cryptography;
using System.Text;

namespace PCChangeTracker.Core;

public sealed class DiffEngine
{
    public Comparison Compare(Snapshot before, Snapshot after)
    {
        if (before.Id == after.Id || before.FinishedAt > after.StartedAt)
            throw new ArgumentException("Choose an earlier and a later, non-overlapping snapshot.");

        var changes = new List<ObservedChange>();
        var gaps = new List<CoverageGap>();
        var unchanged = new List<Category>();
        var categories = before.Results.Select(result => result.Category)
            .Union(after.Results.Select(result => result.Category)).Order();

        if (before.Scope != after.Scope || before.Elevated != after.Elevated)
            return new(before, after, [], categories.Select(category => new CoverageGap(category,
                "Collection scope or administrator access differs. Use a baseline with the same scope and access.")).ToArray(), []);

        foreach (var category in categories)
        {
            var oldRuns = before.Results.Where(result => result.Category == category).ToArray();
            var newRuns = after.Results.Where(result => result.Category == category).ToArray();
            if (oldRuns.Length != 1 || newRuns.Length != 1)
            {
                gaps.Add(new(category, "A unique reading is not available at both endpoints."));
                continue;
            }
            var oldRun = oldRuns[0];
            var newRun = newRuns[0];
            if (oldRun.Status != CollectionStatus.Success || newRun.Status != CollectionStatus.Success)
            {
                gaps.Add(new(category, $"Not compared: earlier reading {oldRun.Status.ToString().ToLowerInvariant()}, later reading {newRun.Status.ToString().ToLowerInvariant()}."));
                continue;
            }
            if (oldRun.Version != newRun.Version || oldRun.FingerprintKeyId != newRun.FingerprintKeyId ||
                before.SchemaVersion != after.SchemaVersion)
            {
                gaps.Add(new(category, "Collector format or protected comparison key changed. Create a new baseline."));
                continue;
            }
            if (HasDuplicateKeys(oldRun.Items) || HasDuplicateKeys(newRun.Items))
            {
                gaps.Add(new(category, "Duplicate identities prevented a reliable comparison."));
                continue;
            }
            var previous = oldRun.Items.ToDictionary(item => item.Key, StringComparer.OrdinalIgnoreCase);
            var current = newRun.Items.ToDictionary(item => item.Key, StringComparer.OrdinalIgnoreCase);
            var countBefore = changes.Count;
            foreach (var key in previous.Keys.Union(current.Keys, StringComparer.OrdinalIgnoreCase).Order(StringComparer.OrdinalIgnoreCase))
            {
                previous.TryGetValue(key, out var oldItem);
                current.TryGetValue(key, out var newItem);
                if (oldItem is not null && newItem is not null && Equal(category, oldItem, newItem)) continue;
                if (category == Category.WindowsUpdates && newItem is null) continue;
                var kind = oldItem is null ? ChangeKind.Added : newItem is null ? ChangeKind.Removed : ChangeKind.Modified;
                var policy = Classify(category, kind, oldItem, newItem, before, after);
                var identity = $"{before.Id}|{after.Id}|{category}|{key.ToUpperInvariant()}|{kind}";
                changes.Add(new(Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity))),
                    category, key, (newItem ?? oldItem)!.Name, kind, oldItem, newItem,
                    policy.Importance, policy.Group, policy.Why, policy.Settings,
                    oldRun.StartedAt, newRun.FinishedAt));
            }
            if (countBefore == changes.Count) unchanged.Add(category);
        }
        return new(before, after, changes.OrderByDescending(change => change.Importance)
            .ThenBy(change => change.Category).ThenBy(change => change.Name, StringComparer.OrdinalIgnoreCase).ToArray(), gaps, unchanged);
    }

    private static bool HasDuplicateKeys(IReadOnlyList<ConfigurationItem> items) =>
        items.Select(item => item.Key).Distinct(StringComparer.OrdinalIgnoreCase).Count() != items.Count;

    private static bool Equal(Category category, ConfigurationItem before, ConfigurationItem after) =>
        before.Name == after.Name && before.Fingerprint == after.Fingerprint &&
        before.Fields.Count == after.Fields.Count && before.Fields.All(field =>
            after.Fields.TryGetValue(field.Key, out var value) &&
            (category == Category.Environment && field.Key == "Entries"
                ? PathComparison.Equal(field.Value, value) : field.Value == value));

    private static Policy Classify(Category category, ChangeKind kind, ConfigurationItem? before,
        ConfigurationItem? after, Snapshot previousSnapshot, Snapshot currentSnapshot)
    {
        var settings = category switch
        {
            Category.Applications => SettingsPage.Apps, Category.Startup => SettingsPage.Startup,
            Category.Services => SettingsPage.Services, Category.ScheduledTasks => SettingsPage.Tasks,
            Category.WindowsUpdates => SettingsPage.Updates, Category.Drivers => SettingsPage.Devices,
            Category.DefaultApps => SettingsPage.Defaults, Category.Audio => SettingsPage.Sound,
            Category.Network => SettingsPage.Network, Category.Protection => SettingsPage.Security,
            Category.Environment => SettingsPage.Environment, _ => SettingsPage.None
        };
        if (category == Category.Protection)
        {
            var reduced = before?.Fields.GetValueOrDefault("Enabled") == "Yes" && after?.Fields.GetValueOrDefault("Enabled") == "No";
            return new(reduced ? Importance.Important : Importance.Review, TriageGroup.ReviewFirst,
                reduced ? "This protection setting changed from on to off. Review it in Windows Security; intent and cause are not known."
                : "A protection setting changed. Check its current state in Windows Security.", settings);
        }
        if (category is Category.Startup or Category.Services or Category.ScheduledTasks)
        {
            var automatic = category == Category.Startup || after?.Fields.GetValueOrDefault("Automatic") == "Yes";
            return new(automatic && kind != ChangeKind.Removed ? Importance.Important : Importance.Review,
                TriageGroup.ReviewFirst, kind == ChangeKind.Removed
                    ? "A background launch registration was removed. Features provided by it may behave differently."
                    : "A background launch configuration was added or changed. Registration does not prove execution. Launch arguments are not stored.", settings);
        }
        if (category == Category.Applications && kind == ChangeKind.Modified && before is not null && after is not null)
        {
            var versionOnly = before.Name == after.Name && before.Fields.Count == after.Fields.Count &&
                !string.IsNullOrWhiteSpace(before.Fields.GetValueOrDefault("Version")) &&
                !string.IsNullOrWhiteSpace(after.Fields.GetValueOrDefault("Version")) &&
                before.Fields.Where(field => field.Key != "Version").All(field => after.Fields.GetValueOrDefault(field.Key) == field.Value);
            var routineCoverage = new[] { Category.Startup, Category.Services, Category.ScheduledTasks, Category.Protection }
                .All(required => CompatibleCoverage(previousSnapshot, currentSnapshot, required));
            if (versionOnly && routineCoverage)
                return new(Importance.Info, TriageGroup.Routine,
                    "Only this registered app's version changed. Review findings elsewhere remain separate; this is not a safety verdict.", settings);
            if (versionOnly || string.IsNullOrWhiteSpace(before.Fields.GetValueOrDefault("Version")) || string.IsNullOrWhiteSpace(after.Fields.GetValueOrDefault("Version")))
                return new(Importance.Info, TriageGroup.Other,
                    "App metadata changed, but version information or related comparison coverage is insufficient to call it routine. Impact has not been assessed.", settings);
        }
        if (category == Category.WindowsUpdates && after?.Fields.GetValueOrDefault("Operation") == "Uninstalled")
            return new(Importance.Review, TriageGroup.ReviewFirst,
                "Windows reported an update uninstallation. Review update history; the reason and effect are not known.", settings);
        var explanation = category switch
        {
            Category.Applications => "An installed-app registration changed. This does not establish which installer made the change.",
            Category.DefaultApps => "Links or files may now open in a different application.",
            Category.Audio => "Applications using the Windows default sound device may use a different speaker or microphone.",
            Category.Network => "This network configuration can affect how the PC connects. Dynamic network changes may be expected.",
            Category.Drivers => "A device driver changed. Device behavior may differ; a driver date is not an installation time.",
            Category.Environment => PathComparison.Describe(before?.Fields.GetValueOrDefault("Entries") ?? "", after?.Fields.GetValueOrDefault("Entries") ?? ""),
            Category.WindowsUpdates => "Windows reported this update-history entry. Related changes do not prove it caused a problem.",
            _ => "A difference was observed; its impact has not been assessed."
        };
        return new(category == Category.WindowsUpdates ? Importance.Info : Importance.Review,
            category == Category.WindowsUpdates ? TriageGroup.Routine : TriageGroup.ReviewFirst, explanation, settings);
    }

    private static bool CompatibleCoverage(Snapshot before, Snapshot after, Category category)
    {
        var oldRuns = before.Results.Where(result => result.Category == category).ToArray();
        var newRuns = after.Results.Where(result => result.Category == category).ToArray();
        return oldRuns.Length == 1 && newRuns.Length == 1 &&
            oldRuns[0].Status == CollectionStatus.Success && newRuns[0].Status == CollectionStatus.Success &&
            oldRuns[0].Version == newRuns[0].Version && oldRuns[0].FingerprintKeyId == newRuns[0].FingerprintKeyId &&
            before.SchemaVersion == after.SchemaVersion && !HasDuplicateKeys(oldRuns[0].Items) && !HasDuplicateKeys(newRuns[0].Items);
    }

    private sealed record Policy(Importance Importance, TriageGroup Group, string Why, SettingsPage Settings);
}