using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections;
using System.IO;
using VisioApi = Microsoft.Office.Interop.Visio;

namespace PowerApps.Samples
{
    public partial class DiagramBuilder
    {
        /// <summary>
        /// Application configuration settings.
        /// </summary>
        static IConfiguration Configuration { get; }

        static DiagramBuilder()
        {
            // Get the path to the appsettings file. If the environment variable is set,
            // use that file path. Otherwise, use the runtime folder's settings file.
            string path = Environment.GetEnvironmentVariable("DATAVERSE_APPSETTINGS");
            if (path == null) path = "appsettings.json";

            // Load the app's configuration settings from the JSON file.
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build();
        }

       [STAThread]
       public static void Main(string[] args)
        {
            ServiceClient service = null;
            String filename = String.Empty;
            VisioApi.Application application;
            VisioApi.Document document;
            DiagramBuilder builder = new DiagramBuilder(service);

            try
            {
                // Create a Dataverse service client using the default connection string.
                service = new ServiceClient(Configuration.GetConnectionString("default"));

                if (service.IsReady)
                {
                    // Load Visio and create a new document.
                    application = new VisioApi.Application();
                    application.Visible = false; // Not showing the UI increases rendering speed  
                    builder.VersionName = application.Version;
                    document = application.Documents.Add(String.Empty);
                    builder._application = application;
                    builder._document = document;

                    // Load the metadata.
                    Console.WriteLine("Loading Metadata...");
                    RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest()
                    {
                        EntityFilters = EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships,
                        RetrieveAsIfPublished = false,

                    };
                    RetrieveAllEntitiesResponse response = (RetrieveAllEntitiesResponse)service.Execute(request);
                    builder._metadataResponse = response;

                    // Diagram all entities if given no command-line parameters, otherwise diagram
                    // those entered as command-line parameters.
                    if (args.Length < 1)
                    {
                        ArrayList entities = new ArrayList();

                        foreach (EntityMetadata entity in response?.EntityMetadata)
                        {
                            // Only draw an entity if it does not exist in the excluded entity table.
                            if (!_excludedEntityTable.ContainsKey(entity.LogicalName.GetHashCode()))
                            {
                                entities.Add(entity.LogicalName);
                            }
                            else
                            {
                                Console.WriteLine("Excluding entity: {0}", entity.LogicalName);
                            }
                        }

                        builder.BuildDiagram(service, (string[])entities.ToArray(typeof(string)), "All Entities");
                        filename = "AllEntities.vsd";
                    }
                    else
                    {
                        builder.BuildDiagram(service, args, String.Join(", ", args));
                        filename = String.Concat(args[0], ".vsd");
                    }

                    // Save the diagram in the current directory using the name of the first
                    // entity argument or "AllEntities" if none were given. Close the Visio application. 
                    document.SaveAs(Directory.GetCurrentDirectory() + "\\" + filename);
                    application.Quit();
                }
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastError);
                    }
                    else
                    {
                        throw service.LastException;
                    }
                }
            }
             catch (Exception ex) 
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
    }
            
 }