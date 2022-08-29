using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the RetrieveAttributeRequest
    /// </summary>
    /// <typeparam name="T">The type of attribute.</typeparam>
    public sealed class RetrieveAttributeResponse<T> : HttpResponseMessage where T : AttributeMetadata
    {
        public T AttributeMetadata => JsonConvert.DeserializeObject<T>(Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
}
