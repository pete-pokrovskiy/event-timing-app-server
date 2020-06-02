using System;
using System.ComponentModel.DataAnnotations;

namespace EventTiming.API.Contract
{
    public class EventTimingItemInput
    {
        [Required]
        public string Artist { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
        public string Comments { get; set; }
    }
}