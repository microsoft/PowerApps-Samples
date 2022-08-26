using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Query
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EntityKeyQueryExpression : MetadataQueryExpression
    {
    }
}