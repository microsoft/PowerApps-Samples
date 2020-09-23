using System;
using System.Collections.Generic;

namespace PowerApps.Samples.Metadata
{
    /// <summary>
    /// The response from the CreateCustomerRelationships Action.
    /// </summary>
    public class CreateCustomerRelationshipsResponse
    {
        /// <summary>
        /// An array of relationship IDs created for the attribute to Account and Contact entities.
        /// </summary>
        public List<Guid> RelationshipIds { get; set; }
        /// <summary>
        /// The MetadataId of the LookupAttributeMetadata that is created.
        /// </summary>
        public Guid AttributeId { get; set; }
    }
}
