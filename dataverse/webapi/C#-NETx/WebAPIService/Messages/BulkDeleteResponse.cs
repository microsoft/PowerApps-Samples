using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    public sealed class BulkDeleteResponse : HttpResponseMessage
    {

        //Provides JObject for property getters
        private JObject _jObject
        {
            get
            {
                return JObject.Parse(Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }
        }

        /// <summary>
        /// The contents of the downloaded file.
        /// </summary>
        public Guid JobId
        {
            get
            {
                return (Guid)_jObject[nameof(JobId)];
            }
        }
    }
}
