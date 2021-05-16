using PowerApps.Samples;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public static partial class Extensions
{
    public static async Task<AddToQueueResponse> AddToQueue<T>(
        this Service service, 
        Queue queue, 
        T target, 
        Queue sourceQueue = null, 
        QueueItem queueItemProperties = null) where T : IEntity
    {
        JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        var path = queue.ToEntityReference().Path;

        var requestcontent = new AddToQueueRequest<T>
        {
            Target = target,
            SourceQueue = sourceQueue,
            QueueItemProperties = queueItemProperties
        };

        try
        {
            string content = JsonSerializer.Serialize(requestcontent, options);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(service.BaseAddress + path + "/Microsoft.Dynamics.CRM.AddToQueue"),
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };

            var response = await service.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AddToQueueResponse>(responseContent);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in AddToQueue: {ex.Message}", ex);
        }
    }
}

