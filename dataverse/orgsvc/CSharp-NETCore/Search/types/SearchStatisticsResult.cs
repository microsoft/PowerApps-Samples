using Newtonsoft.Json;

namespace PowerPlatform.Dataverse.CodeSamples.types
{

    /// <summary>
    /// Used to deserialize the searchstatisticsResponse.response property
    /// </summary>
    class SearchStatisticsResult
    {

        /// <summary>
        /// The storage size in Bytes
        /// </summary>
        [JsonProperty(PropertyName = "storagesizeinbytes")]
        public long StorageSizeInByte { get; set; }


        /// <summary>
        /// The storage size in Megabytes
        /// </summary>
        [JsonProperty(PropertyName = "storagesizeinmb")]
        public long StorageSizeInMb { get; set; }

        /// <summary>
        /// The document count
        /// </summary>
        [JsonProperty(PropertyName = "documentcount")]
        public long DocumentCount { get; set; }
    }
}
