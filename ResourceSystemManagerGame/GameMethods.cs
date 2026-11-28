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
using System.Runtime.InteropServices;
using System.Reflection.Metadata.Ecma335;
namespace RSM;

public static class GameMethods
{
    public static int ammountMultiply;
    public static float xpMultiply;
    public static float spendMultiply;
    public static int cashAmount;
    public static int buildFactor;

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

        TextLabel name = new TextLabel("game_title", Gameplay.version);
        getPlayer newGame = NewGame;
        getPlayer loadGame = Load;
        Action changeLang = ChangeLang;
        Action exit = Gameplay.Exit;
        Button newGameButt = new Button("start_menu", newGame);
        Button loadGameButt = new Button("load_game", loadGame);
        Button changeLangButt = new Button("change_lang", changeLang);
        Button exitButt = new Button("exit_button", exit);

        UiList<TextLabel> menuList = new UiList<TextLabel>(true ,name, newGameButt, loadGameButt, changeLangButt, exitButt);
        menuList.Move(out Button selected);

        if(selected.anonymAction!= null)
        {
            selected.OnClick();
            return null;
        }
        if(selected.PlayerFunc != null)
        {
            selected.OnClick(out Player player);
            return player;
        }

        return null;
        }

    public static void ChangeLang()
    {
        TextLabel langMenu = new TextLabel("choose_lang");
        int index = 1;
        var ass = Assembly.GetExecutingAssembly();
        string[] stream = ass.GetManifestResourceNames();
        string title = "";
        List<TextLabel> listOfElements = new();
        List<string> files = new();
        listOfElements.Add(langMenu);
        foreach(string str in stream)
        {
            if(str.Contains("Locals")){
            using Stream? fstr = ass.GetManifestResourceStream(str);
            StreamReader sr = new StreamReader(fstr);
            string str11 = sr.ReadToEnd();
            Dictionary<string, string> jsonstr = JsonSerializer.Deserialize<Dictionary<string,string>>(str11);
            title = jsonstr["title"];
            Action<object> changelang = Loc.GetLoc;
            Button lang = new Button(title, changelang);
            listOfElements.Add(lang);
            files.Add(str);
            index++;}
        }
        Button exitButt = new Button("exit_button");
        listOfElements.Add(exitButt);
        TextLabel[] arrayOfLang = listOfElements.ToArray();
        UiList<TextLabel> chooseLang = new UiList<TextLabel>(true, arrayOfLang);
        List<TextLabel> listOfLang = listOfElements;
        listOfLang.Remove(exitButt);
        listOfLang.Remove(langMenu);

        chooseLang.Move(out Button button);

        if (button.anonymAction != null)
        {
            button.anonymAction();
        }
        if (button.PlayerAction != null)
        {
            int indexAtLangList = listOfLang.FindIndex(b => b.Equals(button));
            string fileName = stream[indexAtLangList];
            fileName = fileName.Replace("ResourceSystemManagerGame.Locals.", "");
            fileName = fileName.Replace(".json", "");
            button.PlayerAction(fileName);
        }
                return;
    }

    public static Player NewGame()
    {
        InputField newCharName = new InputField("new_char_name", out object Name);
        Name = (string)Name;
        newCharName.Render();
        
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
                player.Name = (string)newCharName.Input;
#pragma warning restore CS8601 // Possible null reference assignment.
                player.ID = Id;
        return player;
        }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
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
            List<TextLabel> listOfElements = new List<TextLabel>();

            TextLabel title = new TextLabel("info_title", player.Name, player.ID, player.Level, player.Expirience, player.GetXpCapForPlayer(), player.Money);
            TextLabel storageTitel = new TextLabel("msg_storage");
            listOfElements.Add(title);
            listOfElements.Add(storageTitel);
            foreach(var r in storage)
            {
                TextLabel resLabel = new TextLabel("res_list", r.Key.Name, r.Value);
                listOfElements.Add(resLabel);
                if(index1 %2 == 0)
            {
                resLabel.Text = resLabel.Text + "\n";
            }
                index1++;
            }

            TextLabel bList = new TextLabel("building_list");
            listOfElements.Add(bList);

            foreach (var b in buildings)
            {
                TextLabel bLabel = new TextLabel("build_in_list", index2+1, b.ProduceType.BuildName, b.Name, b.Level, b.Expirience, b.GetXpCapForBuild(), b.ProduceType.Name);
                if (b.ProduceType.IsProducable == true)
                {
                    foreach(Resource resource in b.ProduceType.ResourceProducedFrom){
                    bLabel.Text = bLabel.Text + String.Format(Loc.Dict["consumed_res"], resource.Name);
                    }
                }
                listOfElements.Add(bLabel);
                index2++;
            }

            Button exitButt = new Button("exit_button");
            listOfElements.Add(exitButt);

            TextLabel[] ArrayOE = listOfElements.ToArray();

            UiList<TextLabel> uiList = new UiList<TextLabel>(true, ArrayOE);
            uiList.Move(out Button exit);

            exit.OnClick();
        }

    public static void BuyBuilding(object obj)
    {
        object[] listOfParams = (object[])obj;
        Player player = (Player)listOfParams[0];
        Resource resType = (Resource)listOfParams[1];
        float Price = (float)listOfParams[2];
        #if !DEBUG
        Console.Clear();
        #endif
        InputField buildNewName = new InputField(Loc.Dict["build_new_name"], out object buildName);

        buildNewName.Render();

        string name = (string)buildNewName.Input;
        if (name != ""){
        Factory factory = new Factory(resType, name, player);
        player.RemoveMoney(Price);
        player.AddBuilding(factory);
        factory.TotalSpending += Price;
        }
        else
        {
            Factory factory = new Factory(resType, "Default", player);
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

        List<TextLabel> listofElements = new();
        TextLabel money = new TextLabel("money_ammount", player.Money);
        TextLabel selectBuild = new TextLabel("select_build");
        listofElements.Add(money);
        listofElements.Add(selectBuild);
        List<Resource> resEnum = Resource.ResourceList;
        foreach(Resource res in resEnum)
        {
           Action<object> action = BuyBuilding;
           Button bInBuyList = new Button("build_in_buy_list", action,res.BuildName, res.Price*buildFactor);
           listofElements.Add(bInBuyList);
        }

        Button exitButt = new Button("exit_button");
        listofElements.Add(exitButt);
        List<TextLabel> listOfBuild = listofElements;
        listOfBuild.Remove(money);
        listOfBuild.Remove(selectBuild);
        listOfBuild.Remove(exitButt);

        TextLabel[] arrayOE = listofElements.ToArray();

        UiList<TextLabel> bUiBuy = new UiList<TextLabel>(true, arrayOE);
        bUiBuy.Move(out Button button);
        Resource resType = null;
        float Price = 0F;

        if(button.PlayerAction != null)
        {
            int indexAtList = listOfBuild.FindIndex(i=>i.Equals(button));
            resType = resEnum[indexAtList];
            Price = resType.Price*buildFactor;
            object[] paramList = {player, resType, Price};
            button.OnClick(paramList);
        }

}

    public static void SellResources(Player player)
    {
        #if !DEBUG
        Console.Clear();
        #endif
        bool IsSell = false;
        IDictionary<Resource, int> playerStorage = player.GetStorage();
        foreach(var res in playerStorage)
        {
            if(res.Value != 0)
            {
                IsSell = true;
            }
        }
        if(IsSell == true){
        Console.WriteLine("Succesfully sell ");
        foreach(var res in playerStorage)
        {
            player.RemoveResource(res.Key, res.Value);
            player.AddMoney(res.Key.Price*res.Value);
            if (res.Value != 0){
            if (res.Value != 1 && res.Value != 11 && (res.Value-1)%10!=0){
            Console.Write(Loc.Dict["sell_res"], res.Value, res.Key.GenName, res.Key.Price*res.Value);
            }
            else
            {
                Console.Write(Loc.Dict["sell_res"], res.Value, res.Key.Name, res.Key.Price*res.Value);
            }}
        }
        #if !DEBUG
        (int Left, int Top) CursorPos = Console.GetCursorPosition();
        Console.SetCursorPosition(CursorPos.Left-2, CursorPos.Top);
        Console.Write("!");
        #endif
        Console.WriteLine(Loc.Dict["money_sell_count"], player.Money);
        }
        else
        {
            Console.WriteLine(Loc.Dict["not_sell"]);
        }
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
                return;
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




