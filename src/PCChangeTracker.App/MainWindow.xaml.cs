using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Threading;

namespace PCChangeTracker.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel viewModel;
    private bool closeAfterCapture;
    private bool allowRealClose;
    private HelpWindow? helpWindow;
    public MainWindow(MainViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(MainViewModel.IsBusy) && closeAfterCapture && !viewModel.IsBusy)
                Dispatcher.BeginInvoke(Close);
            if (args.PropertyName == nameof(MainViewModel.Status)) Announce(StatusText);
            if (args.PropertyName == nameof(MainViewModel.ComparisonSelectionIssue)) Announce(SelectionIssueText);
            if (args.PropertyName == nameof(MainViewModel.SelectedChange) && viewModel.SelectedChange is not null)
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => CloseDetailButton.Focus()));
        };
    }

    private void Announce(UIElement element)
    {
        if (!AutomationPeer.ListenerExists(AutomationEvents.LiveRegionChanged)) return;
        Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        {
            if (!IsLoaded) return;
            var peer = UIElementAutomationPeer.FromElement(element) ?? UIElementAutomationPeer.CreatePeerForElement(element);
            peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }));
    }

    private void OpenHelp(object sender, RoutedEventArgs eventArgs)
    {
        if (helpWindow is not null) { helpWindow.Activate(); return; }
        helpWindow = new HelpWindow(viewModel.Texts) { Owner = this };
        helpWindow.Closed += (_, _) => helpWindow = null;
        helpWindow.Show();
    }

    protected override void OnClosing(CancelEventArgs eventArgs)
    {
        if (!allowRealClose && viewModel.MinimizeToTray)
        {
            eventArgs.Cancel = true;
            Hide();
            return;
        }
        if (viewModel.IsBusy)
        {
            closeAfterCapture = true;
            viewModel.StopBackgroundWork();
            eventArgs.Cancel = true;
            base.OnClosing(eventArgs);
            return;
        }
        base.OnClosing(eventArgs);
    }

    protected override void OnStateChanged(EventArgs eventArgs)
    {
        base.OnStateChanged(eventArgs);
        if (WindowState == WindowState.Minimized && viewModel.MinimizeToTray) Hide();
    }

    protected override void OnClosed(EventArgs eventArgs)
    {
        viewModel.StopBackgroundWork();
        base.OnClosed(eventArgs);
    }

    /// <summary>Bypasses minimize-to-tray so the tray menu's Exit truly quits.</summary>
    public void ForceClose() { allowRealClose = true; Close(); }
}