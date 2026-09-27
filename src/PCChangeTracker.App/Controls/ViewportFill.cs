using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PCChangeTracker.App.Controls;

/// <summary>
/// When <see cref="IsActive"/>, measures its child against the visible height of the enclosing vertical
/// <see cref="ScrollViewer"/> instead of infinity, so a star-sized row (such as a list) fills the page and scrolls
/// internally. The height never drops below <see cref="MinimumEms"/> times the inherited font size: in a short window or
/// at large text sizes the page scrolls instead of clipping. When inactive, the child scrolls like ordinary content.
/// </summary>
public sealed class ViewportFill : Decorator
{
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool),
        typeof(ViewportFill), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty MinimumEmsProperty = DependencyProperty.Register(nameof(MinimumEms), typeof(double),
        typeof(ViewportFill), new FrameworkPropertyMetadata(24d, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(ViewportFill),
        new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

    private ScrollViewer? viewer;

    public ViewportFill()
    {
        Loaded += (_, _) => Attach();
        Unloaded += (_, _) => Detach();
    }

    public bool IsActive { get => (bool)GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }
    /// <summary>Smallest filled height, in multiples of the inherited font size.</summary>
    public double MinimumEms { get => (double)GetValue(MinimumEmsProperty); set => SetValue(MinimumEmsProperty, value); }
    public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }

    internal static double FillHeight(double viewportHeight, double minimumEms, double fontSize) => Math.Max(viewportHeight, minimumEms * fontSize);

    protected override Size MeasureOverride(Size constraint)
    {
        if (Child is null) return default;
        if (!IsActive || !double.IsPositiveInfinity(constraint.Height) || viewer is not { ViewportHeight: > 0 } scrolling)
        {
            Child.Measure(constraint);
            return Child.DesiredSize;
        }
        var height = FillHeight(scrolling.ViewportHeight, MinimumEms, FontSize);
        Child.Measure(new Size(constraint.Width, height));
        return new Size(Child.DesiredSize.Width, height);
    }

    private void Attach()
    {
        Detach();
        for (var parent = VisualTreeHelper.GetParent(this); parent is not null; parent = VisualTreeHelper.GetParent(parent))
            if (parent is ScrollViewer found) { viewer = found; break; }
        if (viewer is null) return;
        viewer.ScrollChanged += ViewportChanged;
        InvalidateMeasure();
    }

    private void Detach()
    {
        if (viewer is not null) viewer.ScrollChanged -= ViewportChanged;
        viewer = null;
    }

    private void ViewportChanged(object sender, ScrollChangedEventArgs eventArgs)
    {
        // Nested scroll viewers (the list itself) bubble ScrollChanged too; only the page viewport matters here.
        if (IsActive && ReferenceEquals(eventArgs.OriginalSource, viewer) && eventArgs.ViewportHeightChange != 0) InvalidateMeasure();
    }
}
