using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to send an email message by using a template.
    /// </summary>
    /// <remarks>
    /// This sample shows how to send an email message by using a template using the
    /// SendEmailFromTemplateRequest message.
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

            // Create a contact record to send an email to (To: field)
            var emailContact = new Entity("contact")
            {
                ["firstname"] = "David",
                ["lastname"] = "Pelton",
                ["emailaddress1"] = "david@contoso.com",
                ["donotemail"] = false
            };
            Guid contactId = service.Create(emailContact);
            entityStore.Add(new EntityReference("contact", contactId));
            Console.WriteLine("Created a sample contact.");
        }

        /// <summary>
        /// Demonstrates how to send an email using a template
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nSending email using template...");

            // Get the contact ID from entityStore
            Guid contactId = entityStore[0].Id;

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
                ["description"] = "SDK Sample for SendEmailFromTemplate Message.",
                ["directioncode"] = true
            };

            // Create a query expression to get one of Email Template of type "contact"
            var queryBuildInTemplates = new QueryExpression
            {
                EntityName = "template",
                ColumnSet = new ColumnSet("templateid", "templatetypecode"),
                Criteria = new FilterExpression()
            };
            queryBuildInTemplates.Criteria.AddCondition("templatetypecode",
                ConditionOperator.Equal, "contact");
            EntityCollection templateEntityCollection = service.RetrieveMultiple(queryBuildInTemplates);

            Guid templateId = Guid.Empty;
            if (templateEntityCollection.Entities.Count > 0)
            {
                templateId = templateEntityCollection.Entities[0].Id;
            }
            else
            {
                throw new ArgumentException("Standard Email Templates are missing");
            }

            // Create the request
            var emailUsingTemplateReq = new SendEmailFromTemplateRequest
            {
                Target = email,

                // Use a built-in Email Template of type "contact"
                TemplateId = templateId,

                // The regarding Id is required, and must be of the same type as the Email Template
                RegardingId = contactId,
                RegardingType = "contact"
            };

            var emailUsingTemplateResp = (SendEmailFromTemplateResponse)service.Execute(emailUsingTemplateReq);

            // Verify that the e-mail has been created
            Guid emailId = emailUsingTemplateResp.Id;
            if (!emailId.Equals(Guid.Empty))
            {
                entityStore.Add(new EntityReference("email", emailId));
                Console.WriteLine("Successfully sent an e-mail message using the template.");
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
