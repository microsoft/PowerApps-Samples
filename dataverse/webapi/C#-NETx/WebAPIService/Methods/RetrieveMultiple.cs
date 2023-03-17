using Newtonsoft.Json.Linq;
using PowerApps.Samples.Messages;

namespace PowerApps.Samples.Methods
{
    public static partial class Extensions
    {
        /// <summary>
        /// Retrieves the results of an OData query.
        /// </summary>
        /// <param name="service">The Service.</param>
        /// <param name="queryUri">An absolute or relative Uri.</param>
        /// <param name="maxPageSize">The maximum number of records to return in a page.</param>
        /// <param name="includeAnnotations">Whether to include annotations with the results.</param>
        /// <returns></returns>
        public static async Task<RetrieveMultipleResponse> RetrieveMultiple(
            this Service service,
            string queryUri,
            int? maxPageSize = null,
            bool includeAnnotations = false)
        {
            RetrieveMultipleRequest request = new(
                queryUri: queryUri,
                maxPageSize: maxPageSize,
                includeAnnotations: includeAnnotations);


            try
            {
                return await service.SendAsync<RetrieveMultipleResponse>(request: request);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
