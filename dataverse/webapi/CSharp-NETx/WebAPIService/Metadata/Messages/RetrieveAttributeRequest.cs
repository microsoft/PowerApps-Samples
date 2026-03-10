using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to retrieve a column definition
    /// </summary>
    public sealed class RetrieveAttributeRequest : HttpRequestMessage
    {
        private string _entityLogicalName;
        private string _logicalName;
        private AttributeType _type = AttributeType.AttributeMetadata;
        private string? _query;
        private string _uri = string.Empty;

        /// <summary>
        /// Initializes the RetrieveAttributeRequest
        /// </summary>
        /// <param name="entityLogicalName">The logical name of the table the column belongs to.</param>
        /// <param name="logicalName">The logical name of the column.</param>
        /// <param name="type">The type of column.</param>
        /// <param name="query">The properties of the column to return.</param>
        public RetrieveAttributeRequest(
            string entityLogicalName,
            string logicalName,
            AttributeType type,
            string? query = null)
        {
            _entityLogicalName = entityLogicalName;
            _logicalName = logicalName;
            _type = type;
            _query = query;

            GenerateUri();

            Method = HttpMethod.Get;
            RequestUri = new Uri(uriString: _uri, uriKind: UriKind.Relative);
            Headers.Add("Consistency", "Strong");
            _query = query;
        }

        /// <summary>
        /// The logical name of the table the column belongs to.
        /// </summary>
        public string EntityLogicalName
        {
            get
            {
                return _entityLogicalName;
            }

            set
            {

                _entityLogicalName = value;
                GenerateUri();
            }
        }

        /// <summary>
        /// The logical name of the column
        /// </summary>
        public string LogicalName
        {
            get
            {
                return _logicalName;
            }

            set
            {

                _logicalName = value;
                GenerateUri();
            }
        }

        /// <summary>
        /// The type of the column.
        /// </summary>
        public AttributeType Type
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

        /// <summary>
        /// The properties of the column to return.
        /// </summary>
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
            string baseUri = new($"EntityDefinitions(LogicalName='{_entityLogicalName}')" +
                $"/Attributes(LogicalName='{_logicalName}')" +
                $"/Microsoft.Dynamics.CRM.{_type}");            
            if (_query != null) {

                baseUri += _query;
            }

            _uri = baseUri;

        }
    }
}

