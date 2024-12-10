using Newtonsoft.Json;
using PowerApps.Samples.Types;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to export a solution
    /// </summary>
    public sealed class ExportSolutionRequest : HttpRequestMessage
    {


        /// <summary>
        /// Initializes the ExportSolutionRequest
        /// </summary>
        /// <param name="parameters"></param>
        public ExportSolutionRequest(ExportSolutionParameters parameters)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "ExportSolution",
                uriKind: UriKind.Relative);

            Content = new StringContent(
                    content: JsonConvert.SerializeObject(parameters, Formatting.Indented),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
