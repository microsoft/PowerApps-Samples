using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace PowerApps.Samples.Metadata.Types
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SecurityPrivilegeMetadata
    {
        [JsonProperty("@odata.type")]
        public string ODataType { get; } = "Microsoft.Dynamics.CRM.SecurityPrivilegeMetadata";

        /// <summary>
        /// Whether the privilege can be basic access level.
        /// </summary>
        public bool CanBeBasic { get; set; }

        /// <summary>
        /// Whether the privilege can be deep access level.
        /// </summary>
        public bool CanBeDeep { get; set; }

        /// <summary>
        /// Whether the privilege can be global access level.
        /// </summary>
        public bool CanBeGlobal { get; set; }

        /// <summary>
        /// Whether the privilege can be local access level.
        /// </summary>
        public bool CanBeLocal { get; set; }

        /// <summary>
        /// Whether the privilege for an external party can be basic access level.
        /// </summary>
        public bool CanBeEntityReference { get; set; }

        /// <summary>
        /// Whether the privilege for an external party can be parent access level.
        /// </summary>
        public bool CanBeParentEntityReference { get; set; }

        /// <summary>
        /// The name of the privilege.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the privilege.
        /// </summary>
        public Guid PrivilegeId { get; set; }

        /// <summary>
        /// The type of the privilege.
        /// </summary>
        public PrivilegeType PrivilegeType { get; set; }
    }
}