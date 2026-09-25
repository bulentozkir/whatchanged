using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace PCChangeTracker.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel viewModel;
    private bool closeAfterCapture;
    private bool allowRealClose;
    private WindowState restoreState = WindowState.Maximized;
    private HelpWindow? helpWindow;
    private UIElement? detailOrigin;
    public MainWindow(MainViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Loaded += (_, _) =>
        {
            if (viewModel.ChoosingScope) CurrentUserScopeCheckBox.Focus();
        };
        viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(MainViewModel.IsBusy) && closeAfterCapture && !viewModel.IsBusy)
                Dispatcher.BeginInvoke(Close);
            if (args.PropertyName == nameof(MainViewModel.Status)) Announce(StatusText);
            if (args.PropertyName == nameof(MainViewModel.ComparisonSelectionIssue)) Announce(SelectionIssueText);
            if (args.PropertyName == nameof(MainViewModel.Page))
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => CurrentPageHeading.BringIntoView()));
            if (args.PropertyName == nameof(MainViewModel.ChoosingScope))
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    if (viewModel.ChoosingScope) CurrentUserScopeCheckBox.Focus();
                    else ChangeScopeButton.Focus();
                }));
            if (args.PropertyName == nameof(MainViewModel.SelectedChange))
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    if (viewModel.SelectedChange is not null) CloseDetailButton.Focus();
                    else if (detailOrigin is { IsVisible: true, IsEnabled: true }) detailOrigin.Focus();
                    else CheckNowButton.Focus();
                }));
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
        var origin = Keyboard.FocusedElement as UIElement;
        helpWindow = new HelpWindow(viewModel.Texts) { Owner = this };
        helpWindow.Closed += (_, _) =>
        {
            helpWindow = null;
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                if (origin is { IsVisible: true, IsEnabled: true }) origin.Focus();
            }));
        };
        helpWindow.Show();
    }

    private TextBlock CurrentPageHeading => viewModel.Page switch
    {
        "History" => HistoryHeading,
        "Sources" => SourcesHeading,
        "Settings" => SettingsHeading,
        _ => ReviewHeading
    };

    protected override void OnPreviewKeyDown(KeyEventArgs eventArgs)
    {
        base.OnPreviewKeyDown(eventArgs);
        if (eventArgs.Key == Key.Escape && viewModel.SelectedChange is not null)
        {
            viewModel.CloseDetailsCommand.Execute(null);
            eventArgs.Handled = true;
            return;
        }
        if (!viewModel.BackgroundInteractive) return;
        if (Keyboard.Modifiers == ModifierKeys.Control && eventArgs.Key is >= Key.D1 and <= Key.D4)
        {
            string[] pages = ["Review", "History", "Sources", "Settings"];
            viewModel.NavigateCommand.Execute(pages[eventArgs.Key - Key.D1]);
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(FocusPage));
            eventArgs.Handled = true;
        }
        else if (eventArgs.Key == Key.F6 && Keyboard.Modifiers is ModifierKeys.None or ModifierKeys.Shift)
        {
            var current = PageNavigation.IsKeyboardFocusWithin ? 0 : MainToolbar.IsKeyboardFocusWithin ? 1 : 2;
            var next = (current + (Keyboard.Modifiers == ModifierKeys.Shift ? 2 : 1)) % 3;
            if (next == 0 && PageNavigation.SelectedItem is ListBoxItem item) { item.BringIntoView(); item.Focus(); }
            else if (next == 1) { MainToolbar.BringIntoView(); MainToolbar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First)); }
            else FocusPage();
            eventArgs.Handled = true;
        }
    }

    private void FocusPage() { CurrentPageHeading.BringIntoView(); CurrentPageHeading.Focus(); }

    private void BeforeDaySelected(object sender, SelectionChangedEventArgs eventArgs)
    {
        if (((ComboBox)sender).SelectedItem is DateTime day) viewModel.BeforeDate = day;
    }

    private void AfterDaySelected(object sender, SelectionChangedEventArgs eventArgs)
    {
        if (((ComboBox)sender).SelectedItem is DateTime day) viewModel.AfterDate = day;
    }

    private void RememberDetailOrigin(object sender, RoutedEventArgs eventArgs) => detailOrigin = sender as UIElement;

    private void ResponsiveColumnsChanged(object sender, SizeChangedEventArgs eventArgs)
    {
        var panel = (UniformGrid)sender;
        panel.Columns = panel.ActualWidth >= TextElement.GetFontSize(panel) * 36 ? 2 : 1;
    }

    protected override void OnClosing(CancelEventArgs eventArgs)
    {
        if (!allowRealClose)
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
        if (WindowState == WindowState.Minimized) Hide();
        else restoreState = WindowState;
    }

    public void RestoreFromTray()
    {
        Show();
        if (WindowState == WindowState.Minimized) WindowState = restoreState;
        Activate();
    }

    protected override void OnClosed(EventArgs eventArgs)
    {
        viewModel.StopBackgroundWork();
        base.OnClosed(eventArgs);
    }

    /// <summary>Bypasses minimize-to-tray so the tray menu's Exit truly quits.</summary>
    public void ForceClose() { allowRealClose = true; Close(); }
}