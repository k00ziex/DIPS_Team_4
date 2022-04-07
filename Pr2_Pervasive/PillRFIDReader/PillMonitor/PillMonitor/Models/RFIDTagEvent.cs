using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillMonitor.Models
{
    class RFIDTagEvent
    {
        public string Tag { get; set; }
        public string State { get; set; } // ["Detected", "Lost"]
        public DateTime Timestamp{ get; set; } 

        public RFIDTagEvent(string tag, DateTime timestamp, string state)
        {
            Tag = tag;
            Timestamp = timestamp;
            State = state;
        }
    }
}
