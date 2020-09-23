using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerApps.Samples.Metadata.Query
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MetadataPropertiesExpression
    {
        public bool AllProperties { get; set; }
        public List<string> PropertyNames { get; set; }
    }
}