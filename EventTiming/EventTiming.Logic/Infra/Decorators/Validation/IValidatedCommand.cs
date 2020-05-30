
namespace EventTiming.Logic.Infra.Decorators.Validation
{
    public interface IValidatedCommand
    {
        ValidationResult ValidationResult { get; set; }

    }
}
