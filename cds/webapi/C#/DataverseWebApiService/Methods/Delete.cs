using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public static partial class Extensions
    {
        public static async Task Delete(this Service service, EntityReference entityReference, string eTag = null)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(service.BaseAddress + entityReference.Path)
                };
                if (eTag != null)
                {
                    //Will prevent delete if eTag value is not current for the record.
                    request.Headers.Add("If-Match", eTag);
                }

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
