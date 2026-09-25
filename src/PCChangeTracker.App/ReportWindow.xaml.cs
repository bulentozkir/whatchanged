using System.Text;
using System.Windows;
using Microsoft.Win32;
using PCChangeTracker.Core;
using PCChangeTracker.App.Localization;

namespace PCChangeTracker.App;

public partial class ReportWindow : Window
{
    private readonly ReportDocument report;
    public UiText Texts { get; }
    public ReportWindow(ReportDocument report, bool advanced, UiText? texts = null)
    {
        Texts = texts ?? new UiText();
        this.report = report;
        InitializeComponent();
        DataContext = this;
        Preview.Text = Texts.TextReport(report);
        Texts.Changed += LanguageChanged;
        JsonButton.Visibility = CsvButton.Visibility = advanced ? Visibility.Visible : Visibility.Collapsed;
        Loaded += (_, _) => Preview.Focus();
    }

    private void LanguageChanged(object? sender, EventArgs eventArgs) => Preview.Text = Texts.TextReport(report);
    protected override void OnClosed(EventArgs eventArgs)
    {
        Texts.Changed -= LanguageChanged;
        base.OnClosed(eventArgs);
    }

    private void CopyText(object sender, RoutedEventArgs eventArgs)
    {
        try { Clipboard.SetText(Preview.Text); }
        catch (System.Runtime.InteropServices.ExternalException) { MessageBox.Show(this, Texts["ClipboardBusy"], Texts["CopyText"]); }
    }
    private void SaveText(object sender, RoutedEventArgs eventArgs) => Save("txt", Preview.Text);
    private void SaveJson(object sender, RoutedEventArgs eventArgs) => Save("json", ReportExporter.ToJson(report));
    private void SaveCsv(object sender, RoutedEventArgs eventArgs) => Save("csv", ReportExporter.ToCsv(report));

    private void Save(string extension, string content)
    {
        var dialog = new SaveFileDialog { Title = Texts["SaveReport"], Filter = $"{extension.ToUpperInvariant()} {Texts["Report"]}|*.{extension}", FileName = $"ChangeTracker-{DateTime.Now:yyyyMMdd-HHmm}.{extension}", OverwritePrompt = true };
        if (dialog.ShowDialog(this) != true) return;
        var temporary = dialog.FileName + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temporary, content, new UTF8Encoding(false));
            File.Move(temporary, dialog.FileName, true);
            MessageBox.Show(this, Texts["ReportSaved"], "ChangeTracker");
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        { MessageBox.Show(this, Texts["ReportSaveFailed"], Texts["SaveReport"]); }
        finally { try { if (File.Exists(temporary)) File.Delete(temporary); } catch (IOException) { } catch (UnauthorizedAccessException) { } }
    }
}