using System.Xml.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to execute a query using FetchXml
    /// </summary>
    public sealed class FetchXmlRequest : HttpRequestMessage
    {

        /// <summary>
        /// Initializes the FetchXmlRequest
        /// </summary>
        /// <param name="entitySetName">The name of the entity set.</param>
        /// <param name="fetchXml">The document containing the fetchXml</param>
        /// <param name="includeAnnotations">Whether annotations should be included in the response.</param>
        public FetchXmlRequest(string entitySetName, XDocument fetchXml, bool includeAnnotations = false)
        {
            Method = HttpMethod.Get;
            RequestUri = new Uri(
                uriString: $"{entitySetName}?fetchXml={WebUtility.UrlEncode(fetchXml.ToString())}&$count=true",
                uriKind: UriKind.Relative);
            if (includeAnnotations)
            {
                Headers.Add("Prefer", "odata.include-annotations=\"*\"");
            }
        }
    }
}
