using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
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
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

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
        _mainWindow.WindowState = Avalonia.Controls.WindowState.Normal;
        _mainWindow.Activate();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
    }
}