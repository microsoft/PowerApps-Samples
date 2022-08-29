using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples.Types
{
    public class LayerDesiredOrder
    {
        LayerDesiredOrderType Type { get; set; }
        List<SolutionInfo> Solutions { get; set; }
    }
}
