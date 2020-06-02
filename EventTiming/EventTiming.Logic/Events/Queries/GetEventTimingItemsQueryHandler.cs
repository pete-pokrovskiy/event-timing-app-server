using Croc.CFB.Logic.Queries;
using EventTiming.Data;
using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.Logic.Events.Queries
{
    public class GetEventTimingItemsQueryHandler : QueryHandler<GetEventTimingItemsQuery, GetEventTimingItemsQueryResult>
    {
        public GetEventTimingItemsQueryHandler(IUow uow, ICurrentUserDataService currentUserDataService) : base(uow, currentUserDataService)
        {
        }

        public override async Task<GetEventTimingItemsQueryResult> Execute(GetEventTimingItemsQuery query)
        {
            var timingItems = (await _uow.EventTimingItemRepository.FindBy(i => i.CreatedById == _currentUserDataService.CurrentUserData.Id
            && i.EventId == query.EventId && (!query.Id.HasValue || i.Id == query.Id)));


            return new GetEventTimingItemsQueryResult
            {
                TimingItems = timingItems.Select(t =>  new EventTimingItemDto
                {
                    Id = t.Id,
                    EventId = t.EventId,
                    Artist = t.Artist,
                    Start = t.Start,
                    Duration = t.Duration,
                    Comments = t.Comments
                }).ToList()
            };
        }
    }
}
