using System;
using System.IO;
using System.Text.Json;
using System.Reflection;

namespace RSM;

public class Loc
{
    public static Dictionary<string, string> Dict = new Dictionary<string, string>();

    public static void GetLoc(string lang)
    {
        string LocalApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string rsmDataPath = Path.Combine(LocalApplicationData, "RSM", "Data", "Loc");
        string resName = Path.Combine(rsmDataPath, $"{lang}.json");

        if(!File.Exists(resName))
        {
            //Console.WriteLine(Loc.Dict["loc_error"]);
            return;
        }
        using StreamReader reader = new StreamReader(resName);
        string jsonText = reader.ReadToEnd();

        Dict = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonText) ?? new Dictionary<string, string>();
        Console.WriteLine(Loc.Dict["loc_succ"]);
    }

}
