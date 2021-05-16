using PowerApps.Samples;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public static partial class Extensions
{
    public static async Task<CalculateTotalTimeIncidentResponse> CalculateTotalTimeIncident(this Service service, EntityReference incidentRef)
    {
        try
        {

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(service.BaseAddress + incidentRef.Path + "/Microsoft.Dynamics.CRM.CalculateTotalTimeIncident")
            };

            var response = await service.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CalculateTotalTimeIncidentResponse>(content);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in CalculateTotalTimeIncident: {ex.Message}", ex);
        }
    }
}
