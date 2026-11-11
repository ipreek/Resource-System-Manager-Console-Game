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
                throw new Exception("Wrong entity name! Name was empty or contains only spaces.");
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
                Console.WriteLine("You reached {0} level!", this.Level);
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

    public Resource(string Name, float Factor, float Price, bool IsProducable, Resource? resType)
    {
        name = Name; factor = Factor; price = Price; isProducable = IsProducable;
        if (resType != null)
        {
            resourceProducedFrom = resType;
        }
    }

    public float Price{get{return price;}set{price=value;}}
    public float Factor{get{return factor;}}
    public string Name{get{return name;}}
    public bool IsProducable{get{return isProducable;}}
    public Resource ResourceProducedFrom{get{return resourceProducedFrom;}}

    public static readonly Resource Coal = new("Coal", 0.7F, 57.14F, false, null);
    public static readonly Resource Ore = new("Ore", 1F, 40F, false, null);
    public static readonly Resource Plant = new("Plant", 1.2F, 33.33F, false, null);
    public static readonly Resource Metal = new("Metal", 0.8F, 75F, true, Ore);
    public static readonly Resource Energy = new("Energy", 0.56F, 167.41F, true, Coal);
    public static readonly Resource Food = new("Food", 0.9F, 104.16F, true, Plant);

}