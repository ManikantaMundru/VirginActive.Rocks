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
            // Arrange
            var strategies = new IRockValidationStrategy[]
            {
                new HealthRockValidationStrategy(),
                new CareerRockValidationStrategy(),
                new OtherRockValidationStrategy()
            };

            var resolver = new RockValidationStrategyResolver(strategies);

            // Act
            var result = resolver.Resolve(RockCategory.Health);

            // Assert
            Assert.IsType<HealthRockValidationStrategy>(result);
        }

        [Fact]
        public void Resolve_WhenStrategyIsNotRegistered_ShouldThrowRockValidationException()
        {
            // Arrange
            var strategies = new IRockValidationStrategy[]
            {
                new HealthRockValidationStrategy()
            };

            var resolver = new RockValidationStrategyResolver(strategies);

            // Act
            var exception = Assert.Throws<RockValidationException>(() => resolver.Resolve(RockCategory.Revenue));

            // Assert
            Assert.Contains("category", exception.Errors.Keys);
        }
    }
}