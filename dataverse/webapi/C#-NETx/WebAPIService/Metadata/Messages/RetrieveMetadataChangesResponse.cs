using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the RetrieveMetadataChanges Function
    /// </summary>
    public sealed class RetrieveMetadataChangesResponse : HttpResponseMessage
    {
        // Cache the async content
        private string? _content;

        //Provides JObject for property getters
        private JObject _jObject
        {
            get
            {
                _content ??= Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return JObject.Parse(_content);
            }
        }

        /// <summary>
        ///  Timestamp identifier for the retrieved metadata.
        /// </summary>
        public string ServerVersionStamp => _jObject[nameof(ServerVersionStamp)].ToString();

        public DeletedMetadataCollection DeletedMetadata => _jObject[nameof(DeletedMetadata)].ToObject<DeletedMetadataCollection>();
        public List<ComplexEntityMetadata> EntityMetadata => _jObject[nameof(EntityMetadata)].ToObject<List<ComplexEntityMetadata>>();
    }
}
