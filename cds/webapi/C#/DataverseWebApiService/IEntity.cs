using System;
using System.Text.Json.Serialization;

namespace PowerApps.Samples
{
    public interface IEntity
    {
        Guid? Id { get; }
        public static string SetName { get; }
        public static string LogicalName { get; }
        EntityReference ToEntityReference();

        [JsonPropertyName("@odata.etag")]
        public string ETag { get; init; }


    }
}
