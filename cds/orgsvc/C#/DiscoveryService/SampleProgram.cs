using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        static void Main(string[] args)
        {
            string username = "yourUserName@yourOrgName.onmicrosoft.com";
            string password = "yourPassword";

            //Set the dataCenter if you know it, otherwise use DataCenter.Unknown to search all.
            DataCenter dataCenter = DataCenter.Unknown;

            //Get all environments for the selected data center;
            List<OrganizationDetail> orgs = GetAllOrganizations(username, password, dataCenter);

            if (orgs.Count.Equals(0))
            {
                Console.WriteLine("No valid environments returned for these credentials.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
                return;

            }

            Console.WriteLine("Type the number of the environments you want to use and press Enter.");

            int number = 0;
            orgs.ForEach(o =>
            {
                number++;

          //Get the Organization Service URL
          string fullOrgServiceUrl = o.Endpoints[EndpointType.OrganizationService];

          // Trim '/XRMServices/2011/Organization.svc' from the end.
          string shortOrgServiceUrl = fullOrgServiceUrl.Substring(0, fullOrgServiceUrl.Length - 34);

                Console.WriteLine("{0} Name: {1} URL: {2}", number, o.FriendlyName, shortOrgServiceUrl);
            });

            string typedValue = string.Empty;
            try
            {
                typedValue = Console.ReadLine();
                int selected = int.Parse(typedValue);
                if (selected <= number)
                {
                    OrganizationDetail org = orgs[selected - 1];
                    Console.WriteLine("You selected {0}", org.FriendlyName);

                    //Get the Organization Service URL for the selected environment
                    string serviceUrl = org.Endpoints[EndpointType.OrganizationService];

                    //Use the selected serviceUrl with CrmServiceClient to get the UserId

                    string conn = $@"AuthType=Office365;
                         Url={serviceUrl};
                         UserName={username};
                         Password={password};
                         RequireNewInstance=True";

                    using (CrmServiceClient svc = new CrmServiceClient(conn))
                    {

                        if (svc.IsReady)
                        {
                            try
                            {
                                var response = (WhoAmIResponse)svc.Execute(new WhoAmIRequest());

                                Console.WriteLine("Your UserId for {0} is: {1}", org.FriendlyName, response.UserId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine(svc.LastCrmError);
                        }
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

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
