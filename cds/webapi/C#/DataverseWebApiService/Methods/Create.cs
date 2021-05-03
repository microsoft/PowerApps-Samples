using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {
        public static async Task<EntityReference> Create(this Service service, IEntity entity)
        {
            try
            {
                JsonSerializerOptions options = new()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                };

                Type type = entity.GetType();

                string content = JsonSerializer.Serialize(entity, type, options);

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(service.BaseAddress + (string)type.GetProperty("SetName").GetValue(null)),
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };

                var response = await service.SendAsync(request);

                return new EntityReference(response.Headers.GetValues("OData-EntityId").FirstOrDefault());

            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Create: {ex.Message}", ex);
            }
        }

    }
}
