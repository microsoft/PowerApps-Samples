using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
        //Define Ids needed in this sample
        private static Guid _crmSdkPublisherId;
        private static System.String _customizationPrefix;
        private static Boolean _createdPublisher = false;
        private static Guid _importWebResourcesSampleSolutionId;
        private static System.String _ImportWebResourcesSolutionUniqueName;
        private static System.Guid[] _webResourceIds = new System.Guid[5];
        private static bool prompt = true;
        /// <summary>
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

            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        //Encodes the Web Resource File
        static public string getEncodedFileContents(String pathToFile)
        {
            FileStream fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read);
            byte[] binaryData = new byte[fs.Length];
            long bytesRead = fs.Read(binaryData, 0, (int)fs.Length);
            fs.Close();
            return System.Convert.ToBase64String(binaryData, 0, binaryData.Length);
        }

        /// <summary>
        /// This method creates a publisher and a solution to use when adding the Web resources.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {

            //Define a new publisher
            Publisher _crmSdkPublisher = new Publisher
            {
                UniqueName = "sdksamplesimportwebresourcessample",
                FriendlyName = "SDK Samples Import Web Resources Sample",
                SupportingWebsiteUrl = "https://learn.microsoft.com/power-apps/developer/data-platform/overview",
                CustomizationPrefix = "sample",
                EMailAddress = "someone@microsoft.com",
                Description = "This publisher is used by the Import Web Resources sample from the Software Development Kit (SDK) documentation."
            };

            //Does publisher already exist?
            var querySDKSamplePublisher = new QueryExpression
            {
                EntityName = Publisher.EntityLogicalName,
                ColumnSet = new ColumnSet("publisherid", "customizationprefix", "friendlyname", "isreadonly"),
                Criteria = new FilterExpression()
            };

            querySDKSamplePublisher.Criteria.AddCondition("uniquename", ConditionOperator.Equal, _crmSdkPublisher.UniqueName);
            EntityCollection querySDKSamplePublisherResults = service.RetrieveMultiple(querySDKSamplePublisher);
            Publisher SDKSamplePublisherResults = null;

            //If it already exists, use it
            if (querySDKSamplePublisherResults.Entities.Count > 0)
            {
                SDKSamplePublisherResults = (Publisher)querySDKSamplePublisherResults.Entities[0];
                _crmSdkPublisherId = (Guid)SDKSamplePublisherResults.PublisherId;
                _customizationPrefix = SDKSamplePublisherResults.CustomizationPrefix;

                Console.WriteLine("Using existing publisher: {0}", SDKSamplePublisherResults.FriendlyName);
            }
            //If it doesn't exist, create it
            if (SDKSamplePublisherResults == null)
            {
                _crmSdkPublisherId = service.Create(_crmSdkPublisher);
                Console.WriteLine(String.Format("Created publisher: {0}.", _crmSdkPublisher.FriendlyName));
                _customizationPrefix = _crmSdkPublisher.CustomizationPrefix;
                // Set this flag to delete the publisher if this sample created it.
                _createdPublisher = true;
                Console.WriteLine("Created new publisher: {0}", _crmSdkPublisher.FriendlyName);
            }

            // Create a Solution
            //Define a solution
            Solution solution = new Solution
            {
                UniqueName = "ImportWebResourcesSample",
                FriendlyName = "Import Web Resources Sample Solution",
                PublisherId = new EntityReference(Publisher.EntityLogicalName, _crmSdkPublisherId),
                Description = "This solution was created by the ImportWebResources sample code in the SDK samples.",
                Version = "1.0"
            };
            // Save save this variable  to use when creating the Web resources in the context of this solution.
            _ImportWebResourcesSolutionUniqueName = solution.UniqueName;

            //Check whether it already exists
            QueryExpression queryCheckForSampleSolution = new QueryExpression
            {
                EntityName = Solution.EntityLogicalName,
                ColumnSet = new ColumnSet("friendlyname"),
                Criteria = new FilterExpression()
            };
            queryCheckForSampleSolution.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solution.UniqueName);

            //Create the solution if it does not already exist.
            EntityCollection querySampleSolutionResults = service.RetrieveMultiple(queryCheckForSampleSolution);
            Solution SampleSolutionResults = null;
            if (querySampleSolutionResults.Entities.Count > 0)
            {
                SampleSolutionResults = (Solution)querySampleSolutionResults.Entities[0];
                _importWebResourcesSampleSolutionId = (Guid)SampleSolutionResults.SolutionId;
                Console.WriteLine("Using existing solution: {0}", SampleSolutionResults.FriendlyName);
            }
            if (SampleSolutionResults == null)
            {
                _importWebResourcesSampleSolutionId = service.Create(solution);
                Console.WriteLine("Created new solution: {0}", solution.FriendlyName);
            }



        }

        /// <summary>
        /// Deletes the custom entity record that was created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the entity created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y") || answer == String.Empty);
            }

            if (deleteRecords)
            {
                //delete Web Resources
                foreach (Guid id in _webResourceIds)
                {
                    service.Delete(WebResource.EntityLogicalName, id);
                }
                Console.WriteLine("Web Resource records have been deleted.");
                //Delete Solution

                service.Delete(Solution.EntityLogicalName, _importWebResourcesSampleSolutionId);
                Console.WriteLine("Solution has been deleted.");

                //
                if (_createdPublisher)
                {
                    // Delete the publisher
                    service.Delete(Publisher.EntityLogicalName, _crmSdkPublisherId);
                    Console.WriteLine("Publisher has been deleted.");
                }

            }
        }
    }
}
