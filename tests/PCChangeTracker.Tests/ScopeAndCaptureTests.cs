using System.Security.Cryptography;
using System.Buffers.Binary;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using PCChangeTracker.App;
using PCChangeTracker.Core;
using PCChangeTracker.Windows;
using Xunit;

namespace PCChangeTracker.Tests;

public sealed class ScopeAndCaptureTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CombinedCaptureKeepsUserUnelevatedAndBothInventories(bool machineElevated)
    {
        static Snapshot Fixture(CollectionScope scope, bool elevated)
        {
            var now = DateTimeOffset.UtcNow;
            return new Snapshot(Guid.NewGuid(), now, now, Enum.GetValues<Category>().Select(category =>
                new CollectionResult(category, PCChangeTracker.Windows.CollectorCatalog.Supports(category, scope) ? CollectionStatus.Success : CollectionStatus.Disabled,
                    now, now, category == Category.Applications ? [new(scope.ToString(), scope.ToString(), new())] : [])).ToArray())
                    { Scope = scope, Elevated = elevated };
        }
        var user = Fixture(CollectionScope.CurrentUser, false);
        var machine = Fixture(CollectionScope.Machine, machineElevated);
        var combined = CaptureService.Combine(user, machine);
        Assert.Equal(CollectionScope.Both, combined.Scope);
        Assert.Equal(machineElevated, combined.Elevated);
        Assert.Equal(2, combined.Results.Single(result => result.Category == Category.Applications).Items.Count);
        Assert.All(combined.Results, result => Assert.Equal(CollectionStatus.Success, result.Status));
        Assert.Throws<InvalidDataException>(() => CaptureService.Combine(user with { Elevated = true }, machine));
        var partial = machine with { Results = machine.Results.Select(result => result.Category == Category.Applications ? result with { Status = CollectionStatus.Failed, Items = [] } : result).ToArray() };
        Assert.Equal(CollectionStatus.Partial, CaptureService.Combine(user, partial).Results.Single(result => result.Category == Category.Applications).Status);
        CaptureService.ValidateRequest(CollectionScope.Both, true);
        CaptureService.ValidateRequest(CollectionScope.Both, false);
    }

    [Fact]
    public void AdvancedDetailsIncludeEveryCapturedFieldAndMetadataWithoutProtectedMaterial()
    {
        var beforeItem = new ConfigurationItem("stable-record", "Device", new()
        {
            ["Version"] = "1", ["Publisher"] = "Example", ["Source"] = @"HKLM\Software\Example",
            ["Endpoint"] = "DEVICE-ID", ["Long value"] = new string('x', 4000), ["Removed field"] = "old", ["Empty field"] = ""
        }) { Fingerprint = "SECRET-FINGERPRINT-BEFORE" };
        var afterItem = beforeItem with
        {
            Fields = new(beforeItem.Fields) { ["Version"] = "2", ["New field"] = "new" },
            Fingerprint = "SECRET-FINGERPRINT-AFTER"
        };
        afterItem.Fields.Remove("Removed field");
        var before = DiffEngineTests.Capture(0, Category.Startup, CollectionStatus.Success, beforeItem);
        var after = DiffEngineTests.Capture(1, Category.Startup, CollectionStatus.Success, afterItem);
        var comparison = new DiffEngine().Compare(before, after);
        var row = new ChangeRow(Assert.Single(comparison.Changes), false, Comparison: comparison);
        Assert.Equal(10, row.FieldDetails.Count);
        Assert.Contains(row.FieldDetails, detail => detail.Field == "Source" && detail.Before == @"HKLM\Software\Example");
        Assert.Contains(row.FieldDetails, detail => detail.Field == "Endpoint" && detail.After == "DEVICE-ID");
        Assert.Contains(row.FieldDetails, detail => detail.Field == "Long value" && detail.After.Length == 4000 && detail.Difference == "Unchanged");
        Assert.Contains(row.FieldDetails, detail => detail.Field == "Removed field" && detail.After == "Not present" && detail.Difference == "Removed");
        Assert.Contains(row.FieldDetails, detail => detail.Field == "New field" && detail.Difference == "Added");
        Assert.Contains(row.FieldDetails, detail => detail.Field == "Empty field" && detail.Before == "(empty value)");
        Assert.Contains(row.ObservationDetails, detail => detail.Field == "Snapshot ID" && detail.Before == before.Id.ToString());
        Assert.Contains(row.ObservationDetails, detail => detail.Field == "Collector version" && detail.After == "1");
        Assert.Contains("cannot be reconstructed", row.HiddenComparison);
        Assert.DoesNotContain("SECRET-FINGERPRINT", row.AdvancedDetails);
        Assert.Equal(3, row.ChangedFields.Count);
        Assert.Contains(row.ChangedFields, detail => detail.Field == "Version" && detail.Before == "1" && detail.After == "2"
            && detail.BeforeLabel == "Version, Before" && detail.AfterLabel == "Version, After");
        Assert.DoesNotContain(row.ChangedFields, detail => detail.Field is "Record identity" or "Source" or "Endpoint" or "Long value");
        Assert.Equal("", row.SimpleSummary);
        Assert.DoesNotContain("SECRET-FINGERPRINT", string.Join("\n", row.ChangedFields));
    }

    [Fact]
    public async Task SelectedSnapshotVersusTodayDoesNotMoveOrUseTheBaseline()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", "CurrentUser");
            var baseline = ComparisonSnapshot(DateTimeOffset.Now.AddDays(-4), "baseline-only");
            var reference = ComparisonSnapshot(DateTimeOffset.Now.AddDays(-2), "selected-reference");
            store.Save(baseline);
            store.Save(reference);
            var service = new RecordingCaptureService();
            var model = new MainViewModel(store, service, directory, () => throw new InvalidOperationException("No elevation expected."));
            model.BeforeDate = reference.FinishedAt.ToLocalTime().Date;
            Assert.Equal(reference.Id, model.CompareFrom!.Id);
            Assert.Empty(service.Requests);
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.Single(service.Requests);
            Assert.Equal(baseline.Id, store.GetBaselineId(CollectionScope.CurrentUser));
            Assert.Equal(reference.Id, model.CompareFrom!.Id);
            Assert.Equal("selected-reference", Assert.Single(model.ReviewItems).Name);
            Assert.Equal(ChangeKind.Removed, model.ReviewItems[0].Change.Kind);
            Assert.Equal(3, store.List().Count);
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Fact]
    public async Task SavedDateComparisonWorksInSimpleModeWithoutCapturing()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", "CurrentUser");
            var first = ComparisonSnapshot(new DateTimeOffset(DateTime.Today.AddDays(-3).AddHours(9)), "older");
            var second = ComparisonSnapshot(first.StartedAt.AddHours(1), "later-same-day");
            var third = ComparisonSnapshot(first.StartedAt.AddDays(1), "next-day");
            foreach (var snapshot in new[] { first, second, third }) store.Save(snapshot);
            var service = new RecordingCaptureService();
            var model = new MainViewModel(store, service, directory);
            Assert.True(model.Simple);
            model.BeforeDate = first.FinishedAt.ToLocalTime().Date;
            Assert.Contains(model.BeforeSnapshots, snapshot => snapshot.Id == first.Id);
            Assert.Contains(model.BeforeSnapshots, snapshot => snapshot.Id == second.Id);
            model.CompareFrom = model.BeforeSnapshots.Single(snapshot => snapshot.Id == first.Id);
            model.CompareWithToday = false;
            model.AfterDate = second.FinishedAt.ToLocalTime().Date;
            model.CompareTo = model.AfterSnapshots.Single(snapshot => snapshot.Id == second.Id);
            Assert.Equal("Compare snapshots", model.CheckActionLabel);
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.Empty(service.Requests);
            Assert.True(model.HasComparison);
            Assert.Contains(model.ReviewItems, row => row.Name == "later-same-day" && row.Change.Kind == ChangeKind.Added);
            Assert.Equal(first.Id, store.GetBaselineId(CollectionScope.CurrentUser));
            Assert.Equal(3, store.List().Count);
            model.Advanced = true;
            Assert.Equal(first.Id, model.CompareFrom!.Id);
            Assert.Equal(second.Id, model.CompareTo!.Id);
            Assert.Empty(service.Requests);
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Fact]
    public async Task MissingDateAndIncompatibleReferenceNeverTriggerCaptureOrElevation()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", "CurrentUser");
            var user = ComparisonSnapshot(DateTimeOffset.Now.AddDays(-3), "user");
            var machine = ComparisonSnapshot(DateTimeOffset.Now.AddDays(-2), "machine") with { Scope = CollectionScope.Machine };
            store.Save(user);
            store.Save(machine);
            var service = new RecordingCaptureService();
            var model = new MainViewModel(store, service, directory, () => throw new InvalidOperationException("No elevation expected."));
            model.BeforeDate = user.StartedAt.AddDays(-5).ToLocalTime().Date;
            Assert.Empty(model.BeforeSnapshots);
            Assert.Null(model.CompareFrom);
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.Contains("No saved snapshot", model.Status);
            model.BeforeDate = machine.FinishedAt.ToLocalTime().Date;
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.Contains("matching scope and access", model.Status);
            model.CompareWithToday = false;
            model.BeforeDate = user.FinishedAt.ToLocalTime().Date;
            model.AfterDate = machine.FinishedAt.ToLocalTime().Date;
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.Contains("same collection scope", model.Status);
            model.AfterDate = user.FinishedAt.ToLocalTime().Date;
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.Contains("two different snapshots", model.Status);
            Assert.Empty(service.Requests);
            Assert.Equal(2, store.List().Count);
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    private static Snapshot ComparisonSnapshot(DateTimeOffset start, string name) => new(Guid.NewGuid(), start, start.AddSeconds(1),
        [new(Category.Startup, CollectionStatus.Success, start, start.AddSeconds(1), [new(name, name, new())])])
        { Scope = CollectionScope.CurrentUser };

    [Fact]
    public async Task FirstRunRequiresScopeChoiceAndNeverElevatesOnSelectionOrModeChange()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            var service = new RecordingCaptureService();
            var confirmations = 0;
            var model = new MainViewModel(store, service, directory, () => { confirmations++; return true; });
            Assert.True(model.ChoosingScope);
            Assert.True(model.CurrentUserScope);
            Assert.True(model.MachineScope);
            Assert.False(model.CaptureCommand.CanExecute(null));
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.Empty(service.Requests);
            model.MachineScope = true;
            model.CurrentUserScope = false;
            model.Advanced = true;
            model.ConfirmScopeCommand.Execute(null);
            Assert.Equal("Machine", store.GetPreference("collection.scope"));
            Assert.Empty(service.Requests);
            Assert.Equal(0, confirmations);
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.Equal((CollectionScope.Machine, false), Assert.Single(service.Requests));
            Assert.Equal(0, confirmations);
            var reopened = new MainViewModel(store, service, directory, () => throw new InvalidOperationException("Unexpected consent prompt."));
            Assert.False(reopened.ChoosingScope);
            Assert.True(reopened.MachineScope);
            Assert.Single(service.Requests);
            Assert.False(store.Load(Assert.Single(store.List()).Id)!.Elevated);
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Fact]
    public async Task AdministratorConsentIsOneCheckOnlyAndDenialPreservesHistory()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", "Machine");
            var service = new RecordingCaptureService();
            var consent = false;
            var confirmations = 0;
            var model = new MainViewModel(store, service, directory, () => { confirmations++; return consent; });
            await model.CaptureAsAdministratorCommand.ExecuteAsync(null);
            Assert.Empty(service.Requests);
            Assert.Empty(store.List());
            consent = true;
            service.DenyAdministrator = true;
            await model.CaptureAsAdministratorCommand.ExecuteAsync(null);
            Assert.Empty(store.List());
            Assert.Null(store.GetBaselineId(CollectionScope.Machine, true));
            Assert.Contains("not granted", model.Status);
            service.DenyAdministrator = false;
            await model.CaptureAsAdministratorCommand.ExecuteAsync(null);
            Assert.True(store.Load(Assert.Single(store.List()).Id)!.Elevated);
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.False(service.Requests[^1].Elevated);
            Assert.Equal(3, confirmations);
            Assert.Equal(2, store.List().Count);
            Assert.NotNull(store.GetBaselineId(CollectionScope.Machine));
            Assert.NotNull(store.GetBaselineId(CollectionScope.Machine, true));
            model.CurrentUserScope = true;
            model.MachineScope = false;
            Assert.False(model.CaptureAsAdministratorCommand.CanExecute(null));
            await model.CaptureAsAdministratorCommand.ExecuteAsync(null);
            Assert.Equal(3, confirmations);
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    private sealed class RecordingCaptureService : ICaptureService
    {
        public List<(CollectionScope Scope, bool Elevated)> Requests { get; } = [];
        public bool DenyAdministrator { get; set; }
        public Func<CancellationToken, Task<Snapshot>>? CaptureResult { get; set; }

        public Task<Snapshot> CaptureAsync(IReadOnlySet<Category> enabled, IProgress<string> progress, CancellationToken cancellationToken,
            CollectionScope scope = CollectionScope.CurrentUser, bool requestAdministratorAccess = false)
        {
            Requests.Add((scope, requestAdministratorAccess));
            if (requestAdministratorAccess && DenyAdministrator) throw new CaptureAccessException("Administrator access was not granted.");
            if (CaptureResult is not null) return CaptureResult(cancellationToken);
            var now = DateTimeOffset.UtcNow;
            return Task.FromResult(new Snapshot(Guid.NewGuid(), now, now,
                enabled.Select(category => new CollectionResult(category, CollectionStatus.Success, now, now, [])).ToArray())
                { Scope = scope, Elevated = requestAdministratorAccess });
        }
    }

    [Theory]
    [InlineData(null, "Retain30Days")]
    [InlineData("unknown", "Retain30Days")]
    [InlineData("Forever", "Forever")]
    [InlineData("Retain30Days", "Retain30Days")]
    [InlineData("Retain90Days", "Retain90Days")]
    [InlineData("Retain180Days", "Retain180Days")]
    [InlineData("Retain365Days", "Retain365Days")]
    public void RetentionDefaultsToThirtyDaysAndHonorsSavedPreferences(string? savedRetention, string expectedRetention)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        MainViewModel? model = null;
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            if (savedRetention is not null) store.SetPreference("retention", savedRetention);
            var service = new RecordingCaptureService();
            model = new MainViewModel(store, service, directory);
            Assert.Equal(expectedRetention, model.SelectedRetention);
            Assert.Equal(savedRetention, store.GetPreference("retention"));
            Assert.Equal("Every4Hours", model.SelectedScheduleInterval);
            Assert.Empty(service.Requests);
        }
        finally
        {
            model?.StopBackgroundWork();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task RetentionCleanupRunsWithoutAutomaticCaptureAndProtectsCheckpointsAndBaselines()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", "CurrentUser");
            store.SetPreference("schedule.interval", "Off");
            var baseline = ComparisonSnapshot(DateTimeOffset.Now.AddDays(-90), "baseline");
            var expired = ComparisonSnapshot(DateTimeOffset.Now.AddDays(-60), "expired");
            var checkpoint = ComparisonSnapshot(DateTimeOffset.Now.AddDays(-45), "checkpoint");
            var recent = ComparisonSnapshot(DateTimeOffset.Now.AddDays(-1), "recent");
            var machineBaseline = baseline with { Id = Guid.NewGuid(), Scope = CollectionScope.Machine };
            foreach (var snapshot in new[] { baseline, expired, checkpoint, recent, machineBaseline }) store.Save(snapshot);
            store.Rename(checkpoint.Id, "Keep this checkpoint");
            var service = new RecordingCaptureService();
            var model = new MainViewModel(store, service, directory);
            Assert.Equal("Retain30Days", model.SelectedRetention);
            Assert.Equal("Off", model.SelectedScheduleInterval);
            model.CompareFrom = model.Snapshots.Single(snapshot => snapshot.Id == expired.Id);
            model.CompareTo = model.Snapshots.Single(snapshot => snapshot.Id == recent.Id);

            await model.CheckScheduleAsync();

            Assert.Null(store.Load(expired.Id));
            Assert.Equal(4, store.List().Count);
            Assert.DoesNotContain(model.Snapshots, snapshot => snapshot.Id == expired.Id);
            Assert.DoesNotContain(expired.FinishedAt.ToLocalTime().Date, model.SnapshotDates);
            Assert.NotNull(store.Load(baseline.Id));
            Assert.NotNull(store.Load(machineBaseline.Id));
            Assert.NotNull(store.Load(checkpoint.Id));
            Assert.NotNull(store.Load(recent.Id));
            Assert.Null(model.CompareFrom);
            Assert.Equal(recent.Id, model.CompareTo!.Id);
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.Empty(service.Requests);
            model.StopBackgroundWork();
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData(null, "Every4Hours")]
    [InlineData("unknown", "Every4Hours")]
    [InlineData("Off", "Off")]
    [InlineData("Every15Minutes", "Every15Minutes")]
    [InlineData("EveryHour", "EveryHour")]
    [InlineData("Every4Hours", "Every4Hours")]
    [InlineData("Every6Hours", "Every6Hours")]
    [InlineData("EveryDay", "EveryDay")]
    [InlineData("EveryWeek", "EveryWeek")]
    public async Task SnapshotFrequencyDefaultsToFourHoursAndHonorsSavedPreferences(string? savedInterval, string expectedInterval)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        MainViewModel? model = null;
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            if (savedInterval is not null) store.SetPreference("schedule.interval", savedInterval);
            var service = new RecordingCaptureService();
            model = new MainViewModel(store, service, directory);
            Assert.Equal(expectedInterval, model.SelectedScheduleInterval);
            Assert.Equal(savedInterval, store.GetPreference("schedule.interval"));
            Assert.Equal("Every 4 hours", Assert.Single(model.ScheduleIntervals, option => option.Code == "Every4Hours").Display);
            Assert.True(model.ChoosingScope);
            await model.CheckScheduleAsync();
            Assert.Empty(service.Requests);
        }
        finally
        {
            model?.StopBackgroundWork();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Theory]
    [InlineData(CollectionScope.CurrentUser)]
    [InlineData(CollectionScope.Machine)]
    [InlineData(CollectionScope.Both)]
    public async Task DefaultSnapshotFrequencyWaitsFourHoursBetweenSuccessfulChecks(CollectionScope scope)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        MainViewModel? model = null;
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", scope.ToString());
            var service = new RecordingCaptureService();
            model = new MainViewModel(store, service, directory, () => throw new InvalidOperationException("No elevation expected."));
            Assert.Equal("Every4Hours", model.SelectedScheduleInterval);
            var lastRunKey = $"schedule.lastRun.{scope}";
            store.SetPreference(lastRunKey, DateTimeOffset.UtcNow.AddMinutes(-239).ToString("O"));
            await model.CheckScheduleAsync();
            Assert.Empty(service.Requests);
            store.SetPreference(lastRunKey, DateTimeOffset.UtcNow.AddMinutes(-241).ToString("O"));
            await model.CheckScheduleAsync();
            await model.CheckScheduleAsync();
            Assert.Equal((scope, false), Assert.Single(service.Requests));
            Assert.Single(store.List());
        }
        finally
        {
            model?.StopBackgroundWork();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Theory]
    [InlineData(CollectionScope.CurrentUser)]
    [InlineData(CollectionScope.Machine)]
    [InlineData(CollectionScope.Both)]
    public async Task AutomaticSnapshotsRespectIntervalAndKeepTheChosenReference(CollectionScope scope)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        MainViewModel? model = null;
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", scope.ToString());
            store.SetPreference("schedule.interval", "Off");
            var baseline = ComparisonSnapshot(DateTimeOffset.Now.AddDays(-4), "baseline") with { Scope = scope };
            var reference = ComparisonSnapshot(DateTimeOffset.Now.AddDays(-2), "reference") with { Scope = scope };
            store.Save(baseline);
            store.Save(reference);
            var service = new RecordingCaptureService();
            model = new MainViewModel(store, service, directory, () => throw new InvalidOperationException("No elevation expected."));
            model.CompareFrom = model.Snapshots.Single(snapshot => snapshot.Id == reference.Id);
            await model.CheckScheduleAsync();
            Assert.Empty(service.Requests);
            model.SelectedScheduleInterval = "EveryHour";
            await model.CheckScheduleAsync();
            await model.CheckScheduleAsync();
            Assert.Equal((scope, false), Assert.Single(service.Requests));
            Assert.Equal(reference.Id, model.CompareFrom!.Id);
            Assert.Equal(baseline.Id, store.GetBaselineId(scope));
            model.CurrentStateOnlyCommand.Execute(null);
            store.SetPreference($"schedule.lastRun.{scope}", DateTimeOffset.UtcNow.AddHours(-2).ToString("O"));
            await model.CheckScheduleAsync();
            Assert.Null(model.CompareFrom);
            Assert.Equal(2, service.Requests.Count);
            Assert.Equal(4, store.List().Count);
        }
        finally { model?.StopBackgroundWork(); if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AutomaticSnapshotCanBeCanceledWithoutSaving(bool exiting)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        MainViewModel? model = null;
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", "CurrentUser");
            var pending = new TaskCompletionSource<Snapshot>(TaskCreationOptions.RunContinuationsAsynchronously);
            var service = new RecordingCaptureService { CaptureResult = token => pending.Task.WaitAsync(token) };
            model = new MainViewModel(store, service, directory) { SelectedScheduleInterval = "EveryHour" };
            var check = model.CheckScheduleAsync();
            Assert.True(model.IsBusy);
            if (exiting) model.StopBackgroundWork();
            else model.CancelCapture();
            await check.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.True(model.IsIdle);
            Assert.Empty(store.List());
            Assert.Null(store.GetPreference("schedule.lastRun.CurrentUser"));
            if (exiting)
            {
                await model.CheckScheduleAsync();
                Assert.Single(service.Requests);
            }
        }
        finally { model?.StopBackgroundWork(); if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData(CollectionScope.Machine, false)]
    [InlineData(CollectionScope.CurrentUser, true)]
    public async Task AutomaticSnapshotRejectsMismatchedScopeOrElevation(CollectionScope returnedScope, bool elevated)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        MainViewModel? model = null;
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", "CurrentUser");
            var service = new RecordingCaptureService
            {
                CaptureResult = _ => Task.FromResult(ComparisonSnapshot(DateTimeOffset.Now, "invalid") with { Scope = returnedScope, Elevated = elevated })
            };
            model = new MainViewModel(store, service, directory) { SelectedScheduleInterval = "EveryHour" };
            await model.CheckScheduleAsync();
            Assert.Empty(store.List());
            Assert.Null(store.GetPreference("schedule.lastRun.CurrentUser"));
            Assert.True(model.IsIdle);
        }
        finally { model?.StopBackgroundWork(); if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData(CollectionScope.CurrentUser)]
    [InlineData(CollectionScope.Machine)]
    [InlineData(CollectionScope.Both)]
    public async Task RetainedHistoryCanBeBrowsedAndComparedOutsideTheCurrentCaptureScope(CollectionScope captureScope)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        MainViewModel? model = null;
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", captureScope.ToString());
            store.SetPreference("schedule.interval", "Off");
            store.SetPreference("retention", "Forever");
            var contexts = new[]
            {
                (Scope: CollectionScope.CurrentUser, Elevated: false),
                (Scope: CollectionScope.Machine, Elevated: false),
                (Scope: CollectionScope.Machine, Elevated: true),
                (Scope: CollectionScope.Both, Elevated: false),
                (Scope: CollectionScope.Both, Elevated: true),
                (Scope: CollectionScope.Legacy, Elevated: false)
            };
            var pairs = contexts.Select(context => (
                Before: ComparisonSnapshot(DateTimeOffset.Now.AddDays(-3), "before") with { Scope = context.Scope, Elevated = context.Elevated },
                After: ComparisonSnapshot(DateTimeOffset.Now.AddDays(-1), "after") with { Scope = context.Scope, Elevated = context.Elevated })).ToArray();
            foreach (var pair in pairs) { store.Save(pair.Before); store.Save(pair.After); }
            var reopenedStore = new HistoryStore(Path.Combine(directory, "history.db"));
            var service = new RecordingCaptureService();
            model = new MainViewModel(reopenedStore, service, directory, () => throw new InvalidOperationException("No elevation expected."));
            Assert.Equal(pairs.Length * 2, model.Snapshots.Count);
            foreach (var pair in pairs)
            {
                model.CompareFrom = model.Snapshots.Single(snapshot => snapshot.Id == pair.Before.Id);
                model.CompareTo = model.Snapshots.Single(snapshot => snapshot.Id == pair.After.Id);
                model.CompareWithToday = false;
                await model.CaptureCommand.ExecuteAsync(null);
                Assert.True(model.HasComparison);
                Assert.Equal(captureScope, model.SelectedScope);
                Assert.Equal(pair.Before.Id, reopenedStore.GetBaselineId(pair.Before.Scope, pair.Before.Elevated));
            }
            Assert.Empty(service.Requests);
            Assert.Equal(pairs.Length * 2, reopenedStore.List().Count);
        }
        finally
        {
            model?.StopBackgroundWork();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void SettingsPersistAcrossRestartWithoutCollectingOrRegisteringOnLoad()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        MainViewModel? model = null;
        MainViewModel? reopened = null;
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", "Both");
            var service = new RecordingCaptureService();
            var registrations = new List<bool>();
            model = new MainViewModel(store, service, directory, setStartupEnabled: enabled => { registrations.Add(enabled); return true; });
            model.SelectedTheme = "Dark";
            model.SelectedFont = "Calibri";
            model.SelectedTextSize = 150;
            model.SelectedButtonTextColor = "ButtonColorForest";
            model.SelectedAppTextColor = "ButtonColorNavy";
            model.SelectedButtonColor = "ButtonColorPurple";
            model.SelectedLabelColor = "ButtonColorMaroon";
            model.SelectedScheduleInterval = "Every6Hours";
            model.SelectedRetention = "Retain90Days";
            model.StartWithWindows = true;
            reopened = new MainViewModel(store, service, directory, setStartupEnabled: _ => throw new InvalidOperationException("Do not register on load."));
            Assert.Equal("Dark", reopened.SelectedTheme);
            Assert.Equal("Calibri", reopened.SelectedFont);
            Assert.Equal(150, reopened.SelectedTextSize);
            Assert.Equal("ButtonColorForest", reopened.SelectedButtonTextColor);
            Assert.Equal("ButtonColorNavy", reopened.SelectedAppTextColor);
            Assert.Equal("ButtonColorPurple", reopened.SelectedButtonColor);
            Assert.Equal("ButtonColorMaroon", reopened.SelectedLabelColor);
            Assert.Equal("Every6Hours", reopened.SelectedScheduleInterval);
            Assert.Equal("Retain90Days", reopened.SelectedRetention);
            Assert.True(reopened.StartWithWindows);
            Assert.True(Assert.Single(registrations));
            Assert.Empty(service.Requests);
        }
        finally
        {
            model?.StopBackgroundWork();
            reopened?.StopBackgroundWork();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FailedStartupRegistrationRestoresThePreviousChoice(bool initiallyEnabled)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("startup.autostart", initiallyEnabled.ToString());
            var model = new MainViewModel(store, new RecordingCaptureService(), directory, setStartupEnabled: _ => false);
            model.StartWithWindows = !initiallyEnabled;
            Assert.Equal(initiallyEnabled, model.StartWithWindows);
            Assert.Equal(initiallyEnabled.ToString(), store.GetPreference("startup.autostart"));
            Assert.Equal(model.Texts["ActionFailed"], model.Status);
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Fact]
    public void StartupCommandQuotesPathsAndKeepsTheSameHistoryDirectory()
    {
        var command = StartupRegistration.BuildCommand(@"C:\Program Files\ChangeTracker\PCChangeTracker.exe", @"C:\History With Spaces\");
        Assert.Equal("\"C:\\Program Files\\ChangeTracker\\PCChangeTracker.exe\" --start-minimized --data-dir \"C:\\History With Spaces\\\\\"", command);
        Assert.Throws<ArgumentException>(() => StartupRegistration.BuildCommand("C:\\bad\"path.exe"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("CurrentUser")]
    [InlineData("Machine")]
    [InlineData("Both")]
    public async Task SourcesDefaultToAllSupportedChecksWithoutImplicitCapture(string? savedScope)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        MainViewModel? model = null;
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            if (savedScope is not null) store.SetPreference("collection.scope", savedScope);
            store.SetPreference("schedule.interval", "Off");
            store.SetPreference("retention", "Forever");
            var service = new RecordingCaptureService();
            model = new MainViewModel(store, service, directory, () => throw new InvalidOperationException("No elevation expected."));
            Assert.Equal(savedScope is null, model.ChoosingScope);
            Assert.All(model.Sources, source =>
            {
                Assert.Equal(CollectorCatalog.Supports(source.Category, model.SelectedScope), source.Available);
                Assert.Equal(source.Available, source.Enabled);
                Assert.Null(store.GetPreference($"source.{model.SelectedScope}.{source.Category}"));
            });
            Assert.True(Assert.Single(model.Sources, source => source.Category == Category.Network).Enabled);
            Assert.True(Assert.Single(model.Sources, source => source.Category == Category.Environment).Enabled);
            Assert.Empty(service.Requests);
            Assert.Empty(store.List());
            if (model.ChoosingScope) model.ConfirmScopeCommand.Execute(null);
            Assert.Empty(service.Requests);
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.Equal((model.SelectedScope, false), Assert.Single(service.Requests));
            var snapshot = store.Load(Assert.Single(store.List()).Id)!;
            Assert.Equal(model.Sources.Where(source => source.Available).Select(source => source.Category).Order(),
                snapshot.Results.Select(result => result.Category).Order());
        }
        finally { model?.StopBackgroundWork(); if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData(0L, "en-US", "0 B")]
    [InlineData(512L, "en-US", "512 B")]
    [InlineData(1536L, "en-US", "1.5 KiB")]
    [InlineData(1572864L, "en-US", "1.5 MiB")]
    [InlineData(1610612736L, "en-US", "1.5 GiB")]
    [InlineData(1649267441664L, "en-US", "1.5 TiB")]
    [InlineData(1572864L, "de-DE", "1,5 MiB")]
    public void HistoryStorageSizeUsesReadableLocalizedUnits(long bytes, string culture, string expected)
    {
        Assert.Equal(expected, MainViewModel.FormatStorageSize(bytes, System.Globalization.CultureInfo.GetCultureInfo(culture)));
    }

    [Fact]
    public async Task SidebarStorageIncludesHistoryFilesAcrossScopesAndRefreshesAfterCapture()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        MainViewModel? model = null;
        try
        {
            var database = Path.Combine(directory, "history.db");
            var store = new HistoryStore(database);
            store.SetPreference("collection.scope", "CurrentUser");
            store.SetPreference("schedule.interval", "Off");
            store.SetPreference("retention", "Forever");
            store.Save(ComparisonSnapshot(DateTimeOffset.Now.AddDays(-2), new string('x', 128 * 1024)) with { Scope = CollectionScope.Machine });
            var service = new RecordingCaptureService
            {
                CaptureResult = _ => Task.FromResult(ComparisonSnapshot(DateTimeOffset.Now, new string('y', 256 * 1024)))
            };
            model = new MainViewModel(store, service, directory);
            Assert.Equal(new FileInfo(database).Length, model.HistoryStorageBytes);
            Assert.True(model.HistoryStorageBytes > 128 * 1024);
            var initialSize = model.HistoryStorageBytes;
            var notifications = new List<string?>();
            model.PropertyChanged += (_, args) => notifications.Add(args.PropertyName);
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={database};Pooling=False"))
            {
                connection.Open();
                using var reader = connection.CreateCommand();
                reader.CommandText = "SELECT COUNT(*) FROM snapshots;";
                Assert.Equal(1L, reader.ExecuteScalar());
                await model.CaptureCommand.ExecuteAsync(null);
                Assert.Single(service.Requests);
                Assert.True(store.List().Count == 2, model.Status);
                Assert.True(File.Exists(database + "-wal"));
                Assert.True(new FileInfo(database + "-wal").Length > 0);
                var expectedBytes = new[] { database, database + "-wal", database + "-shm" }
                    .Where(File.Exists).Sum(path => new FileInfo(path).Length);
                Assert.Equal(expectedBytes, model.HistoryStorageBytes);
                Assert.True(model.HistoryStorageBytes > initialSize);
                Assert.Equal(model.Texts.Format("SnapshotStorageSize", model.StorageSize), model.SnapshotStorageLabel);
            }
            Assert.Equal(new FileInfo(database).Length, model.HistoryStorageBytes);
            Assert.Contains(nameof(MainViewModel.SnapshotStorageLabel), notifications);
            model.SelectedScope = CollectionScope.Machine;
            Assert.Equal(new FileInfo(database).Length, model.HistoryStorageBytes);
            Assert.Equal(2, store.List().Count);
        }
        finally { model?.StopBackgroundWork(); if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData(CollectionScope.CurrentUser)]
    [InlineData(CollectionScope.Machine)]
    [InlineData(CollectionScope.Both)]
    public void SavedSourceChoicesOverrideDefaultsAcrossScopeChangesAndRestart(CollectionScope scope)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        MainViewModel? model = null;
        MainViewModel? reopened = null;
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            store.SetPreference("collection.scope", scope.ToString());
            store.SetPreference("schedule.interval", "Off");
            store.SetPreference("retention", "Forever");
            store.SetPreference("source.Network", "False");
            store.SetPreference("source.Environment", "True");
            store.SetPreference($"source.{scope}.Environment", "False");
            store.SetPreference("source.Applications", "False");
            store.SetPreference($"source.{scope}.Applications", "True");
            store.SetPreference($"source.{scope}.Startup", "invalid");
            var service = new RecordingCaptureService();
            model = new MainViewModel(store, service, directory);
            model.SelectedScope = scope == CollectionScope.Both ? CollectionScope.CurrentUser : CollectionScope.Both;
            model.SelectedScope = scope;
            reopened = new MainViewModel(new HistoryStore(Path.Combine(directory, "history.db")), service, directory);
            foreach (var current in new[] { model, reopened })
                Assert.All(current.Sources, source => Assert.Equal(
                    source.Available && source.Category is not Category.Network and not Category.Environment, source.Enabled));
            Assert.Equal("False", store.GetPreference("source.Network"));
            Assert.Equal("True", store.GetPreference("source.Environment"));
            Assert.Equal("False", store.GetPreference($"source.{scope}.Environment"));
            Assert.Equal("True", store.GetPreference($"source.{scope}.Applications"));
            Assert.Equal("invalid", store.GetPreference($"source.{scope}.Startup"));
            Assert.Empty(service.Requests);
            Assert.Empty(store.List());
        }
        finally
        {
            model?.StopBackgroundWork();
            reopened?.StopBackgroundWork();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task ScopeCheckboxesDefaultToBothAndRejectAnEmptySelection()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerTests", Guid.NewGuid().ToString("N"));
        try
        {
            var store = new HistoryStore(Path.Combine(directory, "history.db"));
            var service = new RecordingCaptureService();
            var model = new MainViewModel(store, service, directory, () => throw new InvalidOperationException("No elevation expected."));
            Assert.Equal(CollectionScope.Both, model.SelectedScope);
            Assert.True(model.CurrentUserScope);
            Assert.True(model.MachineScope);
            model.CurrentUserScope = false;
            Assert.Equal(CollectionScope.Machine, model.SelectedScope);
            model.MachineScope = false;
            Assert.False(model.ConfirmScopeCommand.CanExecute(null));
            model.ConfirmScopeCommand.Execute(null);
            Assert.True(model.ChoosingScope);
            model.CurrentUserScope = true;
            model.MachineScope = true;
            model.ConfirmScopeCommand.Execute(null);
            Assert.Equal("Both", store.GetPreference("collection.scope"));
            await model.CaptureCommand.ExecuteAsync(null);
            Assert.Equal((CollectionScope.Both, false), Assert.Single(service.Requests));
            Assert.NotNull(store.GetBaselineId(CollectionScope.Both));
            Assert.Null(store.GetBaselineId(CollectionScope.CurrentUser));
            Assert.Null(store.GetBaselineId(CollectionScope.Machine));
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Fact]
    public void ElevationRequiresMachineScopeAndUsesOnlyFixedArguments()
    {
        Assert.Throws<ArgumentException>(() => CaptureService.ValidateRequest(CollectionScope.CurrentUser, true));
        Assert.Throws<ArgumentException>(() => CaptureService.ValidateRequest(CollectionScope.Legacy, false));
        CaptureService.ValidateRequest(CollectionScope.Machine, false);
        CaptureService.ValidateRequest(CollectionScope.Machine, true);
        var start = CollectorTransport.ElevationStartInfo(@"C:\Program Files\ChangeTracker\PCChangeTracker.exe", "example-pipe", 42);
        Assert.True(start.UseShellExecute);
        Assert.Equal("runas", start.Verb);
        Assert.Equal(new[] { "--machine-check", "example-pipe", "42" }, start.ArgumentList);
        Assert.False(start.RedirectStandardOutput);
    }

    [Fact]
    public async Task CanceledAdministratorRequestStopsBeforeAnyLaunchOrDataAccess()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            CollectorTransport.CaptureElevatedAsync("", new HashSet<Category> { Category.Services }, cancellation.Token));
    }

    [Fact]
    public void ElevatedRequestRejectsUserSourcesInvalidKeysAndDuplicates()
    {
        var material = RandomNumberGenerator.GetBytes(32);
        try
        {
            CollectorTransport.ValidateMachineRequest(new([Category.Services], material));
            Assert.Throws<InvalidDataException>(() => CollectorTransport.ValidateMachineRequest(new([Category.DefaultApps], material)));
            Assert.Throws<InvalidDataException>(() => CollectorTransport.ValidateMachineRequest(new([Category.Services, Category.Services], material)));
            Assert.Throws<InvalidDataException>(() => CollectorTransport.ValidateMachineRequest(new([Category.Services], [])));
            Assert.Throws<InvalidDataException>(() => CollectorTransport.ValidateMachineRequest(new([], material)));
        }
        finally { CryptographicOperations.ZeroMemory(material); }
    }

    [Fact]
    public async Task CollectorProtocolRejectsOversizedAndTruncatedMessages()
    {
        using var oversized = new MemoryStream();
        var header = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(header, 5000);
        oversized.Write(header);
        oversized.Position = 0;
        await Assert.ThrowsAsync<InvalidDataException>(() => CollectorTransport.ReadAsync<MachineCaptureRequest>(oversized, 4096, CancellationToken.None));
        using var truncated = new MemoryStream([10, 0, 0, 0, 123]);
        await Assert.ThrowsAsync<EndOfStreamException>(() => CollectorTransport.ReadAsync<MachineCaptureRequest>(truncated, 4096, CancellationToken.None));
        using var text = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("too long")));
        await Assert.ThrowsAsync<InvalidDataException>(() => CollectorTransport.ReadTextAsync(text, 3, CancellationToken.None));
    }

    [Fact]
    public async Task CollectorPipeAllowsBoundedLocalRoundTripWithoutElevation()
    {
        var name = "PCChangeTracker.capture." + Guid.NewGuid().ToString("N");
        using var server = CollectorTransport.CreateServer(name);
        using var client = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
        using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var connected = server.WaitForConnectionAsync(deadline.Token);
        await client.ConnectAsync(deadline.Token);
        await connected;
        var material = RandomNumberGenerator.GetBytes(32);
        try
        {
            await CollectorTransport.WriteAsync(server, new MachineCaptureRequest([Category.Services], material), deadline.Token);
            var request = await CollectorTransport.ReadAsync<MachineCaptureRequest>(client, 4096, deadline.Token);
            Assert.Equal(material, request.ComparisonKey);
            Assert.Equal(new[] { Category.Services }, request.Categories);
            CryptographicOperations.ZeroMemory(request.ComparisonKey);
        }
        finally { CryptographicOperations.ZeroMemory(material); }
    }

    [Fact]
    public void CollectorPipeAllowsAdministratorGenericReadWriteButNotPermissionChanges()
    {
        using var server = CollectorTransport.CreateServer("PCChangeTracker.capture." + Guid.NewGuid().ToString("N"));
        var rules = server.GetAccessControl().GetAccessRules(true, false, typeof(SecurityIdentifier)).Cast<PipeAccessRule>().ToArray();
        var administrators = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
        var rule = Assert.Single(rules, entry => entry.IdentityReference.Equals(administrators) && entry.AccessControlType == AccessControlType.Allow);
        var required = PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance | PipeAccessRights.Synchronize;
        Assert.Equal(required, rule.PipeAccessRights & required);
        Assert.Equal((PipeAccessRights)0, rule.PipeAccessRights & (PipeAccessRights.ChangePermissions | PipeAccessRights.TakeOwnership));
        var network = new SecurityIdentifier(WellKnownSidType.NetworkSid, null);
        Assert.Contains(rules, entry => entry.IdentityReference.Equals(network) && entry.AccessControlType == AccessControlType.Deny);
    }

    [Theory]
    [InlineData(Category.Services, CollectionScope.CurrentUser)]
    [InlineData(Category.ScheduledTasks, CollectionScope.CurrentUser)]
    [InlineData(Category.Drivers, CollectionScope.CurrentUser)]
    [InlineData(Category.WindowsUpdates, CollectionScope.CurrentUser)]
    [InlineData(Category.Protection, CollectionScope.CurrentUser)]
    [InlineData(Category.Audio, CollectionScope.Machine)]
    [InlineData(Category.DefaultApps, CollectionScope.Machine)]
    [InlineData(Category.Applications, CollectionScope.Legacy)]
    public void OutOfScopeSourcesCannotBeCreated(Category category, CollectionScope scope)
    {
        Assert.False(CollectorCatalog.Supports(category, scope));
        Assert.Throws<ArgumentException>(() => CollectorCatalog.Create(category, "", scope));
    }

    [Theory]
    [InlineData(CollectionScope.CurrentUser, "CurrentUser/", "Current user", "User")]
    [InlineData(CollectionScope.Machine, "LocalMachine/", "Machine", "Machine")]
    public void RegistryCollectorsStayInTheirSelectedHive(CollectionScope scope, string keyPrefix, string itemScope, string pathScope)
    {
        var material = RandomNumberGenerator.GetBytes(32);
        try
        {
            foreach (var category in new[] { Category.Applications, Category.Startup, Category.Environment })
            {
                var result = CollectorCatalog.Create(category, "", scope, material).Collect(CancellationToken.None);
                Assert.NotEqual(CollectionStatus.Failed, result.Status);
                Assert.All(result.Items, item =>
                {
                    Assert.Equal(category == Category.Environment ? pathScope : itemScope, item.Fields["Scope"]);
                    if (category != Category.Environment) Assert.StartsWith(keyPrefix, item.Key);
                });
            }
        }
        finally { CryptographicOperations.ZeroMemory(material); }
    }

    [Fact]
    public void UserProxyAndMachineNetworkDoNotMix()
    {
        var material = RandomNumberGenerator.GetBytes(32);
        try
        {
            var user = CollectorCatalog.Create(Category.Network, "", CollectionScope.CurrentUser, material).Collect(CancellationToken.None);
            var machine = CollectorCatalog.Create(Category.Network, "", CollectionScope.Machine, material).Collect(CancellationToken.None);
            Assert.NotEqual(CollectionStatus.Failed, user.Status);
            Assert.NotEqual(CollectionStatus.Failed, machine.Status);
            Assert.All(user.Items, item => Assert.Equal("current-user-proxy", item.Key));
            Assert.DoesNotContain(machine.Items, item => item.Key == "current-user-proxy");
            Assert.NotNull(user.FingerprintKeyId);
            Assert.Null(machine.FingerprintKeyId);
        }
        finally { CryptographicOperations.ZeroMemory(material); }
    }

    [Fact]
    public void TransferredComparisonKeyDoesNotNeedAnAdministratorProfile()
    {
        var material = RandomNumberGenerator.GetBytes(32);
        try
        {
            using var first = new ComparisonKey("", material);
            using var second = new ComparisonKey("", material);
            Assert.Equal(first.Id, second.Id);
            Assert.Equal(first.Fingerprint("example"), second.Fingerprint("example"));
            Assert.Throws<CryptographicException>(() => new ComparisonKey("", [1, 2, 3]));
        }
        finally { CryptographicOperations.ZeroMemory(material); }
    }
}