using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace PCChangeTracker.App.Controls;

/// <summary>
/// A decorative icon-font glyph. It is hidden from UI Automation so screen readers announce only the visible label
/// beside it, never a private-use character. Size follows the text-size setting through the shared type ramp.
/// </summary>
public sealed class Glyph : TextBlock
{
    public Glyph()
    {
        SetResourceReference(FontFamilyProperty, "IconFontFamily");
        SetResourceReference(FontSizeProperty, "AppFont16");
        FontWeight = FontWeights.Normal;
        FontStyle = FontStyles.Normal;
        TextWrapping = TextWrapping.NoWrap;
        VerticalAlignment = VerticalAlignment.Center;
    }

    protected override AutomationPeer? OnCreateAutomationPeer() => null;
}
