using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the data from the a DownloadBlockRequest
    /// </summary>
    public sealed class DownloadBlockResponse : HttpResponseMessage
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
        public byte[] Data
        {
            get
            {
                return (byte[])_jObject[nameof(Data)];
            }
        }
    }
}
