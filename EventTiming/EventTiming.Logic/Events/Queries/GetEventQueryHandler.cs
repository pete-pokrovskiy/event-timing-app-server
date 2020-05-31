using Croc.CFB.Logic.Queries;
using EventTiming.Data;
using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Services.Auth;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventTiming.Logic.Events.Queries
{
    public class GetEventQueryHandler : QueryHandler<GetEventQuery, GetEventQueryResult>
    {
        public GetEventQueryHandler(IUow uow, ICurrentUserDataService currentUserDataService) : base(uow, currentUserDataService)
        {
        }

        public override async Task<GetEventQueryResult> Execute(GetEventQuery query)
        {
            var eventItem = (await _uow.EventRepository.FindBy(e => 
            (e.CreatedById == _currentUserDataService.CurrentUserData.Id || 
            e.ModifiedById == _currentUserDataService.CurrentUserData.Id) 
            && e.Id == query.EventId)).FirstOrDefault();

            if(eventItem == null)
            {
                // TODO: not found exception => 404
                throw new Exception($"Не найдено событие с идентификатором {query.EventId}");
            }

            return new GetEventQueryResult
            {
                // TODO: more or less dtos and auto mapper
                Event = new EventDto
                {
                    Id = eventItem.Id,
                    Name = eventItem.Name,
                    Description = eventItem.Description,
                    CreatedDate = eventItem.CreatedDate,
                    ModifiedDate = eventItem.ModifiedDate,
                    StartDateAndTime = eventItem.StartDate
                }
            };          
        }
    }
}
