using Newtonsoft.Json;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// This class is used to populate the string attribute with json format (contoso_energyconsumption)
    /// </summary>
    public class EnergyConsumption
    {
        [JsonProperty("power")]
        public int Power { get; set; }

        [JsonProperty("powerUnit")]
        public string? PowerUnit { get; set; }

        [JsonProperty("voltage")]
        public int Voltage { get; set; }

        [JsonProperty("voltageUnit")]
        public string? VoltageUnit { get; set; }
    }
}
