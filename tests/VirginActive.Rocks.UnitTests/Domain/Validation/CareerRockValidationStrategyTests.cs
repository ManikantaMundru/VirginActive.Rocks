using VirginActive.Rocks.Domain.Exceptions;
using VirginActive.Rocks.Domain.Validation;
using VirginActive.Rocks.Domain.Validation.Strategies;

namespace VirginActive.Rocks.UnitTests.Domain.Validation
{
    public sealed class CareerRockValidationStrategyTests
    {
        private readonly CareerRockValidationStrategy _strategy = new();

        [Fact]
        public void Validate_WhenNoteIsProvided_ShouldNotThrow()
        {
            // Arrange
            var context = new CreateRockValidationContext("Complete AI course", new DateOnly(2026, 8, 30), "This will improve my ai knowledge.");

            // Act
            var exception = Record.Exception(() => _strategy.Validate(context));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Validate_WhenNoteIsMissing_ShouldThrow()
        {
            // Arrange
            var context = new CreateRockValidationContext("Complete AI course", new DateOnly(2026, 8, 30), null);

            // Act
            var exception = Assert.Throws<RockValidationException>(() => _strategy.Validate(context));

            // Assert
            Assert.Contains("note", exception.Errors.Keys);
        }
    }
}
