using EventTiming.Data;
using EventTiming.Logic.Contract.Infra;
using EventTiming.Logic.Services.Auth;
using System.Threading.Tasks;

namespace EventTiming.Logic.Infra
{
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
            where TCommand : ICommand
    {
        protected readonly IUow _uow;
        protected readonly ICurrentUserDataService _currentUserDataService;

        public CommandHandler(IUow uow, ICurrentUserDataService currentUserDataService)
        {
            _uow = uow;
           _currentUserDataService = currentUserDataService;
        }

        public abstract Task Execute(TCommand command);
    }
}
