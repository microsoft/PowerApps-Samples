using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to send bulk email and monitor the async operation results.
    /// </summary>
    /// <remarks>
    /// This sample shows how to send bulk email using the SendBulkMailRequest message
    /// and monitor the results by retrieving records from the asyncoperation table.
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static readonly List<EntityReference> entityStore = new();
        private const int ARBITRARY_MAX_POLLING_TIME = 60;

        #region Sample Methods

        /// <summary>
        /// Sets up sample data required for the demonstration
        /// </summary>
        private static void Setup(ServiceClient service)
        {
            Console.WriteLine("Creating contact records...");

            var emailContact1 = new Entity("contact")
            {
                ["firstname"] = "Adam",
                ["lastname"] = "Carter",
                ["emailaddress1"] = "someone@example.com"
            };

            // Create the first contact
            Guid contact1Id = service.Create(emailContact1);
            entityStore.Add(new EntityReference("contact", contact1Id));
            Console.WriteLine("Contact 1 created.");

            var emailContact2 = new Entity("contact")
            {
                ["firstname"] = "Adina",
                ["lastname"] = "Hagege",
                ["emailaddress1"] = "someone@example.com"
            };

            // Create the second contact
            Guid contact2Id = service.Create(emailContact2);
            entityStore.Add(new EntityReference("contact", contact2Id));
            Console.WriteLine("Contact 2 created.");
        }

        /// <summary>
        /// Demonstrates how to send bulk email and monitor the async operation
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nCreating and sending bulk email...");

            // Get a system user to use as the sender
            var emailSenderRequest = new WhoAmIRequest();
            var emailSenderResponse =
                service.Execute(emailSenderRequest) as WhoAmIResponse;

            // Set tracking ID for bulk mail request
            Guid trackingId = Guid.NewGuid();

            // Get the contact IDs from entityStore
            var contactIds = entityStore.Select(e => e.Id).ToList();

            var bulkMailRequest = new SendBulkMailRequest()
            {
                // Create a query expression for the bulk operation to retrieve
                // the contacts in the email list
                Query = new QueryExpression()
                {
                    EntityName = "contact",
                    ColumnSet = new ColumnSet(new string[] { "contactid" }),
                    Criteria = new FilterExpression()
                    {
                        Conditions =
                        {
                            new ConditionExpression("contactid", ConditionOperator.In, contactIds.ToArray())
                        }
                    }
                },
                // Set the Sender
                Sender = new EntityReference("systemuser", emailSenderResponse.UserId),
                // Use a built-in Microsoft Dynamics CRM email template
                // NOTE: The email template's "template type" must match the type of
                // customers in the email list. Our list contains contacts, so our
                // template must be for contacts.
                TemplateId = new Guid("07B94C1D-C85F-492F-B120-F0A743C540E6"),
                RequestId = trackingId
            };

            // Execute the async bulk email request
            var resp = (SendBulkMailResponse)
                service.Execute(bulkMailRequest);

            Console.WriteLine("Sent bulk email.");

            // Monitor the SendBulkEmail operation
            Console.WriteLine("\nStarting monitoring process...");

            // Retrieve the bulk email async operation using our tracking ID
            var bulkQuery = new QueryByAttribute()
            {
                EntityName = "asyncoperation",
                ColumnSet = new ColumnSet(new string[] { "requestid", "statecode" }),
                Attributes = { "requestid" },
                Values = { trackingId }
            };

            // Retrieve the bulk email async operation
            EntityCollection aResponse = service.RetrieveMultiple(bulkQuery);

            Console.WriteLine("Retrieved bulk email async operation.");

            // Monitor the async operation via polling
            int secondsTicker = ARBITRARY_MAX_POLLING_TIME;
            Entity createdBulkMailOperation = null;

            Console.WriteLine("Checking operation's state for {0} seconds.\n", ARBITRARY_MAX_POLLING_TIME);

            while (secondsTicker > 0)
            {
                // Make sure the async operation was retrieved
                if (aResponse.Entities.Count > 0)
                {
                    // Grab the one bulk operation that has been created
                    createdBulkMailOperation = aResponse.Entities[0];

                    // Check the operation's state
                    var stateCode = createdBulkMailOperation.GetAttributeValue<OptionSetValue>("statecode");
                    if (stateCode.Value != 3) // 3 = Completed
                    {
                        // The operation has not yet completed
                        // Wait a second for the status to change
                        System.Threading.Thread.Sleep(1000);
                        secondsTicker--;

                        // Retrieve a fresh version of the bulk email operation
                        aResponse = service.RetrieveMultiple(bulkQuery);
                    }
                    else
                    {
                        // Stop polling because the operation's state is now complete
                        secondsTicker = 0;
                    }
                }
                else
                {
                    // Wait a second for the async operation to activate
                    System.Threading.Thread.Sleep(1000);
                    secondsTicker--;

                    // Retrieve the entity again
                    aResponse = service.RetrieveMultiple(bulkQuery);
                }
            }

            // Check success
            // Validate async operation succeeded
            if (createdBulkMailOperation != null)
            {
                var stateCode = createdBulkMailOperation.GetAttributeValue<OptionSetValue>("statecode");
                if (stateCode.Value == 3) // 3 = Completed
                {
                    Console.WriteLine("Operation completed successfully.");
                    Console.WriteLine("\nWhen the bulk email operation has completed, all sent emails will");
                    Console.WriteLine("have a status of 'Pending Send' and will be picked up by your email router.");
                }
                else
                {
                    Console.WriteLine("Operation not completed yet.");
                }
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
