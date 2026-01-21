using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
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
        // Specify which language code to use in the sample. If you are using a language
        // other than US English, you will need to modify this value accordingly.
        // See https://learn.microsoft.com/previous-versions/windows/embedded/ms912047(v=winembedded.10)
        private const int _languageCode = 1033;

        // Specify the option set's schema name is used in many operations related to
        // global option sets.
        private const string _globalOptionSetName = "sample_exampleoptionset";

        // Define the IDs needed for this sample.
        private static Guid _optionSetId;
        private static int _insertedOptionValue;
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

                // Use the DeleteOptionValueRequest message 
                // to remove the newly inserted label.
                var deleteOptionValueRequest =new DeleteOptionValueRequest
                    {
                        OptionSetName = _globalOptionSetName,
                        Value = _insertedOptionValue
                    };

                // Execute the request.
                service.Execute(deleteOptionValueRequest);
 Console.WriteLine("Option Set option removed.");
                
                // Create the request to see which components have a dependency on the
                // global option set.
                var dependencyRequest =new RetrieveDependentComponentsRequest
                    {
                        ObjectId = _optionSetId,
                        ComponentType = (int)componenttype.OptionSet
                    };

                var dependencyResponse =(RetrieveDependentComponentsResponse)service.Execute(
                    dependencyRequest);

                // Here you would check the dependencyResponse.EntityCollection property
                // and act as appropriate. However, we know there is exactly one 
                // dependency so this example deals with it directly and deletes 
                // the previously created attribute.
                var deleteAttributeRequest =new DeleteAttributeRequest
                    {
                        EntityLogicalName = Contact.EntityLogicalName,
                        LogicalName = "sample_examplepicklist"
                    };

                service.Execute(deleteAttributeRequest);

                Console.WriteLine("Referring attribute deleted.");             

                // Finally, delete the global option set. Attempting this before deleting
                // the picklist above will result in an exception being thrown.
                
                var deleteRequest = new DeleteOptionSetRequest
                {
                    Name = _globalOptionSetName
                };

                service.Execute(deleteRequest);
                Console.WriteLine("Global Option Set deleted");
                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
