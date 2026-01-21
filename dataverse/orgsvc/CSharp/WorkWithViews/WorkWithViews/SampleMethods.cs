using Microsoft.Crm.Sdk.Messages;
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
        
        private static Guid _customViewId;
        private static Guid _deactivatedViewId;
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

            //CreateRequiredRecords(service);
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
            // TODO Create entity records

            Console.WriteLine("Required records have been created.");
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

                service.Delete(SavedQuery.EntityLogicalName, _customViewId);
                Console.WriteLine("Entity records have been deleted.");
            }
        }

        /// <summary>
        /// Reactivates the view that was deactivated for this sample.
        /// <param name="prompt">Indicates whether to prompt the user to reactivate the view deactivated in this sample.</param>
        /// </summary>
        public static void ReactivateDeactivatedView(CrmServiceClient service, bool prompt)
        {
            bool reactivateView = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want to reactivate the \"Custom Public view\" view? (y/n)");
                String answer = Console.ReadLine();

                reactivateView = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (reactivateView)
            {
                var reactivateViewRequest = new SetStateRequest
                {
                    EntityMoniker = new EntityReference(SavedQuery.EntityLogicalName, _deactivatedViewId),
                    State = new OptionSetValue((int)SavedQueryState.Active),
                    Status = new OptionSetValue(1),
                };
                service.Execute(reactivateViewRequest);
                Console.WriteLine("The view has been reactivated.");
            }
        }

    }
}
