using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexFileAttributeMetadata : ComplexAttributeMetadata
    {
        public new AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Virtual;

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.FileType);

        public int MaxSizeInKB { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexFileAttributeMetadata";
    }
}