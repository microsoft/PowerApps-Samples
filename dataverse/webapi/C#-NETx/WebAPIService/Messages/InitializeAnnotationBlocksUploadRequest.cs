using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to initialize upload of an note
    /// </summary>
    public sealed class InitializeAnnotationBlocksUploadRequest : HttpRequestMessage
    {

        public InitializeAnnotationBlocksUploadRequest(EntityReference target)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "InitializeAnnotationBlocksUpload",
                uriKind: UriKind.Relative);

            JObject body = new()
            {
                {"Target",target.AsJObject(
                            entityLogicalName: "annotation",
                            primaryKeyLogicalName: "annotationid") }
            };

            Content = new StringContent(
                    content: body.ToString(),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
