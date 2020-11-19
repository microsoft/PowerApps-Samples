using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class BooleanOptionSetMetadata : OptionSetMetadataBase
    {
        public OptionMetadata TrueOption { get; set; }
        public OptionMetadata FalseOption { get; set; }
        new public OptionSetType OptionSetType { get; } = OptionSetType.Boolean;
    }
}