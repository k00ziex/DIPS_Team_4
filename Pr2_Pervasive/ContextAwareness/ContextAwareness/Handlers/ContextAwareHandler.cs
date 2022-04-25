using ContextAwareness.DbUtilities;
using ContextAwareness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContextAwareness.Mqtt;
using System.Text.Json;
using System.Timers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ContextAwareness.Handlers
{
    public class ContextAwareHandler
    {
        private readonly DbClient dbClient;
        private readonly MqttClient mqttClient;
        private Timer oneHourTimer;

        private enum State
        {
            Sleeping = 0,
            Awake = 1,
        }

        private enum SubState
        {
            RemindAndAwaitMedicine = 0,
            AllowedToEatOrDrink = 1,
            NotAllowedToEatOrDrink = 2,
        }

        private readonly string lightTopic = "dipsgrp4/outputs/light/commands";
        private readonly string pillReminderTopic = "dipsgrp4/outputs/smartphone/commands/pillreminder";

        private State currentState = State.Sleeping;
        private SubState? currentSubState = null;

        private TimeSpan wakeupTimeAverage = new TimeSpan(8, 30, 0);
        
        private TimeSpan wakeupTimeSlack = new TimeSpan(12, 0, 0); // +- 1 hour window for waking up. 

        public ContextAwareHandler(DbClient client, MqttClient mqttClient)
        {
            dbClient = client ?? throw new ArgumentNullException(nameof(client));
            this.mqttClient = mqttClient ?? throw new ArgumentNullException(nameof(mqttClient));

            dbClient.NewDataAvailable += DbClient_NewDataAvailable;

            // TODO: (Out of scope) Infer state? Should state change events be saved in DB?
            Console.WriteLine("\nContextAwareHandler started");
            PrintState();
        }

        private void DbClient_NewDataAvailable(object sender, NewDataAvailableEventArgs e)
        {
            
            Console.WriteLine("\n### Received new data:\n" + JsonSerializer.Serialize(e.data));
            Console.WriteLine("State was:");
            PrintState();

            if (e.data is WeightSensor bedEvent)
            {
                switch (bedEvent.State)
                {
                    case "On Bed":
                        OnBedEvent(bedEvent);
                        break;
                    case "Off Bed":
                        OffBedEvent(bedEvent);
                        break;

                    default:
                        throw new ArgumentException("Wrong bed event state");
                }
            }
            else if (e.data is RFID pillEvent)
            {
                switch (pillEvent.State)
                {
                    case "Detected":
                        break;

                    case "Lost":
                        PillTakenEvent(pillEvent);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                // :(
            }

            Console.WriteLine("State is now:");
            PrintState();
        }

        private void OffBedEvent(WeightSensor sensorData)
        {
            if (currentState == State.Sleeping)
            {
                if (!HasBeenRemindedToday() && WithinNormalWakeupWindow(sensorData.Timestamp))
                {
                    // Send wakeup time to DB? For calculating average. But is hardcoded for now.
                    currentState = State.Awake;
                    currentSubState = SubState.RemindAndAwaitMedicine;

                    SendPillReminderCommand();
                }
                else if (TimePassedSincePillTaken() < new TimeSpan(1, 0, 0)
                         && HasBeenRemindedToday())
                {
                    currentState = State.Awake;
                    currentSubState = SubState.NotAllowedToEatOrDrink;

                    SendLightCommand("ON");
                }
                else if (TimePassedSincePillTaken() >= new TimeSpan(1, 0, 0) // TODO find out why wrong state - something with time.
                         && HasBeenRemindedToday())
                {
                    currentState = State.Awake;
                    currentSubState = SubState.AllowedToEatOrDrink;
                    SendLightCommand("Off");
                }
            }
            else
            {
                // Do nothing if we're in an "Awake" state
            }
        }

        private void OnBedEvent(WeightSensor sensorData)
        {
            if (currentState == State.Awake)
            {
                currentState = State.Sleeping;
                currentSubState = null;
            }
        }

        private async void PillTakenEvent(RFID sensorData)
        {
            if(currentState == State.Awake && currentSubState == SubState.RemindAndAwaitMedicine)
            {
                currentState = State.Awake;
                currentSubState = SubState.NotAllowedToEatOrDrink;
                
                SendLightCommand("On");
                await dbClient.CreatePillTakenAsync(sensorData);
                InitAndStartOneHourTimer();
            }
            else
            {
                Console.WriteLine("Pill has already been taken, but is taken again :("); 
            }
        }

        private void OneHourPassedEvent(object sender, EventArgs e)
        {
            if(currentState == State.Awake && currentSubState == SubState.NotAllowedToEatOrDrink)
            {
                currentState = State.Awake;
                currentSubState = SubState.AllowedToEatOrDrink;
            }
            SendLightCommand("Off");
            PrintState();
        }

        private bool WithinNormalWakeupWindow(DateTime dateTime)
        {
            var time = dateTime.TimeOfDay;

            var timespanStart = wakeupTimeAverage - wakeupTimeSlack;
            var timespanEnd = wakeupTimeAverage + wakeupTimeSlack;
         
            return (timespanStart <= time && time <= timespanEnd);
        }

        private bool HasBeenRemindedToday()
        {
            var date = DateTime.Now.Date;
            // Query for Timestamp greater than or equal to today
            var dateQuery = new BsonDocument
            {
                {
                    "Timestamp", new BsonDocument{{"$gte", date}}
                }
            };

            var reminder = dbClient.FindReminderAsync(dateQuery).Result;
            if(reminder == null)
            {
                return false;
            }
            return reminder.Timestamp.Date == date;
        }
        
        private TimeSpan TimePassedSincePillTaken()
        {
            var date = DateTime.Today;
            
            var today = DateTime.Today;
            // Query for Timestamp greater than or equal to today
            var filter = new BsonDocument
            {
                {
                    "Timestamp", new BsonDocument{{"$gte", date}}
                }
            };
            var savedEvent = dbClient.FindPillTakenAsync(filter).Result;
            if(savedEvent == null) return TimeSpan.Zero;
            return DateTime.Now.TimeOfDay - savedEvent.Timestamp.TimeOfDay;
        }

        private void SendLightCommand(string onOrOff)
        {
            var lightCommand = new LightCommand
            {
                Data = onOrOff.ToUpperInvariant()
            };
            var lightCommandToJson = JsonSerializer.Serialize(lightCommand);
            mqttClient.Publish(lightCommandToJson, $"{lightTopic}/{onOrOff.ToLowerInvariant()}");
        }

        private async void SendPillReminderCommand()
        {
            var reminderModel = new Reminder
            {
                Comment = "It is time for you to take your daily pull",
                //Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            var reminderModelToJson = JsonSerializer.Serialize(reminderModel);
            mqttClient.Publish(reminderModelToJson, $"{pillReminderTopic}");
            await dbClient.CreateReminderAsync(reminderModel);
        }

        private void PrintState()
        {
            Console.WriteLine($"State: {currentState}");
            if(currentSubState != null) 
            { 
                Console.WriteLine($"SubState: {currentSubState}"); 
            }
            Console.WriteLine();
        }

        private void InitAndStartOneHourTimer()
        {
            Console.WriteLine("Timer elapsed");
            if (oneHourTimer == null) 
            {
                var seconds = 60;
                var conversionRatio = 1000;
                oneHourTimer = new Timer(seconds * conversionRatio);
                oneHourTimer.Elapsed += OneHourPassedEvent;
            }

            oneHourTimer.AutoReset = false;
            oneHourTimer.Start();
        }
    }
}
