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
            // Arrange
            var context = new CreateRockValidationContext("test title", new DateOnly(2026, 8, 30), null);

            // Act
            var exception = Record.Exception(() => _strategy.Validate(context));

            // Assert
            Assert.Null(exception);
        }
    }
}
