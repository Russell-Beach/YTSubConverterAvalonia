using System;
using System.Drawing;
using System.Globalization;
using Avalonia.Data.Converters;

namespace YTSubConverterAvalonia.Converters;

public class ColorToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color) return $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        return "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string colorString) return ColorTranslator.FromHtml(colorString);

        return Color.Empty;
    }
}