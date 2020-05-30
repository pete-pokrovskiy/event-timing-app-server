using Croc.CFB.Logic.Queries;
using EventTiming.Data;
using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Events;
using System.Threading.Tasks;

namespace EventTiming.Logic.Events.Queries
{
    public class GetEventQueryHandler : QueryHandler<GetEventQuery, GetEventQueryResult>
    {
        public GetEventQueryHandler(IUow uow) : base(uow)
        {
        }

        public override async Task<GetEventQueryResult> Execute(GetEventQuery query)
        {
            var eventITem = await _uow.EventRepository.FindByKeyAsync(query.EventId);


            return new GetEventQueryResult
            {
                // TODO: more or less dtos and auto mapper
                Event = new EventDto
                {
                    Id = eventITem.Id,
                    Name = eventITem.Name,
                    Description = eventITem.Description,
                    CreatedDate = eventITem.CreatedDate,
                    ModifiedDate = eventITem.ModifiedDate,
                    StartDateAndTime = eventITem.StartDate
                }
            };          
        }
    }
}
