using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexEntityNameAttributeMetadata : ComplexEnumAttributeMetadata
    {
        [JsonProperty("@odata.type")]
        public new string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexEntityNameAttributeMetadata";

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.EntityNameType);

        public bool IsEntityReferenceStored { get; set; }
    }
}
