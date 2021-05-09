using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {
        public static async Task Delete(this Service service, EntityReference entityReference)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(service.BaseAddress + entityReference.Path)
                };

              var response =  await service.SendAsync(request);
                response.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Delete: {ex.Message}", ex);
            }
        }

    }
}
