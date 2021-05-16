using PowerApps.Samples;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public static partial class Extensions
{
    public static async Task sample_CreateCustomer(
        this Service service,
        string customerType, 
        string accountName = null, 
        string contactFirstName = null, 
        string contactLastName = null)
    {
        JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
       
        var requestcontent = new sample_CreateCustomerRequest
        {
            CustomerType = customerType,
            AccountName = accountName,
            ContactFirstName = contactFirstName,
            ContactLastName = contactLastName
        };

        try
        {
            string content = JsonSerializer.Serialize(requestcontent, options);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(service.BaseAddress  + "sample_CreateCustomer"),
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };

            var response = await service.SendAsync(request);
            response.Dispose();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in sample_CreateCustomer: {ex.Message}", ex);
        }
    }
}

