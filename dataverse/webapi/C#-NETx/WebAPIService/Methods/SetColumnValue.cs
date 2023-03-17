using PowerApps.Samples.Messages;

namespace PowerApps.Samples.Methods
{
    public static partial class Extensions
    {
        /// <summary>
        /// Sets the value of a column for a table row
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="service">The service</param>
        /// <param name="entityReference">A reference to the table row.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value to set</param>
        /// <returns></returns>
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
