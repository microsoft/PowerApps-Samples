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
                        EntityFilters = EntityFilters.Attributes,
                        // When RetrieveAsIfPublished property is set to false, retrieves only the currently published changes. Default setting of the property is false.
                        // When RetrieveAsIfPublished property is set to true, retrieves the changes that are published and those changes that have not been published.
                        RetrieveAsIfPublished = false

                    };

                    // Retrieve the MetaData.
                    var response = (RetrieveAllEntitiesResponse)service.Execute(request);

                    // Create an instance of StreamWriter to write text to a file.
                    // The using statement also closes the StreamWriter.
                    // To view this file, right click the file and choose open with Excel. 
                    // Excel will figure out the schema and display the information in columns.

                    String filename = String.Concat("AttributePicklistValues.xml");
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
                                if (currentEntity.IsCustomizable.Value == true)
                                    metadataWriter.WriteElementString("IsCustomizable", "yes");
                                else
                                    metadataWriter.WriteElementString("IsCustomizable", "no");

                                #region Attributes

                                // Write Entity's Attributes.
                                metadataWriter.WriteStartElement("Attributes");

                                foreach (AttributeMetadata currentAttribute in currentEntity.Attributes)
                                {
                                    // Only write out main attributes.
                                    if (currentAttribute.AttributeOf == null)
                                    {

                                        // Start Attribute Node
                                        metadataWriter.WriteStartElement("Attribute");

                                        // Write Attribute's information.
                                        metadataWriter.WriteElementString("SchemaName", currentAttribute.SchemaName);
                                        metadataWriter.WriteElementString("Type", currentAttribute.AttributeType.Value.ToString());

                                        if (currentAttribute.GetType() == typeof(PicklistAttributeMetadata))
                                        {
                                            PicklistAttributeMetadata optionMetadata = (PicklistAttributeMetadata)currentAttribute;
                                            // Writes the picklist's options
                                            metadataWriter.WriteStartElement("Options");

                                            // Writes the attributes of each picklist option
                                            for (int c = 0; c < optionMetadata.OptionSet.Options.Count; c++)
                                            {
                                                metadataWriter.WriteStartElement("Option");
                                                metadataWriter.WriteElementString("OptionValue", optionMetadata.OptionSet.Options[c].Value.Value.ToString());
                                                metadataWriter.WriteElementString("OptionDescription", optionMetadata.OptionSet.Options[c].Label.UserLocalizedLabel.Label.ToString());
                                                metadataWriter.WriteEndElement();
                                            }

                                            metadataWriter.WriteEndElement();

                                        }
                                        else if (currentAttribute.GetType() == typeof(StateAttributeMetadata))
                                        {
                                            StateAttributeMetadata optionMetadata = (StateAttributeMetadata)currentAttribute;
                                            // Writes the picklist's options
                                            metadataWriter.WriteStartElement("Options");

                                            // Writes the attributes of each picklist option
                                            for (int c = 0; c < optionMetadata.OptionSet.Options.Count; c++)
                                            {
                                                metadataWriter.WriteStartElement("Option");
                                                metadataWriter.WriteElementString("OptionValue", optionMetadata.OptionSet.Options[c].Value.Value.ToString());
                                                metadataWriter.WriteElementString("OptionDescription", optionMetadata.OptionSet.Options[c].Label.UserLocalizedLabel.Label.ToString());
                                                metadataWriter.WriteEndElement();
                                            }

                                            metadataWriter.WriteEndElement();
                                        }
                                        else if (currentAttribute.GetType() == typeof(StatusAttributeMetadata))
                                        {
                                            StatusAttributeMetadata optionMetadata = (StatusAttributeMetadata)currentAttribute;
                                            // Writes the picklist's options
                                            metadataWriter.WriteStartElement("Options");

                                            // Writes the attributes of each picklist option
                                            for (int c = 0; c < optionMetadata.OptionSet.Options.Count; c++)
                                            {
                                                metadataWriter.WriteStartElement("Option");
                                                metadataWriter.WriteElementString("OptionValue", optionMetadata.OptionSet.Options[c].Value.Value.ToString());
                                                metadataWriter.WriteElementString("OptionDescription", optionMetadata.OptionSet.Options[c].Label.UserLocalizedLabel.Label.ToString());
                                                if (optionMetadata.OptionSet.Options[c] is StatusOptionMetadata)
                                                    metadataWriter.WriteElementString("RelatedToState", ((StatusOptionMetadata)optionMetadata.OptionSet.Options[c]).State.ToString());
                                                metadataWriter.WriteEndElement();
                                            }

                                            metadataWriter.WriteEndElement();
                                        }

                                        // End Attribute Node
                                        metadataWriter.WriteEndElement();
                                    }
                                }
                                // End Attributes Node
                                metadataWriter.WriteEndElement();

                                #endregion

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
