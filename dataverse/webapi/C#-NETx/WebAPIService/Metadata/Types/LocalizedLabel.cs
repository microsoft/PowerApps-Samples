using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LocalizedLabel : MetadataBase
    {
        public LocalizedLabel(string label, int languagecode)
        {
            Label = label;
            LanguageCode = languagecode;
        }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.LocalizedLabel";

        public string Label { get; set; }
        public int LanguageCode { get; set; }
        public bool IsManaged { get; set; }
    }
}