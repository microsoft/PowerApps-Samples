using Newtonsoft.Json;
using System;

namespace PowerApps.Samples
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Solution
    {
        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.solution";

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("friendlyname")]
        public string FriendlyName { get; set; }

        [JsonProperty("publisherid")]
        public Guid? PublisherId { get; internal set; }

        private string setpublisherid;

        [JsonProperty("publisherid@odata.bind")]
        public string SetPublisherId { 
            get { return setpublisherid; }
            set { setpublisherid = $"publishers({value})"; }
         }

        [JsonProperty("solutionid")]
        public Guid? SolutionId { get; set; }

        [JsonProperty("solutionpackageversion")]
        public string SolutionPackageVersion { get; set; }

        [JsonProperty("solutiontype")]
        public SolutionType? SolutionType { get; set; }

        [JsonProperty("uniquename")]
        public string UniqueName { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

    }

    public enum SolutionType { 
        None = 0, Snapshot = 1, Internal = 2
    }
}
