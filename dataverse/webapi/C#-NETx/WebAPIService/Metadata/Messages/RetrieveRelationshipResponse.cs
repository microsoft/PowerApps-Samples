using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the RetrieveRelationshipRequest
    /// </summary>
    /// <typeparam name="T">The type of RelationshipMetadata. Either ManyToManyRelationshipMetadata or OneToManyRelationshipMetadata</typeparam>
    public sealed class RetrieveRelationshipResponse<T> : HttpResponseMessage where T : RelationshipMetadataBase
    {
        public T RelationshipMetadata => JsonConvert.DeserializeObject<T>(Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
}
