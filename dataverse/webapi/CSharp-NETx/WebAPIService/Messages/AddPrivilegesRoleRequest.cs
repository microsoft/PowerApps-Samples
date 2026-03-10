using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Types;

namespace PowerApps.Samples.Messages
{
    /// <summary>
    /// Contains the data to add privileges to a role
    /// </summary>
    public sealed class AddPrivilegesRoleRequest : HttpRequestMessage
    {
       
        private JObject _content;

        /// <summary>
        /// Initializes the AddPrivilegesRoleRequest
        /// </summary>
        /// <param name="roleId">The Id of the role to add the privileges to.</param>
        public AddPrivilegesRoleRequest(Guid roleId, List<RolePrivilege> privileges)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: $"roles({roleId})/Microsoft.Dynamics.CRM.AddPrivilegesRole", 
                uriKind: UriKind.Relative);
            _content = new JObject
            {
                ["Privileges"] = JArray.FromObject(privileges)
            };
            Content = new StringContent(
                    content: JsonConvert.SerializeObject(_content),
                    encoding: System.Text.Encoding.UTF8,
                    mediaType: "application/json");
        }
    }
}
