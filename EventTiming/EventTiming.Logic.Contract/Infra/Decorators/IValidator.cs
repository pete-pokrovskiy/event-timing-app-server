using System.Threading.Tasks;

namespace EventTiming.Logic.Contract.Infra.Decorators
{
    public interface IValidator<TCommand> where TCommand : ICommand
    {
        Task Validate(TCommand command);
    }
}
