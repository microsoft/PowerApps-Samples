using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to associate a record to a collection.
    /// </summary>
    public sealed class AssociateRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the AssociateRequest.
        /// </summary>
        /// <param name="baseAddress">The Service.BaseAddress</param>
        /// <param name="entityWithCollection">The entity with the collection.</param>
        /// <param name="collectionName">The name of the collection</param>
        /// <param name="entityToAdd">The record to add</param>
        public AssociateRequest(Uri baseAddress,
            EntityReference entityWithCollection,
            string collectionName,
            EntityReference entityToAdd)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: $"{entityWithCollection.Path}/{collectionName}/$ref", 
                uriKind: UriKind.Relative);
            Content = new StringContent(
                        content: new JObject() 
                        {
                           { "@odata.id", $"{baseAddress}{entityToAdd.Path}"}
                        }.ToString(),
                        encoding: Encoding.UTF8,
                        mediaType: "application/json");
        }
    }
}
