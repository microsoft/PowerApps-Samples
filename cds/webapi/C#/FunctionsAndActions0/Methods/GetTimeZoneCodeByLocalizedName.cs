using PowerApps.Samples;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public static partial class Extensions
{
    public static async Task<GetTimeZoneCodeByLocalizedNameResponse> GetTimeZoneCodeByLocalizedName(
        this Service service, 
        string localizedStandardName,
        int localeId)
    {
        string function = "GetTimeZoneCodeByLocalizedName(LocalizedStandardName=@p1,LocaleId=@p2)";
        string query = $"?@p1='{localizedStandardName}'&@p2={localeId}";

        try
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(service.BaseAddress + function + query)
            };

            var response = await service.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GetTimeZoneCodeByLocalizedNameResponse>(content);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetTimeZoneCodeByLocalizedName: {ex.Message}", ex);
        }
    }
}

