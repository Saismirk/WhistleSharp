using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WhistleSharp.ViewModels;

namespace WhistleSharp.Models;

public static class ConversionTools {
    public const string SEMICIRCLE_PATH =
        @"\override #'(filled . #t)\path #0 #'((moveto 0 -0.7)(lineto 0 0.7)(curveto 0 0.7 0.7 0.7 0.7 0)(curveto 0.7 0 0.7 -0.7 0-0.7)(closepath))";

    public const string PLUS_PATH =
        @"\halign #-1.2 \path #0.2 #'((moveto 0.6 0)(lineto -0.6 0)(moveto 0 0.6)(lineto 0 -0.6)) \vspace #-0.4 ";

    public static readonly Dictionary<string, string> KEY_DICT = new() {
        { "D", "d" },
        { "B", "b" },
        { "Bb", "bes" },
        { "Eb", "ees" },
        { "E", "e" },
        { "A", "a" },
        { "F", "f" },
        { "C", "c" },
        { "G", "g" },
        { "Low F", "f," },
        { "Low C", "c," },
        { "Low D", "d," },
        { "Loe E", "e," },
    };

    public static readonly Dictionary<string, string> NOTE_DICT = new() {
        { "c", "c" },
        { "cis", "c♯/d♭" },
        { "des", "c♯/d♭" },
        { "d", "d" },
        { "dis", "d♯/e♭" },
        { "ees", "d♯/e♭" },
        { "e", "e" },
        { "f", "f" },
        { "fis", "f♯/g♭" },
        { "ges", "f♯/g♭" },
        { "g", "g" },
        { "gis", "g♯/a♭" },
        { "aes", "g♯/a♭" },
        { "a", "a" },
        { "ais", "a♯/b♭" },
        { "bes", "a♯/b♭" },
        { "b", "b" }
    };

    public static List<TinWhistleKey> GetKeys() {
        var jsonString = File.ReadAllText("Resources/keys.json");
        var keys       = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>[]>(jsonString);
        return keys?.Select(key => (TinWhistleKey)key)
                    .ToList()
               ?? new List<TinWhistleKey>();
    }

    public static readonly Dictionary<int, string> TIN_WHISTLE_HOLES = new() {
        { 0, TwHoleMarkup(0, 0, 0, 0, 0, 0) },
        { 1, TwHoleMarkup(1, 0, 0, 0, 0, 0) },
        { -1, TwHoleMarkup(-1, 0, 0, 0, 0, 0) },
        { 2, TwHoleMarkup(1, 1, 0, 0, 0, 0) },
        { -2, TwHoleMarkup(1, -1, 0, 0, 0, 0) },
        { 3, TwHoleMarkup(1, 1, 1, 0, 0, 0) },
        { -3, TwHoleMarkup(1, 1, -1, 0, 0, 0) },
        { 4, TwHoleMarkup(1, 1, 1, 1, 0, 0) },
        { -4, TwHoleMarkup(1, 1, 1, -1, 0, 0) },
        { 5, TwHoleMarkup(1, 1, 1, 1, 1, 0) },
        { -5, TwHoleMarkup(1, 1, 1, 1, -1, 0) },
        { 6, TwHoleMarkup(1, 1, 1, 1, 1, 1) },
        { -6, TwHoleMarkup(1, 1, 1, 1, 1, -1) },
        { 7, TwHoleMarkup(0, 1, 1, 0, 0, 0) },
        { 8, TwHoleMarkup(0, 1, 1, 1, 1, 1) }
    };

    public static readonly Dictionary<string, string> FINGERING_DICT = new() {
        { "6", GetWoodwindMarkup(6) },
        { "6+", GetWoodwindMarkup(6, true) },
        { "6,", GetWoodwindMarkup(-6) },
        { "5", GetWoodwindMarkup(5) },
        { "5+", GetWoodwindMarkup(5, true) },
        { "5,", GetWoodwindMarkup(-5) },
        { "4", GetWoodwindMarkup(4) },
        { "4+", GetWoodwindMarkup(4, true) },
        { "3", GetWoodwindMarkup(3) },
        { "3+", GetWoodwindMarkup(3, true) },
        { "3,", GetWoodwindMarkup(-3) },
        { "2", GetWoodwindMarkup(2) },
        { "2+", GetWoodwindMarkup(2, true) },
        { "2,", GetWoodwindMarkup(-2) },
        { "1", GetWoodwindMarkup(1) },
        { "1+", GetWoodwindMarkup(1, true) },
        { "1,", GetWoodwindMarkup(-1) },
        { "0", GetWoodwindMarkup(0) },
        { "0+", GetWoodwindMarkup(0, true) },
        { "0,", GetWoodwindMarkup(0) },
        { "7", GetWoodwindMarkup(7) },
        { "7+", GetWoodwindMarkup(7, true) },
        { "8", GetWoodwindMarkup(8) },
        { "8+", GetWoodwindMarkup(8, true) }
    };

