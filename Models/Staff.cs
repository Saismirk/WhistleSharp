using System.Collections.Generic;
using System.Linq;
#nullable disable
namespace WhistleSharp.Models;

public class Staff {
    public bool                       Midi  { get; set; }
    public string                     Clef  { get; set; }
    public (string key, string scale) Key   { get; set; }
    public string                     Time  { get; set; }
    public string                     Tempo { get; set; }
    public bool                       Easy  { get; set; }

    public static List<string> Notes { get; private set; }

    public Staff() {
        Midi = false;
        Clef = "\\clef \"treble^8\" ";
        Key = ("D", "Major");
        Time = "\\time 4/4";
        Tempo = "90";
        Easy = false;
        Notes = new();
    }

    public Staff SetMidi(bool midi) {
        Midi = midi;
        return this;
    }

    public Staff SetClef(string clef) {
        Clef = " \\clef " + clef + " ";
        return this;
    }

    public Staff AddNotes(string notes) {
        if (string.IsNullOrEmpty(notes)) {
            Notes = new List<string>() { "\t\tr" };
        } else {
            Notes = ConversionTools.GetNotes(Key.key, notes)
                                   .Select(note => $"\t\t{ConversionTools.GetNoteName(note)}\n")
                                   .ToList();
        }

        return this;
    }

    public Staff SetKey(string key) {
        Key = (key, "Major");
        return this;
    }

    public Staff SetTime(string time) {
        Time = "\\time " + time;
        return this;
    }

    public Staff SetTempo(string tempo) {
        Tempo = tempo;
        return this;
    }

    public string GetStaff() {
        var melody = $@"\fixed bes'' {{
    \once \override TextScript.outside-staff-priority = ##f \textLengthOn
    \numericTimeSignature \key {ConversionTools.KEY_DICT[Key.Item1]} {Key.Item2.ToLower()} {GetEasyHeadsOn()} {Time} {Clef}
    {GetMelodyNotes()}
}}";
        var layout = @"\layout {
        indent = 2\cm
        \context {
            \StaffGroup
            \override StaffGrouper.staff-staff-spacing.basic-distance = #8
        }
    }";
        var midi = Midi ? $"\\midi {{\\tempo 4 = {Tempo}}}" : "";
        return $@"melody = {melody}
\score {{
    \new Staff \with{{
        \textLengthOn
        instrumentName = \markup {{
            \center-column {{ ""Tin Whistle""
                \line {{ ""Key of {Key.Item1}"" }}
            }}}}
        }}{{
        \melody
    }}
    {layout}
    {midi}
}}";
    }

    string GetEasyHeadsOn() => Easy ? "\\easyHeadsOn" : "";

    static string GetMelodyNotes() => string.Join("", Notes);
}