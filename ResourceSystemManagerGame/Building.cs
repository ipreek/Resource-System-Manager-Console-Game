using System;
using System.Diagnostics;
#nullable disable warnings
namespace RSM;

//файл для класса построек и классов построек
public class Building : Entity
{
    const int ammountMultiply = GameMethods.ammountMultiply;
    const float xpMultiply = GameMethods.xpMultiply;
    const float spendMultiply = GameMethods.spendMultiply;
    private float produceQ;
    private Resource produceType;
    private Player owner;
    protected int lastedSeconds = 0;
    const int neededSeconds = 30;
    private float totalSpending = 0;
    private float totalIncome = 0;

    public float TotalSpending{get{return totalSpending;} set{totalSpending = value;}}
    public float TotalIncome{get{return totalIncome;} set{totalIncome = value;}}
    public float TotalProfit{get{return this.TotalIncome-this.TotalSpending;}}
    public int LastedSeconds {get{return this.lastedSeconds;}set{lastedSeconds = value;}}


    public float ProduceQ
    {
        get {return produceQ;}
        set {produceQ = value;}
    }
    public Resource ProduceType
    {
        get {return produceType;}
        set {produceType = value;}
    }

    public Player Owner
    {
        get{return owner;}
        set{owner = value;}
    }

    public int GetXpCapForBuild()
    {
        return xpCap(this.Level);
    }

    public void ProduceCooldown(Player player)
    {
        this.lastedSeconds++;
        if(lastedSeconds >= neededSeconds)
        {
            this.Produce(player);
            this.lastedSeconds = 0;
        }
    }
    //используется для расчета трат(вызывается только если траты есть)
    public bool CalculateSpend(Player player, Resource resType, int ammount, out int ammountSpend, out float moneySpend, out Resource returnType)
            {
                int playerLevel = player.Level;
                int buildingLevel = this.Level;
                float spendQ = resType.Factor;
                moneySpend = ammount*(spendQ+1/playerLevel);
                ammountSpend = 0;
                returnType = null;
                this.totalSpending += moneySpend;
                this.totalIncome += ammount*resType.Price;
                if (resType.IsProducable == true){
                ammountSpend = (int)((1/(playerLevel)+(buildingLevel*spendMultiply))*ammountMultiply);
                returnType = resType.ResourceProducedFrom;
                return true;
                }
                return true;
    }
    public void CompleteSpend(Player player, Resource returnType, int ammount)
    {
        CalculateSpend(player, returnType, ammount, out var ammountSpend, out var moneySpend, out var SpendResource);
        try{
            player.RemoveMoney(moneySpend);
            this.LevelUp((int)(ammount*returnType.Factor*xpMultiply));
            player.AddResource(returnType, ammount);
            
            if (SpendResource != null)
            {
                player.RemoveResource((Resource)SpendResource, ammountSpend);
            }
            }
        catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                GameMethods.Waiting(player);
            }
        }
    public void Produce(object obj)
    {
        Player player = (Player)obj;
        if (player.HasDebt == false){
        int playerLevel = player.Level;
        int buildingLevel = this.Level;
        Resource resType = this.ProduceType;
        float produceQ = resType.Factor;
       

        Console.WriteLine("Factory producing");

        int ammount = (int)((produceQ*(playerLevel+(buildingLevel*spendMultiply)))*ammountMultiply);

        CompleteSpend(player, resType, ammount);
    }
    }

}

public class Factory : Building
{
       public Factory(Resource res, string name, Player? player)
    {
        this.Level = 1;
        this.Name = name;
        this.ProduceQ = res.Factor;
        this.ProduceType = res;
    }

    public Factory(Resource res, string name, Player? player, int xp, int level, int lastedSecs, float totInc, float totSpend)
    {
        this.Level = level;
        this.Expirience = xp;
        this.Name = name;
        this.Owner = player;
        this.ProduceType = res;
        this.ProduceQ = res.Factor;
        this.LastedSeconds = lastedSeconds;
        this.TotalIncome = totInc;
        this.TotalSpending = totSpend;
    }
}
