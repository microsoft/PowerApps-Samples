using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RelationshipType
    {
        OneToManyRelationship, ManyToManyRelationship
    }
}