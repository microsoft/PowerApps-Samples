using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisioApi = Microsoft.Office.Interop.Visio;

namespace PowerApps.Samples
{
  public partial class DiagramBuilder
    {
       

        [STAThread] // Added to support UX
       public static void Main(string[] args)
        {
            CrmServiceClient service = null;
            String filename = String.Empty;
            VisioApi.Application application;
            VisioApi.Document document;
            DiagramBuilder builder = new DiagramBuilder(service);

            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    #region Sample Code
                    ////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate
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
#endregion Demonstrate
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
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
#endregion SampleCode