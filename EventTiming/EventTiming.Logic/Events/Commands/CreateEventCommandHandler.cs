using EventTiming.Data;
using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Infra;
using EventTiming.Logic.Services.Auth;
using System;
using System.Threading.Tasks;

namespace EventTiming.Logic.Events.Commands
{
    public class CreateEventCommandHandler : CommandHandler<CreateEventCommand>
    {
        public CreateEventCommandHandler(IUow uow, ICurrentUserDataService currentUserDataService) : base(uow, currentUserDataService)
        {
        }

        public override async Task Execute(CreateEventCommand command)
        {
            command.Id = Guid.NewGuid();

            _uow.EventRepository.Create(new Domain.Event
            {
                Id = command.Id.Value,
                Name = command.Name,
                Description = command.Description,
                StartDate = command.StartDateAndTime,
                CreatedById = _currentUserDataService.CurrentUserData.Id,
                ModifiedById = _currentUserDataService.CurrentUserData.Id
            });

            await _uow.Commit();
        }
    }
}
