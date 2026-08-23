using VirginActive.Rocks.Domain.Enums;
using VirginActive.Rocks.Domain.Exceptions;
using VirginActive.Rocks.Domain.Validation.Strategies;

namespace VirginActive.Rocks.UnitTests.Domain.Validation
{
    public sealed class RockValidationStrategyResolverTests
    {
        [Fact]
        public void Resolve_WhenCategoryExists_ShouldReturnCorrectStrategy()
        {
            var strategies = new IRockValidationStrategy[]
            {
                new HealthRockValidationStrategy(),
                new CareerRockValidationStrategy(),
                new OtherRockValidationStrategy()
            };

            var resolver = new RockValidationStrategyResolver(strategies);

            var result = resolver.Resolve(RockCategory.Health);

            Assert.IsType<HealthRockValidationStrategy>(result);
        }

        [Fact]
        public void Resolve_WhenStrategyIsNotRegistered_ShouldThrowRockValidationException()
        {
            var strategies = new IRockValidationStrategy[]
            {
                new HealthRockValidationStrategy()
            };

            var resolver = new RockValidationStrategyResolver(strategies);

            var exception = Assert.Throws<RockValidationException>(() => resolver.Resolve(RockCategory.Revenue));

            Assert.Contains("category", exception.Errors.Keys);
        }
    }
}
