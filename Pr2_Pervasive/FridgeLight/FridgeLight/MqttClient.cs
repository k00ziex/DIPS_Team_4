using System;
using Phidget22;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace FridgeLight
{
    class MqttClient
    {
        private const string BrokerUrl = "626582a1d37a4c9da269c096cf520060.s1.eu.hivemq.cloud";
        private const string Username = "dipsgrp4";
        private const string Password = "Dipsgrp4password";
        private const int Port = 8883;
        private uPLibrary.Networking.M2Mqtt.MqttClient _client;
        private readonly DigitalOutput _ch;

        public MqttClient()
        {
            _ch = new DigitalOutput();
            _ch.Channel = 1; // 0 turns on the green light, 1 turns on the red
            _ch.Open(Phidget.DefaultTimeout);
        }

        public void Connect()
        {
            if (_client == null)
            {
                _client = new uPLibrary.Networking.M2Mqtt.MqttClient(BrokerUrl, Port, true, null, null, MqttSslProtocols.TLSv1_2);
            }

            _client.Connect(Guid.NewGuid().ToString(), Username, Password, false, ushort.MaxValue);
            _client.Subscribe(
                new[]{"dipsgrp4/outputs/light/commands/on", "dipsgrp4/outputs/light/commands/off"}, 
                new[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            _client.MqttMsgPublishReceived += (sender, args) =>
            {
                switch (args.Topic)
                {
                    case "dipsgrp4/outputs/light/commands/on":
                        Console.WriteLine("+ Received an ON command.");
                        _ch.DutyCycle = 1;
                        break;
                    case "dipsgrp4/outputs/light/commands/off":
                        Console.WriteLine("- Received an OFF command.");
                        _ch.DutyCycle = 0;
                        break;
                }
            };
        }


        public void Disconnect()
        {
            _client.Disconnect();
        }

        ~MqttClient()
        {
            _ch.Close();
        }
    }
}
