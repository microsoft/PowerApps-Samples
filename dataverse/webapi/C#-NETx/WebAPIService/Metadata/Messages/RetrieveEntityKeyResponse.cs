using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using PowerApps.Samples.Metadata.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the RetrieveEntityKeyRequest
    /// </summary>
    /// <typeparam name="T">The type of response.</typeparam>
    public sealed class RetrieveEntityKeyResponse : HttpResponseMessage
    {
        /// <summary>
        /// Metadata for the requested entity key.
        /// </summary>
        public EntityKeyMetadata EntityKeyMetadata => JsonConvert.DeserializeObject<EntityKeyMetadata>(Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
}
