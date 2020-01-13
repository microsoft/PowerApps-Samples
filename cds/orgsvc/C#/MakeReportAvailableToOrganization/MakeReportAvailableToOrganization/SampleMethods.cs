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
        // Define the IDs needed for this sample.
        private static Guid _reportId;
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

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Creates the email activity.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Define an anonymous type to define the possible values for
            // report type
            var ReportTypeCode = new
            {
                ReportingServicesReport = 1,
                OtherReport = 2,
                LinkedReport = 3
            };

            // Create a report 
            var newReport = new Report
            {
                Name = "Sample Report",
                Description = "Report created by the SDK code sample.",
                ReportTypeCode = new OptionSetValue((int)ReportTypeCode.OtherReport),

                //Base64-encoded "Hello, I am a text file."
                BodyBinary = "SGVsbG8sIEkgYW0gYSB0ZXh0IGZpbGUu",
                FileName = "textfile.txt",
                IsPersonal = true
            };
            _reportId = service.Create(newReport);
            Console.WriteLine("Created {0}.", newReport.Name);
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
                Console.WriteLine("\nDo you want these entity records deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                // Delete all records created in this sample.
                service.Delete(Report.EntityLogicalName, _reportId);
                Console.WriteLine("Entity record(s) have been deleted.");  
            }
        }

    }
}
