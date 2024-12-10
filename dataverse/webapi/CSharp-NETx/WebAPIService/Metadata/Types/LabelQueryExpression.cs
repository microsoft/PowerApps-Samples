using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LabelQueryExpression
    {
        public List<int> FilterLanguages { get; set; }
        public int MissingLabelBehavior { get; set; }
    }
}
