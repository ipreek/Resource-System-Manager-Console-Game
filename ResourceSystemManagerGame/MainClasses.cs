using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Runtime.Versioning;
using System.Xml.XPath;

//файл для основных классов игры

#nullable disable warnings 
namespace RSM;
public abstract class Entity
{
    private string name;
    private int xp;
    private int lvl;
    public static float lvlPow;

    public string Name
    {
        get{return name;}
        set
        {
            if (value != "" && GameMethods.IsOnlySpaces(value) == false)
            {
                name = value;
            }
            else
            {
                throw new Exception(Loc.Dict["empty_name"]);
            }
        }
    }

    public int Expirience
    {
        get{return xp;}
        set{xp = value;}
    }
    public int Level
    {
        get{return lvl;}
        set{lvl = value;}
    }

    protected static int xpCap(int lvl)
    {
        return (int)((Math.Pow(lvl, lvlPow))*100);
    }

    public void LevelUp(int addXp)
    {   

        if (addXp>0){
            int CurrentCap = xpCap(this.Level);
            this.Expirience += addXp; 
            while (this.Expirience >= CurrentCap) //каждое добавление уровня првоеряет, не больше ли опыт кэпа и в конце создает новый кап
            {
                this.Expirience -= CurrentCap;
                this.Level++;
                if (this is Player){
                Console.WriteLine(Loc.Dict["level_reach"], this.Level);}
                CurrentCap = xpCap(this.Level);
            }
        }
    }
}


public class Resource
{
    private string name;
    private float factor;
    private float price;
    private bool isProducable;
    private List<Resource> resourceProducedFrom;
    private string buildName;
    private string genName;
    private string id;
    private static List<Resource> _resourceList = new List<Resource>();

    public Resource(string Name, float Factor, float Price, bool IsProducable, List<string> IDProducedFrom, string BuildName, string Gen_name, string ID)
    {
        genName = Loc.Dict[Gen_name]; id=ID;
        name = Loc.Dict[Name]; factor = Factor; price = Price; isProducable = IsProducable; buildName = Loc.Dict[BuildName];
        if (IDProducedFrom != null)
        {
            foreach(string producedId in IDProducedFrom){
            List<Resource> resType = new List<Resource>();
            foreach(Resource res in _resourceList)
            {
                if (res.ID == producedId)
                {
                    resType.Add(res);
                }

                
            }
                if(resType == null)
                {
                    var DefRes = new Resource("Default", 0.5F, 35F, false, null, "DefaultBuild", "Default", "default");
                    resType.Add(DefRes);
                }

            resourceProducedFrom = resType;
        }}
        _resourceList.Add(this);
    }
    public static List<Resource> ResourceList => _resourceList;
    public string ID{get{return id;}}
    public float Price{get{return price;}set{price=value;}}
    public float Factor{get{return factor;}}
    public string Name{get{return name;}}
    public bool IsProducable{get{return isProducable;}}
    public string BuildName{get{return buildName;}}
    public string GenName{get{return genName;}}
    public List<Resource> ResourceProducedFrom{get{return resourceProducedFrom;}}

}