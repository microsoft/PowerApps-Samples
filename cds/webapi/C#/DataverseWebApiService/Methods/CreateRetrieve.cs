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
        public static async Task<T> CreateRetrieve<T>(this Service service, IEntity entity, string query, bool formattedValues = false) where T : IEntity
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    query = $"?{query}";
                }


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
                    RequestUri = new Uri(service.BaseAddress + (string)type.GetProperty("SetName").GetValue(null) + query),
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Prefer", "return=representation");

                if (formattedValues)
                {
                    request.Headers.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
                }

                var response = await service.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<T>(responseContent);

            }
            catch (Exception ex)
            {
                throw new Exception($"Error in CreateRetrieve: {ex.Message}", ex);
            }
        }

    }
}
