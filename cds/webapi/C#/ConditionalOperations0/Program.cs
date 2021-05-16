using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text.Json;
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

        //References for records referenced in this sample
        private static readonly List<EntityReference> entityRefs = new List<EntityReference>();

        public static async Task Main()
        {
            Console.Title = "Conditional Operations demonstration";

            try
            {
                var service = new Service(config);

                Console.WriteLine("--Starting conditional operations demonstration--\n");
                #region Create required records
                var accountContosoRef = await service.Create(
                   entity: new Account
                        {
                            name = "Contoso Ltd.",
                            telephone1 = "555-0000",
                            revenue = 5000000,
                            description = "Parent company of Contoso Pharmaceuticals, etc."
                        }
                    );
                entityRefs.Add(accountContosoRef); //To delete later

                // Retrieve the account record that was just created.
                string queryOptions = "$select=name,revenue,telephone1,description";
                var retrievedAccountContoso = await service.Retrieve<Account>( 
                    entityReference: accountContosoRef, 
                    query: queryOptions);

                // Store the ETag value from the retrieved record
                string initialAcctETagVal = retrievedAccountContoso.ETag;

                Console.WriteLine("Created and retrieved the initial account, shown below:");
                Console.WriteLine(JsonSerializer.Serialize(
                    value: retrievedAccountContoso,
                    options: new JsonSerializerOptions { WriteIndented = true }));

                #endregion Create required records

                #region Conditional GET
                Console.WriteLine("\n** Conditional GET demonstration **");

                // Attempt to retrieve the account record using a conditional GET defined by a message header with
                // the current ETag value.

                try
                {
                    retrievedAccountContoso = await service.Retrieve<Account>(
                       entityReference: accountContosoRef,
                       query: queryOptions,
                       formattedValues: false,
                       eTag: initialAcctETagVal //Passing this etag value will add the 'If-None-Match' header in the Retrieve method.
                      );

                    // Not expected. The returned response contains content.
                    Console.WriteLine("Instance retrieved using ETag: {0}", initialAcctETagVal);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException is ServiceException exception &&
                        exception.HttpStatusCode.Equals(HttpStatusCode.NotModified))
                    {

                        Console.WriteLine("Account record retrieved using ETag: {0}", initialAcctETagVal);
                        Console.WriteLine("Expected outcome: Entity was not modified so nothing was returned.");
                    }
                    else
                    {
                        //Some other error occurred;
                        throw;
                    }

                }
                // Modify the account instance by updating the telephone1 attribute
                await service.Set(
                    entityReference: accountContosoRef, 
                    property: "telephone1", 
                    value: "555-0001");
                Console.WriteLine("\n\bAccount telephone number updated to '555-0001'.\n");

                // Re-attempt to retrieve using conditional GET defined by a message header with
                // the current ETag value.
                try
                {
                    retrievedAccountContoso = await service.Retrieve<Account>(
                       entityReference: accountContosoRef,
                       query: queryOptions,
                       formattedValues: false,
                       eTag: initialAcctETagVal //Passing this etag value will add the 'If-None-Match' header in the Retrieve method.
                      );

                    // Expected: The returned response contains content.
                    Console.WriteLine("Expected Outcome: Instance retrieved using ETag: {0}", initialAcctETagVal);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException is ServiceException exception &&
                        exception.HttpStatusCode.Equals(HttpStatusCode.NotModified))
                    {

                        Console.WriteLine("Unexpected outcome: Entity was modified so something should be returned.");
                    }
                    else
                    {
                        //Some other error occurred;
                        throw;
                    }

                }

                // Save the updated ETag value
                var updatedAcctETagVal = retrievedAccountContoso.ETag;

                //Display the updated record:
                Console.WriteLine(JsonSerializer.Serialize(
                    value: retrievedAccountContoso,
                    options: new JsonSerializerOptions { WriteIndented = true }));

                #endregion Conditional GET

                #region Optimistic concurrency on delete and update
                Console.WriteLine("\n** Optimistic concurrency demonstration **");

                // Attempt to delete original account (if matches original ETag value).
                // If you replace "initialAcctETagVal" with "updatedAcctETagVal", the delete will
                // succeed. However, we want the delete to fail for now to demonstrate use of the ETag.
                Console.WriteLine("Attempting to delete the account using the original ETag value");

                try
                {
                    await service.Delete(
                        entityReference: accountContosoRef,
                        eTag: initialAcctETagVal //Passing this etag value will add the 'If-Match' header in the Delete method.
                        );

                    // Not expected; this code should not execute.
                    Console.WriteLine("Unexpected outcome: Account deleted!");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException is ServiceException exception &&
                        exception.HttpStatusCode.Equals(HttpStatusCode.PreconditionFailed))
                    {
                        Console.WriteLine("Expected outcome: Account NOT deleted.");
                    }
                    else
                    {
                        //Some other error occurred;
                        throw;
                    }
                }

                Console.WriteLine("Attempting to update the account using the original ETag value");
                var accountUpdate = new Account
                {
                    accountid = accountContosoRef.Id,
                    telephone1 = "555-0002",
                    revenue = 6000000
                };

                try
                {
                    await service.Update(
                        entity: accountUpdate,
                        eTag: initialAcctETagVal //Passing this etag value will add the 'If-Match' header in the Update method.
                        );


                    // Not expected; this code should not execute.
                    Console.WriteLine("Unexpected outcome: Account updated!");

                }
                catch (Exception ex)
                {
                    if (ex.InnerException is ServiceException exception &&
                       exception.HttpStatusCode.Equals(HttpStatusCode.PreconditionFailed))
                    {
                        Console.WriteLine("Expected outcome: Account NOT updated.");
                    }
                    else
                    {
                        //Some other error occurred;
                        throw;
                    }
                }

                // Reattempt update if matches current ETag value.
                Console.WriteLine("Attempting to update the account using the current updatedAcctETagVal ETag value");

                try
                {
                    await service.Update(
                        entity: accountUpdate,
                        eTag: updatedAcctETagVal //Passing this etag value will add the 'If-Match' header in the Update method.
                        );



                    Console.WriteLine("Expected outcome: Account updated.");

                }
                catch (Exception ex)
                {
                    if (ex.InnerException is ServiceException exception &&
                       exception.HttpStatusCode.Equals(HttpStatusCode.PreconditionFailed))
                    {
                        // Not expected; this code should not execute.
                        Console.WriteLine("Unexpected outcome: Account not updated!");
                    }
                    else
                    {
                        //Some other error occurred;
                        throw;
                    }
                }

                // Retrieve and output current account state.
                retrievedAccountContoso = await service.Retrieve<Account>(
                    entityReference: accountContosoRef, 
                    query: queryOptions);
                Console.WriteLine("\nBelow is the final state of the account");

                Console.WriteLine(JsonSerializer.Serialize(
                    value: retrievedAccountContoso,
                    options: new JsonSerializerOptions { WriteIndented = true }));



                #endregion Optimistic concurrency on delete and update

                #region Delete created records

                // Delete (or keep) all the created entity records.
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine().Trim();

                if (!(answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty))
                    entityRefs.Clear();

                foreach (EntityReference entityRef in entityRefs) await service.Delete(entityReference: entityRef);

                #endregion Delete created records 

                Console.WriteLine("--Conditional operations demonstration Completed--");

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
