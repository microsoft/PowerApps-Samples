using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public class Competitor : IEntity
    {
        [JsonIgnore]
        public Guid? Id => competitorid;

        [JsonIgnore]
        public static string SetName => "competitors";

        [JsonIgnore]
        public static string LogicalName => "competitor";

        [JsonPropertyName("@odata.type")]
        public string ODataType => $"Microsoft.Dynamics.CRM.{LogicalName}";


        public Guid? competitorid { get; init; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(competitorid, SetName);
        }

        public string name { get; set; }
        public string strengths { get; set; }

        public List<Opportunity> opportunitycompetitors_association { get; set; }

    }
}