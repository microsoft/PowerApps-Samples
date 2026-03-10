using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to retrieve schema data and changes over time.
    /// </summary>
    public sealed class RetrieveMetadataChangesRequest : HttpRequestMessage
    {
        private string _uri = string.Empty;
        private EntityQueryExpression? _query;
        private DeletedMetadataFilters? _deletedMetadataFilters;
        private string _clientVersionStamp = string.Empty;
        private Guid? _appModuleId;
        private bool? _retrieveAllSettings;

        /// <summary>
        /// Returns an HttpRequestMessage for the RetrieveMetadataChanges Function
        /// </summary>
        public RetrieveMetadataChangesRequest()
        {
            Method = HttpMethod.Get;
            Headers.Add("Consistency", "Strong");
            SetUri();
        }

        /// <summary>
        /// The query criteria for retrieving metadata changes.
        /// </summary>
        public EntityQueryExpression? Query
        {
            get
            {
                return _query;
            }

            set
            {

                _query = value;
                SetUri();


            }
        }

        /// <summary>
        /// The enumeration that filters the deleted metadata to be retrieved.
        /// </summary>
        public DeletedMetadataFilters? DeletedMetadataFilters
        {
            get
            {
                return _deletedMetadataFilters;
            }

            set
            {
                _deletedMetadataFilters = value;
                SetUri();
            }
        }

        /// <summary>
        /// Timestamp value representing when the last request was made.
        /// </summary>
        public string ClientVersionStamp
        {
            get
            {
                return _clientVersionStamp;
            }

            set
            {
                _clientVersionStamp = value;
                SetUri();
            }
        }

        /// <summary>
        /// The unique identifier of the app module.
        /// </summary>
        public Guid? AppModuleId
        {
            get
            {
                return _appModuleId;
            }

            set
            {
                _appModuleId = value;
                SetUri();
            }
        }

        /// <summary>
        /// For internal use only
        /// </summary>
        public bool? RetrieveAllSettings
        {
            get
            {
                return _retrieveAllSettings;
            }

            set
            {
                _retrieveAllSettings = value;
                SetUri();
            }
        }



        private void SetUri()
        {

            List<string> parameters = new();
            List<string> parameterValues = new();


            if (_query != null)
            {
                parameters.Add("Query=@p1");
                parameterValues.Add($"@p1={JsonConvert.SerializeObject(_query)}");
            }

            if (_deletedMetadataFilters != null)
            {
                parameters.Add("DeletedMetadataFilters=@p2");

                parameterValues.Add($"@p2=Microsoft.Dynamics.CRM.DeletedMetadataFilters'{_deletedMetadataFilters}'");
                          
            }

            if (!string.IsNullOrWhiteSpace(_clientVersionStamp))
            {
                parameters.Add($"ClientVersionStamp=@p3");

                parameterValues.Add($"@p3='{_clientVersionStamp}'");

            }
            if (_appModuleId.HasValue)
            {
                parameters.Add("AppModuleId=@p4");
                parameterValues.Add($"@p4={_appModuleId}");
            }

            if (_retrieveAllSettings.HasValue)
            {
                parameters.Add("RetrieveAllSettings=@p5");
                parameterValues.Add($"@p5={_retrieveAllSettings}");
            }

            StringBuilder sb = new("RetrieveMetadataChanges");

            if (parameters.Count > 0) {
                sb.Append($"({string.Join(",",parameters)})?");
                sb.Append(string.Join("&",parameterValues));
            
            }

            RequestUri = new Uri(uriString: sb.ToString(), uriKind: UriKind.Relative);

        }
    }
}
