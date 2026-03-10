namespace PowerApps.Samples.Batch
{
    public class BatchRequest : HttpRequestMessage
    {
        /// <summary>
        /// BatchRequest constructor
        /// </summary>
        /// <param name="serviceBaseAddress">The Service.BaseAddress value.</param>
        public BatchRequest(Uri serviceBaseAddress)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(uriString: "$batch", uriKind: UriKind.Relative);
            Content = new MultipartContent("mixed", $"batch_{Guid.NewGuid()}");
            ServiceBaseAddress = serviceBaseAddress;
        }

        private bool continueOnError;
        private readonly Uri ServiceBaseAddress;

        /// <summary>
        /// Sets the Prefer: odata.continue-on-error request header for the request.
        /// </summary>
        public bool ContinueOnError
        {
            get
            {
                return continueOnError;
            }
            set
            {
                if (continueOnError != value)
                {
                    if (value)
                    {
                        Headers.Add("Prefer", "odata.continue-on-error");
                    }
                    else
                    {
                        Headers.Remove("Prefer");
                    }
                }
                continueOnError = value;
            }
        }

        /// <summary>
        /// Sets the ChangeSets to be included in the request.
        /// </summary>
        public List<ChangeSet> ChangeSets
        {

            set
            {
                value.ForEach(changeSet =>
                {
                    MultipartContent content = new("mixed", $"changeset_{Guid.NewGuid()}");

                    int count = 1;
                    changeSet.Requests.ForEach(request =>
                    {
                        HttpMessageContent messageContent = ToMessageContent(request);
                        messageContent.Headers.Add("Content-ID", count.ToString());

                        content.Add(messageContent);

                        count++;
                    });
                    //Add to the content
                    ((MultipartContent)Content).Add(content);

                });
            }
        }

        /// <summary>
        /// Sets any requests to be sent outside of any ChangeSet
        /// </summary>
        public List<HttpRequestMessage> Requests
        {
            set
            {
                value.ForEach(request =>
                {
                    //Add to the content
                    ((MultipartContent)Content).Add(ToMessageContent(request));

                });
            }
        }

        /// <summary>
        /// Converts a HttpRequestMessage to HttpMessageContent
        /// </summary>
        /// <param name="request">The HttpRequestMessage to convert.</param>
        /// <returns>HttpMessageContent with the correct headers.</returns>
        private HttpMessageContent ToMessageContent(HttpRequestMessage request)
        {

            //Relative URI is not allowed with MultipartContent
            request.RequestUri = new Uri(
                baseUri: ServiceBaseAddress,
                relativeUri: request.RequestUri.ToString());

            if (request.Content != null)
            {
                if (request.Content.Headers.Contains("Content-Type"))
                {
                    request.Content.Headers.Remove("Content-Type");
                }
                request.Content.Headers.Add("Content-Type", "application/json;type=entry");
            }

            HttpMessageContent messageContent = new(request);

            if (messageContent.Headers.Contains("Content-Type"))
            {
                messageContent.Headers.Remove("Content-Type");
            }
            messageContent.Headers.Add("Content-Type", "application/http");
            messageContent.Headers.Add("Content-Transfer-Encoding", "binary");

            return messageContent;

        }
    }
}
