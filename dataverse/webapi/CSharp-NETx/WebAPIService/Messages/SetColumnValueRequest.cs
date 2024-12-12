using System.Text;
using System.Text.Json;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to set a column value
    /// </summary>
    /// <typeparam name="T">The type of the column value to set</typeparam>
    public sealed class SetColumnValueRequest<T> : HttpRequestMessage
    {
        /// <summary>
        /// Initializes the SetColumnValueRequest
        /// </summary>
        /// <param name="entityReference">A reference to the record that has the column.</param>
        /// <param name="propertyName">The name of the column</param>
        /// <param name="value">The value to set</param>
        public SetColumnValueRequest(EntityReference entityReference, string propertyName, T value)
        {
            Method = HttpMethod.Put;
            RequestUri = new Uri(
                uriString: $"{entityReference.Path}/{propertyName}", 
                uriKind: UriKind.Relative);
            Content = new StringContent(
                content: $"{{\"value\": {JsonSerializer.Serialize(value)}}}", 
                encoding: Encoding.UTF8, 
                mediaType: "application/json");
        }
    }
}