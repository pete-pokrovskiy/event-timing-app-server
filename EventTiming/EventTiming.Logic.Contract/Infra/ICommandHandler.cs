using System.Threading.Tasks;

namespace EventTiming.Logic.Contract.Infra
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task Execute(TCommand command);
    }
}
