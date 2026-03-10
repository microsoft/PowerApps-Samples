using Newtonsoft.Json;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AssociatedMenuConfiguration
    {
        /// <summary>
        /// The behavior of the associated menu for an entity relationship.
        /// </summary>
        public AssociatedMenuBehavior Behavior { get; set; }

        /// <summary>
        /// The structure that contains extra data.
        /// </summary>
        public AssociatedMenuGroup Group { get; set; }

        /// <summary>
        /// The label for the associated menu.
        /// </summary>
        public Label Label { get; set; }

        /// <summary>
        /// The order for the associated menu.
        /// </summary>
        public int? Order { get; set; }

        public bool? IsCustomizable { get; set; }
        public string Icon { get; set; }
        public Guid ViewId { get; set; }
        public bool? AvailableOffline { get; set; }
        public string MenuId { get; set; }
        public string QueryApi { get; set; }
    }
}