using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class StateAttributeMetadata : EnumAttributeMetadata
    {
        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.StateAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.State;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.StateType);
    }
}