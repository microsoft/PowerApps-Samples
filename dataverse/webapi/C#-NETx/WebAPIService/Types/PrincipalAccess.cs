using Newtonsoft.Json.Linq;

namespace PowerApps.Samples.Types
{
    /// <summary>
    /// Contains access rights information for the security principal (user or team).
    /// </summary>
    public class PrincipalAccess
    {
        /// <summary>
        /// Gets or sets the access rights of the security principal (user or team).
        /// </summary>
        public AccessRights AccessMask { get; set; }

        /// <summary>
        /// Gets or sets the security principal (user or team).
        /// </summary>
        public JObject? Principal { get; set; }
    }
}
