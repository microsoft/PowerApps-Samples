using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Required to support the interactive login experience
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    // Create any entity records that the demonstration code requires
                    SetUpSample(service);
                    #region Demonstrate
                    #region RetrieveDeploymentLicenseTypeRequest

                    // Create the request
                    var deploymentTypeRequest = new RetrieveDeploymentLicenseTypeRequest();

                    // Execute the request
                    Console.WriteLine("  Fetching the license type for this deployment");
                    var deploymentTypeResponse =
                        (RetrieveDeploymentLicenseTypeResponse)service.Execute(deploymentTypeRequest);

                    // Validate results
                    if (String.IsNullOrEmpty(deploymentTypeResponse.licenseType))
                        throw new Exception("The request did not return any results");

                    string licenseId =
                        new Guid(deploymentTypeResponse.licenseType).ToString().ToUpper();
                    switch (licenseId)
                    {
                        case "9436EA66-8262-4168-9B6C-21DF47D1D121":
                            Console.WriteLine("  License type: SmallBusiness");
                            break;
                        case "5BEEA2E8-8F82-40E8-AE0F-6D8C135E1397":
                            Console.WriteLine("  License type: Professional");
                            break;
                        case "CB96BDD5-5F74-4EA5-8323-9D7E20079002":
                            Console.WriteLine("  License type: Enterprise");
                            break;
                        case "66AE2919-DD58-40CA-A980-AEF7330B2745":
                            Console.WriteLine("  License type: Live");
                            break;
                        case "722E9E15-62DC-48A7-95CF-93131BE27273":
                            Console.WriteLine("  License type: SPLA (Service Provider)");
                            break;
                        default:
                            Console.WriteLine("  Unknown license type with id {0}", licenseId);
                            break;
                    }

                    #endregion

                    #region RetrieveLicenseInfoRequest

                    // create the request
                    var licenseInfoRequest = new RetrieveLicenseInfoRequest();

                    // execute the request
                    Console.WriteLine("  Fetching license info");
                    var licenseInfoResponse =
                        (RetrieveLicenseInfoResponse)service.Execute(licenseInfoRequest);

                    // output the results
                    Console.WriteLine("  Number of licenses available: {0}",
                        licenseInfoResponse.AvailableCount);
                    Console.WriteLine("  Number of licenses used: {0}",
                        licenseInfoResponse.GrantedLicenseCount);

                    #endregion
                    #endregion Demonstrate

                    #region Clean up
                    //CleanUpSample(service);
                    #endregion Clean up
                }
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

        }
    }
}
