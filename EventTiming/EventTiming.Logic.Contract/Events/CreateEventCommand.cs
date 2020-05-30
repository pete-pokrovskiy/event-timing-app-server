using EventTiming.Logic.Contract.Infra;
using System;

namespace EventTiming.Logic.Contract.Events
{
    public class CreateEventCommand : Command
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDateAndTime { get; set; }
    }
}
