using System;

namespace WeightSensor.Models
{
    public class BedEvent
    {
        public string DeviceId { get; set; }
        public string State { get; set; } // ["Awake", "Asleep"]
        public DateTime Timestamp { get; set; }

        public BedEvent(string deviceId, DateTime timestamp, string state)
        {
            DeviceId = deviceId;
            Timestamp = timestamp;
            State = state;
        }
    }
}