using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public class CDSWebApiService : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly ServiceConfig config;
        /// <summary>
        /// The BaseAddresss property of the HttpClient.
        /// </summary>
        public Uri BaseAddress { get { return httpClient.BaseAddress; } }

        public CDSWebApiService(ServiceConfig config)
        {
            this.config = config;
            HttpMessageHandler messageHandler = new OAuthMessageHandler(
                config,
                new HttpClientHandler() { UseCookies = false }
                );
            httpClient = new HttpClient(messageHandler)
            {
                BaseAddress = new Uri(config.Url + $"/api/data/v{config.Version}/")
            };

            httpClient.BaseAddress = new Uri(config.Url + $"/api/data/v{config.Version}/");
            httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutInSeconds);
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            if (config.CallerObjectId != Guid.Empty)
            {
                httpClient.DefaultRequestHeaders.Add("CallerObjectId", config.CallerObjectId.ToString());
            }
        }

        /// <summary>
        /// Creates an entity and returns the URI
        /// </summary>
        /// <param name="entitySetName">The entity set name of the entity to create.</param>
        /// <param name="body">The JObject containing the data of the entity to create.</param>
        /// <returns></returns>
        public Uri PostCreate(string entitySetName, JObject body)
        {
            return PostCreateAsync(entitySetName, body).Result;
        }

        public async Task<Uri> PostCreateAsync(string entitySetName, JObject body)
        {
            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Post, entitySetName))
                {
                    message.Content = new StringContent(body.ToString());
                    message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    using (HttpResponseMessage response = await SendAsync(message))
                    {
                        return new Uri(response.Headers.GetValues("OData-EntityId").FirstOrDefault());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JObject Post(string path, JObject body, Dictionary<string, List<string>> headers = null)
        {
            return PostAsync(path, body, headers).Result;
        }

        public async Task<JObject> PostAsync(string path, JObject body, Dictionary<string, List<string>> headers = null)
        {
            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Post, path))
                {
                    message.Content = new StringContent(body.ToString());
                    message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    using (HttpResponseMessage response = await SendAsync(message))
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        if (string.IsNullOrEmpty(content))
                        {
                            return null;
                        }
                        return JObject.Parse(content);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Patch(Uri uri, JObject body, Dictionary<string, List<string>> headers = null)
        {
            PatchAsync(uri, body, headers).GetAwaiter().GetResult();
        }

        public async Task PatchAsync(Uri uri, JObject body, Dictionary<string, List<string>> headers = null)
        {
            try
            {
                using (var message = new HttpRequestMessage(new HttpMethod("PATCH"), uri))
                {
                    message.Content = new StringContent(body.ToString());
                    message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    if (headers != null)
                    {
                        foreach (KeyValuePair<string, List<string>> header in headers)
                        {
                            message.Headers.Add(header.Key, header.Value);
                        }
                    }
                    using (HttpResponseMessage response = await SendAsync(message))
                    {
                        response.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JToken Get(string path, Dictionary<string, List<string>> headers = null)
        {
            return GetAsync(path, headers).Result;
        }

        public async Task<JToken> GetAsync(string path, Dictionary<string, List<string>> headers = null)
        {
            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Get, path))
                {
                    if (headers != null)
                    {
                        foreach (KeyValuePair<string, List<string>> header in headers)
                        {
                            message.Headers.Add(header.Key, header.Value);
                        }
                    }

                    using (HttpResponseMessage response = await SendAsync(message, HttpCompletionOption.ResponseContentRead))
                    {
                        if (response.StatusCode != HttpStatusCode.NotModified)
                        {
                            return JToken.Parse(await response.Content.ReadAsStringAsync());
                        }
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="uri">The uri of the entity to delete</param>
        public void Delete(Uri uri, Dictionary<string, List<string>> headers = null)
        {
            DeleteAsync(uri, headers).GetAwaiter().GetResult();
        }

        public async Task DeleteAsync(Uri uri, Dictionary<string, List<string>> headers = null)
        {
            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Delete, uri))
                {
                    if (headers != null)
                    {
                        foreach (KeyValuePair<string, List<string>> header in headers)
                        {
                            message.Headers.Add(header.Key, header.Value);
                        }
                    }

                    HttpResponseMessage response = await SendAsync(message);
                    response.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Put(Uri uri, string property, string value)
        {
            PutAsync(uri, property, value).GetAwaiter().GetResult();
        }

        public async Task PutAsync(Uri uri, string property, string value)
        {
            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Put, $"{uri}/{property}"))
                {
                    var body = new JObject
                    {
                        ["value"] = value
                    };
                    message.Content = new StringContent(body.ToString());
                    message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    HttpResponseMessage response = await SendAsync(message);
                    response.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseHeadersRead,
            int retryCount = 0)
        {
            HttpResponseMessage response;
            try
            {
                //The request is cloned so it can be sent again.
                response = await httpClient.SendAsync(request.Clone(), httpCompletionOption);
            }
            catch (Exception)
            {
                throw;
            }

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode != 429)
                {
                    //Not a service protection limit error
                    throw ParseError(response);
                }
                else
                {
                    // Give up re-trying if exceeding the maxRetries
                    if (++retryCount >= config.MaxRetries)
                    {
                        throw ParseError(response);
                    }

                    int seconds;
                    //Try to use the Retry-After header value if it is returned.
                    if (response.Headers.Contains("Retry-After"))
                    {
                        seconds = int.Parse(response.Headers.GetValues("Retry-After").FirstOrDefault());
                    }
                    else
                    {
                        //Otherwise, use an exponential backoff strategy
                        seconds = (int)Math.Pow(2, retryCount);
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(seconds));

                    return await SendAsync(request, httpCompletionOption, retryCount);
                }
            }
            else
            {
                return response;
            }
        }

        /// <summary>
        /// Parses the Web API error
        /// </summary>
        /// <param name="response">The response that failed.</param>
        /// <returns></returns>
        private ServiceException ParseError(HttpResponseMessage response)
        {
            try
            {
                var errorObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                string message = errorObject["error"]["message"].Value<string>();
                int code = Convert.ToInt32(errorObject["error"]["code"].Value<string>(), 16);
                int statusCode = (int)response.StatusCode;
                string reasonPhrase = response.ReasonPhrase;

                return new ServiceException(code, statusCode, reasonPhrase, message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        ~CDSWebApiService()
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
            else
            {
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