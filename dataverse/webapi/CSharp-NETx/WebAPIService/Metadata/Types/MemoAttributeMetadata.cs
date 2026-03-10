using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MemoAttributeMetadata : AttributeMetadata
    {

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.MemoAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Memo;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.MemoType);

        /// <summary>
        /// The format options for the memo attribute.
        /// </summary>
        public StringFormat Format { get; set; }

        /// <summary>
        /// The input method editor (IME) mode for the attribute.
        /// </summary>
        public ImeMode ImeMode { get; set; }

        /// <summary>
        /// The maximum length for the attribute.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Whether the attribute supports localizable values.
        /// </summary>
        public bool IsLocalizable { get; set; }
    }
}