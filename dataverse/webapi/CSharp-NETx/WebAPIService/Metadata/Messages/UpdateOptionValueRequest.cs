using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to update an option
    /// </summary>
    public sealed class UpdateOptionValueRequest : HttpRequestMessage
    {       
        public UpdateOptionValueRequest(
            UpdateOptionValueParameters parameters)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: $"UpdateOptionValue",
                uriKind: UriKind.Relative);

            Content = new StringContent(
                content: JsonConvert.SerializeObject(parameters, Formatting.Indented),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
