using Croc.CFB.Logic.Queries;
using EventTiming.Data;
using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Services.Auth;
using System.Linq;
using System.Threading.Tasks;

namespace EventTiming.Logic.Events.Queries
{
    public class GetAllEventsQueryHandler : QueryHandler<GetAllEventsQuery, GetAllEventsQueryResult>
    {
        public GetAllEventsQueryHandler(IUow uow, ICurrentUserDataService currentUserDataService) : base(uow, currentUserDataService)
        {
        }

        public override async Task<GetAllEventsQueryResult> Execute(GetAllEventsQuery query)
        {
            var eventItems = await _uow.EventRepository.FindBy(e =>
           e.CreatedById == _currentUserDataService.CurrentUserData.Id ||
           e.ModifiedById == _currentUserDataService.CurrentUserData.Id
           );

            return new GetAllEventsQueryResult
            {
                Events = eventItems.Select(e => new EventDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    CreatedDate = e.CreatedDate,
                    ModifiedDate = e.ModifiedDate,
                    StartDateAndTime = e.StartDate
                }).ToList()
            };
        }
    }
}
