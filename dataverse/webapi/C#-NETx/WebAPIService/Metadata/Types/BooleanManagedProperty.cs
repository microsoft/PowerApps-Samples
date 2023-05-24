using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class BooleanManagedProperty
    {
        public BooleanManagedProperty() { }

        public BooleanManagedProperty(bool value) { 
            Value = value;
        }

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.BooleanManagedProperty";
        public bool Value { get; set; }
        public bool CanBeChanged { get; set; }
        public string ManagedPropertyLogicalName { get; set; }      
    }
}