using PCChangeTracker.Core;
using Xunit;

namespace PCChangeTracker.Tests;

public sealed class DiffEngineTests
{
    private static readonly DateTimeOffset Epoch = new(2026, 9, 18, 8, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(CollectionStatus.Failed)]
    [InlineData(CollectionStatus.Partial)]
    [InlineData(CollectionStatus.Disabled)]
    public void UnavailableSourceDoesNotInventRemovals(CollectionStatus status)
    {
        var before = Capture(0, Category.Startup, CollectionStatus.Success, Item("one", "Acme"));
        var after = Capture(1, Category.Startup, status);
        var result = new DiffEngine().Compare(before, after);
        Assert.Empty(result.Changes);
        Assert.Empty(result.Unchanged);
        Assert.Single(result.Gaps);
    }

    [Fact]
    public void EqualRecordsIgnoreDictionaryEnumerationOrder()
    {
        var before = Capture(0, Category.Applications, CollectionStatus.Success,
            new ConfigurationItem("app", "Acme", new() { ["Version"] = "1", ["Publisher"] = "Example" }));
        var after = Capture(1, Category.Applications, CollectionStatus.Success,
            new ConfigurationItem("APP", "Acme", new() { ["Publisher"] = "Example", ["Version"] = "1" }));
        Assert.Empty(new DiffEngine().Compare(before, after).Changes);
    }

    [Fact]
    public void StartupAdditionHasReasonAndHonestTimeWindow()
    {
        var before = Capture(0, Category.Startup, CollectionStatus.Success);
        var after = Capture(1, Category.Startup, CollectionStatus.Success, Item("new", "Acme"));
        var change = Assert.Single(new DiffEngine().Compare(before, after).Changes);
        Assert.Equal(Importance.Important, change.Importance);
        Assert.Equal(TriageGroup.ReviewFirst, change.Group);
        Assert.Contains("does not prove execution", change.Why);
        Assert.Equal(Epoch, change.ObservedFrom);
        Assert.Equal(Epoch.AddDays(1).AddSeconds(1), change.ObservedTo);
    }

    [Fact]
    public void ChangedFingerprintKeyCannotGenerateFalseChanges()
    {
        var before = Capture(0, Category.Startup, CollectionStatus.Success, Item("same", "Acme"));
        var after = Capture(1, Category.Startup, CollectionStatus.Success, Item("same", "Acme"));
        after = after with { Results = [after.Results[0] with { FingerprintKeyId = "different" }] };
        var result = new DiffEngine().Compare(before, after);
        Assert.Empty(result.Changes);
        Assert.Single(result.Gaps);
    }

    [Theory]
    [InlineData(CollectionScope.Legacy, false, CollectionScope.CurrentUser, false)]
    [InlineData(CollectionScope.CurrentUser, false, CollectionScope.Machine, false)]
    [InlineData(CollectionScope.Machine, false, CollectionScope.Machine, true)]
    public void ScopeOrAccessChangesCannotInventRemovals(CollectionScope beforeScope, bool beforeElevated,
        CollectionScope afterScope, bool afterElevated)
    {
        var before = Capture(0, Category.Startup, CollectionStatus.Success, Item("one", "Acme"))
            with { Scope = beforeScope, Elevated = beforeElevated };
        var after = Capture(1, Category.Startup, CollectionStatus.Success)
            with { Scope = afterScope, Elevated = afterElevated };
        var result = new DiffEngine().Compare(before, after);
        Assert.Empty(result.Changes);
        Assert.Empty(result.Unchanged);
        Assert.Contains("scope or administrator access differs", Assert.Single(result.Gaps).Reason);
    }

    [Fact]
    public void HiddenConfigurationChangeStillProducesDifference()
    {
        var before = Capture(0, Category.Network, CollectionStatus.Success,
            new ConfigurationItem("proxy", "Proxy", new() { ["Automatic configuration"] = "https://example.invalid/[path omitted]" }) { Fingerprint = "before" });
        var after = Capture(1, Category.Network, CollectionStatus.Success,
            new ConfigurationItem("proxy", "Proxy", new() { ["Automatic configuration"] = "https://example.invalid/[path omitted]" }) { Fingerprint = "after" });
        Assert.Equal(ChangeKind.Modified, Assert.Single(new DiffEngine().Compare(before, after).Changes).Kind);
    }

    [Fact]
    public void DuplicateIdentityIsACoverageGap()
    {
        var before = Capture(0, Category.Applications, CollectionStatus.Success, Item("same", "Acme"), Item("SAME", "Acme"));
        var after = Capture(1, Category.Applications, CollectionStatus.Success);
        var result = new DiffEngine().Compare(before, after);
        Assert.Empty(result.Changes);
        Assert.Single(result.Gaps);
    }

