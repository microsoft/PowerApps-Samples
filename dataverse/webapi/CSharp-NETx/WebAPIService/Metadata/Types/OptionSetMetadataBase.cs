using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class OptionSetMetadataBase : MetadataBase
    {
        

        /// <summary>
        /// A description for the option set.
        /// </summary>
        public Label Description { get; set; }

        /// <summary>
        /// A label containing the display name for the global option set.
        /// </summary>
        public Label DisplayName { get; set; }

        /// <summary>
        /// TODO: Something related to Virtual Entities
        /// </summary>
        public string ExternalTypeName { get; set; }

        /// <summary>
        /// A string identifying the solution version that the solution component was added in.
        /// </summary>
        public string IntroducedVersion { get; set; }

        /// <summary>
        /// Whether the option set is customizable.
        /// </summary>
        public BooleanManagedProperty IsCustomizable { get; set; }

        /// <summary>
        /// Whether the option set is a custom option set.
        /// </summary>
        public bool? IsCustomOptionSet { get; set; }

        /// <summary>
        /// Whether the option set is a global option set.
        /// </summary>
        public bool? IsGlobal { get; set; }

        /// <summary>
        /// Whether the option set is part of a managed solution.
        /// </summary>
        public bool? IsManaged { get; set; }

        /// <summary>
        /// The name of a global option set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of option set.
        /// </summary>
        public OptionSetType OptionSetType { get; set; }
    }
}