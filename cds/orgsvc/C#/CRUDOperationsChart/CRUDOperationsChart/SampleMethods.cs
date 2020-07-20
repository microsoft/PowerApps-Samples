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
        private static Guid _orgOwnedVisualizationId;
        private static Guid _accountId;
        private static Guid[] _opportunitiyIds;
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
            // Create a sample account
            Account setupAccount = new Account { Name = "Sample Account" };
            _accountId = service.Create(setupAccount);
            Console.WriteLine("Created {0}.", setupAccount.Name);

            // Create some oppotunity records for the visualization
            Opportunity[] setupOpportunities = new Opportunity[]
                {
                    new Opportunity
                    {
                        Name = "Sample Opp 01",
                        EstimatedValue = new Money(120000.00m),
                        ActualValue = new Money(100000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 02",
                        EstimatedValue = new Money(240000.00m),
                        ActualValue = new Money(200000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 03",
                        EstimatedValue = new Money(360000.00m),
                        ActualValue = new Money(300000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 04",
                        EstimatedValue = new Money(500000.00m),
                        ActualValue = new Money(500000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 05",
                        EstimatedValue = new Money(110000.00m),
                        ActualValue = new Money(60000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 06",
                        EstimatedValue = new Money(90000.00m),
                        ActualValue = new Money(70000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 07",
                        EstimatedValue = new Money(620000.00m),
                        ActualValue = new Money(480000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 08",
                        EstimatedValue = new Money(440000.00m),
                        ActualValue = new Money(400000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 09",
                        EstimatedValue = new Money(410000.00m),
                        ActualValue = new Money(400000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp 10",
                        EstimatedValue = new Money(650000.00m),
                        ActualValue = new Money(650000.00m),
                        CustomerId = new EntityReference(Account.EntityLogicalName,
                            _accountId),
                        StepName = "Open"
                    }
                };

            _opportunitiyIds = (from opp in setupOpportunities
                                select service.Create(opp)).ToArray();

            Console.WriteLine("Created few opportunity records for {0}.", setupAccount.Name);

            return;
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
                service.Delete(SavedQueryVisualization.EntityLogicalName,
                    _orgOwnedVisualizationId);
                service.Delete("account", _accountId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
