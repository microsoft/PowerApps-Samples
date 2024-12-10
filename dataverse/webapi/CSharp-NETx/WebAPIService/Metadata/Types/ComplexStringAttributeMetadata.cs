using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexStringAttributeMetadata : ComplexAttributeMetadata
    {
        public new AttributeTypeCode AttributeType { get; } = AttributeTypeCode.String;

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.StringType);

        public int DatabaseLength { get; set; }

        public StringFormat Format { get; set; }

        public StringFormatName FormatName { get; set; }

        public string FormulaDefinition { get; set; }

        public ImeMode ImeMode { get; set; }

        public bool? IsLocalizable { get; set; }

        public int MaxLength { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexStringAttributeMetadata";
        public int SourceTypeMask { get; set; }
        public string YomiOf { get; set; }
    }
}