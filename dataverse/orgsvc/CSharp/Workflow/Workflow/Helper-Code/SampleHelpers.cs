using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using PowerApps.Samples.LoginUX;
using System;
using System.Configuration;
using System.ServiceModel;

namespace PowerApps.Samples
{
    public class SampleHelpers
    {
        /// <summary>
        /// Checks whether the current environment will support this sample.
        /// </summary>
        /// <param name="service">The service to use to check the version. </param>
        /// <param name="minVersion">The minimum version.</param>
        /// <returns>true when the version is higher than the minimum verions, otherwise false.</returns>
        public static bool CheckVersion(CrmServiceClient service, Version minVersion)
        {
            if (service.ConnectedOrgVersion.CompareTo(minVersion) >= 0)
            {
                return true;
            }
            else
            {
                Console.WriteLine("This sample cannot be run against the current version.");
                Console.WriteLine(string.Format("Upgrade your instance to a version above {0} to run this sample.", minVersion.ToString()));
                return false;
            }
        }

        /// <summary>
        /// Imports a solution if it is not already installed.
        /// </summary>
        /// <param name="service">The service to use to import the solution. </param>
        /// <param name="uniqueName">The unique name of the solution to install.</param>
        /// <param name="pathToFile">The path to the solution file.</param>
        /// <returns>true if the solution was installed, otherwise false.</returns>
        public static bool ImportSolution(CrmServiceClient service, string uniqueName, string pathToFile)
        {

            QueryByAttribute queryCheckForSampleSolution = new QueryByAttribute();
            queryCheckForSampleSolution.AddAttributeValue("uniquename", uniqueName);
            queryCheckForSampleSolution.EntityName = "solution";

            EntityCollection querySampleSolutionResults = service.RetrieveMultiple(queryCheckForSampleSolution);

            if (querySampleSolutionResults.Entities.Count > 0)
            {
                Console.WriteLine("The {0} solution is already installed.", uniqueName);

                return false;
            }
            else
            {
                Console.WriteLine("The {0} solution is not installed. Importing the solution....", uniqueName);
                Guid ImportId = Guid.Empty;
                service.ImportSolutionToCrm(pathToFile, out ImportId);
                return true;
            }
        }
        /// <summary>
        /// Prompts user to delete solution. Deletes solution if they choose.
        /// </summary>
        /// <param name="service">The service to use to delete the solution. </param>
        /// <param name="uniqueName">The unique name of the solution to delete.</param>
        /// <returns>true when the solution was deleted, otherwise false.</returns>
        public static bool DeleteSolution(CrmServiceClient service, string uniqueName)
        {
            bool deleteSolution = true;

            Console.WriteLine("Do you want to delete the {0} solution? (y/n)", uniqueName);
            String answer = Console.ReadLine();

            deleteSolution = (answer.StartsWith("y") || answer.StartsWith("Y"));

            if (deleteSolution)
            {
                Console.WriteLine("Deleting the {0} solution....", uniqueName);
                QueryExpression solutionQuery = new QueryExpression
                {
                    EntityName = "solution",
                    ColumnSet = new ColumnSet(new string[] { "solutionid", "friendlyname" }),
                    Criteria = new FilterExpression()
                };
                solutionQuery.Criteria.AddCondition("uniquename", ConditionOperator.Equal, uniqueName);


                Entity solution = service.RetrieveMultiple(solutionQuery).Entities[0];

                if (solution != null)
                {
                    service.Delete("solution", (Guid)solution["solutionid"]);

                    Console.WriteLine("Deleted the {0} solution.", solution["friendlyname"]);
                    return true;
                }
                else
                {
                    Console.WriteLine("No solution named {0} is installed.");
                }
            }
            return false;
        }
        /// <summary>
        /// A function to manage exceptions thrown by console application samples
        /// </summary>
        /// <param name="exceptionFromSample">The exception thrown</param>
        public static void HandleException(Exception exceptionFromSample)
        {
            Console.WriteLine("The application terminated with an error.");

            try
            {
                throw exceptionFromSample;
            }
            catch (FaultException<OrganizationServiceFault> fe)
            {
                Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", fe.Detail.Message);
                Console.WriteLine("Plugin Trace: {0}", fe.Detail.TraceText);
                Console.WriteLine("Inner Fault: {0}",
                    null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (TimeoutException te)
            {
                Console.WriteLine("Message: {0}", te.Message);
                Console.WriteLine("Stack Trace: {0}", te.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == te.InnerException.Message ? "No Inner Fault" : te.InnerException.Message);

            }
            catch (Exception ex)
            {
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

        }

        /// <summary>
        /// Gets a named connection string from App.config
        /// </summary>
        /// <param name="name">The name of the connection string to return</param>
        /// <returns>The named connection string</returns>
        private static string GetConnectionStringFromAppConfig(string name)
        {
            //Verify cds/App.config contains a valid connection string with the name.
            try
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            catch (Exception)
            {
                Console.WriteLine("You can set connection data in cds/App.config before running this sample. - Switching to Interactive Mode");
                return string.Empty;
            }
        }

        public static CrmServiceClient Connect(string name)
        {
            CrmServiceClient service = null;

            //You can specify connection information in cds/App.config to run this sample without the login dialog
            if (string.IsNullOrEmpty(GetConnectionStringFromAppConfig("Connect")))
            {
                // Failed to find a connection string... Show login Dialog. 
                ExampleLoginForm loginFrm = new ExampleLoginForm();
                // Login process is Async, thus we need to detect when login is completed and close the form. 
                loginFrm.ConnectionToCrmCompleted += LoginFrm_ConnectionToCrmCompleted;
                // Show the dialog here. 
                loginFrm.ShowDialog();

                // If the login process completed, assign the connected service to the CRMServiceClient var 
                if (loginFrm.CrmConnectionMgr != null && loginFrm.CrmConnectionMgr.CrmSvc != null && loginFrm.CrmConnectionMgr.CrmSvc.IsReady)
                    service = loginFrm.CrmConnectionMgr.CrmSvc;


            }
            else
            {
                // Try to create via connection string. 
                service = new CrmServiceClient(GetConnectionStringFromAppConfig("Connect"));

            }

            return service;

        }

        /// <summary>
        /// Handle closing the dialog when completed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void LoginFrm_ConnectionToCrmCompleted(object sender, EventArgs e)
        {
            if (sender is ExampleLoginForm)
            {
                ((ExampleLoginForm)sender).Close();
            }
        }
    }
}
