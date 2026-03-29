# YTSubConverterAvalonia

It's [YTSubConverter](https://github.com/arcusmaximus/YTSubConverter), but implemented in Avalonia UI

I made this mostly for fun and to learn about using YTSubConverter.Shared for use in a different project, but figured it would be interesting to share.
Since it uses the YTSubConverter.Shared library, it implements all the same features that YTSubConverter has, but just with a different UI library.

## Compared to YTSubConverter.UI.Win/Mac/Linux

|                            | YTSubConverterAvalonia | YTSubConverter.UI.X |
|----------------------------|:----------------------:|:-------------------:|
| YTSubConverter Conversions |          Yes           |         Yes         |
| Drag & Drop Files          |    Yes<sup>1</sup>     |         Yes         |
| Dark Theme                 |          Yes           |         No          |
| System tray application    |          Yes           |         No          |


<sup>1</sup> Drag & Drop is only supported on Windows, not on Linux/macOS due to limitations in Avalonia's drag & drop support on those platforms.

## Credits
- [arcusmaximus](https://github.com/arcusmaximus/) – For creating YTSubConverter and the YTSubConverter.Shared library, which made this project possible.