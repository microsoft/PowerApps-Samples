using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace PowerApps.Samples
{
    /// <summary>
    /// This program demonstrates use of functions and actions with the 
    /// Power Platform data service Web API.
    /// </summary>
    /// <remarks>Be sure to fill out App.config with your test environment information
    /// and import the provided managed solution into your test environment using the web app
    /// before running this program.</remarks>
    /// <see cref="Https://docs.microsoft.com/powerapps/developer/common-data-service/webapi/samples/functions-actions-csharp"/>
    public class Program
    {
        public static void Main()
        {
            Console.Title = "Function and Actions demonstration";

            // Track entity instance URIs so those records can be deleted prior to exit.
            Dictionary<string, Uri> entityUris = new Dictionary<string, Uri>();
            
            try
            {
                // Get environment configuration information from the connection string in App.config.
                ServiceConfig config = new ServiceConfig(
                    ConfigurationManager.ConnectionStrings["Connect"].ConnectionString);

                // Use the service class that handles HTTP messaging, error handling, and
                // performance optimizations.
                using (CDSWebApiService svc = new CDSWebApiService(config))
                {
                    // Create any entity instances required by the program code that follows
                    CreateRequiredRecords(svc, entityUris);

                    #region Call an unbound function with no parameters
                    Console.WriteLine("\n* Call an unbound function with no parameters.");

                    // Retrieve the current user's full name from the WhoAmI function:
                    Console.Write("\tGetting information on the current user..");
                    JToken currentUser = svc.Get("WhoAmI");

                    // Obtain the user's ID and full name
                    JToken user = svc.Get("systemusers(" + currentUser["UserId"] + ")?$select=fullname");

                    Console.WriteLine("completed.");
                    Console.WriteLine("\tCurrent user's full name is '{0}'.", user["fullname"]);
                    #endregion Call an unbound function with no parameters

                    #region Call an unbound function that requires parameters
                    Console.WriteLine("\n* Call an unbound function that requires parameters");

                    // Retrieve the code for the specified time zone
                    int localeID = 1033;
                    string timeZoneName = "Pacific Standard Time";

                    // Define the unbound function and its parameters
                    string[] uria = new string[] {
                        "GetTimeZoneCodeByLocalizedName",
                        "(LocalizedStandardName=@p1,LocaleId=@p2)",
                        "?@p1='" + timeZoneName + "'&@p2=" + localeID };

                    // This would also work:
                    // string[] uria = ["GetTimeZoneCodeByLocalizedName", "(LocalizedStandardName='" + 
                    //    timeZoneName + "',LocaleId=" + localeId + ")"]; 

                    JToken localizedName = svc.Get(string.Join("", uria));
                    string timeZoneCode = localizedName["TimeZoneCode"].ToString();

                    Console.WriteLine(
                        "\tThe time zone '{0}' has the code '{1}'.", timeZoneName, timeZoneCode);
                    #endregion Call an unbound function that requires parameters

                    #region Call a bound function   
                    Console.WriteLine("\n* Call a bound function");

                    // Retrieve the total time (minutes) spent on all tasks associated with 
                    // incident "Sample Case".
                    string boundUri = entityUris["Sample Case"] +
                        @"/Microsoft.Dynamics.CRM.CalculateTotalTimeIncident()";

                    JToken cttir = svc.Get(boundUri);
                    string totalTime = cttir["TotalTime"].ToString();

                    Console.WriteLine("\tThe total duration of tasks associated with the incident " +
                        "is {0} minutes.", totalTime);
                    #endregion Call a bound function 

                    #region Call an unbound action that requires parameters
                    Console.WriteLine("\n* Call an unbound action that requires parameters");

                    // Close the existing opportunity "Opportunity to win" and mark it as won.
                    JObject opportClose = new JObject()
                    {
                        { "subject", "Won Opportunity" },
                        { "opportunityid@odata.bind", entityUris["Opportunity to win"] }
                    };

                    JObject winOpportParams = new JObject()
                    {
                        { "Status", "3" },
                        { "OpportunityClose", opportClose }
                    };

                    JObject won = svc.Post("WinOpportunity", winOpportParams);

                    Console.WriteLine("\tOpportunity won.");
                    #endregion Call an unbound action that requires parameters

                    #region Call a bound action that requires parameters
                    Console.WriteLine("\n* Call a bound action that requires parameters");

                    // Add a new letter tracking activity to the current user's queue.
                    // First create a letter tracking instance.
                    JObject letterAttributes = new JObject()
                    {
                        {"subject", "Example letter" },
                        {"description", "Body of the letter" }
                    };

                    Console.Write("\tCreating letter 'Example letter'..");

                    Uri letterUri = svc.PostCreate("letters", letterAttributes);
                    entityUris.Add("Example letter", letterUri);

                    Console.WriteLine("completed.");

                    //Retrieve the ID associated with this new letter tracking activity.
                    JToken letter = svc.Get(letterUri + "?$select=activityid,subject");
                    string letterActivityId = (string)letter["activityid"];

                    // Retrieve the URL to current user's queue.
                    string myUserId = (string)currentUser["UserId"];

                    JToken queueRef = svc.Get("systemusers(" + myUserId + ")/queueid/$ref");
                    string myQueueUri = (string)queueRef["@odata.id"];

                    //Add the letter activity to current user's queue, then return its queue ID.
                    JObject targetUri = JObject.Parse(
                      @"{activityid: '" + letterActivityId + @"', '@odata.type': 'Microsoft.Dynamics.CRM.letter' }");

                    JObject addToQueueParams = new JObject()
                    {
                        { "Target", targetUri }
                    };

                    string queueItemId = (string)svc.Post(
                        myQueueUri + "/Microsoft.Dynamics.CRM.AddToQueue", addToQueueParams)["QueueItemId"];

                    Console.WriteLine("\tLetter 'Example letter' added to current user's queue.");
                    Console.WriteLine("\tQueueItemId returned from AddToQueue action: {0}", queueItemId);
                    #endregion Call a bound action that requires parameters

                    #region Call a bound custom action that requires parameters
                    Console.WriteLine("\n* Call a bound custom action that requires parameters");

                    // Add a note to a specified contact. Uses the custom action sample_AddNoteToContact, which
                    // is bound to the contact to annotate, and takes a single param, the note to add. It also  
                    // returns the URI to the new annotation. 

                    JObject note = JObject.Parse(
                        @"{NoteTitle: 'Sample note', NoteText: 'The text content of the note.'}");
                    string actionUri = entityUris["Jon Fogg"].ToString() + "/Microsoft.Dynamics.CRM.sample_AddNoteToContact";

                    JObject contact = svc.Post(actionUri, note);
                    Uri annotationUri = new Uri(svc.BaseAddress + "annotations(" + contact["annotationid"] + ")");
                    entityUris.Add((string)note["NoteTitle"], annotationUri);

                    Console.WriteLine("\tA note with the title '{0}' was created and " +
                        "associated with the contact 'Jon Fogg'.", note["NoteTitle"]);
                    #endregion Call a bound custom action that requires parameters

                    #region Call an unbound custom action that requires parameters
                    Console.WriteLine("\n* Call an unbound custom action that requires parameters");

                    // Create a customer of the specified type, using the custom action sample_CreateCustomer,
                    // which takes two parameters: the type of customer ('account' or 'contact') and the name of 
                    // the new customer.
                    string customerName = "New account customer (sample)";
                    JObject customerAttributes = JObject.Parse(
                        @"{CustomerType: 'account', AccountName: '" + customerName + "'}");

                    JObject response = svc.Post("sample_CreateCustomer", customerAttributes);
                    Console.WriteLine("\tThe account '" + customerName + "' was created.");

                    // Because the CreateCustomer custom action does not return any data about the created instance, 
                    // we must query the customer instance to figure out its URI.
                    JToken customer = svc.Get("accounts?$filter=name eq 'New account customer (sample)'&$select=accountid&$top=1");
                    Uri customerUri = new Uri(svc.BaseAddress + "accounts(" + customer["value"][0]["accountid"] + ")");
                    entityUris.Add( customerName, customerUri );

                    // Try to call the same custom action with invalid parameters, here the same name is
                    // not valid for a contact. (ContactFirstname and ContactLastName parameters are  
                    // required when CustomerType is contact.
                    customerAttributes = JObject.Parse(
                        @"{CustomerType: 'contact', AccountName: '" + customerName + "'}");

                    try
                    {
                        customerUri = svc.PostCreate("sample_CreateCustomer", customerAttributes);
                        Console.WriteLine("\tCall to the custom CreateCustomer action succeeded, which was not expected.");
                    }
                    catch (ServiceException e)
                    {
                        Console.WriteLine("\tCall to the custom CreateCustomer action did not succeed (as was expected).");
                        Console.WriteLine($"\t\tError: {e.Message}");
                    }
                    #endregion Call an unbound custom action that requires parameters

                    DeleteEntityRecords(svc, entityUris);
                }
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red; // Highlight exceptions
                if ( e is AggregateException )
                {
                    foreach (Exception inner in (e as AggregateException).InnerExceptions)
                    { Console.WriteLine("\n" + inner.Message); }
                }
                else if ( e is ServiceException)
                {
                    var ex = e as ServiceException;
                    Console.WriteLine("\nMessage send response: status code {0}, {1}",
                        ex.StatusCode, ex.ReasonPhrase);
                }
                Console.ReadKey(); // Pause terminal
            }
        }

        /// <summary>
        /// Deletes all entity instances created during program execution.
        /// </summary>
        /// <param name="svc">The service class used for messaging.</param>
        /// <param name="entityUris">A collection of entity instance Uri.</param>
        private static void DeleteEntityRecords(CDSWebApiService svc, Dictionary<string, Uri> entityUris)
        {
            // Delete (or keep) all the created entity records.
            Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
            String answer = Console.ReadLine().Trim();

            if (answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty)
            {
                foreach (string entityName in entityUris.Keys)
                {
                    try
                    {
                        Console.Write("Deleting '{0}'..", entityName);
                        svc.Delete(entityUris[entityName].ToString());
                        Console.WriteLine("done");
                    }
                    catch (ServiceException e)
                    { Console.WriteLine("skipping ({0})", e.ReasonPhrase); }
                }
            }
            entityUris.Clear();
        }

        /// <summary> 
        /// Creates the entity instances used by the main demonstration code.
        /// </summary>
        /// <param name="svc">The service class used for messaging.</param>
        /// <param name="entityUris">A collection of entity instance Uri for tracking and 
        /// later deletion (cleanup).</param>
        private static void CreateRequiredRecords(CDSWebApiService svc, Dictionary<string, Uri> entityUris)
        {
            Console.WriteLine("* Creating required records");

            // Create a parent account
            JObject parentAccountAttributes = new JObject() { { "name", "Fourth Coffee" } };
            entityUris.Add( "Fourth Coffee", 
                CreateEntity(svc, "accounts", parentAccountAttributes) );

            // Create an incident with three associated tasks
            JArray taskAttributess = new JArray(new JObject[]
            {
                JObject.Parse(@"{subject: 'Task 1', actualdurationminutes: 30}"),
                JObject.Parse(@"{subject: 'Task 2', actualdurationminutes: 30}"),
                JObject.Parse(@"{subject: 'Task 3', actualdurationminutes: 30}")
            });

            JObject incidentAttributes = new JObject()
            {
                { "title", "Sample Case" },
                { "customerid_account@odata.bind", entityUris["Fourth Coffee"] },
                { "Incident_Tasks", taskAttributess }
            };

            entityUris.Add( "Sample Case", 
                CreateEntity(svc, "incidents", incidentAttributes) );

            // Close the associated tasks so that they represent completed work
            try
            {
                Console.Write("\tSetting each incident task to completed..");
                JToken incidentTaskRefs = svc.Get(entityUris["Sample Case"] + "/Incident_Tasks/$ref");

                // Property value set used to mark tasks as completed. 
                JObject completeCode = JObject.Parse(@"{statecode: 1, statuscode: 5}");

                foreach (JToken taskId in incidentTaskRefs["value"])
                {
                    var taskUri = new Uri(taskId["@odata.id"].ToString());
                    svc.Patch(taskUri, completeCode);
                }
                Console.WriteLine("done.");
            }
            catch (ServiceException e)
            {
                Console.WriteLine("failed.");
                throw e;
            }

            //Create another account and associated opportunity (required for CloseOpportunityAsWon)
            JObject wineryAccountAttributes = new JObject()
            {
                {"name", "Coho Winery" },
                {"opportunity_customer_accounts", JArray.Parse(@"[{ name: 'Opportunity to win' }]") }
            };

            entityUris.Add("Coho Winery", 
                CreateEntity(svc, "accounts", wineryAccountAttributes));

            //Retrieve the URI of the associated opportunity
            try
            {
                Console.Write("\tRetrieving the URI of the opportunity 'Opportunity to win'..");
                JToken custOpporRefs = svc.Get(entityUris["Coho Winery"] + "/opportunity_customer_accounts/$ref");

                entityUris.Add( "Opportunity to win", 
                                new Uri(custOpporRefs["value"][0]["@odata.id"].ToString()) );
                Console.WriteLine("completed.");
            }
            catch (ServiceException e)
            {
                Console.WriteLine("failed.");
                throw e;
            }

            //Create a contact to use with custom action sample_AddNoteToContact
            JObject contactAttributes = JObject.Parse(@"{firstname: 'Jon', lastname: 'Fogg'}");
            entityUris.Add( "Jon Fogg", 
                CreateEntity(svc, "contacts", contactAttributes) );

            Console.WriteLine("* Finished creating required records");
        }

        /// <summary>
        /// Create a new entity instance.
        /// </summary>
        /// <param name="svc">The service class used for messaging.</param>
        /// <param name="entitySet">The set name (collection) of the entity.</param>
        /// <param name="entityAttributes">The attributes of the entity.</param>
        /// <returns>The entity's web address (URI).</returns>
        private static Uri CreateEntity(CDSWebApiService svc, string entitySet, JObject entityAttributes)
        {
            Uri entityUri;
            string name;

            // Singular entity type fron the plural entity set name 
            string entityType = entitySet.Remove(entitySet.Length - 1);

            switch (entitySet)
            {
                case "contacts":
                    if (entityAttributes.ContainsKey("fullname"))
                        name = (string)entityAttributes["fullname"];
                    else
                        name = (string)entityAttributes["firstname"] + " " + entityAttributes["lastname"];
                    break;

                case "incidents":
                    name = (string)entityAttributes["title"];
                    break;

                default:
                    name = (string)entityAttributes["name"];
                    break;
            }

            try
            {
                Console.Write("\tCreating {0} '{1}'..", entityType, name);
                entityUri = svc.PostCreate(entitySet, entityAttributes);
                Console.WriteLine("completed.");
            }
            catch (ServiceException e)
            {
                Console.WriteLine("failed.");
                throw e;
            }

            return entityUri;
        }
    }
}
