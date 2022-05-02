using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContextAwareness.DbUtilities;
using MongoDB.Bson;

namespace ContextAwareness.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientCheckupController : ControllerBase
    {
        private readonly ILogger<PatientCheckupController> _logger;
        private readonly DbClient _dbClient;

        public PatientCheckupController(ILogger<PatientCheckupController> logger, DbClient dbClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbClient = dbClient ?? throw new ArgumentNullException(nameof(dbClient));
        }

        [HttpGet("HasTakenDailyPill")]
        public async Task<IActionResult> HasTakenDailyPillAsync()
        {
            var date = DateTime.Today;

            // Query for Timestamp greater than or equal to today
            var filter = new BsonDocument
            {
                {
                    "Timestamp", new BsonDocument{{"$gte", date}}
                }
            };
            var savedEvent = await _dbClient.FindPillTakenAsync(filter);

            if (savedEvent != null) return Ok($"Yes! The pill was taken at approximately {savedEvent.Timestamp} UTC");

            return Ok("No! The pill has not been taken today.");
        }

        [HttpGet("PillsTakenInInterval")]
        public async Task<IActionResult> PillsTakenInInterval(DateTime from, DateTime to)
        {
            // Query for Timestamp greater than or equal to today
            var filter = new BsonDocument
            {
                {
                    "Timestamp", new BsonDocument{{"$gte", from}, {"$lte", to}}
                }
            };
            var savedEvents = await _dbClient.FindPillTakenInIntervalAsync(filter);


            if (savedEvents != null)
            {
                var listOfTagsAndTimestamps = 
                    savedEvents.ToDictionary(savedEvent => savedEvent.Timestamp, savedEvent => savedEvent.Tag);
                return Ok(listOfTagsAndTimestamps);
            }

            return Ok("No pills taken in the given interval.");
        }
    }
}
