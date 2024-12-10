using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class OptionSetMetadata : OptionSetMetadataBase
    {
        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.OptionSetMetadata";

        public List<OptionMetadata> Options { get; set; }
    }
}