using System;

namespace FridgeLight
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var mqtt = new MqttClient();
            mqtt.Connect();

            while (true)
            {
                // Do nothing
            }
        }
    }
}
