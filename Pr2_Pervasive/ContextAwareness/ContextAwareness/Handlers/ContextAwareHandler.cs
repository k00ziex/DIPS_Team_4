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

        public ContextAwareHandler(DbClient client)
        {
            dbClient = client;

            dbClient.NewDataAvailable += DbClient_NewDataAvailable;
        }

        private void DbClient_NewDataAvailable(object sender, NewDataAvailableEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
