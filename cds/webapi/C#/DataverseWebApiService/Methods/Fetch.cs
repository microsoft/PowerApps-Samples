using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {

        public static async Task<EntityCollection> Fetch(this Service service, string setName, XDocument fetchXml, bool formattedValues = false) 
        {

            try
            {                
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(service.BaseAddress + setName + $"?fetchXml={WebUtility.UrlEncode(fetchXml.ToString())}"),

                };
                if (formattedValues)
                {
                    request.Headers.Add("Prefer", "odata.include-annotations=\"*\"");
                }

                var response = await service.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<EntityCollection>(content);

            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Fetch: {ex.Message}", ex);
            }
        }
    }
}
