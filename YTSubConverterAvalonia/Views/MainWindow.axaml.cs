using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Messaging;
using YTSubConverterAvalonia.ViewModels;

namespace YTSubConverterAvalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AddHandler(DragDrop.DragOverEvent, DragOver);
        AddHandler(DragDrop.DropEvent, Drop);
        
        WeakReferenceMessenger.Default.Register<StyleOptionsVisibilityChangedMessage>(this, (_, m) =>
        {
            Height = m.StyleState ? 500 : 125;
        });
    }
    
    private static void DragOver(object? sender, DragEventArgs e)
    {
        // TODO: make this only register copy effect for the file formats we actually want
        e.DragEffects = e.Data.Contains(DataFormats.FileNames) ? DragDropEffects.Copy : DragDropEffects.None;
    }
    
    private void Drop(object? sender, DragEventArgs e)
    {
        if (DataContext is MainWindowViewModel mainWindow) mainWindow.LoadFile(e.Data.GetFileNames().ToList()[0]);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if(DataContext is MainWindowViewModel mainWindow) mainWindow.SaveStylesOnClose();
        base.OnClosing(e);
    }
}