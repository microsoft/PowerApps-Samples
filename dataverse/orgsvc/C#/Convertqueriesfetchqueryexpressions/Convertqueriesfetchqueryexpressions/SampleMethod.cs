using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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
        private static Guid accountId;
        private static readonly List<Guid> contactIdList = new List<Guid>();
        private static readonly List<Guid> opportunityIdList = new List<Guid>();
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
            DoQueryExpressionToFetchXmlConversion(service);
            DoFetchXmlToQueryExpressionConversion(service);
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

            // Create an account.
            var account = new Account
            {
                Name = "Litware, Inc.",
                Address1_StateOrProvince = "Colorado"
            };
            accountId = (service.Create(account));

            // Create the two contacts.
            var contact = new Contact()
            {
                FirstName = "Ben",
                LastName = "Andrews",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Redmond",
                Address1_StateOrProvince = "WA",
                Address1_Telephone1 = "(206)555-5555",
                ParentCustomerId = new EntityReference
                {
                    Id = accountId,
                    LogicalName = account.LogicalName
                }
            };
            contactIdList.Add(service.Create(contact));

            contact = new Contact()
            {
                FirstName = "Colin",
                LastName = "Wilcox",
                EMailAddress1 = "sample@example.com",
                Address1_City = "Bellevue",
                Address1_StateOrProvince = "WA",
                Address1_Telephone1 = "(425)555-5555",
                ParentCustomerId = new EntityReference
                {
                    Id = accountId,
                    LogicalName = account.LogicalName
                }
            };
            contactIdList.Add(service.Create(contact));

            // Create two opportunities.
            var opportunity = new Opportunity()
            {
                Name = "Litware, Inc. Opportunity 1",
                EstimatedCloseDate = DateTime.Now.AddMonths(6),
                CustomerId = new EntityReference
                {
                    Id = accountId,
                    LogicalName = account.LogicalName
                }
            };
            opportunityIdList.Add(service.Create(opportunity));

            opportunity = new Opportunity()
            {
                Name = "Litware, Inc. Opportunity 2",
                EstimatedCloseDate = DateTime.Now.AddYears(4),
                CustomerId = new EntityReference
                {
                    Id = accountId,
                    LogicalName = account.LogicalName
                }
            };
            opportunityIdList.Add(service.Create(opportunity));
        }

        private static void DoQueryExpressionToFetchXmlConversion(CrmServiceClient service)
        {
            // Build a query expression that we will turn into FetchXML.
            var queryExpression = new QueryExpression()
            {
                Distinct = false,
                EntityName = Contact.EntityLogicalName,
                ColumnSet = new ColumnSet("fullname", "address1_telephone1"),
                LinkEntities =
                    {
                        new LinkEntity
                        {
                            JoinOperator = JoinOperator.LeftOuter,
                            LinkFromAttributeName = "parentcustomerid",
                            LinkFromEntityName = Contact.EntityLogicalName,
                            LinkToAttributeName = "accountid",
                            LinkToEntityName = Account.EntityLogicalName,
                            LinkCriteria =
                            {
                                Conditions =
                                {
                                    new ConditionExpression("name", ConditionOperator.Equal, "Litware, Inc.")
                                }
                            }
                        }
                    },
                Criteria =
                {
                    Filters =
                        {
                            new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("address1_stateorprovince", ConditionOperator.Equal, "WA"),
                                    new ConditionExpression("address1_city", ConditionOperator.In, new String[] {"Redmond", "Bellevue" , "Kirkland", "Seattle"}),
                                    new ConditionExpression("createdon", ConditionOperator.LastXDays, 30),
                                    new ConditionExpression("emailaddress1", ConditionOperator.NotNull)
                                },
                            },
                            new FilterExpression
                            {
                                FilterOperator = LogicalOperator.Or,
                                Conditions =
                                {
                                    new ConditionExpression("address1_telephone1", ConditionOperator.Like, "(206)%"),
                                    new ConditionExpression("address1_telephone1", ConditionOperator.Like, "(425)%")
                                }
                            }
                        }
                }
            };

            // Run the query as a query expression.
            EntityCollection queryExpressionResult =
                service.RetrieveMultiple(queryExpression);
            Console.WriteLine("Output for query as QueryExpression:");
            DisplayContactQueryResults(service,queryExpressionResult);

            // Convert the query expression to FetchXML.
            var conversionRequest = new QueryExpressionToFetchXmlRequest
            {
                Query = queryExpression
            };
            var conversionResponse =
                (QueryExpressionToFetchXmlResponse)service.Execute(conversionRequest);

            // Use the converted query to make a retrieve multiple request to Microsoft Dynamics CRM.
            String fetchXml = conversionResponse.FetchXml;
            var fetchQuery = new FetchExpression(fetchXml);
            EntityCollection result = service.RetrieveMultiple(fetchQuery);

            // Display the results.
            Console.WriteLine("\nOutput for query after conversion to FetchXML:");
            DisplayContactQueryResults(service,result);

        }

        private static void DisplayContactQueryResults(CrmServiceClient service, EntityCollection result)
        {
            Console.WriteLine("List all contacts matching specified parameters");
            Console.WriteLine("===============================================");
            foreach (Entity entity in result.Entities)
            {
                var contact = entity.ToEntity<Contact>();
                Console.WriteLine("Contact ID: {0}", contact.Id);
                Console.WriteLine("Contact Name: {0}", contact.FullName);
                Console.WriteLine("Contact Phone: {0}", contact.Address1_Telephone1);
            }
            Console.WriteLine("<End of Listing>");
            Console.WriteLine();
        }

        private static void DoFetchXmlToQueryExpressionConversion(CrmServiceClient service)
        {
            // Create a Fetch query that we will convert into a query expression.
            var fetchXml =
                @"<fetch mapping='logical' version='1.0'>
                    <entity name='opportunity'>
                        <attribute name='name' />
                        <filter>
                            <condition attribute='estimatedclosedate' operator='next-x-fiscal-years' value='3' />
                        </filter>
                        <link-entity name='account' from='accountid' to='customerid'>
                            <link-entity name='contact' from='parentcustomerid' to='accountid'>
                                <attribute name='fullname' />
                                <filter>
                                    <condition attribute='address1_city' operator='eq' value='Bellevue' />
                                    <condition attribute='address1_stateorprovince' operator='eq' value='WA' />
                                </filter>
                            </link-entity>
                        </link-entity>
                    </entity>
                </fetch>";

            // Run the query with the FetchXML.
            var fetchExpression = new FetchExpression(fetchXml);
            EntityCollection fetchResult =
                service.RetrieveMultiple(fetchExpression);
            Console.WriteLine("\nOutput for query as FetchXML:");
            DisplayOpportunityQueryResults(service,fetchResult);

            // Convert the FetchXML into a query expression.
            var conversionRequest = new FetchXmlToQueryExpressionRequest
            {
                FetchXml = fetchXml
            };

            var conversionResponse =
                (FetchXmlToQueryExpressionResponse)service.Execute(conversionRequest);

            // Use the newly converted query expression to make a retrieve multiple
            // request to Microsoft Dynamics CRM.
            QueryExpression queryExpression = conversionResponse.Query;

            EntityCollection result = service.RetrieveMultiple(queryExpression);

            // Display the results.
            Console.WriteLine("\nOutput for query after conversion to QueryExpression:");
            DisplayOpportunityQueryResults(service,result);

        }

        private static void DisplayOpportunityQueryResults(CrmServiceClient service,EntityCollection result)
        {
            Console.WriteLine("List all opportunities matching specified parameters.");
            Console.WriteLine("===========================================================================");
            foreach (Entity entity in result.Entities)
            {
                var opportunity = entity.ToEntity<Opportunity>();
                Console.WriteLine("Opportunity ID: {0}", opportunity.Id);
                Console.WriteLine("Opportunity: {0}", opportunity.Name);
                var aliased = (AliasedValue)opportunity["contact2.fullname"];
                var contactName = (String)aliased.Value;
                Console.WriteLine("Associated contact: {0}", contactName);
            }
            Console.WriteLine("<End of Listing>");
            Console.WriteLine();
        }
        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool toBeDeleted = true;

            if (prompt)
            {
                // Ask the user if the created entities should be deleted.
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
                if (answer.StartsWith("y") ||
                    answer.StartsWith("Y") ||
                    answer == String.Empty)
                {
                    toBeDeleted = true;
                }
                else
                {
                    toBeDeleted = false;
                }
            }

            if (toBeDeleted)
            {
                // Delete all records created in this sample.
                foreach (Guid contactId in contactIdList)
                {
                    service.Delete(Contact.EntityLogicalName, contactId);
                }

                foreach (Guid opportunityId in opportunityIdList)
                {
                    service.Delete(Opportunity.EntityLogicalName, opportunityId);
                }

                service.Delete(Account.EntityLogicalName, accountId);

                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }
    }
}
