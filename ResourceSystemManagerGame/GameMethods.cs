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
    public const float xpMultiply = 0.2F;
    public const float spendMultiply = 0.1F;
    public const int cashAmount = 1000;
    public const int buildFactor = 15;

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
            Dictionary<Resource, int> storage = player.GetStorage();
            List<Building> buildings = player.GetBuildings();
            int index1 = 0;
            int index2 = 0;

            Console.WriteLine($"Player: {player.Name} ID: {player.ID} Level: {player.Level}({player.Expirience}/{player.GetXpCapForPlayer()}) Money:{player.Money}");
            Console.WriteLine($"Storage:");
            foreach(var r in storage)
            {
                Console.Write($"{r.Key.Name}: {r.Value} ");
                index1++;
                if(index1%3 == 0)
                {
                    Console.Write("\n");
                }
            }
            Console.WriteLine("List of buildings:");
        
            foreach (var b in buildings)
            {
                Console.Write($"Build {index1}: {b.Name}, lvl: {b.Level}({b.Expirience}/{b.GetXpCapForBuild()}), Resource type: {b.ProduceType.Name}");
                if (b.ProduceType.IsProducable == true)
                {
                    Console.Write($", Consumed resource: {b.ProduceType.ResourceProducedFrom.Name}; ");
            }
            else
            {
                Console.Write("; ");
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
        Factory factory = new Factory(res, name, player);
        player.RemoveMoney(Price);
        player.AddBuilding(factory as Building);
    }
    
    public static void Build(Player player)
    {
        Console.WriteLine("Enter number of building you wanna buy:");
        Console.WriteLine($"1.Ore Mineshaft: {Resource.Ore.Price*buildFactor} 2.Coal Mineshaft: {Resource.Coal.Price*buildFactor} 3.Plant Farm: {Resource.Plant.Price*buildFactor}");
        Console.WriteLine($"4.Metal Factory: {Resource.Metal.Price*buildFactor} 5.Power Plant: {Resource.Energy.Price*buildFactor} 6.Food Factory: {Resource.Food.Price*buildFactor}");
        Console.WriteLine("7.Exit");

        string? input = Console.ReadLine();
        float Price = 0;
        Resource? resType = null;

        switch (input)
        {
            case "1":
            resType = Resource.Ore;
            Price = resType.Price*buildFactor;
            BuyBuilding(player, resType, Price);
            break;
            case "2":
            resType = Resource.Coal;
            Price = resType.Price*buildFactor;
            BuyBuilding(player, resType, Price);
            break;
            case "3":
            resType = Resource.Plant;
            Price = resType.Price*buildFactor;
            BuyBuilding(player, resType, Price);
            break;
            case "4":
            resType = Resource.Metal;
            Price = resType.Price*buildFactor;
            BuyBuilding(player, resType, Price);
            break;
            case "5":
            resType = Resource.Energy;
            Price = resType.Price*buildFactor;
            BuyBuilding(player, resType, Price);
            break;
            case "6":
            resType = Resource.Food;
            Price = resType.Price*buildFactor;
            BuyBuilding(player, resType, Price);
            break;
            case "7":
                Gameplay.Update(player);
            break;
            default:
            Gameplay.Update(player);
            break;
        }
    }
    }




