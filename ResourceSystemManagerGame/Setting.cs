using System;
using System.IO;
using System.Text.Json;

namespace RSM;

public class Setting
{
    public static Dictionary<string, string> listOfConfig {get;set;} = new Dictionary<string, string>();

    public static void GetCon()
    {
        string localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string combinedPath = Path.Combine(localData, "RSM", "Data", "Settings");
        string filePath = Path.Combine(combinedPath, "config.json");

        string fileText = File.ReadAllText(filePath);
        listOfConfig = JsonSerializer.Deserialize<Dictionary<string, string>>(fileText);
    }
}
