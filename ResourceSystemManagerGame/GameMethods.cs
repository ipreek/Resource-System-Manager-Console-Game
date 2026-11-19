using System;
using System.Collections;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Threading;
using System.Reflection;
using System.Text.Json;
using System.IO;

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
        Console.WriteLine(Loc.Dict["select_command"]);
        Console.WriteLine(Loc.Dict["start_menu"]);
        string? line = Console.ReadLine();
        if (line != null){
        switch (line)
        {
            case "1":
            #if !DEBUG
            Console.Clear();
            #endif
            Resource.ResourceList.Clear();
                Resource.ResCreate();
                return NewGame();
            case "2":
                #if !DEBUG
                Console.Clear();
                #endif
                Resource.ResourceList.Clear();
                Resource.ResCreate();
                Player player1 = null;
                try{
                        player1 = Load();}catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                if(player1!=null){
                return player1;
                        }
                        else
                        {
                            break;
                        }
            case "3":
            #if !DEBUG
            Console.Clear();
            #endif
            ChangeLang();
            break;
            case "4":
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

    public static void ChangeLang()
    {
        Console.WriteLine(Loc.Dict["choose_lang"]);
        int index = 1;
        var ass = Assembly.GetExecutingAssembly();
        string[] stream = ass.GetManifestResourceNames();
        List<string> files = new();
        foreach(string str in stream)
        {
            if(str.Contains("Locals")){
            using Stream? fstr = ass.GetManifestResourceStream(str);
            StreamReader sr = new StreamReader(fstr);
            string str11 = sr.ReadToEnd();
            Dictionary<string, string> jsonstr = JsonSerializer.Deserialize<Dictionary<string,string>>(str11);
            Console.WriteLine(Loc.Dict["lang_list"], index, jsonstr["title"]);
            files.Add(str);
            index++;}
        }
        Console.WriteLine(Loc.Dict["exit_button"], index);
        string input = Console.ReadLine();

        if(Int32.TryParse(input, out int inputInt) == true)
        {
            if(inputInt == index)
            {
                return;
            }
            else
            {
                string fileName = stream[0];
                fileName = fileName.Replace("ResourceSystemManagerGame.Locals.", "");
                fileName = fileName.Replace(".json", "");
                Loc.GetLoc(fileName);
                return;
            }
        }
        else
        {
            Console.WriteLine(Loc.Dict["wrong_command"]);
        }
    }

    public static Player NewGame()
    {
        while(true){
        Console.WriteLine(Loc.Dict["new_char_name"]);
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

            Console.WriteLine(Loc.Dict["info_title"], player.Name, player.ID, player.Level, player.Expirience, player.GetXpCapForPlayer(), player.Money);
            Console.WriteLine(Loc.Dict["msg_storage"]);
            foreach(var r in storage)
            {
                Console.Write($"{r.Key.Name}: {r.Value} ");
                index1++;
                if(index1%2 == 0)
                {
                    Console.Write("\n");
                }
            }
            Console.WriteLine(Loc.Dict["building_list"]);
        
            foreach (var b in buildings)
            {
                Console.WriteLine(Loc.Dict["build_in_list"], index2+1, b.ProduceType.BuildName, b.Name, b.Level, b.Expirience, b.GetXpCapForBuild(), b.ProduceType.Name);
                if (b.ProduceType.IsProducable == true)
                {
                    Console.Write(Loc.Dict["consumed_res"], b.ProduceType.ResourceProducedFrom.Name);
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
        #if !DEBUG
        Console.Clear();
        #endif
        Console.WriteLine(Loc.Dict["build_new_name"]);
        string? name = Console.ReadLine();
        if (name != ""){
        Factory factory = new Factory(res, name, player);
        player.RemoveMoney(Price);
        player.AddBuilding(factory);
        factory.TotalSpending += Price;
        }
        else
        {
            Factory factory = new Factory(res, "Default", player);
            player.RemoveMoney(Price);
            player.AddBuilding(factory);
            factory.TotalSpending += Price;
        }
    }
    
    public static void Build(Player player)
    {
        #if !DEBUG
        Console.Clear();
        #endif
        while(true){
        int index = 0;
        Console.WriteLine(Loc.Dict["money_ammount"], player.Money);
        Console.WriteLine(Loc.Dict["select_build"]);
        IEnumerable<Resource> resEnum = Resource.ResourceList;
        foreach(Resource res in resEnum)
        {
            Console.Write(Loc.Dict["build_in_buy_list"], index+1, res.BuildName, res.Price*buildFactor);
            index++;
            if (index % 3 == 0)
            {
                Console.Write("\n");
            }
        }

        Console.WriteLine(Loc.Dict["exit_button"], index+1);

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
                Console.WriteLine(Loc.Dict["wrong_command"]);
            }

        }}

    public static void SellResources(Player player)
    {
        #if !DEBUG
        Console.Clear();
        #endif
        Console.WriteLine("Succesfully sell ");
        IDictionary<Resource, int> playerStorage = player.GetStorage();
        foreach(var res in playerStorage)
        {
            player.RemoveResource(res.Key, res.Value);
            player.AddMoney(res.Key.Price*res.Value);
            Console.Write(Loc.Dict["sell_res"], res.Value, res.Key.GenName, res.Key.Price*res.Value);
        }
        #if !DEBUG
        (int Left, int Top) CursorPos = Console.GetCursorPosition();
        Console.SetCursorPosition(CursorPos.Left-2, CursorPos.Top);
        Console.Write("!");
        #endif
        Console.WriteLine(Loc.Dict["money_sell_count"], player.Money);
    }

    private static void SellChoosedBuilding(Player player, Building build)
    {
        #if !DEBUG
        Console.Clear();
        #endif
        float Price = build.ProduceType.Price*buildFactor*(build.Level*xpMultiply);
        player.RemoveBuilding(build);
        player.AddMoney(Price);
        Console.WriteLine(Loc.Dict["sell_build"],build.ProduceType.BuildName, build.Name, Price);
        Console.WriteLine(Loc.Dict["money_sell_count"], player.Money);
    }

    public static void SellBuilding(Player player)
    {
        #if !DEBUG
        Console.Clear();
        #endif
        IEnumerable<Building> buildingList = player.GetBuildings();
        int index = 1;
        Console.WriteLine(Loc.Dict["select_build_sell"]);

        foreach (Building b in buildingList)
        {
            Console.WriteLine(Loc.Dict["build_in_sell_list"],index, b.ProduceType.BuildName, b.Name, b.Level);
            Console.WriteLine(Loc.Dict["total_profit"], b.TotalIncome, b.TotalSpending, b.TotalProfit);
            index++;
        }
        Console.WriteLine(Loc.Dict["exit_button"], index);

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
                Console.WriteLine(Loc.Dict["sure_qu"]);
                Console.WriteLine(Loc.Dict["yes_no"]);
                string input2 = Console.ReadLine();
                switch (input2)
                {
                    case "1":
                        SellChoosedBuilding(player, b);
                    break;
                    case "2":
                        break;
                    default:
                        Console.WriteLine(Loc.Dict["wrong_command"]);
                    break;
                }
            }
        }
        else
        {
            Console.WriteLine(Loc.Dict["wrong_command"]);
        }

    }

    public static void Sell(Player player)
    {
        while(true){
            #if !DEBUG
            Console.Clear();
            #endif
            Console.WriteLine(Loc.Dict["choose_sell_item"]);
            Console.WriteLine(Loc.Dict["list_of_items_for_sell"]);
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
                    Console.WriteLine(Loc.Dict["wrong_command"]);
                break;
            }
            if (ExitWhile == true)
                    {
                        break;
                    }
        }
        
    }

    public static Player? Load()
    {
        #if !DEBUG
        Console.Clear();
        #endif
        
        while(true){
        IList<playerCopy> loadList = SaveAndLoad.LoadAllFiles();

        if (loadList.Count != 0){
            Console.WriteLine(Loc.Dict["choose_load"]);
            int index = 1;
            foreach(playerCopy pC in loadList)
            {
                Console.WriteLine(Loc.Dict["player_in_load"],index, pC.Name, pC.ID, pC.SaveDate);
                index++;
            }
            Console.WriteLine(Loc.Dict["exit_button"], index);
            string input = Console.ReadLine();

            if(Int32.TryParse(input, out int inputInt) && inputInt >0 && inputInt < index+1)
            {
                if(inputInt == index)
                    {
                        return null;
                    }
                    else
                    {
                        Console.WriteLine(Loc.Dict["load_choose"]);
                        string? loadInput = Console.ReadLine();
                        switch (loadInput)
                        {
                            case "1":
                                Player p = SaveAndLoad.Load(loadList[inputInt-1]);
                                List<Building> bList = p.GetBuildings();
                                if(bList.Count != 0){
                                    for(int i =0; i<bList.Count; i++)
                                        {
                                        bList[i].Owner = p;
                                        }
                                    }
                            return(SaveAndLoad.Load(loadList[inputInt-1]));
                            case "2":
                                Console.WriteLine(Loc.Dict["sure_qu"]);
                                Console.WriteLine(Loc.Dict["yes_no"]);
                                string? deleteInput = Console.ReadLine();
                                switch (deleteInput)
                                {
                                    case "1":
                                        string localDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                                        string programPath = Path.Combine(localDataPath, "RSM", "saves");
                                        string savePath = Path.Combine(programPath, $"save_{loadList[inputInt-1].ID}.json");
                                        File.Delete(savePath);
                                    break;
                                    case "2":
                                    break;
                                    default:
                                    Console.WriteLine(Loc.Dict["wrong_command"]);
                                    break;
                                }
                            break;
                            case "3":
                            break;
                            default:
                                Console.WriteLine(Loc.Dict["wrong_command"]);
                            break;
                        }
                    }
            }
            else
            {
                Console.WriteLine(Loc.Dict["wrong_command"]);
            }}
        else
        {
            Console.WriteLine(Loc.Dict["save_not_found"]);
            Console.ReadLine();
            return null;
        }}
    }
    }




