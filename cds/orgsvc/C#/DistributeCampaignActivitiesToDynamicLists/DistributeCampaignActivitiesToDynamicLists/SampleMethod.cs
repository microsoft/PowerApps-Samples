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
        // TODO Define some class members
        //private static Guid member1;
        private static Guid _salesManagerId;
        private static Guid _accountId;
        private static Guid _campaignId;
        private static Guid _campaignActivityId;
        private static Guid _dynamicListId;
        private static Guid _staticListId;
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
            
            // Retrieve a sales manager.
            _salesManagerId = SystemUserProvider.RetrieveMarketingManager(service);

            // Create an account.
            var account = new Account()
            {
                Name = "Fourth Coffee",
                Address1_City = "Seattle"
            };
            _accountId = service.Create(account);
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
                service.Delete(List.EntityLogicalName, _staticListId);
                service.Delete(List.EntityLogicalName, _dynamicListId);
                service.Delete("campaignactivity", _campaignActivityId);
                service.Delete("campaign", _campaignId);
                service.Delete("account", _accountId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
