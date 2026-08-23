using VirginActive.Rocks.Domain.Enums;
using VirginActive.Rocks.Domain.Exceptions;

namespace VirginActive.Rocks.Domain.Validation.Strategies
{
    public sealed class RockValidationStrategyResolver(IEnumerable<IRockValidationStrategy> strategies) : IRockValidationStrategyResolver
    {
        private readonly IReadOnlyDictionary<RockCategory, IRockValidationStrategy> _strategies = strategies.ToDictionary(
                strategy => strategy.Category);

        public IRockValidationStrategy Resolve(RockCategory category)
        {
            if (_strategies.TryGetValue(category, out var strategy))
            {
                return strategy;
            }

            throw new RockValidationException(
                "category",
                $"Unsupported Rock category '{category}'.");
        }
    }
}
