using PowerApps.Samples.Messages;

namespace PowerApps.Samples.Methods
{
    public static partial class Extensions
    {
        /// <summary>
        /// Returns whether a given user has the System Administrator security role
        /// </summary>
        /// <param name="service">The Service</param>
        /// <param name="systemUserId">The systemuserid of the user to test.</param>
        /// <returns></returns>
        public static async Task<bool> IsSystemAdmin(this Service service, Guid systemUserId)
        {
            IsSystemAdminRequest req = new(systemUserId);

            try
            {
                IsSystemAdminResponse resp = await service.SendAsync<IsSystemAdminResponse>(req);
                return resp.HasRole;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
