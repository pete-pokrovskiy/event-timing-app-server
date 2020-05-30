using EventTiming.Logic.Contract.Infra;
using EventTiming.Logic.Contract.Infra.Decorators;
using System.Threading.Tasks;

namespace EventTiming.Logic.Infra.Decorators.BackgroundTasks
{
    public abstract class BackgroundTasksProcessor<TCommand> : IBackgroundTasksProcessor<TCommand> where TCommand : ICommand
    {
        //public IBackgroundTaskQueue Queue { get; }

        //public BackgroundTasksProcessor(IBackgroundTaskQueue queue)
        //{
        //    Queue = queue;
        //}

        //public abstract void RunTasks(TCommand command);
        public async Task RunTasks(TCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}
