using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class StateOptionMetadata : OptionMetadata
    {
        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.StateOptionMetadata";

        /// <summary>
        /// The default status value associated with this state.
        /// </summary>
        public int DefaultStatus { get; set; }

        /// <summary>
        /// The name of the state that does not change.
        /// </summary>
        public string InvariantName { get; set; }
    }
}