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

        /// <summary>
        /// Debug flag, enabling this means that:
        /// 1. Wakeup time can be anytime.
        /// 2. Allowed to eat and drink latency is 20 seconds.
        /// </summary>
        private const bool DEMO_MODE_TIMING = true;
        private TimeSpan allowedToDrinkAndEatLatency = new TimeSpan(0, 1, 0); // 1 hour
        /// <summary>
        /// Flag: Drops database on startup, use to "forget" that user has been reminded of pill, taken pill etc. 
        /// </summary>
        private const bool DEMO_MODE_CLEAR_DB_ON_STARTUP = true;

        private readonly DbClient dbClient;
        private readonly MqttClient mqttClient;
        private Timer eatAndDrinkLatencyTimer;



        private readonly string lightTopic = "dipsgrp4/outputs/light/commands";
        private readonly string pillReminderTopic = "dipsgrp4/outputs/smartphone/commands/pillreminder";

        private TimeSpan wakeupTimeAverage = new TimeSpan(8, 30, 0);

        private TimeSpan wakeupTimeSlack = new TimeSpan(1, 0, 0); // +- 1 hour window for waking up. 

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
        private State currentState = State.Sleeping;
        private SubState? currentSubState = null;
        
        public ContextAwareHandler(DbClient client, MqttClient mqttClient)
        {
            dbClient = client ?? throw new ArgumentNullException(nameof(client));
            this.mqttClient = mqttClient ?? throw new ArgumentNullException(nameof(mqttClient));

            dbClient.NewDataAvailable += DbClient_NewDataAvailable;

            if (DEMO_MODE_TIMING)
            {
                wakeupTimeSlack = new TimeSpan(24, 0, 0); // All wakeup is within timespan
                allowedToDrinkAndEatLatency = new TimeSpan(0, 0, 20);  // Override to 20 seconds
            }
            if (DEMO_MODE_CLEAR_DB_ON_STARTUP)
            {
                dbClient.DeleteDatabase(); // Delete DB to allow for clean flow
            }

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
                if (!HasBeenRemindedToday() && WithinNormalWakeupWindow(sensorData.Timestamp)
                    || !HasTakenPill())
                {
                    // Send wakeup time to DB? For calculating average. But is hardcoded for now.
                    SleepingToRemindAndAwaitMedicineIntakeTransition();
                }
                else if (TimePassedSincePillTaken() < allowedToDrinkAndEatLatency // Not allowed to eat and drink yet
                         && HasBeenRemindedToday())
                {
                    SleepingToNotAllowedToEatDrinkTransition();
                }
                else if (TimePassedSincePillTaken() >= allowedToDrinkAndEatLatency // Allowed to eat and drink
                         && HasBeenRemindedToday())
                {
                    SleepingToAllowedEatDrinkTransition();
                }
            }
            else
            {
                // Do nothing if we're in an "Awake" state
            }
        }

        #region State Transition functions

        private void SleepingToAllowedEatDrinkTransition()
        {
            currentState = State.Awake;
            currentSubState = SubState.AllowedToEatOrDrink;
            SendLightCommand("Off");
        }

        private void SleepingToNotAllowedToEatDrinkTransition()
        {
            currentState = State.Awake;
            currentSubState = SubState.NotAllowedToEatOrDrink;

            SendLightCommand("ON");
        }

        private void SleepingToRemindAndAwaitMedicineIntakeTransition()
        {
            currentState = State.Awake;
            currentSubState = SubState.RemindAndAwaitMedicine;

            SendPillReminderCommand();
        }

        private void ToSleepingTransition()
        {
            if (currentState != State.Awake)
            {
                return;
            }
            currentState = State.Sleeping;
            currentSubState = null;
        }

        private async Task RemindAndAwaitMedicineIntakeToNotAllowedEatDrinkTransition(RFID sensorData)
        {
            currentState = State.Awake;
            currentSubState = SubState.NotAllowedToEatOrDrink;

            SendLightCommand("On");
            await dbClient.CreatePillTakenAsync(sensorData);
            InitAndStartEatAndDrinkLatencyTimer();
        }

        private void NotAllowedToEatOrDrinkToAllowedToEatOrDrinkTransition()
        {
            currentState = State.Awake;
            currentSubState = SubState.AllowedToEatOrDrink;
        }

        #endregion

        #region Event functions - Sensors & timer
        private void OnBedEvent(WeightSensor sensorData)
        {
            ToSleepingTransition();
        }

        private async void PillTakenEvent(RFID sensorData)
        {
            if(currentState == State.Awake && currentSubState == SubState.RemindAndAwaitMedicine)
            {
                await RemindAndAwaitMedicineIntakeToNotAllowedEatDrinkTransition(sensorData);
            }
            else
            {
                Console.WriteLine("Pill has already been taken, but is taken again :("); 
            }
        }

        private void OneHourPassedEvent(object sender, EventArgs e)
        {
            if (currentState == State.Awake && currentSubState == SubState.NotAllowedToEatOrDrink)
            {
                NotAllowedToEatOrDrinkToAllowedToEatOrDrinkTransition();
            }
            SendLightCommand("Off");
            PrintState();
        }

        #endregion

        #region Decision utility functions 
        private bool HasTakenPill()
        {
            var date = DateTime.Today;

            // Query for Timestamp greater than or equal to today
            var filter = new BsonDocument
            {
                {
                    "Timestamp", new BsonDocument{{"$gte", date}}
                }
            };
            var savedEvent = dbClient.FindPillTakenAsync(filter).Result;
            if (savedEvent == null)
            {
                return false;
            }
            else
            {
                return true;
            }
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
            var date = DateTime.UtcNow.Date;
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
            
            // Query for Timestamp greater than or equal to today
            var filter = new BsonDocument
            {
                {
                    "Timestamp", new BsonDocument{{"$gte", date}}
                }
            };
            var savedEvent = dbClient.FindPillTakenAsync(filter).Result;

            return DateTime.UtcNow.TimeOfDay - savedEvent.Timestamp.TimeOfDay;
        }

        #endregion

        private void PrintState()
        {
            Console.WriteLine($"State: {currentState}");
            if (currentSubState != null)
            {
                Console.WriteLine($"SubState: {currentSubState}");
            }
            Console.WriteLine();
        }

        private void InitAndStartEatAndDrinkLatencyTimer()
        {
            Console.WriteLine("Timer elapsed");
            if (eatAndDrinkLatencyTimer == null)
            {
                eatAndDrinkLatencyTimer = new Timer(allowedToDrinkAndEatLatency.TotalMilliseconds);
                //if (DEMO_MODE)
                //{
                //    var seconds = 20;
                //    var toSecondsConversionRatio = 1000;
                //    oneHourTimer = new Timer(allowedToDrinkAndEatLatency.TotalMilliseconds);
                //    oneHourTimer.Elapsed += OneHourPassedEvent;
                //}
                //else
                //{
                //    var minutes = 60;
                //    var secondsInAMinute = 60;
                //    var toSecondsConversionRatio = 1000;
                //    oneHourTimer = new Timer(minutes * secondsInAMinute* toSecondsConversionRatio);
                //    oneHourTimer.Elapsed += OneHourPassedEvent;
                //}

            }

            eatAndDrinkLatencyTimer.AutoReset = false;
            eatAndDrinkLatencyTimer.Start();
        }

        

        #region Output Commands
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
                Comment = "It is time for you to take your daily pill",
                //Id = Guid.NewGuid().ToString(), // Done by DB
                Timestamp = DateTime.UtcNow
            };
            var reminderModelToJson = JsonSerializer.Serialize(reminderModel);
            mqttClient.Publish(reminderModelToJson, $"{pillReminderTopic}");
            await dbClient.CreateReminderAsync(reminderModel);
        }

        #endregion

    }
}
