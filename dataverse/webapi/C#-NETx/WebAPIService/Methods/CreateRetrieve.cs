using Newtonsoft.Json.Linq;
using PowerApps.Samples.Messages;

namespace PowerApps.Samples.Methods
{
    public static partial class Extensions
    {
        /// <summary>
        /// Creates a record and retrieves it.
        /// </summary>
        /// <param name="service">The Service</param>
        /// <param name="entitySetName">The EntitySetName for the table</param>
        /// <param name="record">Contains the data to create the record.</param>
        /// <param name="query">The query string parameters</param>
        /// <param name="includeAnnotations">Whether to include annotations with the data.</param>
        /// <returns>The created record.</returns>
        public static async Task<JObject> CreateRetrieve(
            this Service service,
            string entitySetName,
            JObject record,
            string? query,
            bool includeAnnotations = false)
        {

            CreateRetrieveRequest request = new(
                entitySetName: entitySetName, 
                record: record,
                query: query,
                includeAnnotations: includeAnnotations);

            try
            {
                CreateRetrieveResponse response = await service.SendAsync<CreateRetrieveResponse>(request: request);

                return response.Record;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
