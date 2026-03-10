using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to insert an option
    /// </summary>
    public sealed class InsertOptionValueRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the InsertOptionValueRequest
        /// </summary>
        /// <param name="parameters">Data about the option to insert.</param>
        public InsertOptionValueRequest(
            InsertOptionValueParameters parameters)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: $"InsertOptionValue",
                uriKind: UriKind.Relative);

            Content = new StringContent(
                content: JsonConvert.SerializeObject(parameters, Formatting.Indented),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
