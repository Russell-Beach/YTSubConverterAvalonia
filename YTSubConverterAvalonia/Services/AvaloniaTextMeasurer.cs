using System.Drawing;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using YTSubConverter.Shared;

namespace YTSubConverterAvalonia.Services;

internal class AvaloniaTextMeasurer : ITextMeasurer
{
    private TextLayout? _lastLayout;
    private string? _lastText;
    private string? _lastFont;
    private float _lastSize;
    private bool _lastBold;
    private bool _lastItalic;
    
    public void Dispose()
    {
        _lastLayout = null;
    }

    public SizeF Measure(string text, string font, float size, bool bold, bool italic)
    {
        if (_lastLayout == null ||
            _lastText != text ||
            _lastFont != font ||
            _lastSize != size ||
            _lastBold != bold ||
            _lastItalic != italic)
        {
            var typeface = new Typeface(
                new FontFamily(font),
                italic ? FontStyle.Italic : FontStyle.Normal,
                bold ? FontWeight.Bold : FontWeight.Normal);

            _lastLayout = new TextLayout(
                text,
                typeface,
                size,
                Brushes.Black);

            _lastText = text;
            _lastFont = font;
            _lastSize = size;
            _lastBold = bold;
            _lastItalic = italic;
        }

        return new SizeF(
            (float)_lastLayout.WidthIncludingTrailingWhitespace * 0.97f, (float)_lastLayout.Height);
    }
}