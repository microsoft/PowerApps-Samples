using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    public class InsertOptionValueParameters
    {
        [JsonProperty("Color", NullValueHandling = NullValueHandling.Ignore)]
        public string? Color { get; set; }
        /// <summary>
        /// The name of the global option set.
        /// </summary>
        [JsonProperty("OptionSetName", NullValueHandling = NullValueHandling.Ignore)]
        public string? OptionSetName { get; set; }
        /// <summary>
        /// The name of the attribute when updating a local option set.
        /// </summary>
        [JsonProperty("AttributeLogicalName", NullValueHandling = NullValueHandling.Ignore)]
        public string? AttributeLogicalName { get; set; }
        [JsonProperty("ExternalValue", NullValueHandling = NullValueHandling.Ignore)]
        public string? ExternalValue { get; set; }
        /// <summary>
        /// The logical name of the entity when updating the local option set in a picklist attribute.
        /// </summary>
        [JsonProperty("EntityLogicalName", NullValueHandling = NullValueHandling.Ignore)]
        public string? EntityLogicalName { get; set; }
        /// <summary>
        /// The value for the option.
        /// </summary>
        [JsonProperty("Value", NullValueHandling = NullValueHandling.Ignore)]
        public int Value { get; set; }
        /// <summary>
        /// The label for the option.
        /// </summary>
        [JsonProperty("Label", NullValueHandling = NullValueHandling.Ignore)]
        public Label? Label { get; set; }
        /// <summary>
        /// Description for the option.
        /// </summary>
        [JsonProperty("Description", NullValueHandling = NullValueHandling.Ignore)]
        public Label? Description { get; set; }
        [JsonProperty("ParentValues", NullValueHandling = NullValueHandling.Ignore)]
        public int[]? ParentValues { get; set; }
        /// <summary>
        /// The name of the unmanaged solution that this option value should be associated with.
        /// </summary>
        [JsonProperty("SolutionUniqueName", NullValueHandling = NullValueHandling.Ignore)]
        public string? SolutionUniqueName { get; set; }

    }
}
