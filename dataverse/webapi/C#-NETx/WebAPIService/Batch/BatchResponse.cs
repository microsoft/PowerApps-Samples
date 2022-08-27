namespace PowerApps.Samples.Batch
{
    public class BatchResponse : HttpResponseMessage
    {

        /// <summary>
        /// Gets the responses from the batch operation
        /// </summary>
        public List<HttpResponseMessage> HttpResponseMessages
        {
            get
            {
                List<HttpResponseMessage> httpResponseMessages = new();

                if (Content != null)
                {
                    httpResponseMessages.AddRange(collection: ParseMultipartContent(Content).GetAwaiter().GetResult());
                }

                return httpResponseMessages;
            }
        }
        /// <summary>
        /// Processes the Multi-part content returned from the batch into a list of responses.
        /// </summary>
        /// <param name="content">The Content of the response.</param>
        /// <returns></returns>
        private static async Task<List<HttpResponseMessage>> ParseMultipartContent(HttpContent content)
        {
            MultipartMemoryStreamProvider batchResponseContent = await content.ReadAsMultipartAsync();

            List<HttpResponseMessage> responses = new();

            if (batchResponseContent?.Contents != null)
            {
                batchResponseContent.Contents.ToList().ForEach(async httpContent =>
                {

                    //This is true for changesets
                    if (httpContent.IsMimeMultipartContent())
                    {
                        //Recursive call
                        responses.AddRange(await ParseMultipartContent(httpContent));
                    }

                    //This is for individual responses outside of a change set.
                    else
                    {
                        //Must change Content-Type for ReadAsHttpResponseMessageAsync method to work.
                        httpContent.Headers.Remove("Content-Type");
                        httpContent.Headers.Add("Content-Type", "application/http;msgtype=response");

                        HttpResponseMessage httpResponseMessage = await httpContent.ReadAsHttpResponseMessageAsync();

                        if (httpResponseMessage != null)
                        {
                            responses.Add(httpResponseMessage);
                        }
                    }
                });
            }

            return responses;
        }

    }
}
