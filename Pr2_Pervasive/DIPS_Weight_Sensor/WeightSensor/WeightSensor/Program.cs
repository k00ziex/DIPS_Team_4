using System;
using System.Collections.Generic;
using System.Linq;
using Phidget22;

namespace WeightSensor
{
    class Program
    {
        static void Main(string[] args)
		{
            var bedMonitor = new BedMonitor();
            bedMonitor.Start();
        }
    }
}
