using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.WebServiceClient;
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

        //These sample application registration values are available for all online instances.
        public static string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
        public static string redirectUrl = "app://58145B91-0C36-4500-8554-080854F2AC97";

        // version of ADAL that is linked to deal with diffrent version of ADAL. 
        private static Version _ADALAsmVersion;

        /// <summary>
        /// Gets organization data for user in all regions when data center is unknown
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <param name="dataCenter">The DataCenter enum balue that corresponds to the data center if known. Otherwise DataCenter.Unknown</param>
        /// <returns>A List of OrganizationDetail records</returns>
        public static List<OrganizationDetail> GetAllOrganizations(string userName, string password, DataCenter dataCenter)
        {
            var organizations = new List<OrganizationDetail>();

            //If DataCenter.Unknown is used, loop through all known DataCenters
            if (dataCenter == DataCenter.Unknown)
            {
                foreach (DataCenter dc in (DataCenter[])Enum.GetValues(typeof(DataCenter)))
                {

                    if (dc != DataCenter.Unknown)
                    {
                        //Get the organization detail information for a specific region
                        List<OrganizationDetail> orgs = GetOrganizationsForDataCenter(userName, password, dc);
                        organizations = organizations.Concat(orgs).ToList();
                    }
                }
            }
            else
            {
                //When the data center is not unknown, get data from the specified region
                organizations = GetOrganizationsForDataCenter(userName, password, dataCenter);
            }

            return organizations;
        }

        /// <summary>
        /// Get organization data for a specific known region only
        /// </summary>
        /// <param name="creds"></param>
        /// <param name="dataCenter"></param>
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

            var organizations = new List<OrganizationDetail>();

            using (var dsvc = new DiscoveryWebProxyClient(targeturl))
            {
                dsvc.HeaderToken = GetAccessToken(userName, password, targeturl);

                RetrieveOrganizationsRequest orgsRequest = new RetrieveOrganizationsRequest()
                {
                    AccessType = EndpointAccessType.Default,
                    Release = OrganizationRelease.Current
                };

                try
                {
                    RetrieveOrganizationsResponse response = (RetrieveOrganizationsResponse)dsvc.Execute(orgsRequest);

                    organizations = response.Details.ToList();
                }
                catch (System.ServiceModel.Security.SecurityAccessDeniedException)
                {
                    //This error is expected for regions where the user has no organizations
                }

            };
            return organizations;
        }

        /// <summary>
        /// Forming version tagged UriBuilder
        /// </summary>
        /// <param name="discoveryServiceUri"></param>
        /// <returns></returns>
        private static UriBuilder GetUriBuilderWithVersion(Uri discoveryServiceUri)
        {
            UriBuilder webUrlBuilder = new UriBuilder(discoveryServiceUri);
            string webPath = "web";

            if (!discoveryServiceUri.AbsolutePath.EndsWith(webPath))
            {
                if (discoveryServiceUri.AbsolutePath.EndsWith("/"))
                    webUrlBuilder.Path = string.Concat(webUrlBuilder.Path, webPath);
                else
                    webUrlBuilder.Path = string.Concat(webUrlBuilder.Path, "/", webPath);
            }

            UriBuilder versionTaggedUriBuilder = new UriBuilder(webUrlBuilder.Uri);
            string version = FileVersionInfo.GetVersionInfo(typeof(OrganizationWebProxyClient).Assembly.Location).FileVersion;
            string versionQueryStringParameter = string.Format("SDKClientVersion={0}", version);

            if (string.IsNullOrEmpty(versionTaggedUriBuilder.Query))
            {
                versionTaggedUriBuilder.Query = versionQueryStringParameter;
            }
            else if (!versionTaggedUriBuilder.Query.Contains("SDKClientVersion="))
            {
                versionTaggedUriBuilder.Query = string.Format("{0}&{1}", versionTaggedUriBuilder.Query, versionQueryStringParameter);
            }

            return versionTaggedUriBuilder;
        }

        /// <summary>
        /// Get the Authority and Support data from the requesting system using a sync call. 
        /// </summary>
        /// <param name="targetServiceUrl">Resource URL</param>
        /// <param name="logSink">Log tracer</param>
        /// <returns>Populated AuthenticationParameters or null</returns>
        private static AuthenticationParameters GetAuthorityFromTargetService(Uri targetServiceUrl)
        {
            try
            {
                // if using ADAL > 4.x  return.. // else remove oauth2/authorize from the authority
                if (_ADALAsmVersion == null)
                {
                    // initial setup to get the ADAL version 
                    var AdalAsm = System.Reflection.Assembly.GetAssembly(typeof(IPlatformParameters));
                    if (AdalAsm != null)
                        _ADALAsmVersion = AdalAsm.GetName().Version;
                }

                AuthenticationParameters foundAuthority;
                if (_ADALAsmVersion != null && _ADALAsmVersion >= Version.Parse("5.0.0.0"))
                {
                    foundAuthority = CreateFromUrlAsync(targetServiceUrl);
                }
                else
                {
                    foundAuthority = CreateFromResourceUrlAsync(targetServiceUrl);
                }

                if (_ADALAsmVersion != null && _ADALAsmVersion > Version.Parse("4.0.0.0"))
                {
                    foundAuthority.Authority = foundAuthority.Authority.Replace("oauth2/authorize", "");
                }

                return foundAuthority;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        /// <summary>
        /// Creates authentication parameters from the address of the resource.
        /// </summary>
        /// <param name="targetServiceUrl">Resource URL</param>
        /// <returns>AuthenticationParameters object containing authentication parameters</returns>
        private static AuthenticationParameters CreateFromResourceUrlAsync(Uri targetServiceUrl)
        {
            var result = (Task<AuthenticationParameters>)typeof(AuthenticationParameters)
                .GetMethod("CreateFromResourceUrlAsync").Invoke(null, new[] { targetServiceUrl });
            return result.Result;
        }

        /// <summary>
        /// Creates authentication parameters from the address of the resource.
        /// Invoked for ADAL 5+ which changed the method used to retrieve authentication parameters.
        /// </summary>
        /// <param name="targetServiceUrl">Resource URL</param>
        /// <returns>AuthenticationParameters object containing authentication parameters</returns>
        private static AuthenticationParameters CreateFromUrlAsync(Uri targetServiceUrl)
        {
            var result = (Task<AuthenticationParameters>)typeof(AuthenticationParameters)
                .GetMethod("CreateFromUrlAsync").Invoke(null, new[] { targetServiceUrl });

            return result.Result;
        }


        public static string GetAccessToken(string userName, string password, Uri serviceRoot)
        {
            var targetServiceUrl = GetUriBuilderWithVersion(serviceRoot);
            // Obtain the Azure Active Directory Authentication Library (ADAL) authentication context.
            AuthenticationParameters ap = GetAuthorityFromTargetService(targetServiceUrl.Uri);
            AuthenticationContext authContext = new AuthenticationContext(ap.Authority, false);
            //Note that an Azure AD access token has finite lifetime, default expiration is 60 minutes.
            AuthenticationResult authResult;

            if (userName != string.Empty && password != string.Empty)
            {

                UserPasswordCredential cred = new UserPasswordCredential(userName, password);
                authResult = authContext.AcquireTokenAsync(ap.Resource, clientId, cred).Result;
            }
            else
            {
                PlatformParameters platformParameters = new PlatformParameters(PromptBehavior.Auto);
                authResult = authContext.AcquireTokenAsync(ap.Resource, clientId, new Uri(redirectUrl), platformParameters).Result;
            }

            return authResult.AccessToken;
        }
    }

    /// <summary>
    /// An enum for the known data centers
    /// </summary>
    public enum DataCenter
    {
        [Description("Unknown")]
        Unknown,
        [Description("https://disco.crm.dynamics.com/XRMServices/2011/Discovery.svc/web")]
        NorthAmerica,
        [Description("https://disco.crm2.dynamics.com/XRMServices/2011/Discovery.svc/web")]
        SouthAmerica,
        [Description("https://disco.crm3.dynamics.com/XRMServices/2011/Discovery.svc/web")]
        Canada,
        [Description("https://disco.crm4.dynamics.com/XRMServices/2011/Discovery.svc/web")]
        EMEA,
        [Description("https://disco.crm5.dynamics.com/XRMServices/2011/Discovery.svc/web")]
        APAC,
        [Description("https://disco.crm6.dynamics.com/XRMServices/2011/Discovery.svc/web")]
        Oceania,
        [Description("https://disco.crm7.dynamics.com/XRMServices/2011/Discovery.svc/web")]
        Japan,
        [Description("https://disco.crm8.dynamics.com/XRMServices/2011/Discovery.svc/web")]
        India,
        [Description("https://disco.crm9.dynamics.com/XRMServices/2011/Discovery.svc/web")]
        NorthAmerica2,
        [Description("https://disco.crm11.dynamics.com/XRMServices/2011/Discovery.svc/web")]
        UK,
        [Description("https://disco.crm12.dynamics.com/XRMServices/2011/Discovery.svc/web")]
        France
    }
}
