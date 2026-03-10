using Newtonsoft.Json;
using PowerApps.Samples.Search.Types;

namespace PowerPlatform.Dataverse.CodeSamples.types
{
    /// <summary>
    /// Decribes the searchstatusResponse.response.value property 
    /// </summary>
    class SearchStatusResult
    {
        /// <summary>
        /// The current search status
        /// </summary>
        [JsonProperty("status")]
        public SearchStatus Status { get; set; }

        /// <summary>
        /// The current lockbox status
        /// </summary>
        [JsonProperty("lockboxstatus")]
        public LockboxStatus LockboxStatus { get; set; }

        /// <summary>
        /// The current customer managed key status
        /// </summary>
        [JsonProperty("cmkstatus")]
        public CMKStatus CMKStatus { get; set; }

        /// <summary>
        /// Information on enabled tables
        /// </summary>
        [JsonProperty("entitystatusresults")]
        public List<EntityStatusInfo>? EntityStatusInfo { get; set; }

        /// <summary>
        /// Information about th status of synchronized many-to-many relationships
        /// </summary>
        [JsonProperty("manytomanyrelationshipsyncstatus")]
        public List<ManyToManyRelationshipSyncStatus>? ManyToManyRelationshipSyncStatus { get; set; }
    }
}
