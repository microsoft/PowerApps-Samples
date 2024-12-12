using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;
using System.Xml;

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
                    var request = new RetrieveAllEntitiesRequest()
                    {
                        EntityFilters = EntityFilters.Privileges,
                        // When RetrieveAsIfPublished property is set to false, retrieves only the currently published changes. Defaulted setting is to false.
                        // When RetrieveAsIfPublished property is set to true, retrieves the changes that are published and those changes that have not been published.
                        RetrieveAsIfPublished = false
                    };

                    // Retrieve the MetaData.
                    var response = (RetrieveAllEntitiesResponse)service.Execute(request);


                    // Create an instance of StreamWriter to write text to a file.
                    // The using statement also closes the StreamWriter.
                    // To view this file, right click the file and choose open with Excel. 
                    // Excel will figure out the schema and display the information in columns.

                    String filename = String.Concat("EntityPrivileges.xml");
                    using (var sw = new StreamWriter(filename))
                    {
                        // Create Xml Writer.
                        var metadataWriter = new XmlTextWriter(sw);

                        // Start Xml File.
                        metadataWriter.WriteStartDocument();

                        // Metadata Xml Node.
                        metadataWriter.WriteStartElement("Metadata");

                        foreach (EntityMetadata currentEntity in response.EntityMetadata)
                        {
                            if (currentEntity.IsIntersect.Value == false)
                            {
                                // Start Entity Node
                                metadataWriter.WriteStartElement("Entity");

                                // Write the Entity's Information.
                                metadataWriter.WriteElementString("EntitySchemaName", currentEntity.SchemaName);
                                metadataWriter.WriteElementString("OTC", currentEntity.ObjectTypeCode.ToString());


                                metadataWriter.WriteStartElement("Privileges");

                                foreach (SecurityPrivilegeMetadata privilege in currentEntity.Privileges)
                                {
                                    metadataWriter.WriteStartElement("Privilege");
                                    metadataWriter.WriteElementString("PrivilegeName", privilege.Name.ToString());
                                    metadataWriter.WriteElementString("Id", privilege.PrivilegeId.ToString());
                                    metadataWriter.WriteElementString("Type", privilege.PrivilegeType.ToString());
                                    metadataWriter.WriteElementString("CanBeBasic", privilege.CanBeBasic.ToString());
                                    metadataWriter.WriteElementString("CanBeDeep", privilege.CanBeDeep.ToString());
                                    metadataWriter.WriteElementString("CanBeGlobal", privilege.CanBeGlobal.ToString());
                                    metadataWriter.WriteElementString("CanBeLocal", privilege.CanBeLocal.ToString());

                                    metadataWriter.WriteEndElement();


                                }

                                metadataWriter.WriteEndElement();


                                // End Entity Node
                                metadataWriter.WriteEndElement();
                            }
                        }

                        // End Metadata Xml Node
                        metadataWriter.WriteEndElement();
                        metadataWriter.WriteEndDocument();

                        // Close xml writer.
                        metadataWriter.Close();
                    }

                    Console.WriteLine("Done.");
                    #endregion Demonstrate
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
