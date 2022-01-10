using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    #region Sample Code
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate
                    // Prompt user to install/uninstall sample data.
                    Console.WriteLine("Would you like to:");
                    Console.WriteLine("1) Install sample data for CDS?");

                    Console.WriteLine("2) Uninstall sample data for CDS?");
                    Console.Write("Press [1] to Install, [2] to Uninstall: ");
                    String answer = Console.ReadLine();

                    // Update the sample data based on the user's response.
                    switch (answer)
                    {
                        case "1":
                            Console.WriteLine("Installing sample data...");
                            InstallSampleDataRequest request =
                                new InstallSampleDataRequest();
                            InstallSampleDataResponse response =
                                (InstallSampleDataResponse)service.Execute(request);
                            Console.WriteLine("Sample data successfully installed.");
                            break;
                        case "2":
                            Console.WriteLine("Uninstalling sample data...");
                            UninstallSampleDataRequest request2 =
                                new UninstallSampleDataRequest();
                            UninstallSampleDataResponse response2 =
                                (UninstallSampleDataResponse)service.Execute(request2);
                            Console.WriteLine("Sample data successfully uninstalled.");
                            break;
                        default:
                            Console.WriteLine("Neither option was selected. No changes have been made to your records.");
                            break;
                    }

                }
                #endregion Demonstrate
                #endregion Sample Code
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

