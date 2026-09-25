using System.Windows;
using System.Windows.Controls;

namespace PCChangeTracker.App.Controls;

/// <summary>
/// A settings section: an icon and heading, an optional muted description, and the section's controls.
/// Its template lives in Themes/Controls.xaml; the heading is exposed to assistive technology as a level-2 heading.
/// </summary>
public sealed class SettingsCard : HeaderedContentControl
{
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string),
        typeof(SettingsCard), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public string? Description { get => (string?)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }
}
