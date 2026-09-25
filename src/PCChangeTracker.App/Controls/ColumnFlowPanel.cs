using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PCChangeTracker.App.Controls;

/// <summary>
/// Stacks children into equal-width columns whose heights are balanced without reordering them, so visual, Tab, and
/// screen-reader order all run down one column before the next. With <see cref="FillRowsFirst"/>, children instead fill
/// aligned rows left to right, which suits short form fields. The column count follows the available width and the
/// current text size, falling back to a single column when space is narrow or text is large.
/// </summary>
public sealed class ColumnFlowPanel : Panel
{
    public static readonly DependencyProperty MinColumnEmsProperty = DependencyProperty.Register(nameof(MinColumnEms), typeof(double),
        typeof(ColumnFlowPanel), new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty MaxColumnsProperty = DependencyProperty.Register(nameof(MaxColumns), typeof(int),
        typeof(ColumnFlowPanel), new FrameworkPropertyMetadata(3, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty ColumnSpacingProperty = DependencyProperty.Register(nameof(ColumnSpacing), typeof(double),
        typeof(ColumnFlowPanel), new FrameworkPropertyMetadata(12d, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty RowSpacingProperty = DependencyProperty.Register(nameof(RowSpacing), typeof(double),
        typeof(ColumnFlowPanel), new FrameworkPropertyMetadata(12d, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty FillRowsFirstProperty = DependencyProperty.Register(nameof(FillRowsFirst), typeof(bool),
        typeof(ColumnFlowPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(ColumnFlowPanel),
        new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

    private int[] columnStarts = [];
    private double[] rowHeights = [];

    /// <summary>Minimum column width, in multiples of the inherited font size.</summary>
    public double MinColumnEms { get => (double)GetValue(MinColumnEmsProperty); set => SetValue(MinColumnEmsProperty, value); }
    public int MaxColumns { get => (int)GetValue(MaxColumnsProperty); set => SetValue(MaxColumnsProperty, value); }
    public double ColumnSpacing { get => (double)GetValue(ColumnSpacingProperty); set => SetValue(ColumnSpacingProperty, value); }
    public double RowSpacing { get => (double)GetValue(RowSpacingProperty); set => SetValue(RowSpacingProperty, value); }
    /// <summary>Lays children out left to right in aligned rows (for form fields) instead of balanced top-to-bottom columns.</summary>
    public bool FillRowsFirst { get => (bool)GetValue(FillRowsFirstProperty); set => SetValue(FillRowsFirstProperty, value); }
    public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }

    /// <summary>Number of columns currently laid out.</summary>
    public int ColumnCount { get; private set; }

    internal static int ColumnsFor(double width, double minimumColumnWidth, double spacing, int maximumColumns)
    {
        var maximum = Math.Max(1, maximumColumns);
        if (double.IsInfinity(width) || double.IsNaN(width)) return maximum;
        return Math.Clamp((int)Math.Floor((width + spacing) / (Math.Max(1, minimumColumnWidth) + spacing)), 1, maximum);
    }

    /// <summary>Splits ordered heights into at most <paramref name="columns"/> contiguous groups with the smallest tallest group.</summary>
    internal static int[] BalanceColumns(IReadOnlyList<double> heights, int columns, double spacing)
    {
        var count = heights.Count;
        columns = Math.Clamp(columns, 1, Math.Max(1, count));
        if (count == 0) return [0];
        var prefix = new double[count + 1];
        for (var index = 0; index < count; index++) prefix[index + 1] = prefix[index] + heights[index];
        double Height(int start, int end) => prefix[end] - prefix[start] + spacing * (end - start - 1);
        var best = new double[columns + 1, count + 1];
        var split = new int[columns + 1, count + 1];
        for (var end = 1; end <= count; end++) best[1, end] = Height(0, end);
        for (var group = 2; group <= columns; group++)
            for (var end = group; end <= count; end++)
            {
                best[group, end] = double.PositiveInfinity;
                for (var start = group - 1; start < end; start++)
                {
                    var tallest = Math.Max(best[group - 1, start], Height(start, end));
                    // Ties keep more items in earlier columns, like newspaper columns.
                    if (tallest <= best[group, end]) { best[group, end] = tallest; split[group, end] = start; }
                }
            }
        var starts = new int[columns];
        for (int group = columns, end = count; group >= 1; group--)
        {
            starts[group - 1] = group == 1 ? 0 : split[group, end];
            end = starts[group - 1];
        }
        return starts;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var children = VisibleChildren();
        var columns = Math.Min(ColumnsFor(availableSize.Width, MinColumnEms * FontSize, ColumnSpacing, MaxColumns), Math.Max(1, children.Count));
        ColumnCount = columns;
        var width = ColumnWidth(availableSize.Width, columns);
        foreach (UIElement child in InternalChildren) child.Measure(new Size(width, double.PositiveInfinity));
        var desiredWidth = double.IsInfinity(availableSize.Width) ? columns * width + (columns - 1) * ColumnSpacing : availableSize.Width;
        if (FillRowsFirst)
        {
            rowHeights = new double[(children.Count + columns - 1) / columns];
            for (var index = 0; index < children.Count; index++)
                rowHeights[index / columns] = Math.Max(rowHeights[index / columns], children[index].DesiredSize.Height);
            return new Size(desiredWidth, rowHeights.Sum() + RowSpacing * Math.Max(0, rowHeights.Length - 1));
        }
        columnStarts = BalanceColumns(children.Select(child => child.DesiredSize.Height).ToArray(), columns, RowSpacing);
        var tallest = 0d;
        for (var column = 0; column < columnStarts.Length; column++)
        {
            var end = column + 1 < columnStarts.Length ? columnStarts[column + 1] : children.Count;
            var height = 0d;
            for (var index = columnStarts[column]; index < end; index++)
                height += children[index].DesiredSize.Height + (index > columnStarts[column] ? RowSpacing : 0);
            tallest = Math.Max(tallest, height);
        }
        return new Size(desiredWidth, tallest);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = VisibleChildren();
        var columns = Math.Max(1, ColumnCount);
        var width = ColumnWidth(finalSize.Width, columns);
        foreach (UIElement child in InternalChildren)
            if (child.Visibility == Visibility.Collapsed) child.Arrange(new Rect());
        if (FillRowsFirst)
        {
            var top = 0d;
            for (var row = 0; row < rowHeights.Length; row++)
            {
                for (var column = 0; column < columns && row * columns + column < children.Count; column++)
                    children[row * columns + column].Arrange(new Rect(column * (width + ColumnSpacing), top, width, rowHeights[row]));
                top += rowHeights[row] + RowSpacing;
            }
            return finalSize;
        }
        for (var column = 0; column < columnStarts.Length; column++)
        {
            var end = column + 1 < columnStarts.Length ? columnStarts[column + 1] : children.Count;
            var top = 0d;
            for (var index = columnStarts[column]; index < end; index++)
            {
                var child = children[index];
                child.Arrange(new Rect(column * (width + ColumnSpacing), top, width, child.DesiredSize.Height));
                top += child.DesiredSize.Height + RowSpacing;
            }
        }
        return finalSize;
    }

    private List<UIElement> VisibleChildren() => InternalChildren.Cast<UIElement>().Where(child => child.Visibility != Visibility.Collapsed).ToList();

    private double ColumnWidth(double width, int columns) => double.IsInfinity(width) || double.IsNaN(width)
        ? Math.Max(1, MinColumnEms * FontSize)
        : Math.Max(0, (width - (columns - 1) * ColumnSpacing) / columns);
}
