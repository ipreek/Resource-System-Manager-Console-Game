using System;
using System.Diagnostics;
using System.Threading;
#nullable disable warnings
namespace RSM;

public static class threads
{
    public static bool ThreadIsRunning;

    static readonly Object _lock = new object();
    
    public static void calculateProduce(Player player)
    {
        if (player == null){return;}
        IEnumerable<Building> getBuildings = player?.GetBuildings();
        foreach(var b in getBuildings){
            b.ProduceCooldown(player);
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
                calculateProduce(player);
                lastS = now;
            }
            //Thread.Sleep(50);
        }
    }   
}
