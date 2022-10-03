using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MetadataFilterExpression
    {
        public MetadataFilterExpression() { }
        public MetadataFilterExpression(LogicalOperator filterOperator)
        {
            FilterOperator = filterOperator;
        }   

        public List<MetadataConditionExpression> Conditions { get; set; }
        public LogicalOperator FilterOperator { get; set; }
        public List<MetadataFilterExpression> Filters { get; set; }
    }
}
