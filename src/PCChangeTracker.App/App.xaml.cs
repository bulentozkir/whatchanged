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
    private string currentTheme = Appearance.DefaultTheme;
    private string currentButtonTextColor = "ButtonColorDefault";
    private string currentAppTextColor = "ButtonColorDefault";
    private string currentButtonColor = "ButtonColorDefault";
    private string currentLabelColor = "ButtonColorDefault";
    private System.Windows.Forms.NotifyIcon? trayIcon;
    private System.Windows.Forms.ToolStripMenuItem? trayOpenItem;
    private System.Windows.Forms.ToolStripMenuItem? trayExitItem;

    protected override void OnStartup(StartupEventArgs eventArgs)
    {
        base.OnStartup(eventArgs);
        // ChangeTracker runs only in the user's default, unelevated security context: it never requests elevation,
        // and neither the window nor a collector worker runs when started with administrator rights.
        var dataIndex = Array.IndexOf(eventArgs.Args, "--data-dir");
        var dataDirectory = dataIndex >= 0 && dataIndex + 1 < eventArgs.Args.Length
            ? Path.GetFullPath(eventArgs.Args[dataIndex + 1])
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PCChangeTracker");
        var workerIndex = Array.IndexOf(eventArgs.Args, "--collect");
        if (workerIndex >= 0)
        {
            try
            {
                var scopeIndex = Array.IndexOf(eventArgs.Args, "--scope");
                if (CollectorTransport.IsAdministrator || workerIndex + 1 >= eventArgs.Args.Length ||
                    !Enum.TryParse<Category>(eventArgs.Args[workerIndex + 1], out var category) ||
                    scopeIndex < 0 || scopeIndex + 1 >= eventArgs.Args.Length ||
                    !Enum.TryParse<CollectionScope>(eventArgs.Args[scopeIndex + 1], out var scope) || !CollectorCatalog.Supports(category, scope))
                { Shutdown(2); return; }
                var result = CollectorCatalog.Create(category, dataDirectory, scope).Collect(CancellationToken.None);
                Console.Out.WriteLine(JsonSerializer.Serialize(result));
                Shutdown();
            }
            catch (Exception exception) when (exception is not OutOfMemoryException) { Shutdown(1); }
            return;
        }

        if (CollectorTransport.IsAdministrator)
        {
            ShowStartupMessage("StandardLaunchRequired", MessageBoxImage.Information);
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
        standardBrushes = Appearance.ColorTokens(Resources);
        EventManager.RegisterClassHandler(typeof(Window), FrameworkElement.LoadedEvent,
            new RoutedEventHandler((sender, _) => Controls.WindowTheme.ApplyTitleBar((Window)sender, UsesDarkTitleBar)));
        SystemParameters.StaticPropertyChanged += SystemSettingsChanged;
        try
        {
            var store = new HistoryStore(Path.Combine(dataDirectory, "history.db"));
            currentTheme = Appearance.ThemeOrDefault(store.GetPreference("theme"));
            currentButtonTextColor = store.GetPreference("buttonTextColor") ?? "ButtonColorDefault";
            currentAppTextColor = store.GetPreference("appTextColor") ?? "ButtonColorDefault";
            currentButtonColor = store.GetPreference("buttonColor") ?? "ButtonColorDefault";
            currentLabelColor = store.GetPreference("labelColor") ?? "ButtonColorDefault";
            Resources["AppFontFamily"] = new System.Windows.Media.FontFamily(store.GetPreference("font") ?? "Segoe UI");
            ApplyAppearance();
            var viewModel = new MainViewModel(store, new CaptureService(dataDirectory), dataDirectory);
            SetTextSize(viewModel.SelectedTextSize);
            var window = new MainWindow(viewModel);
            MainWindow = window;
            SetupTrayIcon(window, viewModel);
            activation = new EventWaitHandle(false, EventResetMode.AutoReset, @"Local\PCChangeTracker.Open." + suffix);
            activationRegistration = ThreadPool.RegisterWaitForSingleObject(activation,
                (_, _) => Dispatcher.BeginInvoke(window.ShowForLaunch),
                null, Timeout.Infinite, false);
            if (!eventArgs.Args.Contains("--start-minimized")) window.Show();
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            ShowStartupMessage("HistoryOpenFailed", MessageBoxImage.Warning);
            Shutdown(1);
        }
    }

    /// <summary>Shows a startup message in the Windows display language; saved preferences are not readable yet.</summary>
    private static void ShowStartupMessage(string key, MessageBoxImage image)
    {
        var texts = Localization.UiText.ForSystemLanguage();
        var options = texts.Language.RightToLeft ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : MessageBoxOptions.None;
        MessageBox.Show(texts[key], "ChangeTracker", MessageBoxButton.OK, image, MessageBoxResult.OK, options);
    }

    protected override void OnExit(ExitEventArgs eventArgs)
    {
        SystemParameters.StaticPropertyChanged -= SystemSettingsChanged;
        activationRegistration?.Unregister(null);
        activation?.Dispose();
        var icon = trayIcon?.Icon;
        trayIcon?.Dispose();
        icon?.Dispose();
        if (instanceMutex is not null) { instanceMutex.ReleaseMutex(); instanceMutex.Dispose(); }
        base.OnExit(eventArgs);
    }

    private void SystemSettingsChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(SystemParameters.HighContrast) && standardBrushes is not null)
            Dispatcher.BeginInvoke(ApplyAppearance);
    }

    internal void SetTheme(string theme) { currentTheme = Appearance.ThemeOrDefault(theme); ApplyAppearance(); }
    internal void SetButtonTextColor(string color) { currentButtonTextColor = color; ApplyAppearance(); }
    internal void SetAppTextColor(string color) { currentAppTextColor = color; ApplyAppearance(); }
    internal void SetButtonColor(string color) { currentButtonColor = color; ApplyAppearance(); }
    internal void SetLabelColor(string color) { currentLabelColor = color; ApplyAppearance(); }
    internal void SetFont(string fontFamily) => Resources["AppFontFamily"] = new System.Windows.Media.FontFamily(fontFamily);
    internal void SetTextSize(int percentage) => Appearance.ApplyTextSize(Resources, percentage);

    private void ApplyAppearance()
    {
        if (standardBrushes is null) return;
        Appearance.ApplyAccessibilityColors(Resources, Appearance.BuildPalette(standardBrushes, currentTheme, currentAppTextColor,
            currentButtonTextColor, currentButtonColor, currentLabelColor), SystemParameters.HighContrast);
        foreach (Window window in Windows) Controls.WindowTheme.ApplyTitleBar(window, UsesDarkTitleBar);
    }

    private bool UsesDarkTitleBar => currentTheme == "Dark" && !SystemParameters.HighContrast;

    private void SetupTrayIcon(MainWindow window, MainViewModel viewModel)
    {
        trayIcon = new System.Windows.Forms.NotifyIcon { Icon = LoadTrayIcon(), Text = "ChangeTracker", Visible = true };
        trayIcon.DoubleClick += (_, _) => window.RestoreFromTray();
        trayOpenItem = new System.Windows.Forms.ToolStripMenuItem(viewModel.Texts["TrayOpen"], null, (_, _) => window.RestoreFromTray());
        trayExitItem = new System.Windows.Forms.ToolStripMenuItem(viewModel.Texts["TrayExit"], null, (_, _) => window.ForceClose());
        var menu = new System.Windows.Forms.ContextMenuStrip();
        menu.Items.Add(trayOpenItem);
        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
        menu.Items.Add(trayExitItem);
        trayIcon.ContextMenuStrip = menu;
        viewModel.Texts.Changed += (_, _) =>
        {
            if (trayOpenItem is not null) trayOpenItem.Text = viewModel.Texts["TrayOpen"];
            if (trayExitItem is not null) trayExitItem.Text = viewModel.Texts["TrayExit"];
        };
    }

    private static System.Drawing.Icon LoadTrayIcon()
    {
        using var stream = typeof(App).Assembly.GetManifestResourceStream("PCChangeTracker.AppIcon.ico")!;
        using var icon = new System.Drawing.Icon(stream, System.Windows.Forms.SystemInformation.SmallIconSize);
        return (System.Drawing.Icon)icon.Clone();
    }
}