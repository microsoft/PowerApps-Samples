using PowerApps.Samples.Messages;

namespace PowerApps.Samples.Methods
{
    public static partial class Extensions
    {
        public static async Task SetColumnValue<T>(this Service service, EntityReference entityReference, string propertyName, T value)
        {

            SetColumnValueRequest<T> request = new(entityReference, propertyName, value);

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
