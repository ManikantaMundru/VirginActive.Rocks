namespace VirginActive.Rocks.Domain.Exceptions
{
    public sealed class RockValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public RockValidationException(string field, string message)
            : base(message)
        {
            Errors = new Dictionary<string, string[]>
            {
                [field] = [message]
            };
        }

        public RockValidationException(IReadOnlyDictionary<string, string[]> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }
}
