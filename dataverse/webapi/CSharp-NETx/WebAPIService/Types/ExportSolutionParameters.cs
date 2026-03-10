using Newtonsoft.Json;

namespace PowerApps.Samples.Types
{

    public class ExportSolutionParameters
    {
        /// <summary>
        /// The unique name of the solution.
        /// </summary>
        public string SolutionName { get; set; }
        /// <summary>
        /// Indicates whether the solution should be exported as a managed solution.
        /// </summary>
        public bool Managed { get; set; }
        /// <summary>
        /// Indicates whether auto numbering settings should be included in the solution being exported.
        /// </summary>
        public bool ExportAutoNumberingSettings { get; set; }
        /// <summary>
        /// Indicates whether calendar settings should be included in the solution being exported
        /// </summary>
        public bool ExportCalendarSettings { get; set; }
        /// <summary>
        /// Indicates whether customization settings should be included in the solution being exported.
        /// </summary>
        public bool ExportCustomizationSettings { get; set; }
        /// <summary>
        /// Indicates whether email tracking settings should be included in the solution being exported.
        /// </summary>
        public bool ExportEmailTrackingSettings { get; set; }
        /// <summary>
        /// Indicates whether general settings should be included in the solution being exported.
        /// </summary>
        public bool ExportGeneralSettings { get; set; }
        /// <summary>
        /// Indicates whether marketing settings should be included in the solution being exported.
        /// </summary>
        public bool ExportMarketingSettings { get; set; }
        /// <summary>
        /// Indicates whether outlook synchronization settings should be included in the solution being exported.
        /// </summary>
        public bool ExportOutlookSynchronizationSettings { get; set; }
        /// <summary>
        /// Indicates whether outlook synchronization settings should be included in the solution being exported.
        /// </summary>
        public bool ExportRelationshipRoles { get; set; }
        /// <summary>
        /// Indicates whether ISV.Config settings should be included in the solution being exported.
        /// </summary>
        public bool ExportIsvConfig { get; set; }
        /// <summary>
        /// Indicates whether sales settings should be included in the solution being exported.
        /// </summary>
        public bool ExportSales { get; set; }

        public bool ExportExternalApplications { get; set; }

        [JsonProperty("ExportComponentsParams", NullValueHandling = NullValueHandling.Ignore)]
        public ExportComponentsParams? ExportComponentsParams { get; set; }

    }
}
