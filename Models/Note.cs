using WhistleSharp.ViewModels;

namespace WhistleSharp.Models;

public struct Note {
    public string NoteString { get; set; }
    public string Name       { get; set; }
    public string Markup     { get; set; }
    public string Fingering  { get; set; }
    public string Length     { get; set; }

    public Note(string note, string finger) {
        NoteString = note;
        Name = ConversionTools.TIN_WHISTLE_KEYS[SettingsViewModel.Instance.SelectedKey].Notes[note];
        Markup = ConversionTools.GetNoteMarkup(Name, note.EndsWith("+") || note.StartsWith("8"));
        Fingering = ConversionTools.FINGERING_DICT[note];
        Length = ConversionTools.NOTE_LENGTH_DICT.TryGetValue(finger, out var length) ? length : "";
    }
}