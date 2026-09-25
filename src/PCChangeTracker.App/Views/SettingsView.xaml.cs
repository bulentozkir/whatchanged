using System.Windows.Controls;

namespace PCChangeTracker.App.Views;

/// <summary>Settings and privacy page. Binds to <see cref="MainViewModel"/> through the inherited data context.</summary>
public partial class SettingsView : UserControl
{
    public SettingsView() => InitializeComponent();

    /// <summary>The page heading, focused when the page is opened from the keyboard.</summary>
    public TextBlock Heading => SettingsHeading;
}
