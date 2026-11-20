using System;
using System.IO;
using System.Reflection;

namespace RSM;

public class OnFirstLaunch
{
    public static void CreateJson()
    {
        string LocalDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string rsmDataPath = Path.Combine(LocalDataPath, "RSM", "Data");
        Assembly ass = Assembly.GetExecutingAssembly();
        string[] filesAss = ass.GetManifestResourceNames();
        if (!Directory.Exists(rsmDataPath))
        {
            Directory.CreateDirectory(rsmDataPath);
        }

        string locPath = Path.Combine(rsmDataPath, "Loc");
        if (!Directory.Exists(locPath))
        {
            Directory.CreateDirectory(locPath);}
            foreach(string stream in filesAss)
            {
                if (stream.Contains("Locals"))
                {
                    FileMode fm = FileMode.CreateNew;
                    Stream fileStream = ass.GetManifestResourceStream(stream);
                    StreamReader sr = new StreamReader(fileStream);
                    string jsonText = sr.ReadToEnd();
                    string fileName = stream.Replace("ResourceSystemManagerGame.Locals.", "");
                    string filePath = Path.Combine(locPath, fileName);

                    File.WriteAllText(filePath, jsonText);
                }
            }

        
    }
}
