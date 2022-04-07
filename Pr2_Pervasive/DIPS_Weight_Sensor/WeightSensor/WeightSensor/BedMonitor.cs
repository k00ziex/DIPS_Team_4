using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Phidget22;
using WeightSensor.Models;

namespace WeightSensor
{
    // INSPIRED BY: https://www.phidgets.com/?view=code_samples&lang=CSharp with example for channel 2 for the device 1046_0 - PhidgetBridge 4-Input
    public class BedMonitor
    {
        private const double MConstant = -32405877.610081911;
        private const double AverageWeight0Reading = -9.1820593870967751E-06;

        private MqttClient _mqttClient;
        private const string Topic = "dipsgrp4/sensors/bedmonitor/weightsensor";
        private string _deviceId;

        private bool _isOnBed = true;
        private const double WeightThreshold = 3000.0; // Should be a lot higher in reality, but to actually trigger the events we have lowered it

        private void Publish2Mqtt(string msg)
        {
            _mqttClient.Publish(msg, Topic + "/" + _deviceId);
        }

        private void PrintWeight(object sender, Phidget22.Events.VoltageRatioInputVoltageRatioChangeEventArgs e)
        {
            var weightInGrams = -(AverageWeight0Reading - e.VoltageRatio) * MConstant; // Calculation based on: https://www.phidgets.com/docs/Calibrating_Load_Cells
            if (weightInGrams > WeightThreshold && _isOnBed) // If we've previously read 0kg as the weight, then we must react to this event if it is also sensing above 30 kg
            {
                var bedEvent = new BedEvent(_deviceId, DateTime.UtcNow, "On Bed");

                var bedEventJson = JsonSerializer.Serialize(bedEvent);
                Console.WriteLine($"Bed Event detected: \n{bedEventJson}");
                Publish2Mqtt(bedEventJson);
                _isOnBed = false;

            } else if (weightInGrams < WeightThreshold && !_isOnBed) // Else if we've previously read 30+ kg as the weight, then we must react if it is sensing below 30 kg now
            {
                var bedEvent = new BedEvent(_deviceId, DateTime.UtcNow, "Off Bed");

                var bedEventJson = JsonSerializer.Serialize(bedEvent);
                Console.WriteLine($"Bed Event detected: \n{bedEventJson}");
                Publish2Mqtt(bedEventJson);
                _isOnBed = true;
            }
            // Else, do nothing
        }

        public void Start()
        {
            Console.WriteLine(">> Starting Bed Monitor - Weight Sensor");

            // Create Phidget channel (Depends on connection in bridge)
            var voltageRatioInput = new VoltageRatioInput { Channel = 2 };

            // Add Event handler
            voltageRatioInput.VoltageRatioChange += PrintWeight;


            _mqttClient = new MqttClient();

            try
            {
                _mqttClient.Connect();

                // Open Phidget with timeout
                voltageRatioInput.Open(5000);
                _deviceId = voltageRatioInput.DeviceSerialNumber.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // Wait for the 'Enter' key to be pressed
            Console.ReadLine();


            // Close the Phidget channel
            voltageRatioInput.Close();
        }


        /**************************************************************************************************************************
         *
         * The following functions and constants are used for calculating MCal and the average reading of when no mass is
         * on the weight.
         *
         * ************************************************************************************************************************/

        //private static int _counts = 0;
        //private static List<double> _valuesReadWeight0 = new List<double>();
        //private static List<double> _valuesReadWeightRedbull = new List<double>();
        //private static readonly double weight0 = 0.0;
        //private static readonly double weightRedbull = 265.95;
        //private static double _mcal = 0.0;
        //private static double _averageWeight0 = 0.0;
        //private static double _averageWeightRedbull = 0.0;

        ///// <summary>
        ///// Used for calculating the average read values of a weight 0 and weight X
        ///// </summary>
        ///// <param name="voltageRatioInput2">The </param>
        //private static void CalculateAveragesAndOffset(VoltageRatioInput voltageRatioInput)
        //{
        //    voltageRatioInput.VoltageRatioChange += VoltageRatioChange_Weight0;
        //    while (_counts < 31)
        //    {

        //    }

        //    voltageRatioInput.VoltageRatioChange -= VoltageRatioChange_Weight0;

        //    Console.ReadLine(); // Wait for 'Enter' key to be pressed, so we can put on the weighted object
        //    _counts = 0;
        //    voltageRatioInput.VoltageRatioChange += VoltageRatioChange_WeightX;

        //    while (_counts < 31)
        //    {

        //    }

        //    voltageRatioInput.VoltageRatioChange -= VoltageRatioChange_WeightX;

        //    _averageWeight0 = _valuesReadWeight0.Average();
        //    _averageWeightRedbull = _valuesReadWeightRedbull.Average();

        //    _mcal = (weightRedbull - weight0) / (_averageWeightRedbull - _averageWeight0);
        //}

        //private static void VoltageRatioChange_WeightX(object sender, Phidget22.Events.VoltageRatioInputVoltageRatioChangeEventArgs e)
        //{
        //    Console.WriteLine("VoltageRatio: " + e.VoltageRatio);
        //    _valuesReadWeightRedbull.Add(e.VoltageRatio);
        //    _counts++;
        //}
        //private static void VoltageRatioChange_Weight0(object sender, Phidget22.Events.VoltageRatioInputVoltageRatioChangeEventArgs e)
        //{
        //    Console.WriteLine("VoltageRatio: " + e.VoltageRatio);
        //    _valuesReadWeight0.Add(e.VoltageRatio);
        //    _counts++;
        //}
    }
}