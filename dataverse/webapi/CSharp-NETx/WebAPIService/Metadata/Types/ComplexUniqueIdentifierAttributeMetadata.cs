using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    public class ComplexUniqueIdentifierAttributeMetadata : ComplexAttributeMetadata
    {
        public new AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Uniqueidentifier;

        public new AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.UniqueidentifierType);

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexUniqueIdentifierAttributeMetadata";
    }
}
