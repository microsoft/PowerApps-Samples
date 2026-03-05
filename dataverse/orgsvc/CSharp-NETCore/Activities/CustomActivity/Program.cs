using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to create a custom activity entity.
    /// </summary>
    /// <remarks>
    /// This sample shows how to create a custom activity entity using CreateEntityRequest
    /// and add custom attributes using CreateAttributeRequest.
    ///
    /// Set the appropriate Url and Username values for your test
    /// environment in the appsettings.json file before running this program.
    /// </remarks>
    class Program
    {
        private static string customEntityName = string.Empty;

        #region Sample Methods

        /// <summary>
        /// Sets up sample data required for the demonstration
        /// </summary>
        private static void Setup(ServiceClient service)
        {
            // No setup required for this sample
        }

        /// <summary>
        /// Demonstrates creating a custom activity entity
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("Creating custom activity entity...");

            // The custom prefix would typically be passed in as an argument or
            // determined by the publisher of the custom solution.
            string prefix = "new_";
            customEntityName = prefix + "sampleentity";

            // Create the custom activity entity
            var request = new CreateEntityRequest
            {
                HasNotes = true,
                HasActivities = false,
                PrimaryAttribute = new StringAttributeMetadata
                {
                    SchemaName = "Subject",
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    MaxLength = 100,
                    DisplayName = new Label("Subject", 1033)
                },
                Entity = new EntityMetadata
                {
                    IsActivity = true,
                    SchemaName = customEntityName,
                    DisplayName = new Label("Sample Entity", 1033),
                    DisplayCollectionName = new Label("Sample Entity", 1033),
                    OwnershipType = OwnershipTypes.UserOwned,
                    IsAvailableOffline = true,
                }
            };

            service.Execute(request);
            Console.WriteLine("Created custom activity entity: {0}", customEntityName);

            // Add custom attributes to the custom activity entity
            Console.WriteLine("\nAdding custom attributes...");

            // Add FontFamily attribute
            var fontFamilyAttributeRequest = new CreateAttributeRequest
            {
                EntityName = customEntityName,
                Attribute = new StringAttributeMetadata
                {
                    SchemaName = prefix + "fontfamily",
                    DisplayName = new Label("Font Family", 1033),
                    MaxLength = 100
                }
            };
            service.Execute(fontFamilyAttributeRequest);
            Console.WriteLine("Added FontFamily attribute.");

            // Add FontColor attribute
            var fontColorAttributeRequest = new CreateAttributeRequest
            {
                EntityName = customEntityName,
                Attribute = new StringAttributeMetadata
                {
                    SchemaName = prefix + "fontcolor",
                    DisplayName = new Label("Font Color", 1033),
                    MaxLength = 50
                }
            };
            service.Execute(fontColorAttributeRequest);
            Console.WriteLine("Added FontColor attribute.");

            // Add FontSize attribute
            var fontSizeAttributeRequest = new CreateAttributeRequest
            {
                EntityName = customEntityName,
                Attribute = new IntegerAttributeMetadata
                {
                    SchemaName = prefix + "fontSize",
                    DisplayName = new Label("Font Size", 1033)
                }
            };
            service.Execute(fontSizeAttributeRequest);
            Console.WriteLine("Added FontSize attribute.");

            Console.WriteLine("\nThe custom activity has been created.");
        }

        /// <summary>
        /// Cleans up sample data created during execution
        /// </summary>
        private static void Cleanup(ServiceClient service, bool deleteCreatedRecords)
        {
            if (deleteCreatedRecords && !string.IsNullOrEmpty(customEntityName))
            {
                Console.WriteLine("\nDeleting custom entity...");
                var deleteRequest = new DeleteEntityRequest
                {
                    LogicalName = customEntityName
                };
                service.Execute(deleteRequest);
                Console.WriteLine("Entity has been deleted.");
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
