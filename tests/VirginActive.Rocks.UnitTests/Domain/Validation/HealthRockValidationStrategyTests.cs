using System;
using System.Collections.Generic;
using System.Text;
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
            var context = new CreateRockValidationContext(
                "1234567890",
                new DateOnly(2026, 8, 30),
                null);

            var exception = Record.Exception(
                () => _strategy.Validate(context));

            Assert.Null(exception);
        }

        [Fact]
        public void Validate_WhenTitleHasLessThanTenCharacters_ShouldThrow()
        {
            var context = new CreateRockValidationContext(
                "Exercise",
                new DateOnly(2026, 8, 30),
                null);

            var exception = Assert.Throws<RockValidationException>(
                () => _strategy.Validate(context));

            Assert.Contains("title", exception.Errors.Keys);
        }
    }
}
