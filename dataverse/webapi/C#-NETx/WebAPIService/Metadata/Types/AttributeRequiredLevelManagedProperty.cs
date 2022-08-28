using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
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