using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using YTSubConverterAvalonia.Services;
using YTSubConverterAvalonia.ViewModels;
using YTSubConverterAvalonia.Views;

namespace YTSubConverterAvalonia;

public class App : Application
{
    private MainWindow? _mainWindow;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _mainWindow = new MainWindow();
            var viewModel = new MainWindowViewModel(new AvaloniaFileDialogService(_mainWindow));

            DataContext = viewModel;
            _mainWindow.DataContext = viewModel;

            viewModel.RequestShowMainWindow = ShowMainWindow;
            viewModel.RequestExitApplication = () => desktop.Shutdown();

            desktop.MainWindow = _mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ShowMainWindow()
    {
        if (_mainWindow is null) return;

        _mainWindow.Show();
        _mainWindow.WindowState = WindowState.Normal;
        _mainWindow.Activate();
    }
}