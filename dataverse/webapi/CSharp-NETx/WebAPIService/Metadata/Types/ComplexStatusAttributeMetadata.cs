using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexStatusAttributeMetadata : ComplexEnumAttributeMetadata
    {
        [JsonProperty("@odata.type")]
        public new string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexStatusAttributeMetadata";

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.StatusType);

    }
}
