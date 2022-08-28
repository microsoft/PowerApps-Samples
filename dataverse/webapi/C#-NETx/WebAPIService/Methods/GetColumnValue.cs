using PowerApps.Samples.Messages;

namespace PowerApps.Samples.Methods
{
    public static partial class Extensions
    {
        public static async Task<T> GetColumnValue<T>(
            this Service service,
            EntityReference entityReference, 
            string property)
        {
            GetColumnValueRequest request = new(
                entityReference: entityReference,
                property: property);

            try
            {
                GetColumnValueResponse<T> response = await service.SendAsync<GetColumnValueResponse<T>>(request: request);

                return response.Value;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
