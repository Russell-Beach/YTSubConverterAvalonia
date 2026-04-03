# YTSubConverterAvalonia

It's [YTSubConverter](https://github.com/arcusmaximus/YTSubConverter), but implemented in Avalonia UI

![screenshot-darkmode.png](images/screenshot.png)

I made this mostly for fun and to learn about using YTSubConverter.Shared and improve on my MVVM skills for use in a
different project, but figured it
would be interesting to share.
Since it uses the YTSubConverter.Shared library, it implements all the same features that YTSubConverter has, but just
with a different UI library.

The only main feature differences beyond the UI can be found in the color pickers for highlighting karaoke words and system tray support when the application is closed while auto-convert is active. For the most part, it's probably best to just use [the version of YTSubConverter that is native to your platform](https://github.com/arcusmaximus/YTSubConverter/releases) unless you are a fan of using applications that have 10x the memory footprint.

## Compared to YTSubConverter.UI.Win/Mac/Linux

|                            | YTSubConverterAvalonia | YTSubConverter.UI.X |
|----------------------------|:----------------------:|:-------------------:|
| YTSubConverter Conversions |          Yes           |         Yes         |
| Unified platform UI code   |          Yes           |         No          |
| Drag & Drop Files          |    Yes<sup>1</sup>     |         Yes         |
| Dark Theme                 |          Yes           |         No          |
| System tray application    |          Yes           |         No          |

<sup>1</sup> Drag & Drop is only supported on Windows &
macOS, [not on Linux due to limitations in Avalonia's drag & drop
support.](https://github.com/AvaloniaUI/Avalonia/issues/6085)

## Credits

- [arcusmaximus](https://github.com/arcusmaximus/) – For creating YTSubConverter and the YTSubConverter.Shared library,
  which made this project possible.

## Things to do

Got time and want to commit? Here's things that could be done:

- Implement UI localization using the ResX files in YTSubConverter.Shared
- Implement an ITextMeasurer for Avalonia to give feedback for failed command line conversions
- If you see any code that you can improve, feel free to do so