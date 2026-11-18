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
        var ass = Assembly.GetExecutingAssembly();
        string resName = $"ResourceSystemManagerGame.Locals.{lang}.json";
        using Stream? stream = ass.GetManifestResourceStream(resName);

        if(stream == null)
        {
            Console.WriteLine("Localisation error! File not found!");
            return;
        }
        using StreamReader reader = new StreamReader(stream);
        string jsonText = reader.ReadToEnd();

        Dict = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonText) ?? new Dictionary<string, string>();
    }

}
