using System.Windows;
using System.Windows.Media;

namespace PCChangeTracker.App;

/// <summary>
/// Owns the appearance model: the Light tokens declared in Themes/Colors.xaml, the Dark values for the same keys,
/// the user's named color choices, command-role colors, text size, and Windows high-contrast substitution.
/// </summary>
internal static class Appearance
{
    /// <summary>Theme used when no valid theme preference is saved.</summary>
    internal const string DefaultTheme = "Dark";
    internal static string ThemeOrDefault(string? theme) => theme is "Light" or "Dark" ? theme : DefaultTheme;

    internal static IReadOnlyList<int> TextSizePercentages { get; } = [100, 125, 150, 200];

    /// <summary>Command roles with "{Role}ButtonBackgroundBrush", "{Role}ButtonTextBrush", and "{Role}ButtonBorderBrush" tokens.</summary>
    internal static IReadOnlyList<string> ButtonRoles { get; } = ["Neutral", "Caution", "Danger"];

    /// <summary>Icon tints ("{Tone}IconBrush") that distinguish neutral commands without adding more fill colors.</summary>
    internal static IReadOnlyList<string> IconTones { get; } = ["Info", "Navigate", "Help"];

    internal static readonly IReadOnlyDictionary<string, string> DarkPaletteHex = new Dictionary<string, string>
    {
        ["CanvasBrush"] = "#111418", ["SurfaceBrush"] = "#1A1F26", ["SubtleBackgroundBrush"] = "#262E38", ["InputBackgroundBrush"] = "#1F252D",
        ["CardStrokeBrush"] = "#2E3743", ["TextBrush"] = "#F1F4F8", ["MutedBrush"] = "#BAC4CF", ["LabelBrush"] = "#A9C9F7",
        ["ReviewBrush"] = "#FFB86B", ["SuccessBrush"] = "#66D1A8", ["LineBrush"] = "#7A8898",
        ["AccentBrush"] = "#8AB4F8", ["AccentTextBrush"] = "#081A36",
        ["SelectionBackgroundBrush"] = "#1A2D4A", ["SelectionTextBrush"] = "#E6EFFD", ["SelectionAccentBrush"] = "#8AB4F8",
        ["BrandBrush"] = "#08675F", ["BrandTextBrush"] = "#FFFFFF",
        ["ButtonTextBrush"] = "#F1F4F8", ["ButtonBackgroundBrush"] = "#1A1F26",
        ["NeutralButtonBackgroundBrush"] = "#252C35", ["NeutralButtonTextBrush"] = "#F1F4F8", ["NeutralButtonBorderBrush"] = "#7A8898",
        ["CautionButtonBackgroundBrush"] = "#3A2A10", ["CautionButtonTextBrush"] = "#FFD08A", ["CautionButtonBorderBrush"] = "#D9973A",
        ["DangerButtonBackgroundBrush"] = "#3E1619", ["DangerButtonTextBrush"] = "#FFB4AE", ["DangerButtonBorderBrush"] = "#E5676A",
        ["InfoIconBrush"] = "#8AB4F8", ["NavigateIconBrush"] = "#CDB6FF", ["HelpIconBrush"] = "#F2D35C",
        ["ControlHoverOverlayBrush"] = "#1AFFFFFF", ["ControlPressedOverlayBrush"] = "#2EFFFFFF", ["ScrimOverlayBrush"] = "#99000000"
    };

    internal static readonly IReadOnlyDictionary<(string Theme, string Color), string> ButtonTextColorHex = new Dictionary<(string, string), string>
    {
        [("Light", "ButtonColorNavy")] = "#0B3D91", [("Light", "ButtonColorForest")] = "#1B5E20",
        [("Light", "ButtonColorMaroon")] = "#7A0C2E", [("Light", "ButtonColorPurple")] = "#4A148C",
        [("Dark", "ButtonColorNavy")] = "#8AB4FF", [("Dark", "ButtonColorForest")] = "#8FD39A",
        [("Dark", "ButtonColorMaroon")] = "#FFA6B3", [("Dark", "ButtonColorPurple")] = "#D2B3F5"
    };

    private static readonly IReadOnlyDictionary<(string Theme, string Color), string> ButtonBackgroundColorHex = new Dictionary<(string, string), string>
    {
        [("Light", "ButtonColorNavy")] = "#EBF2FF", [("Light", "ButtonColorForest")] = "#EAF5EE",
        [("Light", "ButtonColorMaroon")] = "#FFF0F3", [("Light", "ButtonColorPurple")] = "#F5F0FF",
        [("Dark", "ButtonColorNavy")] = "#172536", [("Dark", "ButtonColorForest")] = "#18261E",
        [("Dark", "ButtonColorMaroon")] = "#302027", [("Dark", "ButtonColorPurple")] = "#282230"
    };

    /// <summary>Light values previewed by the Default color swatches; a test keeps them equal to Themes/Colors.xaml.</summary>
    internal const string LightTextHex = "#141A21";
    internal const string LightButtonBackgroundHex = "#FFFFFF";

