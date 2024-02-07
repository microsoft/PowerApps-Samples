namespace PowerApps.Samples.Metadata.Messages
{
    public sealed class RetrieveEntityKeyRequest : HttpRequestMessage
    {
        private string _entityLogicalName;
        private string _logicalName;
        private string? _query;
        private string _uri = string.Empty;

        public RetrieveEntityKeyRequest(string entityLogicalName, string logicalName, string? query = null) 
        {
            _entityLogicalName = entityLogicalName;
            _logicalName = logicalName;
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
        /// The logical name of the key
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
                $"/Keys(LogicalName='{_logicalName}')");


            if (_query != null)
            {

                baseUri += _query;
            }

            _uri = baseUri;

        }
    }
}
