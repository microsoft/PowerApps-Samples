using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CascadeConfiguration
    {
        /// <summary>
        /// The referenced entity record owner is changed.
        /// </summary>
        public CascadeType Assign { get; set; }

        /// <summary>
        /// The referenced entity record is deleted.
        /// </summary>
        public CascadeType Delete { get; set; }

        /// <summary>
        /// The record is merged with another record.
        /// </summary> 
        public CascadeType Merge { get; set; }

        /// <summary>
        /// The referencing attribute in a parental relationship changes
        /// </summary> 
        public CascadeType Reparent { get; set; }

        /// <summary>
        /// The referenced entity record is shared with another user.
        /// </summary>
        public CascadeType Share { get; set; }

        /// <summary>
        /// Sharing is removed for the referenced entity record.
        /// </summary>
        public CascadeType Unshare { get; set; }

        /// <summary>
        /// Indicates that the associated activities of the related entity should be included in Activity Associated View for the primary entity.
        /// </summary>
        public CascadeType RollupView { get; set; }
    }
}