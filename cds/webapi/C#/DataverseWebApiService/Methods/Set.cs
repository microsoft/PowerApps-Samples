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
                    
                string path = service.BaseAddress + entityReference.Path + "/" + property;
                string val = string.Empty;


                switch (value) {

                    case EntityReference er:
                        val = $"{{ \"@odata.id\": \"{service.BaseAddress}{er.Path}\" }}";
                        path += "/$ref";
                        break;
                    default:
                        val = $"{{\"value\": {JsonSerializer.Serialize(value)}}}";                            
                        break;                    
                }
                

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(path),
                    Content = new StringContent(val, Encoding.UTF8, "application/json")
                };

               var response = await service.SendAsync(request);
                response.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Set: {ex.Message}", ex);
            }
        }

    }
}
