using System;

namespace RSM
{
    class Program
    {
        
        public static void Main()
        {
            Gameplay.ProgramIsRunning = true;
            OnFirstLaunch.CreateJson();
            Setting.GetCon();
            Console.WriteLine("Loading...");
            Loc.GetLoc("eng");
            Gameplay.Start();
        }
    }
}