    [Fact]
    public void FirewallReductionIsImportant()
    {
        var before = Capture(0, Category.Protection, CollectionStatus.Success, new ConfigurationItem("private", "Private firewall", new() { ["Enabled"] = "Yes" }));
        var after = Capture(1, Category.Protection, CollectionStatus.Success, new ConfigurationItem("private", "Private firewall", new() { ["Enabled"] = "No" }));
        Assert.Equal(Importance.Important, Assert.Single(new DiffEngine().Compare(before, after).Changes).Importance);
    }

    [Fact]
    public void HistoryPruningIsNotAnUpdateUninstall()
    {
        var before = Capture(0, Category.WindowsUpdates, CollectionStatus.Success, Item("event", "KB example"));
        var after = Capture(1, Category.WindowsUpdates, CollectionStatus.Success);
        Assert.Empty(new DiffEngine().Compare(before, after).Changes);
    }

    [Fact]
    public void RoutineVersionChangeDoesNotHideStartupFinding()
    {
        var before = WithRelatedCoverage(Capture(0, Category.Applications, CollectionStatus.Success,
            new ConfigurationItem("app", "Acme", new() { ["Version"] = "1" })));
        var after = WithRelatedCoverage(Capture(1, Category.Applications, CollectionStatus.Success,
            new ConfigurationItem("app", "Acme", new() { ["Version"] = "2" })));
        after = after with { Results = after.Results.Select(result => result.Category == Category.Startup
            ? result with { Items = [Item("new", "New launch")] } : result).ToArray() };
        var result = new DiffEngine().Compare(before, after);
        Assert.Equal(TriageGroup.Routine, Assert.Single(result.Changes, change => change.Category == Category.Applications).Group);
        Assert.Equal(TriageGroup.ReviewFirst, Assert.Single(result.Changes, change => change.Category == Category.Startup).Group);
    }

    [Fact]
    public void RelatedKeyMismatchDisqualifiesRoutineJudgment()
    {
        var before = WithRelatedCoverage(Capture(0, Category.Applications, CollectionStatus.Success,
            new ConfigurationItem("app", "Acme", new() { ["Version"] = "1" })));
        var after = WithRelatedCoverage(Capture(1, Category.Applications, CollectionStatus.Success,
            new ConfigurationItem("app", "Acme", new() { ["Version"] = "2" })));
        after = after with { Results = after.Results.Select(result => result.Category == Category.Startup
            ? result with { FingerprintKeyId = "different" } : result).ToArray() };
        var result = new DiffEngine().Compare(before, after);
        Assert.Equal(TriageGroup.Other, Assert.Single(result.Changes).Group);
        Assert.Single(result.Gaps);
    }

    [Fact]
    public void UpdateUninstallationIsNotAutomaticallyRoutine()
    {
        var before = Capture(0, Category.WindowsUpdates, CollectionStatus.Success);
        var after = Capture(1, Category.WindowsUpdates, CollectionStatus.Success,
            new ConfigurationItem("event", "KB example", new() { ["Operation"] = "Uninstalled" }));
        Assert.Equal(TriageGroup.ReviewFirst, Assert.Single(new DiffEngine().Compare(before, after).Changes).Group);
    }

    private static Snapshot WithRelatedCoverage(Snapshot snapshot) => snapshot with
    {
        Results = snapshot.Results.Concat(new[] { Category.Startup, Category.Services, Category.ScheduledTasks, Category.Protection }
            .Select(category => new CollectionResult(category, CollectionStatus.Success, snapshot.StartedAt, snapshot.FinishedAt, []))).ToArray()
    };

    [Fact]
    public void InvalidPairIsRejected()
    {
        var snapshot = Capture(0, Category.Applications, CollectionStatus.Success);
        Assert.Throws<ArgumentException>(() => new DiffEngine().Compare(snapshot, snapshot));
    }

    [Fact]
    public void PathOrderAndDuplicatesAreMeaningful()
    {
        Assert.False(PathComparison.Equal("C:\\One;C:\\Two", "C:\\Two;C:\\One"));
        Assert.False(PathComparison.Equal("C:\\One;C:\\One", "C:\\One"));
        Assert.True(PathComparison.Equal("C:\\One;C:\\Two", "c:\\one;c:\\two"));
        Assert.Contains("reordered", PathComparison.Describe("C:\\One;C:\\Two", "C:\\Two;C:\\One"));
        Assert.Contains("1 removed", PathComparison.Describe("C:\\One;C:\\One", "C:\\One"));
    }

    private static ConfigurationItem Item(string key, string name) => new(key, name, new());

    internal static Snapshot Capture(int day, Category category, CollectionStatus status, params ConfigurationItem[] items) =>
        new(Guid.NewGuid(), Epoch.AddDays(day), Epoch.AddDays(day).AddSeconds(1),
            [new(category, status, Epoch.AddDays(day), Epoch.AddDays(day).AddSeconds(1), items)]);
}