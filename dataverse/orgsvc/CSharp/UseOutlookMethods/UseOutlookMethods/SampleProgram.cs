using Microsoft.Crm.Outlook.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;


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
                    // Set up the CRM Service.  
                    var outlookService = new CrmOutlookService();

                    // Determine if the Outlook client is running
                    if (outlookService.IsCrmClientLoaded)
                    {
                        if (outlookService.IsCrmDesktopClient)
                        {
                            // The desktop client cannot go offline
                            Console.WriteLine("CRM Client Desktop URL: " +
                                outlookService.ServerUri.AbsoluteUri);
                            Console.WriteLine("CRM Client state: " +
                                outlookService.State.ToString());
                        }
                        else
                        {
                            // See if laptop client is offline
                            if (outlookService.IsCrmClientOffline)
                            {
                                Console.WriteLine("CRM Client Offline URL: " +
                                    outlookService.ServerUri.AbsoluteUri);
                                Console.WriteLine("CRM Client state: " +
                                    outlookService.State.ToString());

                                // Take client online
                                // NOTE: GoOnline() will automatically Sync up with CRM
                                // database, no need to call Sync() manually
                                Console.WriteLine("Going Online...");
                                outlookService.GoOnline();

                                Console.WriteLine("CRM Client state: " +
                                    outlookService.State.ToString());
                            }
                            else
                            {
                                Console.WriteLine("CRM Client Online URL: " +
                                    outlookService.ServerUri.AbsoluteUri);
                                Console.WriteLine("CRM Client state: " +
                                    outlookService.State.ToString());

                                // Take client offline 
                                // NOTE: GoOffline triggers a synchronization of the
                                // offline database with the online server.
                                // If a sync is not required, you can use SetOffline().
                                Console.WriteLine("Going Offline...");
                                outlookService.GoOffline();

                                Console.WriteLine("CRM Client state: " +
                                    outlookService.State.ToString());
                            }
                        }
                    }
                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
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
