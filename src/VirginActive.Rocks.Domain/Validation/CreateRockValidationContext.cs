namespace VirginActive.Rocks.Domain.Validation
{
    public sealed record CreateRockValidationContext(
      string Title,
      DateOnly DueDate,
      string? Note);
}
