using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to update a record
    /// </summary>
    public sealed class UpdateRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the UpdateRequest
        /// </summary>
        /// <param name="entityReference">A reference to the record to update.</param>
        /// <param name="record">Contains the data to update</param>
        /// <param name="preventDuplicateRecord">Whether to throw an error when a duplicate record is detected.</param>
        /// <param name="partitionId">The partition key to use.</param>
        /// <param name="eTag">The current ETag value to compare.</param>
        public UpdateRequest(
            EntityReference entityReference, 
            JObject record, 
            bool preventDuplicateRecord = false, 
            string? partitionId = null, 
            string? eTag = null)
        {
            string path;
            if (partitionId != null)
            {
                path = $"{entityReference.Path}?partitionId={partitionId}";
            }
            else
            {
                path = entityReference.Path;
            }

            Method = HttpMethod.Patch;
            RequestUri = new Uri(
                uriString: path, 
                uriKind: UriKind.Relative);
            Content = new StringContent(
                    content: record.ToString(),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json");        
            if (preventDuplicateRecord)
            {
                //If duplicate detection enabled for table only
                Headers.Add("MSCRM.SuppressDuplicateDetection", "false");
            }
            if (eTag != null)
            {
                //Prevent updating record which has changed on the server
                Headers.Add("If-Match", eTag);
            }
            else
            {   //Ensure no Create
                Headers.Add("If-Match", "*"); 
            }
        }
    }
}
