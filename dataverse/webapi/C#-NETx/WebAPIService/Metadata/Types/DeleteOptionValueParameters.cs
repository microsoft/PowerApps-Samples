using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DeleteOptionValueParameters
    {
        /// <summary>
        /// The name of the global option set that contains the value.
        /// </summary>
        public string? OptionSetName { get; set; }
        /// <summary>
        /// The logical name of the attribute from which to delete the option value.
        /// </summary>
        public string? AttributeLogicalName { get; set; }
        /// <summary>
        /// The logical name of the entity.
        /// </summary>
        public string? EntityLogicalName { get; set; }
        /// <summary>
        /// The logical name of the entity.
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// The solution name associated with this option value.
        /// </summary>
        public string? SolutionUniqueName { get; set; }
    }
}
