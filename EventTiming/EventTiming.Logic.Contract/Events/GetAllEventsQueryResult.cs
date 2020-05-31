using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Infra;
using System.Collections.Generic;

namespace EventTiming.Logic.Contract.Events
{
    public class GetAllEventsQueryResult : QueryResult
    {
        public List<EventDto> Events { get; set; }

    }
}
