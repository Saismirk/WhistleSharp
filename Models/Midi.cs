using System.Diagnostics;
using System.Threading.Tasks;

namespace WhistleSharp.Models;

public static class Midi {
    const string SOUNDFONT_PATH = "Resources/sf2/Tin_Whistle_AIR.sf2";
    const string MIDI_PATH = "Temp/preview.mid";

    static bool     _isPlaying;
    static Process? _subprocessObject;
    public static async void PlayMidiAsync(string midiFilePath = MIDI_PATH, string soundfontPath = SOUNDFONT_PATH) {
        if (_isPlaying) {
            _subprocessObject?.Kill();
            _isPlaying = false;
            return;
        }

        await Task.Yield();
        await Task.Run(async () => {
            _subprocessObject = new Process() {
                StartInfo = {
                    FileName = "fluidsynth/bin/fluidsynth.exe",
                    Arguments = $"-ni {soundfontPath} {midiFilePath} -r 44100",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            _isPlaying = true;
            _subprocessObject.Start();
            await _subprocessObject.WaitForExitAsync();
        });
    }
}