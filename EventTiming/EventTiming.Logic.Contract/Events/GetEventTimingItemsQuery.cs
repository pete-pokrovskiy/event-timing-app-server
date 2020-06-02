using EventTiming.Logic.Contract.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.Logic.Contract.Events
{
    public class GetEventTimingItemsQuery : Query
    {
        public Guid EventId { get; set; }
        public Guid? Id { get; set; }
    }
}
