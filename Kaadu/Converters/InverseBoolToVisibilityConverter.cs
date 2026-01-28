using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Kaadu;

/// <summary>
/// Converts boolean to visibility, with inverse option.
/// </summary>
public sealed class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            var invert = parameter?.ToString() == "Invert";
            var result = invert ? !boolValue : boolValue;
            return result ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
