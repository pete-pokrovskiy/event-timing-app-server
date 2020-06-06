using EventTiming.Data;
using EventTiming.Domain;
using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Infra;
using EventTiming.Logic.Services.Auth;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventTiming.Logic.Events.Commands
{
    public class CreateTimingItemCommandHandler : CommandHandler<CreateTimingItemCommand>
    {
        public CreateTimingItemCommandHandler(IUow uow, ICurrentUserDataService currentUserDataService) : base(uow, currentUserDataService)
        {
        }

        public override async Task Execute(CreateTimingItemCommand command)
        {

            var eventItem = (await _uow.EventRepository.FindBy(e => e.CreatedById == _currentUserDataService.CurrentUserData.Id
                && e.Id == command.EventId)).FirstOrDefault();

            if (eventItem == null)
            {
                throw new Exception($"Не найдено события с идентификатором {command.EventId}");
            }

            var lastEventTimingItemOrder = (await _uow.EventTimingItemRepository.FindBy(et => et.EventId == eventItem.Id)).OrderBy(et => et.Order)
                .LastOrDefault()?.Order;


            command.TimingItem.Id = Guid.NewGuid();

            var timingItem = new EventTimingItem
            {
                Id = command.TimingItem.Id,
                EventId = command.EventId,
                Artist = command.TimingItem.Artist,
                Start = command.TimingItem.Start,
                Duration = command.TimingItem.Duration,
                Comments = command.TimingItem.Comments,
                CreatedById = _currentUserDataService.CurrentUserData.Id,
                ModifiedById = _currentUserDataService.CurrentUserData.Id,
                Order = lastEventTimingItemOrder.HasValue ? (lastEventTimingItemOrder.Value + 1) : 1
            };

            _uow.EventTimingItemRepository.Create(timingItem);
            await _uow.Commit();
        }
    }
}
