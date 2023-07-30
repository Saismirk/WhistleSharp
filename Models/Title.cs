using System.Text;
#nullable disable
namespace WhistleSharp.Models;

public class Title {
    public string TitleText { get; set; }
    public string Composer  { get; set; }
    public string Copyright   { get; set; }

    public Title SetTitle(string title) {
        TitleText = title;
        return this;
    }

    public Title SetComposer(string composer) {
        Composer = composer;
        return this;
    }

    public Title SetCopyright(string copyright) {
        Copyright = copyright;
        return this;
    }

    public string GetTitle() {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("\\version \"2.22.2\"");
        stringBuilder.AppendLine("\\header {");
        if (!string.IsNullOrEmpty(TitleText)) {
            stringBuilder.AppendLine($"  title = \"{TitleText}\"");
        }

        if (!string.IsNullOrEmpty(Composer)) {
            stringBuilder.AppendLine($"  composer = \"{Composer}\"");
        }

        if (!string.IsNullOrEmpty(Copyright)) {
            stringBuilder.AppendLine($"  tagline = \"{Copyright}\"");
        }

        stringBuilder.AppendLine("}");
        return stringBuilder.ToString();
    }
}