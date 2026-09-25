using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PCChangeTracker.Core;
using PCChangeTracker.Windows;
using PCChangeTracker.App.Localization;

namespace PCChangeTracker.App;

public sealed partial class SourceOption(CollectorDescriptor descriptor, UiText? texts = null) : ObservableObject
{
    private readonly UiText text = texts ?? new UiText();
    public Category Category => descriptor.Category;
    public string Name => text.Category(Category);
    public string StatusDisplay => text.Language.Code == "en" ? Status : text[Status switch { "Not checked" => "NotChecked", "Outside scope" => "OutsideScope", _ => Status }];
    public string DetailDisplay => text.Language.Code == "en" || string.IsNullOrEmpty(Detail) ? Detail
        : text[Status == "Partial" ? "PartialHelp" : Status == "Failed" ? "FailedHelp" : "OutsideScope"];
    public string CountDisplay => text.Format("RecordCount", Count);
    public string AccessibilityDescription => string.Join(" ", new[] { Scope, StatusDisplay, CountDisplay, DetailDisplay }.Where(value => !string.IsNullOrWhiteSpace(value)));
    public void RefreshLanguage() => OnPropertyChanged(string.Empty);
    public bool Recommended => descriptor.Recommended;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(AccessibilityDescription))] private string scope = descriptor.Scope;
    [ObservableProperty] private bool available;
    [ObservableProperty] private bool enabled = descriptor.Recommended;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(StatusDisplay)), NotifyPropertyChangedFor(nameof(AccessibilityDescription))] private string status = "Not checked";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(DetailDisplay)), NotifyPropertyChangedFor(nameof(AccessibilityDescription))] private string detail = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(CountDisplay)), NotifyPropertyChangedFor(nameof(AccessibilityDescription))] private int count;
}

public sealed record ChangeDetailRow(string Field, string Before, string After, string Difference)
{
    public string BeforeLabel { get; init; } = "";
    public string AfterLabel { get; init; } = "";
}

public sealed record ChangeRow(ObservedChange Change, bool Expected, UiText? Texts = null, Comparison? Comparison = null)
{
    private readonly UiText text = Texts ?? new UiText();
    public string Name => ReportExporter.Sanitize(Change.Name);
    public string Category => text.Category(Change.Category);
    public string Kind => text[Change.Kind.ToString()];
    public string TechnicalSummary => text.Format("TechnicalSummary", Category, Kind, Window);
    public IReadOnlyList<ChangeDetailRow> ChangedFields
    {
        get
        {
            var rows = new List<ChangeDetailRow>();
            if (Change.Before?.Name != Change.After?.Name)
                rows.Add(Detail("Name", Change.Before?.Name, Change.After?.Name));
            var keys = (Change.Before?.Fields.Keys ?? Enumerable.Empty<string>())
                .Union(Change.After?.Fields.Keys ?? Enumerable.Empty<string>())
                .Where(key => key is not "Source" and not "Endpoint").Order(StringComparer.OrdinalIgnoreCase);
            foreach (var propertyName in keys)
            {
                var before = Change.Before?.Fields.GetValueOrDefault(propertyName);
                var after = Change.After?.Fields.GetValueOrDefault(propertyName);
                if (before != after) rows.Add(Detail(propertyName, before, after));
            }
            return rows;
        }
    }
    public string SimpleSummary => ChangedFields.Count == 0 ? DifferenceSummary : "";
    public IReadOnlyList<ChangeDetailRow> FieldDetails
    {
        get
        {
            var rows = new List<ChangeDetailRow>
            {
                Detail("Name", Change.Before?.Name, Change.After?.Name),
                Detail("RecordIdentity", Change.Before?.Key, Change.After?.Key)
            };
            var keys = (Change.Before?.Fields.Keys ?? Enumerable.Empty<string>()).Union(Change.After?.Fields.Keys ?? Enumerable.Empty<string>())
                .Order(StringComparer.OrdinalIgnoreCase);
            foreach (var propertyName in keys)
                rows.Add(Detail(propertyName, Change.Before?.Fields.GetValueOrDefault(propertyName), Change.After?.Fields.GetValueOrDefault(propertyName)));
            return rows;
        }
    }
    public IReadOnlyList<ChangeDetailRow> ObservationDetails
    {
        get
        {
            if (Comparison is null) return [];
            var before = Comparison.Before.Results.SingleOrDefault(result => result.Category == Change.Category);
            var after = Comparison.After.Results.SingleOrDefault(result => result.Category == Change.Category);
            return
            [
                Detail("SnapshotId", Comparison.Before.Id.ToString(), Comparison.After.Id.ToString()),
                Detail("ScopeAccess", text.Context(Comparison.Before.Scope, Comparison.Before.Elevated), text.Context(Comparison.After.Scope, Comparison.After.Elevated)),
                Detail("SnapshotStart", Timestamp(Comparison.Before.StartedAt), Timestamp(Comparison.After.StartedAt)),
                Detail("SnapshotEnd", Timestamp(Comparison.Before.FinishedAt), Timestamp(Comparison.After.FinishedAt)),
                Detail("CollectorStart", Timestamp(before?.StartedAt), Timestamp(after?.StartedAt)),
                Detail("CollectorEnd", Timestamp(before?.FinishedAt), Timestamp(after?.FinishedAt)),
                Detail("CollectorStatus", before is null ? null : text[before.Status.ToString()], after is null ? null : text[after.Status.ToString()]),
                Detail("CollectorVersion", before?.Version.ToString(), after?.Version.ToString()),
                Detail("SchemaVersion", Comparison.Before.SchemaVersion.ToString(), Comparison.After.SchemaVersion.ToString()),
                Detail("Records", before?.Items.Count.ToString(), after?.Items.Count.ToString())
            ];
        }
    }
    public string HiddenComparison => text[Change.Before?.Fingerprint is null && Change.After?.Fingerprint is null ? "NoHiddenValue"
        : Change.Before is null || Change.After is null ? "HiddenPresence"
        : Change.Before.Fingerprint == Change.After.Fingerprint ? "HiddenUnchanged" : "HiddenChanged"];
    public string AdvancedDetails => Details + "\n\n" + text["AllFields"] + "\n" + RowsText(FieldDetails) +
        "\n\n" + text["ObservationMetadata"] + "\n" + RowsText(ObservationDetails) + "\n\n" + HiddenComparison + "\n" + text["AdvancedPrivacy"];
    private string RowsText(IEnumerable<ChangeDetailRow> rows) => string.Join("\n\n", rows.Select(row =>
        $"{row.Field} ({row.Difference})\n{text["Before"]}: {row.Before}\n{text["After"]}: {row.After}"));
    private ChangeDetailRow Detail(string key, string? before, string? after) => new(text[key], Value(before), Value(after),
        text[before == after ? "UnchangedField" : before is null ? "Added" : after is null ? "Removed" : "Modified"])
        { BeforeLabel = $"{text[key]}, {text["Before"]}", AfterLabel = $"{text[key]}, {text["After"]}" };
    private string Value(string? value) => value is null ? text["NotPresent"] : value.Length == 0 ? text["EmptyValue"] : ReportExporter.Sanitize(value);
    private static string? Timestamp(DateTimeOffset? value) => value?.ToUniversalTime().ToString("O", System.Globalization.CultureInfo.InvariantCulture);
    public string DifferenceSummary
    {
        get
        {
            if (Change.Before is null) return text["AddedSummary"];
            if (Change.After is null) return text["RemovedSummary"];
            var differences = new List<string>();
            if (Change.Before.Name != Change.After.Name)
                differences.Add($"{text["Name"]}: {ReportExporter.Sanitize(Change.Before.Name)} -> {ReportExporter.Sanitize(Change.After.Name)}");
            foreach (var propertyName in Change.Before.Fields.Keys.Union(Change.After.Fields.Keys).Where(propertyName => propertyName is not "Source" and not "Endpoint"))
            {
                var before = Change.Before.Fields.GetValueOrDefault(propertyName);
                var after = Change.After.Fields.GetValueOrDefault(propertyName);
                if (before != after)
                        differences.Add($"{text[propertyName]}: {ReportExporter.Sanitize(before ?? text["NotPresent"])} -> {ReportExporter.Sanitize(after ?? text["NotPresent"])}");
            }
                    if (differences.Count == 0) return text["PrivateSummary"];
                    return string.Join("\n", differences.Take(3)) + (differences.Count > 3 ? "\n" + text.Format("MoreFields", differences.Count - 3) : "");
        }
    }
                public string Priority => Expected ? text["Expected"] : text[Change.Importance.ToString()];
                public string Why => text.Reason(Change);
    public string Before => Summary(Change.Before);
    public string After => Summary(Change.After);
    public string Window => text.Format("DetectedBetween", text.Date(Change.ObservedFrom), text.Date(Change.ObservedTo));
    public string ExpectedAction => text[Expected ? "UndoExpected" : "MarkExpected"];
    public string Details => $"{Category} / {Kind}\n{Window}\n\n{text["Before"]}\n{Before}\n\n{text["After"]}\n{After}\n\n{Why}\n\n{text["Uncertainty"]}";
    private string Summary(ConfigurationItem? item) => item is null ? text["NotPresent"] :
        item.Fields.Count == 0 ? ReportExporter.Sanitize(item.Name) :
            string.Join("\n", item.Fields.Where(field => field.Key is not "Source" and not "Endpoint")
                .Select(field => $"{text[field.Key]}: {ReportExporter.Sanitize(field.Value)}"));
}

