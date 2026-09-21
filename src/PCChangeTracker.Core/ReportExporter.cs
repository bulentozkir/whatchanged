using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PCChangeTracker.Core;

public sealed record ReportItem(string Name, Dictionary<string, string> Fields);
public sealed record ReportChange(string Category, string Name, string Kind, string Importance, string Group,
    bool Expected, ReportItem? Before, ReportItem? After, string Why, string Uncertainty,
    DateTimeOffset From, DateTimeOffset To);
public sealed record ReportCoverage(string Category, string Status, int ItemCount, string ComparisonStatus, string? Detail);
public sealed record ReportDocument(int SchemaVersion, string Product, string Kind, DateTimeOffset GeneratedAt,
    DateTimeOffset? Before, DateTimeOffset After, IReadOnlyList<ReportChange> Changes,
    IReadOnlyList<ReportCoverage> Coverage, IReadOnlyList<ReportItem> CurrentItems, IReadOnlyList<string> UnchangedAreas, string Limitations)
{
    public string CollectionContext { get; init; } = CollectionScope.Legacy.Display();
    public string? BeforeCollectionContext { get; init; }
}

public static partial class ReportExporter
{
    public const string Limitations = "This report describes selected configuration observations. It may miss changes between checks or in unavailable areas. Related events do not establish causation or prove that the PC is safe. Profile paths are redacted, but application and folder names may still identify an organization. Hidden values and checkpoint notes are not exported.";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true, Converters = { new JsonStringEnumConverter() }
    };

    public static string Sanitize(string value)
    {
        var cleaned = ProfilePath().Replace(value, "%UserProfile%");
        cleaned = UrlCredentials().Replace(cleaned, "$1[redacted]@");
        cleaned = Secrets().Replace(cleaned, "$1=[redacted]");
        cleaned = TokenQuery().Replace(cleaned, "$1[redacted]");
        return cleaned.Replace('\0', ' ');
    }

    public static ReportDocument Create(Snapshot snapshot, Comparison? comparison, ISet<string>? expected = null)
    {
        var changes = comparison?.Changes.Select(change => new ReportChange(
            change.Category.Display(), Sanitize(change.Name), change.Kind.ToString(), change.Importance.ToString(),
            change.Group.ToString(), expected?.Contains(change.Id) == true, Project(change.Before), Project(change.After),
            Sanitize(change.Why), change.Uncertainty, change.ObservedFrom, change.ObservedTo)).ToArray() ?? [];
        var coverage = snapshot.Results.Select(result => new ReportCoverage(result.Category.Display(),
            result.Status.ToString(), result.Items.Count,
            comparison is null ? "Not requested" : comparison.Gaps.Any(gap => gap.Category == result.Category) ? "Unavailable" : "Compared",
            comparison?.Gaps.FirstOrDefault(gap => gap.Category == result.Category)?.Reason ?? result.Diagnostic))
            .Select(result => result with { Detail = result.Detail is null ? null : Sanitize(result.Detail) }).ToList();
        if (comparison is not null)
            foreach (var gap in comparison.Gaps.Where(gap => snapshot.Results.All(result => result.Category != gap.Category)))
                coverage.Add(new(gap.Category.Display(), "Missing", 0, "Unavailable", gap.Reason));
        var inventory = comparison is null
            ? snapshot.Results.OrderBy(result => result.Category).SelectMany(result => result.Items.OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                .Select(item => Project(item)! with { Name = $"{result.Category.Display()}: {Sanitize(item.Name)}" })).ToArray()
            : [];
        return new(1, "ChangeTracker", comparison is null ? "Current state (no comparison)" : "Snapshot comparison",
            DateTimeOffset.UtcNow, comparison?.Before.FinishedAt, snapshot.FinishedAt, changes, coverage, inventory,
            comparison?.Unchanged.Select(category => category.Display()).ToArray() ?? [], Limitations)
        {
            CollectionContext = CollectionScopes.Context(snapshot.Scope, snapshot.Elevated),
            BeforeCollectionContext = comparison is null ? null : CollectionScopes.Context(comparison.Before.Scope, comparison.Before.Elevated)
        };
    }

    private static ReportItem? Project(ConfigurationItem? item) => item is null ? null :
        new(Sanitize(item.Name), item.Fields.Where(field => field.Key is not "Endpoint")
            .OrderBy(field => field.Key, StringComparer.Ordinal)
            .ToDictionary(field => Sanitize(field.Key), field => Sanitize(field.Value)));

    public static string ToJson(ReportDocument report) => JsonSerializer.Serialize(report, JsonOptions);

    public static string ToText(ReportDocument report)
    {
        var text = new StringBuilder();
        text.AppendLine(report.Product).AppendLine(report.Kind);
        text.AppendLine($"Scope and access: {report.CollectionContext}");
        if (report.BeforeCollectionContext is not null) text.AppendLine($"Earlier scope and access: {report.BeforeCollectionContext}");
        if (report.Before is not null) text.AppendLine($"Before: {report.Before:O}");
        text.AppendLine($"Checked: {report.After:O}").AppendLine();
        text.AppendLine("COVERAGE");
        foreach (var source in report.Coverage)
            text.AppendLine($"{source.Category}: {source.Status}, {source.ItemCount} items. Comparison: {source.ComparisonStatus}. {source.Detail}");
        text.AppendLine();
        if (report.UnchangedAreas.Count > 0) text.AppendLine("No differences detected in compared areas: " + string.Join(", ", report.UnchangedAreas)).AppendLine();
        if (report.Kind == "Snapshot comparison" && report.Changes.Count == 0)
            text.AppendLine("No differences detected in successfully compared areas. See coverage above.");
        foreach (var change in report.Changes)
        {
            text.AppendLine($"{change.Importance.ToUpperInvariant()}: {change.Name} ({change.Kind}){(change.Expected ? " [Marked expected]" : "")}");
            text.AppendLine($"Detected between {change.From:O} and {change.To:O}");
            text.AppendLine($"Before: {Describe(change.Before)}");
            text.AppendLine($"After:  {Describe(change.After)}");
            text.AppendLine(change.Why).AppendLine(change.Uncertainty).AppendLine();
        }
        foreach (var item in report.CurrentItems) text.AppendLine(Describe(item));
        text.AppendLine().AppendLine(report.Limitations);
        return text.ToString();
    }

    public static string Describe(ReportItem? item) => item is null ? "Not present" :
        $"{item.Name}{(item.Fields.Count == 0 ? "" : ": " + string.Join("; ", item.Fields.Select(field => $"{field.Key}={field.Value}")))}";

    public static string ToCsv(ReportDocument report)
    {
        var text = new StringBuilder();
        text.AppendLine("Section,Category,Name,Kind,Importance,Expected,Before,After,Details");
        void Row(params string[] values) => text.AppendLine(string.Join(',', values.Select(CsvCell)));
        Row("Metadata", "", report.Product, report.Kind, "", "", report.Before?.ToString("O", CultureInfo.InvariantCulture) ?? "",
            report.After.ToString("O", CultureInfo.InvariantCulture), report.Limitations);
        Row("Context", "", "Scope and access", "", "", "", report.BeforeCollectionContext ?? "", report.CollectionContext, "");
        foreach (var source in report.Coverage)
            Row("Coverage", source.Category, "", source.Status, "", "", "", source.ItemCount.ToString(CultureInfo.InvariantCulture), $"Comparison: {source.ComparisonStatus}. {source.Detail}");
        foreach (var category in report.UnchangedAreas) Row("Unchanged", category, "", "", "", "", "", "", "No differences detected between compared observations.");
        foreach (var change in report.Changes)
            Row("Change", change.Category, change.Name, change.Kind, change.Importance, change.Expected.ToString(),
                Describe(change.Before), Describe(change.After), $"{change.From:O} to {change.To:O}. {change.Why} {change.Uncertainty}");
        foreach (var item in report.CurrentItems) Row("Inventory", "", item.Name, "", "", "", "", Describe(item), "");
        return text.ToString();
    }

    private static string CsvCell(string value)
    {
        if (value.Length > 0 && ("=+-@\t\r\n".Contains(value[0]) || "=+-@".Contains(value.TrimStart().FirstOrDefault()))) value = "'" + value;
        return '"' + value.Replace("\"", "\"\"") + '"';
    }

    [GeneratedRegex(@"(?i)[a-z]:\\Users\\[^\\/\r\n\""';]+", RegexOptions.CultureInvariant)]
    private static partial Regex ProfilePath();

    [GeneratedRegex(@"(?i)(https?://)[^/\s@]+@", RegexOptions.CultureInvariant)]
    private static partial Regex UrlCredentials();

    [GeneratedRegex(@"(?i)\b(password|passwd|token|secret|api[_-]?key|connectionstring)\s*[:=]\s*[^;\r\n]+", RegexOptions.CultureInvariant)]
    private static partial Regex Secrets();

    [GeneratedRegex(@"(https?://[^\s?]+\?)[^\s]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex TokenQuery();
}