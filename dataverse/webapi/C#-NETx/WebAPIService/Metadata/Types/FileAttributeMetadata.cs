using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    public  class FileAttributeMetadata : AttributeMetadata
    {
        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.FileAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Virtual;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.FileType);

        public int MaxSizeInKB { get; set; }
    }
}
