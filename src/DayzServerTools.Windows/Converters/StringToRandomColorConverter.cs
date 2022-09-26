using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

using DayzServerTools.Application.Services;

using MColor = System.Windows.Media.Color;

namespace DayzServerTools.Windows.Converters;

internal class StringToRandomColorConverter : IValueConverter
{
    private static Dictionary<string, MColor> _mapping = new Dictionary<string, MColor>();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var strValue = value as string;
        strValue ??= "";

        if (_mapping.ContainsKey(strValue))
        {
            return _mapping[strValue];
        }
        else
        {
            var color = RandomColorPicker.PickColor(strValue);
            var media_color = ToMediaColor(color);
            _mapping[strValue] = media_color;
            return media_color;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static MColor ToMediaColor(Color color)
    {
        return MColor.FromArgb(color.A, color.R, color.G, color.B);
    }
}
