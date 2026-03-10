using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MultiSelectPicklistAttributeMetadata : EnumAttributeMetadata
    {

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.MultiSelectPicklistAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Virtual;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.MultiSelectPicklistType);
        public List<string> ChildPicklistLogicalNames { get; set; }
        public string FormulaDefinition { get; set; }
        public string ParentPicklistLogicalName { get; set; }
        public int SourceTypeMask { get; set; }
    }
}