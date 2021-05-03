using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {

        public static async Task<EntityCollection<T>> RetrieveMultiple<T>(this Service service, string query, bool formattedValues) where T : IEntity
        {

            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    query = $"?{query}";
                }

                string SetName = (string)typeof(T).GetProperty("SetName").GetValue(null);

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(service.BaseAddress + SetName + query),

                };
                if (formattedValues)
                {
                    request.Headers.Add("Prefer", "odata.include-annotations=\"*\"");
                }

                var response = await service.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<EntityCollection<T>>(content);

            }
            catch (Exception ex)
            {
                throw new Exception($"Error in RetrieveMultiple: {ex.Message}", ex);
            }
        }
    }
}
