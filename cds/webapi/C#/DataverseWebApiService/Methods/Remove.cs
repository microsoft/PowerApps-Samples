using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {
        /// <summary>
        /// Disassociates a record from another
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityReference">The record that the item is related to</param>
        /// <param name="navigationProperty">The name of the navigation property</param>
        /// <param name="collectionItem">A reference to the item in a collection-valued navigation property to remove.</param>
        /// <returns></returns>
        public static async Task Remove(this Service service, EntityReference entityReference, string navigationProperty, EntityReference collectionItem = null)
        {
            try
            {
                string path = $"{service.BaseAddress}{entityReference.Path}/{navigationProperty}";

                if (collectionItem == null)
                {
                    path += "/$ref";
                }
                else
                {
                    path += $"({collectionItem.Id})/$ref";
                }

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(path)
                };

                var response = await service.SendAsync(request);
                response.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Remove: {ex.Message}", ex);
            }
        }

    }
}