using PowerApps.Samples.Batch;
using PowerApps.Samples.Messages;
using System.Xml.Linq;

namespace PowerApps.Samples.Methods
{
    public static partial class Extensions
    {
        /// <summary>
        /// Retrieves the results of a FetchXml query.
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="entitySetName">The entity set name</param>
        /// <param name="fetchXml">The fetchXml Query</param>
        /// <param name="includeAnnotations">Whether to include annotations with the results.</param>
        /// <returns>FetchXmlResponse</returns>
        public static async Task<FetchXmlResponse> FetchXml(
            this Service service,
            string entitySetName,
            XDocument fetchXml,
            bool includeAnnotations)
        {
            FetchXmlRequest fetchXmlRequest = new(
                entitySetName: entitySetName,
                fetchXml: fetchXml,
                includeAnnotations
                );

            // Sending the request as a batch to mitigate issues where FetchXml length exceeds the 
            // max length for a URI sent in the query. This way it will be sent in the body.
            BatchRequest batchRequest = new(service.BaseAddress)
            {
                Requests = new List<HttpRequestMessage> { fetchXmlRequest }
            };

            try
            {
                BatchResponse batchResponse = await service.SendAsync<BatchResponse>(batchRequest);

                HttpResponseMessage firstResponse = batchResponse.HttpResponseMessages.FirstOrDefault();

                FetchXmlResponse fetchXmlResponse = firstResponse.As<FetchXmlResponse>();

                return fetchXmlResponse;
            }
            catch (Exception)
            {

                throw;
            }

            

        }
    }
}
