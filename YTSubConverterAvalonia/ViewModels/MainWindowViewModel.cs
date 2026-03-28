using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ObservableCollections;
using YTSubConverter.Shared;
using YTSubConverter.Shared.Formats;
using YTSubConverter.Shared.Formats.Ass;
using YTSubConverter.Shared.Util;
using YTSubConverterAvalonia.Services;

namespace YTSubConverterAvalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Dictionary<string, AssStyleOptions> _styleOptions;
    private Dictionary<string, AssStyle> _styles = new();
    private readonly HashSet<string> _builtInStyleNames;
    private AssStyle? _defaultStyle;
    private DateTime _lastAutoConvertTime = DateTime.MinValue;
    private readonly FileSystemWatcher _subtitleModifyWatcher = new();
    private readonly FileSystemWatcher _subtitleRenameWatcher = new();
    private readonly IFileDialogService _fileService;
    
    public MainWindowViewModel(IFileDialogService fileDialogService)
    {
        _fileService = fileDialogService;
        
        // UI localization goes here

        List<AssStyleOptions> builtInStyleOptions = AssStyleOptionsList.LoadFromString(Resources.DefaultStyleOptions);
        List<AssStyleOptions> customStyleOptions = AssStyleOptionsList.LoadFromFile();
        _styleOptions = customStyleOptions.Concat(builtInStyleOptions).ToDictionaryOverwrite(o => o.Name);
        _builtInStyleNames = builtInStyleOptions.Select(o => o.Name).ToHashSet();
        _subtitleModifyWatcher.Changed += OnFileModified;
        _subtitleRenameWatcher.Changed += OnTmpFileChanged;
        
        ClearUI();
    }
    public MainWindowViewModel() : this(new NoOpFileDialogService())
    {
        // Constructor for use in the IDE previewer since it doesn't have a file dialog service
    }

    [ObservableProperty] public partial ObservableList<AssStyleOptions> DataSource { get; set; } = [];
    [ObservableProperty] public partial bool IsStyleOptionsVisible { get; set; } = false;
    [ObservableProperty] public partial bool IsConvertAvailable { get; set; } = false;
    [ObservableProperty] public partial bool IsAutoConvertAvailable { get; set; } = false;
    [ObservableProperty] public partial bool IsStyleOptionsAvailable { get; set; } = false;
    [ObservableProperty] public partial bool IsAutoConvertActive { get; set; } = false;
    [ObservableProperty] public partial bool ConvertTextVisibility { get; set; } = false;
    [ObservableProperty] public partial string ConvertedTextMessage { get; set; } = "";
    [ObservableProperty] public partial string InputFilePath { get; set; } = "";
    [ObservableProperty] public partial AssStyleOptions? SelectedItem { get; set; } = null;
    [ObservableProperty] public partial int SelectedIndex { get; set; } = -1;

    partial void OnSelectedItemChanged(AssStyleOptions? value)
    {
        if (value is null)
        {
            PreviewHtml = "";
            return;
        }
        
        var style = _styles[value.Name];
        IsShadowsEnabled = style.HasShadow;

        if (style is { HasOutline: true, HasOutlineBox: false })
        {
            IsGlowChecked = true;
            IsGlowEnabled = false;
        }
        else
        {
            IsGlowChecked = style.HasShadow && value.ShadowTypes.Contains(ShadowType.Glow);
        }
        
        IsBevelChecked = style.HasShadow && value.ShadowTypes.Contains(ShadowType.Bevel);
        IsSoftShadowChecked = style.HasShadow && value.ShadowTypes.Contains(ShadowType.SoftShadow);
        IsHardShadowChecked = style.HasShadow && value.ShadowTypes.Contains(ShadowType.HardShadow);
        
        var currentWordTextColor = value.CurrentWordTextColor;
        var currentWordOutlineColor = value.CurrentWordOutlineColor;
        var currentWordShadowColor = value.CurrentWordShadowColor;

        IsUseForKaraokeChecked = value.IsKaraoke;
        IsHighlightForCurrentWordChecked = value.IsKaraoke && !currentWordTextColor.IsEmpty;
        
        IsCurrentWordTextColorEnabled = IsHighlightForCurrentWordChecked;
        CurrentWordTextColor = IsCurrentWordTextColorEnabled ? ColorUtil.ToHtml(currentWordTextColor) : "";
        
        IsCurrentWordOutlineColorEnabled = IsHighlightForCurrentWordChecked && style is { HasOutline: true, HasOutlineBox: false };
        CurrentWordOutlineColor = IsCurrentWordOutlineColorEnabled ? ColorUtil.ToHtml(currentWordOutlineColor) : "";
        
        IsCurrentWordShadowColorEnabled = IsHighlightForCurrentWordChecked && style.HasShadow;
        CurrentWordShadowColor = IsCurrentWordShadowColorEnabled ? ColorUtil.ToHtml(currentWordShadowColor) : "";
        
        
        UpdateStylePreview();
    }
    
    [ObservableProperty] public partial bool IsGlowChecked { get; set; } = false;
    [ObservableProperty] public partial bool IsGlowEnabled { get; set; } = false;
    [ObservableProperty] public partial bool IsShadowsEnabled { get; set; } = false;
    [ObservableProperty] public partial bool IsBevelChecked { get; set; } = false;
    [ObservableProperty] public partial bool IsSoftShadowChecked { get; set; } = false;
    [ObservableProperty] public partial bool IsHardShadowChecked { get; set; } = false;
    [ObservableProperty] public partial bool IsUseForKaraokeChecked { get; set; } = false;
    [ObservableProperty] public partial bool IsHighlightForCurrentWordChecked { get; set; } = false;
    [ObservableProperty] public partial string CurrentWordTextColor { get; set; } = "";
    [ObservableProperty] public partial bool IsCurrentWordTextColorEnabled { get; set; } = false;
    [ObservableProperty] public partial string CurrentWordOutlineColor { get; set; } = "";
    [ObservableProperty] public partial bool IsCurrentWordOutlineColorEnabled { get; set; } = false;
    [ObservableProperty] public partial string CurrentWordShadowColor { get; set; } = "";
    [ObservableProperty] public partial bool IsCurrentWordShadowColorEnabled { get; set; } = false;
    [ObservableProperty] public partial string PreviewHtml { get; set; } = "";

    partial void OnIsGlowCheckedChanged(bool value)
    {
        SelectedItem?.SetShadowTypeEnabled(ShadowType.Glow, value);
        UpdateStylePreview();
    }

    partial void OnIsBevelCheckedChanged(bool value)
    {
        SelectedItem?.SetShadowTypeEnabled(ShadowType.Bevel, value);
        UpdateStylePreview();
    }

    partial void OnIsSoftShadowCheckedChanged(bool value)
    {
        SelectedItem?.SetShadowTypeEnabled(ShadowType.SoftShadow, value);
        UpdateStylePreview();
    }

    partial void OnIsHardShadowCheckedChanged(bool value)
    {
        SelectedItem?.SetShadowTypeEnabled(ShadowType.HardShadow,  value); 
        UpdateStylePreview();
    }
    
    partial void OnIsUseForKaraokeCheckedChanged(bool value)
    {
        SelectedItem?.IsKaraoke = value;
        IsHighlightForCurrentWordChecked = false;
        UpdateStylePreview();
    }
    
    

    [RelayCommand]
    private void ToggleStyleOptions()
    {
        IsStyleOptionsVisible = !IsStyleOptionsVisible;
        WeakReferenceMessenger.Default.Send(new StyleOptionsVisibilityChangedMessage(IsStyleOptionsVisible));
    }

    //TODO: See how YTSubConverter manages it's user interface
    //      https://github.com/arcusmaximus/YTSubConverter/blob/master/YTSubConverter.UI.Linux/MainWindow.cs#L130

    [RelayCommand]
    private async Task Convert()
    {
        try
        {
            var inputExtension = Path.GetExtension(InputFilePath).ToLower();
            SubtitleDocument outputDoc;
            string outputExtension;
        
            switch (inputExtension)
            {
                case ".ass":
                {
                    var inputDoc = new AssDocument(InputFilePath, _styleOptions.Values.ToList());
                    outputDoc = new YttDocument(inputDoc);
                    outputExtension = ".ytt";
                    
                    RefreshStyleList(inputDoc);
                    break;
                }
        
                case ".ytt":
                case ".srv3":
                {
                    // Should there be some kind of way to create VisualizingAssDocument from here?
                    // Currently, it can only be done as a command line option
                    
                    var inputDoc = new YttDocument(InputFilePath);
                    outputDoc = new AssDocument(inputDoc);
                    outputExtension = inputExtension == ".ytt" ? ".reverse.ass" : ".ass";
                    break;
                }
        
                case ".sbv":
                {
                    var inputDoc = new SbvDocument(InputFilePath);
                    outputDoc = new SrtDocument(inputDoc);
                    outputExtension = ".srt";
                    break;
                }
        
                default:
                {
                    var inputDoc = SubtitleDocument.Load(InputFilePath);
                    outputDoc = new YttDocument(inputDoc);
                    outputExtension = ".ytt";
                    break;
                }
            }
        
            var outputFilePath = Path.ChangeExtension(InputFilePath, outputExtension);
            outputDoc.Save(outputFilePath);
        
            ConvertedTextMessage = "Successfully converted: " + Path.GetFileName(outputFilePath);
            ConvertTextVisibility = true;
            await Task.Delay(4000);
            ConvertTextVisibility = false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void OnTmpFileChanged(object sender, FileSystemEventArgs e)
    {
        PerformAutoConvert();
    }

    private void OnFileModified(object sender, FileSystemEventArgs e)
    {
        PerformAutoConvert();
    }

    private void PerformAutoConvert()
    {
        if ((DateTime.Now - _lastAutoConvertTime).TotalMilliseconds < 100) return;
        
        Thread.Sleep(100);
        _ = Convert();
        _lastAutoConvertTime = DateTime.Now;
    }

    [RelayCommand]
    private async Task LoadSubtitleButton()
    {
        var selectedFilePath = await _fileService.PickSingleFileAsync();
        if (string.IsNullOrWhiteSpace(selectedFilePath))
        {
            return;
        }

        LoadFile(selectedFilePath);
    }

    [RelayCommand]
    private void ToggleAutoConvert()
    {
        _subtitleModifyWatcher.EnableRaisingEvents = IsAutoConvertActive;
        _subtitleRenameWatcher.EnableRaisingEvents = IsAutoConvertActive;
        if (IsAutoConvertActive)
            _ = Convert();
    }

    public void LoadFile(string filePath)
    {
        ClearUI();
        
        try
        {
            var doc = SubtitleDocument.Load(filePath);
            PopulateUI(filePath, doc);
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            ClearUI();
            throw;
        }
    }

    private void PopulateUI(string filePath, SubtitleDocument doc)
    {
        InputFilePath = filePath;

        var assDoc = doc as AssDocument;
        if (assDoc is not null)
        {
            RefreshStyleList(assDoc);
            IsStyleOptionsAvailable = true;
        }
        IsAutoConvertAvailable = true;
        IsAutoConvertActive= false;
        
        _subtitleModifyWatcher.EnableRaisingEvents = false;
        _subtitleModifyWatcher.Path = Path.GetDirectoryName(filePath) ?? throw new DirectoryNotFoundException("Could not get directory name from file path");
        _subtitleModifyWatcher.Filter = Path.GetFileName(filePath);
        
        _subtitleRenameWatcher.EnableRaisingEvents = false;
        _subtitleRenameWatcher.Path = Path.GetDirectoryName(filePath) ?? throw new DirectoryNotFoundException("Could not get directory name from file path. Wait, how did the last one pass and this one fail?");
        _subtitleRenameWatcher.Filter = Path.GetFileNameWithoutExtension(filePath) + "_tmp_*" + Path.GetExtension(filePath);
        
        IsConvertAvailable = true;
    }

    private void RefreshStyleList(AssDocument assDoc)
    {
        _styles = assDoc.Styles.ToDictionary(s => s.Name);
        _defaultStyle = assDoc.DefaultStyle;
        
        foreach (var style in _styles.Where(style => !_styleOptions.ContainsKey(style.Key)))
            _styleOptions.Add(style.Key, new AssStyleOptions(style.Value));
        
        // this looks butt ugly and there has to be a better way for this to work
        DataSource = new ObservableList<AssStyleOptions>(assDoc.Styles.Select(s => _styleOptions[s.Name]));

        var styleIndex = assDoc.Styles.IndexOf(s => s.Name == SelectedItem?.Name);
        SelectedIndex = styleIndex >= 0 ? styleIndex : 0; 
        // no need to update SelectedItem it auto follows. Causes some janky behavior if you manually make them follow
    }

    private void ClearUI()
    {
        _styles = new Dictionary<string, AssStyle>();
        _defaultStyle = null;
        InputFilePath = "";
        DataSource = [];
        UpdateStylePreview();
        IsStyleOptionsAvailable = false;
        IsAutoConvertActive = false;
        IsAutoConvertAvailable = false;
        IsConvertAvailable = false;
    }

    public void SaveStylesOnClose()
    {
        AssStyleOptionsList.SaveToFile(
            _styleOptions.Where(p => !_builtInStyleNames.Contains(p.Key))
                .Select(p => p.Value)
        ); 
    }

    private void UpdateStylePreview()
    {
        if(SelectedItem is null)
        {
            PreviewHtml = "";
            return;
        }
        
        var style = _styles[SelectedItem.Name];
        var html = HtmlStylePreviewGenerator.Generate(style, SelectedItem, _defaultStyle, 1);
        
        PreviewHtml = "data:text/html;base64," + System.Convert.ToBase64String(Encoding.UTF8.GetBytes(html));
    }
}

internal sealed class NoOpFileDialogService : IFileDialogService
{
    public Task<string?> PickSingleFileAsync()
    {
        return Task.FromResult<string?>(null);
    }
}

public class StyleOptionsVisibilityChangedMessage(bool styleState)
{
    public bool StyleState { get;} = styleState;
}