using System.Windows;
using System.Windows.Media;

namespace PCChangeTracker.App.Controls;

/// <summary>
/// Attaches a Segoe Fluent/MDL2 glyph (and optional tint) to buttons, navigation items, and cards whose templates
/// render it beside the label. The glyph is decorative: the visible label remains the accessible name.
/// </summary>
public static class Icon
{
    public static readonly DependencyProperty GlyphProperty = DependencyProperty.RegisterAttached("Glyph", typeof(string), typeof(Icon),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty BrushProperty = DependencyProperty.RegisterAttached("Brush", typeof(Brush), typeof(Icon),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static string? GetGlyph(DependencyObject element) => (string?)element.GetValue(GlyphProperty);
    public static void SetGlyph(DependencyObject element, string? value) => element.SetValue(GlyphProperty, value);

    /// <summary>Tint for the glyph; when unset the glyph uses the element's foreground.</summary>
    public static Brush? GetBrush(DependencyObject element) => (Brush?)element.GetValue(BrushProperty);
    public static void SetBrush(DependencyObject element, Brush? value) => element.SetValue(BrushProperty, value);
}
