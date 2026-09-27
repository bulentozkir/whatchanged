using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace PCChangeTracker.App.Controls;

/// <summary>
/// The main command bar. Its first child leads the row and its second child ends it. While both fit, the trailing commands sit
/// at the end of the same row; when they would be squeezed (a narrow window or large text), they move to their own row below,
/// still aligned to the end, so no label is crushed and no command is clipped. Assistive technology sees a named toolbar,
/// which a plain panel cannot provide because panels have no automation peer.
/// </summary>
public sealed class CommandBarPanel : Panel
{
    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(nameof(Spacing), typeof(double),
        typeof(CommandBarPanel), new FrameworkPropertyMetadata(16d, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>Minimum gap between the leading and trailing groups; half of it separates the rows when they stack.</summary>
    public double Spacing { get => (double)GetValue(SpacingProperty); set => SetValue(SpacingProperty, value); }

    /// <summary>Whether the trailing commands currently sit on their own row.</summary>
    public bool IsStacked { get; private set; }

    private UIElement? Leading => InternalChildren.Count > 0 ? InternalChildren[0] : null;
    private UIElement? Trailing => InternalChildren.Count > 1 ? InternalChildren[1] : null;

    internal static bool FitsOnOneRow(double available, double leading, double trailing, double spacing) =>
        leading + (leading > 0 && trailing > 0 ? spacing : 0) + trailing <= available;

    protected override Size MeasureOverride(Size availableSize)
    {
        var natural = new Size(double.PositiveInfinity, availableSize.Height);
        Leading?.Measure(natural);
        Trailing?.Measure(natural);
        var leading = Leading?.DesiredSize ?? default;
        var trailing = Trailing?.DesiredSize ?? default;
        IsStacked = !double.IsPositiveInfinity(availableSize.Width) && !FitsOnOneRow(availableSize.Width, leading.Width, trailing.Width, Spacing);
        foreach (UIElement child in InternalChildren)
            if (child != Leading && child != Trailing) child.Measure(default);
        if (!IsStacked)
            return new Size(leading.Width + Spacing + trailing.Width, Math.Max(leading.Height, trailing.Height));
        var row = new Size(availableSize.Width, double.PositiveInfinity);
        Leading?.Measure(row);
        Trailing?.Measure(row);
        leading = Leading?.DesiredSize ?? default;
        trailing = Trailing?.DesiredSize ?? default;
        return new Size(availableSize.Width, leading.Height + Spacing / 2 + trailing.Height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var leading = Leading?.DesiredSize ?? default;
        var trailing = Trailing?.DesiredSize ?? default;
        var trailingWidth = Math.Min(finalSize.Width, trailing.Width);
        if (IsStacked)
        {
            Leading?.Arrange(new Rect(0, 0, Math.Min(finalSize.Width, leading.Width), leading.Height));
            Trailing?.Arrange(new Rect(finalSize.Width - trailingWidth, leading.Height + Spacing / 2, trailingWidth, trailing.Height));
        }
        else
        {
            Leading?.Arrange(new Rect(0, (finalSize.Height - leading.Height) / 2, leading.Width, leading.Height));
            Trailing?.Arrange(new Rect(finalSize.Width - trailingWidth, (finalSize.Height - trailing.Height) / 2, trailingWidth, trailing.Height));
        }
        foreach (UIElement child in InternalChildren)
            if (child != Leading && child != Trailing) child.Arrange(new Rect());
        return finalSize;
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new CommandBarAutomationPeer(this);

    private sealed class CommandBarAutomationPeer(CommandBarPanel owner) : FrameworkElementAutomationPeer(owner)
    {
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ToolBar;
        protected override string GetClassNameCore() => nameof(CommandBarPanel);
    }
}
