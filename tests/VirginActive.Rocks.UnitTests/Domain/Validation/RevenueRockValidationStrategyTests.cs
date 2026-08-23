using VirginActive.Rocks.Domain.Exceptions;
using VirginActive.Rocks.Domain.Validation;
using VirginActive.Rocks.Domain.Validation.Strategies;

namespace VirginActive.Rocks.UnitTests.Domain.Validation
{
    public sealed class RevenueRockValidationStrategyTests
    {
        private readonly RevenueRockValidationStrategy _strategy;

        public RevenueRockValidationStrategyTests()
        {
            var timeProvider = new TestTimeProvider(
                new DateTimeOffset(
                    2026,
                    8,
                    15,
                    10,
                    0,
                    0,
                    TimeSpan.Zero));

            _strategy =
                new RevenueRockValidationStrategy(timeProvider);
        }

        [Fact]
        public void Validate_WhenDueDateIsWithinCurrentQuarter_ShouldNotThrow()
        {
            var context = new CreateRockValidationContext(
                "Increase monthly recurring revenue",
                new DateOnly(2026, 9, 15),
                null);

            var exception = Record.Exception(() => _strategy.Validate(context));

            Assert.Null(exception);
        }

        [Fact]
        public void Validate_WhenDueDateIsAfterCurrentQuarter_ShouldThrow()
        {
            var context = new CreateRockValidationContext(
                "Increase monthly recurring revenue",
                new DateOnly(2026, 10, 1),
                null);

            Assert.Throws<RockValidationException>(() => _strategy.Validate(context));
        }

        [Fact]
        public void Validate_WhenDueDateIsOnQuarterEnd_ShouldNotThrow()
        {
            var context = new CreateRockValidationContext(
                "Increase monthly recurring revenue",
                new DateOnly(2026, 9, 30),
                null);

            var exception = Record.Exception(() => _strategy.Validate(context));

            Assert.Null(exception);
        }
    }
}
