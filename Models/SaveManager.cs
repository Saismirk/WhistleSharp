using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhistleSharp.ViewModels;

namespace WhistleSharp.Models;

public static class SaveManager {
    static WhistleSharpSaveData GenerateSaveData(SheetData sheetData, FileSettings fileSettings, string notes) =>
        new(
            fileSettings.Title,
            fileSettings.Composer,
            sheetData.SelectedKey.ToString(),
            sheetData.TimeSignatureNumerator,
            sheetData.TimeSignatureDenominator,
            sheetData.Tempo,
            notes,
            fileSettings.Filename,
            fileSettings.Copyright,
            fileSettings.OutputDirectory
           );

    static void SaveDataToJson(WhistleSharpSaveData saveData, string path) {
        var json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public static void SaveData (SheetData sheetData, FileSettings fileSettings, string notes, string path) =>
        SaveDataToJson(GenerateSaveData(sheetData, fileSettings, notes), path);

    static async Task<WhistleSharpSaveData?> LoadDataFromPathAsync(string path) {
        var json = await File.ReadAllTextAsync(path);
        try {
            return JsonConvert.DeserializeObject<WhistleSharpSaveData>(json);
        }
        catch {
            return null;
        }
    }

    public static async Task<(SheetData sheetData, FileSettings fileSettings, string notes, bool succeeded)> LoadSaveDataAsync(string path) {
        var loadResult = await LoadDataFromPathAsync(path);

        if (loadResult is null) {
            return (new SheetData(), new FileSettings(), string.Empty, false);
        }

        var saveData = loadResult.Value;
        var sheetData = new SheetData {
            SelectedKey = int.Parse(saveData.Key),
            Tempo = saveData.Tempo,
            TimeSignatureNumerator = saveData.TimeNumerator,
            TimeSignatureDenominator = saveData.TimeDenominator
        };
        var fileSettings = new FileSettings {
            Title = saveData.Title,
            Composer = saveData.Composer,
            Filename = saveData.Filename,
            Copyright = saveData.Copyright,
            OutputDirectory = saveData.OutputDirectory
        };
        return (sheetData, fileSettings, saveData.Notes, true);
    }
}