using System.Text.Json;
using System.Text.Json.Serialization;

namespace VirginActive.Rocks.IntegrationTests.Helpers
{
    internal static class TestJsonOptions
    {
        public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
    }
}
