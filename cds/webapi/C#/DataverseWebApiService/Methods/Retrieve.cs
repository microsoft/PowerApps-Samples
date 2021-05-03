using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {
        /// <summary>
        /// Retrieves an entity
        /// </summary>
        /// <typeparam name="T">The type of record</typeparam>
        /// <param name="service"></param>
        /// <param name="entityReference">A reference to the record to retrieve.</param>
        /// <param name="query">The OData query</param>
        /// <param name="formattedValues">Whether to include formatted values</param>
        /// <returns></returns>
        public static async Task<T> Retrieve<T>(this Service service, EntityReference entityReference, string query, bool formattedValues) where T : IEntity
        {

            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    query = $"?{query}";
                }

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(service.BaseAddress + entityReference.GetPath() + query),

                };
                if (formattedValues)
                {
                    request.Headers.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
                }

                var response = await service.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<T>(content);

            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Retrieve: {ex.Message}", ex);
            }
        }
    }
}
