using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AttributeQueryExpression
    {
        public MetadataFilterExpression Criteria { get; set; }
        public MetadataPropertiesExpression Properties { get; set; }
    }
}
