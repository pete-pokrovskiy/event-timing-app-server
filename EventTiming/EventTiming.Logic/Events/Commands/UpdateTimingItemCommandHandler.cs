using AutoMapper;
using EventTiming.Data;
using EventTiming.Domain;
using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Infra;
using EventTiming.Logic.Services.Auth;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventTiming.Logic.Events.Commands
{
    public class UpdateTimingItemCommandHandler : CommandHandler<UpdateTimingItemCommand>
    {
        private readonly IMapper _mapper;

        public UpdateTimingItemCommandHandler(IUow uow, ICurrentUserDataService currentUserDataService,
            IMapper mapper
            ) : base(uow, currentUserDataService)
        {
            _mapper = mapper;
        }

        public override async Task Execute(UpdateTimingItemCommand command)
        {
            var timingItem = (await _uow.EventTimingItemRepository.FindByWithTracking(et => et.EventId == command.EventId &&
            et.Id == command.TimingItemId)).FirstOrDefault();

            if (timingItem == null)
            {
                throw new Exception($"Не найден элемент события с идентификатором {command.TimingItemId}");
            }

            timingItem = _mapper.Map<EventTimingItemDto, EventTimingItem>(command.TimingItem, timingItem);

            _uow.EventTimingItemRepository.Update(timingItem);

            await _uow.Commit();
        }
    }
}
