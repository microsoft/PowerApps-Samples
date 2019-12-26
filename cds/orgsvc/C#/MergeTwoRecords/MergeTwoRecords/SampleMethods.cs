using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {

        private static Guid _account1Id;
        private static Guid _account2Id;
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("9.0.0.0")))
            {
                //The environment version is lower than version 9.0.0.0
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
            // Create the first account, which will be merged into
            Account account1 = new Account();
            account1.Name = "Fourth Coffee";
            account1.Description = "Coffee House";

            _account1Id = service.Create(account1);
            Console.WriteLine("Account 1 created with GUID {{{0}}}", _account1Id);
            Console.WriteLine("  Name: {0}", account1.Name);
            Console.WriteLine("  Description: {0}", account1.Description);

            // Create the second account, which will be merged from
            Account account2 = new Account();

            account2.Name = "Fourth Coffee";
            account2.NumberOfEmployees = 55;

            _account2Id = service.Create(account2);
            Console.WriteLine("Account 2 created with GUID {{{0}}}", _account2Id);
            Console.WriteLine("  Name: {0}", account2.Name);
            Console.WriteLine("  Number of Employees: {0}", account2.NumberOfEmployees);
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

                service.Delete(Account.EntityLogicalName, _account1Id);
                service.Delete(Account.EntityLogicalName, _account2Id);
                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
