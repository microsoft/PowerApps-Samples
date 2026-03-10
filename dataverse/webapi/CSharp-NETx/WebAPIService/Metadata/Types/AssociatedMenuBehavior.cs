using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AssociatedMenuBehavior
    {
        UseCollectionName, UseLabel, DoNotDisplay
    }
}