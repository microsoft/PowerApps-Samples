using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ImageAttributeMetadata : AttributeMetadata
    {

        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.ImageAttributeMetadata";

        public AttributeTypeCode AttributeType { get; } = AttributeTypeCode.Virtual;

        public AttributeTypeDisplayName AttributeTypeName { get; } = new AttributeTypeDisplayName(AttributeTypeDisplayNameValues.ImageType);

        /// <summary>
        /// Whether the attribute is the primary image for the entity.
        /// </summary>
        public bool IsPrimaryImage { get; set; }

        /// <summary>
        /// The maximum height of the image.
        /// </summary>
        public short MaxHeight { get; set; }

        /// <summary>
        /// The maximum width of the image.
        /// </summary>
        public short MaxWidth { get; set; }

        /// <summary>
        /// The maximum size for the image.
        /// </summary>
        public int MaxSizeInKB { get; set; }

        /// <summary>
        /// Whether the image can store full-sized images
        /// </summary>
        public bool CanStoreFullImage { get; set; }
    }
}