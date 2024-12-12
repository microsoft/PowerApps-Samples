﻿using Newtonsoft.Json;
using PowerApps.Samples.Metadata.Types;

namespace PowerApps.Samples.Metadata.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the RetrieveEntityDefinitionRequest
    /// </summary>
    public sealed class RetrieveEntityDefinitionResponse : HttpResponseMessage
    {
        /// <summary>
        /// Metadata for the requested entity.
        /// </summary>
        public EntityMetadata EntityMetadata => JsonConvert.DeserializeObject<EntityMetadata>(Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
}
