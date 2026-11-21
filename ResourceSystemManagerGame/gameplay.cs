using System;
using System.Threading;

namespace RSM;

public static class Gameplay
{
    public static bool ProgramIsRunning = false;
    public static Thread producingThread = new Thread(new ParameterizedThreadStart(threads.ProducingThread));
    public static string version;
    static Gameplay(){
        producingThread.Name = "ProducingThread";
    }

    public static bool GetId(out string? playerId)
    {
        playerId = "";
        
        List<playerCopy> listOfPlayers = SaveAndLoad.LoadAllFiles();
        List<string> ListOfIds = new List<string>();
        foreach(playerCopy pC in listOfPlayers)
        {
            ListOfIds.Add(pC.ID);
        }

        ListOfIds.Sort();
        string? lastPlayer = null;

        if(ListOfIds.Count != 0){
        try{
        lastPlayer = ListOfIds[^1];
        }catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }}

        if (lastPlayer == null)
        {
            return false;
        }
        else
        {
            playerId = lastPlayer;
            return true;
        }
    }

    public static void Start()
    {
        #if !DEBUG
        Console.Clear();
        #endif
        while(ProgramIsRunning == true){
        Console.WriteLine("Resource System Manager - console edition\n\t\t{0}", version);
        Player? player = null;
        while(player == null && ProgramIsRunning == true){
            player = GameMethods.Menu();
        }
        if(player != null){
        threads.ThreadIsRunning = true;
        try{
            producingThread = new Thread(new ParameterizedThreadStart(threads.ProducingThread));
            producingThread.Start(player);
            Update(player);
        }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Thread terminated!");
            }
        }}
    }

    public static void Update(Player? player)
    {
        Thread.Sleep(100);
        #if !DEBUG
        Console.Clear();
        #endif
        while (ProgramIsRunning == true)
        {
            Console.WriteLine(Loc.Dict["select_command"]);
            Console.WriteLine(Loc.Dict["gameplay_menu"]);
            string value = Console.ReadLine();
            switch (value)
            {
                case "1":
                    GameMethods.Build(player);
                break;
                case "2":
                #if !DEBUG
                Console.Clear();
                #endif
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
                                #if !DEBUG
                                Console.Clear();
                                #endif
                                break;
                            }
                        }
                }
                    break;
                case "4":
                #if !DEBUG
                Console.Clear();
                #endif
                    GameMethods.Sell(player);
                break;
                case "5":
                #if !DEBUG
                Console.Clear();
                #endif
                try{
                 SaveAndLoad.Save(player);}catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                break;
                case "6":
                #if !DEBUG
                Console.Clear();
                #endif
                producingThread.Interrupt();
                threads.ThreadIsRunning = false;
                producingThread.Join();
                Start();
                break;
                case "MONEY":
                player.AddMoney(1000000000);
                break;
                case "PRODUCE":
                    foreach(var b in player.GetBuildings())
                    {
                        b.Produce(player);
                    }
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
        producingThread.Interrupt();
        threads.ThreadIsRunning = false;
        try{
        producingThread.Join();
        }catch(Exception ex)
        {
            Console.WriteLine("Thread not started!");
        }
    }
}
