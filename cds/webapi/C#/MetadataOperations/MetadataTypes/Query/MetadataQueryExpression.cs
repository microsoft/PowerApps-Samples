using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Query
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public abstract class MetadataQueryExpression : MetadataQueryBase
    {
        public MetadataFilterExpression Criteria { get; set; }
        public MetadataPropertiesExpression Properties { get; set; }
    }
}