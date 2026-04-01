using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Text;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to create, retrieve, update, and delete email attachments.
    /// </summary>
    /// <remarks>
    /// This sample shows how to perform CRUD operations on email attachments
    /// (activitymimeattachment) associated with an email activity.
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

            // Create an email activity
            var email = new Entity("email")
            {
                ["subject"] = "This is an example email",
                ["activityid"] = Guid.NewGuid()
            };

            Guid emailId = service.Create(email);
            entityStore.Add(new EntityReference("email", emailId));
            Console.WriteLine("An e-mail activity is created.");
        }

        /// <summary>
        /// Demonstrates CRUD operations on email attachments
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nPerforming CRUD operations on email attachments...");

            // Get the email ID from entityStore
            var emailRef = entityStore.FirstOrDefault(e => e.LogicalName == "email");
            if (emailRef == null)
            {
                Console.WriteLine("No email found.");
                return;
            }

            // Create three e-mail attachments
            var attachmentIds = new Guid[3];
            for (int i = 0; i < 3; i++)
            {
                var sampleAttachment = new Entity("activitymimeattachment")
                {
                    ["objectid"] = emailRef,
                    ["objecttypecode"] = "email",
                    ["subject"] = String.Format("Sample Attachment {0}", i),
                    ["body"] = Convert.ToBase64String(
                            new ASCIIEncoding().GetBytes("Example Attachment")),
                    ["filename"] = String.Format("ExampleAttachment{0}.txt", i)
                };

                attachmentIds[i] = service.Create(sampleAttachment);
                entityStore.Add(new EntityReference("activitymimeattachment", attachmentIds[i]));
            }

            Console.WriteLine("Created three e-mail attachments for the e-mail activity.");

            // Retrieve an attachment including its id, subject, filename and body
            var singleAttachment = service.Retrieve(
                "activitymimeattachment",
                attachmentIds[0],
                new ColumnSet("activitymimeattachmentid", "subject", "filename", "body"));

            Console.WriteLine("Retrieved an email attachment, {0}.",
                singleAttachment.GetAttributeValue<string>("filename"));

            // Update the attachment
            singleAttachment["filename"] = "ExampleAttachmentUpdated.txt";
            service.Update(singleAttachment);

            Console.WriteLine("Updated the retrieved e-mail attachment to {0}.",
                singleAttachment["filename"]);

            // Retrieve all attachments associated with the email activity
            var attachmentQuery = new QueryExpression
            {
                EntityName = "activitymimeattachment",
                ColumnSet = new ColumnSet("activitymimeattachmentid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "objectid",
                            Operator = ConditionOperator.Equal,
                            Values = { emailRef.Id }
                        },
                        new ConditionExpression
                        {
                            AttributeName = "objecttypecode",
                            Operator = ConditionOperator.Equal,
                            Values = { "email" }
                        }
                    }
                }
            };

            EntityCollection results = service.RetrieveMultiple(attachmentQuery);

            Console.WriteLine("Retrieved all the e-mail attachments. Total count: {0}", results.Entities.Count);
        }

        /// <summary>
        /// Cleans up sample data created during execution
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            if (deleteCreatedRecords && entityStore.Count > 0)
            {
                Console.WriteLine("\nDeleting {0} created record(s)...", entityStore.Count);

                // Delete in reverse order to delete attachments before email
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
