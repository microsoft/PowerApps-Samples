using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {
        /// <summary>
        /// Adds a record to a collection
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityReference">The entity containing the collection</param>
        /// <param name="collectionName">The name of the collection-valued navigation property.</param>
        /// <param name="entityToAdd">A reference to the entity to add to the collection.</param>
        /// <returns></returns>
        public static async Task Add(this Service service, EntityReference entityReference, string collectionName, EntityReference entityToAdd)
        {
            try
            {

                string entityPath = entityReference.Path;

                string content = $"{{ \"@odata.id\": \"{service.BaseAddress}{entityToAdd.Path}\" }}";

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(service.BaseAddress + entityPath+"/"+collectionName+"/$ref"),
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };

               var response =  await service.SendAsync(request);
                response.Dispose();

            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Add: {ex.Message}", ex);
            }
        }

    }
}
