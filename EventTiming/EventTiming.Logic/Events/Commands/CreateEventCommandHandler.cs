using EventTiming.Data;
using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Infra;
using System;
using System.Threading.Tasks;

namespace EventTiming.Logic.Events.Commands
{
    public class CreateEventCommandHandler : CommandHandler<CreateEventCommand>
    {
        public CreateEventCommandHandler(IUow uow) : base(uow)
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
                StartDate = command.StartDateAndTime
            });

            await _uow.CommitAsync();
        }
    }
}
