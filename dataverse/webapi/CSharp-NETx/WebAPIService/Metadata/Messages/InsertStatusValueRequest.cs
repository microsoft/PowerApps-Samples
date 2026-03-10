using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to insert a option into a StatusAttributeMetadata attribute.
    /// </summary>
    public sealed class InsertStatusValueRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the InsertStatusValueRequest
        /// </summary>
        /// <param name="parameters">Data about the option to insert.</param>
        public InsertStatusValueRequest(
            InsertStatusValueParameters parameters)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: $"InsertStatusValue",
                uriKind: UriKind.Relative);

            Content = new StringContent(
                content: JsonConvert.SerializeObject(parameters, Formatting.Indented),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
