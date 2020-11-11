using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Query
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeletedMetadataFilters
    {
        Default = 1,
        Attribute = 2,
        Relationship = 4,
        Label = 8,
        OptionSet = 16,
        All = 31
    }
}