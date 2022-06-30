using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Model;
using Microsoft.PowerPlatform.Dataverse.Client.Auth;
using Microsoft.Xrm.Sdk.Discovery;
using System.ComponentModel;

namespace PowerApps.Samples
{
    class Program
    {
        //These sample application registration values are available for all online instances.
        public static string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
        public static string redirectUrl = "http://localhost";


        static async Task Main()
        {
            string username = "yourUserName@yourOrgName.onmicrosoft.com";
            string password = "yourPassword";

            //Set the Cloud if you want to search other than Commercial.
            Cloud cloud = Cloud.Commercial;

            //Get all environments for the selected data center.
            DiscoverOrganizationsResult orgs = await GetAllOrganizations(username, password, cloud);

            if (orgs.OrganizationDetailCollection.Count.Equals(0))
            {
                Console.WriteLine("No valid environments returned for these credentials.");
                return;
            }

            Console.WriteLine("Type the number of the environments you want to use and press Enter.");

            int number = 0;

            //Display organizations so they can be selected
            foreach (OrganizationDetail organization in orgs.OrganizationDetailCollection)
            {
                number++;

                //Get the Organization URL
                string webAppUrl = organization.Endpoints[EndpointType.WebApplication];

                Console.WriteLine($"{number} Name: {organization.FriendlyName} URL: {webAppUrl}");
            }

            string typedValue = string.Empty;
            try
            {
                typedValue = Console.ReadLine();

                int selected = int.Parse(typedValue);

                if (selected <= number)
                {
                    OrganizationDetail org = orgs.OrganizationDetailCollection[selected - 1];
                    Console.WriteLine($"You selected '{org.FriendlyName}'");

                    //Use the selected org with ServiceClient to get the UserId
                    ShowUserId(org, username, password);
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
        /// Gets organization data for the specified user and cloud
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <param name="cloud">The Cloud enum value that corresponds to the region.</param>
        /// <returns>A List of OrganizationDetail records</returns>
        public static async Task<DiscoverOrganizationsResult> GetAllOrganizations(string userName, string password, Cloud cloud)
        {

            //Get the Cloud URL from the Description Attribute applied for the Cloud member
            var type = typeof(Cloud);
            var memInfo = type.GetMember(cloud.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            string cloudRegionUrl = ((DescriptionAttribute)attributes[0]).Description;

            // Set up user credentials
            var creds = new System.ServiceModel.Description.ClientCredentials();
            creds.UserName.UserName = userName;
            creds.UserName.Password = password;

            //Call DiscoverOnlineOrganizationsAsync
            DiscoverOrganizationsResult organizationsResult = await ServiceClient.DiscoverOnlineOrganizationsAsync(
                   discoveryServiceUri: new Uri($"{cloudRegionUrl}/api/discovery/v2.0/Instances"),
                   clientCredentials: creds,
                   clientId: clientId,
                   redirectUri: new Uri(redirectUrl),
                   isOnPrem: false,
                   authority: "https://login.microsoftonline.com/organizations/",
                   promptBehavior: PromptBehavior.Auto);

            return organizationsResult;
        }

        /// <summary>
        /// Show the user's UserId for the selected organization
        /// </summary>
        /// <param name="org">The selected organization</param>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        private static void ShowUserId(OrganizationDetail org, string username, string password)
        {
            string conn = $@"AuthType=OAuth;
                         Url={org.Endpoints[EndpointType.OrganizationService]};
                         UserName={username};
                         Password={password};
                         ClientId={clientId};
                         RedirectUri={redirectUrl};
                         Prompt=Auto;
                         RequireNewInstance=True";
            ServiceClient svc = new(conn);

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
    }
}