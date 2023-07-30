using System;

namespace WhistleSharp.Models;

[Serializable]
public record struct WhistleSharpSaveData(
    string Title,
    string Composer,
    string Key,
    int TimeNumerator,
    int TimeDenominator,
    int Tempo,
    string Notes,
    string Filename,
    string Copyright,
    string OutputDirectory);