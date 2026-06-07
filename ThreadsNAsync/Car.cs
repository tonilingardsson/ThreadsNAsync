namespace ThreadsNAsync
{
    public class Car
    {
        public string Name { get; set; }
        public double DistanceKm { get; set; }
        public int SpeedKmH { get; set; }
        public bool Finished { get; set; }

        public Car(string name)
        {
            Name = name;
            DistanceKm = 0;
            SpeedKmH = 120;
            Finished = false;
        }
    }
}