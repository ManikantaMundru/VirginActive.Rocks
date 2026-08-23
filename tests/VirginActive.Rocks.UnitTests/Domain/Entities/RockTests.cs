using VirginActive.Rocks.Domain.Entities;
using VirginActive.Rocks.Domain.Enums;
using VirginActive.Rocks.Domain.Exceptions;

namespace VirginActive.Rocks.UnitTests.Domain.Entities
{
    public sealed class RockTests
    {
        [Fact]
        public void ChangeStatus_WhenPendingToCompleted_ShouldUpdateStatus()
        {
            // Arrange
            var rock = CreateRock();

            // Act
            rock.ChangeStatus(RockStatus.Completed);

            // Assert
            Assert.Equal(RockStatus.Completed, rock.Status);
        }

        [Fact]
        public void ChangeStatus_WhenPendingToMissed_ShouldUpdateStatus()
        {
            // Arrange
            var rock = CreateRock();

            // Act
            rock.ChangeStatus(RockStatus.Missed);

            // Assert
            Assert.Equal(RockStatus.Missed, rock.Status);
        }

        [Fact]
        public void ChangeStatus_WhenPendingToPending_ShouldThrowInvalidTransitionException()
        {
            // Arrange
            var rock = CreateRock();

            // Act
            var exception = Assert.Throws<InvalidRockStateTransitionException>(() => rock.ChangeStatus(RockStatus.Pending));

            // Assert
            Assert.Equal(RockStatus.Pending, exception.CurrentStatus);
            Assert.Equal(RockStatus.Pending, exception.RequestedStatus);
        }

        [Theory]
        [InlineData(RockStatus.Completed, RockStatus.Missed)]
        [InlineData(RockStatus.Completed, RockStatus.Pending)]
        [InlineData(RockStatus.Missed, RockStatus.Completed)]
        [InlineData(RockStatus.Missed, RockStatus.Pending)]
        public void ChangeStatus_WhenRockIsNoLongerPending_ShouldThrowInvalidTransitionException(
            RockStatus currentStatus,
            RockStatus requestedStatus)
        {
            // Arrange
            var rock = CreateRock();
            rock.ChangeStatus(currentStatus);

            // Act
            var exception = Assert.Throws<InvalidRockStateTransitionException>(() => rock.ChangeStatus(requestedStatus));

            // Assert
            Assert.Equal(currentStatus, exception.CurrentStatus);
            Assert.Equal(requestedStatus, exception.RequestedStatus);
        }

        [Fact]
        public void Constructor_WhenTitleIsEmpty_ShouldThrowRockValidationException()
        {
            // Arrange
            var title = string.Empty;

            // Act
            var exception = Assert.Throws<RockValidationException>(
                () => new Rock(
                    Guid.NewGuid(),
                    "test_member_1",
                    title,
                    RockCategory.Career,
                    new DateOnly(2026, 8, 31),
                    "This will improve my ai knowledge",
                    new DateTimeOffset(
                        2026, 8, 23,
                        10, 0, 0,
                        TimeSpan.Zero)));

            // Assert
            Assert.Contains("title", exception.Errors.Keys);
        }

        private static Rock CreateRock()
        {
            return new Rock(
                Guid.NewGuid(),
                "test_member_1",
                "Complete ai course",
                RockCategory.Career,
                new DateOnly(2026, 8, 31),
                "This will improve my ai knowledge",
                new DateTimeOffset(
                        2026, 8, 23,
                        10, 0, 0,
                        TimeSpan.Zero));
        }
    }
}
