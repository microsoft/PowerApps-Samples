using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AttributeRequiredLevelManagedProperty
    {
        public AttributeRequiredLevelManagedProperty(AttributeRequiredLevel value)
        {
            Value = value;
        }

        public AttributeRequiredLevel Value { get; set; }

        public bool CanBeChanged { get; set; }
        public string ManagedPropertyLogicalName { get; } = "canmodifyrequirementlevelsettings";
    }
}