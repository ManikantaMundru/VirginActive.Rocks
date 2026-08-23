using VirginActive.Rocks.Domain.Enums;

namespace VirginActive.Rocks.Domain.Exceptions
{
    public sealed class InvalidRockStateTransitionException : Exception
    {
        public RockStatus CurrentStatus { get; }

        public RockStatus RequestedStatus { get; }

        public InvalidRockStateTransitionException(RockStatus currentStatus, RockStatus requestedStatus)
            : base($"Rock status cannot transition from " + $"'{currentStatus}' to '{requestedStatus}'.")
        {
            CurrentStatus = currentStatus;
            RequestedStatus = requestedStatus;
        }
    }
}
