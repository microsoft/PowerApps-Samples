using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    [Flags]
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

