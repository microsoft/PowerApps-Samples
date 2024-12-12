using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexMoneyAttributeMetadata : ComplexAttributeMetadata
    {
        public new AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Money;

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.MoneyType);

        public string CalculationOf { get; set; }

        public string FormulaDefinition { get; set; }

        public ImeMode ImeMode { get; set; }

        public bool? IsBaseCurrency { get; set; }

        public double MaxValue { get; set; }

        public double MinValue { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexMoneyAttributeMetadata";
        public int Precision { get; set; }

        public int PrecisionSource { get; set; }
        public int SourceTypeMask { get; set; }
    }
}