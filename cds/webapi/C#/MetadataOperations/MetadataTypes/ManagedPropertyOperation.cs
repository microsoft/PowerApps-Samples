using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ManagedPropertyOperation
    {
        None,
        Create,
        Update,
        CreateUpdate,
        Delete,
        UpdateDelete = 6,
        All
    }
}