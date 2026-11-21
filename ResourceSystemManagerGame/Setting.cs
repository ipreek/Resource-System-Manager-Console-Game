using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;

namespace RSM;

public class ResourceConfig
{
    public string ID {get;set;}
    public string Name {get;set;}
    public string GenName {get;set;}
    public float Factor {get;set;}
    public float Price {get;set;}
    public bool IsProducable {get;set;}
    public List<string> IDProducedFrom {get;set;}
    public string BuildName {get;set;}

    public ResourceConfig(){}
}
public static class Setting
{
    private static int DefaultPlayerXp {get;set;}
    private static int AmmountMultiply {get;set;}
    private static float XPMultiply {get;set;}
    private static float SpendMultiply {get;set;}
    private static int CashAmount {get;set;}
    private static int BuildFactor {get;set;}
    private static float EntityLvlPow {get;set;}
    private static int BuildingSpendAmmount {get;set;}
    private static int BuildingNeededSecs {get;set;}
    private static string Version {get;set;}

    public static Dictionary<string, string> listOfConfig {get;set;} = new Dictionary<string, string>();
    public static List<ResourceConfig> resListOfConfig {get;set;} = new List<ResourceConfig>();
    public static void GetCon()
    {
        string localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string combinedPath = Path.Combine(localData, "RSM", "Data", "Settings");
        string filePath = Path.Combine(combinedPath, "config.json");

        string fileText = File.ReadAllText(filePath);
        listOfConfig = JsonSerializer.Deserialize<Dictionary<string, string>>(fileText);
        
        Version = listOfConfig["Version"];
        DefaultPlayerXp = Int32.Parse(listOfConfig["DefaultPlayerXP"]);
        AmmountMultiply = Int32.Parse(listOfConfig["AmmountMultiply"]);
        XPMultiply = float.Parse(listOfConfig["XPMultiply"], CultureInfo.InvariantCulture);
        SpendMultiply = float.Parse(listOfConfig["SpendMultiply"], CultureInfo.InvariantCulture);
        CashAmount = Int32.Parse(listOfConfig["CashAmmount"]);
        BuildFactor = Int32.Parse(listOfConfig["BuildFactor"]);
        EntityLvlPow = float.Parse(listOfConfig["EntityLvlPow"], CultureInfo.InvariantCulture);
        BuildingSpendAmmount = Int32.Parse(listOfConfig["BuildingSpendAmmount"]);
        BuildingNeededSecs = Int32.Parse(listOfConfig["BuildingNeededSecs"]);

        GameMethods.ammountMultiply = AmmountMultiply;
        GameMethods.buildFactor = BuildFactor;
        GameMethods.cashAmount = CashAmount;
        GameMethods.spendMultiply = SpendMultiply;
        GameMethods.xpMultiply = XPMultiply;
        Player.defaultXp = DefaultPlayerXp;
        Entity.lvlPow = EntityLvlPow;
        Gameplay.version = Version;
        Building.neededSeconds = BuildingNeededSecs;
        Building.spendAmmount = BuildingSpendAmmount;
    }

    public static void GetResources()
    {
        string localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string combinedPath = Path.Combine(localData, "RSM", "Data", "Settings");
        string filePath = Path.Combine(combinedPath, "resources.json");

        string fileText = File.ReadAllText(filePath);
        resListOfConfig = JsonSerializer.Deserialize<List<ResourceConfig>>(fileText);

        foreach(ResourceConfig resCon in resListOfConfig)
        {
            new Resource(resCon.Name, resCon.Factor, resCon.Price, resCon.IsProducable, resCon.IDProducedFrom, resCon.BuildName, resCon.GenName, resCon.ID);
        }
    }
}
