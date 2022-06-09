using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        /// <summary>
        /// Function to setup the sample
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityLogicalName">Logical name of the entity</param>
        /// <param name="columnLogicalName">Image column name</param>
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }
        }

        /// <summary>
        /// Retrieves publisher details with a given crm servic client and publisher prefix
        /// </summary>
        /// <param name="service">The CrmServiceClient object to use</param>
        /// <param name="prefix">The publisher prefix</param>
        /// <returns>Publisher entity</returns>
        private static Entity RetrievePublisherDetails(CrmServiceClient service, string prefix)
        {
            var publisherFetchXML = $@"<fetch top='50'>
                                        <entity name='publisher'>
                                          <attribute name='customizationprefix' />
                                          <attribute name='address1_addresstypecode' />
                                          <attribute name='uniquename' />
                                          <attribute name='friendlyname' />
                                          <attribute name='publisherid' />
                                          <filter>
                                            <condition attribute='customizationprefix' operator='eq' value='{prefix}' />
                                          </filter>
                                        </entity>
                                      </fetch>";

            return service.RetrieveMultiple(new FetchExpression(publisherFetchXML))?.Entities.FirstOrDefault();
        }

        /// <summary>
        /// Creates a publisher
        /// </summary>
        /// <param name="service">The CrmServiceClient object to use</param>
        /// <returns>Publisher prefix</returns>
        private static string CreatePublisher(CrmServiceClient service)
        {
            Console.WriteLine("============== P U B L I S H E R ================");
            Console.Write("Publisher unique name: ");
            var uniqueName = Console.ReadLine();
            Console.Write("Publisher Display Name: ");
            var displayName = Console.ReadLine();
            Console.Write("Prefix");
            var prefix = Console.ReadLine();

            //Define a new publisher
            Publisher _crmSdkPublisher = new Publisher
            {
                UniqueName = uniqueName,
                FriendlyName = displayName,
                SupportingWebsiteUrl = "https://msdn.microsoft.com/dynamics/crm/default.aspx",
                CustomizationPrefix = prefix,
                EMailAddress = "someone@microsoft.com",
                Description = "This publisher was created with samples from the Microsoft Dynamics CRM SDK"
            };

            //Does publisher already exist?
            QueryExpression querySDKSamplePublisher = new QueryExpression
            {
                EntityName = Publisher.EntityLogicalName,
                ColumnSet = new ColumnSet("publisherid", "customizationprefix"),
                Criteria = new FilterExpression()
            };

            querySDKSamplePublisher.Criteria.AddCondition("uniquename", ConditionOperator.Equal, _crmSdkPublisher.UniqueName);
            EntityCollection querySDKSamplePublisherResults = service.RetrieveMultiple(querySDKSamplePublisher);
            Publisher SDKSamplePublisherResults = null;
            var publisherId = Guid.Empty;
            if (querySDKSamplePublisherResults.Entities.Count > 0)
            {
                SDKSamplePublisherResults = (Publisher)querySDKSamplePublisherResults.Entities[0];
                publisherId = (Guid)SDKSamplePublisherResults.PublisherId;
                prefix = SDKSamplePublisherResults.CustomizationPrefix;
            }
            //If it doesn't exist, create it
            if (SDKSamplePublisherResults == null)
            {
                publisherId = service.Create(_crmSdkPublisher);
                Console.WriteLine($"Created publisher: {_crmSdkPublisher.FriendlyName}.");
                prefix = _crmSdkPublisher.CustomizationPrefix;
            }
            Console.WriteLine("==============================");

            return prefix;
        }

        /// <summary>
        /// Creates the solution for a given service client and publisher prefix
        /// </summary>
        /// <param name="service">The CrmServiceClient object to use</param>
        /// <param name="prefix">The publisher prefix</param>
        /// <returns>Solution unique name</returns>
        /// <exception cref="Exception">Ecception thrown</exception>
        private static string CreateSolution(CrmServiceClient service, string prefix)
        {
            try
            {
                Console.WriteLine("Please provide solution details...\n");
                Console.Write("UniqueName: ");
                var uniqueName = Console.ReadLine();
                Console.Write("Display Name: ");
                var displayName = Console.ReadLine();

                var myCustomPublisher = RetrievePublisherDetails(service, prefix);

                Solution solution = new Solution
                {
                    UniqueName = uniqueName,
                    FriendlyName = displayName,
                    PublisherId = new EntityReference(Publisher.EntityLogicalName,
                                new Guid(myCustomPublisher.Attributes["publisherid"].ToString())),
                    Description = "This solution was created by the WorkWithSolutions sample code in the Microsoft Dynamics CRM SDK samples.",
                    Version = "1.0"
                };

                //Check whether it already exists
                QueryExpression queryCheckForSampleSolution = new QueryExpression
                {
                    EntityName = Solution.EntityLogicalName,
                    ColumnSet = new ColumnSet(),
                    Criteria = new FilterExpression()
                };
                queryCheckForSampleSolution.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solution.UniqueName);

                //Create the solution if it does not already exist.
                EntityCollection querySampleSolutionResults = service.RetrieveMultiple(queryCheckForSampleSolution);
                Solution SampleSolutionResults = null;
                var solutionId = Guid.Empty;
                if (querySampleSolutionResults.Entities.Count > 0)
                {
                    SampleSolutionResults = (Solution)querySampleSolutionResults.Entities[0];
                    solutionId = (Guid)SampleSolutionResults.SolutionId;
                }
                if (SampleSolutionResults == null)
                {
                    solutionId = service.Create(solution);
                }

                Console.WriteLine($"New solution '{displayName}({uniqueName})' is created/fetched with ID('{solutionId}')");

                return uniqueName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR occurred while creating solution: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Prompts user to delete solution. Deletes solution if they choose.
        /// </summary>
        /// <param name="service">The service to use to delete the solution. </param>
        /// <param name="uniqueName">The unique name of the solution to delete.</param>
        /// <returns>true when the solution was deleted, otherwise false.</returns>
        private static bool DeleteSolution(CrmServiceClient service, string uniqueName)
        {
            Console.WriteLine($"Do you want to delete the {uniqueName} solution? (y/n)");
            String answer = Console.ReadLine();

            bool deleteSolution = (answer.StartsWith("y") || answer.StartsWith("Y"));

            if (deleteSolution)
            {
                Console.WriteLine($"Deleting the {uniqueName} solution....");
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
                    Console.WriteLine($"Deleted the {solution["friendlyname"]} solution.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"No solution named {uniqueName} is installed.");
                }
            }
            return false;
        }
   
        /// <summary>
        /// Prompts user to delete publisher. Deletes publisher if they choose.
        /// </summary>
        /// <param name="service">The service to use to delete the solution. </param>
        /// <param name="uniqueName">The unique name of the solution to delete.</param>
        /// <returns>true when the solution was deleted, otherwise false.</returns>
        private static bool DeletePublisher(CrmServiceClient service, string customizationPrefix)
        {
            Console.WriteLine($"Do you want to delete the {customizationPrefix} publisher? (y/n)");
            String answer = Console.ReadLine();
            bool deletePublisher = (answer.StartsWith("y") || answer.StartsWith("Y"));

            if (deletePublisher)
            {
                Console.WriteLine($"Deleting the {customizationPrefix} publisher....");
                QueryExpression solutionQuery = new QueryExpression
                {
                    EntityName = "publisher",
                    ColumnSet = new ColumnSet(new string[] { "solutionid", "customizationprefix" }),
                    Criteria = new FilterExpression()
                };
                solutionQuery.Criteria.AddCondition("uniquename", ConditionOperator.Equal, customizationPrefix);

                Entity publisher = service.RetrieveMultiple(solutionQuery).Entities[0];
                if (publisher != null)
                {
                    service.Delete("publisher", (Guid)publisher["publisherid"]);
                    Console.WriteLine($"Deleted the {publisher["customizationPrefix"]} publisher.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"No publisher named {customizationPrefix} is installed.");
                }
            }
            return false;
        }
    }
}
