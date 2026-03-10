using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    public class InsertStatusValueParameters
    {
        [JsonProperty("Color", NullValueHandling = NullValueHandling.Ignore)]
        public string? Color { get; set; }
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        [JsonProperty("OptionSetName", NullValueHandling = NullValueHandling.Ignore)]
        public string? OptionSetName { get; set; }
        /// <summary>
        /// The logical name of the status attribute.
        /// </summary>
        [JsonProperty("AttributeLogicalName", NullValueHandling = NullValueHandling.Ignore)]
        public string? AttributeLogicalName { get; set; }
        /// <summary>
        /// The logical name of the status attribute.
        /// </summary>
        [JsonProperty("EntityLogicalName", NullValueHandling = NullValueHandling.Ignore)]
        public string? EntityLogicalName { get; set; }
        /// <summary>
        /// Value for the new status.
        /// </summary>
        [JsonProperty("Value", NullValueHandling = NullValueHandling.Ignore)]
        public int? Value { get; set; }
        /// <summary>
        /// Label for the new status.
        /// </summary>
        [JsonProperty("Label", NullValueHandling = NullValueHandling.Ignore)]
        public Label? Label { get; set; }
        /// <summary>
        /// Description for the option.
        /// </summary>
        [JsonProperty("Description", NullValueHandling = NullValueHandling.Ignore)]
        public Label? Description { get; set; }
        /// <summary>
        /// State code for the new status.
        /// </summary>
        [JsonProperty("StateCode", NullValueHandling = NullValueHandling.Ignore)]
        public int StateCode { get; set; }
        /// <summary>
        /// The name of the unmanaged solution that this status options should be associated with.
        /// </summary>
        [JsonProperty("SolutionUniqueName", NullValueHandling = NullValueHandling.Ignore)]
        public string? SolutionUniqueName { get; set; }
    }
}
