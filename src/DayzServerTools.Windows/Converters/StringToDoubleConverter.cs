using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DayzServerTools.Windows.Converters;

internal class StringToDoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string str = value as string;
        if (double.TryParse(str, NumberStyles.Number, culture, out var num))
        {
            return num;
        }
        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.ToString();
    }
}
