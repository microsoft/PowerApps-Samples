using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to retrieve email attachments associated with an email template.
    /// </summary>
    /// <remarks>
    /// This sample shows how to retrieve email attachments associated with an email template
    /// by using the RetrieveMultiple method.
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

            // Define the email template to create
            var emailTemplate = new Entity("template")
            {
                ["title"] = "An example email template",
                ["subject"] = "This is an example email.",
                ["ispersonal"] = false,
                ["templatetypecode"] = "lead",
                ["languagecode"] = 1033 // US English
            };

            Guid emailTemplateId = service.Create(emailTemplate);
            entityStore.Add(new EntityReference("template", emailTemplateId));

            // Create attachments for the template
            for (int i = 0; i < 3; i++)
            {
                var attachment = new Entity("activitymimeattachment")
                {
                    ["subject"] = $"Attachment {i}",
                    ["filename"] = $"ExampleAttachment{i}.txt",
                    ["body"] = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Some Text")),
                    ["objectid"] = new EntityReference("template", emailTemplateId),
                    ["objecttypecode"] = "template"
                };

                Guid attachmentId = service.Create(attachment);
                entityStore.Add(new EntityReference("activitymimeattachment", attachmentId));
            }

            Console.WriteLine("An email template and {0} attachments were created.", entityStore.Count - 1);
        }

        /// <summary>
        /// Demonstrates how to retrieve email attachments for a template
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nRetrieving email attachments...");

            // Get the template ID from entityStore
            Guid emailTemplateId = entityStore[0].Id;

            // Create a query to retrieve attachments
            var query = new QueryExpression
            {
                EntityName = "activitymimeattachment",
                ColumnSet = new ColumnSet("filename"),

                // Define the conditions for each attachment
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        // The ObjectTypeCode must be specified, or else the query
                        // defaults to "email" instead of "template"
                        new ConditionExpression
                        {
                            AttributeName = "objecttypecode",
                            Operator = ConditionOperator.Equal,
                            Values = { "template" }
                        },
                        // Specify which template we need
                        new ConditionExpression
                        {
                            AttributeName = "objectid",
                            Operator = ConditionOperator.Equal,
                            Values = { emailTemplateId }
                        }
                    }
                }
            };

            // Write out the filename of each attachment retrieved
            EntityCollection attachments = service.RetrieveMultiple(query);
            foreach (var attachment in attachments.Entities)
            {
                Console.WriteLine("Retrieved attachment {0}", attachment["filename"]);
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

                // Delete attachments first
                for (int i = entityStore.Count - 1; i >= 1; i--)
                {
                    service.Delete(entityStore[i].LogicalName, entityStore[i].Id);
                }

                // Delete template last
                service.Delete(entityStore[0].LogicalName, entityStore[0].Id);

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
