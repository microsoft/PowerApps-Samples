using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Query
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EntityQueryExpression : MetadataQueryExpression
    {
        public AttributeQueryExpression AttributeQuery { get; set; }
        public EntityKeyQueryExpression KeyQuery { get; set; }
        public LabelQueryExpression LabelQuery { get; set; }
        public RelationshipQueryExpression RelationshipQuery { get; set; }
    }
}