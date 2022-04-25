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

        [HttpGet]
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

            if (savedEvent != null) return Ok($"Yes! The pill was taken at: {savedEvent.Timestamp}");

            return Ok("No! The pill has not been taken today.");
        }
    }
}
