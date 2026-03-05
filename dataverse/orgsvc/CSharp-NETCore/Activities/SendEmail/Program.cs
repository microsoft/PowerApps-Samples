using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to send an email using the SendEmailRequest message.
    /// </summary>
    /// <remarks>
    /// This sample shows how to send an email message using the SendEmailRequest message.
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();

        #region Sample Methods

        /// <summary>
        /// Sets up sample data required for the demonstration
        /// </summary>
        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Setting up sample data...");

            // Create a contact to send an email to (To: field)
            var emailContact = new Entity("contact")
            {
                ["firstname"] = "Nancy",
                ["lastname"] = "Anderson",
                ["emailaddress1"] = "nancy@contoso.com"
            };
            Guid contactId = service.Create(emailContact);
            entityStore.Add(new EntityReference("contact", contactId));
            Console.WriteLine("Created a contact: {0} {1}", emailContact["firstname"], emailContact["lastname"]);

            // Get a system user to send the email (From: field)
            var systemUserRequest = new WhoAmIRequest();
            var systemUserResponse = (WhoAmIResponse)service.Execute(systemUserRequest);
            Guid userId = systemUserResponse.UserId;

            // Create the 'From:' activity party for the email
            var fromParty = new Entity("activityparty")
            {
                ["partyid"] = new EntityReference("systemuser", userId)
            };

            // Create the 'To:' activity party for the email
            var toParty = new Entity("activityparty")
            {
                ["partyid"] = new EntityReference("contact", contactId)
            };
            Console.WriteLine("Created activity parties.");

            // Create an e-mail message
            var email = new Entity("email")
            {
                ["to"] = new EntityCollection(new List<Entity> { toParty }),
                ["from"] = new EntityCollection(new List<Entity> { fromParty }),
                ["subject"] = "SDK Sample e-mail",
                ["description"] = "SDK Sample for SendEmail Message.",
                ["directioncode"] = true
            };
            Guid emailId = service.Create(email);
            entityStore.Add(new EntityReference("email", emailId));
            Console.WriteLine("Created {0}.", email["subject"]);
        }

        /// <summary>
        /// Demonstrates how to send an email
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nSending email...");

            // Get the email ID from entityStore
            Guid emailId = entityStore[1].Id;

            // Use the SendEmail message to send an e-mail message
            var sendEmailreq = new SendEmailRequest
            {
                EmailId = emailId,
                TrackingToken = "",
                IssueSend = true
            };

            service.Execute(sendEmailreq);
            Console.WriteLine("Sent the e-mail message.");
        }

        /// <summary>
        /// Cleans up sample data created during execution
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine("\nDeleting {0} created record(s)...", entityStore.Count);
                foreach (var entityRef in entityStore)
                {
                    service.Delete(entityRef.LogicalName, entityRef.Id);
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
