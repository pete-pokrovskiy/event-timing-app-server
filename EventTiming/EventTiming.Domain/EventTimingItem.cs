using EventTiming.Domain.Base;
using System;

namespace EventTiming.Domain
{
    public class EventTimingItem : Entity
    {
        public Event Event { get; set; }
        public Guid EventId { get; set; }
        public string Artist { get; set; }
        /// <summary>
        /// Start date and time of item - property depends on the previous items in list
        /// </summary>
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
        public string Comments { get; set; }
        /// <summary>
        /// Order of current timing in event's list
        /// </summary>
        public int Order { get; set; }
    }
}
