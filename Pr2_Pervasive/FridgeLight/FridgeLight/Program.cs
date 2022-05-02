using System;

namespace FridgeLight
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("----- Starting Fridge Light ------");

            var mqtt = new MqttClient();
            mqtt.Connect();

            while (true)
            {
                // Do nothing
            }
        }
    }
}
