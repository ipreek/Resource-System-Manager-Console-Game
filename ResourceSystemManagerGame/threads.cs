using System;
using System.Threading;

namespace RSM;

public static class threads
{

    public static void calculateProduce(Object? obj)
    {
        while(Gameplay.ProgramIsRunning == true){
        Player? player = (Player?)obj;
        foreach(var b in player?.GetBuildings()){
            Thread cooldown = new Thread(new ThreadStart(b.Timer));
        }
        }
    }
}
