namespace CarRace;

public class Car
{
    public string Name { get; set; } = "";
    public double DistanceKm { get; set; } = 0;
    public double SpeedKmH { get; set; } = 120;
    public bool Finished { get; set; } = false;
    public DateTime? FinishTime { get; set; }
}