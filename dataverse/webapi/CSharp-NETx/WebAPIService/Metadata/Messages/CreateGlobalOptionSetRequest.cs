using Newtonsoft.Json.Linq;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to create a global OptionSet (Choice)
    /// </summary>
    public sealed class CreateGlobalOptionSetRequest : HttpRequestMessage
    {

        public CreateGlobalOptionSetRequest(OptionSetMetadataBase optionSet, string? solutionUniqueName)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "GlobalOptionSetDefinitions", 
                uriKind: UriKind.Relative);
            if (!string.IsNullOrWhiteSpace(solutionUniqueName))
            {
                Headers.Add("MSCRM.SolutionUniqueName", solutionUniqueName);
            }
            Content = new StringContent(
                content: JObject.FromObject(optionSet).ToString(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
