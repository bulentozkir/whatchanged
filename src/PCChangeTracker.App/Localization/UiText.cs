using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using PCChangeTracker.Core;
using PCChangeTracker.Windows;

namespace PCChangeTracker.App.Localization;

public sealed record AppLanguage(string Code, string NativeName, string EnglishName, string CultureName, bool RightToLeft = false)
{
    public string Display => $"{NativeName} ({EnglishName})";
}

public sealed class UiText : INotifyPropertyChanged
{
    public static IReadOnlyList<AppLanguage> Languages { get; } =
    [
        new("en", "English", "English", "en-US"),
        new("zh-hans", "\u7b80\u4f53\u4e2d\u6587", "Mandarin Chinese", "zh-CN"),
        new("hi", "\u0939\u093f\u0928\u094d\u0926\u0940", "Hindi", "hi-IN"),
        new("es", "Espa\u00f1ol", "Spanish", "es-ES"),
        new("ar", "\u0627\u0644\u0639\u0631\u0628\u064a\u0629 \u0627\u0644\u0641\u0635\u062d\u0649", "Standard Arabic", "ar-SA", true),
        new("fr", "Fran\u00e7ais", "French", "fr-FR"),
        new("bn", "\u09ac\u09be\u0982\u09b2\u09be", "Bengali", "bn-BD"),
        new("pt", "Portugu\u00eas", "Portuguese", "pt-BR"),
        new("id", "Bahasa Indonesia", "Indonesian", "id-ID"),
        new("ur", "\u0627\u0631\u062f\u0648", "Urdu", "ur-PK", true),
        new("ru", "\u0420\u0443\u0441\u0441\u043a\u0438\u0439", "Russian", "ru-RU"),
        new("de", "Deutsch", "German", "de-DE"),
        new("ja", "\u65e5\u672c\u8a9e", "Japanese", "ja-JP"),
        new("pcm", "Naij\u00e1", "Nigerian Pidgin", "en-NG"),
        new("arz", "\u0639\u0631\u0628\u064a \u0645\u0635\u0631\u064a", "Egyptian Arabic", "ar-EG", true),
        new("mr", "\u092e\u0930\u093e\u0920\u0940", "Marathi", "mr-IN"),
        new("vi", "Ti\u1ebfng Vi\u1ec7t", "Vietnamese", "vi-VN"),
        new("te", "\u0c24\u0c46\u0c32\u0c41\u0c17\u0c41", "Telugu", "te-IN"),
        new("ha", "Hausa", "Hausa", "ha-Latn-NG"),
        new("tr", "T\u00fcrk\u00e7e", "Turkish", "tr-TR")
    ];

    private static readonly IReadOnlyDictionary<string, string> English = ReadCatalog("en");
    private IReadOnlyDictionary<string, string> catalog = English;
    public AppLanguage Language { get; private set; } = Languages[0];
    public int Revision { get; private set; }
    public CultureInfo Culture => CultureInfo.GetCultureInfo(Language.CultureName);
    public XmlLanguage XmlLanguage => XmlLanguage.GetLanguage(Language.CultureName);
    public FlowDirection Direction => Language.RightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? Changed;

    public UiText(string? languageCode = null) => ChangeLanguage(languageCode);

    public string this[string key] => catalog.TryGetValue(key, out var value) ? value : English.GetValueOrDefault(key, key);
    public string Format(string key, params object?[] arguments) => string.Format(Culture, this[key], arguments);
    public string Date(DateTimeOffset value, string format = "g") => value.ToLocalTime().ToString(format, Culture);

    public void ChangeLanguage(string? languageCode)
    {
        Language = Languages.FirstOrDefault(language => language.Code.Equals(languageCode, StringComparison.OrdinalIgnoreCase)) ?? Languages[0];
        catalog = Language.Code == "en" ? English : ReadCatalog(Language.Code);
        Revision++;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        Changed?.Invoke(this, EventArgs.Empty);
    }

    internal static IReadOnlyDictionary<string, string> ReadCatalog(string code)
    {
        using var stream = typeof(UiText).Assembly.GetManifestResourceStream($"PCChangeTracker.Strings.{code}.json");
        return stream is null ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(stream)
            ?? throw new InvalidDataException("The language catalog is empty.");
    }

    public string Category(Category category) => this["Category." + category];
    public string ScopeName(CollectionScope scope) => this[scope switch
    {
        CollectionScope.CurrentUser => "UserScope", CollectionScope.Machine => "MachineScope", CollectionScope.Both => "BothScope", _ => "LegacyScope"
    }];
    public string Context(CollectionScope scope, bool elevated) => scope == CollectionScope.Legacy
        ? this["LegacyScope"] : $"{ScopeName(scope)} / {this[elevated ? scope == CollectionScope.Both ? "MachineAdminOnly" : "AdminAccess" : "StandardAccess"]}";

    public string Snapshot(SnapshotSummary snapshot, bool compact = false) => compact
        ? $"{Date(snapshot.CapturedAt, "d")} {Date(snapshot.CapturedAt, "HH:mm:ss.fff zzz")}  |  {(string.IsNullOrWhiteSpace(snapshot.Label) ? this["ManualCheck"] : snapshot.Label)}  |  {Context(snapshot.Scope, snapshot.Elevated)}"
        : $"{Date(snapshot.CapturedAt)}  {(string.IsNullOrWhiteSpace(snapshot.Label) ? this["ManualCheck"] : snapshot.Label)}  ({Context(snapshot.Scope, snapshot.Elevated)})";

