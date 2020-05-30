using EventTiming.Logic.Contract.Infra;
using System.Threading.Tasks;

namespace EventTiming.Logic.Infra.Decorators.BackgroundTasks
{
    public class BackgroundTasksDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _decoratedCommandHandler;
        //private readonly IBackgroundTasksProcessor<TCommand> _backgroundTasksProcessor;

        public BackgroundTasksDecorator(ICommandHandler<TCommand> decoratedCommandHandler)//, IBackgroundTasksProcessor<TCommand> backgroundTasksProcessor)
        {
            _decoratedCommandHandler = decoratedCommandHandler;
           // _backgroundTasksProcessor = backgroundTasksProcessor;
        }


        public async Task Execute(TCommand command)
        {
            await _decoratedCommandHandler.Execute(command);

            //после отработки основной команды запускаем все фоновые задания
            //_backgroundTasksProcessor.RunTasks(command);
        }
    }
}
