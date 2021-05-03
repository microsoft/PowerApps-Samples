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
        public static async Task Update(this Service service, IEntity entity)
        {
            try
            {
                JsonSerializerOptions options = new()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                };

                string content = JsonSerializer.Serialize(entity, entity.GetType(), options);

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Patch,
                    RequestUri = new Uri(service.BaseAddress + entity.ToEntityReference().GetPath()),
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                //Prevent Create
                request.Headers.Add("If-Match", "*");

                var response = await service.SendAsync(request);

            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Update: {ex.Message}", ex);
            }
        }

    }
}
