using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class BigIntAttributeMetadata : AttributeMetadata
    {


        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.BigInt;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.BigIntType);

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.BigIntAttributeMetadata";

        public long? MaxValue { get; set; }

        public long? MinValue { get; set; }
    }
}