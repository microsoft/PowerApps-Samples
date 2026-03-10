using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to delete an option
    /// </summary>
    public sealed class DeleteOptionValueRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the DeleteOptionValueRequest
        /// </summary>
        /// <param name="parameters">Contains the data about the option to delete.</param>
        public DeleteOptionValueRequest(
            DeleteOptionValueParameters parameters)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: $"DeleteOptionValue",
                uriKind: UriKind.Relative);

            Content = new StringContent(
                content: JsonConvert.SerializeObject(parameters, Formatting.Indented),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
