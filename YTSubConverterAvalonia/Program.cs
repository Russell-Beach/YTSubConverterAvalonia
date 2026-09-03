using System;
using System.Runtime.InteropServices;
using Avalonia;
using YTSubConverter.Shared;
using YTSubConverterAvalonia.Services;
using OperatingSystem = System.OperatingSystem;

namespace YTSubConverterAvalonia;

internal sealed class Program
{
    private const int AttachParentProcess = -1;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (OperatingSystem.IsWindows()) AttachConsole(AttachParentProcess);
            using AvaloniaTextMeasurer textMeasurer = new();
            CommandLineHandler.Handle(args, textMeasurer);

            return;
        }

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }

    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(int dwProcessId);
}