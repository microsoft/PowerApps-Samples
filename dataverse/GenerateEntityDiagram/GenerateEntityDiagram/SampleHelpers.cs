using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ServiceModel;

namespace PowerApps.Samples
{
    public class SampleHelpers
    {
        /// <summary>
        /// Imports a solution if it is not already installed.
        /// </summary>
        /// <param name="service">The service to use to import the solution. </param>
        /// <param name="uniqueName">The unique name of the solution to install.</param>
        /// <param name="pathToFile">The path to the solution file.</param>
        /// <returns>true if the solution was installed, otherwise false.</returns>
        public static bool ImportSolution(ServiceClient service, string uniqueName, string pathToFile)
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
                service.ImportSolution(pathToFile, out ImportId);          
                return true;
            }
        }
        /// <summary>
        /// Prompts user to delete solution. Deletes solution if they choose.
        /// </summary>
        /// <param name="service">The service to use to delete the solution. </param>
        /// <param name="uniqueName">The unique name of the solution to delete.</param>
        /// <returns>true when the solution was deleted, otherwise false.</returns>
        public static bool DeleteSolution(ServiceClient service, string uniqueName)
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
        public static void HandleException(Exception exceptionFromSample) {
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
            catch (TimeoutException te) {
                Console.WriteLine("Message: {0}", te.Message);
                Console.WriteLine("Stack Trace: {0}", te.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == te.InnerException.Message ? "No Inner Fault" : te.InnerException.Message);

            }
            catch (Exception ex) {
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
    }
}
