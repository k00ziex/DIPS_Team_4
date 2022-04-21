using ContextAwareness.DbUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using MongoDB.Bson.Serialization;
using System.Text;
using ContextAwareness.Models;
using System.Text.Json;

namespace ContextAwareness.Mqtt
{
    public class MqttClient
    {
        private const string brokerUrl = "626582a1d37a4c9da269c096cf520060.s1.eu.hivemq.cloud";
        private const string user = "dipsgrp4";
        private const string password = "Dipsgrp4password";
        private const int port = 8883;
        

        private uPLibrary.Networking.M2Mqtt.MqttClient client;
        private readonly DbClient dbClient;

        public MqttClient(DbClient client)
        {
            dbClient = client;
        }

        public void Connect()
        {
            if (client == null)
            {
                client = new uPLibrary.Networking.M2Mqtt.MqttClient(brokerUrl, port, true, null, null, MqttSslProtocols.TLSv1_2);
            }

            client.Connect(Guid.NewGuid().ToString(), user, password, false, ushort.MaxValue);
        }


        public void Disconnect()
        {
            if (client.IsConnected)
            {
                client.Disconnect();
            }
        }

        public void SetupEvents()
        {
            if(client != null)
            {
                client.MqttMsgSubscribed += Client_MqttMsgSubscribed;
                client.MqttMsgUnsubscribed += Client_MqttMsgUnsubscribed;
                client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            }

        }

        public void ClearEvents()
        {
            if(client != null)
            {
                client.MqttMsgSubscribed -= Client_MqttMsgSubscribed;
                client.MqttMsgUnsubscribed -= Client_MqttMsgUnsubscribed;
                client.MqttMsgPublishReceived -= Client_MqttMsgPublishReceived;
            }
        }

        public void Subscribe(string[] topic, byte[] qosLevel)
        {
            client.Subscribe(topic, qosLevel);
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

        //Might be a bug with the await n shit
        private async void Client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {

            var message = Encoding.UTF8.GetString(e.Message);
            
            if (e.Topic.ToLower().Contains("bedmonitor"))
            {
                var data = JsonSerializer.Deserialize<WeightSensor>(message);
                await dbClient.CreateWeigtAsync(data);
                dbClient.CreateDataEvent(data);
            } 
            else if(e.Topic.ToLower().Contains("pillmonitor"))
            {
                var data = JsonSerializer.Deserialize<RFID>(message);
                await dbClient.CreateRFIDAsync(data);
                dbClient.CreateDataEvent(data);
            }
            
            
            //Console.WriteLine(e.Topic);
            //Console.WriteLine(e.Message);
        }

        private void Client_MqttMsgUnsubscribed(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgUnsubscribedEventArgs e)
        {
            Console.WriteLine("Unsubscribed from: " + e.ToString());
        }

        private void Client_MqttMsgSubscribed(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine("Subscribed to: " + e.ToString());
        }

        ~MqttClient()
        {
            Disconnect();
            ClearEvents();
        }
    }
}
