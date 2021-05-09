using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {
        public static async Task<T> Get<T>(this Service service, EntityReference entityReference, string property)
        {
            try
            {

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(service.BaseAddress + entityReference.Path + "/" + property),

                };

                var response = await service.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return (T)Convert.ChangeType(JsonDocument.Parse(content).RootElement.GetProperty("value").GetString(), typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Get: {ex.Message}", ex);
            }
        }

    }
}
