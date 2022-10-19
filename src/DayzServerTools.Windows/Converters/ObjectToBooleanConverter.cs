using System;
using System.Globalization;
using System.Windows.Data;

namespace DayzServerTools.Windows.Converters;

internal class ObjectToBooleanConverter : IValueConverter
{
    private object _value = new();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
       return value is not null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? _value : null;
    }
}
