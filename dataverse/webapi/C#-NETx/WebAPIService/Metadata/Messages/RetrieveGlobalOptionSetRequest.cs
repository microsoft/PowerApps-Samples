using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to retrieve a global optionset (choice)
    /// </summary>
    public sealed class RetrieveGlobalOptionSetRequest : HttpRequestMessage
    {
        /// <summary>
        /// Returns a RetrieveGlobalOptionSetRequest
        /// </summary>
        /// <param name="metadataid">The Id of the global optionset</param>
        /// <param name="name">The name of the global optionset</param>
        /// <param name="type">Specify the type of option set: Picklist or Boolean. Defaults to Picklist</param>
        /// <exception cref="Exception"></exception>
        public RetrieveGlobalOptionSetRequest(
            Guid? metadataid = null,
            string? name = null,
            OptionSetType type = OptionSetType.Picklist)
        {

            if (metadataid == null && string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("RetrieveGlobalOptionSetRequest constructor requires either name or metadataid parameters.");
            }
            if (type == OptionSetType.State || type == OptionSetType.Status) {
                throw new Exception("RetrieveGlobalOptionSetRequest type parameter must be OptionSetType.Picklist or OptionSetType.Boolean");               
            }


            //Get the key to use
            string key;
            if (metadataid != null)
            {
                key = metadataid.ToString();
            }
            else
            {
                key = $"Name='{name}'";
            }

            // Boolean global option sets exist, but cannot be used in custom Boolean columns.
            string optionSetType = (type == OptionSetType.Picklist) ? "OptionSetMetadata" : "BooleanOptionSetMetadata";

           string _uri = new($"GlobalOptionSetDefinitions({key})" +
                       $"/Microsoft.Dynamics.CRM.{optionSetType}");

            Method = HttpMethod.Get;
            RequestUri = new Uri(uriString: _uri, uriKind: UriKind.Relative);
            Headers.Add("Consistency", "Strong");
        }
    }
}

