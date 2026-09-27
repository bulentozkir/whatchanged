using System.Windows;

namespace PCChangeTracker.App.Controls;

/// <summary>
/// Lets a ComboBox show a compact template in its closed selection box while the open list keeps the fuller item
/// template. The ComboBox template in Themes/Controls.xaml falls back to the item template when this is unset.
/// </summary>
public static class ComboBoxAssist
{
    public static readonly DependencyProperty SelectionTemplateProperty = DependencyProperty.RegisterAttached("SelectionTemplate",
        typeof(DataTemplate), typeof(ComboBoxAssist), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static DataTemplate? GetSelectionTemplate(DependencyObject element) => (DataTemplate?)element.GetValue(SelectionTemplateProperty);
    public static void SetSelectionTemplate(DependencyObject element, DataTemplate? value) => element.SetValue(SelectionTemplateProperty, value);
}
