using VirginActive.Rocks.Application.Abstractions;
using VirginActive.Rocks.Application.Rocks.Commands;
using VirginActive.Rocks.Application.Rocks.Mappings;
using VirginActive.Rocks.Application.Rocks.Models;
using VirginActive.Rocks.Application.Rocks.Queries;
using VirginActive.Rocks.Domain.Entities;
using VirginActive.Rocks.Domain.Exceptions;
using VirginActive.Rocks.Domain.Validation;
using VirginActive.Rocks.Domain.Validation.Strategies;

namespace VirginActive.Rocks.Application.Rocks
{
    public sealed class RockService(
       IRockRepository repository,
       IRockValidationStrategyResolver strategyResolver,
       TimeProvider timeProvider) : IRockService
    {
        public async Task<RockDto> CreateAsync(CreateRockCommand command, CancellationToken cancellationToken)
        {
            // Validate rules that apply to every Rock before applying category-specific business rules.
            ValidateCreateCommand(command);

            var validationContext = new CreateRockValidationContext(command.Title, command.DueDate, command.Note);

            // Resolve category-specific validation through the Strategy pattern to keep the service open for extension.
            var strategy = strategyResolver.Resolve(command.Category);

            strategy.Validate(validationContext);

            var rock = new Rock(
                Guid.NewGuid(),
                command.MemberId,
                command.Title,
                command.Category,
                command.DueDate,
                command.Note,
                timeProvider.GetUtcNow());

            await repository.AddAsync(rock, cancellationToken);

            return rock.ToDto();
        }

        public async Task<IReadOnlyCollection<RockDto>> GetByMemberAsync(GetMemberRocksQuery query, CancellationToken cancellationToken)
        {
            ValidateMemberId(query.MemberId);

            var rocks = await repository.GetByMemberIdAsync(query.MemberId.Trim(), cancellationToken);

            var filteredRocks = query.Status.HasValue
                ? rocks.Where(x => x.Status == query.Status.Value)
                : rocks;

            return filteredRocks.Select(x => x.ToDto()).ToArray();
        }

        public async Task<RockDto> UpdateStatusAsync(UpdateRockStatusCommand command, CancellationToken cancellationToken)
        {
            ValidateMemberId(command.MemberId);

            if (!Enum.IsDefined(command.Status))
            {
                throw new RockValidationException("status", "Status must be Pending, Completed or Missed.");
            }

            var rock = await repository.GetAsync(command.MemberId.Trim(), command.RockId, cancellationToken);

            if (rock is null)
            {
                throw new RockNotFoundException(command.RockId);
            }

            rock.ChangeStatus(command.Status);

            await repository.UpdateAsync(rock, cancellationToken);

            return rock.ToDto();
        }

        private void ValidateCreateCommand(CreateRockCommand command)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(command.MemberId))
            {
                errors["memberId"] = ["Member ID must not be empty."];
            }

            if (string.IsNullOrWhiteSpace(command.Title))
            {
                errors["title"] = ["Title must not be empty or whitespace."];
            }

            if (!Enum.IsDefined(command.Category))
            {
                errors["category"] = ["Category must be Revenue, Health, Career or Other."];
            }

            var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

            if (command.DueDate < today)
            {
                errors["dueDate"] = ["Due date must not be in the past."];
            }

            if (errors.Count > 0)
            {
                throw new RockValidationException(errors);
            }
        }

        private static void ValidateMemberId(string memberId)
        {
            if (string.IsNullOrWhiteSpace(memberId))
            {
                throw new RockValidationException("memberId", "Member ID must not be empty.");
            }
        }
    }
}
