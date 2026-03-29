using System.Threading.Tasks;

namespace YTSubConverterAvalonia.Services;

public interface IFileDialogService
{
    Task<string?> PickSingleFileAsync();
}