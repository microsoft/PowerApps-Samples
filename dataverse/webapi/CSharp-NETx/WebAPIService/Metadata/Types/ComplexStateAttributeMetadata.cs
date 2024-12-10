using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexStateAttributeMetadata : ComplexEnumAttributeMetadata
    {
        [JsonProperty("@odata.type")]
        public new string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexStateAttributeMetadata";

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.StateType);

    }
}
