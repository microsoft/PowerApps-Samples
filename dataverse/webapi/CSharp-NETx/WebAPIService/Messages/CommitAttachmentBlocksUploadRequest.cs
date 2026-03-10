using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to commits the uploaded data blocks to the activitymimeattachment store.
    /// </summary>
    public sealed class CommitAttachmentBlocksUploadRequest : HttpRequestMessage
    {
        /// <summary>
        /// Commits the uploaded data blocks to the activitymimeattachment store.
        /// </summary>
        /// <param name="target">The target entity.</param>
        /// <param name="blockList">The IDs of the uploaded data blocks, in the correct sequence, that will result in the final activitymimeattachment when the data blocks are combined.</param>
        /// <param name="fileContinuationToken">A token that uniquely identifies a sequence of related data uploads.</param>
        public CommitAttachmentBlocksUploadRequest(
            JObject target,
            List<string> blockList,
            string fileContinuationToken)
        {

            if (!target.ContainsKey("@odata.type"))
            {
                target.Add("@odata.type", "Microsoft.Dynamics.CRM.activitymimeattachment");
            }

            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "CommitAttachmentBlocksUpload",
                uriKind: UriKind.Relative);

            JObject body = new()
            {
                { "Target", target },
                { "BlockList", JToken.FromObject(blockList.ToArray()) },
                { "FileContinuationToken", fileContinuationToken}
            };

            Content = new StringContent(
                    content: body.ToString(),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
