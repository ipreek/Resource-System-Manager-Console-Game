using System;
using System.Threading;

namespace RSM;

public static class Gameplay
{
    const string version = "0.1 build 121102";
    public static bool ProgramIsRunning = false;
    public static Thread producingThread = new Thread(new ParameterizedThreadStart(threads.ProducingThread));
    static Gameplay(){
        producingThread.Name = "ProducingThread";
    }

    private static List<Player?> ListOfPlayers = new List<Player?>();

    public static bool GetId(out string? playerId)
    {
        playerId = "";
        int Length = ListOfPlayers.Count;
        Length = Length >0 ? Length : 1; 
        Player? lastPlayer = null;
        if (ListOfPlayers.Count >0){
            lastPlayer = ListOfPlayers[Length-1];
        }

        if (lastPlayer == null)
        {
            return false;
        }
        else
        {
            playerId = lastPlayer.ID;
            return true;
        }
    }

    public static void Start()
    {
        #if !DEBUG
        Console.Clear();
        #endif
        while(ProgramIsRunning == true){
        Console.WriteLine("Resource System Manager - console edition\n\t\t{0}",version);
        Player? player = null;
        while(player == null && ProgramIsRunning == true){
            player = GameMethods.Menu();
        }
        ListOfPlayers.Add(player);
        try{
            producingThread.Start(player);
            Update(player);
        }catch(Exception ex)
            {
                Console.WriteLine("Thread terminated!");
            }
        }
    }

    public static void Update(Player? player)
    {
        Thread.Sleep(100);
        #if !DEBUG
        Console.Clear();
        #endif
        while (ProgramIsRunning == true)
        {
            Console.WriteLine("\tEnter a command >>");
            Console.WriteLine("1.Build 2.Work 3.Info\n4.Sell 5.Save 6.Menu ");
            string value = Console.ReadLine();
            switch (value)
            {
                case "1":
                    GameMethods.Build(player);
                break;
                case "2":
                //Console.Clear();
                    player?.Work();
                break;
                case "3":
                bool InfoIsOpen = true;
                while(InfoIsOpen){
                    #if !DEBUG
                    Console.Clear();
                    #endif
                    GameMethods.Info(player);

                    Thread.Sleep(500);
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.Enter)
                            {
                                InfoIsOpen = false;
                                break;
                            }
                        }
                }
                    break;
                case "4":
                #if !DEBUG
                Console.Clear();
                #endif
                    //GameMethods.Sell(player);
                break;
                case "5":
                #if !DEBUG
                Console.Clear();
                #endif
                //GameMethods.Save(player);
                break;
                case "6":
                #if !DEBUG
                //Console.Clear();
                #endif
                Start();
                break;
                case "MONEY":
                player.AddMoney(1000000000);
                break;
                default:
                    Console.WriteLine("Wrong command! Try again.");
                break;
            }
        }
    }

    public static void Exit()
    {
        ProgramIsRunning = false;
        try{
        producingThread.Join();
        }catch(Exception ex)
        {
            Console.WriteLine("Thread not started!");
        }
    }
}
