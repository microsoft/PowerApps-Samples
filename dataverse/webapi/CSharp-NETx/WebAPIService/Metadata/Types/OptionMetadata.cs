using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    /// <summary>
    /// Contains metadata representing an option within an Option set.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class OptionMetadata : MetadataBase
    {
        /// <summary>
        /// The value of the option.
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// The label containing the text for the option.
        /// </summary>
        public Label Label { get; set; }

        /// <summary>
        /// The label containing the description for the option.
        /// </summary>
        public Label Description { get; set; }

        /// <summary>
        /// The Hex color assigned to the option
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Whether the option is part of a managed solution.
        /// </summary>
        public bool? IsManaged { get; set; }

        /// <summary>
        /// TODO: Something related to Virtual entities
        /// </summary>
        public string ExternalValue { get; set; }
    }
}