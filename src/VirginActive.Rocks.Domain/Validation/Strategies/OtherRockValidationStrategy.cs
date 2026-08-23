using VirginActive.Rocks.Domain.Enums;

namespace VirginActive.Rocks.Domain.Validation.Strategies
{
    public sealed class OtherRockValidationStrategy : IRockValidationStrategy
    {
        public RockCategory Category => RockCategory.Other;

        public void Validate(CreateRockValidationContext context)
        {
        }
    }
}
