using PillMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Phidget22;

namespace PillMonitor
{
    class RFIDMonitor
    {

        private MqttClient mqttClient;
        private const string topic = "dipsgrp4/sensors/pillmonitor/rfidreader";
        private string rfidReaderID;
        private RFID rfidReader;

        private void Publish2Mqtt(string msg)
        {
            mqttClient.Publish(msg, topic + "/" + rfidReaderID);
        }

        private void OnTagDetected(object sender, Phidget22.Events.RFIDTagEventArgs e)
        {
            var tagEvent = new RFIDTagEvent(
                e.Tag,
                DateTime.UtcNow,
                "Detected"
                );

            var tagEventJson = JsonSerializer.Serialize(tagEvent);

            Console.WriteLine("Tag Detected:\n" + tagEventJson);

            Publish2Mqtt(tagEventJson);
        }

        private void OnTagLost(object sender, Phidget22.Events.RFIDTagLostEventArgs e)
        {
            var tagEvent = new RFIDTagEvent(
                e.Tag,
                DateTime.UtcNow,
                "Lost"
                );

            var tagEventJson = JsonSerializer.Serialize(tagEvent);
            Console.WriteLine("Tag Removed:\n" + tagEventJson);

            Publish2Mqtt(tagEventJson);
        }

        public void Start()
        {
            Console.WriteLine(">> Starting RFID Reader Pill Monitor");

            rfidReader = new RFID();
            rfidReader.Tag += OnTagDetected;
            rfidReader.TagLost += OnTagLost;

            mqttClient = new MqttClient();

            try
            {
                mqttClient.Connect();
                rfidReader.Open(5000);
                this.rfidReaderID = rfidReader.DeviceSerialNumber.ToString();
            }
            catch (PhidgetException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("");
                Console.WriteLine("PhidgetException " + ex.ErrorCode + " (" + ex.Description + "): " + ex.Detail);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught:\n" + e.ToString());
            }
        }

        public void Stop()
        {
            Console.WriteLine(">> Stopping RFID Reader Pill Monitor");
            rfidReader.Close();
            mqttClient.Disconnect();
        }
    }
}
