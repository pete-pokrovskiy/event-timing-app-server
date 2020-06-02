using EventTiming.Logic.Contract.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.Logic.Contract.Events
{
    public class GetTimingItemQuery : Query
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
    }
}
