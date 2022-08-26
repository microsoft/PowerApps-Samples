using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {

        private static Account _initialAccount;
        private static Lead _initialLead;
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

            _initialAccount = new Account()
            {
                Name = "Contoso, Ltd"
            };
            _initialAccount.Id = service.Create(_initialAccount);
            Console.WriteLine("  Created initial Account (Name={0})",
                _initialAccount.Name);

            _initialLead = new Lead()
            {
                Subject = "A Sample Lead",
                LastName = "Wilcox",
                FirstName = "Colin",
            };
            _initialLead.Id = service.Create(_initialLead);
            Console.WriteLine("  Created initial Lead (Subject={0}, Name={1} {2})",
                _initialLead.Subject,
                _initialLead.FirstName,
                _initialLead.LastName);
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
                string answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {

                service.Delete(Account.EntityLogicalName, _initialAccount.Id);
                service.Delete(Lead.EntityLogicalName, _initialLead.Id);
                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
