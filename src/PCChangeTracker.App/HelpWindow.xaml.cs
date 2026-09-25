using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Automation.Peers;
using System.Windows.Threading;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownBlock = Markdig.Syntax.Block;
using DocumentBlock = System.Windows.Documents.Block;
using DocumentInline = System.Windows.Documents.Inline;
using DocumentTable = System.Windows.Documents.Table;
using DocumentList = System.Windows.Documents.List;
using PCChangeTracker.App.Localization;

namespace PCChangeTracker.App;

public partial class HelpWindow : Window
{
    private IReadOnlyList<HelpTopic> topics;
    private bool? compactTables;
    public UiText Texts { get; }

    public HelpWindow(UiText? texts = null)
    {
        Texts = texts ?? new UiText();
        topics = HelpContent.LoadTopics(Texts.Language.Code);
        InitializeComponent();
        DataContext = this;
        Texts.Changed += LanguageChanged;
        ShowTopics("");
        Loaded += (_, _) => HelpSearch.Focus();
    }

    private void LanguageChanged(object? sender, EventArgs eventArgs)
    {
        var index = TopicList.SelectedIndex;
        topics = HelpContent.LoadTopics(Texts.Language.Code);
        compactTables = null;
        HelpSearch.Clear();
        ShowTopics("");
        TopicList.SelectedIndex = Math.Clamp(index, 0, Math.Max(0, topics.Count - 1));
    }

    protected override void OnClosed(EventArgs eventArgs)
    {
        Texts.Changed -= LanguageChanged;
        base.OnClosed(eventArgs);
    }

    private void SearchChanged(object sender, TextChangedEventArgs eventArgs)
    {
        if (TopicList is not null) ShowTopics(HelpSearch.Text);
    }

    private void ShowTopics(string query)
    {
        var selected = (TopicList.SelectedItem as HelpTopic)?.Title;
        var matches = HelpContent.Search(topics, query);
        TopicList.ItemsSource = matches;
        SearchStatus.Text = matches.Count == 1 ? Texts["OneTopic"] : Texts.Format("TopicCount", matches.Count);
        TopicList.SelectedItem = matches.FirstOrDefault(topic => topic.Title == selected) ?? matches.FirstOrDefault();
        if (matches.Count == 0)
            HelpReader.Document = HelpContent.EmptyDocument(Texts["NoHelpMatches"], Texts);
        if (AutomationPeer.ListenerExists(AutomationEvents.LiveRegionChanged))
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                UIElementAutomationPeer.CreatePeerForElement(SearchStatus)?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged)));
    }

    private void TopicChanged(object sender, SelectionChangedEventArgs eventArgs)
    {
        RefreshTopic(true);
    }

    private void ReaderSizeChanged(object sender, SizeChangedEventArgs eventArgs) => RefreshTopic(false);
    private void TextSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> eventArgs) => RefreshTopic(false);

    private void RefreshTopic(bool force)
    {
        if (HelpReader is not null && TopicList.SelectedItem is HelpTopic topic)
        {
            var baseSize = TryFindResource("AppFont17") is double size ? size : 17;
            var compact = HelpReader.ActualWidth / (TextZoom.Value / 100 * baseSize / 17) < 600;
            if (!force && compactTables == compact) return;
            compactTables = compact;
            HelpReader.Document = HelpContent.Render(topic, compact, Texts);
            HelpReader.Document.Blocks.FirstBlock?.BringIntoView();
        }
    }

    private void FocusSearch(object sender, RoutedEventArgs eventArgs) { HelpSearch.Focus(); HelpSearch.SelectAll(); }
    private void CloseHelp(object sender, RoutedEventArgs eventArgs) => Close();

    protected override void OnPreviewKeyDown(KeyEventArgs eventArgs)
    {
        base.OnPreviewKeyDown(eventArgs);
        if (eventArgs.Key != Key.F6 || Keyboard.Modifiers is not (ModifierKeys.None or ModifierKeys.Shift)) return;
        UIElement[] regions = [HelpSearch, TopicList, HelpReader];
        var current = Array.FindIndex(regions, region => region.IsKeyboardFocusWithin);
        var next = (Math.Max(0, current) + (Keyboard.Modifiers == ModifierKeys.Shift ? 2 : 1)) % regions.Length;
        regions[next].Focus();
        eventArgs.Handled = true;
    }
}

internal sealed record HelpTopic(string Title, IReadOnlyList<MarkdownBlock> Blocks, string SearchText);

