using VirginActive.Rocks.Domain.Enums;
using VirginActive.Rocks.Domain.Exceptions;

namespace VirginActive.Rocks.Domain.Validation.Strategies
{
    public sealed class CareerRockValidationStrategy : IRockValidationStrategy
    {
        public RockCategory Category => RockCategory.Career;

        public void Validate(CreateRockValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Note))
            {
                throw new RockValidationException(
                    "note",
                    "A note is required for Career Rocks explaining why this matters.");
            }
        }
    }
}
