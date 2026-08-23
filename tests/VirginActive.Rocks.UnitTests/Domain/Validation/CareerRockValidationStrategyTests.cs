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
            var context = new CreateRockValidationContext(
                "Complete AI course",
                new DateOnly(2026, 8, 30),
                "This will improve my ai knowledge.");

            var exception = Record.Exception(() => _strategy.Validate(context));

            Assert.Null(exception);
        }

        [Fact]
        public void Validate_WhenNoteIsMissing_ShouldThrow()
        {
            var context = new CreateRockValidationContext(
                "Complete Azure certification",
                new DateOnly(2026, 8, 30),
                null);

            var exception = Assert.Throws<RockValidationException>(() => _strategy.Validate(context));

            Assert.Contains("note", exception.Errors.Keys);
        }
    }
}
