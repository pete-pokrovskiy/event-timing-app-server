using EventTiming.Data;
using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Infra;
using EventTiming.Logic.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.Logic.Events.Commands
{
    public class UpdateEventCommandHandler : CommandHandler<UpdateEventCommand>
    {
        public UpdateEventCommandHandler(IUow uow, ICurrentUserDataService currentUserDataService) : base(uow, currentUserDataService)
        {
        }

        public override async Task Execute(UpdateEventCommand command)
        {
            var eventItem = (await _uow.EventRepository.FindByWithTracking(e => e.Id == command.EventId))
                .FirstOrDefault();

            if (eventItem == null)
            {
                throw new Exception($"Не найдено событие с идентификатором {command.Event.Id}");
            }

            eventItem.ModifiedById = _currentUserDataService.CurrentUserData.Id;
            eventItem.Name = command.Event.Name;
            eventItem.StartDate = command.Event.StartDateAndTime;
            eventItem.Description = command.Event.Description;

            _uow.EventRepository.Update(eventItem);
            await _uow.Commit();
        }
    }
}
