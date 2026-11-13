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
    public const int ammountMultiply = 10;
    public const float xpMultiply = 0.2F;
    public const float spendMultiply = 0.1F;
    public const int cashAmount = 1000;
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
                if(index1%2 == 0)
                {
                    Console.Write("\n");
                }
            }
            Console.WriteLine("List of buildings:");
        
            foreach (var b in buildings)
            {
                Console.Write($"{index2+1}.{b.ProduceType.BuildName}: \"{b.Name}\", lvl: {b.Level}({b.Expirience}/{b.GetXpCapForBuild()}), Resource type: {b.ProduceType.Name}");
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
        Console.WriteLine("Enter number of building you wanna buy:\n");
        foreach(Resource res in Resource.ResourceList)
        {
            Console.Write($"{index+1}.{res.BuildName}: {res.Price*buildFactor} ");
            index++;
            if (index % 3 == 0)
            {
                Console.Write("\n");
            }
        }

        int listLength = Resource.ResourceList.Count;
        Console.WriteLine($"{index+1}.Exit");

        string? input = Console.ReadLine();
        float Price = 0;
        Resource? resType = null;

            if(input == (index+1).ToString())
            {
                Gameplay.Update(player);
                break;
            }else if(Int32.TryParse(input, out int integer) == true && integer > 0 && integer <(index+1))
            {
                resType = Resource.ResourceList[integer-1];
                Price = resType.Price*buildFactor;
                BuyBuilding(player, resType, Price);
            }
            else
            {
                Console.WriteLine("Wrong command! Try again.");
            }

        }}
    }




