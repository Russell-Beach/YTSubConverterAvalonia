using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace YTSubConverterAvalonia.Services;

public sealed class AvaloniaFileDialogService(Window ownerWindow) : IFileDialogService
{
    public async Task<string?> PickSingleFileAsync(string title, List<FilePickerFileType> fileTypeFilters)
    {
        var files = await ownerWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = fileTypeFilters
        });

        var selectedFile = files.Count > 0 ? files[0] : null;
        if (selectedFile is null) return null;

        var filePath = selectedFile.TryGetLocalPath();
        return string.IsNullOrWhiteSpace(filePath) ? null : filePath;
    }
}