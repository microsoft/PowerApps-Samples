using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to promote an email message to create an email activity.
    /// </summary>
    /// <remarks>
    /// This sample shows how to create an email activity instance from an email message
    /// using the DeliverPromoteEmailRequest message.
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
            // No setup required for this sample
        }

        /// <summary>
        /// Demonstrates promoting an email message
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Creating contact and promoting email...");

            // Create a contact to send an email to (To: field)
            var emailContact = new Entity("contact")
            {
                ["firstname"] = "Lisa",
                ["lastname"] = "Andrews",
                ["emailaddress1"] = "lisa@contoso.com"
            };
            Guid contactId = service.Create(emailContact);
            entityStore.Add(new EntityReference("contact", contactId));
            Console.WriteLine("Created a sample contact.");

            // Get a system user to send the email (From: field)
            var systemUserRequest = new WhoAmIRequest();
            var systemUserResponse = (WhoAmIResponse)service.Execute(systemUserRequest);

            var cols = new ColumnSet("internalemailaddress");
            var emailSender = service.Retrieve("systemuser", systemUserResponse.UserId, cols);
            string fromEmail = emailSender.GetAttributeValue<string>("internalemailaddress");

            // Create the request
            var deliverEmailRequest = new DeliverPromoteEmailRequest
            {
                Subject = "SDK Sample Email",
                To = emailContact.GetAttributeValue<string>("emailaddress1"),
                From = fromEmail,
                Bcc = string.Empty,
                Cc = string.Empty,
                Importance = "high",
                Body = "This message will create an email activity.",
                SubmittedBy = string.Empty,
                ReceivedOn = DateTime.Now
            };

            // We won't attach a file to the email, but the Attachments property is required
            deliverEmailRequest.Attachments = new EntityCollection(new Entity[0]);
            deliverEmailRequest.Attachments.EntityName = "activitymimeattachment";

            // Execute the request
            var deliverEmailResponse = (DeliverPromoteEmailResponse)service.Execute(deliverEmailRequest);

            // Verify the success
            // Define possible values for email status
            int EmailStatusSent = 3;

            // Query for the delivered email, and verify the status code is "Sent"
            var deliveredMailColumns = new ColumnSet("statuscode");
            var deliveredEmail = service.Retrieve("email", deliverEmailResponse.EmailId, deliveredMailColumns);

            entityStore.Add(new EntityReference("email", deliverEmailResponse.EmailId));

            var statusCode = deliveredEmail.GetAttributeValue<OptionSetValue>("statuscode");
            if (statusCode != null && statusCode.Value == EmailStatusSent)
            {
                Console.WriteLine("Successfully created and delivered the e-mail message.");
            }
            else
            {
                Console.WriteLine("Email was created but status is not 'Sent'.");
            }
        }

        /// <summary>
        /// Cleans up sample data created during execution
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine("\nDeleting {0} created record(s)...", entityStore.Count);
                // Delete in reverse order (email first, then contact)
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
