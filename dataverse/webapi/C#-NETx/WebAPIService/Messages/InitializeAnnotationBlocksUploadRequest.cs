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
        /// <summary>
        /// Initializes the InitializeAnnotationBlocksUploadRequest
        /// </summary>
        /// <param name="target">The annotation record with an 'annotationid' value.</param>
        /// <exception cref="ArgumentException">The target must contain a valid 'annotationid' value.</exception>
        public InitializeAnnotationBlocksUploadRequest(JObject target)
        {
            bool IsValidTarget = target.ContainsKey("annotationid") &&
                (Guid)target["annotationid"] != Guid.Empty;

            if (!IsValidTarget) {
                throw new ArgumentException("The target must contain a valid 'annotationid' value.");
            }

            if (!target.ContainsKey("@odata.type"))
            {
                target.Add("@odata.type", "Microsoft.Dynamics.CRM.annotation");
            }

            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "InitializeAnnotationBlocksUpload",
                uriKind: UriKind.Relative);

            JObject body = new()
            {
                {"Target", target}
            };

            Content = new StringContent(
                    content: body.ToString(),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
