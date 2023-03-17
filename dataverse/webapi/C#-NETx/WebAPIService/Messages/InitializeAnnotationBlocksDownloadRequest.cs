using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to initialize download of an note.
    /// </summary>
    public sealed class InitializeAnnotationBlocksDownloadRequest : HttpRequestMessage
    {

        public InitializeAnnotationBlocksDownloadRequest(EntityReference target)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "InitializeAnnotationBlocksDownload",
                uriKind: UriKind.Relative);

            Content = new StringContent(
                    content: JsonConvert.SerializeObject(
                        value: new JObject() {
                            {
                                "Target", target.AsJObject(
                                    entityLogicalName:"annotation", 
                                    primaryKeyLogicalName:"annotationid")
                            }
                        },
                       formatting: Formatting.Indented),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
