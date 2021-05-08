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
                var contact1 = new Contact
                {
                    firstname = "Rafel",
                    lastname = "Shillo"
                };

                //Create the Contact
                var contact1Ref = await service.Create(contact1);

                Console.WriteLine($"Contact '{contact1.firstname} " +
                        $"{contact1.lastname}' created.");

                entityRefs.Add(contact1Ref); //To delete later

                Console.WriteLine($"Contact URI: {contact1Ref.GetPath()}");

                //Update a contact
                var contact1Add = new Contact
                {
                    contactid = contact1Ref.Id,
                    annualincome = 80000,
                    jobtitle = "Junior Developer"
                };

                //Update the Contact
                await service.Update(contact1Add);

                Console.WriteLine($"Contact '{contact1.firstname} {contact1.lastname}" +
                        "' updated with jobtitle and annual income.");

                string contactQuery1 = "$select=fullname,annualincome,jobtitle,description";

                //Retrieve a contact
                var retrievedContact1 = await service.Retrieve<Contact>(
                    contact1Ref,
                    contactQuery1);

                Console.WriteLine($"Contact '{retrievedContact1.fullname}' retrieved: \n" +
                $"\tAnnual income: {retrievedContact1.annualincome}\n" +
                $"\tJob title: {retrievedContact1.jobtitle} \n" +
                //description is initialized empty.
                $"\tDescription: {retrievedContact1.description}.");

                //Modify specific properties and then update entity instance.
                var contact1Update = new Contact
                {
                    contactid = contact1Ref.Id,
                    jobtitle = "Senior Developer",
                    annualincome = 95000,
                    description = "Assignment to-be-determined"
                };

                //Update the contact
                await service.Update(contact1Update);

                Console.WriteLine($"Contact '{retrievedContact1.fullname}' updated:\n" +
                   $"\tJob title: {contact1Update.jobtitle}\n" +
                   $"\tAnnual income: {contact1Update.annualincome}\n" +
                   $"\tDescription: {contact1Update.description}\n");

                // Change just one property
                string telephone1 = "555-0105";

                //Change the property value
                await service.Set(contact1Ref, "telephone1", telephone1);

                Console.WriteLine($"Contact '{retrievedContact1.fullname}' " +
                        $"phone number updated.");

                //Now retrieve just the single property.
                var telephone1Value = await service.Get<string>(contact1Ref, "telephone1");

                Console.WriteLine($"Contact's telephone # is: {telephone1Value}.");

                #endregion Section 1: Basic Create and Update operations

                #region Section 2: Create record associated to another

                /// <summary>
                /// Demonstrates creation of entity instance and simultaneous association to another,
                ///  existing entity.
                /// </summary>
                ///
                Console.WriteLine("\n--Section 2 started--");

                var account1 = new Account
                {
                    name = "Contoso Ltd",
                    telephone1 = "555-5555"
                };
                //Sets the primary contact value
                account1.Setprimarycontactid(retrievedContact1);

                //Create the account
                var account1Ref = await service.Create(account1);

                entityRefs.Add(account1Ref); //To delete later

                Console.WriteLine($"Account '{account1.name}' created.");
                Console.WriteLine($"Account URI: {account1Ref.GetPath()}");

                string accountQuery1 = "$select=name&" +
                    "$expand=primarycontactid(" +
                    "$select=fullname,jobtitle,annualincome)";

                //Retrieve the account
                var retrievedAccount1 = await service.Retrieve<Account>(account1Ref, accountQuery1);

                Console.WriteLine($"Account '{retrievedAccount1.name}' has primary contact " +
                    $"'{retrievedAccount1.primarycontactid.fullname}':");
                Console.WriteLine($"\tJob title: {retrievedAccount1.primarycontactid.jobtitle} \n" +
                    $"\tAnnual income: {retrievedAccount1.primarycontactid.annualincome}");

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

                var account2 = new Account
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
                var account2Ref = await service.Create(account2);

                Console.WriteLine($"Account '{account2.name}  created.");

                entityRefs.Add(account2Ref); //To delete later
                

                Console.WriteLine($"Account URI: {account2Ref.GetPath()}");

                string accountQuery2 = "$select=name,&" +
                    "$expand=primarycontactid(" +
                    "$select=fullname,jobtitle,annualincome)";

                var retrievedAccount2 = await service.Retrieve<Account>(account2Ref, accountQuery2);

                var retrievedcontact2Ref = retrievedAccount2.primarycontactid.ToEntityReference();

                entityRefs.Add(retrievedcontact2Ref);// To Delete later

                Console.WriteLine($"Account '{retrievedAccount2.name}' " +
                        $"has primary contact '{retrievedAccount2.primarycontactid.fullname}':");

                Console.WriteLine($"\tJob title: {retrievedAccount2.primarycontactid.jobtitle} \n" +
                        $"\tAnnual income: {retrievedAccount2.primarycontactid.annualincome}");


                //Next retrieve same contact and her assigned tasks.
                string contactQuery2 = "$select=fullname&" +
                    "$expand=Contact_Tasks(" +
                    "$select=subject,description,scheduledstart,scheduledend)";

                var retrievedcontact2 = await service.Retrieve<Contact>(retrievedcontact2Ref, contactQuery2);

                Console.WriteLine($"Contact '{retrievedcontact2.fullname}' has the following assigned tasks:");
                foreach (TaskActivity task in retrievedcontact2.Contact_Tasks)
                {
                    Console.WriteLine(
                        $"Subject: {task.subject}, \n" +
                        $"\tDescription: {task.description}\n" +
                        $"\tStart: {task.scheduledstart:d}\n" +
                        $"\tEnd: {task.scheduledend:d}\n");
                }

                #endregion Section 3: Create related entities

                #region Section 4: Associate and Disassociate entities

                //TODO

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
                    await service.Delete(reference);
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
