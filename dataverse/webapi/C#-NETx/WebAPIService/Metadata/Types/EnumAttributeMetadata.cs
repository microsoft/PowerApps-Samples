using Newtonsoft.Json;
using System;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EnumAttributeMetadata : AttributeMetadata
    {


        public int? DefaultFormValue { get; set; }

        public OptionSetMetadata GlobalOptionSet { get; set; }

        private string globalOptionSetId;
        /// <summary>
        /// Use this property to set the GUID value of a global optionset you want to use.
        /// </summary>
        [JsonProperty("GlobalOptionSet@odata.bind", NullValueHandling = NullValueHandling.Ignore)]
        public string SetGlobalOptionSetId
        {
            get
            {
                return (globalOptionSetId == null) ? null : $"/GlobalOptionSetDefinitions({globalOptionSetId})";
            }
            set
            {
                globalOptionSetId = value;
            }
        }

        public OptionSetMetadata OptionSet { get; set; }
    }
}