using EventTiming.Logic.Contract.Infra;
using EventTiming.Logic.Contract.Infra.Decorators;
using System;
using System.Threading.Tasks;

namespace EventTiming.Logic.Infra.Decorators.Notifications
{
    public class NotificationsDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _decoratedCommandHandler;
        private readonly INotificationBuilder<TCommand> _notificationBuilder;
        public NotificationsDecorator(ICommandHandler<TCommand> decoratedCommandHandler,
            INotificationBuilder<TCommand> notificationBuilder)
        {
            _decoratedCommandHandler = decoratedCommandHandler;
            _notificationBuilder = notificationBuilder;
        }

        public async Task Execute(TCommand command)
        {
           await  _decoratedCommandHandler.Execute(command);

            try
            {
               await _notificationBuilder.Build(command);
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "Ошибка в декораторе NotificationsDecorator. Команда {@command}", nameof(TCommand));
            }
        }
    }
}
