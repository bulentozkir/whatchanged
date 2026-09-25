using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PCChangeTracker.App.Controls;

/// <summary>Returns the first bound value that is not null or unset, e.g. an optional icon tint falling back to the foreground.</summary>
public sealed class FirstNonNullConverter : IMultiValueConverter
{
    public object? Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture) =>
        values.FirstOrDefault(value => value is not null && value != DependencyProperty.UnsetValue) ?? DependencyProperty.UnsetValue;

    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
