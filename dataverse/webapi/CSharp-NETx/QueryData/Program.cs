using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Batch;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Methods;
using System.Web;
using System.Xml.Linq;

namespace QueryData
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            List<EntityReference> recordsToDelete = new();
            bool deleteCreatedRecords = true;

            Console.WriteLine("--Starting Query Data sample--");

            #region Section 0: Create Records to query

            Console.WriteLine("\n--Section 0 started--");
            Console.WriteLine("\nCreating records to query...");

            //All the queries in this sample use this data:
            var accountContoso = new JObject {
                { "name", "Contoso, Ltd. (sample)" },
                { "Account_Tasks", new JArray{
                        new JObject{
                                { "subject","Task 1 for Contoso, Ltd."},
                                { "description","Task 1 for Contoso, Ltd. description"},
                                { "actualdurationminutes", 10 }},
                        new JObject{
                                { "subject","Task 2 for Contoso, Ltd."},
                                { "description","Task 2 for Contoso, Ltd. description"},
                                { "actualdurationminutes", 10 }},
                        new JObject{
                                { "subject","Task 3 for Contoso, Ltd."},
                                { "description","Task 3 for Contoso, Ltd. description"},
                                { "actualdurationminutes", 10 }},
                            }
                        },
                { "primarycontactid", new JObject{
                    { "firstname", "Yvonne" },
                    { "lastname", "McKay (sample)" },
                    { "jobtitle", "Coffee Master" },
                    { "annualincome", 45000 },
                    { "Contact_Tasks", new JArray{
                        new JObject{
                                { "subject","Task 1 for Yvonne McKay"},
                                { "description","Task 1 for Yvonne McKay description"},
                                { "actualdurationminutes", 5 }},
                        new JObject{
                                { "subject","Task 2 for Yvonne McKay"},
                                { "description","Task 2 for Yvonne McKay description"},
                                { "actualdurationminutes", 5 }},
                        new JObject{
                                { "subject","Task 3 for Yvonne McKay"},
                                { "description","Task 3 for Yvonne McKay description"},
                                { "actualdurationminutes", 5 }},
                            }
                        }
                    }
                },
                { "contact_customer_accounts", new JArray{
                        new JObject{
                                { "firstname","Susanna"},
                                { "lastname","Stubberod (sample)"},
                                { "jobtitle","Senior Purchaser"},
                                { "annualincome", 52000},
                                { "Contact_Tasks", new JArray{
                                new JObject{
                                        { "subject","Task 1 for Susanna Stubberod"},
                                        { "description","Task 1 for Susanna Stubberod description"},
                                        { "actualdurationminutes", 3 }},
                                new JObject{
                                        { "subject","Task 2 for Susanna Stubberod"},
                                        { "description","Task 2 for Susanna Stubberod description"},
                                        { "actualdurationminutes", 3 }},
                                new JObject{
                                        { "subject","Task 3 for Susanna Stubberod"},
                                        { "description","Task 3 for Susanna Stubberod description"},
                                        { "actualdurationminutes", 3 }},
                                    }
                                }},
                        new JObject{
                                { "firstname","Nancy"},
                                { "lastname","Anderson (sample)"},
                                { "jobtitle","Activities Manager"},
                                { "annualincome", 55500},
                                { "Contact_Tasks", new JArray{
                                new JObject{
                                        { "subject","Task 1 for Nancy Anderson"},
                                        { "description","Task 1 for Nancy Anderson description"},
                                        { "actualdurationminutes", 4 }},
                                new JObject{
                                        { "subject","Task 2 for Nancy Anderson"},
                                        { "description","Task 2 for Nancy Anderson description"},
                                        { "actualdurationminutes", 4 }},
                                new JObject{
                                        { "subject","Task 3 for Nancy Anderson"},
                                        { "description","Task 3 for Nancy Anderson description"},
                                        { "actualdurationminutes", 4 }},
                                    }
                                }},
                        new JObject{
                                { "firstname","Maria"},
                                { "lastname","Cambell (sample)"},
                                { "jobtitle","Accounts Manager"},
                                { "annualincome", 31000},
                                { "Contact_Tasks", new JArray{
                                new JObject{
                                        { "subject","Task 1 for Maria Cambell"},
                                        { "description","Task 1 for Maria Cambell description"},
                                        { "actualdurationminutes", 5 }},
                                new JObject{
                                        { "subject","Task 2 for Maria Cambell"},
                                        { "description","Task 2 for Maria Cambell description"},
                                        { "actualdurationminutes", 5 }},
                                new JObject{
                                        { "subject","Task 3 for Maria Cambell"},
                                        { "description","Task 3 for Maria Cambell description"},
                                        { "actualdurationminutes", 5 }},
                                    }
                                }},
                        new JObject{
                                { "firstname","Scott"},
                                { "lastname","Konersmann (sample)"},
                                { "jobtitle","Accounts Manager"},
                                { "annualincome", 38000},
                                { "Contact_Tasks", new JArray{
                                new JObject{
                                        { "subject","Task 1 for Scott Konersmann"},
                                        { "description","Task 1 for Scott Konersmann description"},
                                        { "actualdurationminutes", 6 }},
                                new JObject{
                                        { "subject","Task 2 for Scott Konersmann"},
                                        { "description","Task 2 for Scott Konersmann description"},
                                        { "actualdurationminutes", 6 }},
                                new JObject{
                                        { "subject","Task 3 for Scott Konersmann"},
                                        { "description","Task 3 for Scott Konersmann description"},
                                        { "actualdurationminutes", 6 }},
                                    }
                                }},
                        new JObject{
                                { "firstname","Robert"},
                                { "lastname","Lyon (sample)"},
                                { "jobtitle","Senior Technician"},
                                { "annualincome", 78000},
                                { "Contact_Tasks", new JArray{
                                new JObject{
                                        { "subject","Task 1 for Robert Lyon"},
                                        { "description","Task 1 for Robert Lyon description"},
                                        { "actualdurationminutes", 7 }},
                                new JObject{
                                        { "subject","Task 2 for Robert Lyon"},
                                        { "description","Task 2 for Robert Lyon description"},
                                        { "actualdurationminutes", 7 }},
                                new JObject{
                                        { "subject","Task 3 for Robert Lyon"},
                                        { "description","Task 3 for Robert Lyon description"},
                                        { "actualdurationminutes", 7 }},
                                    }
                                }},
                        new JObject{
                                { "firstname","Paul"},
                                { "lastname","Cannon (sample)"},
                                { "jobtitle","Ski Instructor"},
                                { "annualincome", 68500},
                                { "Contact_Tasks", new JArray{
                                new JObject{
                                        { "subject","Task 1 for Paul Cannon"},
                                        { "description","Task 1 for Paul Cannon description"},
                                        { "actualdurationminutes", 8 }},
                                new JObject{
                                        { "subject","Task 2 for Paul Cannon"},
                                        { "description","Task 2 for Paul Cannon description"},
                                        { "actualdurationminutes", 8 }},
                                new JObject{
                                        { "subject","Task 3 for Paul Cannon"},
                                        { "description","Task 3 for Paul Cannon description"},
                                        { "actualdurationminutes", 8 }},
                                    }
                                }},
                        new JObject{
                                { "firstname","Rene"},
                                { "lastname","Valdes (sample)"},
                                { "jobtitle","Data Analyst III"},
                                { "annualincome", 86000},
                                { "Contact_Tasks", new JArray{
                                new JObject{
                                        { "subject","Task 1 for Rene Valdes"},
                                        { "description","Task 1 for Rene Valdes description"},
                                        { "actualdurationminutes", 9 }},
                                new JObject{
                                        { "subject","Task 2 for Rene Valdes"},
                                        { "description","Task 2 for Rene Valdes description"},
                                        { "actualdurationminutes", 9 }},
                                new JObject{
                                        { "subject","Task 3 for Rene Valdes"},
                                        { "description","Task 3 for Rene Valdes description"},
                                        { "actualdurationminutes", 9 }},
                                    }
                                }},
                        new JObject{
                                { "firstname","Jim"},
                                { "lastname","Glynn (sample)"},
                                { "jobtitle","Senior International Sales Manager"},
                                { "annualincome", 81400},
                                { "Contact_Tasks", new JArray{
                                new JObject{
                                        { "subject","Task 1 for Jim Glynn"},
                                        { "description","Task 1 for Jim Glynn description"},
                                        { "actualdurationminutes", 10 }},
                                new JObject{
                                        { "subject","Task 2 for Jim Glynn"},
                                        { "description","Task 2 for Jim Glynn description"},
                                        { "actualdurationminutes", 10 }},
                                new JObject{
                                        { "subject","Task 3 for Jim Glynn"},
                                        { "description","Task 3 for Jim Glynn description"},
                                        { "actualdurationminutes", 10 }},
                                    }
                                }}
                            }}
            };

            EntityReference accountContosoRef = await service.Create("accounts", accountContoso);

            recordsToDelete.Add(accountContosoRef); //To delete later

            JObject retrievedAccountContoso = await service.Retrieve(
                entityReference: accountContosoRef,
                query: "?$select=accountid" +
                "&$expand=primarycontactid($select=contactid)," +
                "contact_customer_accounts($select=contactid)");

            var contactYvonneRef = new EntityReference(
                entitySetName: "contacts",
                id: (Guid)retrievedAccountContoso["primarycontactid"]["contactid"]);

            recordsToDelete.Add(contactYvonneRef); //To delete later

            foreach (JObject contact in retrievedAccountContoso["contact_customer_accounts"].Cast<JObject>())
            {

                //Add to the top of the list so these are deleted first
                recordsToDelete.Insert(0, new EntityReference("contacts", (Guid)contact["contactid"]));
            }

            //The related tasks will be deleted automatically when the owning record is deleted.
            Console.WriteLine("Account 'Contoso, Ltd. (sample)' created with 1 primary " +
                    "contact and 8 associated contacts.");

            #endregion Section 0: Create Records to query

            #region Section 1 Selecting specific properties

            Console.WriteLine("\n--Section 1 started--");

            // Basic query: Query using $select against a contact entity to get the properties you want.
            // For performance best practice, always use $select, otherwise all properties are returned
            // Including annotations provides access to formatted values with the
            // @OData.Community.Display.V1.FormattedValue annotation
            Console.WriteLine("-- Basic Query --");

            JObject retrievedContactYvonne = await service.Retrieve(
                entityReference: contactYvonneRef,
                query: "?$select=fullname,jobtitle,annualincome",
                includeAnnotations: true // Adds Request header: "Prefer", "odata.include-annotations=\"*\""
                );

            Console.WriteLine($"Contact basic info:\n" +
                        $"\tFullname: {retrievedContactYvonne["fullname"]}\n" +
                        $"\tJobtitle: {retrievedContactYvonne["jobtitle"]}\n" +
                        $"\tAnnualincome (unformatted): {retrievedContactYvonne["annualincome"]} \n" +
                        $"\tAnnualincome (formatted): {retrievedContactYvonne["annualincome@OData.Community.Display.V1.FormattedValue"]} \n");

            #endregion Section 1 Selecting specific properties

            #region Section 2 Using query functions
            Console.WriteLine("\n--Section 2 started--");

            // Filter criteria:
            // Applying filters to get targeted data.
            // 1) Using standard query functions (e.g.: contains, endswith, startswith)
            // 2) Using Dataverse query functions (e.g.: LastXhours, Last7Days, Today, Between, In, ...)
            // 3) Using filter operators and logical operators (e.g.: eq, ne, gt, and, or, etc…)
            // 4) Set precedence using parenthesis (e.g.: ((criteria1) and (criteria2)) or (criteria3)
            // For more info, see:
            // https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query-data-web-api#filter-results

            Console.WriteLine("-- Filter Criteria --");
            // Filter 1: Using standard query functions to filter results. This operation
            // will query for all contacts with fullname containing the string "(sample)".

            RetrieveMultipleResponse containsSampleinFullNameCollection =
                await service.RetrieveMultiple(queryUri: "contacts?" +
                        "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(fullname,'(sample)') and " +
                        $"_parentcustomerid_value eq {accountContosoRef.Id}",
                        includeAnnotations: true);

            WriteContactResultsTable(
                message: "Contacts filtered by fullname containing '(sample)':",
                collection: containsSampleinFullNameCollection.Records);

            // Filter 2: Using Dataverse query functions to filter results. In this operation, we will query
            // for all contacts that were created in the last hour. For complete list of Dataverse query
            // functions, see: https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/queryfunctions

            RetrieveMultipleResponse createdInLastHourCollection =
                await service.RetrieveMultiple(queryUri: "contacts?" +
                        "$select=fullname,jobtitle,annualincome" +
                        "&$filter=Microsoft.Dynamics.CRM.LastXHours(PropertyName=@p1,PropertyValue=@p2) and " +
                        $"_parentcustomerid_value eq {accountContosoRef.Id}" +
                        $"&@p1='createdon'" +
                        $"&@p2='1'",
                        includeAnnotations: true);

            WriteContactResultsTable(
                "Contacts that were created within the last 1hr:",
                createdInLastHourCollection.Records);

            // Filter 3: Using operators. Building on the previous operation, this will further limit
            // the results by the contact's income. For more info on standard filter operators,
            // https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query-data-web-api#filter-results

            RetrieveMultipleResponse highIncomeContacts =
                await service.RetrieveMultiple(queryUri: "contacts?" +
                        "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(fullname,'(sample)') and " +
                        "annualincome gt 55000  and " +
                        $"_parentcustomerid_value eq {accountContosoRef.Id}",
                        includeAnnotations: true);

            WriteContactResultsTable(
               message: "Contacts with '(sample)' in name and income above $55,000:",
               collection: highIncomeContacts.Records);

            // Filter 4: Set precedence using parentheses. Continue building on the previous
            // operation, this will further limit results by job title. Parentheses and the order of
            // filter statements can impact results returned.

            RetrieveMultipleResponse seniorOrSpecialistsCollection =
                await service.RetrieveMultiple(queryUri: "contacts?" +
                        "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(fullname,'(sample)') and " +
                        "(contains(jobtitle, 'senior') or " +
                        "contains(jobtitle,'manager')) and " +
                        "annualincome gt 55000 and " +
                        $"_parentcustomerid_value eq {accountContosoRef.Id}",
                        includeAnnotations: true);

            WriteContactResultsTable(
                message: "Contacts with '(sample)' in name senior jobtitle or high income:",
                collection: seniorOrSpecialistsCollection.Records);

            #endregion Section 2 Using query functions

            #region Section 3 Ordering and aliases

            Console.WriteLine("\n--Section 3 started--");

            Console.WriteLine("\n-- Order Results --");

            RetrieveMultipleResponse orderedResults =
                await service.RetrieveMultiple("contacts?" +
                        "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(fullname,'(sample)')and " +
                        $"_parentcustomerid_value eq {accountContosoRef.Id}&" +
                        "$orderby=jobtitle asc, annualincome desc",
                        includeAnnotations: true);

            WriteContactResultsTable(
                "Contacts ordered by jobtitle (Ascending) and annualincome (descending)",
                orderedResults.Records);

            // Parameterized aliases can be used as parameters in a query. Use these parameters
            // in $filter and $orderby options. Using the previous operation as basis, parameterizing the
            // query will give us the same results. For more info, see:
            // https://learn.microsoft.com/power-apps/developer/data-platform/webapi/use-web-api-functions#passing-parameters-to-a-function

            Console.WriteLine("\n-- Parameterized Aliases --");

            RetrieveMultipleResponse orderedResultsWithParams =
                await service.RetrieveMultiple("contacts?" +
                        "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(@p1,'(sample)') and " +
                        "@p2 eq @p3&" +
                        "$orderby=@p4 asc, @p5 desc&" +
                        "@p1=fullname&" +
                        "@p2=_parentcustomerid_value&" +
                        $"@p3={accountContosoRef.Id}&" +
                        "@p4=jobtitle&" +
                        "@p5=annualincome",
                        includeAnnotations: true);

            WriteContactResultsTable(
                "Contacts ordered by jobtitle (Ascending) and annualincome (descending)",
                orderedResultsWithParams.Records);

            #endregion Section 3 Ordering and aliases

            #region Section 4 Limit and count results

            Console.WriteLine("\n--Section 4 started--");

            // To limit records returned, use the $top query option.  Specifying a limit number for $top
            // returns at most that number of results per request. Extra results are ignored.
            // For more information, see:
            // https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query-data-web-api#use-top-query-option
            Console.WriteLine("\n-- Top Results --");

            RetrieveMultipleResponse topFive =
                await service.RetrieveMultiple("contacts?" +
                        "$select=fullname,jobtitle,annualincome&" +
                        "$filter=contains(fullname,'(sample)') and " +
                        $"_parentcustomerid_value eq {accountContosoRef.Id}&" +
                        "$top=5",
                        includeAnnotations: true);

            WriteContactResultsTable("Contacts top 5 results:", topFive.Records);

            // Result count - count the number of results matching the filter criteria.
            // Tip: Use count together with the "odata.maxpagesize" to calculate the number of pages in
            // the query.  Note: Dataverse has a max record limit of 5000 records per response.
            Console.WriteLine("\n-- Result Count --");

            //1) Get a count of a collection without the data.

            GetCollectionCountRequest countRequest = new("contacts");
            var countResponse = await service.SendAsync<GetCollectionCountResponse>(countRequest);
            Console.WriteLine($"\nThe contacts collection has {countResponse.Count} contacts.");

            //  2) Get a count along with the data by including $count=true

            RetrieveMultipleResponse countWithData =
                await service.RetrieveMultiple("contacts?" +
                        "$select=fullname,jobtitle,annualincome&" +
                        "$filter=(contains(jobtitle,'senior') or contains(jobtitle, 'manager')) and " +
                        $"_parentcustomerid_value eq {accountContosoRef.Id}" +
                        "&$count=true",
                        includeAnnotations: true);

            WriteContactResultsTable($"{countWithData.Count} " +
                $"Contacts with 'senior' or 'manager' in job title:",
                countWithData.Records);

            #endregion Section 4 Limit and count results

            #region Section 5 Pagination

            Console.WriteLine("\n--Section 5 started--");

            RetrieveMultipleResponse firstPageResults = 
                await service.RetrieveMultiple(
                    queryUri: "contacts?$select=fullname,jobtitle,annualincome&$filter=contains(fullname,'(sample)')&$count=true",
                    maxPageSize: 4,
                    includeAnnotations: true);

            WriteContactResultsTable($"Contacts total: {firstPageResults.TotalRecordCount}    Contacts per page: 4.  \r\nPage 1 of 3:", firstPageResults.Records);

            RetrieveMultipleResponse secondPageResults =
                await service.RetrieveMultiple(
                    queryUri: firstPageResults.NextLink,
                    maxPageSize: 4,
                    includeAnnotations: true);

            WriteContactResultsTable($"Page 2 of 3:", secondPageResults.Records);


            #endregion Section 5 Pagination

            #region Section 6 Expanding results

            Console.WriteLine("\n--Section 6 started--");

            // The $expand option retrieves related information.
            // To retrieve information on associated entities in the same request, use the $expand
            // query option on navigation properties.
            //   1) Expand using single-valued navigation properties (e.g.: via the 'primarycontactid')
            //   2) Expand using partner property (e.g.: from contact to account via the 'account_primary_contact')
            //   3) Expand using collection-valued navigation properties (e.g.: via the 'contact_customer_accounts')
            //   4) Expand using multiple navigation property types in a single request.
            //   5) Nested expands of single-valued navigation properties.
            //   6) Nested $expand having both single-valued and collection-valued navigation properties.

            // Tip: For performance best practice, always use $select statement in an expand option.

            Console.WriteLine("\n-- Expanding Results --");

            //1) Expand using the 'primarycontactid' single-valued navigation property of account Contoso.

            retrievedAccountContoso = await service.Retrieve(
               entityReference: accountContosoRef,
               query: "?$select=name" +
               "&$expand=primarycontactid($select=fullname,jobtitle,annualincome)",
               includeAnnotations: true);

            Console.WriteLine($"\nAccount {retrievedAccountContoso["name"]} has the following primary contact person:\n" +
             $"\tFullname: {retrievedAccountContoso["primarycontactid"]["fullname"]} \n" +
             $"\tJobtitle: {retrievedAccountContoso["primarycontactid"]["jobtitle"]} \n" +
             $"\tAnnualincome: {retrievedAccountContoso["primarycontactid"]["annualincome"]}");

            //2) Expand using the 'account_primary_contact' partner property.

            retrievedContactYvonne =
               await service.Retrieve(
                   entityReference: contactYvonneRef,
                   query: "?$select=fullname,jobtitle,annualincome&$expand=account_primary_contact($select=name)");

            Console.WriteLine($"\nContact '{retrievedContactYvonne["fullname"]}' is the primary contact for the following accounts:");

            foreach (JObject account in retrievedContactYvonne["account_primary_contact"].Cast<JObject>())
            {
                Console.WriteLine($"\t{account["name"]}");
            }

            //3) Expand using the collection-valued 'contact_customer_accounts' navigation property.

            retrievedAccountContoso =
                await service.Retrieve(
                    entityReference: accountContosoRef,
                    query: "?$select=name&$expand=contact_customer_accounts($select=fullname,jobtitle,annualincome)",
                    includeAnnotations: true);


            WriteContactResultsTable(
                $"Account '{retrievedAccountContoso["name"]}' has the following contact customers:",
                retrievedAccountContoso["contact_customer_accounts"]);


            //4) Expand using multiple navigation property types in a single request, specifically:
            //   primarycontactid, contact_customer_accounts, and Account_Tasks.

            Console.WriteLine("\n-- Expanding multiple property types in one request -- ");

            retrievedAccountContoso =
                await service.Retrieve(entityReference: accountContosoRef,
                query: "?$select=name&" +
                        "$expand=primarycontactid($select=fullname,jobtitle,annualincome)," +
                        "contact_customer_accounts($select=fullname,jobtitle,annualincome)," +
                        "Account_Tasks($select=subject,description)",
                includeAnnotations: true);

            Console.WriteLine($"\nAccount {retrievedAccountContoso["name"]} has the following primary contact person:\n" +
                  $"\tFullname: {retrievedAccountContoso["primarycontactid"]["fullname"]} \n" +
                  $"\tJobtitle: {retrievedAccountContoso["primarycontactid"]["jobtitle"]} \n" +
                  $"\tAnnualincome: {retrievedAccountContoso["primarycontactid"]["annualincome"]}");

            WriteContactResultsTable(
                $"Account '{retrievedAccountContoso["name"]}' has the following contact customers:",
                retrievedAccountContoso["contact_customer_accounts"]);

            Console.WriteLine($"\nAccount '{retrievedAccountContoso["name"]}' has the following tasks:");

            foreach (JObject task in retrievedAccountContoso["Account_Tasks"].Cast<JObject>())
            {
                Console.WriteLine($"\t{task["subject"]}");
            }

            // 5) Nested expands

            // The following query applies nested expands to single-valued navigation properties
            // starting with Task entities related to contacts created for this sample.
            RetrieveMultipleResponse contosoTasks =
                await service.RetrieveMultiple(queryUri: $"tasks?" +
                $"$select=subject&" +
                $"$filter=regardingobjectid_contact_task/_accountid_value eq {accountContosoRef.Id}" +
                $"&$expand=regardingobjectid_contact_task($select=fullname;" +
                $"$expand=parentcustomerid_account($select=name;" +
                $"$expand=createdby($select=fullname)))",
                includeAnnotations: true);

            Console.WriteLine("\nExpanded values from Task:");

            DisplayExpandedValuesFromTask(contosoTasks.Records);

            // 6) Nested $expand having both single-valued and collection-valued navigation properties

            // The following query applies nested expands to single-valued and collection-valued navigation properties.
            // Accounts entity is related to AccountTasks and Contacts entities. Contacts entity is further expanded on OwningUser navigation property.

            RetrieveMultipleResponse accounts =
                await service.RetrieveMultiple(queryUri: $"accounts?" +
                $"$select=name,accountid&" +
                $"$filter= accountid eq {accountContosoRef.Id} &" +
                $"$expand=Account_Tasks($select=subject,description),contact_customer_accounts($select=fullname;" +
                $"$expand=owninguser($select=fullname,systemuserid))",
                includeAnnotations: true);

            Console.WriteLine("\nExpanded values from Accounts:");
            DisplayExpandedValuesFromAccount(accounts.Records);

            #endregion Section 6 Expanding results

            #region Section 7 Aggregate results

            Console.WriteLine("\n--Section 7 started--");

            // Get aggregated salary information about Contacts working for Contoso

            Console.WriteLine("\nAggregated Annual Income information for Contoso contacts:");

            RetrieveMultipleResponse contactData =
                await service.RetrieveMultiple(queryUri: $"{accountContosoRef.Path}/contact_customer_accounts?" +
                $"$apply=aggregate(annualincome with average as average, " +
                $"annualincome with sum as total, " +
                $"annualincome with min as minimum, " +
                $"annualincome with max as maximum)",
                includeAnnotations: true);

            Console.WriteLine($"\tAverage income: {contactData.Records[0]["average@OData.Community.Display.V1.FormattedValue"]}");
            Console.WriteLine($"\tTotal income: {contactData.Records[0]["total@OData.Community.Display.V1.FormattedValue"]}");
            Console.WriteLine($"\tMinium income: {contactData.Records[0]["minimum@OData.Community.Display.V1.FormattedValue"]}");
            Console.WriteLine($"\tMaximum income: {contactData.Records[0]["maximum@OData.Community.Display.V1.FormattedValue"]}");

            #endregion Section 7 Aggregate results

            #region Section 8 FetchXML queries

            Console.WriteLine("\n--Section 8 started--");
            
            // Use FetchXML to query for all contacts whose fullname contains '(sample)'.
            // Note: XML string must be URI encoded. For more information, see:
            // https://learn.microsoft.com/power-apps/developer/data-platform/webapi/use-fetchxml-web-api
            // In this sample the FetchXmlResponse class encodes the URI.
            Console.WriteLine("\n-- FetchXML -- ");
            string fetchXmlQuery =
                "<fetch mapping='logical' output-format='xml-platform' version='1.0' distinct='false'>" +
                  "<entity name ='contact'>" +
                    "<attribute name ='fullname' />" +
                    "<attribute name ='jobtitle' />" +
                    "<attribute name ='annualincome' />" +
                    "<order descending ='true' attribute='fullname' />" +
                    "<filter type ='and'>" +
                      "<condition value ='%(sample)%' attribute='fullname' operator='like' />" +
                      $"<condition value ='{accountContosoRef.Id}' attribute='parentcustomerid' operator='eq' />" +
                    "</filter>" +
                  "</entity>" +
                "</fetch>";

            // The FetchXml method uses the FetchXmlResponse class and sends the request 
            // as with BatchRequest so that the URL containing the FetchXml is sent 
            // in the body of the request rather than the URI.
            // Use this pattern to mitigate URL length limits for GET requests.

            FetchXmlResponse contacts = await service.FetchXml(
                entitySetName: "contacts",
                fetchXml: XDocument.Parse(fetchXmlQuery),
                includeAnnotations: true);

            WriteContactResultsTable(
                message: $"Contacts Fetched by fullname containing '(sample)':",
                collection: contacts.Records);

            #region Simple fetchXml Paging

            Console.WriteLine();
            Console.WriteLine("Simple Paging");

            XDocument fetchXmlQueryDoc = XDocument.Parse(fetchXmlQuery);

            // Add attribute to set the page size
            fetchXmlQueryDoc.Root.Add(new XAttribute("count", "4"));

            // Add attribute to set the page
            fetchXmlQueryDoc.Root.Add(new XAttribute("page", "2"));

            // Use FetchXmlRequest this time. Uses GET rather than POST
            FetchXmlRequest page2Request = new(
                entitySetName: "contacts",
                fetchXml: fetchXmlQueryDoc,
                includeAnnotations: true);

            FetchXmlResponse page2Response =
                await service.SendAsync<FetchXmlResponse>(page2Request);

            WriteContactResultsTable(
                message: $"Contacts Fetched by fullname containing '(sample)' - Page 2:",
                collection: page2Response.Records);

            #endregion Simple fetchXml Paging

            #region FetchXML paging with paging cookie

            Console.WriteLine();
            Console.WriteLine("Paging with PagingCookie");

            int page = 1;

            // Using the same FetchXml
            // Add attribute to set the paging cookie
            fetchXmlQueryDoc.Root.Add(new XAttribute("paging-cookie", ""));

            // Reset the page
            fetchXmlQueryDoc.Root.Attribute("page").Value = page.ToString();

            // Reset the count
            fetchXmlQueryDoc.Root.Attribute("count").Value = "3";

            // Send first request
            FetchXmlResponse cookiePagedContacts = await service.FetchXml(
                entitySetName: "contacts",
                fetchXml: fetchXmlQueryDoc,
                includeAnnotations: true);

            // Output results of first request
            WriteContactResultsTable(
            message: $"Paging with fetchxml cookie - Page {page}:",
            collection: cookiePagedContacts.Records);

            // Loop through subsequent requests while more records match criteria
            while (cookiePagedContacts.MoreRecords) {

                page++;

                fetchXmlQueryDoc.Root.Attribute("page").Value = page.ToString();

                // Extract the FetchxmlPagingCookie XML document value from the response.
                var cookieDoc = XDocument.Parse(cookiePagedContacts.FetchxmlPagingCookie);

                // Extract the encoded pagingcookie attribute value from the FetchxmlPagingCookie XML document
                string pagingCookie = cookieDoc.Root.Attribute("pagingcookie").Value;

                // Double URL decode the pagingCookie string value
                string decodedPagingCookie = HttpUtility.UrlDecode(HttpUtility.UrlDecode(pagingCookie));

                // Set the paging cookie value in the FetchXML paging-cookie attribute
                fetchXmlQueryDoc.Root.Attribute("paging-cookie").Value = decodedPagingCookie;

                // Send the request
                cookiePagedContacts = await service.FetchXml(
                entitySetName: "contacts",
                fetchXml: fetchXmlQueryDoc,
                includeAnnotations: true);

                // Output results of subsequent requests
                WriteContactResultsTable(
                message: $"Paging with fetchxml cookie - Page {page}:",
                collection: cookiePagedContacts.Records);

            }


            #endregion FetchXML paging with paging cookie

            #endregion Section 8 FetchXML queries

            #region Section 9 Using predefined queries

            Console.WriteLine("\n--Section 9 started--");

            // Use predefined queries of the following two types:
            //   1) Saved query (system view)
            //   2) User query (saved view)
            // For more info, see:
            // https://learn.microsoft.com/power-apps/developer/data-platform/webapi/retrieve-and-execute-predefined-queries#predefined-queries

            // 1) Saved Query - retrieve "Active Accounts", run it, then display the results.
            Console.WriteLine("\n-- Saved Query -- ");

            // Get the 'Active Accounts' Saved Query Id
            RetrieveMultipleResponse activeAccountsSavedQueryIdResponse =
                await service.RetrieveMultiple(
                    queryUri: "savedqueries?$select=name,savedqueryid" +
                    "&$filter=name eq 'Active Accounts'");
            var activeAccountsSavedQueryId = (Guid)activeAccountsSavedQueryIdResponse.Records.FirstOrDefault()["savedqueryid"];

            // Get 3 accounts using the saved query
            RetrieveMultipleResponse activeAccounts =
                await service.RetrieveMultiple(
                    queryUri: $"accounts?savedQuery={activeAccountsSavedQueryId}",
                    maxPageSize: 3,
                    includeAnnotations: true);

            DisplayFormattedEntities(
                label: "\nActive Accounts",
                entities: activeAccounts.Records,
                properties: new string[] { "name", "_primarycontactid_value", "telephone1" });

            // 2) Create a user query, then retrieve and execute it to display its results.
            // For more info, see:
            // https://learn.microsoft.com/power-apps/developer/data-platform/saved-queries
            Console.WriteLine("\n-- User Query -- ");


            var userQuery = new JObject()
            {
                { "name","My User Query"},
                { "description","User query to display contact info."},
                { "querytype",0},
                { "returnedtypecode", "contact" },
                { "fetchxml", @"<fetch mapping='logical' output-format='xml-platform' version='1.0' distinct='false'>
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
                 </fetch>" }
            };

            EntityReference myUserQueryRef = await service.Create(
                entitySetName: "userqueries",
                record: userQuery);
            recordsToDelete.Add(myUserQueryRef); //To delete later

            // Use the query to return results:
            RetrieveMultipleResponse myUserQueryResults =
                await service.RetrieveMultiple(
                    queryUri: $"contacts?userQuery={myUserQueryRef.Id}",
                    maxPageSize: 3,
                    includeAnnotations: true);

            WriteContactResultsTable($"Contacts Fetched by My User Query:", myUserQueryResults.Records);

            #endregion Section 9 Using predefined queries

            #region Section 10: Delete sample records

            Console.WriteLine("\n--Section 10 started--");
            // Delete all the created sample records.  Note that explicit deletion is not required  
            // for contact tasks because these are automatically cascade-deleted with owner.  

            if (!deleteCreatedRecords)
            {
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                string answer = Console.ReadLine();
                answer = answer.Trim();
                if (!(answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty))
                { recordsToDelete.Clear(); }
                else
                {
                    Console.WriteLine("\nDeleting created records.");
                }
            }
            else
            {
                Console.WriteLine("\nDeleting created records.");
            }

            List<HttpRequestMessage> deleteRequests = new();

            foreach (EntityReference recordToDelete in recordsToDelete)
            {
                deleteRequests.Add(new DeleteRequest(recordToDelete));
            }

            BatchRequest batchRequest = new(service.BaseAddress)
            {
                Requests = deleteRequests
            };

            await service.SendAsync(batchRequest);


            #endregion Section 10: Delete sample records

            Console.WriteLine("--Query Data sample complete--");
        }

        /// <summary>
        /// Helper method to show results returned by a query
        /// </summary>
        /// <param name="message">The text to display</param>
        /// <param name="collection">The collection of records to display</param>
        private static void WriteContactResultsTable(string message, JToken collection)
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
            foreach (JObject contact in collection)
            {
                Console.WriteLine($"\t|{contact["fullname"],col1}|" +
                    $"{contact["jobtitle"],col2}|" +
                    $"{contact["annualincome@OData.Community.Display.V1.FormattedValue"],col3}");
            }
        }

        /// <summary> Displays formatted entity collections to the console. </summary>
        /// <param name="label">Descriptive text output before collection contents </param>
        /// <param name="collection"> JObject containing array of entities to output by property </param>
        /// <param name="properties"> Array of properties within each entity to output. </param>
        private static void DisplayFormattedEntities(string label, JArray entities, string[] properties)
        {
            Console.Write(label);
            int lineNum = 0;
            foreach (JObject entity in entities)
            {
                lineNum++;
                List<string> propsOutput = new List<string>();
                //Iterate through each requested property and output either formatted value if one
                //exists, otherwise output plain value.
                foreach (string prop in properties)
                {
                    string propValue;
                    string formattedProp = prop + "@OData.Community.Display.V1.FormattedValue";
                    if (null != entity[formattedProp])
                    {
                        propValue = entity[formattedProp].ToString();
                    }
                    else
                    {
                        if (null != entity[prop])
                        {
                            propValue = entity[prop].ToString();
                        }
                        else
                        {
                            propValue = "NULL";
                        }
                    }
                    propsOutput.Add(propValue);
                }
                Console.Write("\n\t{0}) {1}", lineNum, string.Join(", ", propsOutput));
            }
            Console.Write("\n");
        }

        /// <summary>
        /// Helper method to display expanded values from task records
        /// </summary>
        /// <param name="collection">The collection of task records.</param>
        private static void DisplayExpandedValuesFromTask(JToken collection)
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
            foreach (JObject task in collection)
            {
                Console.WriteLine($"\t|{task["subject"],col1}|" +
                    $"{task["regardingobjectid_contact_task"]["fullname"],col2}|" +
                    $"{task["regardingobjectid_contact_task"]["parentcustomerid_account"]["name"],col3}|" +
                    $"{task["regardingobjectid_contact_task"]["parentcustomerid_account"]["createdby"]["fullname"],col4}");

            }
        }

        /// <summary>
        /// Helper method to display expanded values from account records
        /// </summary>
        /// <param name="collection">The collection of account records.</param>
        private static void DisplayExpandedValuesFromAccount(JToken collection)
        {

            //Display column widths for task Lookup Values Table
            const int col1 = -30;
            const int col2 = -30;

            //rows
            foreach (JObject account in collection.Cast<JObject>())
            {
                Console.WriteLine($"Account: {account["name"]}");

                Console.WriteLine($"\t|{"Account Task",col1}|");
                Console.WriteLine($"\t|{new string('-', col1 * -1),col1}|");

                foreach (JObject accountTasks in account["Account_Tasks"].Cast<JObject>())
                {
                    Console.WriteLine($"\t|{accountTasks["subject"],col1}|");
                }

                Console.WriteLine($"\n\t|{"Contact",col1}|" + $"{"System User",col2}|");
                Console.WriteLine($"\t|{new string('-', col1 * -1),col1}|" +
                    $"{new string('-', col2 * -1),col2}|");

                foreach (JObject contacts in account["contact_customer_accounts"].Cast<JObject>())
                {
                    Console.WriteLine($"\t|{contacts["fullname"],col1}|" +
                        $"{contacts["owninguser"]["fullname"],col2}|");
                }
            }
        }
    }
}