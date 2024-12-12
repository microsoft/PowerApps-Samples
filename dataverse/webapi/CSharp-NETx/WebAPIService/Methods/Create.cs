using Newtonsoft.Json.Linq;
using PowerApps.Samples.Messages;

namespace PowerApps.Samples.Methods
{
    public static partial class Extensions
    {
        /// <summary>
        /// Creates a record
        /// </summary>
        /// <param name="service">The Service</param>
        /// <param name="entitySetName">The EntitySetName for the table</param>
        /// <param name="record">Contains the data to create the record.</param>
        /// <returns>A reference to the created record.</returns>
        public static async Task<EntityReference> Create(
            this Service service, 
            string entitySetName, 
            JObject record) {

            CreateRequest request = new(entitySetName: entitySetName, record:record);

            try
            {
                CreateResponse response = await service.SendAsync<CreateResponse>(request: request);

                return response.EntityReference;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
