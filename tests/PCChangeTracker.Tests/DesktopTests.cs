using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Xml.Linq;
using PCChangeTracker.Core;
using Xunit;

namespace PCChangeTracker.Tests;

[CollectionDefinition("Desktop", DisableParallelization = true)]
public sealed class DesktopCollection;

[Collection("Desktop")]
public sealed class DesktopTests
{
    private static double Luminance(System.Windows.Media.Color color)
    {
        static double Linear(byte channel)
        {
            var value = channel / 255d;
            return value <= 0.04045 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
        }
        return 0.2126 * Linear(color.R) + 0.7152 * Linear(color.G) + 0.0722 * Linear(color.B);
    }

    private static double Contrast(System.Windows.Media.Color foreground, System.Windows.Media.Color background)
    {
        var first = Luminance(foreground);
        var second = Luminance(background);
        return (Math.Max(first, second) + 0.05) / (Math.Min(first, second) + 0.05);
    }

    private static System.Windows.Media.Color Hex(string value) => (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(value);

    [Fact]
    public void ApplicationExecutableContainsTheChangeTrackerIcon()
    {
        using var icon = System.Drawing.Icon.ExtractAssociatedIcon(FindExecutable());
        Assert.NotNull(icon);
        using var bitmap = icon.ToBitmap();
        AssertBrandedIcon(bitmap);
    }

    [Theory]
    [InlineData(16)]
    [InlineData(20)]
    [InlineData(24)]
    [InlineData(32)]
    [InlineData(40)]
    [InlineData(48)]
    [InlineData(64)]
    [InlineData(128)]
    [InlineData(256)]
    public void ApplicationIconFillsNativeSizes(int size)
    {
        using var stream = typeof(PCChangeTracker.App.App).Assembly.GetManifestResourceStream("PCChangeTracker.AppIcon.ico")!;
        var decoder = new System.Windows.Media.Imaging.IconBitmapDecoder(stream,
            System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);
        var frame = Assert.Single(decoder.Frames, frame => frame.PixelWidth == size && frame.PixelHeight == size);
        var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
        encoder.Frames.Add(frame);
        using var buffer = new MemoryStream();
        encoder.Save(buffer);
        buffer.Position = 0;
        using var bitmap = new System.Drawing.Bitmap(buffer);
        AssertBrandedIcon(bitmap);
        if (size is 16 or 32 or 48)
        {
            var directory = Path.Combine(FindRoot(), "artifacts", "screenshots");
            Directory.CreateDirectory(directory);
            bitmap.Save(Path.Combine(directory, $"application-icon-{size}.png"), ImageFormat.Png);
        }
    }

    private static void AssertBrandedIcon(System.Drawing.Bitmap bitmap)
    {
        var brandPixels = 0;
        var opaquePixels = 0;
        var left = bitmap.Width;
        var right = 0;
        var top = bitmap.Height;
        var bottom = 0;
        for (var vertical = 0; vertical < bitmap.Height; vertical++)
            for (var horizontal = 0; horizontal < bitmap.Width; horizontal++)
            {
                var pixel = bitmap.GetPixel(horizontal, vertical);
                if (pixel.A > 200)
                {
                    opaquePixels++;
                    left = Math.Min(left, horizontal);
                    right = Math.Max(right, horizontal);
                    top = Math.Min(top, vertical);
                    bottom = Math.Max(bottom, vertical);
                }
                if (pixel.A > 200 && Math.Abs(pixel.R - 8) < 18 && Math.Abs(pixel.G - 103) < 18 && Math.Abs(pixel.B - 95) < 18)
                    brandPixels++;
            }
        Assert.True(brandPixels >= 8, "The native icon does not contain the ChangeTracker artwork.");
        Assert.True(opaquePixels >= bitmap.Width * bitmap.Height * 0.55, "The icon has too much empty space.");
        Assert.True(right - left + 1 >= bitmap.Width * 0.85 && bottom - top + 1 >= bitmap.Height * 0.85,
            "The visible mark is too small for its icon canvas.");
    }

    [Fact]
    public void AccessiblePaletteMeetsTextAndControlContrastTargets()
    {
        var resources = XDocument.Load(Path.Combine(FindRoot(), "src", "PCChangeTracker.App", "App.xaml"));
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        XNamespace xaml = "http://schemas.microsoft.com/winfx/2006/xaml";
        var colors = resources.Descendants(presentation + "SolidColorBrush").ToDictionary(
            element => (string)element.Attribute(xaml + "Key")!,
            element => (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString((string)element.Attribute("Color")!));
        double ContrastByKey(string foreground, string background) => Contrast(colors[foreground], colors[background]);
        foreach (var background in new[] { "SurfaceBrush", "CanvasBrush" })
        {
            foreach (var foreground in new[] { "TextBrush", "MutedBrush", "ReviewBrush" })
                Assert.True(ContrastByKey(foreground, background) >= 7, $"Insufficient text contrast: {foreground}/{background}");
            Assert.True(ContrastByKey("LineBrush", background) >= 3, $"Insufficient control-boundary contrast: {background}");
        }
        Assert.True(ContrastByKey("AccentTextBrush", "AccentBrush") >= 4.5);
        Assert.DoesNotContain(resources.Descendants(presentation + "Style"), style => (string?)style.Attribute(xaml + "Key") == "ModeSegment");
    }

    [Theory]
    [InlineData(100, 1d)]
    [InlineData(125, 1.25d)]
    [InlineData(150, 1.5d)]
    [InlineData(200, 2d)]
    [InlineData(-1, 1d)]
    public void TextSizeScalesAllSharedFontRoles(int percentage, double scale)
    {
        var resources = new System.Windows.ResourceDictionary();
        PCChangeTracker.App.App.ApplyTextSize(resources, percentage);
        foreach (var size in new[] { 14, 16, 17, 18, 19, 20, 22, 24, 26 })
            Assert.Equal(size * scale, Assert.IsType<double>(resources["AppFont" + size]));
    }

    [Fact]
    public void CustomButtonTextColorsMeetContrastAgainstBothThemeSurfaces()
    {
        var lightSurface = Hex("#FFFFFF");
        var darkSurface = Hex(PCChangeTracker.App.App.DarkPaletteHex["SurfaceBrush"]);
        foreach (var pair in PCChangeTracker.App.App.ButtonTextColorHex)
        {
            var surface = pair.Key.Theme == "Dark" ? darkSurface : lightSurface;
            Assert.True(Contrast(Hex(pair.Value), surface) >= 4.5,
                $"Insufficient button text contrast: {pair.Key.Color} ({pair.Key.Theme}) against its surface");
        }
    }

    [Fact]
    public void EveryCustomColorCombinationPreservesContrastAndHighContrastOverrides()
    {
        var resources = XDocument.Load(Path.Combine(FindRoot(), "src", "PCChangeTracker.App", "App.xaml"));
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        XNamespace xaml = "http://schemas.microsoft.com/winfx/2006/xaml";
        var standard = resources.Descendants(presentation + "SolidColorBrush").ToDictionary(
            element => (string)element.Attribute(xaml + "Key")!,
            element => (object)new System.Windows.Media.SolidColorBrush(Hex((string)element.Attribute("Color")!)));
        var codes = new[] { "ButtonColorDefault", "ButtonColorNavy", "ButtonColorForest", "ButtonColorMaroon", "ButtonColorPurple" };
        foreach (var theme in new[] { "Light", "Dark" })
        foreach (var foreground in codes)
        foreach (var background in codes)
        {
            var palette = PCChangeTracker.App.App.BuildPalette(standard, theme, foreground, foreground, background, foreground);
            System.Windows.Media.Color Color(string key) => ((System.Windows.Media.SolidColorBrush)palette[key]).Color;
            foreach (var surface in new[] { "CanvasBrush", "SurfaceBrush" })
                foreach (var text in new[] { "TextBrush", "LabelBrush" })
                    Assert.True(Contrast(Color(text), Color(surface)) >= 7, $"{theme}: {foreground}/{surface}");
            Assert.True(Contrast(Color("ButtonTextBrush"), Color("ButtonBackgroundBrush")) >= 4.5, $"{theme}: {foreground}/{background}");
            Assert.True(Contrast(Color("LineBrush"), Color("ButtonBackgroundBrush")) >= 3, $"{theme}: button outline/{background}");
            Assert.True(Contrast(Color("AccentTextBrush"), Color("AccentBrush")) >= 4.5, $"{theme}: primary button/{background}");
            var applied = new System.Windows.ResourceDictionary();
            PCChangeTracker.App.App.ApplyAccessibilityColors(applied, palette, true);
            Assert.Same(System.Windows.SystemColors.WindowTextBrush, applied["TextBrush"]);
            Assert.Same(System.Windows.SystemColors.WindowTextBrush, applied["LabelBrush"]);
            Assert.Same(System.Windows.SystemColors.WindowTextBrush, applied["ButtonTextBrush"]);
            Assert.Same(System.Windows.SystemColors.WindowBrush, applied["ButtonBackgroundBrush"]);
        }
    }

    [Fact]
    public void HighContrastPaletteUsesSystemColorsAndRestoresOriginals()
    {
        var normal = new Dictionary<string, object>
        {
            ["TextBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black),
            ["LineBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray),
            ["SurfaceBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
            ["AccentBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Teal),
            ["AccentTextBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White)
        };
        var resources = new System.Windows.ResourceDictionary();
        PCChangeTracker.App.App.ApplyAccessibilityColors(resources, normal, true);
        Assert.Same(System.Windows.SystemColors.WindowTextBrush, resources["TextBrush"]);
        Assert.Same(System.Windows.SystemColors.WindowTextBrush, resources["LineBrush"]);
        Assert.Same(System.Windows.SystemColors.WindowBrush, resources["SurfaceBrush"]);
        Assert.Same(System.Windows.SystemColors.HighlightTextBrush, resources["AccentTextBrush"]);
        PCChangeTracker.App.App.ApplyAccessibilityColors(resources, normal, false);
        foreach (var pair in normal) Assert.Same(pair.Value, resources[pair.Key]);
    }

    [Fact]
    [Trait("Category", "Desktop")]
    public void DateComparisonAndOfflineHelpWorkInSimpleMode()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerUi", Guid.NewGuid().ToString("N"));
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        store.SetPreference("schedule.interval", "Off");
        store.SetPreference("retention", "Forever");
        store.SetPreference("collection.scope", "CurrentUser");
        var first = Seed(0, false) with { Scope = CollectionScope.CurrentUser };
        var second = Seed(0, true) with
        {
            Scope = CollectionScope.CurrentUser,
            StartedAt = first.StartedAt.AddHours(2), FinishedAt = first.FinishedAt.AddHours(2)
        };
        store.Save(first);
        store.Save(second);
        store.Save(Seed(1, false) with { Scope = CollectionScope.CurrentUser });
        store.Rename(first.Id, "Before setup");
        store.Rename(second.Id, "After setup");
        var summaries = store.List();
        var start = new ProcessStartInfo(FindExecutable()) { UseShellExecute = false };
        start.ArgumentList.Add("--data-dir");
        start.ArgumentList.Add(directory);
        using var process = Process.Start(start)!;
        try
        {
            Assert.True(process.WaitForInputIdle(15000));
            var window = WaitForWindow(process.Id, "ChangeTracker");
            Assert.Equal(WindowVisualState.Maximized, ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Current.WindowVisualState);
            Assert.True(((SelectionItemPattern)FindId(window, "SimpleMode").GetCurrentPattern(SelectionItemPattern.Pattern)).Current.IsSelected);
            var texts = new PCChangeTracker.App.Localization.UiText("en");
            var beforePicker = FindId(window, "BeforeSnapshot");
            Assert.Equal(ControlType.ComboBox, beforePicker.Current.ControlType);
            if (beforePicker.TryGetCurrentPattern(ValuePattern.Pattern, out var valuePattern))
                Assert.True(((ValuePattern)valuePattern).Current.IsReadOnly);
            SelectSnapshot(window, "BeforeSnapshot", texts.Snapshot(summaries.Single(snapshot => snapshot.Id == first.Id), true));
            ((SelectionItemPattern)FindId(window, "CompareSaved").GetCurrentPattern(SelectionItemPattern.Pattern)).Select();
            SelectSnapshot(window, "AfterSnapshot", texts.Snapshot(summaries.Single(snapshot => snapshot.Id == second.Id), true));
            Assert.True(SpinWait.SpinUntil(() => FindId(window, "CheckNow").Current.Name == "Compare snapshots", 5000));
            var screenshots = Path.Combine(FindRoot(), "artifacts", "screenshots");
            Directory.CreateDirectory(screenshots);
            Screenshot(window, Path.Combine(screenshots, "comparison-dates.png"));
            InvokeId(window, "CheckNow");
            WaitForText(window, "Custom comparison. The baseline has not changed.");
            Assert.Equal(3, store.List().Count);
            Assert.Equal(first.Id, store.GetBaselineId(CollectionScope.CurrentUser));
            ((SelectionItemPattern)FindId(window, "AdvancedMode").GetCurrentPattern(SelectionItemPattern.Pattern)).Select();
            ((ExpandCollapsePattern)FindId(window, "ComparisonOptions").GetCurrentPattern(ExpandCollapsePattern.Pattern)).Expand();
            AssertSelectedSnapshot(window, "BeforeSnapshot", texts.Snapshot(summaries.Single(snapshot => snapshot.Id == first.Id), true));
            AssertSelectedSnapshot(window, "AfterSnapshot", texts.Snapshot(summaries.Single(snapshot => snapshot.Id == second.Id), true));
            var day = first.FinishedAt.ToLocalTime().Date.ToString("D", texts.Culture);
            SelectSnapshot(window, "BeforeDate", day);
            SelectSnapshot(window, "AfterDate", day);
            var filteredPicker = FindId(window, "BeforeSnapshot");
            var filteredPopup = (ExpandCollapsePattern)filteredPicker.GetCurrentPattern(ExpandCollapsePattern.Pattern);
            filteredPopup.Expand();
            Assert.Equal(2, filteredPicker.FindAll(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem)).Count);
            filteredPopup.Collapse();
            SelectSnapshot(window, "BeforeSnapshot", texts.Snapshot(summaries.Single(snapshot => snapshot.Id == first.Id), true));
            SelectSnapshot(window, "AfterSnapshot", texts.Snapshot(summaries.Single(snapshot => snapshot.Id == second.Id), true));
            Screenshot(window, Path.Combine(screenshots, "advanced-comparison-dates.png"));
            ((SelectionItemPattern)FindId(window, "SimpleMode").GetCurrentPattern(SelectionItemPattern.Pattern)).Select();
            AssertSelectedSnapshot(window, "BeforeSnapshot", texts.Snapshot(summaries.Single(snapshot => snapshot.Id == first.Id), true));
            AssertSelectedSnapshot(window, "AfterSnapshot", texts.Snapshot(summaries.Single(snapshot => snapshot.Id == second.Id), true));
            InvokeId(window, "CheckNow");
            WaitForText(window, "Custom comparison. The baseline has not changed.");
            ((ExpandCollapsePattern)FindId(window, "ComparisonOptions").GetCurrentPattern(ExpandCollapsePattern.Pattern)).Collapse();
            AutomationElement? routineGroup = null;
            Assert.True(SpinWait.SpinUntil(() =>
            {
                routineGroup = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "RoutineGroup"));
                return routineGroup is not null;
            }, 5000), "The routine-activity group did not appear after the comparison completed.");
            Assert.Contains("Routine / expected activity", routineGroup!.Current.Name);

            InvokeId(window, "HelpMe");
            var help = WaitForWindow(process.Id, "Help me - ChangeTracker");
            var search = (ValuePattern)FindId(help, "HelpSearch").GetCurrentPattern(ValuePattern.Pattern);
            search.SetValue("DPAPI");
            WaitForText(help, "1 topic");
            WaitForText(help, "Privacy And Storage");
            search.SetValue("snapshot frequency");
            WaitForText(help, "1 topic");
            WaitForText(help, "Settings");
            Screenshot(help, Path.Combine(screenshots, "offline-help-settings.png"));
            search.SetValue("not-a-real-help-term-123");
            WaitForText(help, "0 topics");
            search.SetValue("presentation modes");
            WaitForText(help, "Simple And Advanced");
            Screenshot(help, Path.Combine(screenshots, "offline-help.png"));
            var zoom = FindId(help, "HelpTextSize");
            ((RangeValuePattern)zoom.GetCurrentPattern(RangeValuePattern.Pattern)).SetValue(140);
            ((TransformPattern)help.GetCurrentPattern(TransformPattern.Pattern)).Resize(820, 640);
            Assert.False(FindId(help, "HelpSearch").Current.IsOffscreen);
            Assert.False(FindId(help, "CloseHelp").Current.IsOffscreen);
            Screenshot(help, Path.Combine(screenshots, "offline-help-large-text.png"));
            InvokeId(help, "CloseHelp");
            Assert.Equal(3, store.List().Count);
            window = WaitForWindow(process.Id, "ChangeTracker");
            AutomationElement? comparisonOptions = null;
            Assert.True(SpinWait.SpinUntil(() =>
            {
                comparisonOptions = window.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "ComparisonOptions"));
                return comparisonOptions is not null && comparisonOptions.Current.IsEnabled;
            }, 5000), "The main window did not become available after closing help.");
            ((ExpandCollapsePattern)comparisonOptions!.GetCurrentPattern(ExpandCollapsePattern.Pattern)).Expand();
            ((SelectionItemPattern)FindId(window, "CompareToday").GetCurrentPattern(SelectionItemPattern.Pattern)).Select();
            Assert.True(SpinWait.SpinUntil(() => FindId(window, "CheckNow").Current.Name == "Check now", 5000));
            ResizeWindow(window, 960, 720);
            Assert.False(FindId(window, "BeforeSnapshot").Current.IsOffscreen);
            Assert.False(FindId(window, "CheckNow").Current.IsOffscreen);
            Screenshot(window, Path.Combine(screenshots, "comparison-small-window.png"));
            ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Close();
            ExitFromTray(process);
        }
        finally
        {
            if (!process.HasExited) { process.Kill(true); process.WaitForExit(10000); }
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Fact]
    [Trait("Category", "Desktop")]
    public void LanguageSelectionPersistsAcrossRestartWithoutAffectingCaptureState()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerUi", Guid.NewGuid().ToString("N"));
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        store.SetPreference("collection.scope", "CurrentUser");
        var baseline = Seed(0, false) with { Scope = CollectionScope.CurrentUser };
        store.Save(baseline);
        store.SetBaseline(baseline.Id);
        var info = new ProcessStartInfo(FindExecutable()) { UseShellExecute = false };
        info.ArgumentList.Add("--data-dir");
        info.ArgumentList.Add(directory);
        using var process = Process.Start(info)!;
        try
        {
            Assert.True(process.WaitForInputIdle(15000));
            var window = WaitForWindow(process.Id, "ChangeTracker");
            InvokeId(window, "SettingsPage");
            var picker = FindId(window, "LanguagePicker");
            Assert.Equal("Language", picker.Current.Name);
            var expand = (ExpandCollapsePattern)picker.GetCurrentPattern(ExpandCollapsePattern.Pattern);
            expand.Expand();
            var options = picker.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem));
            Assert.Equal(20, options.Count);
            var spanish = options.Cast<AutomationElement>().Single(option => option.Current.Name == "Español (Spanish)");
            ((SelectionItemPattern)spanish.GetCurrentPattern(SelectionItemPattern.Pattern)).Select();
            expand.Collapse();
            WaitForText(window, "Revisar cambios");
            Assert.Equal("es", store.GetPreference("language"));
            Assert.Equal(baseline.Id, store.GetBaselineId(CollectionScope.CurrentUser));
            Assert.Single(store.List());
            var screenshots = Path.Combine(FindRoot(), "artifacts", "screenshots");
            Directory.CreateDirectory(screenshots);
            Screenshot(window, Path.Combine(screenshots, "language-spanish.png"));
            ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Close();
            ExitFromTray(process, "es");
        }
        finally
        {
            if (!process.HasExited) { process.Kill(true); process.WaitForExit(10000); }
        }
        Assert.Equal(baseline.Id, store.GetBaselineId(CollectionScope.CurrentUser));
        Assert.Single(store.List());
        var restart = new ProcessStartInfo(FindExecutable()) { UseShellExecute = false };
        restart.ArgumentList.Add("--data-dir");
        restart.ArgumentList.Add(directory);
        using var reopened = Process.Start(restart)!;
        try
        {
            Assert.True(reopened.WaitForInputIdle(15000));
            var window = WaitForWindow(reopened.Id, "ChangeTracker");
            WaitForText(window, "Revisar cambios");
            Assert.Equal("es", store.GetPreference("language"));
            Assert.Equal(baseline.Id, store.GetBaselineId(CollectionScope.CurrentUser));
            Assert.Single(store.List());
            ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Close();
            ExitFromTray(reopened, "es");
        }
        finally
        {
            if (!reopened.HasExited) { reopened.Kill(true); reopened.WaitForExit(10000); }
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Fact]
    [Trait("Category", "Desktop")]
    public void SidebarStorageRemainsVisibleWithRetentionTooltip()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerUi", Guid.NewGuid().ToString("N"));
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        store.SetPreference("collection.scope", "Both");
        store.SetPreference("schedule.interval", "Off");
        store.SetPreference("retention", "Forever");
        store.Save(Seed(0, false) with { Scope = CollectionScope.CurrentUser });
        store.Save(Seed(1, true) with { Scope = CollectionScope.Machine });
        var start = new ProcessStartInfo(FindExecutable()) { UseShellExecute = false };
        start.ArgumentList.Add("--data-dir");
        start.ArgumentList.Add(directory);
        using var process = Process.Start(start)!;
        var originalPointer = System.Windows.Forms.Cursor.Position;
        System.Drawing.Point? hoverPoint = null;
        try
        {
            Assert.True(process.WaitForInputIdle(15000));
            var window = WaitForWindow(process.Id, "ChangeTracker");
            var texts = new PCChangeTracker.App.Localization.UiText("en");
            foreach (var page in new[] { "ReviewPage", "HistoryPage", "SourcesPage", "SettingsPage" })
            {
                InvokeId(window, page);
                var label = FindId(window, "SnapshotStorage");
                Assert.False(label.Current.IsOffscreen);
                Assert.StartsWith("Snapshot storage: ", label.Current.Name);
                Assert.Equal(texts["SnapshotStorageHelp"], label.Current.HelpText);
                Assert.True(label.Current.BoundingRectangle.Bottom <= FindId(window, "HelpMe").Current.BoundingRectangle.Top);
            }
            ResizeWindow(window, 960, 720);
            var storage = FindId(window, "SnapshotStorage");
            var bounds = storage.Current.BoundingRectangle;
            Assert.True(bounds.Top > FindId(window, "SettingsPage").Current.BoundingRectangle.Bottom);
            Assert.True(bounds.Right <= FindId(window, "SettingsPage").Current.BoundingRectangle.Right + 1);
            storage.SetFocus();
            Assert.True(SpinWait.SpinUntil(() => storage.Current.HasKeyboardFocus, 5000));
            hoverPoint = new System.Drawing.Point((int)(bounds.Left + bounds.Width / 2), (int)(bounds.Top + bounds.Height / 2));
            System.Windows.Forms.Cursor.Position = hoverPoint.Value;
            var pointedElement = AutomationElement.FromPoint(new System.Windows.Point(hoverPoint.Value.X, hoverPoint.Value.Y));
            Assert.Equal(process.Id, pointedElement.Current.ProcessId);
            AutomationElement? tooltip = null;
            Assert.True(SpinWait.SpinUntil(() =>
            {
                var roots = AutomationElement.RootElement.FindAll(TreeScope.Children,
                    new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id)).Cast<AutomationElement>();
                var condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolTip);
                tooltip = roots.Select(root => root.Current.ControlType == ControlType.ToolTip ? root : root.FindFirst(TreeScope.Descendants, condition))
                    .FirstOrDefault(candidate => candidate is not null && !candidate.Current.IsOffscreen);
                return tooltip is not null && !tooltip.Current.IsOffscreen;
            }, 5000), $"The snapshot storage tooltip did not open on hover; hit={pointedElement.Current.AutomationId}; pointer={System.Windows.Forms.Cursor.Position}; target={hoverPoint}.");
            Assert.True(tooltip!.Current.Name == texts["SnapshotStorageHelp"] || tooltip.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, texts["SnapshotStorageHelp"])) is not null);
            var screenshots = Path.Combine(FindRoot(), "artifacts", "screenshots");
            Directory.CreateDirectory(screenshots);
            Screenshot(window, Path.Combine(screenshots, "snapshot-storage-sidebar.png"));
            if (tooltip.Current.NativeWindowHandle != 0) Screenshot(tooltip, Path.Combine(screenshots, "snapshot-storage-tooltip.png"));
            Assert.Equal(2, store.List().Count);
            ExitFromTray(process);
        }
        finally
        {
            if (hoverPoint == System.Windows.Forms.Cursor.Position) System.Windows.Forms.Cursor.Position = originalPointer;
            if (!process.HasExited) { process.Kill(true); process.WaitForExit(10000); }
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Fact]
    [Trait("Category", "Desktop")]
    public void SettingsMenuAppliesAppearanceAndRestoresFromTrayWithoutCollecting()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerUi", Guid.NewGuid().ToString("N"));
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        store.SetPreference("collection.scope", "CurrentUser");
        store.SetPreference("startup.minimizeToTray", "False");
        store.SetPreference("schedule.lastRun.CurrentUser", DateTimeOffset.UtcNow.ToString("O"));
        store.SetPreference("retention.lastRun.Retain90Days", DateTimeOffset.UtcNow.ToString("O"));
        var baseline = Seed(0, false) with { Scope = CollectionScope.CurrentUser };
        store.Save(baseline);
        var start = new ProcessStartInfo(FindExecutable()) { UseShellExecute = false };
        start.ArgumentList.Add("--data-dir");
        start.ArgumentList.Add(directory);
        using var process = Process.Start(start)!;
        try
        {
            Assert.True(process.WaitForInputIdle(15000));
            var window = WaitForWindow(process.Id, "ChangeTracker");
            InvokeId(window, "SettingsPage");
            SelectSnapshot(window, "ThemePicker", "Dark");
            SelectSnapshot(window, "FontPicker", "Calibri");
            var texts = new PCChangeTracker.App.Localization.UiText("en");
            SelectSnapshot(window, "ButtonTextColorPicker", texts["ButtonColorForest"]);
            SelectSnapshot(window, "AppTextColorPicker", texts["ButtonColorNavy"]);
            SelectSnapshot(window, "ButtonColorPicker", texts["ButtonColorForest"]);
            SelectSnapshot(window, "LabelColorPicker", texts["ButtonColorMaroon"]);
            Assert.Equal("Dark", store.GetPreference("theme"));
            Assert.Equal("Calibri", store.GetPreference("font"));
            Assert.Equal("ButtonColorForest", store.GetPreference("buttonTextColor"));
            Assert.Equal("ButtonColorNavy", store.GetPreference("appTextColor"));
            Assert.Equal("ButtonColorForest", store.GetPreference("buttonColor"));
            Assert.Equal("ButtonColorMaroon", store.GetPreference("labelColor"));
            Assert.DoesNotContain(window.FindAll(TreeScope.Descendants, Condition.TrueCondition).Cast<AutomationElement>(),
                element => element.Current.Name.Contains("NamedOption {", StringComparison.Ordinal));
            var screenshots = Path.Combine(FindRoot(), "artifacts", "screenshots");
            Directory.CreateDirectory(screenshots);
            Screenshot(window, Path.Combine(screenshots, "settings-dark.png"));
            SelectSnapshot(window, "FrequencyPicker", texts["EveryWeek"]);
            SelectSnapshot(window, "RetentionPicker", texts["Retain90Days"]);
            Assert.Equal("EveryWeek", store.GetPreference("schedule.interval"));
            Assert.Equal("Retain90Days", store.GetPreference("retention"));
            Assert.NotNull(FindId(window, "StartWithWindows"));
            InvokeId(window, "Report");
            var report = WaitForWindow(process.Id, "Report preview - ChangeTracker");
            var preview = report.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Sanitized report preview"));
            var textPattern = (TextPattern)preview.GetCurrentPattern(TextPattern.Pattern);
            Assert.Equal("Calibri", textPattern.DocumentRange.GetAttributeValue(TextPattern.FontNameAttribute));
            Invoke(report, "Close");
            Assert.True(SpinWait.SpinUntil(() => window.Current.IsEnabled, 5000));
            Assert.Null(window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "MinimizeToTray")));
            ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Close();
            Assert.True(SpinWait.SpinUntil(() => { process.Refresh(); return process.MainWindowHandle == IntPtr.Zero; }, 5000));
            Assert.False(process.HasExited);
            using (var activate = Process.Start(start)!)
                Assert.True(activate.WaitForExit(10000));
            window = WaitForWindow(process.Id, "ChangeTracker");
            Assert.Equal("False", store.GetPreference("startup.minimizeToTray"));
            SelectSnapshot(window, "ThemePicker", "Light");
            Assert.Equal("Light", store.GetPreference("theme"));
            ResizeWindow(window, 960, 720);
            Assert.True(((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).WaitForInputIdle(5000));
            var settings = FindId(window, "SettingsContent");
            var scrollbar = settings.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ScrollBar));
            var rightEdge = scrollbar?.Current.BoundingRectangle.Left ?? settings.Current.BoundingRectangle.Right;
            foreach (var pickerId in new[] { "AppTextColorPicker", "LabelColorPicker", "ButtonColorPicker", "ButtonTextColorPicker" })
                Assert.True(FindId(window, pickerId).Current.BoundingRectangle.Right <= rightEdge + 1, "Clipped selector: " + pickerId);
            Screenshot(window, Path.Combine(screenshots, "settings-light-small.png"));
            Assert.Single(store.List());
            Assert.Equal(baseline.Id, store.GetBaselineId(CollectionScope.CurrentUser));
            ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Close();
            ExitFromTray(process);
        }
        finally
        {
            if (!process.HasExited) { process.Kill(true); process.WaitForExit(10000); }
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("False")]
    [InlineData("True")]
    [Trait("Category", "Desktop")]
    public void HiddenStartupAndMinimizeKeepRunningUntilTrayExit(string? previousTrayChoice)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerUi", Guid.NewGuid().ToString("N"));
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        store.SetPreference("collection.scope", "CurrentUser");
        store.SetPreference("schedule.interval", "Off");
        if (previousTrayChoice is not null) store.SetPreference("startup.minimizeToTray", previousTrayChoice);
        var start = new ProcessStartInfo(FindExecutable()) { UseShellExecute = false };
        start.ArgumentList.Add("--data-dir");
        start.ArgumentList.Add(directory);
        start.ArgumentList.Add("--start-minimized");
        using var process = Process.Start(start)!;
        try
        {
            Assert.True(process.WaitForInputIdle(15000));
            Assert.NotEqual(IntPtr.Zero, FindTrayWindow(process));
            process.Refresh();
            Assert.Equal(IntPtr.Zero, process.MainWindowHandle);
            Assert.False(process.HasExited);
            InvokeTrayCommand(process, new PCChangeTracker.App.Localization.UiText("en")["TrayOpen"]);
            var window = WaitForWindow(process.Id, "ChangeTracker");
            Assert.Equal(WindowVisualState.Maximized, ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Current.WindowVisualState);
            var handle = (nint)window.Current.NativeWindowHandle;
            Assert.NotEqual(0, GetWindowLong(handle, -20) & 0x40000);
            Assert.NotEqual(IntPtr.Zero, SendMessageTimeout(handle, 0x7F, 1, 0, 2, 5000, out var windowIcon));
            Assert.NotEqual(IntPtr.Zero, windowIcon);
            using (var icon = System.Drawing.Icon.FromHandle(windowIcon))
            using (var bitmap = icon.ToBitmap()) AssertBrandedIcon(bitmap);
            ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).SetWindowVisualState(WindowVisualState.Minimized);
            Assert.True(SpinWait.SpinUntil(() => { process.Refresh(); return process.MainWindowHandle == IntPtr.Zero; }, 5000));
            Assert.False(process.HasExited);
            var trayWindow = FindTrayWindow(process);
            Assert.True(PostMessage(trayWindow, 0x800, 1, 0x203));
            window = WaitForWindow(process.Id, "ChangeTracker");
            Assert.Equal(WindowVisualState.Maximized, ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Current.WindowVisualState);
            ResizeWindow(window, 1000, 740);
            var restoredSize = window.Current.BoundingRectangle.Size;
            ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).SetWindowVisualState(WindowVisualState.Minimized);
            Assert.True(SpinWait.SpinUntil(() => { process.Refresh(); return process.MainWindowHandle == IntPtr.Zero; }, 5000));
            InvokeTrayCommand(process, new PCChangeTracker.App.Localization.UiText("en")["TrayOpen"]);
            window = WaitForWindow(process.Id, "ChangeTracker");
            Assert.Equal(WindowVisualState.Normal, ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Current.WindowVisualState);
            Assert.Equal(restoredSize, window.Current.BoundingRectangle.Size);
            ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Close();
            Assert.True(SpinWait.SpinUntil(() => { process.Refresh(); return process.MainWindowHandle == IntPtr.Zero; }, 5000));
            Assert.False(process.HasExited);
            Assert.Empty(store.List());
            ExitFromTray(process);
        }
        finally
        {
            if (!process.HasExited) { process.Kill(true); process.WaitForExit(10000); }
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void OfflineHelpIsEmbeddedAndSearchesFullTopicText()
    {
        Assert.Equal(File.ReadAllText(Path.Combine(FindRoot(), "Helpme.md")), PCChangeTracker.App.HelpContent.ReadSource());
        var topics = PCChangeTracker.App.HelpContent.LoadTopics();
        Assert.True(topics.Count >= 15);
        Assert.Contains(topics, topic => topic.Title == "Settings");
        Assert.Contains(PCChangeTracker.App.HelpContent.Search(topics, "retention"), topic => topic.Title == "Settings");
        Assert.Contains(topics, topic => topic.Title == "Simple And Advanced");
        Assert.Contains(topics, topic => topic.Title == "Compare Two Saved Snapshots");
        Assert.Contains(topics, topic => topic.Title == "Compare A Snapshot With Today");
        Assert.Contains(PCChangeTracker.App.HelpContent.Search(topics, "uac"), topic => topic.Title == "Administrator Access");
        Assert.Contains(PCChangeTracker.App.HelpContent.Search(topics, "DPAPI"), topic => topic.Title == "Privacy And Storage");
        foreach (var term in new[] { "Snapshot storage", "history.db-wal", "KiB", "Size unavailable" })
            Assert.Contains(PCChangeTracker.App.HelpContent.Search(topics, term), topic => topic.Title == "Privacy And Storage");
        Assert.Contains(PCChangeTracker.App.HelpContent.Search(topics, "not backfilled"), topic => topic.Title == "Collection Sources And Limits");
        Assert.Contains(PCChangeTracker.App.HelpContent.Search(topics, "Storage size stays large"), topic => topic.Title == "Troubleshooting");
        foreach (var term in new[] { "F6", "Ctrl+4", "200%", "Screen-Reader Information" })
            Assert.Contains(PCChangeTracker.App.HelpContent.Search(topics, term), topic => topic.Title == "Accessibility And Keyboard Use");
        Assert.Empty(PCChangeTracker.App.HelpContent.Search(topics, "not-a-real-help-term-123"));
        Assert.Equal(topics.Count, PCChangeTracker.App.HelpContent.Search(topics, "").Count);
    }

    [Theory]
    [InlineData("zz-ZZ")]
    [InlineData("../../history.db")]
    public void OfflineHelpFallsBackSafelyForUnavailableLanguages(string languageCode)
    {
        Assert.Equal(PCChangeTracker.App.HelpContent.ReadSource(), PCChangeTracker.App.HelpContent.ReadSource(languageCode));
    }

    [Theory]
    [InlineData("zh-hans")]
    [InlineData("hi")]
    [InlineData("es")]
    [InlineData("ar")]
    [InlineData("fr")]
    [InlineData("bn")]
    [InlineData("pt")]
    [InlineData("id")]
    [InlineData("ur")]
    [InlineData("ru")]
    [InlineData("de")]
    [InlineData("ja")]
    [InlineData("pcm")]
    [InlineData("arz")]
    [InlineData("mr")]
    [InlineData("vi")]
    [InlineData("te")]
    [InlineData("ha")]
    [InlineData("tr")]
    public void OfflineHelpHasDistinctTranslatedGuideWithMatchingTopicCoverage(string languageCode)
    {
        var english = PCChangeTracker.App.HelpContent.ReadSource();
        var translated = PCChangeTracker.App.HelpContent.ReadSource(languageCode);
        Assert.Equal(File.ReadAllText(Path.Combine(FindRoot(), "help", languageCode + ".md")), translated);
        Assert.NotEqual(english, translated);
        var englishTopics = PCChangeTracker.App.HelpContent.LoadTopics();
        var translatedTopics = PCChangeTracker.App.HelpContent.LoadTopics(languageCode);
        Assert.Equal(englishTopics.Count, translatedTopics.Count);
        var texts = new PCChangeTracker.App.Localization.UiText(languageCode);
        Assert.Contains(translatedTopics, topic => topic.Title == texts["NavSettings"]);
        Assert.Contains("DPAPI", translated);
        Assert.Contains("JSON/CSV", translated);
        Assert.Contains("allowElevation", translated);
        var storageTopic = Assert.Single(PCChangeTracker.App.HelpContent.Search(translatedTopics, "history.db-wal"));
        Assert.Contains(PCChangeTracker.App.HelpContent.Search(translatedTopics, "DPAPI"), topic => topic.Title == storageTopic.Title);
        Assert.Contains("history.db-shm", translated);
        Assert.Contains("KiB", translated);
        var accessibilityTopic = Assert.Single(PCChangeTracker.App.HelpContent.Search(translatedTopics, "Ctrl+4"));
        foreach (var term in new[] { "F6", "200%" })
            Assert.Contains(PCChangeTracker.App.HelpContent.Search(translatedTopics, term), topic => topic.Title == accessibilityTopic.Title);
    }

    [Fact]
    public void OfflineHelpRendersNativeHeadingsTablesAndLists()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var topics = PCChangeTracker.App.HelpContent.LoadTopics();
                var modes = PCChangeTracker.App.HelpContent.Render(topics.Single(topic => topic.Title == "Simple And Advanced"));
                Assert.Contains(modes.Blocks.Cast<System.Windows.Documents.Block>(), block => block is System.Windows.Documents.Table);
                Assert.Equal(26, Assert.IsType<System.Windows.Documents.Paragraph>(modes.Blocks.FirstBlock).FontSize);
                var text = new System.Windows.Documents.TextRange(modes.ContentStart, modes.ContentEnd).Text;
                Assert.Contains("JSON and CSV", text);
                Assert.DoesNotContain("| ---", text);
                Assert.Equal(System.Windows.TextAlignment.Left, modes.TextAlignment);
                var compact = PCChangeTracker.App.HelpContent.Render(topics.Single(topic => topic.Title == "Simple And Advanced"), true);
                Assert.DoesNotContain(compact.Blocks.Cast<System.Windows.Documents.Block>(), block => block is System.Windows.Documents.Table);
                Assert.Contains("Advanced: ", new System.Windows.Documents.TextRange(compact.ContentStart, compact.ContentEnd).Text);
                var started = PCChangeTracker.App.HelpContent.Render(topics.Single(topic => topic.Title == "Getting Started"));
                Assert.Contains(started.Blocks.Cast<System.Windows.Documents.Block>(), block => block is System.Windows.Documents.List);
                var storage = PCChangeTracker.App.HelpContent.Render(topics.Single(topic => topic.Title == "Privacy And Storage"));
                Assert.Contains(storage.Blocks.Cast<System.Windows.Documents.Block>(), block => block is System.Windows.Documents.List);
                var storageText = new System.Windows.Documents.TextRange(storage.ContentStart, storage.ContentEnd).Text;
                Assert.Contains("Manage Disk Usage", storageText);
                Assert.Contains("history.db-wal", storageText);
                Assert.DoesNotContain("###", storageText);
            }
            catch (Exception exception) { failure = exception; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "Offline document rendering did not finish.");
        if (failure is not null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
    }

    [Fact]
    public void PackageManifestMatchesPartnerCenterIdentity()
    {
        var manifest = XDocument.Load(Path.Combine(FindRoot(), "packaging", "AppxManifest.xml"));
        XNamespace schema = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
        XNamespace visual = "http://schemas.microsoft.com/appx/manifest/uap/windows10";
        var package = manifest.Root!;
        Assert.Equal("BulentOzkir.ChangeTracker", (string?)package.Element(schema + "Identity")?.Attribute("Name"));
        Assert.Equal("CN=06D08AF4-6BB1-40DF-9B96-5DF27BEE0635", (string?)package.Element(schema + "Identity")?.Attribute("Publisher"));
        Assert.Equal("Bulent Ozkir", (string?)package.Element(schema + "Properties")?.Element(schema + "PublisherDisplayName"));
        Assert.Equal("ChangeTracker", (string?)package.Element(schema + "Properties")?.Element(schema + "DisplayName"));
        var application = package.Element(schema + "Applications")!.Element(schema + "Application")!;
        Assert.Equal("PCChangeTracker.exe", (string?)application.Attribute("Executable"));
        Assert.Equal("ChangeTracker", (string?)application.Element(visual + "VisualElements")?.Attribute("DisplayName"));
        XNamespace restricted = "http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities";
        Assert.Equal(new[] { "allowElevation", "runFullTrust" }, package.Element(schema + "Capabilities")!
            .Elements(restricted + "Capability").Select(capability => (string)capability.Attribute("Name")!).Order());
        var executableManifest = XDocument.Load(Path.Combine(FindRoot(), "src", "PCChangeTracker.App", "app.manifest"));
        XNamespace execution = "urn:schemas-microsoft-com:asm.v3";
        var level = Assert.Single(executableManifest.Descendants(execution + "requestedExecutionLevel"));
        Assert.Equal("asInvoker", (string?)level.Attribute("level"));
        Assert.Equal("false", (string?)level.Attribute("uiAccess"));
    }

    [Fact]
    [Trait("Category", "Desktop")]
    public void ManualCapturePersistsBaselineAndSecondCheck()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerUi", Guid.NewGuid().ToString("N"));
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        store.SetPreference("schedule.interval", "Off");
        var enabled = new[] { Category.Applications, Category.Startup, Category.DefaultApps, Category.Network, Category.Environment };
        foreach (var category in Enum.GetValues<Category>()) store.SetPreference("source." + category, enabled.Contains(category).ToString());
        var info = new ProcessStartInfo(FindExecutable()) { UseShellExecute = false };
        info.ArgumentList.Add("--data-dir");
        info.ArgumentList.Add(directory);
        using var process = Process.Start(info)!;
        try
        {
            Assert.True(process.WaitForInputIdle(15000));
            AutomationElement? window = null;
            Assert.True(SpinWait.SpinUntil(() =>
            {
                window = AutomationElement.RootElement.FindFirst(TreeScope.Children, new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id),
                    new PropertyCondition(AutomationElement.NameProperty, "ChangeTracker")));
                return window is not null;
            }, 15000));
            Assert.Empty(store.List());
            Assert.False(FindId(window!, "CheckNow").Current.IsEnabled);
            Assert.Equal(ToggleState.On, ((TogglePattern)FindId(window!, "CurrentUserScope").GetCurrentPattern(TogglePattern.Pattern)).Current.ToggleState);
            Assert.Equal(ToggleState.On, ((TogglePattern)FindId(window!, "MachineScope").GetCurrentPattern(TogglePattern.Pattern)).Current.ToggleState);
            ((TogglePattern)FindId(window!, "MachineScope").GetCurrentPattern(TogglePattern.Pattern)).Toggle();
            Assert.True(SpinWait.SpinUntil(() => ((TogglePattern)FindId(window!, "MachineScope").GetCurrentPattern(TogglePattern.Pattern)).Current.ToggleState == ToggleState.Off, 5000),
                "The machine scope checkbox did not switch off.");
            var screenshots = Path.Combine(FindRoot(), "artifacts", "screenshots");
            Directory.CreateDirectory(screenshots);
            Screenshot(window!, Path.Combine(screenshots, "scope-choice.png"));
            InvokeId(window!, "ConfirmScope");
            Assert.True(SpinWait.SpinUntil(() => store.GetPreference("collection.scope") == "CurrentUser", 5000),
                "Scope selection was not saved as CurrentUser: " + store.GetPreference("collection.scope"));
            Assert.True(SpinWait.SpinUntil(() => FindId(window!, "CheckNow").Current.IsEnabled, 5000));
            Assert.Empty(store.List());
            InvokeId(window!, "CheckNow");
            Assert.True(SpinWait.SpinUntil(() => store.List().Count == 1, 60000), "The first capture was not saved.");
            WaitForText(window!, "Baseline ready. No comparison yet.");
            var baselineId = store.GetBaselineId(CollectionScope.CurrentUser);
            Assert.True(baselineId.HasValue, "No current-user baseline; captured scopes: " + string.Join(", ", store.List().Select(saved => saved.Context)));
            var snapshot = store.Load(baselineId!.Value)!;
            Assert.Equal(CollectionScope.CurrentUser, snapshot.Scope);
            Assert.False(snapshot.Elevated);
            Assert.Null(store.BaselineId);
            Assert.All(snapshot.Results.Where(result => enabled.Contains(result.Category)), result => Assert.Equal(CollectionStatus.Success, result.Status));
            Assert.True(SpinWait.SpinUntil(() => FindId(window!, "CheckNow").Current.IsEnabled, 5000));
            InvokeId(window!, "CheckNow");
            Assert.True(SpinWait.SpinUntil(() => store.List().Count == 2, 60000), "The second capture was not saved.");
            Assert.Equal(baselineId, store.GetBaselineId(CollectionScope.CurrentUser));
            Assert.True(SpinWait.SpinUntil(() => FindId(window!, "CheckNow").Current.IsEnabled, 5000));
            InvokeId(window!, "CheckNow");
            Assert.True(SpinWait.SpinUntil(() => !FindId(window!, "CheckNow").Current.IsEnabled, 5000));
            ((WindowPattern)window!.GetCurrentPattern(WindowPattern.Pattern)).Close();
            Assert.True(SpinWait.SpinUntil(() => { process.Refresh(); return process.MainWindowHandle == IntPtr.Zero; }, 5000));
            Assert.False(process.HasExited);
            Assert.True(SpinWait.SpinUntil(() => store.List().Count == 3, 60000), "Closing the window interrupted the active capture.");
            Assert.Equal(baselineId, store.GetBaselineId(CollectionScope.CurrentUser));
            ExitFromTray(process);
            Assert.Equal(3, store.List().Count);
        }
        finally
        {
            if (!process.HasExited) { process.Kill(true); process.WaitForExit(10000); }
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Fact]
    [Trait("Category", "Desktop")]
    public void SwitchingToMachineScopeUsesStandardAccessAndANewBaseline()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerUi", Guid.NewGuid().ToString("N"));
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        store.SetPreference("collection.scope", "CurrentUser");
        foreach (var category in Enum.GetValues<Category>())
            store.SetPreference("source." + category, (category == Category.Applications).ToString());
        var info = new ProcessStartInfo(FindExecutable()) { UseShellExecute = false };
        info.ArgumentList.Add("--data-dir");
        info.ArgumentList.Add(directory);
        using var process = Process.Start(info)!;
        try
        {
            Assert.True(process.WaitForInputIdle(15000));
            AutomationElement? window = null;
            Assert.True(SpinWait.SpinUntil(() =>
            {
                window = AutomationElement.RootElement.FindFirst(TreeScope.Children, new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id),
                    new PropertyCondition(AutomationElement.NameProperty, "ChangeTracker")));
                return window is not null;
            }, 15000));
            InvokeId(window!, "CheckNow");
            Assert.True(SpinWait.SpinUntil(() => store.List().Count == 1, 30000));
            Assert.True(SpinWait.SpinUntil(() => FindId(window!, "CheckNow").Current.IsEnabled, 5000));
            var userBaseline = store.GetBaselineId(CollectionScope.CurrentUser);
            InvokeId(window!, "ChangeScope");
            ((TogglePattern)FindId(window!, "MachineScope").GetCurrentPattern(TogglePattern.Pattern)).Toggle();
            ((TogglePattern)FindId(window!, "CurrentUserScope").GetCurrentPattern(TogglePattern.Pattern)).Toggle();
            InvokeId(window!, "ConfirmScope");
            Assert.True(SpinWait.SpinUntil(() => store.GetPreference("collection.scope") == "Machine", 5000));
            Assert.Single(store.List());
            Assert.False(FindId(window!, "CheckAsAdministrator").Current.IsOffscreen);
            Assert.Null(store.GetBaselineId(CollectionScope.Machine));
            InvokeId(window!, "CheckNow");
            Assert.True(SpinWait.SpinUntil(() => store.List().Count == 2, 30000));
            Assert.True(SpinWait.SpinUntil(() => FindId(window!, "CheckNow").Current.IsEnabled, 5000));
            Assert.Equal(userBaseline, store.GetBaselineId(CollectionScope.CurrentUser));
            var machine = store.Load(store.GetBaselineId(CollectionScope.Machine)!.Value)!;
            Assert.Equal(CollectionScope.Machine, machine.Scope);
            Assert.False(machine.Elevated);
            Assert.Null(store.GetBaselineId(CollectionScope.Machine, true));
            Assert.All(machine.Results.Single(result => result.Category == Category.Applications).Items,
                item => Assert.Equal("Machine", item.Fields["Scope"]));
            ((WindowPattern)window!.GetCurrentPattern(WindowPattern.Pattern)).Close();
            ExitFromTray(process);
        }
        finally
        {
            if (!process.HasExited) { process.Kill(true); process.WaitForExit(10000); }
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Fact]
    [Trait("Category", "Desktop")]
    public void NativeWindowSupportsReviewModesAnnotationsAndReport()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerUi", Guid.NewGuid().ToString("N"));
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        store.SetPreference("collection.scope", "CurrentUser");
        var first = Seed(0, false);
        var second = Seed(1, true);
        store.Save(first);
        store.Save(second);
        store.Rename(first.Id, "Before setup");
        var rootPath = FindRoot();
        var executable = FindExecutable();
        var info = new ProcessStartInfo(executable) { UseShellExecute = false };
        info.ArgumentList.Add("--data-dir");
        info.ArgumentList.Add(directory);
        using var process = Process.Start(info)!;
        try
        {
            Assert.True(process.WaitForInputIdle(15000));
            AutomationElement? window = null;
            Assert.True(SpinWait.SpinUntil(() =>
            {
                window = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                    new AndCondition(new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id),
                        new PropertyCondition(AutomationElement.NameProperty, "ChangeTracker")));
                return window is not null;
            }, 15000), "The main window did not appear.");
            window!.SetFocus();
            WaitForText(window, "3 changes to review");
            var advanced = FindId(window, "AdvancedMode");
            Assert.Equal(ControlType.RadioButton, advanced.Current.ControlType);
            Assert.Equal(ControlType.RadioButton, FindId(window, "SimpleMode").Current.ControlType);
            Assert.Contains("JSON/CSV", advanced.Current.HelpText);
            Assert.NotNull(FindId(window, "BeforeSnapshot"));
            Assert.NotNull(FindId(window, "CompareToday"));
            ((ExpandCollapsePattern)FindId(window, "ComparisonOptions").GetCurrentPattern(ExpandCollapsePattern.Pattern)).Collapse();
            var screenshots = Path.Combine(rootPath, "artifacts", "screenshots");
            Directory.CreateDirectory(screenshots);
            var beforeValue = FindName(FindId(window, "SimpleChangeValues"), "Name, Before");
            var afterValue = FindName(FindId(window, "SimpleChangeValues"), "Name, After");
            Assert.True(((ValuePattern)beforeValue.GetCurrentPattern(ValuePattern.Pattern)).Current.IsReadOnly);
            Assert.Equal("Not present", ((ValuePattern)beforeValue.GetCurrentPattern(ValuePattern.Pattern)).Current.Value);
            Assert.Equal("Example Sync", ((ValuePattern)afterValue.GetCurrentPattern(ValuePattern.Pattern)).Current.Value);
            Screenshot(window, Path.Combine(screenshots, "review-simple-readable.png"));
            var inspect = FindId(window, "InspectChange");
            ((InvokePattern)inspect.GetCurrentPattern(InvokePattern.Pattern)).Invoke();
            var detail = FindId(window, "ChangeDetails");
            Assert.NotNull(FindName(detail, "Name, After"));
            Assert.False(FindId(window, "ReviewPage").Current.IsEnabled);
            Assert.False(FindId(window, "HelpMe").Current.IsEnabled);
            Assert.True(SpinWait.SpinUntil(() => FindId(window, "CloseChangeDetails").Current.HasKeyboardFocus, 5000));
            Screenshot(window, Path.Combine(screenshots, "change-details-simple.png"));
            InvokeId(window, "CloseChangeDetails");
            Assert.True(SpinWait.SpinUntil(() => inspect.Current.HasKeyboardFocus, 5000));
            var selection = (SelectionItemPattern)advanced.GetCurrentPattern(SelectionItemPattern.Pattern);
            Assert.False(selection.Current.IsSelected);
            selection.Select();
            Assert.True(SpinWait.SpinUntil(() => store.GetPreference("mode") == "advanced", 5000));

            Screenshot(window, Path.Combine(screenshots, "review-desktop.png"));

            var advancedFields = FindId(window, "AdvancedFields");
            Assert.Equal(ExpandCollapseState.Expanded, ((ExpandCollapsePattern)advancedFields.GetCurrentPattern(ExpandCollapsePattern.Pattern)).Current.ExpandCollapseState);
            Assert.NotNull(FindId(window, "AdvancedObservationMetadata"));
            WaitForText(window, "Record identity");
            var offender = window.FindAll(TreeScope.Descendants, Condition.TrueCondition).Cast<AutomationElement>()
                .FirstOrDefault(element => element.Current.Name.Contains("PRIVATE-HMAC"));
            Assert.True(offender is null, offender is null ? "" :
                $"LEAK in ControlType={offender.Current.ControlType.ProgrammaticName} AutomationId={offender.Current.AutomationId} ClassName={offender.Current.ClassName}\nFULL NAME: {offender.Current.Name}");
            Screenshot(window, Path.Combine(screenshots, "change-details-advanced.png"));

            Invoke(window, "Mark expected");
            WaitForText(window, "2 changes to review");
            Assert.Single(store.ExpectedChanges());

            InvokeId(window, "HistoryPage");
            Assert.NotNull(FindId(window, "ComparisonOptions"));
            Screenshot(window, Path.Combine(screenshots, "snapshots-desktop.png"));
            InvokeId(window, "ReviewPage");
            InvokeId(window, "Report");
            AutomationElement? reportWindow = null;
            Assert.True(SpinWait.SpinUntil(() =>
            {
                reportWindow = window.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.NameProperty, "Report preview - ChangeTracker"))
                    ?? AutomationElement.RootElement.FindFirst(TreeScope.Children,
                        new AndCondition(new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id),
                            new PropertyCondition(AutomationElement.NameProperty, "Report preview - ChangeTracker")));
                return reportWindow is not null;
            }, 5000));
            var preview = reportWindow!.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, "Sanitized report preview"));
            var reportText = ((ValuePattern)preview.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
            Assert.StartsWith("ChangeTracker" + Environment.NewLine, reportText);
            Assert.Contains("Snapshot comparison", reportText);
            Assert.Contains("Firewall", reportText);
            Assert.DoesNotContain("PRIVATE-HMAC", reportText);
            Invoke(reportWindow, "Close");

            Assert.True(SpinWait.SpinUntil(() => window.Current.IsEnabled, 5000));
            var originalWidth = window.Current.BoundingRectangle.Width;
            ResizeWindow(window, 980, 720);
            Assert.True(SpinWait.SpinUntil(() => window.Current.BoundingRectangle.Width < originalWidth, 5000),
                $"Original width {originalWidth}; resized width {window.Current.BoundingRectangle.Width}.");
            Assert.False(FindId(window, "CheckNow").Current.IsOffscreen);
            Assert.False(FindId(window, "AdvancedMode").Current.IsOffscreen);
            Screenshot(window, Path.Combine(screenshots, "review-small-window.png"));
            ((SelectionItemPattern)FindId(window, "SimpleMode").GetCurrentPattern(SelectionItemPattern.Pattern)).Select();
            Assert.True(SpinWait.SpinUntil(() => store.GetPreference("mode") == "simple", 5000));
            Assert.Equal(first.Id, store.BaselineId);
            Assert.Equal(2, store.List().Count);
            InvokeId(window, "ChangeScope");
            Assert.True(SpinWait.SpinUntil(() => FindId(window, "CurrentUserScope").Current.HasKeyboardFocus, 5000));
            Assert.False(FindId(window, "HelpMe").Current.IsEnabled);
            Assert.True(FindId(window, "ScopeHelpMe").Current.IsEnabled);
            InvokeId(window, "ConfirmScope");
            Assert.True(SpinWait.SpinUntil(() => FindId(window, "ChangeScope").Current.HasKeyboardFocus, 5000));
            Assert.True(FindId(window, "HelpMe").Current.IsEnabled);
            ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Close();
            ExitFromTray(process);
        }
        finally
        {
            if (!process.HasExited) { process.Kill(true); process.WaitForExit(10000); }
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    private static Snapshot Seed(int day, bool changed)
    {
        var start = new DateTimeOffset(2026, 9, 17 + day, 9, 0, 0, TimeSpan.Zero);
        var results = new List<CollectionResult>();
        foreach (var category in Enum.GetValues<Category>())
        {
            ConfigurationItem[] items = category switch
            {
                Category.Applications => [new("sample-app", "Example Editor", new() { ["Version"] = changed ? "2.0" : "1.0", ["Publisher"] = "Example" })],
                Category.Startup => changed ? [new ConfigurationItem("sample-startup", "Example Sync", new() { ["Launch value"] = "Privately compared; contents not stored" }) { Fingerprint = "PRIVATE-HMAC" }] : [],
                Category.Protection => [new("private-firewall", "Private Firewall", new() { ["Enabled"] = changed ? "No" : "Yes" })],
                Category.DefaultApps => [new(".pdf", "PDF files", new() { ["Application"] = changed ? "Example PDF Reader" : "Microsoft Edge" })],
                _ => []
            };
            results.Add(new(category, CollectionStatus.Success, start, start.AddSeconds(1), items));
        }
        return new(Guid.NewGuid(), start, start.AddSeconds(1), results);
    }

    private static AutomationElement FindId(AutomationElement parent, string id)
    {
        AutomationElement? element = null;
        var found = SpinWait.SpinUntil(() =>
        {
            element = parent.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, id));
            return element is not null;
        }, 5000);
        if (!found)
        {
            var state = parent.TryGetCurrentPattern(WindowPattern.Pattern, out var pattern)
                ? ((WindowPattern)pattern).Current.WindowVisualState.ToString() : "Not a window";
            var handle = (nint)parent.Current.NativeWindowHandle;
            var style = GetWindowLong(handle, -16);
            var foreground = GetForegroundWindow();
            GetWindowThreadProcessId(foreground, out var foregroundProcess);
            Assert.Fail($"Control not found: {id}; parent={parent.Current.Name}; handle={handle}; state={state}; offscreen={parent.Current.IsOffscreen}; nativeStyle=0x{style:X8}; foregroundProcess={foregroundProcess}.");
        }
        return element!;
    }

    private static AutomationElement WaitForWindow(int processId, string title)
    {
        AutomationElement? window = null;
        Assert.True(SpinWait.SpinUntil(() =>
        {
            var ownedRoots = AutomationElement.RootElement.FindAll(TreeScope.Children,
                new PropertyCondition(AutomationElement.ProcessIdProperty, processId)).Cast<AutomationElement>().ToArray();
            var condition = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, title),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
            window = ownedRoots.FirstOrDefault(root => root.Current.Name == title)
                ?? ownedRoots.Select(root => root.FindFirst(TreeScope.Descendants, condition)).FirstOrDefault(candidate => candidate is not null);
            return window is not null && !window.Current.IsOffscreen &&
                ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Current.WindowVisualState != WindowVisualState.Minimized;
        }, 10000), "Window not found: " + title);
        return window!;
    }

    private static void SelectSnapshot(AutomationElement window, string pickerId, string label)
    {
        var picker = FindId(window, pickerId);
        picker.SetFocus();
        Assert.True(SpinWait.SpinUntil(() => !picker.Current.IsOffscreen && picker.Current.HasKeyboardFocus, 5000),
            "The selector did not become visible and focused: " + pickerId);
        Assert.True(((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).WaitForInputIdle(5000));
        var expand = (ExpandCollapsePattern)picker.GetCurrentPattern(ExpandCollapsePattern.Pattern);
        expand.Expand();
        AutomationElement? item = null;
        var found = SpinWait.SpinUntil(() =>
        {
            if (expand.Current.ExpandCollapseState != ExpandCollapseState.Expanded)
            {
                picker.SetFocus();
                expand.Expand();
                return false;
            }
            item = picker.FindFirst(TreeScope.Descendants, new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem), new PropertyCondition(AutomationElement.NameProperty, label)));
            return item is not null;
        }, 5000);
        var available = picker.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
            .Cast<AutomationElement>().Select(option => option.Current.Name);
        Assert.True(found, $"Choice unavailable: {label}; state: {expand.Current.ExpandCollapseState}; options: {string.Join(", ", available)}");
        ((SelectionItemPattern)item!.GetCurrentPattern(SelectionItemPattern.Pattern)).Select();
        expand.Collapse();
        AssertSelectedSnapshot(window, pickerId, label);
        Assert.True(SpinWait.SpinUntil(() => expand.Current.ExpandCollapseState == ExpandCollapseState.Collapsed, 5000));
        Assert.True(((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).WaitForInputIdle(5000));
    }

    private static void InvokeId(AutomationElement parent, string id)
    {
        var element = FindId(parent, id);
        if (element.TryGetCurrentPattern(InvokePattern.Pattern, out var invoke)) ((InvokePattern)invoke).Invoke();
        else ((SelectionItemPattern)element.GetCurrentPattern(SelectionItemPattern.Pattern)).Select();
    }

    [Theory]
    [InlineData("Light", 100, "en")]
    [InlineData("Dark", 100, "en")]
    [InlineData("Dark", 200, "en")]
    [InlineData("Light", 150, "ar")]
    [Trait("Category", "Desktop")]
    public void AllPagesAndSecondaryWindowsExposeReadableAccessibleControls(string theme, int textSize, string languageCode)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerUi", Guid.NewGuid().ToString("N"));
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        store.SetPreference("collection.scope", "Both");
        store.SetPreference("schedule.interval", "Off");
        store.SetPreference("retention", "Forever");
        store.SetPreference("theme", theme);
        store.SetPreference("textSize", textSize.ToString());
        store.SetPreference("language", languageCode);
        var texts = new PCChangeTracker.App.Localization.UiText(languageCode);
        store.Save(Seed(0, false) with { Scope = CollectionScope.Both });
        store.Save(Seed(1, true) with { Scope = CollectionScope.Both });
        var start = new ProcessStartInfo(FindExecutable()) { UseShellExecute = false };
        start.ArgumentList.Add("--data-dir");
        start.ArgumentList.Add(directory);
        using var process = Process.Start(start)!;
        try
        {
            Assert.True(process.WaitForInputIdle(15000));
            var window = WaitForWindow(process.Id, "ChangeTracker");
            ResizeWindow(window, 1000, 740);
            foreach (var page in new[] { "ReviewPage", "HistoryPage", "SourcesPage", "SettingsPage" })
            {
                InvokeId(window, page);
                Assert.True(((SelectionItemPattern)FindId(window, page).GetCurrentPattern(SelectionItemPattern.Pattern)).Current.IsSelected);
                var headingId = page.Replace("Page", "Heading", StringComparison.Ordinal);
                Assert.True(SpinWait.SpinUntil(() => !FindId(window, headingId).Current.IsOffscreen, 5000), "Page heading was not brought into view: " + headingId);
                AssertNamedControls(window);
                if (page == "HistoryPage")
                {
                    var rows = FindId(window, "SnapshotList").FindAll(TreeScope.Descendants,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem));
                    Assert.NotEmpty(rows.Cast<AutomationElement>());
                    Assert.All(rows.Cast<AutomationElement>(), row => Assert.Contains(texts.Context(CollectionScope.Both, false), row.Current.Name));
                }
                if (page == "SourcesPage")
                {
                    Assert.Contains("PATH", FindId(window, "SourceEnvironment").Current.HelpText);
                    Assert.False(string.IsNullOrWhiteSpace(FindId(window, "SourceNetwork").Current.ItemStatus));
                }
                var screenshots = Path.Combine(FindRoot(), "artifacts", "screenshots");
                Directory.CreateDirectory(screenshots);
                Screenshot(window, Path.Combine(screenshots, $"accessible-{page}-{theme}-{textSize}-{languageCode}.png"));
            }
            InvokeId(window, "Report");
            var report = WaitForWindow(process.Id, texts["ReportTitle"]);
            Assert.True(SpinWait.SpinUntil(() => FindId(report, "ReportContent").Current.HasKeyboardFocus, 5000));
            AssertNamedControls(report);
            Screenshot(report, Path.Combine(FindRoot(), "artifacts", "screenshots", $"accessible-report-{theme}-{textSize}-{languageCode}.png"));
            Invoke(report, texts["Close"]);
            InvokeId(window, "HelpMe");
            var help = WaitForWindow(process.Id, texts["HelpTitle"]);
            Assert.True(SpinWait.SpinUntil(() => FindId(help, "HelpSearch").Current.HasKeyboardFocus, 5000));
            AssertNamedControls(help);
            ((ValuePattern)FindId(help, "HelpSearch").GetCurrentPattern(ValuePattern.Pattern)).SetValue("history.db-wal");
            WaitForText(help, texts["OneTopic"]);
            var topics = FindId(help, "HelpTopics").FindAll(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem));
            var storageTopic = Assert.Single(PCChangeTracker.App.HelpContent.Search(PCChangeTracker.App.HelpContent.LoadTopics(languageCode), "history.db-wal"));
            Assert.Equal(storageTopic.Title, Assert.Single(topics.Cast<AutomationElement>()).Current.Name);
            Screenshot(help, Path.Combine(FindRoot(), "artifacts", "screenshots", $"accessible-help-{theme}-{textSize}-{languageCode}.png"));
            InvokeId(help, "CloseHelp");
            Assert.Equal(2, store.List().Count);
        }
        finally
        {
            if (!process.HasExited) { process.Kill(true); process.WaitForExit(10000); }
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    private static void AssertNamedControls(AutomationElement root)
    {
        var namedTypes = new[] { ControlType.Button, ControlType.CheckBox, ControlType.RadioButton, ControlType.ComboBox, ControlType.Edit, ControlType.List, ControlType.ListItem, ControlType.Slider };
        foreach (var element in root.FindAll(TreeScope.Descendants, Condition.TrueCondition).Cast<AutomationElement>())
        {
            var current = element.Current;
            if (current.IsEnabled && current.IsKeyboardFocusable && !current.IsOffscreen && namedTypes.Contains(current.ControlType))
                Assert.False(string.IsNullOrWhiteSpace(current.Name), $"Unnamed {current.ControlType.ProgrammaticName}: {current.AutomationId} / {current.ClassName}");
            foreach (var internalName in new[] { "SourceOption", "HelpTopic {", "SnapshotSummary {", "ChangeDetailRow {", "PRIVATE-HMAC" })
                Assert.DoesNotContain(internalName, current.Name);
        }
    }

    [Fact]
    [Trait("Category", "Desktop")]
    public void KeyboardNavigationTextSizingAndModalEscapeWorkWithoutCollecting()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PCChangeTrackerUi", Guid.NewGuid().ToString("N"));
        var store = new HistoryStore(Path.Combine(directory, "history.db"));
        store.SetPreference("collection.scope", "Both");
        store.SetPreference("schedule.interval", "Off");
        store.SetPreference("retention", "Forever");
        store.Save(Seed(0, false) with { Scope = CollectionScope.Both });
        store.Save(Seed(1, true) with { Scope = CollectionScope.Both });
        var start = new ProcessStartInfo(FindExecutable()) { UseShellExecute = false };
        start.ArgumentList.Add("--data-dir");
        start.ArgumentList.Add(directory);
        using var process = Process.Start(start)!;
        try
        {
            Assert.True(process.WaitForInputIdle(15000));
            var window = WaitForWindow(process.Id, "ChangeTracker");
            ResizeWindow(window, 1000, 740);
            FindId(window, "ReviewPage").SetFocus();
            SendKeysTo(process, "{DOWN}");
            Assert.True(SpinWait.SpinUntil(() => ((SelectionItemPattern)FindId(window, "HistoryPage").GetCurrentPattern(SelectionItemPattern.Pattern)).Current.IsSelected, 5000));
            SendKeysTo(process, "{F6}");
            Assert.True(SpinWait.SpinUntil(() => FindId(window, "SimpleMode").Current.HasKeyboardFocus, 5000));
            SendKeysTo(process, "{F6}");
            Assert.True(SpinWait.SpinUntil(() => FindId(window, "HistoryHeading").Current.HasKeyboardFocus, 5000));
            SendKeysTo(process, "+{F6}");
            Assert.True(SpinWait.SpinUntil(() => FindId(window, "SimpleMode").Current.HasKeyboardFocus, 5000));
            SendKeysTo(process, "^4");
            Assert.True(SpinWait.SpinUntil(() => FindId(window, "SettingsHeading").Current.HasKeyboardFocus, 5000));
            SelectSnapshot(window, "AppTextSize", "150%");
            Assert.True(SpinWait.SpinUntil(() => store.GetPreference("textSize") == "150", 5000));
            SendKeysTo(process, "^1");
            Assert.True(SpinWait.SpinUntil(() => FindId(window, "ReviewHeading").Current.HasKeyboardFocus, 5000));
            var inspect = FindId(window, "InspectChange");
            inspect.SetFocus();
            SendKeysTo(process, "{ENTER}");
            Assert.True(SpinWait.SpinUntil(() => FindId(window, "CloseChangeDetails").Current.HasKeyboardFocus, 5000));
            SendKeysTo(process, "^4");
            Assert.True(((SelectionItemPattern)FindId(window, "ReviewPage").GetCurrentPattern(SelectionItemPattern.Pattern)).Current.IsSelected);
            SendKeysTo(process, "+{TAB}");
            Assert.False(FindId(window, "HelpMe").Current.IsEnabled);
            Assert.False(FindId(window, "SnapshotStorage").Current.HasKeyboardFocus);
            SendKeysTo(process, "{ESC}");
            Assert.True(SpinWait.SpinUntil(() => inspect.Current.HasKeyboardFocus, 5000));
            SendKeysTo(process, "{F1}");
            var help = WaitForWindow(process.Id, "Help me - ChangeTracker");
            Assert.True(SpinWait.SpinUntil(() => FindId(help, "HelpSearch").Current.HasKeyboardFocus, 5000));
            SendKeysTo(process, "{F6}");
            SendKeysTo(process, "{F6}");
            Assert.True(SpinWait.SpinUntil(() => FindId(help, "HelpContent").Current.HasKeyboardFocus, 5000));
            SendKeysTo(process, "^f");
            Assert.True(SpinWait.SpinUntil(() => FindId(help, "HelpSearch").Current.HasKeyboardFocus, 5000));
            SendKeysTo(process, "{ESC}");
            Assert.True(SpinWait.SpinUntil(() => inspect.Current.HasKeyboardFocus, 5000));
            Assert.Equal(2, store.List().Count);
            ExitFromTray(process);
        }
        finally
        {
            if (!process.HasExited) { process.Kill(true); process.WaitForExit(10000); }
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    private static void SendKeysTo(Process process, string keys)
    {
        GetWindowThreadProcessId(GetForegroundWindow(), out var foregroundProcess);
        Assert.Equal((uint)process.Id, foregroundProcess);
        System.Windows.Forms.SendKeys.SendWait(keys);
    }

    private static AutomationElement FindName(AutomationElement parent, string name) => parent.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.NameProperty, name)) ?? throw new InvalidOperationException($"The accessible element '{name}' was not found.");

    private static void AssertSelectedSnapshot(AutomationElement window, string pickerId, string name)
    {
        Assert.True(SpinWait.SpinUntil(() =>
        {
            var selected = ((SelectionPattern)FindId(window, pickerId).GetCurrentPattern(SelectionPattern.Pattern)).Current.GetSelection();
            return selected.Length == 1 && selected[0].Current.Name == name;
        }, 5000), $"The selected value in {pickerId} changed unexpectedly.");
    }

    private static void ResizeWindow(AutomationElement window, double width, double height)
    {
        var pattern = (WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern);
        pattern.SetWindowVisualState(WindowVisualState.Normal);
        Assert.True(SpinWait.SpinUntil(() => pattern.Current.WindowVisualState == WindowVisualState.Normal, 5000));
        ((TransformPattern)window.GetCurrentPattern(TransformPattern.Pattern)).Resize(width, height);
        Assert.True(pattern.WaitForInputIdle(5000));
    }

    private static nint FindTrayWindow(Process process)
    {
        nint trayWindow = 0;
        Assert.True(SpinWait.SpinUntil(() =>
        {
            EnumWindows((handle, parameter) =>
            {
                GetWindowThreadProcessId(handle, out var processId);
                if (processId != process.Id) return true;
                var className = new System.Text.StringBuilder(256);
                GetClassName(handle, className, className.Capacity);
                if (!className.ToString().StartsWith("WindowsForms10.Window", StringComparison.Ordinal)) return true;
                var identifier = new TrayIconIdentifier { Size = (uint)Marshal.SizeOf<TrayIconIdentifier>(), Window = handle, Id = 1 };
                if (Shell_NotifyIconGetRect(ref identifier, out _) != 0) return true;
                trayWindow = handle;
                return false;
            }, 0);
            return trayWindow != 0;
        }, 10000), "The app's registered notification-area icon was not found.");
        return trayWindow;
    }

    private static void InvokeTrayCommand(Process process, string label)
    {
        const uint notifyIconCallback = 0x800;
        const nint rightButtonReleased = 0x205;
        Assert.True(PostMessage(FindTrayWindow(process), notifyIconCallback, 1, rightButtonReleased));
        AutomationElement? command = null;
        Assert.True(SpinWait.SpinUntil(() =>
        {
            var roots = AutomationElement.RootElement.FindAll(TreeScope.Children,
                new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id)).Cast<AutomationElement>();
            var condition = new AndCondition(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem),
                new PropertyCondition(AutomationElement.NameProperty, label), new PropertyCondition(AutomationElement.IsOffscreenProperty, false));
            command = roots.Select(root => root.FindFirst(TreeScope.Descendants, condition)).FirstOrDefault(item => item is not null);
            return command is not null;
        }, 5000), "Tray command not found: " + label);
        ((InvokePattern)command!.GetCurrentPattern(InvokePattern.Pattern)).Invoke();
    }

    private static void ExitFromTray(Process process, string languageCode = "en")
    {
        InvokeTrayCommand(process, new PCChangeTracker.App.Localization.UiText(languageCode)["TrayExit"]);
        Assert.True(process.WaitForExit(10000), "Tray Exit did not terminate the application.");
        Assert.Equal(0, process.ExitCode);
    }

    private static void Invoke(AutomationElement parent, string name)
    {
        var button = parent.FindAll(TreeScope.Descendants, new AndCondition(
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
            new PropertyCondition(AutomationElement.NameProperty, name))).Cast<AutomationElement>().First(element => !element.Current.IsOffscreen);
        ((InvokePattern)button.GetCurrentPattern(InvokePattern.Pattern)).Invoke();
    }

    private static void WaitForText(AutomationElement window, string value) => Assert.True(SpinWait.SpinUntil(() =>
        window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, value)) is not null, 10000), "Missing UI text: " + value);

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "PCChangeTracker.slnx"))) directory = directory.Parent;
        return directory?.FullName ?? throw new InvalidOperationException("Repository root not found.");
    }

    private static string FindExecutable() => Path.Combine(FindRoot(), "src", "PCChangeTracker.App", "bin",
        new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name, "net10.0-windows", "PCChangeTracker.exe");

    private static void Screenshot(AutomationElement window, string path)
    {
        var bounds = window.Current.BoundingRectangle;
        using var bitmap = new System.Drawing.Bitmap((int)bounds.Width, (int)bounds.Height);
        using var graphics = System.Drawing.Graphics.FromImage(bitmap);
        var context = graphics.GetHdc();
        try { Assert.True(PrintWindow((nint)window.Current.NativeWindowHandle, context, 2)); }
        finally { graphics.ReleaseHdc(context); }
        bitmap.Save(path, ImageFormat.Png);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PrintWindow(nint window, nint deviceContext, uint flags);

    private delegate bool EnumWindowsCallback(nint window, nint parameter);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumWindows(EnumWindowsCallback callback, nint parameter);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(nint window, out uint processId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(nint window, System.Text.StringBuilder className, int capacity);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PostMessage(nint window, uint message, nint parameter, nint detail);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowLong(nint window, int index);

    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern nint SendMessageTimeout(nint window, uint message, nint parameter, nint detail, uint flags, uint timeout, out nint result);

    [DllImport("shell32.dll")]
    private static extern int Shell_NotifyIconGetRect(ref TrayIconIdentifier identifier, out NativeRectangle rectangle);

    [StructLayout(LayoutKind.Sequential)]
    private struct TrayIconIdentifier
    {
        public uint Size;
        public nint Window;
        public uint Id;
        public Guid Guid;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRectangle
    {
        public int Left, Top, Right, Bottom;
    }
}