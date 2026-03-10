using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class UniqueIdentifierAttributeMetadata : AttributeMetadata
    {

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.UniqueIdentifierAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Uniqueidentifier;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.UniqueidentifierType);
    }
}