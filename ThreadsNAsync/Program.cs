using System.Threading.Tasks;

namespace CarRace;

internal class Program
{
    static async Task Main(string[] args)
    {
        var cars = new List<Car>
        {
            new Car { Name = "Volvo" },
            new Car { Name = "Saab" }
        };

        Console.WriteLine("Biltävlingen startar!");
        Console.WriteLine("Banan är 5 km lång.");
        Console.WriteLine("Alla bilar startar på 120 km/h.");
        Console.WriteLine("Tryck Enter eller skriv 'status' för status. Skriv 'exit' för att avsluta.");
        Console.WriteLine();

        var tasks = cars.Select(car => RunCarAsync(car, cars)).ToList();
        var inputTask = ListenForInputAsync(cars);

        await Task.WhenAll(tasks);
        // When all cars are done, we can let the inputTask finish
        await inputTask;

        Console.WriteLine("Tävlingen är slut. Tryck Enter för att stänga.");
        Console.ReadLine();
    }

    private static bool _raceFinished = false;
    private static readonly object _lock = new();
    private static readonly object _consoleLock = new();
    private static string? _winner;

    static async Task RunCarAsync(Car car, List<Car> cars)
    {
        ConsoleWriteLineSafe($"{car.Name} har startat!");

        while (true)
        {
            await Task.Delay(1000); // 1 second

            lock (_lock)
            {
                if (_raceFinished || car.Finished)
                    return;

                // 120 km/h -> 120 / 3600 km per second
                car.DistanceKm += car.SpeedKmH / 3600.0;

                if (car.DistanceKm >= 5.0)
                {
                    car.DistanceKm = 5.0;
                    car.Finished = true;
                    car.FinishTime = DateTime.Now;

                    if (_winner == null)
                    {
                        _winner = car.Name;
                        ConsoleWriteLineSafe($"{car.Name} gick i mål och VANN tävlingen!");
                    }
                    else
                    {
                        ConsoleWriteLineSafe($"{car.Name} gick i mål.");
                    }

                    if (cars.All(c => c.Finished))
                    {
                        _raceFinished = true;
                    }

                    return;
                }
            }
        }
    }

    static async Task ListenForInputAsync(List<Car> cars)
    {
        while (!_raceFinished)
        {
            string? input = await Task.Run(() => Console.ReadLine());

            if (_raceFinished)
                return;

            if (string.IsNullOrWhiteSpace(input) || input.Trim().ToLower() == "status")
            {
                ShowStatus(cars);
            }
            else if (input.Trim().ToLower() == "exit")
            {
                _raceFinished = true;
                return;
            }
        }
    }

    static void ShowStatus(List<Car> cars)
    {
        lock (_lock)
        {
            ConsoleWriteLineSafe("");
            ConsoleWriteLineSafe("Statusuppdatering:");

            foreach (var car in cars)
            {
                ConsoleWriteLineSafe(
                    $"{car.Name} - Sträcka: {car.DistanceKm:F2} km, Hastighet: {car.SpeedKmH:F0} km/h");
            }

            ConsoleWriteLineSafe("");
        }
    }

    static void ConsoleWriteLineSafe(string message)
    {
        lock (_consoleLock)
        {
            Console.WriteLine(message);
        }
    }

    static async Task HandleRandomEventAsync(Car car, Random random)
    {
        int roll = random.Next(1, 51); // 1–50

        if (roll == 1)
        {
            ConsoleWriteLineSafe($"{car.Name}: Slut på bensin! Stannar 15 sekunder.");
            await Task.Delay(15000);
        }
        else if (roll <= 3)
        {
            ConsoleWriteLineSafe($"{car.Name}: Punktering! Stannar 10 sekunder.");
            await Task.Delay(10000);
        }
        else if (roll <= 8)
        {
            ConsoleWriteLineSafe($"{car.Name}: Fågel på vindrutan! Stannar 5 sekunder.");
            await Task.Delay(5000);
        }
        else if (roll <= 18)
        {
            lock (_lock)
            {
                car.SpeedKmH -= 1;
            }
            ConsoleWriteLineSafe($"{car.Name}: Motorfel! Hastigheten sänks till {car.SpeedKmH} km/h.");
        }
    }
}