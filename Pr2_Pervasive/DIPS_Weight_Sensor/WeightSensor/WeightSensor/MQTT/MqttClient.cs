using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;

namespace WeightSensor.MQTT
{
    // Same as for the RFID Monitor
    class MqttClient
    {
        private const string BrokerUrl = "626582a1d37a4c9da269c096cf520060.s1.eu.hivemq.cloud";
        private const string Username = "dipsgrp4";
        private const string Password = "Dipsgrp4password";
        private const int Port = 8883;
        private uPLibrary.Networking.M2Mqtt.MqttClient _client;

        public void Connect()
        {
            if (_client == null)
            {
                _client = new uPLibrary.Networking.M2Mqtt.MqttClient(BrokerUrl, Port, true, null, null, MqttSslProtocols.TLSv1_2);
            }

            _client.Connect(Guid.NewGuid().ToString(), Username, Password, false, ushort.MaxValue);
        }

        public void Publish(string message, string topic)
        {
            if (!_client.IsConnected)
            {
                Connect();
            }
            _client.Publish(
                topic,
                Encoding.ASCII.GetBytes(message),
                uPLibrary.Networking.M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                false
            );
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }
    }
}