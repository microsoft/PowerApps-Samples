using PowerApps.Samples;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public static partial class Extensions
{
    public static async Task<int> GetCount(this Service service, string entitySetName)
    {
        try
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(service.BaseAddress + $"{entitySetName}/$count")
            };

            var response = await service.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<int>(content);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetCount: {ex.Message}", ex);
        }
    }
}
