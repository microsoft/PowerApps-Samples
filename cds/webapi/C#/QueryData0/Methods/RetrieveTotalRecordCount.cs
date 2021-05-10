using PowerApps.Samples;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public static partial class Extensions
{
    public static async Task<RetrieveTotalRecordCountResponse> RetrieveTotalRecordCount(this Service service, params string[] entityLogicalNames)
    {
        try
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(service.BaseAddress + $"RetrieveTotalRecordCount(EntityNames={JsonSerializer.Serialize(entityLogicalNames)})")
            };

            var response = await service.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RetrieveTotalRecordCountResponse>(content);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in RetrieveTotalRecordCount: {ex.Message}", ex);
        }
    }
}