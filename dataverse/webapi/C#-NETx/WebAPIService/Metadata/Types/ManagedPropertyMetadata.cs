using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    /// <summary>
    /// For internal use only
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ManagedPropertyMetadata : MetadataBase
    {
        public Label Description { get; set; }
        public Label DisplayName { get; set; }
        public string EnablesAttributeName { get; set; }
        public string EnablesEntityName { get; set; }
        public int ErrorCode { get; set; }
        public ManagedPropertyEvaluationPriority EvaluationPriority { get; set; }
        public string IntroducedVersion { get; set; }
        public bool IsGlobalForOperation { get; set; }
        public bool IPrivate { get; set; }
        public string LogicalName { get; set; }
        public ManagedPropertyType ManagedPropertyType { get; set; }
        public ManagedPropertyOperation Operation { get; set; }
    }
}