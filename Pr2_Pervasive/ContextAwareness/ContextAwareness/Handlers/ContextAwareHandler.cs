using ContextAwareness.DbUtilities;
using ContextAwareness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        private State currentState;
        private SubState currentSubState;

        private bool hasNotBeenPillReminded = true;
        private TimeSpan wakeupTimeAverage = new TimeSpan(8, 30, 0);
        
        private TimeSpan wakeupTimeSlack = new TimeSpan(1, 0, 0); // +- 1 hour window for waking up. 

        public ContextAwareHandler(DbClient client)
        {
            dbClient = client;

            dbClient.NewDataAvailable += DbClient_NewDataAvailable;
        }

        private void DbClient_NewDataAvailable(object sender, NewDataAvailableEventArgs e)
        {
            if (e.data is WeightSensor)
            {

            }
            else if (e.data is RFID)
            {

            }
            else
            {
                // :(
            }
        }

        private void OffBedEvent()
        {
            switch (currentState)
            {
                case State.Sleeping:
                    if(hasNotBeenPillReminded && WithinNormalWakeupWindow(DateTime.Now))
                    {

                    }
                    break;



                default:
                    break;
                
            }
        }

        private void OnBedEvent()
        {

        }

        private void PillTakenEvent()
        {

        }

        private bool WithinNormalWakeupWindow(DateTime dateTime)
        {
            var time = dateTime.TimeOfDay;

            var timespanStart = wakeupTimeAverage - wakeupTimeSlack;
            var timespanEnd = wakeupTimeAverage - wakeupTimeSlack;
         
            return (timespanStart <= time && time <= timespanEnd);
        }
    }
}
