using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexPicklistAttributeMetadata : ComplexEnumAttributeMetadata
    {
        [JsonProperty("@odata.type")]
        public new string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexPicklistAttributeMetadata";

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.PicklistType);

        public string? FormulaDefinition { get; set; }

        public int SourceTypeMask { get; set; }

        public string? ParentPicklistLogicalName { get; set; }

        public List<string> ChildPicklistLogicalNames { get; set; } = new List<string>();
    }
}
