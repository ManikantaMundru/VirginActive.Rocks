using VirginActive.Rocks.Domain.Common;
using VirginActive.Rocks.Domain.Enums;
using VirginActive.Rocks.Domain.Exceptions;

namespace VirginActive.Rocks.Domain.Validation.Strategies
{
    public sealed class RevenueRockValidationStrategy(TimeProvider timeProvider) : IRockValidationStrategy
    {
        public RockCategory Category => RockCategory.Revenue;

        public void Validate(CreateRockValidationContext context)
        {
            // Calculate the current calendar quarter dynamically so the rule remains valid across years.
            var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
            var (quarterStart, quarterEnd) = today.GetQuarterRange();

            if (context.DueDate < quarterStart || context.DueDate > quarterEnd)
            {
                throw new RockValidationException(
                    "dueDate",
                    $"Revenue Rock due date must fall within the current quarter " +
                    $"({quarterStart:yyyy-MM-dd} to {quarterEnd:yyyy-MM-dd}).");
            }
        }
    }
}
