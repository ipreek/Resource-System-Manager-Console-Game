using System;
using System.Threading;

namespace RSM;

public static class threads
{
    const int timerSecs = 1;
    public static void calculateProduce(Object? obj)
    {
        while(Gameplay.ProgramIsRunning == true){
        Player? player = (Player?)obj;
        foreach(var b in player?.GetBuildings()){
            TimerCallback tm = new TimerCallback(b.Produce); //вызывает функцию производства по истечению таймера
            Timer timer = new Timer(tm,player,0,timerSecs*1000); //ждёт 30 секунд
        }
        }
    }
}
