using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ManagedPropertyAttributeMetadata : AttributeMetadata
    {

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ManagedPropertyAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.ManagedProperty;
        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.ManagedPropertyType);
        public string ManagedPropertyLogicalName { get; set; }
        public int ParentComponentType { get; set; }
        public string ParentAttributeName { get; set; }
        public AttributeTypeCode ValueAttributeTypeCode { get; set; }
    }
}