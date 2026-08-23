using VirginActive.Rocks.Domain.Enums;

namespace VirginActive.Rocks.Domain.Validation.Strategies
{
    public interface IRockValidationStrategy
    {
        RockCategory Category { get; }

        void Validate(CreateRockValidationContext context);
    }
}
