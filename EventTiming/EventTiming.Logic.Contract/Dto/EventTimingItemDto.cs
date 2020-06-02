using System;

namespace EventTiming.Logic.Contract.Dto
{
    public class EventTimingItemDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string Artist { get; set; }
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
        public string Comments { get; set; }
    }
}
