using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerApps.Samples.Metadata.Query
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LabelQueryExpression : MetadataQueryBase
    {
        public List<int> FilterLanguages { get; set; }
        public int MissingLabelBehavior { get; set; }
    }
}