using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        /// <summary>
        /// Gets organization data for the specified user.
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <param name="cloud">The Cloud enum value that corresponds to the data center if known. Otherwise Cloud.Unknown</param>
        /// <returns>A List of OrganizationDetail records</returns>
        public static List<OrganizationDetail> GetAllOrganizations(string userName, string password, Cloud cloud)
        {
            //If Cloud.Unknown is used, choose Commercial
            if (cloud == Cloud.Unknown)
            {
                cloud = Cloud.Commercial;
            }
            // Get data from Organization
            return GetOrganizationsForCloud(userName, password, cloud);
        }

        /// <summary>
        /// Get organization data for a specific known region only
        /// </summary>
        /// <param name="creds">User's credentials.</param>
        /// <param name="cloud">Target Cloud.</param>
        /// <returns></returns>
        public static List<OrganizationDetail> GetOrganizationsForCloud(string userName, string password, Cloud cloud)
        {
            if (cloud == Cloud.Unknown)
            {
                throw new ArgumentOutOfRangeException("Cloud.Unknown cannot be used as a parameter for this method.");
            }

            //Get the Cloud URL from the Description Attribute applied for the Cloud member
            var type = typeof(Cloud);
            var memInfo = type.GetMember(cloud.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            Uri targeturl = new Uri(((DescriptionAttribute)attributes[0]).Description);

            // Set up user credentials
            var creds = new System.ServiceModel.Description.ClientCredentials();
            creds.UserName.UserName = userName;
            creds.UserName.Password = password;
            Uri appReplyUri = new Uri(redirectUrl);

            // Call to get organizations from global discovery
            var organizations = CrmServiceClient.DiscoverGlobalOrganizations(
                  discoveryServiceUri:targeturl, 
                  clientCredentials: creds, 
                  user: null, 
                  clientId: clientId,
                  redirectUri: appReplyUri, 
                  tokenCachePath: "",
                  isOnPrem: false,
                  authority: string.Empty, 
                  promptBehavior: PromptBehavior.Auto);

            return organizations.ToList();
        }

    }

    /// <summary>
    /// An enum for the known Clouds
    /// </summary>
    public enum Cloud
    {
        [Description("Unknown")]
        Unknown,
        [Description("https://globaldisco.crm.dynamics.com/api/discovery/v2.0/Instances")]
        Commercial,
        [Description("https://globaldisco.crm9.dynamics.com/api/discovery/v2.0/Instances")]
        GCC,
        [Description("https://globaldisco.crm.microsoftdynamics.us/api/discovery/v2.0/Instances")]
        USG,
        [Description("https://globaldisco.crm.appsplatform.us/api/discovery/v2.0/Instances")]
        DOD,
        [Description("https://globaldisco.crm.dynamics.cn/api/discovery/v2.0/Instances")]
        CHINA
    }
}
