using Newtonsoft.Json;
using System.Text;
using System.Web;

namespace PowerApps.Samples
{
    /// <summary>
    /// Contains method to generate Http samples in Markdown
    /// </summary>
    public class SampleGenerator
    {
        /// <summary>
        /// Generates an Http sample
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="response">The response</param>
        /// <param name="baseAddress">The service base address</param>
        /// <param name="outputFolder">The location to save the markdown text of the sample.</param>
        /// <returns></returns>
        public static async Task WriteHttpSample(HttpRequestMessage request, HttpResponseMessage response, Uri baseAddress, string outputFolder)
        {
            string[] unwantedRequestHeaders = new string[] { "Authorization", "User-Agent", "CRM.ServiceId" };
            string[] unwantedResponseHeaders = new string[] {
                "Cache-Control",
                "x-ms-service-request-id",
                "Set-Cookie",
                "Strict-Transport-Security",
                "REQ_ID",
                "AuthActivityId",
                "x-ms-dop-hint",
                "x-ms-ratelimit-time-remaining-xrm-requests",
                "x-ms-ratelimit-burst-remaining-xrm-requests",
                "x-ms-utilization-percent",
                "X-Source",
                "Public",
                "Date",
                "CRM.ServiceId",
                "Location" // needed in some cases.

            };

            StringBuilder sb = new();
            sb.AppendLine("**Request**");
            sb.AppendLine();
            sb.AppendLine("```http");
            sb.AppendLine($"{request.Method.Method} [Organization Uri]{request.RequestUri.AbsolutePath + HttpUtility.UrlDecode(request.RequestUri.Query)}");
            foreach (var item in request.Headers)
            {
                if (!unwantedRequestHeaders.Contains(item.Key))
                {
                    string value = string.Join("; ", item.Value);
                    sb.AppendLine($"{item.Key}: {value}");
                }
            }
            // Add example Authorization header
            sb.AppendLine("Authorization: Bearer <access token>");

            if (request.Content != null)
            {
                foreach (var item in request.Content.Headers)
                {
                    if (!unwantedRequestHeaders.Contains(item.Key))
                    {
                        string value = string.Join("; ", item.Value);
                        sb.AppendLine($"{item.Key}: {value}");
                    }
                }


                if (request.Content is StringContent)
                {
                    sb.AppendLine();
                    sb.AppendLine(await request.Content.ReadAsStringAsync());
                }
                if (request.Content is MultipartContent)
                {

                    sb.AppendLine();
                    var provider = new MultipartMemoryStreamProvider();

                    await request.Content.ReadAsMultipartAsync(provider);

                    sb.AppendLine(await request.Content.ReadAsStringAsync());

                }
                if (request.Content is ByteArrayContent && !(request.Content is StringContent))
                {
                    sb.AppendLine();
                    sb.AppendLine("< byte[] content ommited for brevity>");
                }
                if (request.Content is StreamContent)
                {
                    sb.AppendLine();
                    sb.AppendLine("< stream content ommited for brevity>");
                }


            }
            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("**Response**");
            sb.AppendLine();
            sb.AppendLine("```http");
            sb.AppendLine($"HTTP/1.1 {(int)response.StatusCode} {response.StatusCode}");
            foreach (var item in response.Headers)
            {
                if (!unwantedResponseHeaders.Contains(item.Key))
                {
                    string value = string.Join("; ", item.Value);

                    if (item.Key == "OData-EntityId")
                    {
                        value = value.Replace(baseAddress.ToString(), "[Organization Uri]/api/data/v9.2/");
                    }

                    sb.AppendLine($"{item.Key}: {value}");
                }
            }

            if (response.Content != null)
            {

                if (response.Content.IsMimeMultipartContent())
                {
                    string unformattedContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(unformattedContent))
                    {
                        sb.AppendLine();
                        unformattedContent = unformattedContent.Replace(baseAddress.ToString(), "[Organization Uri]/api/data/v9.2/");
                        sb.AppendLine(unformattedContent);
                    }
                }
                else
                {
                    string unformattedContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(unformattedContent))
                    {
                        sb.AppendLine();
                        unformattedContent = unformattedContent.Replace(baseAddress.ToString(), "[Organization Uri]/api/data/v9.2/");
                        try
                        {
                            sb.AppendLine(JsonPrettify(unformattedContent));
                        }
                        catch (Exception)
                        {
                            sb.AppendLine("< byte[] content removed for brevity. >");
                        }

                        
                    }
                }


            }
            sb.AppendLine("```");
            sb.AppendLine();

            try
            {
                string date = DateTime.Now.ToString("yyyy-M-dd-HH-mm-ss-fffffff");
                string url = request.RequestUri.LocalPath.Replace("/", "-");
                url = url.Replace("-api-data-v9.2", "");

                File.WriteAllText($"{outputFolder}\\{date}-{request.Method.Method}{url}.txt", sb.ToString());
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static string JsonPrettify(string json)
        {
            using StringReader stringReader = new(json);
            using StringWriter stringWriter = new();
            JsonTextReader jsonReader = new(stringReader);
            JsonTextWriter jsonWriter = new(stringWriter) { Formatting = Formatting.Indented };
            jsonWriter.WriteToken(jsonReader);
            return stringWriter.ToString();
        }
    }
}
