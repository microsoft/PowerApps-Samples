using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {
        public static async Task Delete(this Service service, EntityReference entityreference)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(service.BaseAddress + entityreference.GetPath())
                };

                await service.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Delete: {ex.Message}", ex);
            }
        }

    }
}
