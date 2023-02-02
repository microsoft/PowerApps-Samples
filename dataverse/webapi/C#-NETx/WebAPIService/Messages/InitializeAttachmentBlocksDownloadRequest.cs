using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to initialize download of an attachment.
    /// </summary>
    public sealed class InitializeAttachmentBlocksDownloadRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the InitializeAttachmentBlocksDownloadRequest
        /// </summary>
        /// <param name="target">A reference to the attachment to download.</param>
        public InitializeAttachmentBlocksDownloadRequest(EntityReference target)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "InitializeAttachmentBlocksDownload",
                uriKind: UriKind.Relative);

            Content = new StringContent(
                    content: JsonConvert.SerializeObject(
                        value: new JObject() {
                            {
                                "Target", target.AsJObject(
                                    entityLogicalName:"activitymimeattachment",
                                    primaryKeyLogicalName:"activitymimeattachmentid")
                            }
                        },
                       formatting: Formatting.Indented),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