public sealed record NamedOption(string Code, string DisplayKey, UiText? Texts = null)
{
    private readonly UiText text = Texts ?? new UiText();
    public string Display => text[DisplayKey];
    public System.Windows.Media.SolidColorBrush? Swatch { get; init; }
    public override string ToString() => Display;
}

public sealed partial class MainViewModel : ObservableObject
{
    private readonly HistoryStore store;
    private readonly ICaptureService captureService;
    private readonly Func<bool> confirmAdministratorAccess;
    private readonly Func<bool, bool> setStartupEnabled;
    private readonly DiffEngine diffEngine = new();
    private CancellationTokenSource? captureCancellation;
    private Snapshot? currentSnapshot;
    private Comparison? comparison;
    private IReadOnlyList<ChangeRow> allChanges = [];
    private bool restoringSources;
    private bool updatingComparisonChoices;
    private bool comparisonChoicesInitialized;
    private bool backgroundWorkStopped;
    private string statusKey = "Ready";
    private object?[] statusArguments = [];
    private readonly System.Windows.Threading.DispatcherTimer scheduleTimer;

    private static readonly (string Code, int Minutes)[] ScheduleMinutes =
    [
        ("Off", 0), ("Every15Minutes", 15), ("EveryHour", 60), ("Every4Hours", 240), ("Every6Hours", 360), ("EveryDay", 1440), ("EveryWeek", 10080)
    ];
    private static readonly (string Code, int Days)[] RetentionDays =
    [
        ("Forever", 0), ("Retain30Days", 30), ("Retain90Days", 90), ("Retain180Days", 180), ("Retain365Days", 365)
    ];
    private static readonly string[] ButtonTextColorCodes = ["ButtonColorDefault", "ButtonColorNavy", "ButtonColorForest", "ButtonColorMaroon", "ButtonColorPurple"];

    public string DataDirectory { get; }
    public UiText Texts { get; }
    public IReadOnlyList<AppLanguage> Languages => UiText.Languages;
    public AppLanguage SelectedLanguage
    {
        get => Texts.Language;
        set
        {
            if (value is null || !IsIdle || value.Code == Texts.Language.Code) return;
            RunSafely(() => { store.SetPreference("language", value.Code); Texts.ChangeLanguage(value.Code); });
        }
    }
    public string Status => Texts.Format(statusKey, statusArguments);
    private void SetStatus(string key, params object?[] arguments)
    { statusKey = key; statusArguments = arguments; OnPropertyChanged(nameof(Status)); }
    public IReadOnlyList<NamedOption> Themes { get; private set; } = [];
    public IReadOnlyList<string> Fonts { get; } = ["Segoe UI", "Arial", "Calibri", "Consolas"];
    public IReadOnlyList<NamedOption> ButtonTextColors { get; private set; } = [];
    public IReadOnlyList<NamedOption> ButtonColors { get; private set; } = [];
    public IReadOnlyList<NamedOption> ScheduleIntervals { get; private set; } = [];
    public IReadOnlyList<NamedOption> RetentionOptions { get; private set; } = [];
    [ObservableProperty] private string selectedTheme = "Light";
    [ObservableProperty] private string selectedFont = "Segoe UI";
    [ObservableProperty] private int selectedTextSize = 100;
    public IReadOnlyList<int> TextSizes => App.TextSizePercentages;
    [ObservableProperty] private string selectedButtonTextColor = "ButtonColorDefault";
    [ObservableProperty] private string selectedAppTextColor = "ButtonColorDefault";
    [ObservableProperty] private string selectedButtonColor = "ButtonColorDefault";
    [ObservableProperty] private string selectedLabelColor = "ButtonColorDefault";
    [ObservableProperty] private string selectedScheduleInterval = "Every4Hours";
    [ObservableProperty] private string selectedRetention = "Retain30Days";
    [ObservableProperty] private bool startWithWindows;
    public ObservableCollection<SourceOption> Sources { get; } = [];
    public ObservableCollection<SnapshotSummary> Snapshots { get; } = [];
    public ObservableCollection<SnapshotSummary> BeforeSnapshots { get; } = [];
    public ObservableCollection<SnapshotSummary> AfterSnapshots { get; } = [];
    public IEnumerable<SnapshotSummary> BeforeSnapshotChoices => Advanced ? BeforeSnapshots : Snapshots;
    public IEnumerable<SnapshotSummary> AfterSnapshotChoices => Advanced ? AfterSnapshots : Snapshots;
    /// <summary>Distinct local calendar dates that have at least one retained snapshot; the only dates the date pickers may select.</summary>
    public IReadOnlyList<DateTime> SnapshotDates { get; private set; } = [];
    public ObservableCollection<ChangeRow> ReviewItems { get; } = [];
    public ObservableCollection<ChangeRow> RoutineItems { get; } = [];
    public ObservableCollection<ChangeRow> OtherItems { get; } = [];
    public ObservableCollection<string> Gaps { get; } = [];
    public ObservableCollection<string> Inventory { get; } = [];
    public IReadOnlyList<string> CategoryFilters { get; private set; } = new[] { "All categories" }.Concat(Enum.GetValues<Category>().Select(category => category.Display())).ToArray();

