using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using PowerApps.Samples.Messages;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;


namespace PowerApps.Samples
{
    public class Service : IDisposable
    {
        // Service configuration data passed into the constructor
        private readonly Config config;

        private static IServiceProvider _serviceProvider { get; set; }
        private readonly string WebAPIClientName = "WebAPI";
        private bool _disposedValue;
        private string? _sessionToken = null;
        private int _recommendedDegreeOfParallelism;
        private Guid _userId;
        private Guid _businessUnitId;
        private Guid _organizationId;


        /// <summary>
        /// The constructor for the service
        /// </summary>
        /// <param name="configParam"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public Service(Config configParam)
        {
            config = configParam;
            BaseAddress = new Uri($"{config.Url}/api/data/v{config.Version}/");

            IHostBuilder builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices(services =>
            {
                if (config.DisableCookies)
                {
                    //Don't use cookies
                    services.AddHttpClient(
                        name: WebAPIClientName,
                        configureClient: ConfigureHttpClient
                        )
                    .ConfigurePrimaryHttpMessageHandler(() =>
                        new HttpClientHandler
                        {
                            UseCookies = false
                        }
                    ).AddPolicyHandler(GetRetryPolicy(config));

                }
                else //Use cookies
                {
                    services.AddHttpClient(
                        name: WebAPIClientName,
                        configureClient: ConfigureHttpClient)
                    .AddPolicyHandler(GetRetryPolicy(config));
                }
            });


            builder.ConfigureLogging(logging => {
                // Removing default logging providers
                // so that output is not sent to console.
                // You may wish to enable logging.
                // More information:
                // https://learn.microsoft.com/dotnet/core/extensions/logging-providers
                logging.ClearProviders();
            });

            // Add the named HttpClient configuration to the service provider.
            _serviceProvider = builder.Build().Services;

            // Send a simple request to access the recommended degree of parallelism (DOP).
            var whoAmIResponse =  SendAsync<WhoAmIResponse>(new WhoAmIRequest()).GetAwaiter().GetResult();
            _recommendedDegreeOfParallelism = int.Parse(whoAmIResponse.Headers.GetValues("x-ms-dop-hint").FirstOrDefault());
            // Set the users details
            _userId = whoAmIResponse.UserId;
            _businessUnitId = whoAmIResponse.BusinessUnitId;
            _organizationId = whoAmIResponse.OrganizationId;

        }

