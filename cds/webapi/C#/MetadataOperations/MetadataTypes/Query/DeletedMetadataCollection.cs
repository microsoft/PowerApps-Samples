using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PowerApps.Samples.Metadata.Query
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DeletedMetadataCollection
    {
        public int Count { get; set; }
        public bool IsReadOnly { get; set; }

        public List<DeletedMetadataFilters> Keys { get; set; }

        public List<Guid> Values { get; set; }
    }
}