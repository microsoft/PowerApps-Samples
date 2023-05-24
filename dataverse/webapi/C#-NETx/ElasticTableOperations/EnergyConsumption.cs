using Newtonsoft.Json;

namespace ElasticTableOperations
{
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
