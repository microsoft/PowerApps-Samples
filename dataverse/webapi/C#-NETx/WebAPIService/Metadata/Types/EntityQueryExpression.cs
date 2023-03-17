using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EntityQueryExpression
    {
        public AttributeQueryExpression AttributeQuery { get; set; }
        public MetadataFilterExpression Criteria { get; set; }
        public EntityKeyQueryExpression KeyQuery { get; set; }
        public LabelQueryExpression LabelQuery { get; set; }
        public MetadataPropertiesExpression Properties { get; set; }
        public RelationshipQueryExpression RelationshipQuery { get; set; }
    }
}
