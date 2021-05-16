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

        private static EntityReference sampleCaseRef, opportunityToWinRef, contactJonFoggRef;

        public static async Task Main()
        {
            Console.Title = "Function and Actions demonstration";

            try
            {
                var service = new Service(config);

                await CreateRequiredRecords(service);

                #region Call an unbound function with no parameters
                Console.WriteLine("\n* Call an unbound function with no parameters.");

                WhoAmIResponse whoAmI = await service.WhoAmI();

                var userRef = new EntityReference(whoAmI.UserId, "systemusers");

                // Obtain the user's full name
                SystemUser user = await service.Retrieve<SystemUser>(userRef, "$select=fullname");

                Console.WriteLine($"\tCurrent user's full name is '{user.fullname}'.");

                #endregion Call an unbound function with no parameters

                #region Call an unbound function that requires parameters
                Console.WriteLine("\n* Call an unbound function that requires parameters");

                string localizedTimeZoneName = "Pacific Standard Time";
                int localeIdEnglish = 1033;

                var response = await service.GetTimeZoneCodeByLocalizedName( 
                    localizedStandardName: localizedTimeZoneName, 
                    localeId: localeIdEnglish);

                Console.WriteLine(
                        $"\tThe time zone '{localizedTimeZoneName}' has the code '{response.TimeZoneCode}'.");

                #endregion Call an unbound function that requires parameters

                #region Call a bound function   
                Console.WriteLine("\n* Call a bound function");

                CalculateTotalTimeIncidentResponse calculateTotalTimeIncidentResponse = 
                    await service.CalculateTotalTimeIncident(sampleCaseRef);

                Console.WriteLine("\tThe total duration of tasks associated with the incident " +
                        $"is {calculateTotalTimeIncidentResponse.TotalTime} minutes.");

                #endregion Call a bound function 

                #region Call an unbound action that requires parameters
                Console.WriteLine("\n* Call an unbound action that requires parameters");

                //An Opportunity representing the one alread created in CreateRequiredRecords
                var opportunityToWin = new Opportunity { 
                    opportunityid = opportunityToWinRef.Id
                };

                //One of the parameters for the WinOpportunity Actions
                var opportunityCloseActivity = new OpportunityCloseActivity { 
                    subject = "Won Opportunity"
                };
                //Associate the OpportunityCloseActivity with the Opportunity Created
                opportunityCloseActivity.Setopportunityid(opportunityToWin);

                await service.WinOpportunity(
                    status: 3, //The Opportunity.statuscode value for 'Won'.
                    opportunityClose: opportunityCloseActivity
                 );

                Console.WriteLine("\tOpportunity closed as 'Won' with related OpportunityClose activity.");

                #endregion Call an unbound action that requires parameters

                #region Call a bound action that requires parameters
                Console.WriteLine("\n* Call a bound action that requires parameters");

                //Create a letter to add to the user's queue
                var letter = new LetterActivity { 
                    subject = "Example Letter",
                    description = "Body of the letter"
                };

                Console.WriteLine("\tCreating letter 'Example letter'..");

                letter = await service.CreateRetrieve<LetterActivity>(letter, "$select=activityid");
                entityRefs.Add(letter.ToEntityReference()); ; //To Delete later

                //Get the users queue
                user = await service.Retrieve<SystemUser>(userRef, "$select=systemuserid&$expand=queueid($select=queueid)");
                var userQueue = user.queueid;

                AddToQueueResponse addQueueResponse = await service.AddToQueue( 
                    queue: userQueue, //The queue the item will be added to
                    target: letter //The item to add
                    );

                Console.WriteLine("\tLetter 'Example letter' added to current user's queue.");
                Console.WriteLine($"\tQueueItemId returned from AddToQueue action: {addQueueResponse.QueueItemId}");

                #endregion Call a bound action that requires parameters

                #region Call a bound custom action that requires parameters
                Console.WriteLine("\n* Call a bound custom action that requires parameters");

                /* This is the definition of the custom action in $metadata
                <Action Name="sample_AddNoteToContact" IsBound="true">
                    <Parameter Name="entity" Type="mscrm.contact" Nullable="false" />
                    <Parameter Name="NoteTitle" Type="Edm.String" Nullable="false" Unicode="false" />
                    <Parameter Name="NoteText" Type="Edm.String" Nullable="false" Unicode="false" />
                    <ReturnType Type="mscrm.annotation" Nullable="false" />
                </Action> 
                */


                var contactParam = new Contact { 
                    contactid = contactJonFoggRef.Id //Instantiate a Contact with the id of one created earlier
                };

                Annotation note = await service.sample_AddNoteToContact(contactParam, "Sample Note", "The text content of the note.");

                //Retrieve the properties of the note created with the Contact
                note = await service.Retrieve<Annotation>(note.ToEntityReference(), "$select=subject,notetext&$expand=objectid_contact($select=fullname)");

                Console.WriteLine($"\tA note with the title '{note.subject}'" +
                    $"\n\tand content: '{note.notetext}'" +
                    $"\n\twas created and associated with the contact '{note.objectid_contact.fullname}'.");

                #endregion Call a bound custom action that requires parameters

                #region Call an unbound custom action that requires parameters
                Console.WriteLine("\n* Call an unbound custom action that requires parameters");

                // Create a customer of the specified type, using the custom action sample_CreateCustomer,
                // which takes two parameters: the type of customer ('account' or 'contact') and the name of 
                // the new customer.

                /* This is the definition of the custom action in $metadata
                <Action Name="sample_CreateCustomer">
                    <Parameter Name="CustomerType" Type="Edm.String" Nullable="false" Unicode="false" />
                    <Parameter Name="AccountName" Type="Edm.String" Unicode="false" />
                    <Parameter Name="ContactFirstName" Type="Edm.String" Unicode="false" />
                    <Parameter Name="ContactLastName" Type="Edm.String" Unicode="false" />
                </Action> 
                */

                string accountname = "New account customer (sample)";
                await service.sample_CreateCustomer("account", accountname);
                Console.WriteLine($"\tThe account '{accountname}' was created.");

                // Because the CreateCustomer custom action does not return any data about the created instance, 
                // we must query the customer instance to figure out its Id so we can delete it
                var accounts = await service.RetrieveMultiple<Account>($"$select=name&$filter=name eq '{accountname}'&$top=1");
                var createdAccount = accounts.Value.FirstOrDefault();
                entityRefs.Add(createdAccount.ToEntityReference()); //To delete later

                // Try to call the same custom action with invalid parameters, here the same name is
                // not valid for a contact. (ContactFirstname and ContactLastName parameters are  
                // required when CustomerType is contact.

                try
                {
                    await service.sample_CreateCustomer("contact", accountname);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n\t Expected Error: \n\t\t{ex.Message}");
                }

                #endregion Call an unbound custom action that requires parameters

                DeleteEntityRecords(service);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

        private static async Task CreateRequiredRecords(Service service)
        {

            Console.WriteLine("* Creating required records");

            var fourthCoffee = await service.CreateRetrieve<Account>(new Account { name = "Fourth Coffee" }, "$select=accountid");
            var fourthCoffeeRef = fourthCoffee.ToEntityReference();
            entityRefs.Add(fourthCoffeeRef); //To delete later

            var sampleCase = new Incident
            {
                title = "Sample Case",
                Incident_Tasks = new List<TaskActivity>{
                    {
                        new TaskActivity{
                            subject = "Task 1",
                            actualdurationminutes = 30
                        }

                    },
                    {
                        new TaskActivity{
                            subject = "Task 2",
                            actualdurationminutes = 30
                        }

                    },
                    {
                        new TaskActivity{
                            subject = "Task 3",
                            actualdurationminutes = 30
                        }

                    }
                }
            };
            sampleCase.Setcustomerid_account(fourthCoffee);

            sampleCase = await service.CreateRetrieve<Incident>(sampleCase, "$select=incidentid&$expand=Incident_Tasks($select=activityid)");
            sampleCaseRef = sampleCase.ToEntityReference();
            entityRefs.Insert(0, sampleCaseRef); //To delete later
            sampleCase.Incident_Tasks.ForEach(async t =>
            {

                var taskToUpdate = new TaskActivity
                {
                    activityid = t.Id,
                    statecode = 1,
                    statuscode = 5
                };

                await service.Update(taskToUpdate);

            });

            var accountCohoWinery = new Account
            {
                name = "Coho Winery",
                opportunity_customer_accounts = new List<Opportunity> { 
                    { 
                        new Opportunity { name = "Opportunity to Win" } 
                    } 
                }
            };
            accountCohoWinery = await service.CreateRetrieve<Account>(
                entity: accountCohoWinery, 
                query: "$select=accountid&$expand=opportunity_customer_accounts($select=opportunityid)"
                );


            opportunityToWinRef = accountCohoWinery.opportunity_customer_accounts.FirstOrDefault().ToEntityReference();
            entityRefs.Add(opportunityToWinRef);           
            entityRefs.Add(accountCohoWinery.ToEntityReference());

            //Create a contact to use with custom action sample_AddNoteToContact
           contactJonFoggRef =  await service.Create(new Contact { firstname="Jon", lastname = "Fogg" });
           entityRefs.Add(contactJonFoggRef); //To delete later

            Console.WriteLine("* Finished creating required records");            

        }

        private static void DeleteEntityRecords(Service service)
        {
            // Delete (or keep) all the created entity records.
            Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
            string answer = Console.ReadLine().Trim();

            if (answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty)
            {
                entityRefs.ForEach(e =>
                {

                    try
                    {
                        service.Delete(e).GetAwaiter().GetResult();
                        Console.WriteLine($"{e.Path} Deleted.");
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is ServiceException exception)
                        {

                            Console.WriteLine($"Skipping {e.Path} because {exception.ReasonPhrase}.");
                        }
                        else
                        {
                            throw;
                        }
                    }
                });
            }
            entityRefs.Clear();
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
