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
        /// <returns>The Uri for the created entity record.</returns>
        public Uri PostCreate(string entitySetName, object body)
        {
            try
            {
                return PostCreateAsync(entitySetName, body).GetAwaiter().GetResult();
            }
            catch (Exception)
            {

                throw;
            }
        }
      
        /// <summary>
        /// Creates an entity asynchronously and returns the URI
        /// </summary>
        /// <param name="entitySetName">The entity set name of the entity to create.</param>
        /// <param name="body">The JObject containing the data of the entity to create.</param>
        /// <returns>The Uri for the created entity record.</returns>
        public async Task<Uri> PostCreateAsync(string entitySetName, object body)
        {
            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Post, entitySetName))
                {
                    message.Content = new StringContent(JObject.FromObject(body).ToString());
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


        /// <summary>
        /// Posts a payload to the specified resource.
        /// </summary>
        /// <param name="path">The path to the resource</param>
        /// <param name="body">The payload to send.</param>
        /// <param name="headers">Any headers to control optional behaviors.</param>
        /// <returns>The response from the request.</returns>
        public JObject Post(string path, object body, Dictionary<string, List<string>> headers = null)
        {
            return PostAsync(path, body, headers).GetAwaiter().GetResult();;
        }

        /// <summary>
        /// Posts a payload to the specified resource asynchronously.
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="body">The payload to send.</param>
        /// <param name="headers">Any headers to control optional behaviors.</param>
        /// <returns>The response from the request.</returns>
        public async Task<JObject> PostAsync(string path, object body, Dictionary<string, List<string>> headers = null)
        {
            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Post, path))
                {
                    message.Content = new StringContent(JObject.FromObject(body).ToString());
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

        /// <summary>
        /// Sends a PATCH request to update a resource.
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="body">The payload to send to update the resource.</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        public void Patch(Uri uri, object body, Dictionary<string, List<string>> headers = null)
        {
            PatchAsync(uri, body, headers).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends a PATCH request to update a resource asynchronously
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="body">The payload to send to update the resource.</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        /// <returns>Task</returns>
        public async Task PatchAsync(Uri uri, object body, Dictionary<string, List<string>> headers = null)
        {
            try
            {
                using (var message = new HttpRequestMessage(new HttpMethod("PATCH"), uri))
                {
                    message.Content = new StringContent(JObject.FromObject(body).ToString());
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

        /// <summary>
        /// Retrieves data from a specified resource.
        /// </summary>
        /// <param name="path">The path to the resource</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        /// <returns>The response from the request.</returns>
        public JToken Get(string path, Dictionary<string, List<string>> headers = null)
        {
            return GetAsync(path, headers).GetAwaiter().GetResult();;
        }

        /// <summary>
        /// Retrieves data from a specified resource asychronously.
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        /// <returns>The response to the request.</returns>
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
        /// <param name="path">The path to the resource to delete</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        public void Delete(string path, Dictionary<string, List<string>> headers = null)
        {
            DeleteAsync(path, headers).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes an entity asychronously
        /// </summary>
        /// <param name="path">The path to the resource to delete.</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        /// <returns>Task</returns>
        public async Task DeleteAsync(string path, Dictionary<string, List<string>> headers = null)
        {
            if (!path.StartsWith(BaseAddress.ToString()))
            {
                path = BaseAddress + path;
            }

            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Delete, new Uri(path)))
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

        /// <summary>
        /// Updates a property of an entity
        /// </summary>
        /// <param name="path">The path to the entity.</param>
        /// <param name="property">The name of the property to update.</param>
        /// <param name="value">The value to set.</param>
        public void Put(string path, string property, string value)
        {
            PutAsync(path, property, value).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Updates a property of an entity asychronously
        /// </summary>
        /// <param name="path">The path to the entity.</param>
        /// <param name="property">The name of the property to update.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>Task</returns>
        public async Task PutAsync(string path, string property, string value)
        {
            if (!path.StartsWith(BaseAddress.ToString()))
            {
                path = BaseAddress + path;
            }

            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Put, new Uri($"{path}/{property}")))
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

        #region new methods

        /// <summary>
        /// Gets a typed response from a specified resource.
        /// </summary>
        /// <typeparam name="T">The type of response</typeparam>
        /// <param name="path">The path to the resource.</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        /// <returns>The typed response from the request.</returns>
        public T Get<T>(string path, Dictionary<string, List<string>> headers = null)
        {
            return GetAsync<T>(path, headers).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets a typed response from a specified resource asychronously
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="path">The path to the resource.</param>
        /// <param name="headers"></param>
        /// <returns>Any custom headers to control optional behaviors.</returns>
        public async Task<T> GetAsync<T>(string path, Dictionary<string, List<string>> headers = null)
        {
            return (await GetAsync(path, headers)).ToObject<T>();
        }

        /// <summary>
        /// Sends a POST to create a typed entity and retrieves the created entity.
        /// </summary>
        /// <typeparam name="T">The path to the entityset and any query string parameters to specify the properties to return.</typeparam>
        /// <param name="path">The path to the entityset.</param>
        /// <param name="body">The payload to send to create the entity record.</param>
        /// <param name="headers">Any headers to control optional behaviors.</param>
        /// <returns>The typed entity record created.</returns>
        public T PostGet<T>(string path, object body, Dictionary<string, List<string>> headers = null)
        {
            return PostGetAsync<T>(path, body, headers).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sents a POST to create an entity and retrieves the created entity.
        /// </summary>
        /// <param name="path">The path to the entityset and any query string parameters to specify the properties to return.</param>
        /// <param name="body">The payload to send to create the entity record.</param>
        /// <param name="headers">Any headers to control optional behaviors.</param>
        /// <returns>An object containing data for the created entity.</returns>
        public JObject PostGet(string path, object body, Dictionary<string, List<string>> headers = null)
        {
            return PostGetAsync(path, body, headers).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sents a POST to create a typed entity and retrieves the created entity asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of entity to create and retrieve.</typeparam>
        /// <param name="path">The path to the entityset and any query string parameters to specify the properties to return.</param>
        /// <param name="body">The payload to send to create the entity record.</param>
        /// <param name="headers">Any headers to control optional behaviors.</param>
        /// <returns>The typed entity record created.</returns>
        public async Task<T> PostGetAsync<T>(string path, object body, Dictionary<string, List<string>> headers = null)
        {
            return (await PostGetAsync(path, body, headers)).ToObject<T>();
        }

        /// <summary>
        /// Sents a POST to create an entity and retrieves the created entity asynchronously.
        /// </summary>
        /// <param name="path">The path to the entityset.</param>
        /// <param name="body">The payload to send to create the entity record.</param>
        /// <param name="headers">Any headers to control optional behaviors.</param>
        /// <returns>An object containing data for the created entity.</returns>
        public async Task<JObject> PostGetAsync(string path, object body, Dictionary<string, List<string>> headers = null)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, List<string>>();
            }
            headers.Add("Prefer", new List<string> { "return=representation" });
            return await PostAsync(path, body, headers);
        }

        /// <summary>
        /// Used to update metadata
        /// </summary>
        /// <param name="path">The path to the metadata resource.</param>
        /// <param name="metadataItem">The payload to send to update the metadata resource.</param>
        /// <param name="mergeLabels">Whether any existing language labels should be merged.</param>
        public void Put(string path, JObject metadataItem, bool mergeLabels)
        {
            PutAsync(path, metadataItem, mergeLabels).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Used to update metadata asychronously.
        /// </summary>
        /// <param name="path">The path to the metadata resource.</param>
        /// <param name="metadataItem">The payload to send to update the metadata resource.</param>
        /// <param name="mergeLabels">Whether any existing language labels should be merged.</param>
        public async Task PutAsync(string path, JObject metadataItem, bool mergeLabels)
        {
            try
            {
                using (var message = new HttpRequestMessage(HttpMethod.Put, path))
                {
                    if (mergeLabels)
                    {
                        message.Headers.Add("MSCRM.MergeLabels", "true");
                        message.Headers.Add("Consistency", "Strong");
                    }

                    message.Content = new StringContent(metadataItem.ToString());
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

        #endregion new methods

        /// <summary>
        /// Sends all requests with retry capabilities
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <param name="httpCompletionOption">Indicates if HttpClient operations should be considered completed either as soon as a response is available, or after reading the entire response message including the content.</param>
        /// <param name="retryCount">The number of retry attempts</param>
        /// <returns>The response for the request.</returns>
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
                int code = 0;
                string message = "no content returned",
                       content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                
                if (content.Length > 0)
                {
                    var errorObject = JObject.Parse(content);
                    message = errorObject["error"]["message"].Value<string>();
                    code = Convert.ToInt32(errorObject["error"]["code"].Value<string>(), 16);
                }
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