using Newtonsoft.Json.Linq;
using PowerApps.Samples.Metadata.Types;
using System.Text;

namespace PowerApps.Samples.Metadata.Messages
{
    /// <summary>
    /// Contains the data to create a new customer lookup attribute, and optionally, to add it to a specified unmanaged solution.
    /// </summary>
    public sealed class CreateCustomerRelationshipsRequest : HttpRequestMessage
    {
        /// <summary>
        /// Returns the HttpRequestMessage to create a customer lookup attribute.
        /// </summary>
        /// <param name="lookup">The metadata for the lookup field used to store the ID of the related record.</param>
        /// <param name="oneToManyRelationships">The metadata array for the one-to-many relationships to the Account and Contact entity.</param>
        /// <param name="solutionUniqueName">The name of the unmanaged solution to which you want to add this customer lookup attribute to.</param>
        public CreateCustomerRelationshipsRequest(
            ComplexLookupAttributeMetadata lookup, 
            List<ComplexOneToManyRelationshipMetadata> oneToManyRelationships, 
            string? solutionUniqueName)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "CreateCustomerRelationships", 
                uriKind: UriKind.Relative);


            JObject _body = new()
            {
                ["Lookup"] = JObject.FromObject(lookup),
                ["OneToManyRelationships"] = JArray.FromObject(oneToManyRelationships)
            };

            if (!string.IsNullOrWhiteSpace(solutionUniqueName))
            {
                _body["SolutionUniqueName"] = solutionUniqueName;
            }

            Content = new StringContent(
                content: _body.ToString(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
