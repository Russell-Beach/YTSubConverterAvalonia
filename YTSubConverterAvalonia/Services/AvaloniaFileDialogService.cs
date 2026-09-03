using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace YTSubConverterAvalonia.Services;

public sealed class AvaloniaFileDialogService(Window ownerWindow) : IFileDialogService
{
    public async Task<string?> PickSingleFileAsync()
    {
        var translatedFilePickerNames = YTSubConverter.Shared.Resources.SubtitleFileFilter.Split('|').Select(x => x.Trim()).ToArray();
        
        var files = await ownerWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Subtitle File",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType(translatedFilePickerNames[0]) { Patterns = ["*.ass"] },
                new FilePickerFileType(translatedFilePickerNames[2]) { Patterns = ["*.sbv", "*.ytt", "*.srv3"] },
                new FilePickerFileType(translatedFilePickerNames[4]) { Patterns = ["*.ttml", "*.xml", "*.dfxp"] }
            ]
        });

        var selectedFile = files.FirstOrDefault();
        if (selectedFile is null) return null;

        var filePath = selectedFile.TryGetLocalPath();
        return string.IsNullOrWhiteSpace(filePath) ? null : filePath;
    }
}