internal static class HelpContent
{
    internal static string ReadSource(string? languageCode = null)
    {
        var assembly = typeof(HelpWindow).Assembly;
        var language = languageCode?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(language) || language.Length > 35 || !language.All(character => char.IsAsciiLetterOrDigit(character) || character == '-'))
            language = "en";
        using var stream = assembly.GetManifestResourceStream($"PCChangeTracker.Helpme.{language}.md")
            ?? assembly.GetManifestResourceStream($"PCChangeTracker.Helpme.{language.Split('-')[0]}.md")
            ?? assembly.GetManifestResourceStream("PCChangeTracker.Helpme.md")
            ?? throw new InvalidOperationException("The offline guide is missing from this build.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    internal static IReadOnlyList<HelpTopic> LoadTopics(string? languageCode = null)
    {
        var document = Markdown.Parse(ReadSource(languageCode), new MarkdownPipelineBuilder().UsePipeTables().Build());
        var topics = new List<HelpTopic>();
        var title = "";
        var blocks = new List<MarkdownBlock>();
        void AddTopic()
        {
            if (blocks.Count > 0) topics.Add(new(title, blocks.ToArray(), string.Join("\n", blocks.Select(BlockText))));
        }
        foreach (var block in document)
        {
            if (block is HeadingBlock { Level: 2 } heading)
            {
                AddTopic();
                title = InlineText(heading.Inline);
                blocks.Clear();
            }
            if (title.Length > 0) blocks.Add(block);
        }
        AddTopic();
        return topics;
    }

    internal static IReadOnlyList<HelpTopic> Search(IReadOnlyList<HelpTopic> topics, string query)
    {
        var terms = query.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return topics.Where(topic => terms.All(term => (topic.Title + "\n" + topic.SearchText).Contains(term, StringComparison.OrdinalIgnoreCase))).ToArray();
    }

    internal static FlowDocument EmptyDocument(string message, UiText? texts = null)
    {
        var document = NewDocument(texts);
        document.Blocks.Add(new Paragraph(new Run(message)));
        return document;
    }

    internal static FlowDocument Render(HelpTopic topic, bool compactTables = false, UiText? texts = null)
    {
        var document = NewDocument(texts);
        foreach (var block in topic.Blocks) document.Blocks.Add(RenderBlock(block, compactTables));
        return document;
    }

    private static FlowDocument NewDocument(UiText? texts = null)
    {
        var document = new FlowDocument
        {
            FontFamily = new FontFamily("Segoe UI"), FontSize = 17,
            PagePadding = new Thickness(24, 18, 24, 24), ColumnWidth = double.PositiveInfinity,
            TextAlignment = texts?.Language.RightToLeft == true ? TextAlignment.Right : TextAlignment.Left,
            FlowDirection = texts?.Direction ?? FlowDirection.LeftToRight,
            Language = texts?.XmlLanguage ?? System.Windows.Markup.XmlLanguage.GetLanguage("en-US")
        };
        document.SetResourceReference(FlowDocument.ForegroundProperty, "TextBrush");
        document.SetResourceReference(FlowDocument.BackgroundProperty, "SurfaceBrush");
        document.SetResourceReference(FlowDocument.FontFamilyProperty, "AppFontFamily");
        document.SetResourceReference(FlowDocument.FontSizeProperty, "AppFont17");
        return document;
    }

    private static DocumentBlock RenderBlock(MarkdownBlock block, bool compactTables)
    {
        switch (block)
        {
            case HeadingBlock heading:
                var headingParagraph = ParagraphFrom(heading.Inline);
                headingParagraph.FontSize = heading.Level <= 2 ? 26 : 20;
                if (Application.Current?.TryFindResource("AppFont26") is double)
                    headingParagraph.SetResourceReference(TextElement.FontSizeProperty, heading.Level <= 2 ? "AppFont26" : "AppFont20");
                headingParagraph.FontWeight = FontWeights.SemiBold;
                headingParagraph.Margin = new Thickness(0, 12, 0, 14);
                headingParagraph.KeepWithNext = true;
                System.Windows.Automation.AutomationProperties.SetHeadingLevel(headingParagraph,
                    heading.Level <= 2 ? System.Windows.Automation.AutomationHeadingLevel.Level2 : System.Windows.Automation.AutomationHeadingLevel.Level3);
                return headingParagraph;
            case ParagraphBlock paragraph:
                return ParagraphFrom(paragraph.Inline);
            case ListBlock list:
                var result = new DocumentList { MarkerStyle = list.IsOrdered ? TextMarkerStyle.Decimal : TextMarkerStyle.Disc, Margin = new Thickness(22, 0, 0, 16) };
                foreach (var item in list.OfType<ListItemBlock>())
                {
                    var listItem = new ListItem();
                    foreach (var child in item) listItem.Blocks.Add(RenderBlock(child, compactTables));
                    result.ListItems.Add(listItem);
                }
                return result;
            case Markdig.Extensions.Tables.Table table:
                if (compactTables) return RenderCompactTable(table);
                var rendered = new DocumentTable { CellSpacing = 0, Margin = new Thickness(0, 4, 0, 18) };
                foreach (var column in table.ColumnDefinitions) rendered.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
                var rows = new TableRowGroup();
                foreach (var row in table.OfType<Markdig.Extensions.Tables.TableRow>())
                {
                    var documentRow = new System.Windows.Documents.TableRow();
                    foreach (var cell in row.OfType<Markdig.Extensions.Tables.TableCell>())
                    {
                        var documentCell = new System.Windows.Documents.TableCell
                        {
                            Padding = new Thickness(8), BorderThickness = new Thickness(0, 0, 0, 1),
                            FontWeight = row.IsHeader ? FontWeights.SemiBold : FontWeights.Normal
                        };
                        documentCell.SetResourceReference(TextElement.ForegroundProperty, "TextBrush");
                        documentCell.SetResourceReference(System.Windows.Documents.TableCell.BorderBrushProperty, "LineBrush");
                        if (row.IsHeader) documentCell.SetResourceReference(TextElement.BackgroundProperty, "CanvasBrush");
                        foreach (var child in cell) documentCell.Blocks.Add(RenderBlock(child, compactTables));
                        documentRow.Cells.Add(documentCell);
                    }
                    rows.Rows.Add(documentRow);
                }
                rendered.RowGroups.Add(rows);
                return rendered;
            case CodeBlock code:
                var codeParagraph = new Paragraph(new Run(code.Lines.ToString())) { FontFamily = new FontFamily("Consolas"), Padding = new Thickness(12), Margin = new Thickness(0, 0, 0, 16) };
                codeParagraph.SetResourceReference(TextElement.BackgroundProperty, "CanvasBrush");
                return codeParagraph;
            case ContainerBlock container:
                var section = new Section();
                foreach (var child in container) section.Blocks.Add(RenderBlock(child, compactTables));
                return section;
            case LeafBlock leaf:
                return leaf.Inline is not null ? ParagraphFrom(leaf.Inline) : new Paragraph(new Run(leaf.Lines.ToString()));
            default:
                return new Paragraph();
        }
    }

    private static Section RenderCompactTable(Markdig.Extensions.Tables.Table table)
    {
        var section = new Section();
        var rows = table.OfType<Markdig.Extensions.Tables.TableRow>().ToArray();
        var labels = rows.FirstOrDefault(row => row.IsHeader)?.Select(BlockText).ToArray() ?? [];
        foreach (var row in rows.Where(row => !row.IsHeader))
        {
            var cells = row.ToArray();
            for (var index = 0; index < cells.Length; index++)
            {
                var paragraph = new Paragraph { Margin = new Thickness(0, index == 0 ? 12 : 0, 0, 10) };
                if (index == 0) paragraph.Inlines.Add(new Bold(new Run(BlockText(cells[index]))));
                else
                {
                    if (index < labels.Length) paragraph.Inlines.Add(new Bold(new Run(labels[index] + ": ")));
                    paragraph.Inlines.Add(new Run(BlockText(cells[index])));
                }
                section.Blocks.Add(paragraph);
            }
        }
        return section;
    }

    private static Paragraph ParagraphFrom(ContainerInline? content)
    {
        var paragraph = new Paragraph { Margin = new Thickness(0, 0, 0, 14) };
        if (content is not null)
            foreach (var inline in content) paragraph.Inlines.Add(RenderInline(inline));
        return paragraph;
    }

    private static DocumentInline RenderInline(Markdig.Syntax.Inlines.Inline inline)
    {
        switch (inline)
        {
            case LiteralInline literal: return new Run(literal.Content.ToString());
            case CodeInline code: return new Run(code.Content) { FontFamily = new FontFamily("Consolas") };
            case LineBreakInline line: return line.IsHard ? new LineBreak() : new Run(" ");
            case EmphasisInline emphasis:
                Span styled = emphasis.DelimiterCount >= 2 ? new Bold() : new Italic();
                foreach (var child in emphasis) styled.Inlines.Add(RenderInline(child));
                return styled;
            case LinkInline link:
                return new Run(InlineText(link) + (string.IsNullOrEmpty(link.Url) ? "" : $" ({link.Url})"));
            case HtmlInline html: return new Run(html.Tag);
            case ContainerInline container:
                var span = new Span();
                foreach (var child in container) span.Inlines.Add(RenderInline(child));
                return span;
            default: return new Run("");
        }
    }

    private static string InlineText(ContainerInline? container) => container is null ? "" : string.Concat(container.Select(inline => inline switch
    {
        LiteralInline literal => literal.Content.ToString(),
        CodeInline code => code.Content,
        LineBreakInline => " ",
        ContainerInline nested => InlineText(nested),
        _ => ""
    }));

    private static string BlockText(MarkdownBlock block) => block switch
    {
        CodeBlock code => code.Lines.ToString(),
        LeafBlock leaf => InlineText(leaf.Inline),
        ContainerBlock container => string.Join(" ", container.Select(BlockText)),
        _ => ""
    };
}