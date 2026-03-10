using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Label
    {
        public Label(string text, int languageCode)
        {
            LocalizedLabels = new List<LocalizedLabel>() { new LocalizedLabel(text, languageCode) };
            UserLocalizedLabel = new LocalizedLabel(text, languageCode);
        }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.Label";

        public List<LocalizedLabel> LocalizedLabels { get; set; }
        public LocalizedLabel UserLocalizedLabel { get; set; }
    }
}