using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public class Service : IDisposable
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly Config config;

        public Uri BaseAddress { get; }

        public Service(Config configParam)
        {
            config = configParam;
            httpClient.BaseAddress = new Uri($"{config.Url}/api/data/v{config.Version}/");
            httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutInSeconds);
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            if (config.CallerObjectId != Guid.Empty)
            {
                httpClient.DefaultRequestHeaders.Add("CallerObjectId", config.CallerObjectId.ToString());
            }

            BaseAddress = httpClient.BaseAddress;
        }


        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, int retryCount = 0)
        {

            await Authenticate();

            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending request", ex);
            }

            //Handle service protection limits
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {

                if (++retryCount <= config.MaxRetries)
                {
                    int seconds;
                    if (response.Headers.Contains("Retry-After"))
                    {
                        seconds = int.Parse(response.Headers.GetValues("Retry-After").FirstOrDefault());

                    }
                    else
                    {
                        seconds = (int)Math.Pow(2, retryCount);
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(seconds));
                    return await SendAsync(request, retryCount);
                }
                else
                {
                    throw new Exception($"{config.MaxRetries} retries attempted without success.");
                }

            }

            if (!response.IsSuccessStatusCode)
            {
                var exception = await ParseError(response);
                throw exception;

            }


            return response;

        }

        public static async Task<ServiceException> ParseError(HttpResponseMessage response)
        {
            var requestId = response.Headers.GetValues("REQ_ID").FirstOrDefault();
            var content = await response.Content.ReadAsStringAsync();
            ODataError oDataError = null;

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                oDataError = JsonSerializer.Deserialize<ODataError>(content, options);
            }
            catch (Exception)
            {
                //Error may not be in OData Error format
            }

            if (oDataError != null)
            {

                var exception = new ServiceException(oDataError.Error.Message)
                {
                    ODataError = oDataError,
                    Content = content,
                    ReasonPhrase = response.ReasonPhrase,
                    HttpStatusCode = response.StatusCode,
                    RequestId = requestId
                };
                return exception;


            }
            else
            {
                var exception = new ServiceException(response.ReasonPhrase)
                {
                    Content = content,
                    ReasonPhrase = response.ReasonPhrase,
                    HttpStatusCode = response.StatusCode,
                    RequestId = requestId
                };
                return exception;
            }
        }

        private async Task Authenticate()
        {

            try
            {
                httpClient.DefaultRequestHeaders.Authorization =
              new AuthenticationHeaderValue("Bearer", await config.GetAccessToken());
            }
            catch (Exception ex)
            {
                throw new Exception($"There was a problem authenticating: {ex.Message}", ex);
            }
        }


        ~Service()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {

                ReleaseClient();
            }

            ReleaseClient();
        }

        private void ReleaseClient()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
            }
        }
    }
}
