using System;
using ReactiveUI;
#nullable disable
using WhistleSharp.Models;

namespace WhistleSharp.ViewModels;

public class SettingsViewModel : ViewModelBase {
    public static event Action<SettingsViewModel> OnUpdateSettings;
    public static event Action                    OnKeyChanged;
    public static SettingsViewModel               Instance { get; private set; }
    static        SheetData                       _sheetData;
    static        FileSettings                    _fileSettings;
    public        string[]                        KeyOptions => ConversionTools.AvailableKeys;

    public int SelectedKey {
        get => _sheetData.SelectedKey;
        set {
            this.RaiseAndSetIfChanged(ref _sheetData.SelectedKey, value);
            OnKeyChanged?.Invoke();
            OnPropertyChangedInvoke();
        }
    }

    public string Tempo {
        get => _sheetData.Tempo.ToString();
        set {
            this.RaiseAndSetIfChanged(ref _sheetData.Tempo, int.Parse(value));
            OnPropertyChangedInvoke();
        }
    }

    public string TimeSignature => _sheetData.TimeSignature;

    public int TimeSignatureNumerator {
        get => _sheetData.TimeSignatureNumerator;
        set {
            this.RaiseAndSetIfChanged(ref _sheetData.TimeSignatureNumerator, value);
            OnPropertyChangedInvoke();
        }
    }

    public int TimeSignatureDenominator {
        get => _sheetData.TimeSignatureDenominator;
        set {
            this.RaiseAndSetIfChanged(ref _sheetData.TimeSignatureDenominator, value);
            OnPropertyChangedInvoke();
        }
    }

    public SheetData SheetData {
        get => _sheetData;
        set => this.RaiseAndSetIfChanged(ref _sheetData, value);
    }

    public FileSettings FileSettings {
        get => _fileSettings;
        set => this.RaiseAndSetIfChanged(ref _fileSettings, value);
    }

    public string Title {
        get => _fileSettings.Title;
        set {
            this.RaiseAndSetIfChanged(ref _fileSettings.Title, value);
            OnPropertyChangedInvoke();
        }
    }

    public string Copyright {
        get => _fileSettings.Copyright;
        set {
            this.RaiseAndSetIfChanged(ref _fileSettings.Copyright, value);
            OnPropertyChangedInvoke();
        }
    }

    public string Composer {
        get => _fileSettings.Composer;
        set {
            this.RaiseAndSetIfChanged(ref _fileSettings.Composer, value);
            OnPropertyChangedInvoke();
        }
    }

    public string Filename {
        get => _fileSettings.Filename;
        set {
            this.RaiseAndSetIfChanged(ref _fileSettings.Filename, value);
            OnPropertyChangedInvoke();
        }
    }

    public string OutputDirectory {
        get => _fileSettings.OutputDirectory;
        set => this.RaiseAndSetIfChanged(ref _fileSettings.OutputDirectory, value);
    }

    public void SetKey(int key) => _sheetData.SelectedKey = key;

    public string GetKey() => KeyOptions[SheetData.SelectedKey];

    public void UpdateSettings() => OnUpdateSettings?.Invoke(this);

    public SettingsViewModel() {
        if (Instance != null) return;
        Instance = this;
        SheetData = new SheetData {
            SelectedKey = 0,
            Tempo = 120,
            TimeSignatureNumerator = 3,
            TimeSignatureDenominator = 4
        };

        FileSettings = new FileSettings {
            Title = "Untitled",
            Composer = "Unknown",
            Copyright = "",
            Filename = "untitled",
            OutputDirectory = System.IO.Directory.GetCurrentDirectory()
        };
    }
}