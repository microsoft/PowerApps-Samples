using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        /// <summary>
        /// Function to setup the sample
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityLogicalName">Logical Name of the entity</param>
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

        private static Entity RetrieveSolutionDetails(CrmServiceClient service, string publisherName)
        {
            var publisherId = RetrievePublisherDetails(service, publisherName)?.Id;

            if (publisherId == Guid.Empty)
            {
                Console.WriteLine($"No publisher found with Name({publisherName})");
                return null;
            }

            var solutionFetchXML = $@"<fetch>
                                      <entity name='solution'>
                                        <attribute name='uniquename' />
                                        <attribute name='publisherid' />
                                        <attribute name='friendlyname' />
                                        <attribute name='createdon' />
                                        <attribute name='description' />
                                        <filter>
                                          <condition attribute='publisherid' operator='eq' value='{publisherId}' />
                                        </filter>
                                      </entity>
                                    </fetch>";
            var resultEntities = service.RetrieveMultiple(new FetchExpression(solutionFetchXML))?.Entities;
            if (resultEntities.Count > 0)
            {
                var solution = resultEntities[0];
                return solution;
            }
            return null;
        }

        private static Entity RetrievePublisherDetails(CrmServiceClient service, string publisherName)
        {
            var publisherFetchXML = $@"<fetch top='50'>
                                        <entity name='publisher'>
                                          <attribute name='customizationprefix' />
                                          <attribute name='address1_addresstypecode' />
                                          <attribute name='uniquename' />
                                          <attribute name='friendlyname' />
                                          <attribute name='publisherid' />
                                          <filter>
                                            <condition attribute='friendlyname' operator='eq' value='{publisherName}' />
                                          </filter>
                                        </entity>
                                      </fetch>";

            return service.RetrieveMultiple(new FetchExpression(publisherFetchXML))?.Entities.FirstOrDefault();
        }

        private static EntityMetadata RetrieveEntityMetadata(CrmServiceClient service)
        {
            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = entitySchemaName
            };
            RetrieveEntityResponse retrieveAccountEntityResponse = (RetrieveEntityResponse)service.Execute(retrieveEntityRequest);
            EntityMetadata accountEntity = retrieveAccountEntityResponse.EntityMetadata;

            Console.WriteLine($"PrimaryImageAttribute for '{entitySchemaName}' ==> {accountEntity.PrimaryImageAttribute}");
            return accountEntity;
        }

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
                CustomizationPrefix = "sample",
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

            //If it already exists, use it
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
                Console.WriteLine(String.Format("Created publisher: {0}.", _crmSdkPublisher.FriendlyName));
                prefix = _crmSdkPublisher.CustomizationPrefix;
            }
            Console.WriteLine("==============================");

            return prefix;
        }

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

                // Create a Solution
                //Define a solution
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


        private static void ExportSolution(CrmServiceClient service)
        {
            // Export or package a solution
            //Export an a solution
            Console.WriteLine($"Stated solution export methods...");
            Console.WriteLine("Solution unique name that needs to be exported: ");
            var solutionUniqueName = Console.ReadLine();
            ExportSolutionRequest exportSolutionRequest = new ExportSolutionRequest();
            exportSolutionRequest.Managed = false;
            exportSolutionRequest.SolutionName = solutionUniqueName;

            ExportSolutionResponse exportSolutionResponse = (ExportSolutionResponse)service.Execute(exportSolutionRequest);

            byte[] exportXml = exportSolutionResponse.ExportSolutionFile;
            string filename = solutionUniqueName + ".zip";

            Console.Write("Please provide download path: ");
            var exportPath = Path.Combine(Path.GetDirectoryName(Console.ReadLine()), filename);

            File.WriteAllBytes(exportPath, exportXml);

            Console.WriteLine("Solution exported to {0}.", exportPath + filename);
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteImageAttributeDemoEntity(bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want to delete the entity created for this sample? (y/n) [y]: ");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y") || answer == String.Empty);
            }

            if (deleteRecords)
            {
                DeleteEntityRequest der = new DeleteEntityRequest() { LogicalName = entitySchemaName.ToLower() };
                _service.Execute(der);
                Console.WriteLine("The Image Attribute Demo entity has been deleted.");
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
