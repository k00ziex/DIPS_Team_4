using System;

namespace WeightSensor.Models
{
    public class BedEvent
    {
        public string DeviceId { get; set; }
        public string State { get; set; } // ["On Bed", "Off Bed"]
        public DateTime Timestamp { get; set; }

        public BedEvent(string deviceId, DateTime timestamp, string state)
        {
            DeviceId = deviceId;
            Timestamp = timestamp;
            State = state;
        }
    }
}