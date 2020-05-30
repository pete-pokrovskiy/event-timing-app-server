
using System.Threading.Tasks;

namespace EventTiming.Logic.Contract.Infra.Decorators
{
    public interface IBackgroundTasksProcessor<TCommand> where TCommand : ICommand
    {
        Task RunTasks(TCommand command);
    }
}
