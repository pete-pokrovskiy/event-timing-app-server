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
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
    }
}