    internal static Color ColorForChoice(string theme, string code, bool background = false)
    {
        var choices = background ? ButtonBackgroundColorHex : ButtonTextColorHex;
        if (choices.TryGetValue((theme, code), out var hex)) return Parse(hex);
        return Parse(theme == "Dark"
            ? DarkPaletteHex[background ? "NeutralButtonBackgroundBrush" : "TextBrush"]
            : background ? LightButtonBackgroundHex : LightTextHex);
    }

    /// <summary>Collects the declared color tokens, including those in merged dictionaries.</summary>
    internal static Dictionary<string, object> ColorTokens(ResourceDictionary resources)
    {
        var tokens = new Dictionary<string, object>();
        void Collect(ResourceDictionary dictionary)
        {
            foreach (var merged in dictionary.MergedDictionaries) Collect(merged);
            foreach (var key in dictionary.Keys.OfType<string>())
                if (dictionary[key] is SolidColorBrush brush) tokens[key] = brush;
        }
        Collect(resources);
        return tokens;
    }

    internal static Dictionary<string, object> BuildPalette(IReadOnlyDictionary<string, object> lightBrushes, string theme,
        string appTextColor = "ButtonColorDefault", string buttonTextColor = "ButtonColorDefault",
        string buttonColor = "ButtonColorDefault", string labelColor = "ButtonColorDefault")
    {
        var palette = new Dictionary<string, object>(lightBrushes);
        if (theme == "Dark")
            foreach (var pair in DarkPaletteHex) palette[pair.Key] = new SolidColorBrush(Parse(pair.Value));
        foreach (var (key, code) in new[] { ("TextBrush", appTextColor), ("ButtonTextBrush", buttonTextColor), ("LabelBrush", labelColor) })
            if (ButtonTextColorHex.ContainsKey((theme, code)))
                palette[key] = new SolidColorBrush(ColorForChoice(theme, code));
        if (ButtonBackgroundColorHex.ContainsKey((theme, buttonColor)))
        {
            palette["ButtonBackgroundBrush"] = new SolidColorBrush(ColorForChoice(theme, buttonColor, true));
            palette["AccentBrush"] = new SolidColorBrush(ColorForChoice(theme, buttonColor));
            palette["SelectionAccentBrush"] = palette["AccentBrush"];
        }
        // A custom button color applies uniformly to every command so the chosen, contrast-tested pair is what users see.
        if (ButtonBackgroundColorHex.ContainsKey((theme, buttonColor)) || ButtonTextColorHex.ContainsKey((theme, buttonTextColor)))
        {
            foreach (var role in ButtonRoles)
            {
                palette[role + "ButtonBackgroundBrush"] = palette["ButtonBackgroundBrush"];
                palette[role + "ButtonTextBrush"] = palette["ButtonTextBrush"];
                palette[role + "ButtonBorderBrush"] = palette["LineBrush"];
            }
            foreach (var tone in IconTones) palette[tone + "IconBrush"] = palette["ButtonTextBrush"];
        }
        return palette;
    }

    internal static void ApplyAccessibilityColors(ResourceDictionary resources, IReadOnlyDictionary<string, object> standard, bool highContrast)
    {
        foreach (var pair in standard)
            resources[pair.Key] = !highContrast ? pair.Value : pair.Key switch
            {
                "AccentBrush" or "SelectionBackgroundBrush" or "BrandBrush" => SystemColors.HighlightBrush,
                "AccentTextBrush" or "SelectionTextBrush" or "SelectionAccentBrush" or "BrandTextBrush" => SystemColors.HighlightTextBrush,
                "CanvasBrush" or "SurfaceBrush" => SystemColors.WindowBrush,
                _ when pair.Key.EndsWith("BackgroundBrush", StringComparison.Ordinal) => SystemColors.WindowBrush,
                _ when pair.Key.EndsWith("OverlayBrush", StringComparison.Ordinal) => Brushes.Transparent,
                _ => SystemColors.WindowTextBrush
            };
    }

    /// <summary>Type ramp sizes, in DIPs at 100%. Body text is 15; secondary text is never smaller than 14.</summary>
    internal static IReadOnlyList<int> FontSizes { get; } = [14, 15, 16, 17, 18, 19, 20, 22, 24, 26];

    internal static void ApplyTextSize(ResourceDictionary resources, int percentage)
    {
        if (!TextSizePercentages.Contains(percentage)) percentage = 100;
        foreach (var size in FontSizes)
            resources["AppFont" + size] = size * percentage / 100d;
        resources["SidebarWidth"] = new GridLength(SidebarWidth(percentage));
    }

    /// <summary>The sidebar grows at half the rate of the text so navigation labels break between words rather than inside them.</summary>
    internal static double SidebarWidth(int percentage) => 216 * (1 + (percentage / 100d - 1) / 2);

    private static Color Parse(string hex) => (Color)ColorConverter.ConvertFromString(hex);
}
