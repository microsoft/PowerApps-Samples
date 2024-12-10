using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OwnershipTypes
    {
        None,
        UserOwned,
        TeamOwned,
        BusinessOwned = 4,
        OrganizationOwned = 8,
        BusinessParented = 16
    }
}