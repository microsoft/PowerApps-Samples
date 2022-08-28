using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ManagedPropertyEvaluationPriority
    {
        None, Low, Normal, High, Essential
    }
}