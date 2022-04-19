namespace WeightSensor
{
    class Program
    {
        static void Main()
		{
            var bedMonitor = new BedMonitor();
            bedMonitor.Start();
        }
    }
}
