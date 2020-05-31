using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.Logic.Contract.Events
{
    public class UpdateEventCommand : Command
    {
        public Guid EventId { get; set; }
        public EventDto Event { get; set; }
    }
}
