using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to initialize download of an file column value
    /// </summary>
    public sealed class InitializeFileBlocksDownloadRequest : HttpRequestMessage
    {

        public InitializeFileBlocksDownloadRequest(
            string entityLogicalName,
            string primaryKeyLogicalName,
            Guid entityId,
            string fileAttributeName)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "InitializeFileBlocksDownload",
                uriKind: UriKind.Relative);

            JObject content = new() {

                { "Target",new JObject(){
                        { primaryKeyLogicalName,entityId },
                        { "@odata.type",$"Microsoft.Dynamics.CRM.{entityLogicalName}"}
                    }
                },
                { "FileAttributeName", fileAttributeName}
            };

            Content = new StringContent(
                    content: content.ToString(),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