    public static readonly Dictionary<string, string> NOTE_LENGTH_DICT = new() {
        { "l", "\\longa" },
        { "w", "1" },
        { "ww", "\\breve" },
        { "h", "2" },
        { "h.", "2." },
        { "q", "4" },
        { "q.", "4." },
        { "q'", "8" },
        { "q'.", "8." },
        { "q''", "16" },
        { "q''.", "16." },
        { "q'''", "32" },
        { "q'''.", "32." },
        { "q''''", "64" },
        { "q''''.", "64." },
        { "q'''''", "128" },
        { "q''''''.", "128." },
        { "q''''''", "256" },
        { "q'''''''", "256." },
        { "", "" }
    };

    static string GetWoodwindMarkup(int index, bool overblown = false) {
        if (!TIN_WHISTLE_HOLES.TryGetValue(index, out var markup)) return TIN_WHISTLE_HOLES[0];
        markup = markup.Replace("overblow", overblown ? PLUS_PATH : "\" \" \\vspace #-0.4 ");
        return markup;
    }

    public static readonly List<TinWhistleKey> TIN_WHISTLE_KEYS = GetKeys();

    public static string[] AvailableKeys { get; } = TIN_WHISTLE_KEYS.Select(key => key.Name)
                                                                    .ToArray();

    public static TinWhistleKey GetKey(string keyName) => TIN_WHISTLE_KEYS?.FirstOrDefault(key => key.Name == keyName)
                                                          ?? new TinWhistleKey("", new Dictionary<string, string>());

    public static List<Note> GetNotes(string key, string notes) {
        List<Note> notesList = new();
        _ = GetKey(key);
        if (string.IsNullOrEmpty(notes)) return notesList;
        var notePattern = new Regex(@"([0-8][+,]?)((?:l|ww|w|h|q'{0,6})\.?)?");
        var matches     = notePattern.Matches(notes);
        foreach (Match match in matches) {
            try {
                var note = new Note(match.Groups[1].Value, match.Groups[2].Value);
                notesList.Add(note);
            }
            catch (KeyNotFoundException) {
                throw new KeyNotFoundException($"Note {match.Groups[1].Value} is not available in key {key}");
            }
        }

        return notesList;
    }

    static string TwHole(int filled) {
        if (filled == -1) {
            return "\\halign #-1 \\combine " + SEMICIRCLE_PATH + " \\draw-circle #0.7 #0.1 ##f \\vspace #-0.3";
        }

        var filling = filled == 0 ? "##f" : "##t";
        return $"\\halign #-1 \\draw-circle #0.7 #0.1 {filling} \\vspace #-0.35";
    }

    public static string TwHoleMarkup(int one = 0, int two = 0, int three = 0, int four = 0, int five = 0, int six = 0) =>
        $@"^\markup {{
        \lower #-3.2
        \dir-column {{
            overblow
            {TwHole(six)}
            {TwHole(five)}
            {TwHole(four)}
            \vspace #+0.15
            {TwHole(three)}
            {TwHole(two)}
            {TwHole(one)}
            notemarkupid
        }}
    }}";

    public static string GetNoteMarkup(string note, bool highOctave = false) {
        var     notePattern = new Regex(@"([a-g](?:es|is)?)");
        var     match       = notePattern.Match(note);
        var     noteName    = match.Groups[1].Value.Trim('\'').AsSpan();
        string? sharp       = null;
        var     sb          = new StringBuilder();
        if (noteName.EndsWith("es")) {
            sb.Append(noteName[..^2]);
            sharp = "\\flat";
        } else if (noteName.EndsWith("is")) {
            sb.Append(noteName[..^2]);
            sharp = "\\sharp";
        } else {
            sb.Append(noteName);
        }

        if (highOctave) {
            sb[0] = char.ToUpper(sb[0]);
        }

        var noteString = sb.ToString();
        return !string.IsNullOrEmpty(sharp)
                   ? $"\\concat{{\\tiny\"{noteString}\"\\teeny{sharp}}}"
                   : $"\"{noteString}\"";
    }

    public static string GetNoteName(Note note) =>
        note.Name + note.Length + (TIN_WHISTLE_KEYS[SettingsViewModel.Instance.SelectedKey].Notes.ContainsKey(note.NoteString)
                                       ? note.Fingering.Replace("notemarkupid", note.Markup)
                                       : "");
}