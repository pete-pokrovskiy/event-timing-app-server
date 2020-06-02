using EventTiming.Data;
using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Infra;
using EventTiming.Logic.Services.Auth;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventTiming.Logic.Events.Commands
{
    public class DeleteEventCommandHandler : CommandHandler<DeleteEventCommand>
    {
        public DeleteEventCommandHandler(IUow uow, ICurrentUserDataService currentUserDataService) : base(uow, currentUserDataService)
        {
        }

        public override  async Task Execute(DeleteEventCommand command)
        {
            var eventItem = (await _uow.EventRepository.FindBy(e => e.CreatedById == _currentUserDataService.CurrentUserData.Id
                && e.Id == command.EventId)).FirstOrDefault();

            if (eventItem == null)
            {
                throw new Exception($"Не найдено события с идентификатором {command.EventId}");
            }

            var timingItems = (await _uow.EventTimingItemRepository.FindBy(i => i.CreatedById == _currentUserDataService.CurrentUserData.Id
            && i.EventId == eventItem.Id));

            if(timingItems.Any())
            {
                _uow.EventTimingItemRepository.Delete(timingItems);
            }

            _uow.EventRepository.Delete(eventItem);

            await _uow.Commit();
        }
    }
}
