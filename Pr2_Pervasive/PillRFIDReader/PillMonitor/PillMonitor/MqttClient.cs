using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using System.ComponentModel.DataAnnotations;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace PillMonitor
{
    class MqttClient
    {
        private const string brokerUrl = "626582a1d37a4c9da269c096cf520060.s1.eu.hivemq.cloud";
        private const string username = "dipsgrp4";
        private const string password = "Dipsgrp4password";
        private const int port = 8883;
        private uPLibrary.Networking.M2Mqtt.MqttClient client;

        public void Connect()
        {
            if(client == null)
            {
                client = new uPLibrary.Networking.M2Mqtt.MqttClient(brokerUrl, port, true, null, null, MqttSslProtocols.TLSv1_2);
            }

            client.Connect(Guid.NewGuid().ToString(), username, password, false, ushort.MaxValue);
        }

        public void Publish(string message, string topic)
        {
            if (!client.IsConnected)
            {
                Connect();
            }
            client.Publish(
                topic, 
                Encoding.ASCII.GetBytes(message), 
                uPLibrary.Networking.M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, 
                false
                );
        }
    }
}
