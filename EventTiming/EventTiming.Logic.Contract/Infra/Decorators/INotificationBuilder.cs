

using System.Threading.Tasks;

namespace EventTiming.Logic.Contract.Infra.Decorators
{
    public interface INotificationBuilder<TCommand> where TCommand : ICommand
    {
        Task Build(TCommand command);
    }
}
