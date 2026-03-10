using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to retrieve the ComplexEntityMetadata for a table definition.
    /// </summary>
    public sealed class RetrieveEntityRequest : HttpRequestMessage
    {
        // This uses the RetrieveEntity Function
        // https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/retrieveentity
        // It returns a ComplexEntityMetadata ComplexType rather than EntityMetadata EntityType
        // Not including LogicalName parameter because it cannot be used without MetadataId
        // See RetrieveEntityDefinitionRequest to
        //  - Return an EntityMetadata type and use OData $select to choose properties
        //  - Use the LogicalName as the key without MetadataId

        private Guid _metadataId;
        private EntityFilters _entityFilters = EntityFilters.Entity;
        private bool _retrieveAsIfPublished;
        private string _uri = string.Empty;

        /// <summary>
        /// Initializes the RetrieveEntityRequest
        /// </summary>
        /// <param name="metadataId">The Id of the table definition.</param>
        /// <param name="entityFilters">Filter to control how much data for each entity is retrieved.</param>
        /// <param name="retrieveAsIfPublished">Whether to retrieve the metadata that has not been published.</param>
        public RetrieveEntityRequest(
            Guid metadataId,
            EntityFilters entityFilters = EntityFilters.Entity,
            bool retrieveAsIfPublished = false)
        {

            _metadataId = metadataId;
            _entityFilters = entityFilters;
            _retrieveAsIfPublished = retrieveAsIfPublished;

            GenerateUri();

            Method = HttpMethod.Get;
            RequestUri = new Uri(uriString: _uri, uriKind: UriKind.Relative);
            Headers.Add("Consistency", "Strong");
        }

        /// <summary>
        /// Filter to control how much data for each entity is retrieved.
        /// </summary>
        public EntityFilters EntityFilters
        {
            get
            {
                return _entityFilters;
            }

            set
            {

                _entityFilters = value;
                GenerateUri();
            }
        }

        /// <summary>
        /// The Id of the table definition.
        /// </summary>
        public Guid MetadataId
        {
            get
            {
                return _metadataId;
            }

            set
            {

                _metadataId = value;
                GenerateUri();
            }
        }

        /// <summary>
        /// Whether to retrieve the metadata that has not been published.
        /// </summary>
        public bool RetrieveAsIfPublished
        {
            get
            {
                return _retrieveAsIfPublished;
            }

            set
            {

                _retrieveAsIfPublished = value;
                GenerateUri();
            }
        }

        private void GenerateUri()
        {
            StringBuilder sb = new("RetrieveEntity(EntityFilters=@p1,MetadataId=@p2,RetrieveAsIfPublished=@p3)?");
            sb.Append($"@p1=Microsoft.Dynamics.CRM.EntityFilters'{_entityFilters}'&");
            sb.Append($"@p2={_metadataId}&");
            sb.Append($"@p3={_retrieveAsIfPublished.ToString().ToLower()}");

            _uri = sb.ToString();

        }
    }
}

