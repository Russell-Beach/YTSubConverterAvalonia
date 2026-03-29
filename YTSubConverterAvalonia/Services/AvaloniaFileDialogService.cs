using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace YTSubConverterAvalonia.Services;

public sealed class AvaloniaFileDialogService(Window ownerWindow) : IFileDialogService
{
    public async Task<string?> PickSingleFileAsync()
    {
        var files = await ownerWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Subtitle File",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Advanced Substation Alpha files") { Patterns = ["*.ass"] },
                new FilePickerFileType("YouTube subtitles") { Patterns = ["*.sbv", "*.ytt", "*.srv3"] },
                new FilePickerFileType("Timed Text Markup Language") { Patterns = ["*.ttml", "*.xml", "*.dfxp"] }
            ]
        });

        var selectedFile = files.FirstOrDefault();
        if (selectedFile is null) return null;

        var filePath = selectedFile.TryGetLocalPath();
        return string.IsNullOrWhiteSpace(filePath) ? null : filePath;
    }
}