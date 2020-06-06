using AutoMapper;
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
        private readonly IMapper _mapper;

        public GetEventTimingItemsQueryHandler(IUow uow, ICurrentUserDataService currentUserDataService,
            IMapper mapper) : base(uow, currentUserDataService)
        {
            _mapper = mapper;
        }

        public override async Task<GetEventTimingItemsQueryResult> Execute(GetEventTimingItemsQuery query)
        {
            var timingItems = (await _uow.EventTimingItemRepository.FindBy(i => i.CreatedById == _currentUserDataService.CurrentUserData.Id
            && i.EventId == query.EventId && (!query.Id.HasValue || i.Id == query.Id))).ToList();

            return new GetEventTimingItemsQueryResult
            {
                TimingItems = (_mapper.Map<List<EventTimingItemDto>>(timingItems)).OrderBy(et => et.Order)
            };
        }
    }
}
