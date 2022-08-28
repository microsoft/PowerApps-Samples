using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Types
{
    public class SolutionComponentOption
    {
        public ImportDecision ImportDecision { get; set; }

        public JObject Component { get; set; }
    }
}
