using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
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
        // Define the IDs as well as strings needed for this sample.
        public static Guid _instanceAttributeID;
        public static Guid _seriesAttributeID;
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

            
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user to delete 
        /// the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records to be deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                DeleteAttributeRequest delSeriesAttribute = new DeleteAttributeRequest
                {
                    LogicalName = "new_customappseriesattribute",
                    EntityLogicalName = RecurringAppointmentMaster.EntityLogicalName
                };
                service.Execute(delSeriesAttribute);

                DeleteAttributeRequest delInstanceAttribute = new DeleteAttributeRequest
                {
                    LogicalName = "new_customappinstanceattribute",
                    EntityLogicalName = Appointment.EntityLogicalName
                };
                service.Execute(delInstanceAttribute);

                // Publish all the changes to the solution.
                PublishAllXmlRequest delRequest = new PublishAllXmlRequest();
                service.Execute(delRequest);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
