using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class BooleanOptionSetMetadata : OptionSetMetadataBase
    {
        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.BooleanOptionSetMetadata";
        public OptionMetadata TrueOption { get; set; }
        public OptionMetadata FalseOption { get; set; }
        new public OptionSetType OptionSetType { get; } = OptionSetType.Boolean;
    }
}