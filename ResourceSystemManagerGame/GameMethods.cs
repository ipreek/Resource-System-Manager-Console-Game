using System;
using System.Collections;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Threading;

namespace RSM;

public static class GameMethods
{
    public const int ammountMultiply = 100;
    public const float xpMultiply = 0.5F;
    public const float spendMultiply = 0.1F;
    public const int cashAmount = 10000;
    public const int buildFactor = 250;

    public static bool IsOnlySpaces(string str)
    { 
        int spacesAmmount = 0;
        try{
        foreach (char ch in str)
        {
            if (ch==' ')
            {
                spacesAmmount++;
            }
        }}catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        if (spacesAmmount == str.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static Player? Menu()
    {
        while(Gameplay.ProgramIsRunning == true){
        Console.WriteLine("Enter a command >>");
        Console.WriteLine("1.New game 2.Load game 3.Exit");
        string? line = Console.ReadLine();
        if (line != null){
        switch (line)
        {
            case "1":
            #if !DEBUG
            Console.Clear();
            #endif
                return NewGame();
            case "2":
                //GameMethods.Load();
                #if !DEBUG
                Console.Clear();
                #endif
                Gameplay.Exit();
                return null;
            case "3":
            #if !DEBUG
                Console.Clear();
                #endif
                Gameplay.Exit();
                return null;
            default:
                Console.WriteLine("Wrong command! Try again.");
                return null;
            }}
        return null;
        }
        return null;
    }

    public static Player NewGame()
    {
        while(true){
        Console.WriteLine("Enter name of your character: >>");
        Player player = new Player();
        string? lastId = Gameplay.GetId(out lastId) ? lastId : null;
        string Id;
        if (lastId != null){
        Int32.TryParse(lastId, out int intId);    
        intId++;
        Id = intId.ToString();
            }
            else
            {
                Id = "1";
            }
        try{
#pragma warning disable CS8601 // Possible null reference assignment.
                player.Name = Console.ReadLine();
#pragma warning restore CS8601 // Possible null reference assignment.
                player.ID = Id;
        return player;
        }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public static void Waiting(Player player)
    {
        Thread.Sleep(700);
        Gameplay.Update(player);
    }

    public static void Info(Player player)
    {
            IDictionary<Resource, int> storage = player.GetStorage();
            IEnumerable<Building> buildings = player.GetBuildings();
            int index1 = 0;
            int index2 = 0;

            Console.WriteLine($"Player: {player.Name} ID: {player.ID} Level: {player.Level}({player.Expirience}/{player.GetXpCapForPlayer()}) Money:{player.Money}");
            Console.WriteLine($"Storage:");
            foreach(var r in storage)
            {
                Console.Write($"{r.Key.Name}: {r.Value} ");
                index1++;
                if(index1%2 == 0)
                {
                    Console.Write("\n");
                }
            }
            Console.WriteLine("List of buildings:");
        
            foreach (var b in buildings)
            {
                Console.WriteLine($"{index2+1}.{b.ProduceType.BuildName}: \"{b.Name}\", lvl: {b.Level}({b.Expirience}/{b.GetXpCapForBuild()}), Resource type: {b.ProduceType.Name}");
                if (b.ProduceType.IsProducable == true)
                {
                    Console.Write($", Consumed resource: {b.ProduceType.ResourceProducedFrom.Name}; ");
            }
                index2++;
                if(index2%5 == 0)
                {
                    Console.Write("\n");
                }
            }
        }

    public static void BuyBuilding(Player player,Resource res, float Price)
    {
        Console.WriteLine("Enter name to your building:");
        string? name = Console.ReadLine();
        if (name != ""){
        Factory factory = new Factory(res, name, player);
        player.RemoveMoney(Price);
        player.AddBuilding(factory);
        }
        else
        {
            Factory factory = new Factory(res, "Default", player);
            player.RemoveMoney(Price);
            player.AddBuilding(factory);
        }
    }
    
    public static void Build(Player player)
    {
        while(true){
        int index = 0;
        Console.WriteLine($"You have {player.Money} Money");
        Console.WriteLine("Enter number of building you wanna buy:");
        IEnumerable<Resource> resEnum = Resource.ResourceList;
        foreach(Resource res in resEnum)
        {
            Console.Write($"{index+1}.{res.BuildName}: {res.Price*buildFactor} ");
            index++;
            if (index % 3 == 0)
            {
                Console.Write("\n");
            }
        }

        Console.WriteLine($"{index+1}.Exit");

        string? input = Console.ReadLine();
        float Price = 0;
        Resource? resType = null;
        IReadOnlyList<Resource> resList = Resource.ResourceList;

            if(input == (index+1).ToString())
            {
                Gameplay.Update(player);
                break;
            }else if(Int32.TryParse(input, out int integer) == true && integer > 0 && integer <(index+1))
            {
                resType = resList[integer-1];
                Price = resType.Price*buildFactor;
                BuyBuilding(player, resType, Price);
            }
            else
            {
                Console.WriteLine("Wrong command! Try again.");
            }

        }}

    public static void SellResources(Player player)
    {
        Console.WriteLine("Succesfully sell ");
        IDictionary<Resource, int> playerStorage = player.GetStorage();
        foreach(var res in playerStorage)
        {
            player.RemoveResource(res.Key, res.Value);
            player.AddMoney(res.Key.Price*res.Value);
            Console.Write($"{res.Value} {res.Key.Name} for {res.Key.Price*res.Value}, ");
        }
        (int Left, int Top) CursorPos = Console.GetCursorPosition();
        Console.SetCursorPosition(CursorPos.Left-2, CursorPos.Top);
        Console.Write("!");
        Console.WriteLine("Now you have {0}!", player.Money);
    }

    private static void SellChoosedBuilding(Player player, Building build)
    {
        float Price = build.ProduceType.Price*buildFactor*(build.Level*xpMultiply);
        player.RemoveBuilding(build);
        player.AddMoney(Price);
        Console.WriteLine($"Succesfully sell {build.ProduceType.BuildName} \"{build.Name}\" for {Price}");
        Console.WriteLine($"You now have {player.Money}");
    }

    public static void SellBuilding(Player player)
    {
        IEnumerable<Building> buildingList = player.GetBuildings();
        int index = 1;
        Console.WriteLine("Choose building you want to sell:");

        foreach (Building b in buildingList)
        {
            Console.WriteLine($"{index}.{b.ProduceType.BuildName} \"{b.Name}\", LvL {b.Level}");
            Console.WriteLine($"\tTotal income (expectable): {b.TotalIncome}. Total spendings: {b.TotalSpending}. Total profit: {b.TotalProfit}");
            index++;
        }
        Console.WriteLine($"{index}.Exit");

        string? input = Console.ReadLine();

        bool parse = Int32.TryParse(input, out int inputInt);

        if (parse == true)
        {
            if(inputInt == index)
            {
                Sell(player);
            }else if(inputInt >0 && inputInt < index)
            {
                IList<Building> bList = player.GetBuildings();
                Building b = bList[inputInt-1];
                Console.WriteLine("Are you sure?");
                Console.WriteLine("  1.Yes 2.No");
                string input2 = Console.ReadLine();
                switch (input2)
                {
                    case "1":
                        SellChoosedBuilding(player, b);
                    break;
                    case "2":
                        break;
                    default:
                        Console.WriteLine("Wrong Command! Try again.");
                    break;
                }
            }
        }
        else
        {
            Console.WriteLine("Wrong Command! Try again!");
        }

    }

    public static void Sell(Player player)
    {
        while(true){
            Console.WriteLine("Choose what you want to sell");
        Console.WriteLine("1.Building 2.Resources 3.Exit");
            string? input1 = Console.ReadLine();
            bool ExitWhile = false;
            switch (input1)
            {
                case "1":
                    SellBuilding(player);
                break;
                case "2":
                    SellResources(player);
                break;
                case "3":
                    ExitWhile = true;
                break;
                default:
                    Console.WriteLine("Wrong Command! Try again.");
                break;
            }
            if (ExitWhile == true)
                    {
                        break;
                    }
        }
    }
    }




