using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the RetrieveGlobalOptionSetRequest
    /// </summary>
    /// <typeparam name="T">The type of optionset: either OptionSetMetadata or BooleanOptionSetMetadata.</typeparam>
    public sealed class RetrieveGlobalOptionSetResponse<T> : HttpResponseMessage where T : OptionSetMetadataBase
    {
        public T OptionSetMetadata => JsonConvert.DeserializeObject<T>(Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
}
