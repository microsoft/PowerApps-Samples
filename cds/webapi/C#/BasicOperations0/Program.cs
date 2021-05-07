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

        static async Task Main(string[] args)
        {
            //List of references for records created in this sample
            List<EntityReference> entityRefs = new List<EntityReference>();
            bool deleteCreatedRecords = true;

            //Configures the service
            var config = new Config
            {
                Url = appSettings["Url"],
                GetAccessToken = GetToken, //Function defined in app (below) to manage getting OAuth token
                //Optional settings that have defaults if not specified:
                MaxRetries = byte.Parse(appSettings["MaxRetries"]), //Default: 2
                TimeoutInSeconds = ushort.Parse(appSettings["TimeoutInSeconds"]), //Default: 120
                Version = appSettings["Version"], //Default 9.1
                CallerObjectId = new Guid(appSettings["CallerObjectId"]) //Default empty Guid
            };

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
                var contact1Ref = await service.Create(contact1);
                Console.WriteLine($"Contact '{contact1.firstname} " +
                        $"{contact1.lastname}' created.");
                entityRefs.Add(contact1Ref); //To delete later
                Console.WriteLine($"Contact URI: {contact1Ref.GetPath()}");

                //Update a contact
                var contact1Add = new Contact { 
                    contactid = contact1Ref.Id,
                    annualincome = 80000,
                    jobtitle = "Junior Developer"
                };
               await service.Update(contact1Add);
                Console.WriteLine($"Contact '{contact1.firstname} {contact1.lastname}" +
                        "' updated with jobtitle and annual income.");

                //Retrieve a contact
                var retrievedContact1 = await service.Retrieve<Contact>(
                    contact1Ref, 
                    "$select=fullname,annualincome,jobtitle,description");

                Console.WriteLine($"Contact '{retrievedContact1.fullname}' retrieved: \n" +
                $"\tAnnual income: {retrievedContact1.annualincome}\n" +
                $"\tJob title: {retrievedContact1.jobtitle} \n" +
                //description is initialized empty.
                $"\tDescription: {retrievedContact1.description}.");

                //Modify specific properties and then update entity instance.
                var contact1Update = new Contact { 
                    contactid = contact1Ref.Id,
                    jobtitle = "Senior Developer",
                    annualincome = 95000,
                    description = "Assignment to-be-determined"
                };
                await service.Update(contact1Update);

                Console.WriteLine($"Contact '{retrievedContact1.fullname}' updated:\n" +
                   $"\tJob title: {contact1Update.jobtitle}\n" +
                   $"\tAnnual income: {contact1Update.annualincome}\n" +
                   $"\tDescription: {contact1Update.description}\n");

                // Change just one property
                string telephone1 = "555-0105";
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

                var account1 = new Account { 
                   name = "Contoso Ltd",
                   telephone1 = "555-5555"
                };
                account1.Setprimarycontactid(retrievedContact1);

                var account1Ref = await service.Create(account1);
                entityRefs.Add(account1Ref); //To delete later
                Console.WriteLine($"Account '{account1.name}' created.");
                Console.WriteLine($"Account URI: {account1Ref.GetPath()}");

                string accountQuery = "$select=name&$expand=primarycontactid($select=fullname,jobtitle,annualincome)";
                var retrievedAccount1 = await service.Retrieve<Account>(account1Ref, accountQuery);

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

                //TODO

                #endregion Section 3: Create related entities

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
