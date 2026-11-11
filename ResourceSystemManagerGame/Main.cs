using System;

namespace RSM
{
    class Program
    {
        
        public static void Main()
        {
            Gameplay.ProgramIsRunning = true;
            Console.WriteLine("Loading...");
            Gameplay.Start();
        }
    }
}
