using VirginActive.Rocks.Domain.Enums;
using VirginActive.Rocks.Domain.Exceptions;

namespace VirginActive.Rocks.Domain.Entities
{
    public sealed class Rock
    {
        public Guid Id { get; }
        public string MemberId { get; }
        public string Title { get; }
        public RockCategory Category { get; }
        public DateOnly DueDate { get; }
        public string? Note { get; }
        public RockStatus Status { get; private set; } = RockStatus.Pending;
        public DateTimeOffset CreatedAtUtc { get; }

        public Rock(
            Guid id,
            string memberId,
            string title,
            RockCategory category,
            DateOnly dueDate,
            string? note,
            DateTimeOffset createdAtUtc)
        {
            if (id == Guid.Empty)
            {
                throw new RockValidationException(nameof(id), "Rock Id is required.");
            }

            if (string.IsNullOrWhiteSpace(memberId))
            {
                throw new RockValidationException(nameof(memberId), "MemberId is required.");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new RockValidationException(nameof(title), "Title is required.");
            }

            Id = id;
            MemberId = memberId;
            Title = title;
            Category = category;
            DueDate = dueDate;
            Note = note;
            CreatedAtUtc = createdAtUtc;
        }

        public void ChangeStatus(RockStatus newStatus)
        {
            // Once a Rock leaves Pending it becomes terminal and cannot transition again.
            if (Status != RockStatus.Pending)
            {
                throw new InvalidRockStateTransitionException(
                    Status,
                    newStatus);
            }

            // The only valid transitions are Pending -> Completed and Pending -> Missed.
            if (newStatus is not RockStatus.Completed
                and not RockStatus.Missed)
            {
                throw new InvalidRockStateTransitionException(
                    Status,
                    newStatus);
            }

            Status = newStatus;
        }
    }
}
