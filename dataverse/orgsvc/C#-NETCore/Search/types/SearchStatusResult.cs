using Newtonsoft.Json;
using PowerApps.Samples.Search.Types;

namespace PowerPlatform.Dataverse.CodeSamples.types
{
    class SearchStatusResult
    {
        [JsonProperty("status")]
        public SearchStatus Status { get; set; }

        [JsonProperty("lockboxstatus")]
        public LockboxStatus LockboxStatus { get; set; }

        [JsonProperty("cmkstatus")]
        public CMKStatus CMKStatus { get; set; }

        [JsonProperty("entitystatusresults")]
        public List<EntityStatusInfo>? EntityStatusInfo { get; set; }

        [JsonProperty("manytomanyrelationshipsyncstatus")]
        public List<ManyToManyRelationshipSyncStatus>? ManyToManyRelationshipSyncStatus { get; set; }
    }
}
