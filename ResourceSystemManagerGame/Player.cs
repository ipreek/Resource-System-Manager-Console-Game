using System;
using System.Linq.Expressions;
using System.Xml.XPath;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
#nullable disable warnings
namespace RSM;

//файл для класса игрока
public class Player : Entity
{
    private List<Building>? playerBuildings = new List<Building> ();
    private readonly Object _buildingLock = new();
    private float money;
    private bool hasDebt;
    private string id;
    const int defaultXp = 10;

    private Dictionary<Resource, int> Storage = new Dictionary<Resource, int>()
    {
        {Resource.Coal, 0},{Resource.Energy,0},{Resource.Food,0},{Resource.Metal,0},{Resource.Ore,0},
        {Resource.Plant,0}
    };

    public void AddResource(Resource res, int ammount)
    {
        lock (_buildingLock){
        Storage[res] += ammount;
        }
    }

    public string ID
    {
        get{return id;}
        set{id = value;}
    }
    public void RemoveResource(Resource res, int ammount)
    {
        int resAmm = Storage[res];
        if(ammount >=0){
            if (resAmm - ammount < 0)
            {
                throw new InvalidOperationException();
            }
            else
            {
                lock(_buildingLock){
                Storage[res] -= ammount;}
            }
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public bool HasDebt
    {
        get{return hasDebt;}
        set{hasDebt = value;}
    }
    public float Money
    {
        get {return money;}
        set {money = value;}
    }

    public void AddMoney(float plus)
    {
        float defaultMoney = this.Money; //нужно для проверки, если дефолтмани меньше 0, а итоговое значение больше нуля, то убирает статус долга
        lock(_buildingLock){
        this.Money += plus;}
        if (defaultMoney<0 && this.Money>=0)
        {
            this.HasDebt = false;
        }

    }
    public void RemoveMoney(float minus)
    {
        float defaultMoney = this.Money; //нужно для проверки, если дефолтмани больше нуля, а итогове значение меньше нуля, то дает статус долга
        lock (_buildingLock){
        this.Money -= minus;}
        if (this.Money < 0 && defaultMoney >=0)
        {
            this.HasDebt = true;
        }
    }

    public void AddBuilding(Building b)
    {
        lock (_buildingLock){
        playerBuildings.Add(b);
        b.Owner = this;
        }
    }
    public void RemoveBuilding(Building b)
    {
        lock (_buildingLock){
        playerBuildings.Remove(b);
        b.Owner = null;
        }
    }
    public List<Building> GetBuildings()
    {
        List<Building> newList = new List<Building>{};
        foreach(Building b in this.playerBuildings)
        {
            newList.Add(b);
        }
        lock(_buildingLock){
        return newList;
        }
    }

    public void Work()
    {
        int Level = this.Level;
        float Payment = (Level*GameMethods.spendMultiply)*GameMethods.cashAmount;
        int Xp = defaultXp;
        this.LevelUp(Xp);
        this.AddMoney(Payment);
        Console.WriteLine("You earned {0} money!", Payment);
        Console.WriteLine("You gained {0} experience point!", Xp);
        GameMethods.Waiting(this);
    }

    public int GetXpCapForPlayer()
    {
        return xpCap(this.Level);
    }

    public Dictionary<Resource, int> GetStorage()
    {
        Dictionary<Resource, int> resDict = new Dictionary<Resource, int>();
        foreach(var res in Storage)
        {
            resDict.Add(res.Key, res.Value);
        }
        lock(_buildingLock){
        return resDict;}
    }

    public Player()
    {
        this.Level = 1;
        this.Expirience = 0;
    }
    
    public Player(string name)
    {
        this.Level=1;
        this.Expirience = 0;
        this.Name = name;
    }
    public Player(string name, int level, int xp, Dictionary<Resource, int> resDict, List<Building> buildList)
    {
        this.Level = level;
        this.Expirience = xp;
        this.Name = name;
        foreach(var res in resDict)
        {
            this.AddResource(res.Key, res.Value);
        }
        foreach(Building b in buildList)
        {
            this.AddBuilding(b);
        }
    }
}
