using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MoneyAttributeMetadata : AttributeMetadata
    {

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.MoneyAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Money;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.MoneyType);

        /// <summary>
        /// The input method editor (IME) mode for the attribute.
        /// </summary>
        public ImeMode ImeMode { get; set; }

        /// <summary>
        /// The maximum value for the attribute.
        /// </summary>
        public double MaxValue { get; set; }

        /// <summary>
        /// The minimum value for the attribute.
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// The precision for the attribute.
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// The precision source for the attribute.
        /// </summary>
        public int PrecisionSource { get; set; }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string CalculationOf { get; set; }

        /// <summary>
        /// The formula definition for calculated and rollup attributes.
        /// </summary>
        public string FormulaDefinition { get; set; }

        /// <summary>
        /// A bitmask value that describes the sources of data used in a calculated attribute or whether the data sources are invalid.
        /// </summary>
        public int SourceTypeMask { get; set; }

        /// <summary>
        /// The base currency or the transaction currency.
        /// </summary>
        public bool IsBaseCurrency { get; set; }
    }
}