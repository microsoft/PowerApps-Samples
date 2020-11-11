using System.Collections.Generic;

namespace PowerApps.Samples
{
    public class LayerDesiredOrder
    {
        public LayerDesiredOrderType Type { get; set;}
        public List<SolutionInfo> Solutions { get; set; } = new List<SolutionInfo>();
    }
}
