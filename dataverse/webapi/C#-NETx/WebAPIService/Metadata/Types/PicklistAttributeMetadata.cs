using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PicklistAttributeMetadata : EnumAttributeMetadata
    {
        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.PicklistAttributeMetadata";


        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Picklist;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.PicklistType);

        /// <summary>
        /// Gets or sets the formula definition for calculated and rollup attributes.
        /// </summary>
        public string FormulaDefinition { get; set; }

        /// <summary>
        /// Gets the bitmask value that describes the sources of data used in a calculated attribute or whether the data sources are invalid.
        /// </summary>
        public int SourceTypeMask { get; set; }
    }
}