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
        // Define the IDs needed for this sample.    
        public static Guid _accountId;
        public static Guid[] _setupOpportunitiyIds;
        private static bool prompt = true;
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

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Create a sample account record.
        /// Create 3 opportunity records associate to the account record.
        /// Mark all 3 opportunity records as won.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {

            var OpportunityStatusCodes = new
            {
                Won = 3
            };

            // Instantiate a account entity record and set its property values.
            // See the Entity Metadata topic in the SDK documentation
            // to determine which attributes must be set for each entity.
            Account setupAccount = new Account
            {
                Name = "Example Account"
            };

            _accountId = service.Create(setupAccount);

            Console.WriteLine("Created {0}.", setupAccount.Name);


            EntityReference setupCustomer = new EntityReference
            {
                LogicalName = Account.EntityLogicalName,
                Id = _accountId
            };

            // Create 3 sample opportunities.
            // Instantiate opportunity entity records and set there property values.
            // See the Entity Metadata topic in the SDK documentation
            // to determine which attributes must be set for each entity.
            Opportunity[] setupOpportunities = new[]
                {
                    new Opportunity
                    {
                        Name = "Sample Opp 1",
                        EstimatedValue = new Money {Value = 120000.00m},
                        CustomerId = setupCustomer,
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp With Duplicate Name",
                        EstimatedValue = new Money { Value = 240000.00m},
                        CustomerId = setupCustomer,
                    },
                    new Opportunity
                    {
                        Name = "Sample Opp With Duplicate Name",
                        EstimatedValue = new Money{ Value = 360000.00m},
                        CustomerId = setupCustomer,
                    },
                };

            _setupOpportunitiyIds = (from opp in setupOpportunities
                                     select service.Create(opp)).ToArray();

            // "Win" each of the opportunities
            foreach (Guid oppId in _setupOpportunitiyIds)
            {
                WinOpportunityRequest winRequest = new WinOpportunityRequest
                {
                    OpportunityClose = new OpportunityClose
                    {
                        OpportunityId = new EntityReference
                        {
                            LogicalName = "opportunity",
                            Id = oppId
                        },

                        // Mark these entities as won in the year 2008 to 
                        // have some testable results.
                        ActualEnd = new DateTime(2009, 11, 1, 12, 0, 0)
                    },
                    Status = new OptionSetValue(OpportunityStatusCodes.Won)
                };
                service.Execute(winRequest);
            }

            Console.WriteLine("Created {0} sample opportunity records and updated as won for this sample.", _setupOpportunitiyIds.Length);

            return;
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
                foreach (Guid oppId in _setupOpportunitiyIds)
                {
                    service.Delete("opportunity", oppId);
                }

                service.Delete("account", _accountId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

    }
}
