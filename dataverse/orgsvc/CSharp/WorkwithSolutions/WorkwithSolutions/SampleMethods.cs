using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {

        private static Guid _powerappsSdkPublisherId;
        private static System.String _customizationPrefix;
        private static Guid _solutionsSampleSolutionId;
        static System.String ManagedSolutionLocation = @"C:\temp\ManagedSolutionForImportExample.zip";
        static String outputDir = @"C:\temp\";
        private static bool prompt = true;
        // Specify which language code to use in the sample. If you are using a language
        // other than US English, you will need to modify this value accordingly.
        // See https://learn.microsoft.com/previous-versions/windows/embedded/ms912047(v=winembedded.10)
        private const int _languageCode = 1033;
        // <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }

            //Import Sample Solution
            /*if(SampleHelpers.ImportSolution(service,"SampleSolution", "SampleSolution.zip"))
            {
                //Wait a minute if the solution is being imported. This will give time for the new metadata to be cached.
                Thread.Sleep(TimeSpan.FromSeconds(60));
            }*/

            CreateRequiredRecords(service);

        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            SampleHelpers.DeleteSolution(service, "SampleSolution");
        }
        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create a managed solution for the Install or upgrade a solution sample

            Guid _tempPublisherId = new Guid();
            System.String _tempCustomizationPrefix = "new";
            Guid _tempSolutionsSampleSolutionId = new Guid();
            System.String _TempGlobalOptionSetName = "_TempSampleGlobalOptionSetName";
            Boolean _publisherCreated = false;
            Boolean _solutionCreated = false;


            //Define a new publisher
            Publisher _powerappsSdkPublisher = new Publisher
            {
                UniqueName = "sdksamples",
                FriendlyName = "PowerApps SDK Samples",
                SupportingWebsiteUrl = "https://learn.microsoft.com/power-apps/developer/data-platform/overview",
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

            querySDKSamplePublisher.Criteria.AddCondition("uniquename", ConditionOperator.Equal, _powerappsSdkPublisher.UniqueName);
            EntityCollection querySDKSamplePublisherResults = service.RetrieveMultiple(querySDKSamplePublisher);
            Publisher SDKSamplePublisherResults = null;

            //If it already exists, use it
            if (querySDKSamplePublisherResults.Entities.Count > 0)
            {
                SDKSamplePublisherResults = (Publisher)querySDKSamplePublisherResults.Entities[0];
                _tempPublisherId = (Guid)SDKSamplePublisherResults.PublisherId;
                _tempCustomizationPrefix = SDKSamplePublisherResults.CustomizationPrefix;
            }
            //If it doesn't exist, create it
            if (SDKSamplePublisherResults == null)
            {
                _tempPublisherId = service.Create(_powerappsSdkPublisher);
                _tempCustomizationPrefix = _powerappsSdkPublisher.CustomizationPrefix;
                _publisherCreated = true;
            }

            //Create a Solution
            //Define a solution
            Solution solution = new Solution
            {
                UniqueName = "samplesolutionforImport",
                FriendlyName = "Sample Solution for Import",
                PublisherId = new EntityReference(Publisher.EntityLogicalName, _tempPublisherId),
                Description = "This solution was created by the WorkWithSolutions sample code in the PowerApps SDK samples.",
                Version = "1.0"
            };

            //Check whether it already exists
            QueryExpression querySampleSolution = new QueryExpression
            {
                EntityName = Solution.EntityLogicalName,
                ColumnSet = new ColumnSet(),
                Criteria = new FilterExpression()
            };
            querySampleSolution.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solution.UniqueName);

            EntityCollection querySampleSolutionResults = service.RetrieveMultiple(querySampleSolution);
            Solution SampleSolutionResults = null;
            if (querySampleSolutionResults.Entities.Count > 0)
            {
                SampleSolutionResults = (Solution)querySampleSolutionResults.Entities[0];
                _tempSolutionsSampleSolutionId = (Guid)SampleSolutionResults.SolutionId;
            }
            if (SampleSolutionResults == null)
            {
                _tempSolutionsSampleSolutionId = service.Create(solution);
                _solutionCreated = true;
            }

            // Add a solution Component
            OptionSetMetadata optionSetMetadata = new OptionSetMetadata()
            {
                Name = _tempCustomizationPrefix + _TempGlobalOptionSetName,
                DisplayName = new Label("Example Option Set", _languageCode),
                IsGlobal = true,
                OptionSetType = OptionSetType.Picklist,
                Options =
                    {
                        new OptionMetadata(new Label("Option A", _languageCode), null),
                        new OptionMetadata(new Label("Option B", _languageCode), null )
                    }
            };
            CreateOptionSetRequest createOptionSetRequest = new CreateOptionSetRequest
            {
                OptionSet = optionSetMetadata,
                SolutionUniqueName = solution.UniqueName

            };


            service.Execute(createOptionSetRequest);

            //Export an a solution


            ExportSolutionRequest exportSolutionRequest = new ExportSolutionRequest();
            exportSolutionRequest.Managed = true;
            exportSolutionRequest.SolutionName = solution.UniqueName;

            ExportSolutionResponse exportSolutionResponse = (ExportSolutionResponse)service.Execute(exportSolutionRequest);

            byte[] exportXml = exportSolutionResponse.ExportSolutionFile;
            System.IO.Directory.CreateDirectory(outputDir);
            File.WriteAllBytes(ManagedSolutionLocation, exportXml);

            // Delete the solution and the components so it can be installed.

            DeleteOptionSetRequest delOptSetReq = new DeleteOptionSetRequest { Name = (_tempCustomizationPrefix + _TempGlobalOptionSetName).ToLower() };
            service.Execute(delOptSetReq);

            if (_solutionCreated)
            {
                service.Delete(Solution.EntityLogicalName, _tempSolutionsSampleSolutionId);
            }

            if (_publisherCreated)
            {
                service.Delete(Publisher.EntityLogicalName, _tempPublisherId);
            }

            Console.WriteLine("Managed Solution created and copied to {0}", ManagedSolutionLocation);

        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                service.Delete(Solution.EntityLogicalName, _solutionsSampleSolutionId);
                service.Delete(Publisher.EntityLogicalName, _powerappsSdkPublisherId);
                // Remove the managed solution created by the create required fields code.
                File.Delete(ManagedSolutionLocation);


                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
