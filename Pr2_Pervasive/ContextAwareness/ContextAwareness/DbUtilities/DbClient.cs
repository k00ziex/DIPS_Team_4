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
        private readonly string pillTakenCollectionName = "PillTakenEvents";

        private readonly IMongoCollection<RFID> rfidCollection;
        private readonly IMongoCollection<WeightSensor> weightCollection;
        private readonly IMongoCollection<Reminder> reminderCollection;
        private readonly IMongoCollection<RFID> pillTakenCollection;

        public DbClient()
        {
            if (client == null)
            {
                client = new MongoClient(connectionString);
            }
            database = client.GetDatabase(databaseName);

            rfidCollection = database.GetCollection<RFID>(rfidCollectionName);
            weightCollection = database.GetCollection<WeightSensor>(weightCollectionName);
            //database.CreateCollection(reminderCollectionName);
            //database.CreateCollection(pillTakenCollectionName);
            reminderCollection = database.GetCollection<Reminder>(reminderCollectionName);
            pillTakenCollection= database.GetCollection<RFID>(pillTakenCollectionName);
        }

        public async Task CreateRFIDAsync(RFID newRFID) =>
            await rfidCollection.InsertOneAsync(newRFID);

        public async Task CreateWeigtAsync(WeightSensor newWeight) =>
            await weightCollection.InsertOneAsync(newWeight);

        public async Task CreateReminderAsync(Reminder newReminder) =>
            await reminderCollection.InsertOneAsync(newReminder);

        public async Task<Reminder> FindReminderAsync(FilterDefinition<Reminder> filter) =>
            await reminderCollection.Find(filter).FirstOrDefaultAsync();

        public async Task CreatePillTakenAsync(RFID newPillTakenEvent) =>
            await pillTakenCollection.InsertOneAsync(newPillTakenEvent);

        public async Task<RFID> FindPillTakenAsync(FilterDefinition<RFID> filter) =>
            await pillTakenCollection.Find(filter).FirstOrDefaultAsync();

        public async Task<List<RFID>> FindPillTakenInIntervalAsync(FilterDefinition<RFID> filter) =>
            await pillTakenCollection.Find(filter).ToListAsync();

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

        public async void DeleteDatabase()
        {
            await client.DropDatabaseAsync(this.databaseName);
        }
    }
}