    [ObservableProperty] private string page = "Review";
    [ObservableProperty] private CollectionScope selectedScope = CollectionScope.Both;
    [ObservableProperty] private bool choosingScope;
    [ObservableProperty] private bool advanced;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string headline = "Start with a known setup";
    [ObservableProperty] private string subtitle = "No snapshots yet";
    [ObservableProperty] private string baselineLabel = "No baseline";
    [ObservableProperty] private string coverage = "Nothing has been checked";
    [ObservableProperty] private string unchangedSummary = "";
    [ObservableProperty] private string searchText = "";
    [ObservableProperty] private string categoryFilter = "All categories";
    [ObservableProperty] private bool showAllReviews;
    [ObservableProperty] private int reviewCount;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(RoutineHeading))] private int routineCount;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(OtherHeading))] private int otherCount;
    [ObservableProperty] private bool hasSnapshot;
    [ObservableProperty] private bool hasComparison;
    [ObservableProperty] private string checkpointName = "";
    [ObservableProperty] private SnapshotSummary? selectedSnapshot;
    [ObservableProperty] private SnapshotSummary? compareFrom;
    [ObservableProperty] private SnapshotSummary? compareTo;
    [ObservableProperty] private DateTime? beforeDate;
    [ObservableProperty] private DateTime? afterDate;
    [ObservableProperty] private bool compareWithToday = true;
    [ObservableProperty] private bool comparisonPickerExpanded = true;
    [ObservableProperty] private ChangeRow? selectedChange;

    public bool IsIdle => !IsBusy;
    public bool ScopeChosen => !ChoosingScope;
    /// <summary>Whether background controls should be reachable at all, i.e. no modal overlay (scope choice or change detail) is covering them.</summary>
    public bool BackgroundInteractive => ScopeChosen && SelectedChange is null;
    public bool CanConfirmScope => IsIdle && SelectedScope is CollectionScope.CurrentUser or CollectionScope.Machine or CollectionScope.Both;
    public bool CanCapture => CanConfirmScope && !ChoosingScope;
    public bool CanCaptureWithAdministrator => CanCapture && CompareWithToday && MachineScope;
    public bool CompareSavedSnapshots { get => !CompareWithToday; set { if (value) CompareWithToday = false; } }
    public string CheckActionLabel => Texts[CompareWithToday ? "CheckNow" : "Compare"];
    public string CheckActionHelp => CheckActionLabel;
    public string BeforeSelectionSummary => CompareFrom is not null ? Texts.Context(CompareFrom.Scope, CompareFrom.Elevated)
        : Texts[BeforeDate is not null ? "NoDateSnapshots" : "NoReference"];
    public string AfterSelectionSummary => CompareWithToday ? Texts["PendingSnapshot"]
        : CompareTo is not null ? Texts.Context(CompareTo.Scope, CompareTo.Elevated) : Texts["NoDateSnapshots"];
    public string ComparisonSelectionIssue => !CompareWithToday ? SavedComparisonIssue()
        : CompareFrom is null ? BeforeDate is null ? "" : Texts["NoMatchingDate"]
        : CompareFrom.Scope != SelectedScope ? Texts["ScopeMismatch"]
        : CompareFrom.Elevated ? Texts["AdminReference"]
        : "";
    public bool CurrentUserScope
    {
        get => SelectedScope is CollectionScope.CurrentUser or CollectionScope.Both;
        set { if (IsIdle) SelectedScope = (CollectionScope)(value ? (int)SelectedScope | 1 : (int)SelectedScope & ~1); }
    }
    public bool MachineScope
    {
        get => SelectedScope is CollectionScope.Machine or CollectionScope.Both;
        set { if (IsIdle) SelectedScope = (CollectionScope)(value ? (int)SelectedScope | 2 : (int)SelectedScope & ~2); }
    }
    public string ScopeLabel => Texts.Format("StandardDefault", Texts.ScopeName(SelectedScope));
    public bool Simple { get => !Advanced; set { if (value) Advanced = false; } }
    public string SelectedChangeDetails => SelectedChange is null ? "" : Advanced ? SelectedChange.AdvancedDetails : SelectedChange.Details;
    public bool IsFirstRun => !HasSnapshot;
    public bool IsBaselineOnly => HasSnapshot && !HasComparison;
    public bool HasGaps => Gaps.Count > 0;
    public string GapHeading => Texts.Format("GapCount", Gaps.Count);
    public string RoutineHeading => Texts.Format("RoutineCount", RoutineCount);
    public string OtherHeading => Texts.Format("OtherCount", OtherCount);
    public string BaselineText => Texts.Format("BaselineLabel", BaselineLabel);
    public string StorageText => Texts.Format("DatabaseSize", StorageSize);
    public string SnapshotStorageLabel => Texts.Format("SnapshotStorageSize", StorageSize);
    public bool HasMoreReviews => ReviewCount > 3 && !ShowAllReviews;
    public bool HasNoReviews => HasComparison && ReviewCount == 0;
    public string StorageSize => HistoryStorageBytes is { } bytes ? FormatStorageSize(bytes, Texts.Culture) : Texts["StorageUnavailable"];
    internal long? HistoryStorageBytes
    {
        get
        {
            long bytes = 0;
            foreach (var suffix in new[] { "", "-wal", "-shm" })
            {
                try { bytes += new FileInfo(Path.Combine(DataDirectory, "history.db" + suffix)).Length; }
                catch (FileNotFoundException) { }
                catch (DirectoryNotFoundException) { }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException) { return null; }
            }
            return bytes;
        }
    }

    internal static string FormatStorageSize(long bytes, System.Globalization.CultureInfo culture)
    {
        string[] units = ["B", "KiB", "MiB", "GiB", "TiB"];
        var unit = 0;
        var size = (double)bytes;
        while (size >= 1024 && unit < units.Length - 1) { size /= 1024; unit++; }
        return size.ToString(unit == 0 ? "0" : "0.0", culture) + " " + units[unit];
    }

    public MainViewModel(HistoryStore store, ICaptureService captureService, string dataDirectory, Func<bool>? confirmAdministratorAccess = null,
        Func<bool, bool>? setStartupEnabled = null)
    {
        this.store = store;
        this.captureService = captureService;
        Texts = new UiText(store.GetPreference("language"));
        this.confirmAdministratorAccess = confirmAdministratorAccess ?? (() => System.Windows.MessageBox.Show(
            Texts["AdminConfirm"], Texts["AdminConfirmTitle"], System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question,
            System.Windows.MessageBoxResult.No) == System.Windows.MessageBoxResult.Yes);
        DataDirectory = dataDirectory;
        this.setStartupEnabled = setStartupEnabled ?? (enabled => Environment.ProcessPath is { } executable &&
            StartupRegistration.TrySetEnabled(enabled, executable, DataDirectory));
        advanced = store.GetPreference("mode") == "advanced";
        if (Enum.TryParse<CollectionScope>(store.GetPreference("collection.scope"), out var savedScope) &&
            savedScope is CollectionScope.CurrentUser or CollectionScope.Machine or CollectionScope.Both) selectedScope = savedScope;
        else choosingScope = true;
        restoringSources = true;
        foreach (var descriptor in CollectorCatalog.All)
        {
            var option = new SourceOption(descriptor, Texts);
            option.PropertyChanged += (_, args) =>
            {
                if (!restoringSources && args.PropertyName == nameof(SourceOption.Enabled))
                    RunSafely(() => store.SetPreference($"source.{SelectedScope}.{option.Category}", option.Enabled.ToString()));
            };
            Sources.Add(option);
        }
        restoringSources = false;
        ApplySourceScope();
        RefreshHistory();
        if ((Snapshots.FirstOrDefault(snapshot => snapshot.Scope == SelectedScope && !snapshot.Elevated) ?? Snapshots.FirstOrDefault()) is { } latest)
            ShowSnapshot(latest.Id);
        selectedTheme = store.GetPreference("theme") == "Dark" ? "Dark" : "Light";
        selectedFont = Fonts.Contains(store.GetPreference("font")) ? store.GetPreference("font")! : "Segoe UI";
        selectedTextSize = int.TryParse(store.GetPreference("textSize"), out var textSize) && TextSizes.Contains(textSize) ? textSize : 100;
        selectedButtonTextColor = RestoreColorPreference("buttonTextColor");
        selectedAppTextColor = RestoreColorPreference("appTextColor");
        selectedButtonColor = RestoreColorPreference("buttonColor");
        selectedLabelColor = RestoreColorPreference("labelColor");
        selectedScheduleInterval = ScheduleMinutes.Any(entry => entry.Code == store.GetPreference("schedule.interval")) ? store.GetPreference("schedule.interval")! : "Every4Hours";
        selectedRetention = RetentionDays.Any(entry => entry.Code == store.GetPreference("retention")) ? store.GetPreference("retention")! : "Retain30Days";
        startWithWindows = bool.TryParse(store.GetPreference("startup.autostart"), out var startupEnabled) && startupEnabled;
        scheduleTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(60) };
        scheduleTimer.Tick += async (_, _) => await CheckScheduleAsync();
        UpdateScheduleTimer();
        Texts.Changed += (_, _) => RefreshLanguage();
        RefreshLanguage();
    }

    private void RefreshLanguage()
    {
        var filterIndex = CategoryFilters.ToList().IndexOf(CategoryFilter);
        CategoryFilters = new[] { Texts["AllCategories"] }.Concat(Enum.GetValues<Category>().Select(Texts.Category)).ToArray();
        CategoryFilter = CategoryFilters[Math.Max(0, filterIndex)];
        ButtonTextColors = ButtonTextColorCodes.Select(code => new NamedOption(code, code, Texts)
            { Swatch = new(App.ColorForChoice(SelectedTheme, code)) }).ToArray();
        ButtonColors = ButtonTextColorCodes.Select(code => new NamedOption(code, code, Texts)
            { Swatch = new(App.ColorForChoice(SelectedTheme, code, true)) }).ToArray();
        ScheduleIntervals = ScheduleMinutes.Select(entry => new NamedOption(entry.Code, entry.Code, Texts)).ToArray();
        RetentionOptions = RetentionDays.Select(entry => new NamedOption(entry.Code, entry.Code, Texts)).ToArray();
        Themes = new[] { new NamedOption("Light", "ThemeLight", Texts), new NamedOption("Dark", "ThemeDark", Texts) };
        var selectedChangeId = SelectedChange?.Change.Id;
        foreach (var source in Sources)
        {
            source.Scope = Texts.SourceDescription(source.Category, SelectedScope);
            source.RefreshLanguage();
        }
        if (currentSnapshot is not null) Display(currentSnapshot, comparison);
        else
        {
            Headline = Texts["StartHeadline"]; Subtitle = Texts["NoSnapshots"];
            Coverage = Texts["NothingChecked"]; BaselineLabel = Texts["NoBaseline"];
        }
        if (selectedChangeId is not null) SelectedChange = allChanges.FirstOrDefault(row => row.Change.Id == selectedChangeId);
        OnPropertyChanged(string.Empty);
    }

    partial void OnSelectedScopeChanged(CollectionScope value)
    {
        OnPropertyChanged(nameof(CurrentUserScope));
        OnPropertyChanged(nameof(MachineScope));
        OnPropertyChanged(nameof(ScopeLabel));
        ApplySourceScope();
        OnPropertyChanged(nameof(ComparisonSelectionIssue));
        NotifyCaptureCommands();
        ConfirmScopeCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(CanConfirmScope));
    }

    partial void OnChoosingScopeChanged(bool value)
    {
        OnPropertyChanged(nameof(ScopeChosen));
        OnPropertyChanged(nameof(BackgroundInteractive));
        NotifyCaptureCommands();
    }

    private void NotifyCaptureCommands()
    {
        OnPropertyChanged(nameof(CanCapture));
        OnPropertyChanged(nameof(CanCaptureWithAdministrator));
        CaptureCommand.NotifyCanExecuteChanged();
        CaptureAsAdministratorCommand.NotifyCanExecuteChanged();
        CompareSelectedCommand.NotifyCanExecuteChanged();
    }

    private void ApplySourceScope()
    {
        restoringSources = true;
        try
        {
            foreach (var source in Sources)
            {
                source.Available = CollectorCatalog.Supports(source.Category, SelectedScope);
                source.Scope = Texts.SourceDescription(source.Category, SelectedScope);
                var saved = store.GetPreference($"source.{SelectedScope}.{source.Category}") ?? store.GetPreference("source." + source.Category);
                source.Enabled = source.Available && (bool.TryParse(saved, out var enabled) ? enabled : source.Recommended);
                source.Status = source.Available ? "Not checked" : "Outside scope";
                source.Count = 0;
                source.Detail = "";
                source.RefreshLanguage();
            }
        }
        finally { restoringSources = false; }
    }

    [RelayCommand(CanExecute = nameof(IsIdle))]
    private void ChangeScope() { SelectedChange = null; ChoosingScope = true; }

    [RelayCommand(CanExecute = nameof(CanConfirmScope))]
    private void ConfirmScope()
    {
        if (!CanConfirmScope) return;
        RunSafely(() =>
        {
            store.SetPreference("collection.scope", SelectedScope.ToString());
            ChoosingScope = false;
            ComparisonPickerExpanded = true;
            RefreshHistory(true);
            var latest = Snapshots.FirstOrDefault(snapshot => snapshot.Scope == SelectedScope && !snapshot.Elevated);
            if (latest is not null) ShowSnapshot(latest.Id);
            else
            {
                currentSnapshot = null; comparison = null; HasSnapshot = false; HasComparison = false;
                allChanges = []; FilterChanges(); Inventory.Clear(); Gaps.Clear(); OnPropertyChanged(nameof(HasGaps));
                Subtitle = Texts["NoSnapshots"];
                Coverage = Texts["NothingChecked"]; UnchangedSummary = "";
                ApplySourceScope();
            }
            SetStatus("ScopeSelected", Texts.ScopeName(SelectedScope));
        });
    }

    partial void OnAdvancedChanged(bool value)
    {
        OnPropertyChanged(nameof(Simple));
        OnPropertyChanged(nameof(SelectedChangeDetails));
        OnPropertyChanged(nameof(BeforeSnapshotChoices));
        OnPropertyChanged(nameof(AfterSnapshotChoices));
        RunSafely(() => store.SetPreference("mode", value ? "advanced" : "simple"));
    }
    partial void OnSelectedThemeChanged(string value)
    {
        RunSafely(() => store.SetPreference("theme", value));
        (System.Windows.Application.Current as App)?.SetTheme(value);
        foreach (var choice in ButtonTextColors) choice.Swatch!.Color = App.ColorForChoice(value, choice.Code);
        foreach (var choice in ButtonColors) choice.Swatch!.Color = App.ColorForChoice(value, choice.Code, true);
    }
    partial void OnSelectedFontChanged(string value)
    {
        RunSafely(() => store.SetPreference("font", value));
        (System.Windows.Application.Current as App)?.SetFont(value);
    }
    partial void OnSelectedTextSizeChanged(int value)
    {
        if (!TextSizes.Contains(value)) return;
        RunSafely(() => store.SetPreference("textSize", value.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        (System.Windows.Application.Current as App)?.SetTextSize(value);
    }
    private string RestoreColorPreference(string key)
    {
        var color = store.GetPreference(key);
        return ButtonTextColorCodes.Contains(color) ? color! : "ButtonColorDefault";
    }
    private void ApplyColorPreference(string key, string value, Action<App, string> apply)
    {
        if (!ButtonTextColorCodes.Contains(value)) return;
        RunSafely(() => store.SetPreference(key, value));
        if (System.Windows.Application.Current is App app) apply(app, value);
    }
    partial void OnSelectedButtonTextColorChanged(string value) => ApplyColorPreference("buttonTextColor", value, (app, color) => app.SetButtonTextColor(color));
    partial void OnSelectedAppTextColorChanged(string value) => ApplyColorPreference("appTextColor", value, (app, color) => app.SetAppTextColor(color));
    partial void OnSelectedButtonColorChanged(string value) => ApplyColorPreference("buttonColor", value, (app, color) => app.SetButtonColor(color));
    partial void OnSelectedLabelColorChanged(string value) => ApplyColorPreference("labelColor", value, (app, color) => app.SetLabelColor(color));
    partial void OnSelectedScheduleIntervalChanged(string value)
    {
        RunSafely(() => store.SetPreference("schedule.interval", value));
        UpdateScheduleTimer();
    }
    partial void OnSelectedRetentionChanged(string value)
    {
        RunSafely(() => store.SetPreference("retention", value));
        UpdateScheduleTimer();
    }
    private void UpdateScheduleTimer()
    {
        if (SelectedScheduleInterval == "Off" && SelectedRetention == "Forever") scheduleTimer.Stop();
        else if (!scheduleTimer.IsEnabled) scheduleTimer.Start();
    }
    partial void OnStartWithWindowsChanged(bool oldValue, bool newValue)
    {
        try
        {
            if (!setStartupEnabled(newValue)) throw new InvalidOperationException("Startup registration was not changed.");
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            startWithWindows = oldValue;
            OnPropertyChanged(nameof(StartWithWindows));
            SetStatus("ActionFailed");
            return;
        }
        RunSafely(() => store.SetPreference("startup.autostart", newValue.ToString()));
    }
    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsIdle));
        NotifyCaptureCommands();
        CancelCaptureCommand.NotifyCanExecuteChanged();
        ChangeScopeCommand.NotifyCanExecuteChanged();
        ConfirmScopeCommand.NotifyCanExecuteChanged();
        SelectBaselineCommand.NotifyCanExecuteChanged();
        CurrentStateOnlyCommand.NotifyCanExecuteChanged();
    }
    partial void OnHasSnapshotChanged(bool value) { OnPropertyChanged(nameof(IsFirstRun)); OnPropertyChanged(nameof(IsBaselineOnly)); }
    partial void OnHasComparisonChanged(bool value) { OnPropertyChanged(nameof(IsBaselineOnly)); OnPropertyChanged(nameof(HasNoReviews)); }
    partial void OnSearchTextChanged(string value) => FilterChanges();
    partial void OnCategoryFilterChanged(string value) => FilterChanges();
    partial void OnShowAllReviewsChanged(bool value) => FilterChanges();
    partial void OnSelectedSnapshotChanged(SnapshotSummary? value) => CheckpointName = value?.Label ?? "";
    partial void OnSelectedChangeChanged(ChangeRow? value)
    {
        OnPropertyChanged(nameof(SelectedChangeDetails));
        OnPropertyChanged(nameof(BackgroundInteractive));
    }

    partial void OnBeforeDateChanged(DateTime? value)
    {
        if (!updatingComparisonChoices) RefreshDateChoices(true, false);
    }

    partial void OnAfterDateChanged(DateTime? value)
    {
        if (!updatingComparisonChoices) RefreshDateChoices(false, true);
    }

    partial void OnCompareFromChanged(SnapshotSummary? value)
    {
        if (!updatingComparisonChoices && value is not null)
        {
            updatingComparisonChoices = true;
            BeforeDate = value.CapturedAt.ToLocalTime().Date;
            updatingComparisonChoices = false;
            RefreshDateChoices();
        }
        NotifyComparisonSelection();
    }

    partial void OnCompareToChanged(SnapshotSummary? value)
    {
        if (!updatingComparisonChoices && value is not null)
        {
            updatingComparisonChoices = true;
            AfterDate = value.CapturedAt.ToLocalTime().Date;
            updatingComparisonChoices = false;
            RefreshDateChoices();
        }
        NotifyComparisonSelection();
    }

    partial void OnCompareWithTodayChanged(bool value)
    {
        OnPropertyChanged(nameof(CompareSavedSnapshots));
        OnPropertyChanged(nameof(CheckActionLabel));
        OnPropertyChanged(nameof(CheckActionHelp));
        NotifyComparisonSelection();
        NotifyCaptureCommands();
    }

    private void NotifyComparisonSelection()
    {
        OnPropertyChanged(nameof(BeforeSelectionSummary));
        OnPropertyChanged(nameof(AfterSelectionSummary));
        OnPropertyChanged(nameof(ComparisonSelectionIssue));
    }

    private void RefreshDateChoices(bool beforeChanged = false, bool afterChanged = false)
    {
        var beforeId = CompareFrom?.Id;
        var afterId = CompareTo?.Id;
        updatingComparisonChoices = true;
        try
        {
            BeforeSnapshots.Clear();
            foreach (var snapshot in Snapshots.Where(snapshot => BeforeDate is null || snapshot.CapturedAt.ToLocalTime().Date == BeforeDate.Value.Date))
                BeforeSnapshots.Add(snapshot);
            AfterSnapshots.Clear();
            foreach (var snapshot in Snapshots.Where(snapshot => AfterDate is null || snapshot.CapturedAt.ToLocalTime().Date == AfterDate.Value.Date))
                AfterSnapshots.Add(snapshot);
            CompareFrom = beforeChanged ? BeforeDate is null ? null : BeforeSnapshots.FirstOrDefault()
                : BeforeSnapshots.FirstOrDefault(snapshot => snapshot.Id == beforeId);
            CompareTo = afterChanged ? AfterSnapshots.FirstOrDefault()
                : AfterSnapshots.FirstOrDefault(snapshot => snapshot.Id == afterId);
        }
        finally { updatingComparisonChoices = false; }
        NotifyComparisonSelection();
    }

    [RelayCommand(CanExecute = nameof(IsIdle))]
    private void SelectBaseline()
    {
        var baseline = Snapshots.FirstOrDefault(snapshot => snapshot.Id == store.GetBaselineId(SelectedScope));
        SetComparisonChoices(baseline, CompareTo);
    }

    [RelayCommand(CanExecute = nameof(IsIdle))]
    private void CurrentStateOnly()
    {
        SetComparisonChoices(null, CompareTo);
        CompareWithToday = true;
    }

    private void SetComparisonChoices(SnapshotSummary? before, SnapshotSummary? after)
    {
        updatingComparisonChoices = true;
        try
        {
            BeforeDate = before?.CapturedAt.ToLocalTime().Date;
            AfterDate = after?.CapturedAt.ToLocalTime().Date;
            CompareFrom = before;
            CompareTo = after;
        }
        finally { updatingComparisonChoices = false; }
        RefreshDateChoices();
    }

    [RelayCommand]
    private void Navigate(string destination)
    {
        Page = destination;
    }

    partial void OnPageChanged(string value)
    {
        if (value is "Settings" or "Sources") ComparisonPickerExpanded = false;
    }

    [RelayCommand(CanExecute = nameof(CanCapture))]
    private Task CaptureAsync()
    {
        if (!CompareWithToday) { if (CanCapture) CompareSelected(); return Task.CompletedTask; }
        return CaptureSelectedAsync(false);
    }

    [RelayCommand(CanExecute = nameof(CanCaptureWithAdministrator))]
    private async Task CaptureAsAdministratorAsync()
    {
        if (!CanCaptureWithAdministrator) return;
        if (!Sources.Any(source => source.Enabled && source.Available))
        { SetStatus("ChooseSource"); Page = "Sources"; return; }
        if (!TryGetCaptureReference(true, out _)) return;
        if (!confirmAdministratorAccess())
        { SetStatus("AdminNotRequested"); return; }
        await CaptureSelectedAsync(true);
    }

    private async Task CaptureSelectedAsync(bool requestAdministratorAccess)
    {
        if (!CanCapture || !CompareWithToday) return;
        if (!TryGetCaptureReference(requestAdministratorAccess, out var before)) return;
        if (!Sources.Any(source => source.Enabled && source.Available)) { SetStatus("ChooseSource"); Page = "Sources"; return; }
        var scope = SelectedScope;
        IsBusy = true;
        captureCancellation = new CancellationTokenSource();
        try
        {
            SetStatus(requestAdministratorAccess ? "WaitingApproval" : "CheckingStandard");
            var enabled = Sources.Where(source => source.Enabled && source.Available).Select(source => source.Category).ToHashSet();
            var snapshot = await captureService.CaptureAsync(enabled, new Progress<string>(message => SetStatus("Checking", Texts.ProgressSource(message))), captureCancellation.Token,
                scope, requestAdministratorAccess);
            captureCancellation.Token.ThrowIfCancellationRequested();
            if (snapshot.Scope != scope || snapshot.Elevated != requestAdministratorAccess)
                throw new CaptureAccessException("The result did not match the requested scope and access. Nothing was saved.");
            var result = before is null ? null : diffEngine.Compare(before, snapshot);
            await Task.Run(() => store.Save(snapshot));
            RefreshHistory();
            Display(snapshot, result);
            ComparisonPickerExpanded = false;
            Page = "Review";
            SetStatus("CheckSaved", Texts.Context(snapshot.Scope, snapshot.Elevated), Texts.Date(snapshot.FinishedAt, "t"));
        }
        catch (OperationCanceledException) { SetStatus("CheckCanceled"); }
        catch (CaptureAccessException exception) { SetStatus(exception.Message.Contains("not granted", StringComparison.OrdinalIgnoreCase) ? "AdminDenied" : "AdminUnavailable"); }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        { SetStatus("SaveFailed"); }
        finally { captureCancellation.Dispose(); captureCancellation = null; IsBusy = false; }
    }

    private bool TryGetCaptureReference(bool administratorAccess, out Snapshot? before)
    {
        before = null;
        if (CompareFrom is null)
        {
            if (BeforeDate is null) return true;
            SetStatus("NoMatchingDate");
            return false;
        }
        before = store.Load(CompareFrom.Id);
        if (before is null) { SetStatus("MissingReference"); return false; }
        if (before.Scope != SelectedScope || before.Elevated != administratorAccess)
        {
            SetStatus("ReferenceMismatch");
            return false;
        }
        if (before.FinishedAt > DateTimeOffset.UtcNow)
        { SetStatus("FutureReference"); return false; }
        return true;
    }

    [RelayCommand(CanExecute = nameof(IsBusy))]
    public void CancelCapture() => captureCancellation?.Cancel();

    [RelayCommand]
    private void ShowAll() => ShowAllReviews = true;

    [RelayCommand]
    private void Inspect(ChangeRow? row) => SelectedChange = row;

    [RelayCommand]
    private void CloseDetails() => SelectedChange = null;

    [RelayCommand]
    private void ToggleExpected(ChangeRow? row)
    {
        if (row is null) return;
        RunSafely(() =>
        {
            var wasInspecting = SelectedChange?.Change.Id == row.Change.Id;
            store.SetExpected(row.Change.Id, !row.Expected);
            RebuildChanges();
            if (wasInspecting) SelectedChange = allChanges.FirstOrDefault(change => change.Change.Id == row.Change.Id);
        });
    }

    [RelayCommand]
    private void ClearFilters() { SearchText = ""; CategoryFilter = Texts["AllCategories"]; ShowAllReviews = false; }

    [RelayCommand]
    private void OpenSettings(ChangeRow? row)
    {
        var destination = row?.Change.Settings switch
        {
            SettingsPage.Apps => "ms-settings:appsfeatures", SettingsPage.Startup => "ms-settings:startupapps",
            SettingsPage.Defaults => "ms-settings:defaultapps", SettingsPage.Sound => "ms-settings:sound",
            SettingsPage.Network => "ms-settings:network-status", SettingsPage.Security => "windowsdefender:",
            SettingsPage.Updates => "ms-settings:windowsupdate-history", _ => null
        };
        RunSafely(() =>
        {
            if (destination is not null) Process.Start(new ProcessStartInfo(destination) { UseShellExecute = true });
            else
            {
                var system = Environment.GetFolderPath(Environment.SpecialFolder.System);
                var file = row?.Change.Settings switch { SettingsPage.Services => "services.msc", SettingsPage.Tasks => "taskschd.msc", SettingsPage.Devices => "devmgmt.msc", SettingsPage.Environment => "SystemPropertiesAdvanced.exe", _ => null };
                if (file is not null) Process.Start(new ProcessStartInfo(Path.Combine(system, file)) { UseShellExecute = true });
            }
        });
    }

    [RelayCommand]
    private void ViewSnapshot() { if (SelectedSnapshot is not null) RunSafely(() => { ShowSnapshot(SelectedSnapshot.Id); Page = "Review"; }); }

    [RelayCommand(CanExecute = nameof(CanCapture))]
    private void CompareSelected()
    {
        var issue = SavedComparisonIssue();
        if (issue.Length > 0) { SetStatus(SavedComparisonIssueKey()); return; }
        RunSafely(() =>
        {
            var before = store.Load(CompareFrom!.Id) ?? throw new InvalidOperationException("Before snapshot missing.");
            var after = store.Load(CompareTo!.Id) ?? throw new InvalidOperationException("After snapshot missing.");
            if (before.Id == after.Id || before.FinishedAt > after.StartedAt) { SetStatus("Chronological"); return; }
            if (before.Scope != after.Scope || before.Elevated != after.Elevated)
            { SetStatus("ScopeMismatch"); return; }
            Display(after, diffEngine.Compare(before, after));
            ComparisonPickerExpanded = false;
            Page = "Review";
            SetStatus("Compared");
        });
    }

    private string SavedComparisonIssue() => SavedComparisonIssueKey() is { Length: > 0 } key ? Texts[key] : "";

    private string SavedComparisonIssueKey()
    {
        if (CompareFrom is null || CompareTo is null) return "ChooseBoth";
        if (CompareFrom.Id == CompareTo.Id) return "DifferentSnapshots";
        if (CompareFrom.CapturedAt >= CompareTo.CapturedAt) return "EarlierFirst";
        if (CompareFrom.Scope != CompareTo.Scope || CompareFrom.Elevated != CompareTo.Elevated)
            return "ScopeMismatch";
        return "";
    }

    [RelayCommand]
    private void NameCheckpoint()
    {
        if (SelectedSnapshot is null) return;
        if (CheckpointName.Trim().Length is < 1 or > 120) { SetStatus("CheckpointLength"); return; }
        var id = SelectedSnapshot.Id;
        RunSafely(() => { store.Rename(id, CheckpointName); RefreshHistory(); SelectedSnapshot = Snapshots.First(snapshot => snapshot.Id == id); SetStatus("CheckpointSaved"); });
    }

    [RelayCommand]
    private void UseBaseline()
    {
        if (SelectedSnapshot is null || IsBusy) return;
        if (System.Windows.MessageBox.Show(Texts["ReplaceBaselineConfirm"],
            Texts["ReplaceBaselineTitle"], System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question, System.Windows.MessageBoxResult.No) != System.Windows.MessageBoxResult.Yes) return;
        var id = SelectedSnapshot.Id;
        RunSafely(() => { store.SetBaseline(id); RefreshHistory(); if (currentSnapshot is not null) ShowSnapshot(currentSnapshot.Id); SetStatus("BaselineUpdated"); });
    }

    [RelayCommand]
    private void DeleteSnapshot()
    {
        if (SelectedSnapshot is null || IsBusy) return;
        if (SelectedSnapshot.Id == store.GetBaselineId(SelectedSnapshot.Scope, SelectedSnapshot.Elevated))
        { SetStatus("ChooseNewBaseline"); return; }
        if (System.Windows.MessageBox.Show(Texts["DeleteConfirm"], Texts["DeleteTitle"],
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question, System.Windows.MessageBoxResult.No) != System.Windows.MessageBoxResult.Yes) return;
        RunSafely(() => { store.Delete(SelectedSnapshot.Id); RefreshHistory(); if (Snapshots.FirstOrDefault() is { } latest) ShowSnapshot(latest.Id); SetStatus("SnapshotDeleted"); });
    }

    [RelayCommand]
    private void ClearHistory()
    {
        if (IsBusy) return;
        if (System.Windows.MessageBox.Show(Texts["ClearConfirm"],
            Texts["ClearHistory"], System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning, System.Windows.MessageBoxResult.No) != System.Windows.MessageBoxResult.Yes) return;
        RunSafely(() =>
        {
            store.Clear(); RefreshHistory(true); currentSnapshot = null; comparison = null; HasSnapshot = false; HasComparison = false;
            allChanges = []; FilterChanges(); Inventory.Clear(); Gaps.Clear(); OnPropertyChanged(nameof(HasGaps)); OnPropertyChanged(nameof(GapHeading));
            Headline = Texts["StartHeadline"]; Subtitle = Texts["NoSnapshots"]; Coverage = Texts["NothingChecked"]; UnchangedSummary = ""; SelectedChange = null;
            foreach (var source in Sources) { source.Status = source.Available ? "Not checked" : "Outside scope"; source.Count = 0; source.Detail = ""; }
            SetStatus("HistoryCleared");
        });
    }

    [RelayCommand]
    private void PreviewReport()
    {
        if (currentSnapshot is null) { SetStatus("CaptureForReport"); return; }
        RunSafely(() =>
        {
            var report = ReportExporter.Create(currentSnapshot, comparison, store.ExpectedChanges());
            new ReportWindow(report, Advanced, Texts) { Owner = System.Windows.Application.Current.MainWindow }.ShowDialog();
        });
    }

    [RelayCommand]
    private void CopyDetails(ChangeRow? row)
    {
        if (row is not null) RunSafely(() => { System.Windows.Clipboard.SetText(Advanced ? row.AdvancedDetails : row.Details); SetStatus("DetailsCopied"); });
    }

    private void RefreshHistory(bool resetComparison = false)
    {
        var beforeId = CompareFrom?.Id;
        var afterId = CompareTo?.Id;
        Snapshots.Clear();
        foreach (var snapshot in store.List()) Snapshots.Add(snapshot);
        SnapshotDates = Snapshots.Select(snapshot => snapshot.CapturedAt.ToLocalTime().Date).Distinct().OrderByDescending(date => date).ToArray();
        OnPropertyChanged(nameof(SnapshotDates));
        var baseline = Snapshots.FirstOrDefault(snapshot => snapshot.Id == store.GetBaselineId(SelectedScope));
        BaselineLabel = baseline is null ? Texts["NoBaseline"] : Texts.Snapshot(baseline);
        SelectedSnapshot = Snapshots.FirstOrDefault(snapshot => snapshot.Scope == SelectedScope && !snapshot.Elevated) ?? Snapshots.FirstOrDefault();
        if (resetComparison || !comparisonChoicesInitialized)
            SetComparisonChoices(baseline, Snapshots.FirstOrDefault(snapshot => snapshot.Scope == SelectedScope && !snapshot.Elevated));
        else
        {
            updatingComparisonChoices = true;
            CompareFrom = Snapshots.FirstOrDefault(snapshot => snapshot.Id == beforeId);
            CompareTo = Snapshots.FirstOrDefault(snapshot => snapshot.Id == afterId);
            updatingComparisonChoices = false;
            RefreshDateChoices();
        }
        comparisonChoicesInitialized = true;
        OnPropertyChanged(nameof(StorageSize));
        OnPropertyChanged(nameof(StorageText));
        OnPropertyChanged(nameof(SnapshotStorageLabel));
        OnPropertyChanged(nameof(BaselineText));
    }

    private void ShowSnapshot(Guid id)
    {
        var snapshot = store.Load(id) ?? throw new InvalidOperationException("Snapshot missing.");
        var baseline = store.GetBaselineId(snapshot.Scope, snapshot.Elevated) is { } baselineId ? store.Load(baselineId) : null;
        var result = baseline is not null && baseline.Id != snapshot.Id && baseline.FinishedAt <= snapshot.StartedAt
            ? diffEngine.Compare(baseline, snapshot) : null;
        Display(snapshot, result);
    }

    private void Display(Snapshot snapshot, Comparison? result)
    {
        currentSnapshot = snapshot; comparison = result; HasSnapshot = true; HasComparison = result is not null;
        SelectedChange = null;
        Subtitle = result is null ? Texts.Format("CheckedAt", Texts.Date(snapshot.FinishedAt, "f"))
            : Texts.Format("DateRange", Texts.Date(result.Before.FinishedAt), Texts.Date(snapshot.FinishedAt));
        Subtitle += " / " + Texts.Context(snapshot.Scope, snapshot.Elevated);
        var baseline = Snapshots.FirstOrDefault(summary => summary.Id == store.GetBaselineId(snapshot.Scope, snapshot.Elevated));
        BaselineLabel = baseline is null ? Texts["NoBaseline"] : Texts.Snapshot(baseline);
        OnPropertyChanged(nameof(BaselineText));
        var successes = snapshot.Results.Count(source => source.Status == CollectionStatus.Success);
        var enabled = snapshot.Results.Count(source => source.Status != CollectionStatus.Disabled);
        var disabled = snapshot.Results.Count - enabled;
        Coverage = Texts.Format("CoverageCount", successes, enabled) + (disabled > 0 ? Texts.Format("DisabledCount", disabled) : "");
        UnchangedSummary = result?.Unchanged.Count > 0
            ? Texts.Format("Unchanged", string.Join(", ", result.Unchanged.Select(Texts.Category))) : "";
        Gaps.Clear();
        foreach (var source in snapshot.Results)
        {
            var option = Sources.FirstOrDefault(option => option.Category == source.Category);
            if (option is not null && snapshot.Scope == SelectedScope)
            { option.Status = source.Status.ToString(); option.Count = source.Items.Count; option.Detail = source.Diagnostic ?? ""; option.RefreshLanguage(); }
            if (source.Status is CollectionStatus.Failed or CollectionStatus.Partial)
                Gaps.Add($"{Texts.Category(source.Category)}: {(Texts.Language.Code == "en" ? source.Diagnostic : Texts[source.Status == CollectionStatus.Partial ? "PartialHelp" : "FailedHelp"])}");
        }
        if (result is not null)
            foreach (var gap in result.Gaps.Where(gap => snapshot.Results.Any(source => source.Category == gap.Category && source.Status != CollectionStatus.Disabled)))
                if (!Gaps.Any(text => text.StartsWith(Texts.Category(gap.Category) + ":", StringComparison.Ordinal)))
                    Gaps.Add($"{Texts.Category(gap.Category)}: {(Texts.Language.Code == "en" ? gap.Reason : Texts["GapHelp"])}");
        OnPropertyChanged(nameof(HasGaps));
        OnPropertyChanged(nameof(GapHeading));
        Inventory.Clear();
        foreach (var source in snapshot.Results.Where(source => source.Status is CollectionStatus.Success or CollectionStatus.Partial))
            foreach (var item in source.Items.Take(1000))
                Inventory.Add($"{Texts.Category(source.Category)}  |  {ReportExporter.Sanitize(item.Name)}  |  {string.Join("; ", item.Fields.Where(field => field.Key is "Version" or "Application" or "Device" or "Enabled" or "Startup").Select(field => $"{Texts[field.Key]}: {ReportExporter.Sanitize(field.Value)}"))}");
        RebuildChanges();
    }

    private void RebuildChanges()
    {
        var expected = store.ExpectedChanges();
        allChanges = comparison?.Changes.Select(change => new ChangeRow(change, expected.Contains(change.Id), Texts, comparison)).ToArray() ?? [];
        FilterChanges();
    }

    private void FilterChanges()
    {
        var filtered = allChanges.Where(row => (CategoryFilter == Texts["AllCategories"] || row.Category == CategoryFilter) &&
            (string.IsNullOrWhiteSpace(SearchText) || (row.Name + " " + row.Category + " " + row.Why).Contains(SearchText, StringComparison.OrdinalIgnoreCase))).ToArray();
        var reviews = filtered.Where(row => !row.Expected && row.Change.Group == TriageGroup.ReviewFirst).ToArray();
        var routine = filtered.Where(row => row.Expected || row.Change.Group == TriageGroup.Routine).ToArray();
        var other = filtered.Where(row => !row.Expected && row.Change.Group == TriageGroup.Other).ToArray();
        ReviewCount = reviews.Length; RoutineCount = routine.Length; OtherCount = other.Length;
        ReviewItems.Clear(); foreach (var row in ShowAllReviews ? reviews : reviews.Take(3)) ReviewItems.Add(row);
        RoutineItems.Clear(); foreach (var row in routine) RoutineItems.Add(row);
        OtherItems.Clear(); foreach (var row in other) OtherItems.Add(row);
        Headline = !HasSnapshot ? Texts["StartHeadline"] : !HasComparison ? Texts["SavedHeadline"]
            : reviews.Length == 0 ? Texts["NoReviews"] : reviews.Length == 1 ? Texts["OneReview"] : Texts.Format("ReviewCount", reviews.Length);
        OnPropertyChanged(nameof(HasMoreReviews)); OnPropertyChanged(nameof(HasNoReviews));
    }

    internal async Task CheckScheduleAsync()
    {
        try
        {
            if (backgroundWorkStopped || !IsIdle || ChoosingScope) return;
            if (SelectedRetention != "Forever")
            {
                var cleanupKey = $"retention.lastRun.{SelectedRetention}";
                var cleanupDue = !DateTimeOffset.TryParse(store.GetPreference(cleanupKey), System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.RoundtripKind, out var lastCleanup) || DateTimeOffset.UtcNow - lastCleanup >= TimeSpan.FromDays(1);
                if (cleanupDue)
                {
                    ApplyRetention();
                    RefreshHistory();
                    store.SetPreference(cleanupKey, DateTimeOffset.UtcNow.ToString("O", System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            var minutes = ScheduleMinutes.FirstOrDefault(entry => entry.Code == SelectedScheduleInterval).Minutes;
            if (minutes <= 0) return;
            var lastRunKey = $"schedule.lastRun.{SelectedScope}";
            var due = !DateTimeOffset.TryParse(store.GetPreference(lastRunKey), System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.RoundtripKind, out var lastRun) || DateTimeOffset.UtcNow - lastRun >= TimeSpan.FromMinutes(minutes);
            if (due) await RunScheduledCaptureAsync();
        }
        catch (Exception exception) when (exception is not OutOfMemoryException) { SetStatus("ActionFailed"); }
    }

    private async Task RunScheduledCaptureAsync()
    {
        if (!Sources.Any(source => source.Enabled && source.Available)) return;
        var scope = SelectedScope;
        IsBusy = true;
        captureCancellation = new CancellationTokenSource();
        try
        {
            var enabled = Sources.Where(source => source.Enabled && source.Available).Select(source => source.Category).ToHashSet();
            var snapshot = await captureService.CaptureAsync(enabled, new Progress<string>(_ => { }), captureCancellation.Token, scope, false);
            captureCancellation.Token.ThrowIfCancellationRequested();
            if (snapshot.Scope != scope || snapshot.Elevated)
                throw new CaptureAccessException("The result did not match the requested scope and access. Nothing was saved.");
            await Task.Run(() => store.Save(snapshot));
            store.SetPreference($"schedule.lastRun.{scope}", DateTimeOffset.UtcNow.ToString("O", System.Globalization.CultureInfo.InvariantCulture));
            ApplyRetention();
            RefreshHistory();
            SetStatus("ScheduledCheckSaved", Texts.Date(snapshot.FinishedAt, "t"));
        }
        catch (OperationCanceledException) { SetStatus("CheckCanceled"); }
        catch (Exception exception) when (exception is not OutOfMemoryException) { SetStatus("SaveFailed"); }
        finally { captureCancellation.Dispose(); captureCancellation = null; IsBusy = false; }
    }

    internal void StopBackgroundWork()
    {
        backgroundWorkStopped = true;
        scheduleTimer.Stop();
        captureCancellation?.Cancel();
    }

    internal void ApplyRetention()
    {
        var days = RetentionDays.FirstOrDefault(entry => entry.Code == SelectedRetention).Days;
        if (days <= 0) return;
        var cutoff = DateTimeOffset.UtcNow.AddDays(-days);
        var protectedIds = Enum.GetValues<CollectionScope>()
            .SelectMany(scope => new[] { store.GetBaselineId(scope, false), store.GetBaselineId(scope, true) })
            .Where(id => id.HasValue).Select(id => id!.Value).ToHashSet();
        foreach (var snapshot in store.List().Where(snapshot => snapshot.CapturedAt < cutoff && string.IsNullOrWhiteSpace(snapshot.Label) && !protectedIds.Contains(snapshot.Id)).ToArray())
            try { store.Delete(snapshot.Id); } catch (InvalidOperationException) { }
    }

    private void RunSafely(Action action)
    {
        try { action(); }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        { SetStatus("ActionFailed"); }
    }
}