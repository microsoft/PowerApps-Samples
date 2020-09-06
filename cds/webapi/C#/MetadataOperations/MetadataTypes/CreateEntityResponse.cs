using System;

namespace PowerApps.Samples.Metadata
{
    /// <summary>
    /// Contains the response from the CreateEntity method.
    /// </summary>
    public class CreateEntityResponse
    {
        /// <summary>
        /// Gets the MetadataId of the primary attribute for the newly created entity.
        /// </summary>
        public Guid AttributeId { get; set; }
        /// <summary>
        /// Gets the MetadataId of the newly created entity.
        /// </summary>
        public Guid EntityId { get; set; }
    }
}
