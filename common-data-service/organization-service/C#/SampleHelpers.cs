using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
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
        /// <returns></returns>
        public static bool CheckVersion(IOrganizationService service, Version minVersion)
        {

            RetrieveVersionResponse crmVersionResp = (RetrieveVersionResponse)service.Execute(new RetrieveVersionRequest());

            Version currentVersion = new Version(crmVersionResp.Version);

            if (currentVersion.CompareTo(minVersion) >= 0)
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
        /// <param name="service">The service to use to check the version. </param>
        /// <param name="uniqueName">The unique name of the solution to install.</param>
        /// <param name="pathToFile">The path to the solution file.</param>
        /// <returns>true if the solution was installed, otherwise false.</returns>
        public static bool ImportSolution(IOrganizationService service, string uniqueName, string pathToFile)
        {

            Console.WriteLine("Checking whether the {0} solution is already installed.", uniqueName);

            QueryByAttribute queryCheckForSampleSolution = new QueryByAttribute();
            queryCheckForSampleSolution.AddAttributeValue("uniquename", uniqueName);
            queryCheckForSampleSolution.EntityName = Solution.EntityLogicalName;

            EntityCollection querySampleSolutionResults = service.RetrieveMultiple(queryCheckForSampleSolution);

            if (querySampleSolutionResults.Entities.Count > 0)
            {
                Console.WriteLine("The {0} solution is already installed.", uniqueName);

                return false;
            }
            else
            {
                Console.WriteLine("The {0} solution is not installed. Importing the solution....", uniqueName);
                byte[] fileBytes = File.ReadAllBytes(pathToFile);
                ImportSolutionRequest impSolReq = new ImportSolutionRequest()
                {
                    CustomizationFile = fileBytes, 
                };

                service.Execute(impSolReq);

                return true;
            }
        }
        /// <summary>
        /// Deletes a solution
        /// </summary>
        /// <param name="service">The service to use to delete the solution. </param>
        /// <param name="uniqueName">The unique name of the solution to delete.</param>
        /// <returns>true when the solution was deleted, otherwise false.</returns>
        public static bool DeleteSolution(IOrganizationService service, string uniqueName)
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
                    EntityName = Solution.EntityLogicalName,
                    ColumnSet = new ColumnSet(new string[] { "solutionid", "friendlyname" }),
                    Criteria = new FilterExpression()
                };
                solutionQuery.Criteria.AddCondition("uniquename", ConditionOperator.Equal, uniqueName);


                Solution solution = (Solution)service.RetrieveMultiple(solutionQuery).Entities[0];

                if (solution != null)
                {
                    service.Delete(Solution.EntityLogicalName, (Guid)solution.SolutionId);

                    Console.WriteLine("Deleted the {0} solution.", solution.FriendlyName);
                    return true;
                }
                else
                {
                    Console.WriteLine("No solution named {0} is installed.");
                }
            }
            return false;
        }
    }
}
