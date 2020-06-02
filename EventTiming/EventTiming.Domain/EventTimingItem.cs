using EventTiming.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.Domain
{
    public class EventTimingItem : Entity
    {
        public Event Event { get; set; }
        public Guid EventId { get; set; }
        public string Artist { get; set; }
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
        public string Comments { get; set; }
    }
}
