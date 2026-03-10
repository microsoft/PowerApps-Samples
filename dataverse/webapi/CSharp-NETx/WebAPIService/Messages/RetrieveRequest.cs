using System;

namespace PowerApps.Samples.Messages
{

    /// <summary>
    /// Contains the data to retrieve a record
    /// </summary>
    public sealed class RetrieveRequest : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the RetrieveRequest
        /// </summary>
        /// <param name="entityReference">A reference to the record to retrieve</param>
        /// <param name="query">The query parameters to determine the data to return.</param>
        /// <param name="includeAnnotations">Whether to include annotations in the response.</param>
        /// <param name="eTag">The current ETag value to compare.</param>
        public RetrieveRequest(EntityReference entityReference, string? query, bool includeAnnotations = false, string? eTag = null, string? partitionid = null)
        {
            Method = HttpMethod.Get;

            string parameters;

            if (!string.IsNullOrWhiteSpace(partitionid))
            {
                parameters = $"?partitionId={partitionid}";
                if (query.StartsWith("?"))
                {
                    query = string.Concat("&", query.AsSpan(1));
                }
                parameters += query;
            }
            else
            {
                parameters = query;
            }

            RequestUri = new Uri(
                uriString: $"{entityReference.Path}{parameters}",
                uriKind: UriKind.Relative);
            if (includeAnnotations)
            {
                Headers.Add("Prefer", "odata.include-annotations=\"*\"");
            }
            if (eTag != null)
            {
                // Don't return record if it is the same on the server
                Headers.Add("If-None-Match", eTag);
            }
        }
    }
}
