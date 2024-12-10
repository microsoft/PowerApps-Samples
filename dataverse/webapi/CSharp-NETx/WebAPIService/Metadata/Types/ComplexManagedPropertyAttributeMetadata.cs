using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexManagedPropertyAttributeMetadata : ComplexAttributeMetadata
    {
        public new AttributeTypeCode AttributeType { get; } = AttributeTypeCode.ManagedProperty;

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.ManagedPropertyType);

        public string ManagedPropertyLogicalName { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexManagedPropertyAttributeMetadata";
        public string ParentAttributeName { get; set; }
        public int ParentComponentType { get; set; }
        public AttributeTypeCode ValueAttributeTypeCode { get; set; }
    }
}