using VirginActive.Rocks.Domain.Exceptions;
using VirginActive.Rocks.Domain.Validation;
using VirginActive.Rocks.Domain.Validation.Strategies;

namespace VirginActive.Rocks.UnitTests.Domain.Validation
{
    public sealed class RevenueRockValidationStrategyTests
    {
        private const string ValidTitle = "Increase monthly recurring revenue";

        private readonly RevenueRockValidationStrategy _strategy;

        public RevenueRockValidationStrategyTests()
        {
            var timeProvider = new TestTimeProvider(new DateTimeOffset(2026, 8, 15, 10, 0, 0, TimeSpan.Zero));

            _strategy = new RevenueRockValidationStrategy(timeProvider);
        }

        [Fact]
        public void Validate_WhenDueDateIsWithinCurrentQuarter_ShouldNotThrow()
        {
            // Arrange
            var context = new CreateRockValidationContext(ValidTitle, new DateOnly(2026, 9, 15), null);

            // Act
            var exception = Record.Exception(() => _strategy.Validate(context));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Validate_WhenDueDateIsAfterCurrentQuarter_ShouldThrow()
        {
            // Arrange
            var context = new CreateRockValidationContext(ValidTitle, new DateOnly(2026, 10, 1), null);

            // Act
            var exception = Assert.Throws<RockValidationException>(() => _strategy.Validate(context));

            // Assert
            Assert.Contains("dueDate", exception.Errors.Keys);
        }

        [Fact]
        public void Validate_WhenDueDateIsOnQuarterEnd_ShouldNotThrow()
        {
            // Arrange
            var context = new CreateRockValidationContext(ValidTitle, new DateOnly(2026, 9, 30), null);

            // Act
            var exception = Record.Exception(() => _strategy.Validate(context));

            // Assert
            Assert.Null(exception);
        }
    }
}
