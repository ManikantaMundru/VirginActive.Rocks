using VirginActive.Rocks.Domain.Enums;

namespace VirginActive.Rocks.Domain.Validation.Strategies
{
    public interface IRockValidationStrategyResolver
    {
        IRockValidationStrategy Resolve(RockCategory category);
    }
}
