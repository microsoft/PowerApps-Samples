using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexDecimalAttributeMetadata : ComplexAttributeMetadata
    {
        public new AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Decimal;

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.DecimalType);

        public string FormulaDefinition { get; set; }

        public ImeMode ImeMode { get; set; }

        public decimal MaxValue { get; set; }

        public decimal MinValue { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexDecimalAttributeMetadata";
        public int Precision { get; set; }
        public int SourceTypeMask { get; set; }
    }
}