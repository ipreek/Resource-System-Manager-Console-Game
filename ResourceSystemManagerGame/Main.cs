using System;

namespace RSM
{
    class Program
    {
        
        public static void Main()
        {
            Gameplay.ProgramIsRunning = true;
            OnFirstLaunch.CreateJson();
            Console.WriteLine("Loading...");
            Loc.GetLoc("eng");
            Gameplay.Start();
        }
    }
}
