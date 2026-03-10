using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class StatusAttributeMetadata : EnumAttributeMetadata
    {

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.StatusAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Status;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.StatusType);
    }
}