using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Messages
{

    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the CreateMultipleRequest
    /// </summary>
    public class CreateMultipleResponse : HttpResponseMessage
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
        /// The ID values of the created records
        /// </summary>
        public Guid[] Ids => _jObject[nameof(Ids)].ToObject<Guid[]>();
    }
}
