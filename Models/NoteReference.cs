namespace WhistleSharp.ViewModels;

public class NoteReference {
    public string Hole { get; set; }
    public string Note { get; set; }

    public NoteReference(string hole, string note) {
        Hole = hole;
        Note = note;
    }
}