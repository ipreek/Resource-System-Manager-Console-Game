using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;

#nullable disable warnings
namespace RSM;

public static class threads
{
    public static bool ThreadIsRunning;
    private static int lastedSecs = 0;
    static readonly Object _lock = new object();
    
    public static void calculateProduce(Player player)
    {
        if (player == null){return;}
        IEnumerable<Building> getBuildings = player?.GetBuildings();
        foreach(var b in getBuildings){
            b.ProduceCooldown(player);
        }
    }

    public static void calculateProducePrice()
    {
        lastedSecs++;
        if(lastedSecs >= 10){
        IList<Resource> getRess = Resource.ResourceList;
        foreach(var res in getRess)
        {
            float randomNumber = Random.Shared.NextSingle();
            randomNumber *= 0.1F;
            randomNumber *= res.Price;
            int UpOrDown = Random.Shared.Next(-1,2);
            randomNumber *= UpOrDown;
            res.Price += randomNumber;
        }
        lastedSecs = 0;            
        }
    }

    public static void ProducingThread(Object? obj)
    {
        var sw = Stopwatch.StartNew();
        double lastS = 0;
        while(ThreadIsRunning == true)
        {
            Player? player = obj as Player;
            double now = sw.Elapsed.TotalSeconds;
            double dt = (now-lastS);
            if(dt >= 1)
            {
                calculateProducePrice();
                calculateProduce(player);
                lastS = now;
            }
            //Thread.Sleep(50);
        }
    }   
}
