using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Types
{
    public class ImportSolutionParameters
    {
        [JsonProperty("OverwriteUnmanagedCustomizations", NullValueHandling = NullValueHandling.Ignore)]
        public bool OverwriteUnmanagedCustomizations { get; set; }

        [JsonProperty("PublishWorkflows", NullValueHandling = NullValueHandling.Ignore)]
        public bool PublishWorkflows { get; set; }

        public byte[] CustomizationFile { get; set; }

        [JsonProperty("ImportJobId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid ImportJobId { get; set; }

        [JsonProperty("ConvertToManaged", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ConvertToManaged { get; set; }

        [JsonProperty("SkipProductUpdateDependencies", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SkipProductUpdateDependencies { get; set; }

        [JsonProperty("HoldingSolution", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HoldingSolution { get; set; }

        [JsonProperty("SkipQueueRibbonJob", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SkipQueueRibbonJob { get; set; }

        [JsonProperty("LayerDesiredOrder", NullValueHandling = NullValueHandling.Ignore)]
        public LayerDesiredOrder? LayerDesiredOrder { get; set; }

        [JsonProperty("AsyncRibbonProcessing", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AsyncRibbonProcessing { get; set; }

        [JsonProperty("ComponentParameters", NullValueHandling = NullValueHandling.Ignore)]
        public JObject ComponentParameters { get; set; }

        [JsonProperty("IsTemplateMode", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsTemplateMode { get; set; }

        [JsonProperty("SolutionParameters", NullValueHandling = NullValueHandling.Ignore)]
        public SolutionParameters? SolutionParameters { get; set; }

        [JsonProperty("TemplateDisplayNamePrefix", NullValueHandling = NullValueHandling.Ignore)]
        public string? TemplateDisplayNamePrefix { get; set; }




    }
}
