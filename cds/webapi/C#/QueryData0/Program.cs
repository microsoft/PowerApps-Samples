using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PowerApps.Samples
{
    class Program
    {
        private static readonly IConfiguration appSettings = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        //Establishes the MSAL app to manage caching access tokens
        private static readonly IPublicClientApplication app = PublicClientApplicationBuilder.Create(appSettings["ClientId"])
            .WithRedirectUri(appSettings["RedirectUri"])
            .WithAuthority(appSettings["Authority"])
            .Build();

        //Configures the service
        private static readonly Config config = new Config
        {
            Url = appSettings["Url"],
            GetAccessToken = GetToken, //Function defined in app (below) to manage getting OAuth token
            //Optional settings that have defaults if not specified:
            MaxRetries = byte.Parse(appSettings["MaxRetries"]), //Default: 2
            TimeoutInSeconds = ushort.Parse(appSettings["TimeoutInSeconds"]), //Default: 120
            Version = appSettings["Version"], //Default 9.1
            CallerObjectId = new Guid(appSettings["CallerObjectId"]) //Default empty Guid
        };

        //References for records referenced in this sample
        private static readonly List<EntityReference> entityRefs = new List<EntityReference>();

        //Control whether records created for this sample should be deleted.
        private static readonly bool deleteCreatedRecords = true;

        private static EntityReference accountContosoRef, contactYvonneMcKayRef;

        public static async Task Main()
        {
            try
            {
                var service = new Service(config);
                Console.WriteLine("\n--Starting Query Data --");
                //Create the records that this sample will query.
                await CreateRequiredRecords(service);

                //Get the id and name of the account created to use as a filter.
                var contoso = await service.Retrieve<Account>(accountContosoRef,"$select=accountid,name");
                var contosoId = contoso.Id;
                string contosoName = contoso.name;

                #region Selecting specific properties

                // Basic query: Query using $select against a contact entity to get the properties you want.
                // For performance best practice, always use $select, otherwise all properties are returned
                Console.WriteLine("-- Basic Query --");

                var contactYvonneMcKay = await service.Retrieve<Contact>(contactYvonneMcKayRef, "$select=fullname,jobtitle,annualincome", true);


                Console.WriteLine($"Contact basic info:\n" +
                        $"\tFullname: {contactYvonneMcKay.fullname}\n" +
                        $"\tJobtitle: {contactYvonneMcKay.jobtitle}\n" +
                        $"\tAnnualincome (unformatted): {contactYvonneMcKay.annualincome} \n" +
                        $"\tAnnualincome (formatted): {contactYvonneMcKay.annualincome_display} \n");

                #endregion Selecting specific properties

                #region Using query functions

                // Filter criteria:
                // Applying filters to get targeted data.
                // 1) Using standard query functions (e.g.: contains, endswith, startswith)
                // 2) Using CDS query functions (e.g.: LastXhours, Last7Days, Today, Between, In, ...)
                // 3) Using filter operators and logical operators (e.g.: eq, ne, gt, and, or, etc…)
                // 4) Set precedence using parenthesis (e.g.: ((criteria1) and (criteria2)) or (criteria3)
                // For more info, see:
                //https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/webapi/query-data-web-api#filter-results

                Console.WriteLine("-- Filter Criteria --");
                //Filter 1: Using standard query functions to filter results.  In this operation, we
                //will query for all contacts with fullname containing the string "(sample)".

                string filter1Query = "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(fullname,'(sample)') and " +
                        $"_parentcustomerid_value eq {contosoId}";

                var containsSampleinFullNameCollection = await service.RetrieveMultiple<Contact>(filter1Query, true);

                WriteContactResultsTable(
                        "Contacts filtered by fullname containing '(sample)':",
                        containsSampleinFullNameCollection.Value);

                //Filter 2: Using CDS query functions to filter results. In this operation, we will query
                //for all contacts that were created in the last hour. For complete list of CDS query
                //functions, see: https://docs.microsoft.com/dynamics365/customer-engagement/web-api/queryfunctions

                string filter2Query = "$select=fullname,jobtitle,annualincome&" +
                    "$filter=Microsoft.Dynamics.CRM.LastXHours(PropertyName='createdon',PropertyValue='1') and " +
                    $"_parentcustomerid_value eq {contosoId}";

                var createdInLastHourCollection = await service.RetrieveMultiple<Contact>(filter2Query, true);

                WriteContactResultsTable(
                        "Contacts that were created within the last 1hr:",
                        createdInLastHourCollection.Value);


                //Filter 3: Using operators. Building on the previous operation, we further limit
                //the results by the contact's income. For more info on standard filter operators,
                //https://docs.microsoft.com/powerapps/developer/common-data-service/webapi/query-data-web-api#filter-results

                string filter3Query = "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(fullname,'(sample)') and " +
                        "annualincome gt 55000  and " +
                        $"_parentcustomerid_value eq {contosoId}";

                var highIncomeContacts = await service.RetrieveMultiple<Contact>(filter3Query, true);

                WriteContactResultsTable(
                        "Contacts with '(sample)' in name and income above $55,000:",
                        highIncomeContacts.Value);

                //Filter 4: Set precedence using parentheses. Continue building on the previous
                //operation, we further limit results by job title. Parentheses and the order of
                //filter statements can impact results returned.

                string filter4Query = "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(fullname,'(sample)') and " +
                        "(contains(jobtitle, 'senior') or " +
                        "contains(jobtitle,'manager')) and " +
                        "annualincome gt 55000 and " +
                        $"_parentcustomerid_value eq {contosoId}";

                var seniorOrSpecialistsCollection = await service.RetrieveMultiple<Contact>(filter4Query, true);

                WriteContactResultsTable(
                        "Contacts with '(sample)' in name senior jobtitle or high income:",
                        seniorOrSpecialistsCollection.Value);

                #endregion Using query functions

                #region Ordering and aliases

                //Results can be ordered in descending or ascending order.
                Console.WriteLine("\n-- Order Results --");

                string orderedResultsQuery = "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(fullname,'(sample)')and " +
                        $"_parentcustomerid_value eq {contosoId}&" +
                        "$orderby=jobtitle asc, annualincome desc";

                var orderedResults = await service.RetrieveMultiple<Contact>(orderedResultsQuery, true);

                WriteContactResultsTable(
                        "Contacts ordered by jobtitle (Ascending) and annualincome (descending)",
                        orderedResults.Value);

                //Parameterized aliases can be used as parameters in a query. These parameters can be used
                //in $filter and $orderby options. Using the previous operation as basis, parameterizing the
                //query will give us the same results. For more info, see:
                //https://docs.microsoft.com/powerapps/developer/common-data-service/webapi/use-web-api-functions#passing-parameters-to-a-function

                Console.WriteLine("\n-- Parameterized Aliases --");

                string orderedResultsWithParamsQuery = "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(@p1,'(sample)') and " +
                        "@p2 eq @p3&" +
                        "$orderby=@p4 asc, @p5 desc&" +
                        "@p1=fullname&" +
                        "@p2=_parentcustomerid_value&" +
                        $"@p3={contosoId}&" +
                        "@p4=jobtitle&" +
                        "@p5=annualincome";

                var orderedResultsWithParams = await service.RetrieveMultiple<Contact>(orderedResultsWithParamsQuery, true);

                WriteContactResultsTable(
                        "Contacts ordered by jobtitle (Ascending) and annualincome (descending)",
                        orderedResultsWithParams.Value);

                #endregion Ordering and aliases

                #region Limit results

                //To limit records returned, use the $top query option.  Specifying a limit number for $top
                //returns at most that number of results per request. Extra results are ignored.
                //For more information, see:
                // https://docs.microsoft.com/powerapps/developer/common-data-service/webapi/query-data-web-api#use-top-query-option
                Console.WriteLine("\n-- Top Results --");

                string topFiveQuery = "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(fullname,'(sample)') and " +
                        $"_parentcustomerid_value eq {contosoId}&" +
                        "$top=5";

                var topFive = await service.RetrieveMultiple<Contact>(topFiveQuery, true);

                WriteContactResultsTable("Contacts top 5 results:", topFive.Value);

                //Result count - count the number of results matching the filter criteria.
                //Tip: Use count together with the "odata.maxpagesize" to calculate the number of pages in
                //the query.  Note: CDS has a max record limit of 5000 records per response.
                Console.WriteLine("\n-- Result Count --");
                //1) Get a count of a collection without the data.
                var count = await service.GetCount("contacts");
                Console.WriteLine($"\nThe contacts collection has {count} contacts.");
                //2) Get a count that can go above 5000
                RetrieveTotalRecordCountResponse totalContactsCount = await service.RetrieveTotalRecordCount(Contact.LogicalName,Account.LogicalName);
                var countOfContacts = totalContactsCount.EntityRecordCountCollection.Values[0];
                Console.WriteLine($"\nThe contacts table has {countOfContacts} contacts.");
                //When below 5000, this number is lower because the total count isn't recalculated with every operation

                //  3) Get a count along with the data.

                string countWithDataQuery = "$select=fullname,jobtitle,annualincome&" +
                        "$filter=(contains(jobtitle,'senior') or contains(jobtitle, 'manager')) and " +
                        $"_parentcustomerid_value eq {contosoId}" +
                        "&$count=true";

                var countWithData = await service.RetrieveMultiple<Contact>(countWithDataQuery, true);

                WriteContactResultsTable($"{countWithData.Count} " +
                        $"Contacts with 'senior' or 'manager' in job title:",
                        countWithData.Value);

                #endregion Limit results

                #region Expanding results

                //The expand option retrieves related information.
                //To retrieve information on associated entities in the same request, use the $expand
                //query option on navigation properties.
                //  1) Expand using single-valued navigation properties (e.g.: via the 'primarycontactid')
                //  2) Expand using partner property (e.g.: from contact to account via the 'account_primary_contact')
                //  3) Expand using collection-valued navigation properties (e.g.: via the 'contact_customer_accounts')
                //  4) Expand using multiple navigation property types in a single request.
                //  5) Multi-level expands

                // Tip: For performance best practice, always use $select statement in an expand option.
                Console.WriteLine("\n-- Expanding Results --");

                //1) Expand using the 'primarycontactid' single-valued navigation property of Contoso.

                var expandPrimaryContactIdQuery = "$select=name&" +
                        "$expand=primarycontactid($select=fullname,jobtitle,annualincome)";

                var accountContoso = await service.Retrieve<Account>(accountContosoRef, expandPrimaryContactIdQuery);

                Console.WriteLine($"\nAccount {accountContoso.name} has the following primary contact person:\n" +
                     $"\tFullname: {accountContoso.primarycontactid.fullname} \n" +
                     $"\tJobtitle: {accountContoso.primarycontactid.jobtitle} \n" +
                     $"\tAnnualincome: {accountContoso.primarycontactid.annualincome}");


                //2) Expand using the 'account_primary_contact' partner property.

                var expandAccountPrimaryContactQuery = "$select=fullname,jobtitle,annualincome&" +
                    "$expand=account_primary_contact($select=name)";

                 contactYvonneMcKay = await service.Retrieve<Contact>(contactYvonneMcKayRef, expandAccountPrimaryContactQuery);

                Console.WriteLine($"\nContact '{contactYvonneMcKay.fullname}' is the primary contact for the following accounts:");
                foreach (Account account in contactYvonneMcKay.account_primary_contact)
                {
                    Console.WriteLine($"\t{account.name}");
                }

                //3) Expand using the collection-valued 'contact_customer_accounts' navigation property.

                var expandContactCustomerAccountsQuery = "$select=name&" +
                        "$expand=contact_customer_accounts($select=fullname,jobtitle,annualincome)";

                accountContoso = await service.Retrieve<Account>(accountContosoRef, expandContactCustomerAccountsQuery, true);

                WriteContactResultsTable(
                        $"Account '{accountContoso.name}' has the following contact customers:",
                        accountContoso.contact_customer_accounts);

                //4) Expand using multiple navigation property types in a single request, specifically:
                //   primarycontactid, contact_customer_accounts, and Account_Tasks.

                Console.WriteLine("\n-- Expanding multiple property types in one request -- ");

                var expandMultipleQuery = "$select=name&" +
                        "$expand=primarycontactid($select=fullname,jobtitle,annualincome)," +
                        "contact_customer_accounts($select=fullname,jobtitle,annualincome)," +
                        "Account_Tasks($select=subject,description)";

                accountContoso = await service.Retrieve<Account>(accountContosoRef, expandMultipleQuery, true);

                Console.WriteLine($"\nAccount {accountContoso.name} has the following primary contact person:\n" +
                                    $"\tFullname: {accountContoso.primarycontactid.fullname} \n" +
                                    $"\tJobtitle: {accountContoso.primarycontactid.jobtitle} \n" +
                                    $"\tAnnualincome: {accountContoso.primarycontactid.annualincome}");

                WriteContactResultsTable(
                        $"Account '{accountContoso.name}' has the following contact customers:",
                        accountContoso.contact_customer_accounts);

                Console.WriteLine($"\nAccount '{accountContoso.name}' has the following tasks:");

                foreach (TaskActivity task in accountContoso.Account_Tasks)
                {
                    Console.WriteLine($"\t{task.subject}");
                }

                // 5) Multi-level expands
                //The following query applies nested expands to single-valued navigation properties
                // starting with Task entities related to contacts created for this sample.

                var multiLevelExpandQuery = "$select=subject&" +
                        $"$filter=regardingobjectid_contact_task/_accountid_value eq {contosoId}" +
                        "&$expand=regardingobjectid_contact_task($select=fullname;" +
                        "$expand=parentcustomerid_account($select=name;" +
                        "$expand=createdby($select=fullname)))";

                var contosoTasks = await service.RetrieveMultiple<TaskActivity>(multiLevelExpandQuery, true);

                Console.WriteLine("\nExpanded values from Task:");

                DisplayExpandedValuesFromTask(contosoTasks.Value);

                #endregion Expanding results

                #region Aggregate results

                //Get aggregated salary information about Contacts working for Contoso

                Console.WriteLine("\nAggregated Annual Income information for Contoso contacts:");

                string applyExpression = "aggregate(annualincome with average as average, " +
                        $"annualincome with sum as total, " +
                        $"annualincome with min as minimum, " +
                        $"annualincome with max as maximum)";

                Dictionary<string, JsonElement> results = await service.Apply(accountContosoRef.Path + "/contact_customer_accounts", applyExpression, true);


                Console.WriteLine($"\tAverage income: {results["average@OData.Community.Display.V1.FormattedValue"]}");
                Console.WriteLine($"\tTotal income: {results["total@OData.Community.Display.V1.FormattedValue"]}");
                Console.WriteLine($"\tMinium income: {results["minimum@OData.Community.Display.V1.FormattedValue"]}");
                Console.WriteLine($"\tMaximum income: {results["maximum@OData.Community.Display.V1.FormattedValue"]}");


                #endregion Aggregate results

                #region FetchXML queries

                //Use FetchXML to query for all contacts whose fullname contains '(sample)'.
                //Note: XML string must be URI encoded. For more information, see:
                //https://docs.microsoft.com/powerapps/developer/common-data-service/webapi/retrieve-and-execute-predefined-queries#use-custom-fetchxml
                Console.WriteLine("\n-- FetchXML -- ");
                string fetchXmlQuery =
                    $@"<fetch mapping='logical'>
                         <entity name ='contact'>
                          <attribute name ='fullname' />
                          <attribute name ='jobtitle' />
                          <attribute name ='annualincome' />
                          <order descending ='true' attribute='fullname' />
                          <filter type ='and'>
                           <condition value ='%(sample)%' attribute='fullname' operator='like' />
                           <condition value ='{contosoId}' attribute='parentcustomerid' operator='eq' />
                          </filter>
                         </entity>
                        </fetch>";

                //Fetch returns a 'flat' structure that doesn't allow for typed serialization
                var contacts = await service.Fetch("contacts",XDocument.Parse(fetchXmlQuery), true);

                WriteContactFetchResultsTable($"Contacts Fetched by fullname containing '(sample)':", contacts);


                #endregion FetchXML queries


                #region Using predefined queries

                //Use predefined queries of the following two types:
                //  1) Saved query (system view)
                //  2) User query (saved view)
                //For more info, see:
                //https://docs.microsoft.com/powerapps/developer/common-data-service/webapi/retrieve-and-execute-predefined-queries#predefined-queries

                //1) Saved Query - retrieve "Active Accounts", run it, then display the results.
                Console.WriteLine("\n-- Saved Query -- ");

                var savedQueryResults = await service.RetrieveMultiple<SavedQuery>("$select=savedqueryid&$filter=name eq 'Active Accounts'");
                var savedqueryid = (Guid)savedQueryResults.Value[0].savedqueryid;             

                var activeAccounts = await service.RetrieveQuery("accounts", QueryType.savedQuery, savedqueryid, true);

                DisplayFormattedEntities("\nActive Accounts", activeAccounts);

                //2) Create a user query, then retrieve and execute it to display its results.
                //For more info, see:
                //https://docs.microsoft.com/powerapps/developer/common-data-service/saved-queries
                Console.WriteLine("\n-- User Query -- ");

                var userQuery = new UserQuery { 
                name = "My User Query",
                description = "User query to display contact info.",
                querytype = 0,
                returnedtypecode = "contact",
                fetchxml = @"<fetch mapping='logical' output-format='xml-platform' version='1.0' distinct='false'>
                                <entity name ='contact'>
                                    <attribute name ='fullname' />
                                    <attribute name ='contactid' />
                                    <attribute name ='jobtitle' />
                                    <attribute name ='annualincome' />
                                    <order descending ='false' attribute='fullname' />
                                    <filter type ='and'>
                                        <condition value ='%(sample)%' attribute='fullname' operator='like' />
                                        <condition value ='%Manager%' attribute='jobtitle' operator='like' />
                                        <condition value ='55000' attribute='annualincome' operator='gt' />
                                    </filter>
                                </entity>
                             </fetch>"
                };

                var userQueryRef = await service.Create(userQuery);
                entityRefs.Add(userQueryRef); //To delete later

                //Use the query to return results:
                var userQueryResults = await service.RetrieveQuery("contacts", QueryType.userQuery, userQueryRef.Id);

                DisplayFormattedEntities(userQuery.name, userQueryResults);

                #endregion Using predefined queries


                DeleteRequiredRecords(service);
                Console.WriteLine("\n--Query Data Completed--");
            }
            catch (Exception)
            {

                throw;
            }

        }

        private static async Task CreateRequiredRecords(Service service)
        {

            var account1 = new Account
            {
                name = "Contoso, Ltd. (sample)",
                Account_Tasks = new List<TaskActivity> {
                    {
                        new TaskActivity{
                            subject = "Task 1 for Contoso, Ltd.",
                            description = "Task 1 for Contoso, Ltd. description",
                            actualdurationminutes = 10
                        }
                    },
                    {
                        new TaskActivity{
                            subject = "Task 2 for Contoso, Ltd.",
                            description = "Task 2 for Contoso, Ltd. description",
                            actualdurationminutes = 10
                        }
                    },
                    {
                        new TaskActivity{
                            subject = "Task 3 for Contoso, Ltd.",
                            description = "Task 3 for Contoso, Ltd. description",
                            actualdurationminutes = 10
                        }
                    }
                },
                primarycontactid = new Contact
                {
                    firstname = "Yvonne",
                    lastname = "McKay (sample)",
                    jobtitle = "Coffee Master",
                    annualincome = 45000,
                    Contact_Tasks = new List<TaskActivity> {
                        {
                            new TaskActivity{
                                subject = "Task 1 for Yvonne McKay",
                                description = "Task 1 for Yvonne McKay. description",
                                actualdurationminutes = 10
                            }
                        },
                        {
                            new TaskActivity{
                                subject = "Task 2 for Yvonne McKay",
                                description = "Task 2 for Yvonne McKay description",
                                actualdurationminutes = 10
                            }
                        },
                        {
                            new TaskActivity{
                                subject = "Task 3 for Yvonne McKay",
                                description = "Task 3 for Yvonne McKay description",
                                actualdurationminutes = 10
                            }
                        }
                    }
                },

                contact_customer_accounts = new List<Contact> {
                    {
                        new Contact{
                            firstname = "Susanna",
                            lastname = "Stubberod (sample)",
                            jobtitle = "Senior Purchaser",
                            annualincome = 52000,
                            Contact_Tasks = new List<TaskActivity>{
                                {
                                    new TaskActivity{
                                        subject = "Task 1 for Susanna Stubberod",
                                        description = "Task 1 for Susanna Stubberod description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 2 for Susanna Stubberod",
                                        description = "Task 2 for Susanna Stubberod description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 3 for Susanna Stubberod",
                                        description = "Task 3 for Susanna Stubberod description",
                                        actualdurationminutes = 10
                                    }
                                }

                            }

                        }
                    },
                    {
                        new Contact{
                            firstname = "Nancy",
                            lastname = "Anderson (sample)",
                            jobtitle = "Activities Manager",
                            annualincome = 55500,
                            Contact_Tasks = new List<TaskActivity>{
                                {
                                    new TaskActivity{
                                        subject = "Task 1 for Nancy Anderson",
                                        description = "Task 1 for Nancy Anderson description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 2 for Nancy Anderson",
                                        description = "Task 2 for Nancy Anderson description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 3 for Nancy Anderson",
                                        description = "Task 3 for Nancy Anderson description",
                                        actualdurationminutes = 10
                                    }
                                }

                            }

                        }
                    },
                    {
                        new Contact{
                            firstname = "Maria",
                            lastname = "Cambell (sample)",
                            jobtitle = "Senior Purchaser",
                            annualincome = 52000,
                            Contact_Tasks = new List<TaskActivity>{
                                {
                                    new TaskActivity{
                                        subject = "Task 1 for Maria Cambell",
                                        description = "Task 1 for Maria Cambell description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 2 for Maria Cambell",
                                        description = "Task 2 for Maria Cambell description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 3 for Maria Cambell",
                                        description = "Task 3 for Maria Cambell description",
                                        actualdurationminutes = 10
                                    }
                                }

                            }

                        }
                    },
                    {
                        new Contact{
                            firstname = "Scott",
                            lastname = "Konersmann (sample)",
                            jobtitle = "Accounts Manager",
                            annualincome = 38000,
                            Contact_Tasks = new List<TaskActivity>{
                                {
                                    new TaskActivity{
                                        subject = "Task 1 for Scott Konersmann",
                                        description = "Task 1 for Scott Konersmann description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 2 for Scott Konersmann",
                                        description = "Task 2 for Scott Konersmann description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 3 for Scott Konersmann",
                                        description = "Task 3 for Scott Konersmann description",
                                        actualdurationminutes = 10
                                    }
                                }

                            }

                        }
                    },
                    {
                        new Contact{
                            firstname = "Robert",
                            lastname = "Lyon (sample)",
                            jobtitle = "Senior Technician",
                            annualincome = 78000,
                            Contact_Tasks = new List<TaskActivity>{
                                {
                                    new TaskActivity{
                                        subject = "Task 1 for Robert Lyon",
                                        description = "Task 1 for Robert Lyon description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 2 for Robert Lyon",
                                        description = "Task 2 for Robert Lyon description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 3 for Robert Lyon",
                                        description = "Task 3 for Robert Lyon description",
                                        actualdurationminutes = 10
                                    }
                                }

                            }

                        }
                    },
                    {
                        new Contact{
                            firstname = "Paul",
                            lastname = "Cannon (sample)",
                            jobtitle = "Ski Instructor",
                            annualincome = 68500,
                            Contact_Tasks = new List<TaskActivity>{
                                {
                                    new TaskActivity{
                                        subject = "Task 1 for Paul Cannon",
                                        description = "Task 1 for Paul Cannon description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 2 for Paul Cannon",
                                        description = "Task 2 for Paul Cannon description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 3 for Paul Cannon",
                                        description = "Task 3 for Paul Cannon description",
                                        actualdurationminutes = 10
                                    }
                                }

                            }

                        }
                    },
                    {
                        new Contact{
                            firstname = "Rene",
                            lastname = "Valdes (sample)",
                            jobtitle = "Data Analyst III",
                            annualincome = 86000,
                            Contact_Tasks = new List<TaskActivity>{
                                {
                                    new TaskActivity{
                                        subject = "Task 1 for Rene Valdes",
                                        description = "Task 1 for Rene Valdes description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 2 for Rene Valdes",
                                        description = "Task 2 for Rene Valdes description",
                                        actualdurationminutes = 10
                                    }
                                },
                                {
                                    new TaskActivity{
                                        subject = "Task 3 for Rene Valdes",
                                        description = "Task 3 for Rene Valdes description",
                                        actualdurationminutes = 10
                                    }
                                }

                            }

                        }
                    }
                }

            };

            accountContosoRef = await service.Create(account1);
            
            entityRefs.Add(accountContosoRef);
            var yvonneMcKay = service.Retrieve<Account>(accountContosoRef, "$select=accountid&$expand=primarycontactid").GetAwaiter().GetResult().primarycontactid;
            contactYvonneMcKayRef = yvonneMcKay.ToEntityReference();
            entityRefs.Add(contactYvonneMcKayRef);
            var contacts = await service.RetrieveRelatedMultiple<Contact>(accountContosoRef, "contact_customer_accounts", "$select=contactid");
            //Add to the top of the list so these are deleted first
            contacts.Value.ForEach(c => entityRefs.Insert(0, c.ToEntityReference()));

            //The related tasks will be deleted automatically when the owning record is deleted.
            Console.WriteLine("Account 'Contoso, Ltd. (sample)' created with 1 primary " +
                    "contact and 8 associated contacts.");

        }

        private static void DeleteRequiredRecords(Service service)
        {

            if (!deleteCreatedRecords)
            {
                Console.Write("\nDo you want these sample entity records deleted? (y/n) [y]: ");
                string answer = Console.ReadLine();
                answer = answer.Trim();
                if (!(answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty))
                {
                    return;
                }
            }

            Console.WriteLine("\nDeleting data created for this sample:");

            entityRefs.ForEach(async x =>
            {
                Console.Write(".");
                await service.Delete(x);
            });

            Console.WriteLine("\nData created for this sample deleted.");
        }

        private static void WriteContactFetchResultsTable(string message, EntityCollection collection)
        {
            //Display column widths for contact results table
            const int col1 = -27;
            const int col2 = -35;
            const int col3 = -15;

            Console.WriteLine($"\n{message}");
            //header
            Console.WriteLine($"\t|{"Full Name",col1}|" +
                $"{"Job Title",col2}|" +
                $"{"Annual Income",col3}");
            Console.WriteLine($"\t|{new string('-', col1 * -1),col1}|" +
                $"{new string('-', col2 * -1),col2}|" +
                $"{new string('-', col3 * -1),col3}");
            //rows
            foreach (var contact in collection.Value)
            {
                Console.WriteLine($"\t|{contact["fullname"],col1}|" +
                    $"{contact["jobtitle"],col2}|" +
                    $"{contact["annualincome@OData.Community.Display.V1.FormattedValue"],col3}");
            }
        }

        private static void WriteContactResultsTable(string message, List<Contact> collection)
        {
            //Display column widths for contact results table
            const int col1 = -27;
            const int col2 = -35;
            const int col3 = -15;

            Console.WriteLine($"\n{message}");
            //header
            Console.WriteLine($"\t|{"Full Name",col1}|" +
                $"{"Job Title",col2}|" +
                $"{"Annual Income",col3}");
            Console.WriteLine($"\t|{new string('-', col1 * -1),col1}|" +
                $"{new string('-', col2 * -1),col2}|" +
                $"{new string('-', col3 * -1),col3}");
            //rows
            foreach (Contact contact in collection)
            {
                Console.WriteLine($"\t|{contact.fullname,col1}|" +
                    $"{contact.jobtitle,col2}|" +
                    $"{contact.annualincome_display,col3}");
            }
        }

        private static void DisplayExpandedValuesFromTask(List<TaskActivity> collection)
        {

            //Display column widths for task Lookup Values Table
            const int col1 = -30;
            const int col2 = -30;
            const int col3 = -25;
            const int col4 = -20;

            Console.WriteLine($"\t|{"Subject",col1}|" +
            $"{"Contact",col2}|" +
            $"{"Account",col3}|" +
            $"{"Account CreatedBy",col4}");
            Console.WriteLine($"\t|{new string('-', col1 * -1),col1}|" +
                $"{new string('-', col2 * -1),col2}|" +
                $"{new string('-', col3 * -1),col3}|" +
                $"{new string('-', col4 * -1),col4}");
            //rows
            foreach (TaskActivity task in collection)
            {
                Console.WriteLine($"\t|{task.subject,col1}|" +
                    $"{task.regardingobjectid_contact_task.fullname,col2}|" +
                    $"{task.regardingobjectid_contact_task.parentcustomerid_account.name,col3}|" +
                    $"{task.regardingobjectid_contact_task.parentcustomerid_account.createdby.fullname,col4}");

            }
        }

        /// <summary> Displays formatted entity collections to the console. </summary>
        /// <param name="label">Descriptive text output before collection contents </param>
        /// <param name="collection"> EntityCollection containing array of entities to output by property </param>
        /// <param name="properties"> Array of properties within each entity to output. </param>
        private static void DisplayFormattedEntities(string label, EntityCollection entities)
        {
           

            Console.Write(label);
            int lineNum = 0;
            foreach (Dictionary<string,JsonElement> entity in entities.Value)
            {
                lineNum++;
                List<string> propsOutput = new List<string>();
                //Iterate through each requested property and output either formatted value if one
                //exists, otherwise output plain value.
                foreach (string key in entity.Keys)
                {
                    if (key != "@odata.etag") {

                        if (entity.ContainsKey(key + "@OData.Community.Display.V1.FormattedValue"))
                        {
                            propsOutput.Add(entity[key + "@OData.Community.Display.V1.FormattedValue"].ToString());
                        }
                        else
                        {
                            propsOutput.Add(entity[key].ToString());
                        }
                    }                    
                }
                Console.Write("\n\t{0}) {1}", lineNum, string.Join(", ", propsOutput));
            }
            Console.Write("\n");
        }


        //Passed in the Config to manage getting the token by the service
        internal static async Task<string> GetToken()
        {

            List<string> scopes = new List<string> { $"{appSettings["Url"]}/user_impersonation" };

            AuthenticationResult result = null;
            var accounts = await app.GetAccountsAsync();


            if (accounts.Any())
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
            }
            else
            {
                //https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-desktop-acquire-token?tabs=dotnet#username-and-password

                if (!string.IsNullOrEmpty(appSettings["Password"]) && !string.IsNullOrEmpty(appSettings["UserPrincipalName"]))
                {
                    try
                    {
                        SecureString password = new NetworkCredential("", appSettings["Password"]).SecurePassword;

                        result = await app.AcquireTokenByUsernamePassword(scopes.ToArray(), appSettings["UserPrincipalName"], password)
                            .ExecuteAsync();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else
                {
                    Console.WriteLine("Need password in appsettings.json.");
                }
            }

            return result.AccessToken;
        }
    }
}
