using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal class ComplexBooleanAttributeMetadata : ComplexAttributeMetadata
    {
        public new AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Boolean;

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.BooleanType);

        public bool? DefaultValue { get; set; }

        public string FormulaDefinition { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexBooleanAttributeMetadata";
        public ComplexBooleanOptionSetMetadata OptionSet { get; set; }
        public string SourceTypeMask { get; set; }
    }
}