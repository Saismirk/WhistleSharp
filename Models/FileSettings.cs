using System;

namespace WhistleSharp.ViewModels;

[Serializable]
public struct FileSettings {
    public string Title;
    public string Composer;
    public string Copyright;
    public string Filename;
    public string OutputDirectory;
}