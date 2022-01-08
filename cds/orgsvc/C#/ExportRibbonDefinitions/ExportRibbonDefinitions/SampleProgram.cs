using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Required to support the interactive login experience
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    // Create any entity records that the demonstration code requires
                    SetUpSample(service);
                    #region Demonstrate
                    //Create export folder for ribbon xml files if not already exist.
                    if (!Directory.Exists(exportFolder))
                        Directory.CreateDirectory(exportFolder);

                    //Retrieve the Application Ribbon
                    var appribReq = new RetrieveApplicationRibbonRequest();
                    var appribResp = (RetrieveApplicationRibbonResponse)service.Execute(appribReq);

                    System.String applicationRibbonPath = Path.GetFullPath(exportFolder + "\\applicationRibbon.xml");
                    File.WriteAllBytes(applicationRibbonPath, unzipRibbon(appribResp.CompressedApplicationRibbonXml));
                    //Write the path where the file has been saved.
                    Console.WriteLine(applicationRibbonPath);
                    //Retrieve system Entity Ribbons
                    RetrieveEntityRibbonRequest entRibReq = new RetrieveEntityRibbonRequest() { RibbonLocationFilter = RibbonLocationFilters.All };

                    foreach (System.String entityName in entitiesWithRibbons)
                    {
                        entRibReq.EntityName = entityName;
                        RetrieveEntityRibbonResponse entRibResp = (RetrieveEntityRibbonResponse)service.Execute(entRibReq);

                        System.String entityRibbonPath = Path.GetFullPath(exportFolder + "\\" + entityName + "Ribbon.xml");
                        File.WriteAllBytes(entityRibbonPath, unzipRibbon(entRibResp.CompressedEntityXml));
                        //Write the path where the file has been saved.
                        Console.WriteLine(entityRibbonPath);
                    }

                    //Check for custom entities
                    var raer = new RetrieveAllEntitiesRequest() { EntityFilters = EntityFilters.Entity };

                    var resp = (RetrieveAllEntitiesResponse)service.Execute(raer);

                    foreach (EntityMetadata em in resp.EntityMetadata)
                    {
                        if (em.IsCustomEntity == true && em.IsIntersect == false)
                        {
                            entRibReq.EntityName = em.LogicalName;
                            RetrieveEntityRibbonResponse entRibResp = (RetrieveEntityRibbonResponse)service.Execute(entRibReq);

                            System.String entityRibbonPath = Path.GetFullPath(exportFolder + "\\" + em.LogicalName + "Ribbon.xml");
                            File.WriteAllBytes(entityRibbonPath, unzipRibbon(entRibResp.CompressedEntityXml));
                            //Write the path where the file has been saved.
                            Console.WriteLine(entityRibbonPath);
                        }
                    }

                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
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
