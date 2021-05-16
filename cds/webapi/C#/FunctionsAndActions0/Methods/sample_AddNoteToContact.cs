using PowerApps.Samples;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public static partial class Extensions
{
    public static async Task<Annotation> sample_AddNoteToContact(
        this Service service,
        Contact entity,
        string noteTitle,
        string noteText)
    {
        JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        var path = entity.ToEntityReference().Path;

        var requestcontent = new sample_AddNoteToContactRequest
        {
            NoteTitle = noteTitle,
            NoteText = noteText
        };

        try
        {
            string content = JsonSerializer.Serialize(requestcontent, options);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(service.BaseAddress + path + "/Microsoft.Dynamics.CRM.sample_AddNoteToContact"),
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };

            var response = await service.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Annotation>(responseContent);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in sample_AddNoteToContact: {ex.Message}", ex);
        }
    }
}

