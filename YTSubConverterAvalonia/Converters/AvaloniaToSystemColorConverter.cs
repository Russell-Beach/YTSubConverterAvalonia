using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace YTSubConverterAvalonia.Converters;

public class AvaloniaToSystemColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        return Colors.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        return System.Drawing.Color.Empty;
    }
}