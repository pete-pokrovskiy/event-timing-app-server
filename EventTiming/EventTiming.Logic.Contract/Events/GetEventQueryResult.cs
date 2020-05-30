using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Infra;

namespace EventTiming.Logic.Contract.Events
{
    public class GetEventQueryResult : QueryResult
    {
        public EventDto Event { get; set; }
    }
}
