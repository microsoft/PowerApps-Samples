using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;

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

        

        public static async Task Main()
        {
            //List of references for records created in this sample
            List<EntityReference> entityRefs = new List<EntityReference>();
            bool deleteCreatedRecords = true;

            var service = new Service(config);

            try
            {
                Console.WriteLine("--Starting Basic Operations--");

                #region Section 1: Basic Create and Update operations

                Console.WriteLine("--Section 1 started--");

                //Create a contact
                var contactRafelShillo = new Contact
                {
                    firstname = "Rafel",
                    lastname = "Shillo"
                };

                //Create the Contact
                var contactRafelShilloRef = await service.Create( entity: contactRafelShillo);

                Console.WriteLine($"Contact '{contactRafelShillo.firstname} " +
                        $"{contactRafelShillo.lastname}' created.");

                entityRefs.Add(contactRafelShilloRef); //To delete later

                Console.WriteLine($"Contact URI: {contactRafelShilloRef.Path}");

                //Update a contact
                var contactRafelShilloUpdate1 = new Contact
                {
                    contactid = contactRafelShilloRef.Id,
                    annualincome = 80000,
                    jobtitle = "Junior Developer"
                };

                //Update the Contact
                await service.Update(entity: contactRafelShilloUpdate1);

                Console.WriteLine($"Contact '{contactRafelShillo.firstname} {contactRafelShillo.lastname}" +
                        "' updated with jobtitle and annual income.");               

                //Retrieve a contact
                var retrievedContactRafelShillo = await service.Retrieve<Contact>(
                    entityReference: contactRafelShilloRef,
                    query: "$select=fullname,annualincome,jobtitle,description");

                Console.WriteLine($"Contact '{retrievedContactRafelShillo.fullname}' retrieved: \n" +
                $"\tAnnual income: {retrievedContactRafelShillo.annualincome}\n" +
                $"\tJob title: {retrievedContactRafelShillo.jobtitle} \n" +
                //description is initialized empty.
                $"\tDescription: {retrievedContactRafelShillo.description}.");

                //Modify specific properties and then update entity instance.
                var contactRafelShilloUpdate2 = new Contact
                {
                    contactid = contactRafelShilloRef.Id,
                    jobtitle = "Senior Developer",
                    annualincome = 95000,
                    description = "Assignment to-be-determined"
                };

                //Update the contact
                await service.Update(entity: contactRafelShilloUpdate2);

                Console.WriteLine($"Contact '{retrievedContactRafelShillo.fullname}' updated:\n" +
                   $"\tJob title: {contactRafelShilloUpdate2.jobtitle}\n" +
                   $"\tAnnual income: {contactRafelShilloUpdate2.annualincome}\n" +
                   $"\tDescription: {contactRafelShilloUpdate2.description}\n");

                // Change just one property
                string telephone1 = "555-0105";

                //Change the property value
                await service.Set(
                    entityReference: contactRafelShilloRef, 
                    property: "telephone1", 
                    value: telephone1);

                Console.WriteLine($"Contact '{retrievedContactRafelShillo.fullname}' " +
                        $"phone number updated.");

                //Now retrieve just the single property.
                var telephone1Value = await service.Get<string>(
                    entityReference: contactRafelShilloRef, 
                    property: "telephone1");

                Console.WriteLine($"Contact's telephone # is: {telephone1Value}.");

                #endregion Section 1: Basic Create and Update operations

                #region Section 2: Create record associated to another

                /// <summary>
                /// Demonstrates creation of entity instance and simultaneous association to another,
                ///  existing entity.
                /// </summary>
                ///
                Console.WriteLine("\n--Section 2 started--");

                var accountContoso = new Account
                {
                    name = "Contoso Ltd",
                    telephone1 = "555-5555"
                };
                //Sets the primary contact value
                accountContoso.Setprimarycontactid(retrievedContactRafelShillo);

                //Create the account
                var accountContosoRef = await service.Create(entity: accountContoso);

                entityRefs.Add(item: accountContosoRef); //To delete later

                Console.WriteLine($"Account '{accountContoso.name}' created.");
                Console.WriteLine($"Account URI: {accountContosoRef.Path}");

                string accountQuery1 = "$select=name&" +
                    "$expand=primarycontactid(" +
                    "$select=fullname,jobtitle,annualincome)";

                //Retrieve the account
                var retrievedAccountContoso = await service.Retrieve<Account>( 
                    entityReference: accountContosoRef, 
                    query: accountQuery1);

                Console.WriteLine($"Account '{retrievedAccountContoso.name}' has primary contact " +
                    $"'{retrievedAccountContoso.primarycontactid.fullname}':");
                Console.WriteLine($"\tJob title: {retrievedAccountContoso.primarycontactid.jobtitle} \n" +
                    $"\tAnnual income: {retrievedAccountContoso.primarycontactid.annualincome}");

                #endregion Section 2: Create record associated to another

                #region Section 3: Create related entities

                /// <summary>
                /// Demonstrates creation of entity instance and related entities in a single operation.
                /// </summary>
                ///
                Console.WriteLine("\n--Section 3 started--");
                //Create the following entries in one operation: an account, its
                // associated primary contact, and open tasks for that contact.  These
                // entity types have the following relationships:
                //    Accounts
                //       |---[Primary] Contact (N-to-1)
                //              |---Tasks (1-to-N)

                var accountFourthCoffee = new Account
                {
                    name = "Fourth Coffee",
                    primarycontactid = new Contact
                    {
                        firstname = "Susie",
                        lastname = "Curtis",
                        annualincome = 48000,
                        jobtitle = "Coffee Master",
                        Contact_Tasks = new List<TaskActivity> {
                            {
                                new TaskActivity{
                                 subject = "Sign invoice",
                                 description = "Invoice #12321",
                                 scheduledend = DateTimeOffset.Parse("4/19/2022")
                                }
                            },
                            {
                                new TaskActivity{
                                 subject = "Setup new display",
                                 description = "Theme is - Spring is in the air",
                                 scheduledend = DateTimeOffset.Parse("4/20/2022")
                                }
                            },
                            {
                                new TaskActivity{
                                 subject = "Conduct training",
                                 description = "Train team on making our new blended coffee",
                                 scheduledend = DateTimeOffset.Parse("6/1/2022")
                                }
                            }
                        }
                    }
                };

                //Create the account, contact, and related tasks
                var accountFourthCoffeeRef = await service.Create(entity: accountFourthCoffee);

                Console.WriteLine($"Account '{accountFourthCoffee.name}  created.");

                entityRefs.Add(accountFourthCoffeeRef); //To delete later
                

                Console.WriteLine($"Account URI: {accountFourthCoffeeRef.Path}");

                string accountQuery2 = "$select=name,&" +
                    "$expand=primarycontactid(" +
                    "$select=fullname,jobtitle,annualincome)";

                var retrievedAccountFourthCoffee = await service.Retrieve<Account>(
                    entityReference: accountFourthCoffeeRef, 
                    query: accountQuery2);

                var retrievedContactSusieCurtisRef = retrievedAccountFourthCoffee.primarycontactid.ToEntityReference();

                entityRefs.Add(retrievedContactSusieCurtisRef);// To Delete later

                Console.WriteLine($"Account '{retrievedAccountFourthCoffee.name}' " +
                        $"has primary contact '{retrievedAccountFourthCoffee.primarycontactid.fullname}':");

                Console.WriteLine($"\tJob title: {retrievedAccountFourthCoffee.primarycontactid.jobtitle} \n" +
                        $"\tAnnual income: {retrievedAccountFourthCoffee.primarycontactid.annualincome}");


                //Next retrieve same contact and her assigned tasks.
                string contactQuery2 = "$select=fullname&" +
                    "$expand=Contact_Tasks(" +
                    "$select=subject,description,scheduledstart,scheduledend)";

                var retrievedContactSusieCurtis = await service.Retrieve<Contact>(
                    entityReference: retrievedContactSusieCurtisRef, 
                    query: contactQuery2);

                Console.WriteLine($"Contact '{retrievedContactSusieCurtis.fullname}' has the following assigned tasks:");
                foreach (TaskActivity task in retrievedContactSusieCurtis.Contact_Tasks)
                {
                    Console.WriteLine(
                        $"Subject: {task.subject}, \n" +
                        $"\tDescription: {task.description}\n" +
                        $"\tStart: {task.scheduledstart:d}\n" +
                        $"\tEnd: {task.scheduledend:d}\n");
                }

                #endregion Section 3: Create related entities

                #region Section 4: Associate and Disassociate entities

                /// <summary>
                /// Demonstrates associating and disassociating of existing entity instances.
                /// </summary>
                Console.WriteLine("\n--Section 4 started--");
                //Add 'Rafel Shillo' to the contact list of 'Fourth Coffee',
                // a 1-to-N relationship.
                await service.Add(
                    entityReference: accountFourthCoffeeRef, 
                    collectionName: "contact_customer_accounts", 
                    entityToAdd: contactRafelShilloRef);

                //Retrieve and output all contacts for account 'Fourth Coffee'

                var contacts = await service.RetrieveRelatedMultiple<Contact>(
                    parent: accountFourthCoffeeRef, 
                    navigationProperty: "contact_customer_accounts", 
                    query: "$select=fullname,jobtitle");

                Console.WriteLine($"Contact list for account '{retrievedAccountFourthCoffee.name}':");

                foreach (Contact contact in contacts.Value)
                {
                    Console.WriteLine($"\tName: {contact.fullname}, Job title: {contact.jobtitle}");
                }

                //Dissociate the contact from the account. 
                await service.Remove(
                    entityReference: accountFourthCoffeeRef, 
                    navigationProperty: "contact_customer_accounts", 
                    collectionItem: contactRafelShilloRef);

                //'Rafel Shillo' was removed from the the contact list of 'Fourth Coffe

                //Associate an opportunity to a competitor, an N-to-N relationship.

                var competitorAW = new Competitor { 
                    name = "Adventure Works",
                    strengths = "Strong promoter of private tours for multi-day outdoor adventures"
                };

                var competitorAWRef = await service.Create(entity: competitorAW);
                entityRefs.Add(item: competitorAWRef); //To delete later

                var oppor1 = new Opportunity { 
                    name = "River rafting adventure",
                    description = "Sales team on a river-rafting offsite and team building"
                };

                var oppor1Ref = await service.Create(entity: oppor1);
                entityRefs.Add(item: oppor1Ref); //To delete later

                //Associate opportunity to competitor via opportunitycompetitors_association.
                await service.Add(
                    entityReference: oppor1Ref, 
                    collectionName: "opportunitycompetitors_association", 
                    entityToAdd: competitorAWRef);

                //Retrieve all opportunities for competitor 'Adventure Works'
                var retrievedOpporList1 = await service.RetrieveRelatedMultiple<Opportunity>(
                    parent: competitorAWRef, 
                    navigationProperty: "opportunitycompetitors_association", 
                    query: "$select=name,description");


                Console.WriteLine($"Competitor '{competitorAW.name}' has the following opportunities:");
                foreach (Opportunity op in retrievedOpporList1.Value)
                {
                    Console.WriteLine($"\tName: {op.name}, \n" +
                        $"\tDescription: {op.description}");
                }

                //Dissociate opportunity from competitor.
                await service.Remove(
                    entityReference: oppor1Ref, 
                    navigationProperty: "opportunitycompetitors_association", 
                    collectionItem: competitorAWRef);

                // 'River rafting adventure' opportunity disassociated with 'Adventure Works' competitor

                #endregion Section 4: Associate and Disassociate entities

                #region Section 5: Delete sample entities

                if (!deleteCreatedRecords)
                {
                    Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                    string answer = Console.ReadLine();
                    answer = answer.Trim();
                    if (!(answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty))
                    { entityRefs.Clear(); }
                    else
                    {
                        Console.WriteLine("\nDeleting created records.");
                    }
                }
                else
                {
                    Console.WriteLine("\nDeleting created records.");
                }

                foreach (EntityReference reference in entityRefs)
                {
                    await service.Delete(entityReference: reference);
                }

                #endregion Section 5: Delete sample entities
                Console.WriteLine("--Basic Operations Completed--");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

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
