using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class OrderOptionParameters
    {

        /// <summary>
        /// The name of the global option set you want to edit options for.
        /// </summary>
        public string? OptionSetName { get; set; }

        /// <summary>
        /// The name of the table for a local optionset
        /// </summary>
        public string? EntityLogicalName { get; set; }


        /// <summary>
        /// The name of the attribute for a local optionset
        /// </summary>
        public string? AttributeLogicalName { get; set; }

        /// <summary>
        /// The array of option values in the wanted order.
        /// </summary>
        public int[] Values { get; set; }

        /// <summary>
        /// Unique name of the solution.
        /// </summary>
        public string? SolutionUniqueName { get; set; }

    }
}
