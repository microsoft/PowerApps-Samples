using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Query;
using System.Collections.Generic;

namespace PowerApps.Samples.Metadata
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RetrieveMetadataChangesResponse
    {
        public DeletedMetadataCollection DeletedMetadata { get; set; }
        public List<ComplexEntityMetadata> EntityMetadata { get; set; }
        public string ServerVersionStamp { get; set; }
    }
}