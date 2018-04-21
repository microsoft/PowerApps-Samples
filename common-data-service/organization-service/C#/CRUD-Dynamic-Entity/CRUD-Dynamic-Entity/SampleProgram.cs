using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
using System.ServiceModel;

namespace PowerApps.Samples
{
    partial class SampleProgram
    {
        static void Main(string[] args)
        {
            try {
                using (CrmServiceClient csc = new CrmServiceClient(ConfigurationManager.ConnectionStrings["Connect"].ConnectionString)) {
                    if (csc.IsReady)
                    {
                        IOrganizationService service = csc.OrganizationServiceProxy;

                        //Add code here
                        //////////////////////////////////////////////
                        Entity account = new Entity("account");
                        account["name"] = "Test Account";

                        //User service methods directly:
                        // Guid accountid = service.Create(account);

                        //Use method defined in SampleMethods.cs
                        Guid accountid = CreateEntity(service, account);



                        //////////////////////////////////////////////
                        Console.WriteLine("The sample completed successfully");
                    }
                    else {
                        if (csc.LastCrmError.Equals("Unable to Login to Dynamics CRM"))
                        {
                            Console.WriteLine("Check the connection string values in common-data-service/App.config.");
                            throw new Exception(csc.LastCrmError);                            
                        }
                        else {
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
    }
}
