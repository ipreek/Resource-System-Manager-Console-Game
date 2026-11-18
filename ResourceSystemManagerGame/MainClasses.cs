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
    const float lvlPow = 1.2F;

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
    private Resource resourceProducedFrom;
    private string buildName;
    private string genName;
    private static readonly List<Resource> _resourceList = new List<Resource>();

    Resource(string Name, float Factor, float Price, bool IsProducable, Resource? resType, string BuildName, string Gen_name)
    {
        genName = Gen_name;
        name = Name; factor = Factor; price = Price; isProducable = IsProducable; buildName = BuildName;
        if (resType != null)
        {
            resourceProducedFrom = resType;
        }
        ResourceList.Add(this);
    }
    public static List<Resource> ResourceList => _resourceList;

    public float Price{get{return price;}set{price=value;}}
    public float Factor{get{return factor;}}
    public string Name{get{return name;}}
    public bool IsProducable{get{return isProducable;}}
    public string BuildName{get{return buildName;}}
    public string GenName{get{return genName;}}
    public Resource ResourceProducedFrom{get{return resourceProducedFrom;}}

    public static void ResCreate(){
    Resource Coal = new(Loc.Dict["res_Coal"], 0.7F, 57.14F, false, null, Loc.Dict["build_Coal"], Loc.Dict["gen_Coal"]);
    Resource Ore = new(Loc.Dict["res_Ore"], 1F, 40F, false, null, Loc.Dict["build_Ore"], Loc.Dict["gen_Ore"]);
    Resource Plant = new(Loc.Dict["res_Plant"], 1.2F, 33.33F, false, null, Loc.Dict["build_Plant"], Loc.Dict["gen_Plant"]);
    Resource Metal = new(Loc.Dict["res_Metal"], 0.8F, 75F, true, Ore, Loc.Dict["build_Metal"], Loc.Dict["gen_Metal"]);
    Resource Energy = new(Loc.Dict["res_Energy"], 0.56F, 167.41F, true, Coal, Loc.Dict["build_Energy"], Loc.Dict["gen_Energy"]);
    Resource Food = new(Loc.Dict["res_Food"], 0.9F, 104.16F, true, Plant, Loc.Dict["build_Food"], Loc.Dict["gen_Food"]);
    }
}