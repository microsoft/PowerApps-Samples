using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class IntegerAttributeMetadata : AttributeMetadata
    {

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.IntegerAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Integer;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.IntegerType);

        /// <summary>
        /// The maximum supported value for this attribute.
        /// </summary>
        public int? MaxValue { get; set; }

        /// <summary>
        /// The minimum supported value for this attribute.
        /// </summary>
        public int? MinValue { get; set; }

        /// <summary>
        /// The format options for the integer attribute.
        /// </summary>
        public IntegerFormat Format { get; set; }

        /// <summary>
        /// A bitmask value that describes the sources of data used in a calculated attribute or whether the data sources are invalid.
        /// </summary>
        public int SourceTypeMask { get; set; }

        /// <summary>
        /// The formula definition for calculated and rollup attributes.
        /// </summary>
        public string FormulaDefinition { get; set; }
    }
}