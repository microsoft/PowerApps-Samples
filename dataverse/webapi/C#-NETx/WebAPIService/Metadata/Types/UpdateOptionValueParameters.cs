using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class UpdateOptionValueParameters
    {
        public string? Color { get; set; }
        /// <summary>
        /// The name of the global option set.
        /// </summary>
        public string? OptionSetName { get; set; }
        /// <summary>
        /// The name of the attribute when updating a local option set.
        /// </summary>
        public string? AttributeLogicalName { get; set; }

        public string? ExternalValue { get; set; }
        /// <summary>
        /// The logical name of the entity when updating the local option set in a picklist attribute.
        /// </summary>

        public string? EntityLogicalName { get; set; }
        /// <summary>
        /// The value for the option.
        /// </summary>

        public int Value { get; set; }
        /// <summary>
        /// The label for the option.
        /// </summary>

        public Label? Label { get; set; }
        /// <summary>
        /// Description for the option.
        /// </summary>

        public Label? Description { get; set; }
        /// <summary>
        /// Indicates whether to keep text defined for languages not included in the Label.
        /// </summary>

        public bool MergeLabels { get; set; }

        public int[]? ParentValues { get; set; }
        /// <summary>
        /// The name of the unmanaged solution that this option value should be associated with.
        /// </summary>

        public string? SolutionUniqueName { get; set; }

    }
}
