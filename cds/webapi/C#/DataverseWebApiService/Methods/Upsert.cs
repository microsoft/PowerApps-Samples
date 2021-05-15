using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {
        public static async Task Upsert(this Service service, IEntity entity)
        {
            try
            {
                EntityReference entityRef;
                try
                {
                    entityRef = entity.ToEntityReference();
                }
                catch (Exception)
                {

                    throw new Exception("Entity passed to Upsert must have the primary id value set.");
                }

                JsonSerializerOptions options = new()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                };

                string content = JsonSerializer.Serialize(entity, entity.GetType(), options);

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Patch,
                    RequestUri = new Uri(service.BaseAddress + entityRef.Path),
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };

                var response = await service.SendAsync(request);
                response.Dispose();

            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Upsert: {ex.Message}", ex);
            }
        }

    }
}
