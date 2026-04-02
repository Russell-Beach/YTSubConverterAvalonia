using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using YTSubConverterAvalonia.ViewModels;

namespace YTSubConverterAvalonia.Views;

public partial class MainWindow : Window
{
    private static readonly HashSet<string> SupportedSubtitleExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".ass", ".sbv", ".ytt", ".srv3", ".ttml", ".xml", ".dfxp"
    };

    public MainWindow()
    {
        InitializeComponent();
        AddHandler(DragDrop.DragOverEvent, DragOver);
        AddHandler(DragDrop.DropEvent, Drop);

        WeakReferenceMessenger.Default.Register<StyleOptionsVisibilityChangedMessage>(this,
            (_, m) => { Height = m.StyleState ? 500 : 125; });
    }

    private static void DragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = GetFirstSupportedFile(e.DataTransfer) is not null
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        var firstFile = GetFirstSupportedFile(e.DataTransfer);
        if (firstFile is not null && DataContext is MainWindowViewModel mainWindow)
            mainWindow.LoadFile(firstFile.Path.LocalPath);
    }

    private static IStorageItem? GetFirstSupportedFile(IDataTransfer dataTransfer)
    {
        if (!dataTransfer.Contains(DataFormat.File)) return null;

        return dataTransfer.TryGetFiles()?.FirstOrDefault(file =>
            SupportedSubtitleExtensions.Contains(Path.GetExtension(file.Path.LocalPath)));
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (DataContext is MainWindowViewModel mainWindow) mainWindow.SaveStylesOnClose();
        base.OnClosing(e);
    }
}