using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RelationshipQueryExpression
    {
        public MetadataFilterExpression Criteria { get; set; }
        public MetadataPropertiesExpression Properties { get; set; }
    }
}
