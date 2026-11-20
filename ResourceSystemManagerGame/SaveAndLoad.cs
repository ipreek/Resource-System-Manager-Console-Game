using System;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Runtime.InteropServices.JavaScript;


namespace RSM;

public class playerCopy
{
    public DateTime SaveDate {get;set;}
    public string ID {get;set;}
    public string Name {get;set;}
    public int Level {get;set;}
    public int XP {get;set;}
    public float Money {get;set;}
    public List<BuildCopy> BuildingList {get;set;}  = new List<BuildCopy>(); 
    public IDictionary<string, int> ResourceList {get;set;} = new Dictionary<string, int>();
    public Dictionary<string, float> ResCopyList {get;set;} = new Dictionary<string, float>();
}
public class BuildCopy
{
    public string Name {get;set;}
    public int Level {get;set;}
    public int XP {get;set;}
    public float TotalIncome {get;set;}
    public float TotalSpend {get;set;}
    public string Resource {get;set;}
    public int lastedSecs {get;set;}
}

public class ResCopy
{
    public string ID{get;set;}
    public float Price{get;set;}
}

public class SaveAndLoad
{

    static object SaveLock = new object();
    
    public static void Save(Player p)
    {
        List<BuildCopy> buildListCopy = new List<BuildCopy>();
        List<ResCopy> ResCopyList_local = new List<ResCopy>();
        List<Resource> ResList = Resource.ResourceList;
        foreach(var res in ResList)
        {
            ResCopy resCopy = new ResCopy ();
            resCopy.ID = res.ID;
            resCopy.Price = res.Price;
            ResCopyList_local.Add(resCopy);
        }
        DateTime dt = DateTime.UtcNow;
            playerCopy playerC = new playerCopy
            {
                ID = p.ID,
                SaveDate = dt,
                Name = p.Name,
                Level = p.Level,
                XP = p.Expirience,
                Money = p.Money,
                BuildingList = {},
                ResourceList = {},
                ResCopyList = {}
            };

            foreach(var b in p.GetBuildings())
            {
                BuildCopy bC = new BuildCopy{Name = b.Name, lastedSecs = b.LastedSeconds, Level = b.Level, XP = b.Expirience, Resource = b.ProduceType.ID, TotalIncome = b.TotalIncome, TotalSpend = b.TotalSpending};
                playerC.BuildingList.Add(bC);
            }

            foreach(var res in p.GetStorage())
            {
                playerC.ResourceList.Add(res.Key.ID, res.Value);
            }
            foreach(var resC in ResCopyList_local)
            {
                playerC.ResCopyList.Add(resC.ID, resC.Price);
            }

        string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string combinedPath = Path.Combine(myDocs, "RSM", "saves");

        if (!Directory.Exists(combinedPath))
        {
            Directory.CreateDirectory(combinedPath);
        }

        string tempPath = Path.Combine(combinedPath, $"save_{p.ID}.tmp");
        string filePath = Path.Combine(combinedPath, $"save_{p.ID}.json");

        var options = new JsonSerializerOptions {WriteIndented = true};

        string jsonText = JsonSerializer.Serialize<playerCopy>(playerC, options);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        int retries = 0;
        lock(SaveLock){
            while( retries <6 ){
                try{
                    File.WriteAllText(tempPath, jsonText);

                    File.Move(tempPath, filePath);
                    Console.WriteLine(Loc.Dict["succ_save"]);
                    break;
                }catch (IOException ex)
                {
                    retries++;
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(60);
                }
            }
        }
        }

        public static List<playerCopy> LoadAllFiles()
    {
        string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string combinedPath = Path.Combine(directoryPath, "RSM", "saves");

        if (!(Directory.Exists(combinedPath)))
        {
            Directory.CreateDirectory(combinedPath);
        }

        List<playerCopy> jsonStrings = new List<playerCopy>();

        string[] fileNames = Directory.GetFiles(combinedPath); 

        foreach(string s in fileNames)
        {
            string filePath = Path.Combine(combinedPath, s);
            string getText = File.ReadAllText(filePath);
            playerCopy playerC = null;
            try{
                playerC = JsonSerializer.Deserialize<playerCopy>(getText);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            jsonStrings.Add(playerC);
        }

        return jsonStrings;
    }

    public static Player? Load(playerCopy pC)
    {
        Dictionary<Resource, int> resDict = new Dictionary<Resource, int>();
        List<Resource> resList = Resource.ResourceList;
        List<Building> buildings = new List<Building>();

        foreach(var res in pC.ResourceList)
        {
            List<Resource> findedRes = (resList.Where<Resource>(r => r.ID == res.Key)).ToList<Resource>();
            Resource neededRes = findedRes[0];
            resDict.Add(neededRes, res.Value);
        }
        foreach(BuildCopy bC in pC.BuildingList)
        {
            List<Resource> resType = (Resource.ResourceList.Where<Resource>(r => r.ID == bC.Resource)).ToList<Resource>();
            Resource res = resType[0];
            Building newBuild = new Factory(res, bC.Name,null , bC.XP, bC.Level, bC.lastedSecs, bC.TotalIncome, bC.TotalSpend);
            buildings.Add(newBuild);
        }
        foreach(var resC in pC.ResCopyList)
        {
            List<Resource> findedRes = (resList.Where<Resource>(r => r.ID == resC.Key)).ToList<Resource>();
            Resource neededRes = findedRes[0];
            neededRes.Price = resC.Value;
        }

        Player player = new Player(pC.Name, pC.Level, pC.XP, resDict, buildings, pC.ID, pC.Money);
        return player;
    }
    }

