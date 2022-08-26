using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        public static Version _productVersion = null;
        // Create storage for new attributes being created
        public static List<AttributeMetadata> addedAttributes;

        // Specify which language code to use in the sample. If you are using a language
        // other than US English, you will need to modify this value accordingly.
        // See http://msdn.microsoft.com/en-us/library/0h88fahh.aspx
        public const int _languageCode = 1033;

        // Define the IDs/variables needed for this sample.
        public static int _insertedStatusValue;
        private static bool prompt = true;
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

            //CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Creates the email activity.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // TODO Create entity records

            Console.WriteLine("Required records have been created.");
        }


        /// <summary>
        /// Deletes the custom entity record that was created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the entity created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                #region How to delete attribute
                // Delete all attributes created for this sample.
                foreach (AttributeMetadata anAttribute in addedAttributes)
                {
                    // Create the request object
                    var deleteAttribute = new DeleteAttributeRequest
                    {
                        // Set the request properties 
                        EntityLogicalName = Contact.EntityLogicalName,
                        LogicalName = anAttribute.LogicalName
                    };
                    // Execute the request
                    service.Execute(deleteAttribute);
                }
                #endregion How to delete attribute

                #region How to remove inserted status value
                // Delete the newly inserted status value.
                // Create the request object
                var deleteRequest = new DeleteOptionValueRequest
                {
                    AttributeLogicalName = "statuscode",
                    EntityLogicalName = Contact.EntityLogicalName,
                    Value = _insertedStatusValue
                };

                // Execute the request
                service.Execute(deleteRequest);

                Console.WriteLine("Deleted all attributes created for this sample.");
                #endregion How to remove inserted status value

                #region Revert the changed state value
                // Revert the state value label from Open to Active.
                // Create the request.
                var revertStateValue = new UpdateStateValueRequest
                {
                    AttributeLogicalName = "statecode",
                    EntityLogicalName = Contact.EntityLogicalName,
                    Value = 1,
                    Label = new Microsoft.Xrm.Sdk.Label("Active", _languageCode)
                };

                // Execute the request.
                service.Execute(revertStateValue);

                // NOTE: All customizations must be published before they can be used.
                service.Execute(new PublishAllXmlRequest());

                Console.WriteLine(
                    "Reverted {0} state attribute of {1} entity from 'Open' to '{2}'.",
                    revertStateValue.AttributeLogicalName,
                    revertStateValue.EntityLogicalName,
                    revertStateValue.Label.LocalizedLabels[0].Label
                    );
                #endregion Revert the changed state value
            

            
            }
        }

    }
}
