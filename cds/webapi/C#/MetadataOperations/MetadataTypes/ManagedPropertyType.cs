using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ManagedPropertyType
    {
        Operation, Attribute, CustomEvaluator, Custom
    }
}