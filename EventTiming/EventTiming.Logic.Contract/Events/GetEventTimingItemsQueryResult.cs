using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.Logic.Contract.Events
{
    public class GetEventTimingItemsQueryResult : QueryResult
    {
        public List<EventTimingItemDto> TimingItems { get; set; }
    }
}
