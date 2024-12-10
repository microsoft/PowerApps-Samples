using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DecimalAttributeMetadata : AttributeMetadata
    {


        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.DecimalAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Decimal;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.DecimalType);

        /// <summary>
        /// The minimum supported value for this attribute.
        /// </summary>
        public decimal MaxValue { get; set; }

        /// <summary>
        /// The maximum supported value for this attribute.
        /// </summary>
        public decimal MinValue { get; set; }

        /// <summary>
        /// The precision for the attribute.
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// The input method editor (IME) mode for the attribute.
        /// </summary>
        public ImeMode? ImeMode { get; set; }

        /// <summary>
        /// A bitmask value that describes the sources of data used in a calculated attribute or whether the data sources are invalid.
        /// </summary>
        public int? SourceTypeMask { get; set; }

        /// <summary>
        /// The formula definition for calculated and rollup attributes.
        /// </summary>
        public string FormulaDefinition { get; set; }
    }
}