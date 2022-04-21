using ContextAwareness.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContextAwareness.DbUtilities
{
    public class NewDataAvailableEventArgs : EventArgs
    {
        public string Type { get; set; }

    }
    public class DbClient
    {
        private MongoClient client;
        private IMongoDatabase database;

        private readonly string connectionString = "mongodb://localhost:27017";
        private readonly string databaseName = "ContextAwareMetrics";
        private readonly string rfidCollectionName = "RFIDData";
        private readonly string weightCollectionName = "WeightSensorData";

        private readonly IMongoCollection<RFID> rfidCollection;
        private readonly IMongoCollection<WeightSensor> weightCollection;

        public DbClient()
        {
            if (client == null)
            {
                client = new MongoClient(connectionString);
            }
            database = client.GetDatabase(databaseName);

            rfidCollection = database.GetCollection<RFID>(rfidCollectionName);
            weightCollection = database.GetCollection<WeightSensor>(weightCollectionName);
        }

        public async Task CreateRFIDAsync(RFID newRFID) =>
            await rfidCollection.InsertOneAsync(newRFID);

        public async Task CreateWeigtAsync(WeightSensor newWeight) =>
            await weightCollection.InsertOneAsync(newWeight);


        public void CreateDataEvent(string type)
        {
            NewDataAvailableEventArgs args = new NewDataAvailableEventArgs();
            args.Type = type;
            OnNewDataAvailable(args);
        }

        public event EventHandler<NewDataAvailableEventArgs> NewDataAvailable;

        protected virtual void OnNewDataAvailable(NewDataAvailableEventArgs e)
        {
            EventHandler<NewDataAvailableEventArgs> handler = NewDataAvailable;
            if(handler != null)
            {
                handler(this, e);
            }
        }
    }
}
