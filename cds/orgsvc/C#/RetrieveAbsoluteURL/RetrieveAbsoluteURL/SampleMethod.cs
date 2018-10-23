using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
        private static Guid _spSiteId;
        private static Guid _spDocLocId;
        private static bool prompt = true;
        private static String _siteAbsoluteURL = "http://www.example.com";

        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
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

        /// <summary>
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Instantiate a SharePoint site object.
            // See the Entity Metadata topic in the SDK documentation to determine 
            // which attributes must be set for each entity.
            SharePointSite spSite = new SharePointSite
            {
                Name = "Sample SharePoint Site",
                Description = "Sample SharePoint Site Location record",
                AbsoluteURL = _siteAbsoluteURL,
                IsGridPresent = true
            };

            // Create a SharePoint site record named Sample SharePoint Site.
            _spSiteId = service.Create(spSite);
            Console.WriteLine("{0} created.", spSite.Name);

            // Instantiate a SharePoint document location object.
            // See the Entity Metadata topic in the SDK documentation to determine 
            // which attributes must be set for each entity.
            SharePointDocumentLocation spDocLoc = new SharePointDocumentLocation
            {
                Name = "Sample SharePoint Document Location",
                Description = "Sample SharePoint Document Location record",

                // Set the Sample SharePoint Site created earlier as the parent site.
                ParentSiteOrLocation = new EntityReference(SharePointSite.EntityLogicalName, _spSiteId),
                RelativeUrl = "spdocloc"
            };

            // Create a SharePoint document location record named Sample SharePoint Document Location.
            _spDocLocId = service.Create(spDocLoc);
            Console.WriteLine("{0} created.", spDocLoc.Name);
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
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
                service.Delete(SharePointDocumentLocation.EntityLogicalName, _spDocLocId);
                service.Delete(SharePointSite.EntityLogicalName, _spSiteId);
                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
