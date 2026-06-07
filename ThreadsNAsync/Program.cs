using System;
using System.Diagnostics;
using System.Threading;

namespace ThreadsNAsync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var race = new Race();
            
            // Create the cars
            race.AddCar(new Car("Volvo"));
            race.AddCar(new Car("Saab"));
            race.AddCar(new Car("Tesla"));

            // Create/start the thread to listen for user input
            Thread inputThread = new Thread(race.ListenForStatus);
            // Start the thread as a background thread so it will automatically stop when the main thread finishes
            inputThread.IsBackground = true; // Just like the bacon and eggs in Petter's example
            inputThread.Start();

            // Start the race, one thread per car, like Petter's with the bacon, eggs, and toast
            race.StartRace();

            Console.WriteLine("Tryck på valfri tangent för att avsluta.");
            Console.ReadKey();
        }
    }
}