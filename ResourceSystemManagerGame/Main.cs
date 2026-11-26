using System;

namespace RSM
{
    class Program
    {
        
        public static void Main()
        {
            Gameplay.ProgramIsRunning = true;
            OnFirstLaunch.CreateJson();
            Loc.GetLoc("eng");
            Setting.GetCon();
            Setting.GetResources();
            Console.WriteLine("Loading...");

            Gameplay.Start();
        }
    }
}
