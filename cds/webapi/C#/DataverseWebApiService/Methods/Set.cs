using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {
        public static async Task Set<T>(this Service service, EntityReference entityReference, string property, T value)
        {
            try
            {
                var val = JsonSerializer.Serialize(value);
                

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(service.BaseAddress + entityReference.GetPath()+"/"+property),
                    Content = new StringContent($"{{\"value\": {val}}}", Encoding.UTF8, "application/json")
                };

                await service.SendAsync(request);             
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Set: {ex.Message}", ex);
            }
        }

    }
}
