using Newtonsoft.Json.Linq;
using System.Text;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to upsert a record.
    /// </summary>
    public sealed class UpsertRequest : HttpRequestMessage
    {


        /// <summary>
        /// Initializes the UpsertRequest
        /// </summary>
        /// <param name="entityReference">A reference to a record. This should use alternate keys.</param>
        /// <param name="record">The data to create or update.</param>
        /// <param name="upsertBehavior">Control the upsert behavior.</param>
        public UpsertRequest(
            EntityReference entityReference,
            JObject record,
            UpsertBehavior upsertBehavior = UpsertBehavior.CreateOrUpdate)
        {
            Method = HttpMethod.Patch;
            RequestUri = new Uri(uriString: entityReference.Path, uriKind: UriKind.Relative);
            Content = new StringContent(
                    content: record.ToString(),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json");
            switch (upsertBehavior)
            {
                case UpsertBehavior.PreventCreate:
                    Headers.Add("If-Match", "*");
                    break;
                case UpsertBehavior.PreventUpdate:
                    Headers.Add("If-None-Match", "*");
                    break;
            }
        }
    }

    /// <summary>
    /// Specifies the behavior for an Upsert operation.
    /// </summary>
    public enum UpsertBehavior
    {
        CreateOrUpdate = 0,
        PreventUpdate = 1,
        PreventCreate = 2
    }
}
