using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples.Metadata.Messages
{

    public sealed class RetrieveRelationshipRequest : HttpRequestMessage
    {
        private Guid? _metadataId;
        private string _schemaName;
        private RelationshipType _type = RelationshipType.OneToManyRelationship;
        private string? _query;
        private string _uri = string.Empty;


        public RetrieveRelationshipRequest(
            RelationshipType type,
            Guid? metadataId = null,
            string schemaName = null,            
            string? query = null)
        {
            _metadataId = metadataId;
            _schemaName = schemaName;
            _type = type;
            _query = query;

            GenerateUri();

            Method = HttpMethod.Get;
            RequestUri = new Uri(uriString: _uri, uriKind: UriKind.Relative);
            Headers.Add("Consistency", "Strong");
            _query = query;
        }


        public Guid? MetadataId
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


        public string SchemaName
        {
            get
            {
                return _schemaName;
            }

            set
            {

                _schemaName = value;
                GenerateUri();
            }
        }

        public RelationshipType Type
        {
            get
            {
                return _type;
            }

            set
            {

                _type = value;
                GenerateUri();
            }
        }

        public string Query
        {
            get
            {
                return _query;
            }

            set
            {

                _query = value;
                GenerateUri();
            }
        }

        /// <summary>
        /// Generates the URI based on the current values
        /// </summary>
        private void GenerateUri()
        {
            
            if (string.IsNullOrWhiteSpace(_schemaName) && _metadataId == null)
            {
                throw new Exception("MetadataId or SchemaName must contain a value");
            }
            string key;
            if (_metadataId != null)
            {
                key = _metadataId.ToString();
            }
            else
            {
                key = $"SchemaName='{_schemaName}'";
            }

            string typeName = _type == RelationshipType.OneToManyRelationship ? "OneToManyRelationshipMetadata" : "ManyToManyRelationshipMetadata";

            string baseUri = new($"RelationshipDefinitions({key})" +
                $"/Microsoft.Dynamics.CRM.{typeName}");
            if (_query != null)
            {
                baseUri += _query;
            }

            _uri = baseUri;

        }
    }
}

