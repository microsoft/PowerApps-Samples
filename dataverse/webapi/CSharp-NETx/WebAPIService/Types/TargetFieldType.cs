using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Types
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TargetFieldType
    {
        All,
        ValidForCreate,
        ValidForUpdate,
        ValidForRead
    }
}
