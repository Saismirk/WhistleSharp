namespace WhistleSharp.ViewModels;

public struct SheetData {
    public int SelectedKey;
    public int Tempo;
    public int TimeSignatureNumerator;
    public int TimeSignatureDenominator;
    public string TimeSignature => $"{TimeSignatureNumerator}/{TimeSignatureDenominator}";
}