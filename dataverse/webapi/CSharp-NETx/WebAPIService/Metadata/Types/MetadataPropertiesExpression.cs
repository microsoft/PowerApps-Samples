using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MetadataPropertiesExpression
    {
        public MetadataPropertiesExpression(){}

        public MetadataPropertiesExpression(params string[] propertyNames)
        {

            PropertyNames = propertyNames.ToList();
        }

        public bool AllProperties { get; set; }
        public List<string> PropertyNames { get; set; }
    }
}
