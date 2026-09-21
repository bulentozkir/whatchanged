using PCChangeTracker.Core;
using Xunit;

namespace PCChangeTracker.Tests;

public sealed class HistoryAndReportTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void HistoryReopensAndManualChecksNeverMoveBaseline()
    {
        var path = Path.Combine(directory, "history.db");
        var store = new HistoryStore(path);
        var first = DiffEngineTests.Capture(0, Category.Applications, CollectionStatus.Success);
        var second = DiffEngineTests.Capture(1, Category.Applications, CollectionStatus.Success);
        store.Save(first);
        store.Save(second);
        store.Rename(first.Id, "Before setup");
        store.SetPreference("mode", "advanced");
        var reopened = new HistoryStore(path);
        Assert.Equal(first.Id, reopened.BaselineId);
        Assert.Equal(2, reopened.List().Count);
        Assert.Equal("Before setup", reopened.List()[1].Label);
        Assert.Equal("advanced", reopened.GetPreference("mode"));
        Assert.Equal(second.Id, reopened.Load(second.Id)!.Id);
        Assert.Throws<InvalidOperationException>(() => reopened.Delete(first.Id));
    }

    [Fact]
    public void FailedCaptureCannotBecomeBaseline()
    {
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        Assert.Throws<InvalidOperationException>(() => store.Save(DiffEngineTests.Capture(0, Category.Applications, CollectionStatus.Failed)));
        Assert.Empty(store.List());
        Assert.Null(store.BaselineId);
    }

    [Fact]
    public void BaselinesRemainSeparateForScopeAndAccessAndSurviveReopen()
    {
        var path = Path.Combine(directory, "history.db");
        var store = new HistoryStore(path);
        var legacy = DiffEngineTests.Capture(0, Category.Applications, CollectionStatus.Success);
        var user = legacy with { Id = Guid.NewGuid(), Scope = CollectionScope.CurrentUser };
        var machine = legacy with { Id = Guid.NewGuid(), Scope = CollectionScope.Machine };
        var administrator = machine with { Id = Guid.NewGuid(), Elevated = true };
        foreach (var snapshot in new[] { legacy, user, machine, administrator }) store.Save(snapshot);
        var laterUser = user with { Id = Guid.NewGuid() };
        store.Save(laterUser);
        var reopened = new HistoryStore(path);
        Assert.Equal(legacy.Id, reopened.BaselineId);
        Assert.Equal(user.Id, reopened.GetBaselineId(CollectionScope.CurrentUser));
        Assert.Equal(machine.Id, reopened.GetBaselineId(CollectionScope.Machine));
        Assert.Equal(administrator.Id, reopened.GetBaselineId(CollectionScope.Machine, true));
        Assert.True(reopened.Load(administrator.Id)!.Elevated);
        Assert.Equal(CollectionScope.CurrentUser, reopened.List().Single(snapshot => snapshot.Id == user.Id).Scope);
        Assert.Throws<InvalidOperationException>(() => reopened.Delete(machine.Id));
        reopened.SetBaseline(laterUser.Id);
        reopened.Delete(user.Id);
        Assert.Equal(laterUser.Id, reopened.GetBaselineId(CollectionScope.CurrentUser));
        reopened.SetPreference("collection.scope", "CurrentUser");
        reopened.Clear();
        Assert.Null(reopened.BaselineId);
        Assert.Null(reopened.GetBaselineId(CollectionScope.Machine, true));
        Assert.Null(reopened.GetBaselineId(CollectionScope.CurrentUser));
        Assert.Equal("CurrentUser", reopened.GetPreference("collection.scope"));
    }

    [Fact]
    public void ExpectedAnnotationIsReversibleAndOccurrenceScoped()
    {
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        store.SetExpected("occurrence-a", true);
        Assert.Contains("occurrence-a", store.ExpectedChanges());
        Assert.DoesNotContain("occurrence-b", store.ExpectedChanges());
        store.SetExpected("occurrence-a", false);
        Assert.Empty(store.ExpectedChanges());
    }

    [Fact]
    public void OlderHistoryUpgradesWithoutRelabelingMixedSnapshots()
    {
        var path = Path.Combine(directory, "history.db");
        var store = new HistoryStore(path);
        var snapshot = DiffEngineTests.Capture(0, Category.Startup, CollectionStatus.Success);
        store.Save(snapshot);
        using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={path};Pooling=False"))
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE snapshots SET payload=json_remove(payload, '$.Scope', '$.Elevated'); PRAGMA user_version=1;";
            command.ExecuteNonQuery();
        }
        var reopened = new HistoryStore(path);
        Assert.Equal(snapshot.Id, reopened.BaselineId);
        Assert.Equal(CollectionScope.Legacy, reopened.Load(snapshot.Id)!.Scope);
        Assert.Equal("Legacy mixed scope", Assert.Single(reopened.List()).Context);
        Assert.Null(reopened.GetBaselineId(CollectionScope.CurrentUser));
        using var upgraded = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={path};Pooling=False");
        upgraded.Open();
        using var version = upgraded.CreateCommand();
        version.CommandText = "PRAGMA user_version;";
        Assert.Equal(2L, version.ExecuteScalar());
    }

    [Fact]
    public void EveryReportFormatIdentifiesScopeAndAdministratorAccess()
    {
        var snapshot = DiffEngineTests.Capture(0, Category.Services, CollectionStatus.Success)
            with { Scope = CollectionScope.Machine, Elevated = true };
        var report = ReportExporter.Create(snapshot, null);
        foreach (var content in new[] { ReportExporter.ToText(report), ReportExporter.ToJson(report), ReportExporter.ToCsv(report) })
            Assert.Contains("Machine-wide / Administrator access", content);
        var other = DiffEngineTests.Capture(1, Category.Services, CollectionStatus.Success)
            with { Scope = CollectionScope.CurrentUser, Elevated = false };
        var comparison = ReportExporter.Create(other, new DiffEngine().Compare(snapshot, other));
        Assert.Equal("Machine-wide / Administrator access", comparison.BeforeCollectionContext);
        Assert.Equal("Current user / Standard access", comparison.CollectionContext);
        Assert.All(comparison.Coverage, coverage => Assert.Equal("Unavailable", coverage.ComparisonStatus));
    }

    [Fact]
    public void DuplicateSaveRollsBackWithoutMovingBaseline()
    {
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        var first = DiffEngineTests.Capture(0, Category.Applications, CollectionStatus.Success);
        store.Save(first);
        Assert.Throws<Microsoft.Data.Sqlite.SqliteException>(() => store.Save(first));
        Assert.Single(store.List());
        Assert.Equal(first.Id, store.BaselineId);
    }

    [Fact]
    public void ReportsDoNotSerializeHiddenValuesAndRedactProfilePaths()
    {
        var item = new ConfigurationItem("private-id", "Example", new()
        {
            ["Location"] = @"C:\Users\Alice\Documents\tool",
            ["Another location"] = @"C:\Users\Alice Smith\Documents\tool",
            ["Endpoint"] = "PRIVATE-ENDPOINT-ID",
            ["Info"] = "token=very-secret"
        }) { Fingerprint = "PRIVATE-HMAC" };
        var snapshot = DiffEngineTests.Capture(0, Category.Startup, CollectionStatus.Success, item);
        var report = ReportExporter.Create(snapshot, null);
        foreach (var content in new[] { ReportExporter.ToJson(report), ReportExporter.ToText(report), ReportExporter.ToCsv(report) })
        {
            Assert.DoesNotContain("Alice", content);
            Assert.DoesNotContain("very-secret", content);
            Assert.DoesNotContain("PRIVATE-HMAC", content);
            Assert.DoesNotContain("Smith", content);
            Assert.DoesNotContain("PRIVATE-ENDPOINT-ID", content);
            Assert.DoesNotContain("private-id", content);
            Assert.Contains("%UserProfile%", content);
        }
    }

    [Fact]
    public void ReportShowsFailedComparisonDespiteSuccessfulCurrentRead()
    {
        var before = DiffEngineTests.Capture(0, Category.Startup, CollectionStatus.Failed);
        var after = DiffEngineTests.Capture(1, Category.Startup, CollectionStatus.Success);
        var report = ReportExporter.Create(after, new DiffEngine().Compare(before, after));
        Assert.Equal("Success", Assert.Single(report.Coverage).Status);
        Assert.Equal("Unavailable", report.Coverage[0].ComparisonStatus);
        Assert.Empty(report.UnchangedAreas);
        Assert.Contains("Comparison: Unavailable", ReportExporter.ToText(report));
    }

    [Fact]
    public void CsvNeutralizesFormulaLikeItemNames()
    {
        var first = DiffEngineTests.Capture(0, Category.Startup, CollectionStatus.Success);
        var second = DiffEngineTests.Capture(1, Category.Startup, CollectionStatus.Success,
            new ConfigurationItem("key", "=DANGEROUS()", new()));
        var report = ReportExporter.Create(second, new DiffEngine().Compare(first, second));
        Assert.Contains("\"'=DANGEROUS()\"", ReportExporter.ToCsv(report));
    }

    public void Dispose()
    {
        if (Directory.Exists(directory)) Directory.Delete(directory, true);
    }
}