#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using WhistleSharp.Models;

namespace WhistleSharp.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public static MainWindowViewModel? Instance;

    static bool                    _isOperationInProcess;
    static CancellationTokenSource _cancellationTokenSource = new();
    static bool                    _isMultiPagePreview      = false;
    static int                     _previewPageCount        = 1;
    static int                     _previewPage             = 1;
    static bool                    _isDocumentDirty;
    const  string                  PREVIEW_OUTPUT_PATH = "Temp/preview";
    const  string                  TEMP_FOLDER         = "Temp/";

    string        _selectedFilePath;
    static string _appHeader = "WhistleSharp - Untitled";

    public ObservableCollection<NoteReference> NoteReferences    { get; private set; } = new();
    public InputViewModel                      InputViewModel    { get; }              = new();
    public SettingsViewModel                   SettingsViewModel { get; }              = new();

    public ICommand GenerateCommand { get; }
    public ICommand PlayMidiCommand { get; }
    public ICommand OpenFileCommand { get; }
    public ICommand SaveFileCommand { get; }
    public ICommand SaveAsCommand   { get; }

    public bool IsMultiPagePreview {
        get => _isMultiPagePreview;
        set => this.RaiseAndSetIfChanged(ref _isMultiPagePreview, value);
    }

    public int PreviewPageCount {
        get => _previewPageCount;
        private set => this.RaiseAndSetIfChanged(ref _previewPageCount, value);
    }

    public int PreviewPage {
        get => _previewPage;
        set {
            this.RaiseAndSetIfChanged(ref _previewPage, value);
            UpdatePreviewImage();
        }
    }

    static bool IsDocumentDirty {
        set {
            if (Instance is not null)
            Instance.AppHeader = value && !_appHeader.EndsWith("*") ? _appHeader + "*" : _appHeader.Trim('*');
            _isDocumentDirty = value;
        }
    }

    public string SelectedFilePath {
        get => _selectedFilePath;
        set => this.RaiseAndSetIfChanged(ref _selectedFilePath, value);
    }

    public string AppHeader {
        get => _appHeader;
        private set => this.RaiseAndSetIfChanged(ref _appHeader, value);
    }

    public MainWindowViewModel() {
        Instance = this;
        Task.Run(ClearTempFolder);
        _selectedFilePath = string.Empty;
        GenerateCommand = ReactiveCommand.Create(Generate);
        PlayMidiCommand = ReactiveCommand.Create(PlayMidi);
        OpenFileCommand = ReactiveCommand.CreateFromTask(OpenFile);
        SaveFileCommand = ReactiveCommand.CreateFromTask(SaveFile);
        SaveAsCommand = ReactiveCommand.CreateFromTask(SaveAs);
        OnPropertyChanged += SetDocumentDirty;
        SettingsViewModel.OnKeyChanged += UpdateNoteReferences;
        //GeneratePreview(_cancellationTokenSource);
        UpdateNoteReferences();
    }

    public async void UpdateNoteReferences() {
        await Task.Yield();
        _ = ConversionTools.GetKey(SettingsViewModel.GetKey());
        App.MainWindow?.UpdateReferenceTable(GetNoteReferences());
    }

    ObservableCollection<string[]> GetNoteReferences() =>
        new(ConversionTools.TIN_WHISTLE_KEYS[SettingsViewModel.SelectedKey].Notes
                           .Select(x => (x.Key.ToString(), x.Value.Trim('\'').Trim(',')))
                           .Where(x => ConversionTools.NOTE_DICT.ContainsKey(x.Item2))
                           .Select(x => new[] {
                                x.Item1, x.Item1.EndsWith("+")
                                             ? ConversionTools.NOTE_DICT[x.Item2].ToUpper()
                                             : ConversionTools.NOTE_DICT[x.Item2]
                            }));

    void Generate() {
        new Sheet().SetKey(SettingsViewModel.GetKey())
                   .SetTempo(SettingsViewModel.SheetData.Tempo.ToString())
                   .SetTime(SettingsViewModel.SheetData.TimeSignature)
                   .SetTitle(SettingsViewModel.FileSettings.Title)
                   .SetComposer(SettingsViewModel.FileSettings.Composer)
                   .SetCopyright(SettingsViewModel.FileSettings.Copyright)
                   .AddNotes(InputViewModel.Input)
                   .OutputPng($"{SettingsViewModel.FileSettings.OutputDirectory}/{SettingsViewModel.FileSettings.Filename}");
    }

    void PlayMidi() {
        Midi.PlayMidiAsync();
    }

    async void GeneratePreview(CancellationTokenSource cts) {
        var previewPaths = GetPreviewImagePaths();
        ClearTempFolder(previewPaths);
        await WaitUntilOperationFinished();
        _isOperationInProcess = true;
        await Task.Run(() => new Sheet().SetKey(SettingsViewModel.GetKey())
                                        .SetTempo(SettingsViewModel.SheetData.Tempo.ToString())
                                        .SetTime(SettingsViewModel.SheetData.TimeSignature)
                                        .SetTitle(SettingsViewModel.FileSettings.Title)
                                        .SetComposer(SettingsViewModel.FileSettings.Composer)
                                        .SetCopyright(SettingsViewModel.FileSettings.Copyright)
                                        .SetMidi(true)
                                        .AddNotes(InputViewModel.Input)
                                        .OutputPngAsync($"{SettingsViewModel.FileSettings.OutputDirectory}/{PREVIEW_OUTPUT_PATH}"));
        UpdatePreviewImage();
        _isOperationInProcess = false;
    }

    static async Task WaitUntilOperationFinished() {
        while (_isOperationInProcess) {
            await Task.Delay(100);
        }
    }

    static string[] GetPreviewImagePaths() => Directory.GetFiles($"{Directory.GetCurrentDirectory()}/{TEMP_FOLDER}")
                                                       .Where(path => path.EndsWith("png"))
                                                       .ToArray();

    Avalonia.Media.Imaging.Bitmap? LoadPreviewImages() {
        var paths = GetPreviewImagePaths();
        if (paths.Length == 0) {
            return null;
        }

        var pathCount = paths.Length;
        IsMultiPagePreview = pathCount > 1;
        PreviewPageCount = pathCount;
        if (PreviewPage > PreviewPageCount) {
            PreviewPage = PreviewPageCount;
        }

        return new Avalonia.Media.Imaging.Bitmap(paths[PreviewPage - 1]);
    }

    static void ClearTempFolder(ReadOnlySpan<string> paths) {
        try {
            foreach (var path in paths) {
                File.Delete(path);
            }
        }
        catch { }
    }

    static void ClearTempFolder() {
        try {
            var paths = Directory.GetFiles($"{Directory.GetCurrentDirectory()}/{TEMP_FOLDER}");
            foreach (var path in paths) {
                File.Delete(path);
            }
        }
        catch { }
    }

    async void UpdatePreviewImage() {
        var previewImage = LoadPreviewImages();
        await Task.Yield();
        App.MainWindow.UpdatePreviewImage(previewImage);
    }

    void SetDocumentDirty() {
        if (Sheet.operationInProcess) {
            _cancellationTokenSource.Cancel();
        }

        _cancellationTokenSource = new();
        GeneratePreview(_cancellationTokenSource);
        IsDocumentDirty = true;
    }

    async Task OpenFile() {
        var fileDialog = new OpenFileDialog {
            Title = "Open WhistleSharp File",
            AllowMultiple = false
        };

        fileDialog.Filters.Add(new FileDialogFilter {
            Name = "WhistleSharp Files",
            Extensions = { "json" },
        });
        var result = await fileDialog.ShowAsync(App.MainWindow);
        if (result is { Length: > 0 }) {
            SelectedFilePath = result[0];
            AppHeader = $"WhistleSharp - {Path.GetFileName(SelectedFilePath)}";
            var settings = SettingsViewModel;
            var (sheetData, fileSettings, notes, succeeded) = await SaveManager.LoadSaveDataAsync(SelectedFilePath);
            if (succeeded) {
                settings.SheetData = sheetData;
                settings.FileSettings = fileSettings;
                settings.UpdateSettings();
                InputViewModel.UpdateInput(notes);
                IsDocumentDirty = false;
                return;
            }

            Debug.WriteLine($"Failed to load file at {SelectedFilePath}");
        }
    }

    async Task SaveFile() {
        if (string.IsNullOrEmpty(SelectedFilePath)) {
            await SaveAs();
            return;
        }

        Save();
    }

    void Save() {
        SaveManager.SaveData(SettingsViewModel.SheetData, SettingsViewModel.FileSettings, InputViewModel.Input, SelectedFilePath);
        IsDocumentDirty = false;
    }

    async Task SaveAs() {
        var fileDialog = new SaveFileDialog();
        fileDialog.Title = "Save WhistleSharp File";

        fileDialog.Filters.Add(new FileDialogFilter {
            Name = "WhistleSharp Files",
            Extensions = { "json" },
        });
        var result = await fileDialog.ShowAsync(App.MainWindow);
        if (result is { Length: > 0 }) {
            SelectedFilePath = result;
            Save();
        }
    }
}