namespace VirginActive.Rocks.Domain.Exceptions
{
    public sealed class RockNotFoundException(Guid rockId) : Exception($"Rock '{rockId}' was not found.")
    {
    }
}
