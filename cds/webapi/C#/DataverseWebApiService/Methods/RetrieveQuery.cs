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

        public static async Task<EntityCollection> RetrieveQuery(this Service service, string setName, QueryType type, Guid id, bool formattedValues = false)
        {

            try
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(service.BaseAddress + setName + $"?{Enum.GetName(typeof(QueryType),type)}={id}"),

                };
                if (formattedValues)
                {
                    request.Headers.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
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

    public enum QueryType {
        savedQuery,
        userQuery
    }
}
