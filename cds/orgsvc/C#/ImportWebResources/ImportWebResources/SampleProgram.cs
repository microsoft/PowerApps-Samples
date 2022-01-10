using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using System.Xml.Linq;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    #region Sample Code
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    //Read the descriptive data from the XML file
                    XDocument xmlDoc = XDocument.Load("../../ImportJob.xml");

                    //Create a collection of anonymous type references to each of the Web Resources
                    var webResources = from webResource in xmlDoc.Descendants("webResource")
                                       select new
                                       {
                                           path = webResource.Element("path").Value,
                                           displayName = webResource.Element("displayName").Value,
                                           description = webResource.Element("description").Value,
                                           name = webResource.Element("name").Value,
                                           type = webResource.Element("type").Value
                                       };

                    // Loop through the collection creating Web Resources
                    int counter = 0;
                    foreach (var webResource in webResources)
                    {
                        //<snippetImportWebResources2>
                        //Set the Web Resource properties
                        WebResource wr = new WebResource
                        {
                            Content = getEncodedFileContents(@"../../" + webResource.path),
                            DisplayName = webResource.displayName,
                            Description = webResource.description,
                            Name = _customizationPrefix + webResource.name,
                            LogicalName = WebResource.EntityLogicalName,
                            WebResourceType = new OptionSetValue(Int32.Parse(webResource.type))
                        };

                        // Using CreateRequest because we want to add an optional parameter
                        var cr = new CreateRequest
                        {
                            Target = wr
                        };
                        //Set the SolutionUniqueName optional parameter so the Web Resources will be
                        // created in the context of a specific solution.
                        cr.Parameters.Add("SolutionUniqueName", _ImportWebResourcesSolutionUniqueName);

                        CreateResponse cresp = (CreateResponse)service.Execute(cr);
                        //</snippetImportWebResources2>
                        // Capture the id values for the Web Resources so the sample can delete them.
                        _webResourceIds[counter] = cresp.id;
                        counter++;
                        Console.WriteLine("Created Web Resource: {0}", webResource.displayName);
                    }

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
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
            #endregion Sample Code
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

