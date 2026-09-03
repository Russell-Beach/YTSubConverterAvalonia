using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Color = System.Drawing.Color;

namespace YTSubConverterAvalonia.Converters;

public class AvaloniaToSystemColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color) return Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        return Colors.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Avalonia.Media.Color color) return Color.FromArgb(color.A, color.R, color.G, color.B);
        return Color.Empty;
    }
}