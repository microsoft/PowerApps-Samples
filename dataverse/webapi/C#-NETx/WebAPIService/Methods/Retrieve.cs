using Newtonsoft.Json.Linq;
using PowerApps.Samples.Messages;

namespace PowerApps.Samples.Methods
{
    public static partial class Extensions
    {
        /// <summary>
        /// Retrieves a record.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityReference">A reference to the record to retrieve</param>
        /// <param name="query">The query string parameters</param>
        /// <param name="includeAnnotations">Whether to include annotations with the data.</param>
        /// <param name="eTag">The current ETag value to compare.</param>
        /// <returns></returns>
        public static async Task<JObject> Retrieve(
            this Service service, 
            EntityReference entityReference, 
            string? query, 
            bool includeAnnotations = false,
            string? eTag = null,
            string? partitionId = null)
        {
            RetrieveRequest request = new(
                entityReference: entityReference,
                query: query,
                includeAnnotations: includeAnnotations,
                eTag: eTag, 
                partitionid:partitionId);

            try
            {
                RetrieveResponse response = await service.SendAsync<RetrieveResponse>(request: request);

                return response.Record;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
