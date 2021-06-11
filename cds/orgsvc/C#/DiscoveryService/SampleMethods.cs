using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        /// <summary>
        /// Gets organization data for user in all regions when data center is unknown
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <param name="dataCenter">The DataCenter enum value that corresponds to the data center if known. Otherwise DataCenter.Unknown</param>
        /// <returns>A List of OrganizationDetail records</returns>
        public static List<OrganizationDetail> GetAllOrganizations(string userName, string password, DataCenter dataCenter)
        {
            //If DataCenter.Unknown is used, choose Commercial
            if (dataCenter == DataCenter.Unknown)
            {
                dataCenter = DataCenter.Commercial;
            }
            // Get data from Organization
            return GetOrganizationsForDataCenter(userName, password, dataCenter);
        }

        /// <summary>
        /// Get organization data for a specific known region only
        /// </summary>
        /// <param name="creds">User's credentials.</param>
        /// <param name="dataCenter">Target datacenter.</param>
        /// <returns></returns>
        public static List<OrganizationDetail> GetOrganizationsForDataCenter(string userName, string password, DataCenter dataCenter)
        {
            if (dataCenter == DataCenter.Unknown)
            {
                throw new ArgumentOutOfRangeException("DataCenter.Unknown cannot be used as a parameter for this method.");
            }

            //Get the DataCenter URL from the Description Attribute applied for the DataCenter member
            var type = typeof(DataCenter);
            var memInfo = type.GetMember(dataCenter.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            Uri targeturl = new Uri(((DescriptionAttribute)attributes[0]).Description);

            // Set up user credentials
            var creds = new System.ServiceModel.Description.ClientCredentials();
            creds.UserName.UserName = userName;
            creds.UserName.Password = password;
            Uri appReplyUri = new Uri(redirectUrl);

            // Call to get organizations from global discovery
            var organizations = CrmServiceClient.DiscoverGlobalOrganizations(
                    targeturl, creds, null, clientId, appReplyUri, "", false, string.Empty, PromptBehavior.Auto);


            return organizations.ToList();
        }

    }

    /// <summary>
    /// An enum for the known data centers
    /// </summary>
    public enum DataCenter
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
