using VirginActive.Rocks.Domain.Validation;
using VirginActive.Rocks.Domain.Validation.Strategies;

namespace VirginActive.Rocks.UnitTests.Domain.Validation
{
    public sealed class OtherRockValidationStrategyTests
    {
        private readonly OtherRockValidationStrategy _strategy = new();

        [Fact]
        public void Validate_ShouldNotApplyAnyAdditionalValidation()
        {
            var context = new CreateRockValidationContext(
                "test title",
                new DateOnly(2026, 8, 30),
                null);

            var exception = Record.Exception(() => _strategy.Validate(context));

            Assert.Null(exception);
        }
    }
}
