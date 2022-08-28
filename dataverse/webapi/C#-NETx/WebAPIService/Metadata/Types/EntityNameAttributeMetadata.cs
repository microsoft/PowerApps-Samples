using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EntityNameAttributeMetadata : EnumAttributeMetadata
    {

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.EntityNameAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.EntityName;
        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.EntityNameType);
        public bool IsEntityReferenceStored { get; set; }
    }
}