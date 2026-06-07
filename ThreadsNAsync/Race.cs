using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsNAsync
{
    internal class Race
    {

        // RunCar method: what each car thread does
        private readonly List<Car> _cars = new List<Car>();
        // To protect shared race state and car properties, we use a lock object.
        // This is like the frying pan in Petter's example that we used
        // to cook the bacon, eggs, and toast without them getting mixed up.
        private readonly object _lock = new();
        private readonly object _consoleLock = new();

        private bool _raceFinished = false;
        private string? _winner = null;

        public void AddCar(Car car)
        {
            _cars.Add(car);
        }

        // ConsoleWriteLineSafe: to avoid interleaving of console output from
        // different threads
        private void ConsoleWriteLineSafe(string message)
            {
                lock (_consoleLock)
                {
                    Console.WriteLine(message);
                }
            }

        public void StartRace()
        {
            List<Thread> carThreads = new();

            ConsoleWriteLineSafe("Biltävlingen startar nu!");
            ConsoleWriteLineSafe("Banan är 5 km lång.");
            ConsoleWriteLineSafe("Alla bilar startar på 120 km/h.");
            ConsoleWriteLineSafe("Tryck Enter eller skriv 'status' för att visa status.");
            ConsoleWriteLineSafe("");

            // Start a thread for each car, similar to Petter's example with the bacon, eggs, and toast
            foreach (var car in _cars)
            {
                Thread carThread = new Thread(() => RunCar(car));
                carThreads.Add(carThread);
                carThread.Start();
            }

            // Vänta tills alla biltrådar är klara
            foreach (var thread in carThreads)
            {
                // Join() is used to wait for a thread to finish before proceeding.
                // In this case, we want to wait for all car threads to finish before
                // we allow the main thread to exit.
                // Like waiting for the bacon, eggs, and toast to be ready before we eat breakfast.
                thread.Join();
            }

            _raceFinished = true;

            // Communicate the user the race is over
            ConsoleWriteLineSafe("");
            ConsoleWriteLineSafe("Tävlingen är slut!"); 
        }
    
        // HandleRandomEvent / HandeRandomEventAsync
        private void RunCar (Car car)
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                int secondsPassed = 0;

                ConsoleWriteLineSafe($"{car.Name} har started.");

                while (!_raceFinished && ! car.Finished)
                {
                    // 1 second passes
                    Thread.Sleep(1000);
                    secondsPassed++;

                    lock (_lock)
                    {
                        if (_raceFinished || car.Finished)
                        {
                            return;
                        }

                    // Calculate distance, consider app uses ms and speed is km/h (1h = 3600 sec)
                    car.DistanceKm += (double)car.SpeedKmH / 3600;
                    if (car.DistanceKm >= 5)
                        {
                            car.DistanceKm = 5;
                            car.Finished = true;

                            if (_winner == null)
                            {
                                _winner = car.Name;
                                ConsoleWriteLineSafe($"{car.Name} kom i mål och har vunnit tävlingen!");
                            }
                            else
                            {
                                ConsoleWriteLineSafe($"{car.Name} har kommit i mål!");
                            }

                            if (_cars.All(c => c.Finished))
                            {
                                _raceFinished = true;
                                ConsoleWriteLineSafe("Alla biler har kommit i mål! Nu är tävlingen slut!");
                            }

                            return;
                        }
                    }

                    // Every ten seconds send a random event
                    //if (secondsPassed % 10 == 0)
                    if (secondsPassed % 10== 0)
                    {
                        // Here is where the accidents are RANDOMly happen/run
                        HandleRandomEventAsync(car, random).Wait();
                    }
                }

            }

        // Applying HandleRandomEventAsync method
        private async Task HandleRandomEventAsync(Car car, Random random)
        {
            int roll = random.Next(1, 51);

            if (roll == 1)
            {
                ConsoleWriteLineSafe($"{car.Name}: Slut på bensin! Stannar i 15 sekunder.");
                await Task.Delay(15000);
            }
            else if (roll <= 3)
            {
                ConsoleWriteLineSafe($"{car.Name}: Punktering! Stannar i 10 sekunder.");
                await Task.Delay(10000);
            }
            else if (roll <= 8)
            {
                ConsoleWriteLineSafe($"{car.Name}: Fågel på vindrutan! Stannar i 5 sekunder.");
                await Task.Delay(5000);
            }
            else if (roll <= 18)
            {
                lock (_lock)
                {
                    if (car.SpeedKmH > 60)
                    {
                        car.SpeedKmH -= 1;
                        ConsoleWriteLineSafe($"{car.Name}: Motorfel! Ny hastighet: {car.SpeedKmH} km/h.");
                    }
                    else
                    {
                        ConsoleWriteLineSafe($"{car.Name}: Motorfel, men bilen håller minimihastigheten 60 km/h.");
                    }
                }
            }
        
        }

            // ListenForStatus / ShowStatus
            public void ListenForStatus()
            {
                // If the race is not finished, read the user input.
                while (!_raceFinished)
                {

                    string? input = Console.ReadLine();
                    // If it's finished, exit the method and end the thread
                    if (_raceFinished)
                    {
                        return;
                    }

                    // Otherwise, if the user types "status", show the status of
                    if (string.IsNullOrWhiteSpace(input) || input.Trim().ToLower() == "status")
                    {
                        ShowStatus();
                    }
                    // If the user types "exit", end the race
                    else if (input.Trim().ToLower() == "exit")
                    {
                        _raceFinished = true;
                        return;
                    }
                }
            }

            private void ShowStatus()
            {
                lock ( _lock)
                {
                    ConsoleWriteLineSafe("");
                    ConsoleWriteLineSafe("Statusuppdatering:");

                    foreach (var car in _cars)
                    {
                        ConsoleWriteLineSafe(
                            $"{car.Name} - Sträcka: {car.DistanceKm:F2} km, Hastighet: {car.SpeedKmH} km/h");
                    }

                    ConsoleWriteLineSafe("");
                }
            }

    }
}