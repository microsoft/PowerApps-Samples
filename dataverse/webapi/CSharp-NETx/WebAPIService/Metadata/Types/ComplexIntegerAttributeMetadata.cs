using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexIntegerAttributeMetadata : ComplexAttributeMetadata
    {
        public new AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Integer;

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.IntegerType);

        public IntegerFormat Format { get; set; }

        public string FormulaDefinition { get; set; }

        public int MaxValue { get; set; }

        public int MinValue { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexIntegerAttributeMetadata";
        public int SourceTypeMask { get; set; }
    }
}