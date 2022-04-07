using System;
using System.Net;
using System.Text.Json;
using PillMonitor.Models;


// Based on guide: https://www.phidgets.com/?view=code_samples&lang=CSharp
namespace PillMonitor
{
    class Program
    {
        

        static void Main(string[] args)
        {
            var rfidMonitor = new RFIDMonitor();
            rfidMonitor.Start();
            Console.Read();
            rfidMonitor.Stop();
        }
    }
}
