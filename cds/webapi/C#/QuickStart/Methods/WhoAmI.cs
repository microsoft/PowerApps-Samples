using PowerApps.Samples;
using QuickStart;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public static partial class Extensions
{
    public static async Task<WhoAmIResponse> WhoAmI(this Service service)
    {
        try
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(service.BaseAddress + "WhoAmI")
            };

            var response = await service.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<WhoAmIResponse>(content);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

