using PowerApps.Samples;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public static partial class Extensions
{
    public static async Task WinOpportunity(this Service service, int status, OpportunityCloseActivity opportunityClose)
    {
        JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        var requestcontent = new WinOpportunityRequest { 
            OpportunityClose = opportunityClose,
            Status = status
        };

        try
        {
            string content = JsonSerializer.Serialize(requestcontent, options);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(service.BaseAddress + "WinOpportunity"),
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };

            var response = await service.SendAsync(request);
            response.Dispose();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in WinOpportunity: {ex.Message}", ex);
        }
    }
}

