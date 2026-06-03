using CarRace;

namespace CarRace;

internal class Program
{
    static void Main(string[] args)
    {
        var cars = new List<Car>
        {
            new Car { Name = "Volvo" },
            new Car { Name = "Saab" }
        };

        Console.WriteLine("Biltävlingen startar!");
        Console.WriteLine("Banan är 5 km lång.");
        Console.WriteLine("Alla bilar startar på 120 km/h.");

        Console.WriteLine("Tryck Enter för att avsluta (ingen tävling ännu)...");
        Console.ReadLine();
    }
}