        /// <summary>
        /// HttpClient configuration set in the service constructor
        /// </summary>
        /// <param name="httpClient"></param>
        void ConfigureHttpClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = BaseAddress;
            httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutInSeconds);
            httpClient.DefaultRequestHeaders.Add("User-Agent", $"WebAPIService/{Assembly.GetExecutingAssembly().GetName().Version}");
            // Set default headers for all requests
            // See https://learn.microsoft.com/power-apps/developer/data-platform/webapi/compose-http-requests-handle-errors#http-headers
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", "null");           
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Specifies the Retry policies
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(Config config)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(httpResponseMessage => httpResponseMessage.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: config.MaxRetries,
                    sleepDurationProvider: (count, response, context) =>
                    {
                        int seconds;
                        HttpResponseHeaders headers = response.Result.Headers;

                        // Use the value of the Retry-After header if it exists
                        // See https://learn.microsoft.com/power-apps/developer/data-platform/api-limits#retry-operations

                        if (headers.Contains("Retry-After"))
                        {

                            seconds = int.Parse(headers.GetValues("Retry-After").FirstOrDefault());
                        }
                        else
                        {
                            seconds = (int)Math.Pow(2, count);
                        }
                        return TimeSpan.FromSeconds(seconds);
                    },
                    onRetryAsync: (_, _, _, _) => { return Task.CompletedTask; }
                );
        }

        /// <summary>
        /// Provides access to the IHttpClientFactory from the service provider.
        /// </summary>
        /// <returns></returns>
        private static IHttpClientFactory GetHttpClientFactory()
        {
            return (IHttpClientFactory)_serviceProvider.GetService(typeof(IHttpClientFactory));

        }



        /// <summary>
        /// Processes requests and returns responses. Manages Service Protection Limit errors.
        /// </summary>
        /// <param name="request">The request to send.</param>
        /// <returns>The response from the HttpClient</returns>
        /// <exception cref="Exception"></exception>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            // Session token used by elastic tables to enable strong consistency
            // See https://learn.microsoft.com/power-apps/developer/data-platform/use-elastic-tables?tabs=webapi#sending-the-session-token
            if (!string.IsNullOrWhiteSpace(_sessionToken) && request.Method == HttpMethod.Get) {
                request.Headers.Add("MSCRM.SessionToken", _sessionToken);
            }

            // Set the access token using the function from the Config passed to the constructor
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await config.GetAccessToken());

            // Get the named HttpClient from the IHttpClientFactory
            var client = GetHttpClientFactory().CreateClient(WebAPIClientName);

            HttpResponseMessage response = await client.SendAsync(request);

            // Capture the current session token value
            // See https://learn.microsoft.com/power-apps/developer/data-platform/use-elastic-tables?tabs=webapi#getting-the-session-token
            if (response.Headers.Contains("x-ms-session-token"))
            {
                _sessionToken = response.Headers.GetValues("x-ms-session-token")?.FirstOrDefault()?.ToString();
            }

             //SampleGenerator.WriteHttpSample(request, response, BaseAddress, "H:\\temp\\GeneratedSamples");

            // Throw an exception if the request is not successful
            if (!response.IsSuccessStatusCode)
            {
                ServiceException exception = await ParseError(response);
                throw exception;
            }
            return response;
        }

        /// <summary>
        /// Processes requests with typed responses
        /// </summary>
        /// <typeparam name="T">The type derived from HttpResponseMessage</typeparam>
        /// <param name="request">The request</param>
        /// <returns></returns>
        public async Task<T> SendAsync<T>(HttpRequestMessage request) where T : HttpResponseMessage
        {
            HttpResponseMessage response = await SendAsync(request);

            // 'As' method is Extension of HttpResponseMessage see Extensions.cs
            return response.As<T>();
        }

        public static async Task<ServiceException> ParseError(HttpResponseMessage response)
        {
            string requestId = string.Empty;
            if (response.Headers.Contains("REQ_ID"))
            {
                requestId = response.Headers.GetValues("REQ_ID").FirstOrDefault();
            }

            var content = await response.Content.ReadAsStringAsync();
            ODataError? oDataError = null;

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
                // Error may not be in correct OData Error format, so keep trying...
            }

            if (oDataError?.Error != null)
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
                try
                {
                    ODataException oDataException = JsonSerializer.Deserialize<ODataException>(content);

                    ServiceException otherException = new(oDataException.Message)
                    {
                        Content = content,
                        ReasonPhrase = response.ReasonPhrase,
                        HttpStatusCode = response.StatusCode,
                        RequestId = requestId
                    };
                    return otherException;

                }
                catch (Exception)
                {

                }

                //When nothing else works
                ServiceException exception = new(response.ReasonPhrase)
                {
                    Content = content,
                    ReasonPhrase = response.ReasonPhrase,
                    HttpStatusCode = response.StatusCode,
                    RequestId = requestId
                };
                return exception;
            }
        }

        /// <summary>
        /// The BaseAddress property of the WebAPI httpclient.
        /// </summary>
        public Uri BaseAddress { get; }

        /// <summary>
        /// The recommended degree of parallelism for the connection.
        /// </summary>
        public int RecommendedDegreeOfParallelism {
            get {
                return _recommendedDegreeOfParallelism;
            }
        }

        /// <summary>
        /// The UserId of the connected user
        /// </summary>
        public Guid UserId
        {
            get
            {
                return _userId;
            }
        }

        /// <summary>
        /// The OrganizationId of the connected user
        /// </summary>
        public Guid OrganizationId
        {
            get
            {
                return _organizationId;
            }
        }

        /// <summary>
        /// The BusinessUnitId of the connected user
        /// </summary>
        public Guid BusinessUnitId
        {
            get
            {
                return _businessUnitId;
            }
        }

        ~Service() => Dispose(false);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }
                _disposedValue = true;
            }
        }
    }
}