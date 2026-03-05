using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates querying data using Language-Integrated Query (LINQ)
    /// </summary>
    /// <remarks>
    /// This sample shows various LINQ query patterns including:
    /// - Simple where clauses and filtering
    /// - Joins (inner, left, self)
    /// - Operators (equals, not equals, greater than, contains, StartsWith, EndsWith)
    /// - Ordering and paging
    /// - String and math operations
    /// - Late-bound entity queries
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();

        #region Sample Methods

        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating sample data...");

            // Create contacts for LINQ samples
            var contact1 = new Entity("contact")
            {
                ["firstname"] = "Colin",
                ["lastname"] = "Wilcox",
                ["address1_city"] = "Redmond",
                ["address1_stateorprovince"] = "WA",
                ["address1_postalcode"] = "98052",
                ["anniversary"] = new DateTime(2010, 3, 5),
                ["creditlimit"] = new Money(300),
                ["description"] = "Alpine Ski House",
                ["numberofchildren"] = 1,
                ["address1_latitude"] = 47.6741667,
                ["address1_longitude"] = -122.1202778
            };
            Guid contactId1 = service.Create(contact1);
            entityStore.Add(new EntityReference("contact", contactId1));

            var contact2 = new Entity("contact")
            {
                ["firstname"] = "Brian",
                ["lastname"] = "Smith",
                ["address1_city"] = "Bellevue",
                ["address1_stateorprovince"] = "WA",
                ["address1_postalcode"] = "98008",
                ["anniversary"] = new DateTime(2010, 4, 5),
                ["creditlimit"] = new Money(30000),
                ["description"] = "Coho Winery",
                ["numberofchildren"] = 2,
                ["address1_latitude"] = 47.6105556,
                ["address1_longitude"] = -122.1994444
            };
            Guid contactId2 = service.Create(contact2);
            entityStore.Add(new EntityReference("contact", contactId2));

            var contact3 = new Entity("contact")
            {
                ["firstname"] = "Darren",
                ["lastname"] = "Parker",
                ["address1_city"] = "Kirkland",
                ["address1_stateorprovince"] = "WA",
                ["address1_postalcode"] = "98033",
                ["anniversary"] = new DateTime(2010, 10, 5),
                ["creditlimit"] = new Money(10000),
                ["description"] = "Coho Winery",
                ["numberofchildren"] = 2
            };
            Guid contactId3 = service.Create(contact3);
            entityStore.Add(new EntityReference("contact", contactId3));

            var contact4 = new Entity("contact")
            {
                ["firstname"] = "Ben",
                ["lastname"] = "Smith",
                ["address1_city"] = "Kirkland",
                ["address1_stateorprovince"] = "WA",
                ["anniversary"] = new DateTime(2010, 7, 5),
                ["creditlimit"] = new Money(12000),
                ["description"] = "Coho Winery",
                ["numberofchildren"] = 2,
                ["creditonhold"] = true
            };
            Guid contactId4 = service.Create(contact4);
            entityStore.Add(new EntityReference("contact", contactId4));

            // Create accounts
            var account1 = new Entity("account")
            {
                ["name"] = "Coho Winery",
                ["address1_name"] = "Coho Vineyard & Winery",
                ["address1_city"] = "Redmond"
            };
            Guid accountId1 = service.Create(account1);
            entityStore.Add(new EntityReference("account", accountId1));

            var lead = new Entity("lead")
            {
                ["firstname"] = "Diogo",
                ["lastname"] = "Andrade"
            };
            Guid leadId = service.Create(lead);
            entityStore.Add(new EntityReference("lead", leadId));

            var account2 = new Entity("account")
            {
                ["name"] = "Contoso Ltd",
                ["parentaccountid"] = new EntityReference("account", accountId1),
                ["address1_name"] = "Contoso Pharmaceuticals",
                ["address1_city"] = "Redmond",
                ["originatingleadid"] = new EntityReference("lead", leadId),
                ["primarycontactid"] = new EntityReference("contact", contactId2)
            };
            Guid accountId2 = service.Create(account2);
            entityStore.Add(new EntityReference("account", accountId2));

            // Create additional accounts for simple queries
            var account3 = new Entity("account")
            {
                ["name"] = "Fourth Coffee",
                ["address1_stateorprovince"] = "Colorado"
            };
            entityStore.Add(new EntityReference("account", service.Create(account3)));

            var account4 = new Entity("account")
            {
                ["name"] = "School of Fine Art",
                ["address1_stateorprovince"] = "Illinois",
                ["address1_county"] = "Lake County"
            };
            entityStore.Add(new EntityReference("account", service.Create(account4)));

            var account5 = new Entity("account")
            {
                ["name"] = "Tailspin Toys",
                ["address1_stateorprovince"] = "Washington",
                ["address1_county"] = "King County"
            };
            entityStore.Add(new EntityReference("account", service.Create(account5)));

            Console.WriteLine("Setup complete.");
            Console.WriteLine();
        }

        private static void Run(ServiceClient service)
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("LINQ Query Examples");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            using (OrganizationServiceContext orgContext = new OrganizationServiceContext(service))
            {
                // Example 1: Simple where clause with Contains
                Console.WriteLine("1. Simple where clause - Accounts containing 'Contoso'");
                Console.WriteLine("-".PadRight(70, '-'));
                var query1 = from a in orgContext.CreateQuery("account")
                             where ((string)a["name"]).Contains("Contoso")
                             select new
                             {
                                 Name = a["name"],
                                 City = a.GetAttributeValue<string>("address1_city")
                             };
                foreach (var a in query1)
                {
                    Console.WriteLine($"  {a.Name} - {a.City}");
                }
                Console.WriteLine();

                // Example 2: Multiple where clauses
                Console.WriteLine("2. Multiple where clauses - Contoso in Redmond");
                Console.WriteLine("-".PadRight(70, '-'));
                var query2 = from a in orgContext.CreateQuery("account")
                             where ((string)a["name"]).Contains("Contoso")
                             where a.GetAttributeValue<string>("address1_city") == "Redmond"
                             select new
                             {
                                 Name = a["name"],
                                 City = a["address1_city"]
                             };
                foreach (var a in query2)
                {
                    Console.WriteLine($"  {a.Name} - {a.City}");
                }
                Console.WriteLine();

                // Example 3: Inner join
                Console.WriteLine("3. Inner join - Contacts with their accounts");
                Console.WriteLine("-".PadRight(70, '-'));
                var query3 = from c in orgContext.CreateQuery("contact")
                             join a in orgContext.CreateQuery("account")
                             on c["contactid"] equals a["primarycontactid"]
                             select new
                             {
                                 ContactName = c["fullname"],
                                 AccountName = a["name"]
                             };
                foreach (var item in query3)
                {
                    Console.WriteLine($"  Contact: {item.ContactName}, Account: {item.AccountName}");
                }
                Console.WriteLine();

                // Example 4: Left join
                Console.WriteLine("4. Left join - Accounts with and without primary contacts");
                Console.WriteLine("-".PadRight(70, '-'));
                var query4 = from a in orgContext.CreateQuery("account")
                             join c in orgContext.CreateQuery("contact")
                             on a["primarycontactid"] equals c["contactid"] into gr
                             from c_joined in gr.DefaultIfEmpty()
                             select new
                             {
                                 AccountName = a["name"],
                                 ContactName = c_joined != null ? c_joined.GetAttributeValue<string>("fullname") : "(none)"
                             };
                foreach (var item in query4)
                {
                    Console.WriteLine($"  Account: {item.AccountName}, Contact: {item.ContactName}");
                }
                Console.WriteLine();

                // Example 5: Using Distinct
                Console.WriteLine("5. Distinct - Unique contact last names");
                Console.WriteLine("-".PadRight(70, '-'));
                var query5 = (from c in orgContext.CreateQuery("contact")
                              select c.GetAttributeValue<string>("lastname")).Distinct();
                foreach (var lastName in query5)
                {
                    Console.WriteLine($"  {lastName}");
                }
                Console.WriteLine();

                // Example 6: Equals operator
                Console.WriteLine("6. Equals operator - Contacts named Colin");
                Console.WriteLine("-".PadRight(70, '-'));
                var query6 = from c in orgContext.CreateQuery("contact")
                             where c.GetAttributeValue<string>("firstname") == "Colin"
                             select new
                             {
                                 FirstName = c["firstname"],
                                 LastName = c["lastname"],
                                 City = c.GetAttributeValue<string>("address1_city")
                             };
                foreach (var c in query6)
                {
                    Console.WriteLine($"  {c.FirstName} {c.LastName} - {c.City}");
                }
                Console.WriteLine();

                // Example 7: Not equals operator
                Console.WriteLine("7. Not equals - Contacts not in Redmond");
                Console.WriteLine("-".PadRight(70, '-'));
                var query7 = from c in orgContext.CreateQuery("contact")
                             where c.GetAttributeValue<string>("address1_city") != "Redmond"
                             select new
                             {
                                 FirstName = c["firstname"],
                                 LastName = c["lastname"],
                                 City = c["address1_city"]
                             };
                foreach (var c in query7)
                {
                    Console.WriteLine($"  {c.FirstName} {c.LastName} - {c.City}");
                }
                Console.WriteLine();

                // Example 8: Greater than operator with Money
                Console.WriteLine("8. Greater than - Contacts with credit limit > $20,000");
                Console.WriteLine("-".PadRight(70, '-'));
                var query8 = from c in orgContext.CreateQuery("contact")
                             where c.GetAttributeValue<Money>("creditlimit") != null
                             && c.GetAttributeValue<Money>("creditlimit").Value > 20000
                             select new
                             {
                                 FirstName = c["firstname"],
                                 LastName = c["lastname"],
                                 CreditLimit = c.GetAttributeValue<Money>("creditlimit")
                             };
                foreach (var c in query8)
                {
                    Console.WriteLine($"  {c.FirstName} {c.LastName} - ${c.CreditLimit?.Value}");
                }
                Console.WriteLine();

                // Example 9: Greater than with DateTime
                Console.WriteLine("9. Date comparison - Anniversaries after Feb 5, 2010");
                Console.WriteLine("-".PadRight(70, '-'));
                var query9 = from c in orgContext.CreateQuery("contact")
                             where c.GetAttributeValue<DateTime?>("anniversary") > new DateTime(2010, 2, 5)
                             select new
                             {
                                 FirstName = c["firstname"],
                                 LastName = c["lastname"],
                                 Anniversary = c.GetAttributeValue<DateTime?>("anniversary")
                             };
                foreach (var c in query9)
                {
                    Console.WriteLine($"  {c.FirstName} {c.LastName} - {c.Anniversary?.ToShortDateString()}");
                }
                Console.WriteLine();

                // Example 10: Contains operator
                Console.WriteLine("10. Contains operator - Description contains 'Alpine'");
                Console.WriteLine("-".PadRight(70, '-'));
                var query10 = from c in orgContext.CreateQuery("contact")
                              where c.GetAttributeValue<string>("description") != null
                              && ((string)c["description"]).Contains("Alpine")
                              select new
                              {
                                  FirstName = c["firstname"],
                                  LastName = c["lastname"]
                              };
                foreach (var c in query10)
                {
                    Console.WriteLine($"  {c.FirstName} {c.LastName}");
                }
                Console.WriteLine();

                // Example 11: StartsWith operator
                Console.WriteLine("11. StartsWith - First names starting with 'Bri'");
                Console.WriteLine("-".PadRight(70, '-'));
                var query11 = from c in orgContext.CreateQuery("contact")
                              where c.GetAttributeValue<string>("firstname") != null
                              && ((string)c["firstname"]).StartsWith("Bri")
                              select new
                              {
                                  FirstName = c["firstname"],
                                  LastName = c["lastname"]
                              };
                foreach (var c in query11)
                {
                    Console.WriteLine($"  {c.FirstName} {c.LastName}");
                }
                Console.WriteLine();

                // Example 12: EndsWith operator
                Console.WriteLine("12. EndsWith - Last names ending with 'cox'");
                Console.WriteLine("-".PadRight(70, '-'));
                var query12 = from c in orgContext.CreateQuery("contact")
                              where c.GetAttributeValue<string>("lastname") != null
                              && ((string)c["lastname"]).EndsWith("cox")
                              select new
                              {
                                  FirstName = c["firstname"],
                                  LastName = c["lastname"]
                              };
                foreach (var c in query12)
                {
                    Console.WriteLine($"  {c.FirstName} {c.LastName}");
                }
                Console.WriteLine();

                // Example 13: AND/OR operators
                Console.WriteLine("13. AND/OR operators - (Redmond OR Bellevue) AND CreditLimit >= $200");
                Console.WriteLine("-".PadRight(70, '-'));
                var query13 = from c in orgContext.CreateQuery("contact")
                              where ((c.GetAttributeValue<string>("address1_city") == "Redmond" ||
                                     c.GetAttributeValue<string>("address1_city") == "Bellevue") &&
                                     c.GetAttributeValue<Money>("creditlimit") != null &&
                                     c.GetAttributeValue<Money>("creditlimit").Value >= 200)
                              select new
                              {
                                  FirstName = c["firstname"],
                                  LastName = c["lastname"],
                                  City = c["address1_city"],
                                  CreditLimit = c.GetAttributeValue<Money>("creditlimit")
                              };
                foreach (var c in query13)
                {
                    Console.WriteLine($"  {c.FirstName} {c.LastName} - {c.City} - ${c.CreditLimit?.Value}");
                }
                Console.WriteLine();

                // Example 14: OrderBy descending
                Console.WriteLine("14. OrderBy descending - Contacts by credit limit");
                Console.WriteLine("-".PadRight(70, '-'));
                var query14 = from c in orgContext.CreateQuery("contact")
                              where c.GetAttributeValue<Money>("creditlimit") != null
                              orderby c.GetAttributeValue<Money>("creditlimit").Value descending
                              select new
                              {
                                  FirstName = c["firstname"],
                                  LastName = c["lastname"],
                                  CreditLimit = c.GetAttributeValue<Money>("creditlimit")
                              };
                foreach (var c in query14)
                {
                    Console.WriteLine($"  ${c.CreditLimit?.Value} - {c.FirstName} {c.LastName}");
                }
                Console.WriteLine();

                // Example 15: Multiple OrderBy
                Console.WriteLine("15. Multiple OrderBy - By last name desc, first name asc");
                Console.WriteLine("-".PadRight(70, '-'));
                var query15 = from c in orgContext.CreateQuery("contact")
                              orderby c.GetAttributeValue<string>("lastname") descending,
                                     c.GetAttributeValue<string>("firstname") ascending
                              select new
                              {
                                  FirstName = c["firstname"],
                                  LastName = c["lastname"]
                              };
                foreach (var c in query15)
                {
                    Console.WriteLine($"  {c.LastName}, {c.FirstName}");
                }
                Console.WriteLine();

                // Example 16: Skip and Take (paging)
                Console.WriteLine("16. Skip and Take - Skip 1, take 2 contacts");
                Console.WriteLine("-".PadRight(70, '-'));
                var query16 = (from c in orgContext.CreateQuery("contact")
                               where c.GetAttributeValue<string>("lastname") != "Parker"
                               orderby c.GetAttributeValue<string>("firstname")
                               select new
                               {
                                   FirstName = c["firstname"],
                                   LastName = c["lastname"]
                               }).Skip(1).Take(2);
                foreach (var c in query16)
                {
                    Console.WriteLine($"  {c.FirstName} {c.LastName}");
                }
                Console.WriteLine();

                // Example 17: First
                Console.WriteLine("17. First - Get first contact");
                Console.WriteLine("-".PadRight(70, '-'));
                var firstContact = orgContext.CreateQuery("contact").First();
                Console.WriteLine($"  {firstContact.GetAttributeValue<string>("fullname")}");
                Console.WriteLine();

                // Example 18: Count with Distinct
                Console.WriteLine("18. Count - Number of accounts with county specified");
                Console.WriteLine("-".PadRight(70, '-'));
                var accountsWithCounty = (from a in orgContext.CreateQuery("account")
                                          where a.GetAttributeValue<string>("address1_county") != null
                                          select a).ToArray().Count();
                Console.WriteLine($"  Accounts with county: {accountsWithCounty}");
                Console.WriteLine();

                // Example 19: Distinct count of states
                Console.WriteLine("19. Distinct count - Unique states with accounts");
                Console.WriteLine("-".PadRight(70, '-'));
                var statesWithAccounts = (from a in orgContext.CreateQuery("account")
                                          where a.GetAttributeValue<string>("address1_stateorprovince") != null
                                          select a.GetAttributeValue<string>("address1_stateorprovince"))
                                          .Distinct().ToArray().Count();
                Console.WriteLine($"  Unique states: {statesWithAccounts}");
                Console.WriteLine();

                // Example 20: Self join
                Console.WriteLine("20. Self join - Accounts with parent accounts");
                Console.WriteLine("-".PadRight(70, '-'));
                var query20 = from a in orgContext.CreateQuery("account")
                              join a2 in orgContext.CreateQuery("account")
                              on a["parentaccountid"] equals a2["accountid"]
                              select new
                              {
                                  AccountName = a["name"],
                                  ParentName = a2["name"]
                              };
                foreach (var a in query20)
                {
                    Console.WriteLine($"  {a.AccountName} -> Parent: {a.ParentName}");
                }
                Console.WriteLine();

                // Example 21: Multiple joins
                Console.WriteLine("21. Multiple joins - Account, Contact, and Lead");
                Console.WriteLine("-".PadRight(70, '-'));
                var query21 = from a in orgContext.CreateQuery("account")
                              join c in orgContext.CreateQuery("contact")
                              on a["primarycontactid"] equals c["contactid"]
                              join l in orgContext.CreateQuery("lead")
                              on a["originatingleadid"] equals l["leadid"]
                              select new
                              {
                                  ContactName = c["fullname"],
                                  AccountName = a["name"],
                                  LeadName = l["fullname"]
                              };
                foreach (var item in query21)
                {
                    Console.WriteLine($"  Contact: {item.ContactName}, Account: {item.AccountName}, Lead: {item.LeadName}");
                }
                Console.WriteLine();

                // Example 22: GetAttributeValue method
                Console.WriteLine("22. GetAttributeValue - Strongly typed attribute access");
                Console.WriteLine("-".PadRight(70, '-'));
                var query22 = from c in orgContext.CreateQuery("contact")
                              where c.GetAttributeValue<int?>("numberofchildren") > 1
                              select new
                              {
                                  FirstName = c.GetAttributeValue<string>("firstname"),
                                  LastName = c.GetAttributeValue<string>("lastname"),
                                  NumberOfChildren = c.GetAttributeValue<int?>("numberofchildren")
                              };
                foreach (var c in query22)
                {
                    Console.WriteLine($"  {c.FirstName} {c.LastName} - Children: {c.NumberOfChildren}");
                }
                Console.WriteLine();

                // Example 23: Math operations
                Console.WriteLine("23. Math operations - Round, Floor, Ceiling on latitude");
                Console.WriteLine("-".PadRight(70, '-'));
                var query23 = from c in orgContext.CreateQuery("contact")
                              where c.GetAttributeValue<double?>("address1_latitude") != null
                              select new
                              {
                                  Name = c.GetAttributeValue<string>("fullname"),
                                  Latitude = c.GetAttributeValue<double?>("address1_latitude"),
                                  Round = Math.Round(c.GetAttributeValue<double>("address1_latitude")),
                                  Floor = Math.Floor(c.GetAttributeValue<double>("address1_latitude")),
                                  Ceiling = Math.Ceiling(c.GetAttributeValue<double>("address1_latitude"))
                              };
                foreach (var c in query23)
                {
                    Console.WriteLine($"  {c.Name}: {c.Latitude} -> Round:{c.Round}, Floor:{c.Floor}, Ceiling:{c.Ceiling}");
                }
                Console.WriteLine();
            }

            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("All LINQ query examples completed successfully!");
            Console.WriteLine("=".PadRight(70, '='));
        }

        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            Console.WriteLine();
            Console.WriteLine("Cleaning up...");
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine($"Deleting {entityStore.Count} created record(s)...");
                // Delete in reverse order to respect dependencies
                for (int i = entityStore.Count - 1; i >= 0; i--)
                {
                    service.Delete(entityStore[i].LogicalName, entityStore[i].Id);
                }
                Console.WriteLine("Records deleted.");
            }
        }

        #endregion

        #region Application Setup

        /// <summary>
        /// Contains the application's configuration settings.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Loads the application configuration settings from a JSON file.
        /// </summary>
        Program()
        {
            // Get the path to the appsettings file. If the environment variable is set,
            // use that file path. Otherwise, use the runtime folder's settings file.
            string? path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS");
            if (path == null) path = "appsettings.json";

            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }

        static void Main(string[] args)
        {
            Program app = new();

            // Create a Dataverse service client using the default connection string.
            ServiceClient serviceClient =
                new(app.Configuration.GetConnectionString("default"));

            if (!serviceClient.IsReady)
            {
                Console.WriteLine("Failed to connect to Dataverse.");
                Console.WriteLine("Error: {0}", serviceClient.LastError);
                return;
            }

            Console.WriteLine("Connected to Dataverse.");
            Console.WriteLine();

            bool deleteCreatedRecords = true;

            try
            {
                Setup(serviceClient);
                Run(serviceClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Cleanup(serviceClient, deleteCreatedRecords);

                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                serviceClient.Dispose();
            }
        }

        #endregion
    }
}
