using System;
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

    public void Timer()
    {
        int sec = 0;
        TimerCallback tm = new TimerCallback(Produce); //вызывает функцию производства по истечению таймера
        Timer timer = new Timer(tm,sec,0,30*1000); //ждёт 30 секунд
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

    //используется для расчета трат(вызывается только если траты есть)
    public bool CalculateSpend(Player player, Resource resType, int ammount, out int ammountSpend, out float moneySpend, out Resource returnType)
            {
                int playerLevel = player.Level;
                int buildingLevel = this.Level;
                float spendQ = resType.Factor;
                moneySpend = ammount*(spendQ+1/playerLevel);
                ammountSpend = 0;
                returnType = null;
                if (resType.IsProducable == true){
                ammountSpend = (int)((spendQ*(1/(playerLevel+(buildingLevel*spendMultiply))))*ammountMultiply);
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


        int ammount = (int)((produceQ*(playerLevel+(buildingLevel*spendMultiply)))*ammountMultiply);

        CompleteSpend(player, resType, ammount);
        
    }
    }

}

public class Factory : Building
{
       public Factory(Resource res, string name, Player? player)
    {
        this.Name = name;
        this.ProduceQ = res.Factor;
        this.ProduceType = res;
        player.AddBuilding(this);
    }
}
