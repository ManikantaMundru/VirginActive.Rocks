using VirginActive.Rocks.Domain.Enums;
using VirginActive.Rocks.Domain.Exceptions;

namespace VirginActive.Rocks.Domain.Validation.Strategies
{
    public sealed class HealthRockValidationStrategy : IRockValidationStrategy
    {
        public RockCategory Category => RockCategory.Health;

        public void Validate(CreateRockValidationContext context)
        {
            if (context.Title.Trim().Length < 10)
            {
                throw new RockValidationException(
                    "title",
                    "Health Rock title must be at least 10 characters.");
            }
        }
    }
}
