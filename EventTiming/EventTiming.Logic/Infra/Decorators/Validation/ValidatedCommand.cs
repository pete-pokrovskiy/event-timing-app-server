
using EventTiming.Logic.Contract.Infra;

namespace EventTiming.Logic.Infra.Decorators.Validation
{
    public abstract class ValidatedCommand : Command, IValidatedCommand
    {
        public ValidationResult ValidationResult { get; set; } = new ValidationResult();
    }
}
