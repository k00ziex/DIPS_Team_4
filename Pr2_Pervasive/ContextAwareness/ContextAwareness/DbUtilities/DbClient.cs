using ContextAwareness.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContextAwareness.DbUtilities
{
    public class NewDataAvailableEventArgs : EventArgs
    {
        //public string Type { get; set; } 
        public object data { get; set; }

    }
    public class DbClient
    {
        private MongoClient client;
        private IMongoDatabase database;

        private readonly string connectionString = "mongodb://localhost:27017";
        private readonly string databaseName = "ContextAwareMetrics";
        private readonly string rfidCollectionName = "RFIDData";
        private readonly string weightCollectionName = "WeightSensorData";
        private readonly string reminderCollectionName = "Reminders";

        private readonly IMongoCollection<RFID> rfidCollection;
        private readonly IMongoCollection<WeightSensor> weightCollection;
        private readonly IMongoCollection<Reminder> reminderCollection;

        public DbClient()
        {
            if (client == null)
            {
                client = new MongoClient(connectionString);
            }
            database = client.GetDatabase(databaseName);

            rfidCollection = database.GetCollection<RFID>(rfidCollectionName);
            weightCollection = database.GetCollection<WeightSensor>(weightCollectionName);
            reminderCollection = database.GetCollection<Reminder>(reminderCollectionName);

        }

        public async Task CreateRFIDAsync(RFID newRFID) =>
            await rfidCollection.InsertOneAsync(newRFID);

        public async Task CreateWeigtAsync(WeightSensor newWeight) =>
            await weightCollection.InsertOneAsync(newWeight);

        public async Task CreateReminderAsync(Reminder newReminder) =>
            await reminderCollection.InsertOneAsync(newReminder);

        public async Task FindReminderAsync(FilterDefinition<Reminder> filter) =>
            await reminderCollection.Find(filter).FirstOrDefaultAsync();
        

        public void CreateDataEvent(object data)
        {
            NewDataAvailableEventArgs args = new NewDataAvailableEventArgs();
            args.data = data;
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
