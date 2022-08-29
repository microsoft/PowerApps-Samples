using Newtonsoft.Json.Linq;
using PowerApps.Samples.Messages;

namespace PowerApps.Samples.Methods
{
    public static partial class Extensions
    {
        /// <summary>
        /// Updates a record.
        /// </summary>
        /// <param name="service">The Service</param>
        /// <param name="entityReference">A reference to the record to update.</param>
        /// <param name="record">The data to update.</param>
        /// <param name="preventDuplicateRecord">Whether to throw an error when a duplicate record is detected.</param>
        /// <param name="partitionId">The partition key to use.</param>
        /// <param name="eTag">The current ETag value to compare.</param>
        /// <returns></returns>
        public static async Task Update(
            this Service service, 
            EntityReference entityReference, 
            JObject record, 
            bool preventDuplicateRecord = false,
            string? partitionId = null,
            string? eTag = null)
        {

            UpdateRequest request = new(
                entityReference: entityReference, 
                record: record,
                preventDuplicateRecord: preventDuplicateRecord,
                partitionId: partitionId,
                eTag: eTag);

            try
            {
                await service.SendAsync(request: request);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
