using Moq;
using VirginActive.Rocks.Application.Abstractions;
using VirginActive.Rocks.Application.Rocks;
using VirginActive.Rocks.Application.Rocks.Commands;
using VirginActive.Rocks.Domain.Entities;
using VirginActive.Rocks.Domain.Enums;
using VirginActive.Rocks.Domain.Exceptions;
using VirginActive.Rocks.Domain.Validation;
using VirginActive.Rocks.Domain.Validation.Strategies;
using VirginActive.Rocks.UnitTests.Domain;

namespace VirginActive.Rocks.UnitTests.Application
{
    public sealed class RockServiceTests
    {
        private const string MemberId = "test-member";

        private readonly Mock<IRockRepository> _mockRockRepository = new();
        private readonly Mock<IRockValidationStrategyResolver> _mockRockValidationStrategyResolver = new();
        private readonly Mock<IRockValidationStrategy> _mockRockValidationStrategy = new();
        private readonly TestTimeProvider _timeProvider =
            new(new DateTimeOffset(2026, 8, 23, 10, 0, 0, TimeSpan.Zero));

        private readonly RockService _service;

        public RockServiceTests()
        {
            _mockRockValidationStrategyResolver
                .Setup(x => x.Resolve(It.IsAny<RockCategory>()))
                .Returns(_mockRockValidationStrategy.Object);

            _service = new RockService(
                _mockRockRepository.Object,
                _mockRockValidationStrategyResolver.Object,
                _timeProvider);
        }

        [Fact]
        public async Task CreateAsync_WhenRequestIsValid_ShouldCreateRock()
        {
            // Arrange
            var command = CreateCommand();

            _mockRockRepository
                .Setup(x => x.AddAsync(It.IsAny<Rock>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Rock rock, CancellationToken _) => rock);

            // Act
            var result = await _service.CreateAsync(command, CancellationToken.None);

            // Assert
            Assert.Equal(command.MemberId, result.MemberId);
            Assert.Equal(command.Title, result.Title);
            Assert.Equal(RockStatus.Pending, result.Status);

            _mockRockValidationStrategyResolver.Verify(
                x => x.Resolve(command.Category), Times.Once);

            _mockRockValidationStrategy.Verify(
                x => x.Validate(It.IsAny<CreateRockValidationContext>()), Times.Once);

            _mockRockRepository.Verify(
                x => x.AddAsync(It.IsAny<Rock>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenMemberIdIsEmpty_ShouldThrowRockValidationException()
        {
            // Arrange
            var command = CreateCommand(memberId: " ");

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RockValidationException>(
                () => _service.CreateAsync(command, CancellationToken.None));

            Assert.Contains("memberId", ex.Errors.Keys);

            _mockRockRepository.Verify(
                x => x.AddAsync(It.IsAny<Rock>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenRockDoesNotExist_ShouldThrowRockNotFoundException()
        {
            // Arrange
            var rockId = Guid.NewGuid();

            _mockRockRepository
                .Setup(x => x.GetAsync(MemberId, rockId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Rock?)null);

            var command = new UpdateRockStatusCommand(MemberId, rockId, RockStatus.Completed);

            // Act & Assert
            await Assert.ThrowsAsync<RockNotFoundException>(
                () => _service.UpdateStatusAsync(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenTransitionIsValid_ShouldUpdateRock()
        {
            // Arrange
            var rock = CreateRock(MemberId);

            _mockRockRepository
                .Setup(x => x.GetAsync(MemberId, rock.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rock);

            var command = new UpdateRockStatusCommand(MemberId, rock.Id, RockStatus.Completed);

            // Act
            var result = await _service.UpdateStatusAsync(command, CancellationToken.None);

            // Assert
            Assert.Equal(RockStatus.Completed, result.Status);

            _mockRockRepository.Verify(
                x => x.UpdateAsync(rock, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenTransitionIsInvalid_ShouldThrowInvalidRockStateTransitionException()
        {
            // Arrange
            var rock = CreateRock(MemberId);
            rock.ChangeStatus(RockStatus.Completed);

            _mockRockRepository
                .Setup(x => x.GetAsync(MemberId, rock.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rock);

            var command = new UpdateRockStatusCommand(MemberId, rock.Id, RockStatus.Missed);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidRockStateTransitionException>(
                () => _service.UpdateStatusAsync(command, CancellationToken.None));

            _mockRockRepository.Verify(
                x => x.UpdateAsync(rock, It.IsAny<CancellationToken>()), Times.Never);
        }

        private static CreateRockCommand CreateCommand(
            string memberId = MemberId,
            string title = "Complete ai course",
            RockCategory category = RockCategory.Career,
            DateOnly? dueDate = null,
            string? note = "Important for career progression.") =>
            new(
                memberId,
                title,
                category,
                dueDate ?? new DateOnly(2026, 8, 31),
                note);

        private static Rock CreateRock(
            string memberId,
            RockCategory category = RockCategory.Other,
            DateOnly? dueDate = null) =>
            new(
                Guid.NewGuid(),
                memberId,
                "test",
                category,
                dueDate ?? new DateOnly(2026, 8, 31),
                null,
                new DateTimeOffset(2026, 8, 23, 10, 0, 0, TimeSpan.Zero));
    }
}
