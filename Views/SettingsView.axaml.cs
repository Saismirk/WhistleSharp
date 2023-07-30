#nullable enable
using Avalonia.Controls;
using WhistleSharp.ViewModels;

namespace WhistleSharp.Views;

public partial class SettingsView : UserControl {
    SettingsViewModel ViewModel => (SettingsViewModel)DataContext!;

    public SettingsView() {
        InitializeComponent();
        KeysComboBox.SelectionChanged += KeyComboBoxOnSelectionChanged;
        foreach (var key in ViewModel.KeyOptions) {
            KeysComboBox.Items.Add(key);
        }

        KeysComboBox.SelectedIndex = ViewModel.SheetData.SelectedKey;
        SettingsViewModel.OnUpdateSettings += UpdateSettings;
    }

    void UpdateSettings(SettingsViewModel settings) {
        TempoBox.Text = settings.Tempo;
        TimeSignatureNumerator.Text = settings.TimeSignatureNumerator.ToString();
        TimeSignatureDenominator.Text = settings.TimeSignatureDenominator.ToString();
        Title.Text = settings.Title;
        Composer.Text = settings.Composer;
        Copyright.Text = settings.Copyright;
        Filename.Text = settings.Filename;
        Output.Text = settings.OutputDirectory;
    }

    void KeyComboBoxOnSelectionChanged(object? sender, SelectionChangedEventArgs e) =>
        ViewModel.SetKey(KeysComboBox.SelectedIndex);
}