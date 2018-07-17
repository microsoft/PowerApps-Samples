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

        static String prefix;
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
            DeleteCustomEntity(service, prefix, prompt);
        }

        /// <summary>
        /// Deletes the custom entity that was created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the entity created in this sample.</param>
        /// </summary>
        public static void DeleteCustomEntity(CrmServiceClient service, String prefix, bool prompt)
        {
            bool deleteEntity = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want this custom entity deleted? (y/n)");
                char answer = Convert.ToChar(Console.ReadLine().Substring(0, 1));

                deleteEntity = (answer == 'y' || answer == 'Y');
            }

            if (deleteEntity)
            {
                DeleteEntityRequest request = new DeleteEntityRequest()
                {
                    LogicalName = prefix + "sampleentity",
                };
                service.Execute(request);

                Console.WriteLine("Entity has been deleted.");
            }
        }
    }
}
