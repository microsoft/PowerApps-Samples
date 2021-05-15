using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class EntityCollection<T> where T : IEntity
    {

        [JsonPropertyName("@Microsoft.Dynamics.CRM.totalrecordcount")]
        public int TotalRecordCount { get; set; }

        [JsonPropertyName("@odata.count")]
        public int Count { get; set; }

        [JsonPropertyName("@Microsoft.Dynamics.CRM.totalrecordcountlimitexceeded")]
        public bool TotalRecordCountExceeded { get; set; }

        [JsonPropertyName("value")]
        public List<T> Value { get; set; }

        [JsonPropertyName("@odata.nextLink")]
        public Uri NextLink { get; set; }

        [JsonIgnore]
        public bool MoreRecords
        {
            get
            {
                if (NextLink != null)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
