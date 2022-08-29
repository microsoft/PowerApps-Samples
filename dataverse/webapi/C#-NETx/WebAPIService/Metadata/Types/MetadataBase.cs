using Newtonsoft.Json;
using System;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public abstract class MetadataBase : Crmmodelbaseentity
    {
        /// <summary>
        /// Indicates whether the item of metadata has changed.
        /// </summary>
        public bool? HasChanged { get; set; }

        /// <summary>
        /// A unique identifier for the metadata item.
        /// </summary>
        public Guid? MetadataId { get; set; }
    }
}