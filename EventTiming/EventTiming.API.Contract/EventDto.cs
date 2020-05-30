using System;
using System.ComponentModel.DataAnnotations;

namespace EventTiming.API.Contract
{
    public class EventDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        
        [Required]
        public DateTime StartDateAndTime { get; set; }
    }
}
