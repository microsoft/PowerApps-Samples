using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using System.Xml.Serialization;

namespace PowerPlatform.Dataverse.CodeSamples
{
    /// <summary>
    /// Demonstrates how to create an email using a template.
    /// </summary>
    /// <remarks>
    /// This sample shows how to instantiate an email record from a template using the
    /// InstantiateTemplateRequest message and serialize the result to XML.
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

            // Create an account
            var account = new Entity("account")
            {
                ["name"] = "Fourth Coffee"
            };
            Guid accountId = service.Create(account);
            entityStore.Add(new EntityReference("account", accountId));
            Console.WriteLine("Created a sample account: Fourth Coffee.");

            // Define the body and subject of the email template in XML format
            string bodyXml =
               "<?xml version=\"1.0\" ?>"
               + "<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">"
               + "<xsl:output method=\"text\" indent=\"no\"/><xsl:template match=\"/data\">"
               + "<![CDATA["
               + "This message is to notify you that a new account has been created."
               + "]]></xsl:template></xsl:stylesheet>";

            string subjectXml =
               "<?xml version=\"1.0\" ?>"
               + "<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">"
               + "<xsl:output method=\"text\" indent=\"no\"/><xsl:template match=\"/data\">"
               + "<![CDATA[New account notification]]></xsl:template></xsl:stylesheet>";

            string presentationXml =
               "<template><text><![CDATA["
               + "This message is to notify you that a new account has been created."
               + "]]></text></template>";

            string subjectPresentationXml =
               "<template><text><![CDATA[New account notification]]></text></template>";

            // Create an e-mail template
            var template = new Entity("template")
            {
                ["title"] = "Sample E-mail Template for Account",
                ["body"] = bodyXml,
                ["subject"] = subjectXml,
                ["presentationxml"] = presentationXml,
                ["subjectpresentationxml"] = subjectPresentationXml,
                ["templatetypecode"] = "account",
                ["languagecode"] = 1033, // For US English
                ["ispersonal"] = false
            };

            Guid templateId = service.Create(template);
            entityStore.Add(new EntityReference("template", templateId));
            Console.WriteLine("Created Sample E-mail Template for Account.");
        }

        /// <summary>
        /// Demonstrates creating an email using a template
        /// </summary>
        private static void Run(ServiceClient service)
        {
            Console.WriteLine("\nCreating email using template...");

            // Get the template and account IDs from entityStore
            var templateRef = entityStore.FirstOrDefault(e => e.LogicalName == "template");
            var accountRef = entityStore.FirstOrDefault(e => e.LogicalName == "account");

            if (templateRef == null || accountRef == null)
            {
                Console.WriteLine("Required records not found.");
                return;
            }

            // Use the InstantiateTemplate message to create an e-mail message using a template
            var instTemplateReq = new InstantiateTemplateRequest
            {
                TemplateId = templateRef.Id,
                ObjectId = accountRef.Id,
                ObjectType = "account"
            };

            var instTemplateResp = (InstantiateTemplateResponse)service.Execute(instTemplateReq);

            // Serialize the email message to XML and save to a file
            var serializer = new XmlSerializer(typeof(InstantiateTemplateResponse));
            string filename = "email-message.xml";
            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, instTemplateResp);
            }

            Console.WriteLine("Created e-mail using the template.");
            Console.WriteLine("Email message serialized to: {0}", filename);
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
