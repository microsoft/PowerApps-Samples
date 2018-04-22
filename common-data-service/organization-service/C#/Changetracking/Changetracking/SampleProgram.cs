using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.Threading;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        static void Main(string[] args)
        {
           
            try
            {
                //You must specify connection information in common-data-service/App.config to run this sample.
                using (CrmServiceClient csc = new CrmServiceClient(GetConnectionStringFromAppConfig("Connect")))
                {
                    if (csc.IsReady)
                    {
                        IOrganizationService service = csc.OrganizationServiceProxy;

                        //Add code here
                        //////////////////////////////////////////////

                        // Check that the current version is greater than the minimum version
                        if (SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
                        {
                            //Import the ChangeTrackingSample solution
                            if (SampleHelpers.ImportSolution(service, "ChangeTrackingSample", "ChangeTrackingSample_1_0_0_0_managed.zip"))
                            {
                                Console.WriteLine("Waiting for the alternate key index to be created.......");
                                Thread.Sleep(50000);
                            }

                            if (WaitForEntityAndKeysToBeActive(service, "sample_book"))
                            {
                                // Create 10 sample book records.
                                CreateBookRecordsForSample(service);

                                //Continue from here...

                            }
                            else {
                                Console.WriteLine("There is a problem creating the index for the book code alternate key for the sample_book entity.");
                                Console.WriteLine("The sample cannot continue. Please try again.");
                                //Delete the ChangeTrackingSample solution
                                SampleHelpers.DeleteSolution(service, "ChangeTrackingSample");
                                return;

                            }


                            //Delete the ChangeTrackingSample solution
                            SampleHelpers.DeleteSolution(service, "ChangeTrackingSample");
                        }


                        //////////////////////////////////////////////
                        Console.WriteLine("The sample completed successfully");
                        return;
                    }
                    else
                    {
                        if (csc.LastCrmError.Equals("Unable to Login to Dynamics CRM"))
                        {
                            Console.WriteLine("Check the connection string values in common-data-service/App.config.");
                            throw new Exception(csc.LastCrmError);
                        }
                        else
                        {
                            throw csc.LastCrmException;
                        }
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Plugin Trace: {0}", ex.Detail.TraceText);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<OrganizationServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Plugin Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
            // Additional exceptions to catch: SecurityTokenValidationException, ExpiredSecurityTokenException,
            // SecurityAccessDeniedException, MessageSecurityException, and SecurityNegotiationException.

            finally
            {

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

        }
        /// <summary>
        /// Gets a named connection string from App.config
        /// </summary>
        /// <param name="name">The name of the connection string to return</param>
        /// <returns>The named connection string</returns>
        static string GetConnectionStringFromAppConfig(string name) {
            //Verify common-data-service/App.config contains a valid connection string with the name.
            try
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            catch (Exception)
            {
                Console.WriteLine("You must set connection data in common-data-service/App.config before running this sample.");
                return string.Empty;
            }
        }
    }
}
