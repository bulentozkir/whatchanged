using System.Diagnostics;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows;
using PCChangeTracker.Core;
using PCChangeTracker.Windows;

namespace PCChangeTracker.App;

public partial class App : Application
{
    private Mutex? instanceMutex;
    private EventWaitHandle? activation;
    private RegisteredWaitHandle? activationRegistration;
    private Dictionary<string, object>? standardBrushes;
    private string currentTheme = "Light";
    private string currentButtonTextColor = "ButtonColorDefault";
    private string currentAppTextColor = "ButtonColorDefault";
    private string currentButtonColor = "ButtonColorDefault";
    private string currentLabelColor = "ButtonColorDefault";
    private System.Windows.Forms.NotifyIcon? trayIcon;
    private System.Windows.Forms.ToolStripMenuItem? trayOpenItem;
    private System.Windows.Forms.ToolStripMenuItem? trayExitItem;

    protected override async void OnStartup(StartupEventArgs eventArgs)
    {
        base.OnStartup(eventArgs);
        if (eventArgs.Args.Contains("--machine-check"))
        {
            if (eventArgs.Args.Length != 3 || eventArgs.Args[0] != "--machine-check" || !int.TryParse(eventArgs.Args[2], out var parentId))
            { Shutdown(2); return; }
            Shutdown(await CollectorTransport.RunHelperAsync(eventArgs.Args[1], parentId));
            return;
        }
        var dataIndex = Array.IndexOf(eventArgs.Args, "--data-dir");
        var dataDirectory = dataIndex >= 0 && dataIndex + 1 < eventArgs.Args.Length
            ? Path.GetFullPath(eventArgs.Args[dataIndex + 1])
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PCChangeTracker");
        var workerIndex = Array.IndexOf(eventArgs.Args, "--collect");
        if (workerIndex >= 0)
        {
            byte[]? material = null;
            try
            {
                var scopeIndex = Array.IndexOf(eventArgs.Args, "--scope");
                if (workerIndex + 1 >= eventArgs.Args.Length || !Enum.TryParse<Category>(eventArgs.Args[workerIndex + 1], out var category) ||
                    scopeIndex < 0 || scopeIndex + 1 >= eventArgs.Args.Length ||
                    !Enum.TryParse<CollectionScope>(eventArgs.Args[scopeIndex + 1], out var scope) || !CollectorCatalog.Supports(category, scope))
                { Shutdown(2); return; }
                if (eventArgs.Args.Contains("--key-stdin"))
                {
                    if (scope != CollectionScope.Machine || !CollectorTransport.IsAdministrator) { Shutdown(2); return; }
                    material = new byte[32];
                    using var inputDeadline = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    await Console.OpenStandardInput().ReadExactlyAsync(material, inputDeadline.Token);
                    dataDirectory = "";
                }
                var result = CollectorCatalog.Create(category, dataDirectory, scope, material).Collect(CancellationToken.None);
                Console.Out.WriteLine(JsonSerializer.Serialize(result));
                Shutdown();
            }
            catch (Exception exception) when (exception is not OutOfMemoryException) { Shutdown(1); }
            finally { if (material is not null) CryptographicOperations.ZeroMemory(material); }
            return;
        }

        if (CollectorTransport.IsAdministrator)
        {
            MessageBox.Show("Open ChangeTracker normally, not as administrator. Administrator access is requested separately for a single machine-wide check.",
                "ChangeTracker", MessageBoxButton.OK, MessageBoxImage.Information);
            Shutdown();
            return;
        }

        var suffix = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(dataDirectory.ToUpperInvariant())))[..20];
        instanceMutex = new Mutex(true, @"Local\PCChangeTracker." + suffix, out var first);
        if (!first)
        {
            try { using var existing = EventWaitHandle.OpenExisting(@"Local\PCChangeTracker.Open." + suffix); existing.Set(); }
            catch (WaitHandleCannotBeOpenedException) { }
            instanceMutex.Dispose();
            instanceMutex = null;
            Shutdown();
            return;
        }
        standardBrushes = new[] { "CanvasBrush", "SurfaceBrush", "TextBrush", "MutedBrush", "AccentBrush", "AccentTextBrush", "ReviewBrush", "LineBrush", "ButtonTextBrush", "ButtonBackgroundBrush", "LabelBrush" }
            .ToDictionary(name => name, name => Resources[name]);
        SystemParameters.StaticPropertyChanged += SystemSettingsChanged;
        try
        {
            var store = new HistoryStore(Path.Combine(dataDirectory, "history.db"));
            currentTheme = store.GetPreference("theme") == "Dark" ? "Dark" : "Light";
            currentButtonTextColor = store.GetPreference("buttonTextColor") ?? "ButtonColorDefault";
            currentAppTextColor = store.GetPreference("appTextColor") ?? "ButtonColorDefault";
            currentButtonColor = store.GetPreference("buttonColor") ?? "ButtonColorDefault";
            currentLabelColor = store.GetPreference("labelColor") ?? "ButtonColorDefault";
            Resources["AppFontFamily"] = new System.Windows.Media.FontFamily(store.GetPreference("font") ?? "Segoe UI");
            ApplyAppearance();
            var viewModel = new MainViewModel(store, new CaptureService(dataDirectory), dataDirectory);
            var window = new MainWindow(viewModel);
            MainWindow = window;
            SetupTrayIcon(window, viewModel);
            activation = new EventWaitHandle(false, EventResetMode.AutoReset, @"Local\PCChangeTracker.Open." + suffix);
            activationRegistration = ThreadPool.RegisterWaitForSingleObject(activation,
                (_, _) => Dispatcher.BeginInvoke(() => { window.Show(); window.WindowState = WindowState.Normal; window.Activate(); }),
                null, Timeout.Infinite, false);
            var startMinimized = eventArgs.Args.Contains("--start-minimized");
            if (startMinimized && viewModel.MinimizeToTray) { /* Stay hidden; restore from the tray icon. */ }
            else if (startMinimized) { window.WindowState = WindowState.Minimized; window.Show(); }
            else window.Show();
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            MessageBox.Show("The local history could not be opened. Existing data has not been deleted. Close other copies of the app and check the data folder before retrying.",
                "ChangeTracker", MessageBoxButton.OK, MessageBoxImage.Warning);
            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs eventArgs)
    {
        SystemParameters.StaticPropertyChanged -= SystemSettingsChanged;
        activationRegistration?.Unregister(null);
        activation?.Dispose();
        trayIcon?.Dispose();
        if (instanceMutex is not null) { instanceMutex.ReleaseMutex(); instanceMutex.Dispose(); }
        base.OnExit(eventArgs);
    }

    private void SystemSettingsChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(SystemParameters.HighContrast) && standardBrushes is not null)
            Dispatcher.BeginInvoke(ApplyAppearance);
    }

    internal void SetTheme(string theme) { currentTheme = theme; ApplyAppearance(); }
    internal void SetButtonTextColor(string color) { currentButtonTextColor = color; ApplyAppearance(); }
    internal void SetAppTextColor(string color) { currentAppTextColor = color; ApplyAppearance(); }
    internal void SetButtonColor(string color) { currentButtonColor = color; ApplyAppearance(); }
    internal void SetLabelColor(string color) { currentLabelColor = color; ApplyAppearance(); }
    internal void SetFont(string fontFamily) => Resources["AppFontFamily"] = new System.Windows.Media.FontFamily(fontFamily);

    private void ApplyAppearance()
    {
        if (standardBrushes is null) return;
        ApplyAccessibilityColors(Resources, BuildPalette(standardBrushes, currentTheme, currentAppTextColor,
            currentButtonTextColor, currentButtonColor, currentLabelColor), SystemParameters.HighContrast);
    }

    internal static readonly IReadOnlyDictionary<string, string> DarkPaletteHex = new Dictionary<string, string>
    {
        ["CanvasBrush"] = "#14191C", ["SurfaceBrush"] = "#1E2528", ["TextBrush"] = "#F2F5F6", ["MutedBrush"] = "#B7C2C7",
        ["AccentBrush"] = "#1FBFAA", ["AccentTextBrush"] = "#04211D", ["ReviewBrush"] = "#F2A65A", ["LineBrush"] = "#647680",
        ["ButtonTextBrush"] = "#F2F5F6", ["ButtonBackgroundBrush"] = "#1E2528", ["LabelBrush"] = "#F2F5F6"
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

    internal static System.Windows.Media.Color ColorForChoice(string theme, string code, bool background = false)
    {
        var choices = background ? ButtonBackgroundColorHex : ButtonTextColorHex;
        var fallback = theme == "Dark" ? DarkPaletteHex[background ? "SurfaceBrush" : "TextBrush"] : background ? "#FFFFFF" : "#172126";
        return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(
            choices.TryGetValue((theme, code), out var hex) ? hex : fallback);
    }

    internal static Dictionary<string, object> BuildPalette(IReadOnlyDictionary<string, object> lightBrushes, string theme,
        string appTextColor = "ButtonColorDefault", string buttonTextColor = "ButtonColorDefault",
        string buttonColor = "ButtonColorDefault", string labelColor = "ButtonColorDefault")
    {
        var palette = new Dictionary<string, object>(lightBrushes);
        if (theme == "Dark")
            foreach (var pair in DarkPaletteHex)
                palette[pair.Key] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(pair.Value));
        foreach (var (key, code) in new[] { ("TextBrush", appTextColor), ("ButtonTextBrush", buttonTextColor), ("LabelBrush", labelColor) })
            if (ButtonTextColorHex.ContainsKey((theme, code)))
                palette[key] = new System.Windows.Media.SolidColorBrush(ColorForChoice(theme, code));
        if (ButtonBackgroundColorHex.ContainsKey((theme, buttonColor)))
        {
            palette["ButtonBackgroundBrush"] = new System.Windows.Media.SolidColorBrush(ColorForChoice(theme, buttonColor, true));
            palette["AccentBrush"] = new System.Windows.Media.SolidColorBrush(ColorForChoice(theme, buttonColor));
        }
        return palette;
    }

    private void SetupTrayIcon(MainWindow window, MainViewModel viewModel)
    {
        trayIcon = new System.Windows.Forms.NotifyIcon { Icon = LoadTrayIcon(), Text = "ChangeTracker", Visible = viewModel.MinimizeToTray };
        trayIcon.DoubleClick += (_, _) => RestoreWindow(window);
        trayOpenItem = new System.Windows.Forms.ToolStripMenuItem(viewModel.Texts["TrayOpen"], null, (_, _) => RestoreWindow(window));
        trayExitItem = new System.Windows.Forms.ToolStripMenuItem(viewModel.Texts["TrayExit"], null, (_, _) => window.ForceClose());
        var menu = new System.Windows.Forms.ContextMenuStrip();
        menu.Items.Add(trayOpenItem);
        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
        menu.Items.Add(trayExitItem);
        trayIcon.ContextMenuStrip = menu;
        viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(MainViewModel.MinimizeToTray) && trayIcon is not null) trayIcon.Visible = viewModel.MinimizeToTray;
        };
        viewModel.Texts.Changed += (_, _) =>
        {
            if (trayOpenItem is not null) trayOpenItem.Text = viewModel.Texts["TrayOpen"];
            if (trayExitItem is not null) trayExitItem.Text = viewModel.Texts["TrayExit"];
        };
    }

    private static void RestoreWindow(MainWindow window) { window.Show(); window.WindowState = WindowState.Normal; window.Activate(); }

    private static System.Drawing.Icon LoadTrayIcon()
    {
        using var stream = typeof(App).Assembly.GetManifestResourceStream("PCChangeTracker.TrayIcon.png")!;
        using var bitmap = new System.Drawing.Bitmap(stream);
        return System.Drawing.Icon.FromHandle(bitmap.GetHicon());
    }

    internal static void ApplyAccessibilityColors(ResourceDictionary resources, IReadOnlyDictionary<string, object> standard, bool highContrast)
    {
        foreach (var pair in standard)
            resources[pair.Key] = !highContrast ? pair.Value : pair.Key switch
            {
                "CanvasBrush" or "SurfaceBrush" or "ButtonBackgroundBrush" => SystemColors.WindowBrush,
                "AccentBrush" => SystemColors.HighlightBrush,
                "AccentTextBrush" => SystemColors.HighlightTextBrush,
                _ => SystemColors.WindowTextBrush
            };
    }
}