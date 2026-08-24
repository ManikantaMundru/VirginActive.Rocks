using VirginActive.Rocks.Domain.Exceptions;
using VirginActive.Rocks.Domain.Validation;
using VirginActive.Rocks.Domain.Validation.Strategies;

namespace VirginActive.Rocks.UnitTests.Domain.Validation
{
    public sealed class HealthRockValidationStrategyTests
    {
        private readonly HealthRockValidationStrategy _strategy = new();

        [Fact]
        public void Validate_WhenTitleHasAtLeastTenCharacters_ShouldNotThrow()
        {
            // Arrange
            var context = new CreateRockValidationContext("1234567890", new DateOnly(2026, 8, 30), null);

            // Act
            var exception = Record.Exception(() => _strategy.Validate(context));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Validate_WhenTitleHasLessThanTenCharacters_ShouldThrow()
        {
            // Arrange
            var context = new CreateRockValidationContext("Exercise", new DateOnly(2026, 8, 30), null);

            // Act
            var exception = Assert.Throws<RockValidationException>(() => _strategy.Validate(context));

            // Assert
            Assert.Contains("title", exception.Errors.Keys);
        }
    }
}