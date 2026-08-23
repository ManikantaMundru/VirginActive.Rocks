namespace VirginActive.Rocks.Application.Exceptions
{
    public sealed class ProfileEnrichmentException(string message, Exception? innerException = null) : Exception(message, innerException)
    {
    }
}
