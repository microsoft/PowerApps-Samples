using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ComplexEnumAttributeMetadata : ComplexAttributeMetadata
    {
        public int DefaultFormValue { get; set; }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ComplexEnumAttributeMetadata";       


        public ComplexOptionSetMetadata OptionSet { get; set; }
    }
}