using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to initialize upload of an file column
    /// </summary>
    public sealed class InitializeFileBlocksUploadRequest : HttpRequestMessage
    {
        /// <summary>
        /// Intitializes the InitializeFileBlocksUploadRequest
        /// </summary>
        /// <param name="entityLogicalName">The logical name of the table.</param>
        /// <param name="primaryKeyLogicalName">The logical name of the primary key of the table.</param>
        /// <param name="entityId">The id of the table row.</param>
        /// <param name="fileAttributeName">The logical name of the file column.</param>
        /// <param name="fileName">A filename to associate with the binary data file.</param>
        public InitializeFileBlocksUploadRequest(string entityLogicalName,
            string primaryKeyLogicalName,
            Guid entityId,
            string fileAttributeName,
            string fileName)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "InitializeFileBlocksUpload",
                uriKind: UriKind.Relative);

            JObject content = new() {
                { "Target",new JObject(){
                        { primaryKeyLogicalName,entityId },
                        { "@odata.type",$"Microsoft.Dynamics.CRM.{entityLogicalName}"}
                    }
                },
                { "FileName", fileName},
                { "FileAttributeName", fileAttributeName}
            };

            Content = new StringContent(
                    content: content.ToString(),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
