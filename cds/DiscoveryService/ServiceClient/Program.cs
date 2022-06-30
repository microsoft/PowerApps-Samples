using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Model;
using Microsoft.Xrm.Sdk.Discovery;
using System.ComponentModel;

namespace PowerApps.Samples
{
    class Program
    {
        //These sample application registration values are available for all online instances.
        public static string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
        public static string redirectUrl = "http://localhost";
        static readonly string username = "yourUserName@yourOrgName.onmicrosoft.com";
        static readonly string password = "yourPassword";
        //Set the Cloud if you want to search other than Commercial.
        static readonly Cloud cloud = Cloud.Commercial;

        static async Task Main()
        {
            //Get all environments for the selected data center.
            DiscoverOrganizationsResult orgs = await GetAllOrganizations(username, password, cloud);

            if (orgs.OrganizationDetailCollection.Count.Equals(0)) {
                Console.WriteLine("No valid environments returned for these credentials.");
                return;
            }

            Console.WriteLine("Type the number of the environments you want to use and press Enter.");

            int number = 0;

            foreach (OrganizationDetail organization in orgs.OrganizationDetailCollection)
            {
                number++;

                //Get the Organization Service URL
                string fullOrgServiceUrl = organization.Endpoints[EndpointType.OrganizationService];

                // Trim '/XRMServices/2011/Organization.svc' from the end.
                string shortOrgServiceUrl = fullOrgServiceUrl.Substring(0, fullOrgServiceUrl.Length - 34);

                Console.WriteLine($"{number} Name: {organization.FriendlyName} URL: {shortOrgServiceUrl}");
            }

            string typedValue = string.Empty;
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                typedValue = Console.ReadLine();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
                int selected = int.Parse(typedValue);
#pragma warning restore CS8604 // Possible null reference argument.
                if (selected <= number)
                {
                    OrganizationDetail org = orgs.OrganizationDetailCollection[selected - 1];
                    Console.WriteLine($"You selected {org.FriendlyName}");

                    //Get the Organization Service URL for the selected environment
                    string serviceUrl = org.Endpoints[EndpointType.OrganizationService];

                    //Use the selected serviceUrl with ServiceClient to get the UserId

                    string conn = $@"AuthType=OAuth;
                         Url={serviceUrl};
                         UserName={username};
                         Password={password};
                         ClientId={clientId};
                         RedirectUri={redirectUrl};
                         Prompt=Auto;
                         RequireNewInstance=True";

                    using ServiceClient svc = new(conn);

                    if (svc.IsReady)
                    {
                        try
                        {
                            var response = (WhoAmIResponse)svc.Execute(new WhoAmIRequest());

                            Console.WriteLine($"Your UserId for {org.FriendlyName} is: {response.UserId}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine(svc.LastError);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The selected value is not valid.");
                }

            }
            catch (ArgumentOutOfRangeException aex)
            {
                Console.WriteLine(aex.Message);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to process value: {0}", typedValue);
            }

        }

        /// <summary>
        /// Gets organization data for the specified user.
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <param name="cloud">The Cloud enum value that corresponds to the data center if known. Otherwise Cloud.Unknown</param>
        /// <returns>A List of OrganizationDetail records</returns>
        public static async Task<DiscoverOrganizationsResult> GetAllOrganizations(string userName, string password, Cloud cloud)
        {
            // Get data from Organization
            return await GetOrganizationsForCloud(userName, password, cloud);
        }

        /// <summary>
        /// Get organization data for a specific known region only
        /// </summary>
        /// <param name="creds">User's credentials.</param>
        /// <param name="cloud">Target Cloud.</param>
        /// <returns></returns>
        public static async Task<DiscoverOrganizationsResult> GetOrganizationsForCloud(string userName, string password, Cloud cloud)
        {

            //Get the Cloud URL from the Description Attribute applied for the Cloud member
            var type = typeof(Cloud);
            var memInfo = type.GetMember(cloud.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            Uri targeturl = new(((DescriptionAttribute)attributes[0]).Description);

            // Set up user credentials
            var creds = new System.ServiceModel.Description.ClientCredentials();
            creds.UserName.UserName = userName;
            creds.UserName.Password = password;
            Uri appReplyUri = new(redirectUrl);


            DiscoverOrganizationsResult organizationsResult = await ServiceClient.DiscoverOnlineOrganizationsAsync(
                   discoveryServiceUri: targeturl,
                   clientCredentials: creds,
                   clientId: clientId,
                   redirectUri: appReplyUri,
                   isOnPrem: false,
                   authority: "https://login.microsoftonline.com/organizations/",
                   promptBehavior: Microsoft.PowerPlatform.Dataverse.Client.Auth.PromptBehavior.Auto);

            return organizationsResult;
        }
    }


}