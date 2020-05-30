using EventTiming.Logic.Contract.Infra;
using EventTiming.Logic.Contract.Infra.Decorators;
using System.Threading.Tasks;

namespace EventTiming.Logic.Infra.Decorators.Validation
{
    public class ValidationDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand, IValidatedCommand
    {
        private readonly ICommandHandler<TCommand> _decoratedCommandHandler;
        private readonly IValidator<TCommand> _validator;

        public ValidationDecorator(ICommandHandler<TCommand> decoratedCommandHandler, IValidator<TCommand> validator )
        {
            _decoratedCommandHandler = decoratedCommandHandler;
            _validator = validator;
        }

        public async Task Execute(TCommand command)
        {
            await _validator.Validate(command);

            if (command.ValidationResult.IsValid)
            {
                await _decoratedCommandHandler.Execute(command);
            }
        }
    }
}
