using ContextAwareness.DbUtilities;
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

        public ContextAwareHandler(DbClient client)
        {
            dbClient = client;

            dbClient.NewDataAvailable += DbClient_NewDataAvailable;
        }

        private void DbClient_NewDataAvailable(object sender, NewDataAvailableEventArgs e)
        {
            //Maybe check the type of the event data and call the corresponding methods
        }

        private void OffBedEvent()
        {

        }

        private void PillTakenEvent()
        {

        }
    }
}
