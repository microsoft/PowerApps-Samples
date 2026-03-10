using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexDateTimeAttributeMetadata : ComplexAttributeMetadata
    {
        public new AttributeTypeCode AttributeType { get; } = AttributeTypeCode.DateTime;

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.DateTimeType);

        public BooleanManagedProperty CanChangeDateTimeBehavior { get; set; }

        public DateTimeBehavior DateTimeBehavior { get; set; }

        public DateTimeFormat Format { get; set; }

        public string FormulaDefinition { get; set; }

        public ImeMode ImeMode { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexDateTimeAttributeMetadata";
        public int SourceTypeMask { get; set; }
    }
}