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
        ["CanvasBrush"] = "#14191C", ["SurfaceBrush"] = "#1E2528", ["SubtleBackgroundBrush"] = "#2A3439", ["InputBackgroundBrush"] = "#242C30",
        ["CardStrokeBrush"] = "#2E393E", ["TextBrush"] = "#F2F5F6", ["MutedBrush"] = "#B7C2C7", ["LabelBrush"] = "#F2F5F6",
        ["LineBrush"] = "#6B7D87", ["ReviewBrush"] = "#F2A65A", ["AccentBrush"] = "#1FBFAA", ["AccentTextBrush"] = "#04211D",
        ["ButtonTextBrush"] = "#F2F5F6", ["ButtonBackgroundBrush"] = "#1E2528",
        ["NeutralButtonBackgroundBrush"] = "#2A3338", ["NeutralButtonTextBrush"] = "#F2F5F6", ["NeutralButtonBorderBrush"] = "#7A8C96",
        ["CautionButtonBackgroundBrush"] = "#3A2A0F", ["CautionButtonTextBrush"] = "#FFD08A", ["CautionButtonBorderBrush"] = "#D9973A",
        ["DangerButtonBackgroundBrush"] = "#3D1518", ["DangerButtonTextBrush"] = "#FFB4AE", ["DangerButtonBorderBrush"] = "#E5676A",
        ["InfoIconBrush"] = "#8EC5FF", ["NavigateIconBrush"] = "#C8B1FF", ["HelpIconBrush"] = "#F2D95C",
        ["ControlHoverOverlayBrush"] = "#1AFFFFFF", ["ControlPressedOverlayBrush"] = "#2EFFFFFF"
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

    internal static Color ColorForChoice(string theme, string code, bool background = false)
    {
        var choices = background ? ButtonBackgroundColorHex : ButtonTextColorHex;
        var fallback = theme == "Dark" ? DarkPaletteHex[background ? "SurfaceBrush" : "TextBrush"] : background ? "#FFFFFF" : "#172126";
        return Parse(choices.TryGetValue((theme, code), out var hex) ? hex : fallback);
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
                "CanvasBrush" or "SurfaceBrush" => SystemColors.WindowBrush,
                _ when pair.Key.EndsWith("BackgroundBrush", StringComparison.Ordinal) => SystemColors.WindowBrush,
                _ when pair.Key.EndsWith("OverlayBrush", StringComparison.Ordinal) => Brushes.Transparent,
                "AccentBrush" => SystemColors.HighlightBrush,
                "AccentTextBrush" => SystemColors.HighlightTextBrush,
                _ => SystemColors.WindowTextBrush
            };
    }

    internal static void ApplyTextSize(ResourceDictionary resources, int percentage)
    {
        if (!TextSizePercentages.Contains(percentage)) percentage = 100;
        foreach (var size in new[] { 14, 16, 17, 18, 19, 20, 22, 24, 26 })
            resources["AppFont" + size] = size * percentage / 100d;
    }

    private static Color Parse(string hex) => (Color)ColorConverter.ConvertFromString(hex);
}
