using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexMemoAttributeMetadata : ComplexAttributeMetadata
    {
        public new AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Memo;

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.MemoType);

        public StringFormat Format { get; set; }

        public ImeMode ImeMode { get; set; }

        public bool? IsLocalizable { get; set; }

        public int MaxLength { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexMemoAttributeMetadata";
    }
}