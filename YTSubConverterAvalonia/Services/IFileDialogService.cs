using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace YTSubConverterAvalonia.Services;

public interface IFileDialogService
{
    Task<string?> PickSingleFileAsync(string title, List<FilePickerFileType> fileTypeFilters);
}