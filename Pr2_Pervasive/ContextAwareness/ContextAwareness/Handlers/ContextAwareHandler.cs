using ContextAwareness.DbUtilities;
using ContextAwareness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;


namespace ContextAwareness.Handlers
{


    public class ContextAwareHandler
    {
        private readonly DbClient dbClient;

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

        private bool hasNotBeenPillReminded = true;
        private TimeSpan wakeupTimeAverage = new TimeSpan(8, 30, 0);
        
        private TimeSpan wakeupTimeSlack = new TimeSpan(1, 0, 0); // +- 1 hour window for waking up. 

        public ContextAwareHandler(DbClient client)
        {
            dbClient = client;

            dbClient.NewDataAvailable += DbClient_NewDataAvailable;

            // Infer state? Should state change events be saved in DB?
            Console.WriteLine("\nContextAwareHandler started");
            PrintState();
        }

        private void DbClient_NewDataAvailable(object sender, NewDataAvailableEventArgs e)
        {
            Console.WriteLine("Received data:\n" + JsonSerializer.Serialize(e.data));
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

            PrintState();
        }

        private void OffBedEvent(WeightSensor sensorData)
        {
            switch (currentState)
            {
                case State.Sleeping:
                    if ( !HasBeenRemindedToday() && WithinNormalWakeupWindow(sensorData.Timestamp) )
                    {
                        // Send wakeup time to DB? For calculating average. But is hardcoded for now.
                        currentState = State.Awake;
                        currentSubState = SubState.RemindAndAwaitMedicine;

                        // TODO: Pill reminder. 
                        SendPillRemminderCommand();
                    }
                    else if (TimePassedSincePillTaken() < new TimeSpan(1,0,0)
                        && HasBeenRemindedToday())
                    {
                        currentState = State.Awake;
                        currentSubState = SubState.NotAllowedToEatOrDrink;

                        // Lightcommand on
                        SendLightOnCommand();
                    }
                    else if (TimePassedSincePillTaken() >= new TimeSpan(1,0,0) 
                        && HasBeenRemindedToday())
                    {

                    }
                    break;
                default:
                    break;
                
            }
        }

        

        private void OnBedEvent(WeightSensor sensorData)
        {
            currentState = State.Sleeping;
            currentSubState = null;
        }

        private void PillTakenEvent(RFID sensorData)
        {

        }

        private bool WithinNormalWakeupWindow(DateTime dateTime)
        {
            var time = dateTime.TimeOfDay;

            var timespanStart = wakeupTimeAverage - wakeupTimeSlack;
            var timespanEnd = wakeupTimeAverage - wakeupTimeSlack;
         
            return (timespanStart <= time && time <= timespanEnd);
        }

        private bool HasBeenRemindedToday()
        {
            throw new NotImplementedException(); // TODO: DO
        }
        
        private TimeSpan TimePassedSincePillTaken()
        {
            throw new NotImplementedException(); // TODO: DO
        }

        private void SendLightOnCommand()
        {
            throw new NotImplementedException();
        }

        private void SendPillRemminderCommand()
        {
            throw new NotImplementedException();
        }

        private void PrintState()
        {
            Console.WriteLine("\nContextAwareHandler State is:");
            Console.Write($"State: {currentState}");
            if(currentSubState != null) 
            { 
                Console.WriteLine($"SubState: {currentSubState}"); 
            }
        }
    }
}
