using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace WhistleSharp.Models;

public class Sheet {
    Thread   _thread;
    Process _subprocessObject;
    Staff    _staff;
    Title    _header;

    public static bool operationInProcess;
    public Sheet() {
        _staff = new Staff();
        _header = new Title();
    }

    public Sheet AddNotes(string notes) {
        _staff.AddNotes(notes);
        return this;
    }

    public Sheet SetMidi(bool midi) {
        _staff.SetMidi(midi);
        return this;
    }

    public Sheet SetKey(string key) {
        _staff.SetKey(key);
        return this;
    }

    public Sheet SetTime(string time) {
        _staff.SetTime(time);
        return this;
    }

    public Sheet SetTempo(string tempo) {
        _staff.SetTempo(tempo);
        return this;
    }

    public Sheet SetClef(string clef) {
        _staff.SetClef(clef);
        return this;
    }

    public Sheet SetTitle(string title) {
        _header.SetTitle(title);
        return this;
    }

    public Sheet SetComposer(string composer) {
        _header.SetComposer(composer);
        return this;
    }

    public Sheet SetCopyright(string tagline) {
        _header.SetCopyright(tagline);
        return this;
    }

    public string GetOutput(string filename = "output") {
        CheckOutputFolder(filename);
        var outputFilename = $"{filename}.ly";
        File.WriteAllText(outputFilename, GetHeader() + GetStaff());
        var lilypath = Path.Combine(Directory.GetCurrentDirectory(), "LilyPond/usr/bin/lilypond.exe");
        return lilypath;
    }

    public string OutputPdf(string filename = "output") {
        var outputFilename = GetOutput(filename);
        var lilypath       = Path.Combine(Directory.GetCurrentDirectory(), "LilyPond/usr/bin/lilypond.exe");
        var processStartInfo = new ProcessStartInfo(lilypath) {
            Arguments = $"-o \"{Path.GetDirectoryName(outputFilename)}\" \"{outputFilename}\"",
            CreateNoWindow = true,
            UseShellExecute = false
        };
        Process.Start(processStartInfo)?.WaitForExit();
        return $"{filename}.pdf";
    }

    public string OutputPng(string filename = "output", Action onComplete = null) {
        var executablePath = GetOutput(filename);
        if (_thread is { IsAlive: true }) {
            _thread.Join();
        }

        _thread = new Thread(() => StartGenPngSubprocess(executablePath, filename, onComplete));
        _thread.IsBackground = true;
        _thread.Priority = ThreadPriority.Highest;
        _thread.Start();
        return $"{filename}.png";
    }

    public async Task<string> OutputPngAsync(string filename = "output") {
        var executablePath = GetOutput(filename);
        Debug.WriteLine($"OutputPngAsync: {Thread.CurrentThread.ManagedThreadId}");
        await StartGenPngSubprocessAsync(executablePath, filename);
        return $"{filename}.png";
    }

    static void CheckOutputFolder(string filename) {
        var outputFolder = Path.GetDirectoryName(filename);
        if (!string.IsNullOrEmpty(outputFolder) && !Directory.Exists(outputFolder)) {
            Directory.CreateDirectory(outputFolder);
        }
    }

    string GetHeader() => _header.GetTitle();
    string GetStaff()  => _staff.GetStaff();
    void StartGenPngSubprocess(string path, string filename, Action onComplete) {
        try {
            _subprocessObject?.Kill();
        }
        catch {
            // Ignore exceptions
        }

        CheckOutputFolder(filename);
        var startupInfo = new ProcessStartInfo {
            FileName = path,
            Arguments = $"-s -djob-count=16 --png -dresolution=90 -o \"{Path.GetDirectoryName(filename)}\" \"{filename}.ly\"",
            CreateNoWindow = true,
            UseShellExecute = false
        };
        operationInProcess = true;
        _subprocessObject = Process.Start(startupInfo);
        _subprocessObject?.WaitForExit();
        operationInProcess = false;
        onComplete?.Invoke();
    }

    public static async Task<bool> WaitForOperationToCompleteAsync(CancellationTokenSource cts) {
        while (operationInProcess) {
            if (cts.IsCancellationRequested) {
                return false;
            }
            await Task.Delay(16);
        }
        return true;
    }

    async Task StartGenPngSubprocessAsync(string path, string filename) {
        try {
            _subprocessObject?.Kill();
        }
        catch {
            // Ignore exceptions
        }

        CheckOutputFolder(filename);
        var startupInfo = new ProcessStartInfo {
            FileName = path,
            Arguments = $"-s -djob-count=16 --png -dresolution=90 -o \"{Path.GetDirectoryName(filename)}\" \"{filename}.ly\"",
            CreateNoWindow = true,
            UseShellExecute = false
        };
        Debug.WriteLine($"StartGenPngSubprocessAsync1: {Thread.CurrentThread.ManagedThreadId}");
        _subprocessObject = Process.Start(startupInfo);
        await _subprocessObject!.WaitForExitAsync();
        Debug.WriteLine($"StartGenPngSubprocessAsync2: {Thread.CurrentThread.ManagedThreadId}");
    }
}