    public string Reason(ObservedChange change) => Language.Code == "en" ? change.Why : this[change.Category switch
    {
        Core.Category.Protection => "ReasonProtection",
        Core.Category.Startup or Core.Category.Services or Core.Category.ScheduledTasks => "ReasonLaunch",
        Core.Category.Applications => change.Group == TriageGroup.Routine ? "ReasonVersion" : "ReasonApp",
        Core.Category.DefaultApps => "ReasonDefaults", Core.Category.Audio => "ReasonAudio",
        Core.Category.Network => "ReasonNetwork", Core.Category.Drivers => "ReasonDrivers",
        Core.Category.Environment => "ReasonPath", Core.Category.WindowsUpdates => "ReasonUpdate", _ => "ReasonOther"
    }];

    public string SourceDescription(Category category, CollectionScope scope)
    {
        if (Language.Code == "en") return CollectorCatalog.Describe(category, scope);
        if (!CollectorCatalog.Supports(category, scope)) return this["OutsideScope"];
        return this[scope == CollectionScope.Both ? "SourceBoth" : scope == CollectionScope.Machine ? "SourceMachine" : "SourceUser"] + " " + this[category switch
        {
            Core.Category.Network => "SourceNetwork", Core.Category.Environment => "SourcePath", _ => "SourcePrivate"
        }];
    }

    public string ProgressSource(string original) => CollectorCatalog.All.FirstOrDefault(descriptor =>
        original.Equals("Checking " + descriptor.Name.ToLowerInvariant() + "...", StringComparison.OrdinalIgnoreCase)) is { } source
        ? Category(source.Category) : this["CollectionSources"];

    public string TextReport(ReportDocument report)
    {
        if (Language.Code == "en") return ReportExporter.ToText(report);
        string ContextText(string context)
        {
            if (context == CollectionScope.Legacy.Display()) return this["LegacyScope"];
            return Context(context.StartsWith("Current user +", StringComparison.Ordinal) ? CollectionScope.Both
                : context.StartsWith("Machine-wide", StringComparison.Ordinal) ? CollectionScope.Machine : CollectionScope.CurrentUser,
                context.Contains("administrator", StringComparison.OrdinalIgnoreCase));
        }
        string CategoryText(string name) => Enum.GetValues<Core.Category>().FirstOrDefault(category => category.Display() == name) is var category
            && category.Display() == name ? Category(category) : name;
        string ItemText(ReportItem? item) => item is null ? this["NotPresent"] : item.Name + ": " +
            string.Join("; ", item.Fields.Select(property => $"{this[property.Key]}={property.Value}"));
        var result = new StringBuilder().AppendLine(report.Product).AppendLine(this[report.Kind == "Snapshot comparison" ? "ComparisonReport" : "CurrentReport"]);
        result.AppendLine(this["ScopeAccess"] + ": " + ContextText(report.CollectionContext));
        if (report.Before is { } before) result.AppendLine(this["Before"] + ": " + Date(before, "F"));
        result.AppendLine(this["After"] + ": " + Date(report.After, "F")).AppendLine().AppendLine(this["Coverage"]);
        foreach (var source in report.Coverage)
        {
            result.AppendLine($"{CategoryText(source.Category)}: {this[source.Status]}, {Format("RecordCount", source.ItemCount)}.");
            if (source.ComparisonStatus == "Unavailable") result.AppendLine(this["GapHelp"]);
        }
        if (report.UnchangedAreas.Count > 0) result.AppendLine(Format("Unchanged", string.Join(", ", report.UnchangedAreas.Select(CategoryText))));
        foreach (var change in report.Changes)
        {
            result.AppendLine().AppendLine($"{this[change.Importance]}: {change.Name} ({this[change.Kind]}){(change.Expected ? " [" + this["Expected"] + "]" : "")}");
            result.AppendLine(Format("DetectedBetween", Date(change.From), Date(change.To)));
            result.AppendLine(this["Before"] + ": " + ItemText(change.Before));
            result.AppendLine(this["After"] + ": " + ItemText(change.After));
            result.AppendLine(this["Uncertainty"]);
        }
        foreach (var item in report.CurrentItems) result.AppendLine(ItemText(item));
        return result.AppendLine().AppendLine(this["ReportLimitations"]).ToString();
    }
}

[MarkupExtensionReturnType(typeof(string))]
public sealed class LocExtension(string key) : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        return new Binding($"DataContext.Texts[{key}]")
        {
            RelativeSource = target?.TargetObject is Window ? RelativeSource.Self : new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1),
            Mode = BindingMode.OneWay
        }.ProvideValue(serviceProvider);
    }
}

public sealed class SnapshotTextConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] is not SnapshotSummary snapshot || values[1] is not UiText text) return "";
        return parameter as string == "Context" ? text.Context(snapshot.Scope, snapshot.Elevated)
            : text.Snapshot(snapshot, parameter as string == "Compact");
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}