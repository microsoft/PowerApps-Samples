using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
        private const String _customEntityName = "sample_ImageAttributeDemo";

        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)

        {
            
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }

            CreateImageAttributeDemoEntity(service);
        }

        /// <summary>
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateImageAttributeDemoEntity(CrmServiceClient service)
        {
            //Create a Custom entity
            CreateEntityRequest createrequest = new CreateEntityRequest
            {

                //Define the entity
                Entity = new EntityMetadata
                {
                    SchemaName = "sample_ImageAttributeDemo",
                    
                    DisplayName = new Microsoft.Xrm.Sdk.Label("Image Attribute Demo", 1033),
                    DisplayCollectionName = new Microsoft.Xrm.Sdk.Label("Image Attribute Demos", 1033),
                    Description = new Microsoft.Xrm.Sdk.Label("An entity created by an SDK sample to demonstrate how to upload and retrieve entity images.", 1033),
                    OwnershipType = OwnershipTypes.UserOwned,
                    IsActivity = false,

                },

                // Define the primary attribute for the entity
                PrimaryAttribute = new StringAttributeMetadata
                {
                    SchemaName = "sample_Name",
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    MaxLength = 100,
                    FormatName = StringFormatName.Text,
                    DisplayName = new Microsoft.Xrm.Sdk.Label("Name", 1033),
                    Description = new Microsoft.Xrm.Sdk.Label("The primary attribute for the Image Attribute Demo entity.", 1033)
                }

            };
            service.Execute(createrequest);
            Console.WriteLine("The Image Attribute Demo entity has been created.");

            //Create an Image attribute for the custom entity
            // Only one Image attribute can be added to an entity that doesn't already have one.
            CreateAttributeRequest createEntityImageRequest = new CreateAttributeRequest
            {
                EntityName = _customEntityName.ToLower(),
                Attribute = new ImageAttributeMetadata
                {
                    SchemaName = "EntityImage", //The name is always EntityImage
                                                //Required level must be AttributeRequiredLevel.None
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    DisplayName = new Microsoft.Xrm.Sdk.Label("Image", 1033),
                    Description = new Microsoft.Xrm.Sdk.Label("An image to show with this demonstration.", 1033)

                }
            };
            service.Execute(createEntityImageRequest);
            Console.WriteLine("The Image attribute has been created.");

            QueryExpression qe = new QueryExpression("systemform");
            qe.Criteria.AddCondition("type", ConditionOperator.Equal, 2); //main form
            qe.Criteria.AddCondition("objecttypecode", ConditionOperator.Equal, _customEntityName.ToLower());
            qe.ColumnSet.AddColumn("formxml");

            SystemForm ImageAttributeDemoMainForm = (SystemForm)service.RetrieveMultiple(qe).Entities[0];

            XDocument ImageAttributeDemoMainFormXml = XDocument.Parse(ImageAttributeDemoMainForm.FormXml);
            //Set the showImage attribute so the entity image will be displayed
            ImageAttributeDemoMainFormXml.Root.SetAttributeValue("showImage", true);

            //Updating the entity form definition
            ImageAttributeDemoMainForm.FormXml = ImageAttributeDemoMainFormXml.ToString();

            service.Update(ImageAttributeDemoMainForm);
            Console.WriteLine("The Image Attribute Demo main form has been updated to show images.");


            PublishXmlRequest pxReq1 = new PublishXmlRequest { ParameterXml = String.Format(@"
   <importexportxml>
    <entities>
     <entity>{0}</entity>
    </entities>
   </importexportxml>", _customEntityName.ToLower()) };
            service.Execute(pxReq1);

            Console.WriteLine("The Image Attribute Demo entity was published");
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteImageAttributeDemoEntity(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want to delete the entity created for this sample? (y/n) [y]: ");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y") || answer == String.Empty);
            }

            if (deleteRecords)
            {
                DeleteEntityRequest der = new DeleteEntityRequest() { LogicalName = _customEntityName.ToLower() };
                service.Execute(der);
                Console.WriteLine("The Image Attribute Demo entity has been deleted.");
            }
        }
    }

}
