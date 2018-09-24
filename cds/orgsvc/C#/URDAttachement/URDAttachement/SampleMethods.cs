using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
        private static Guid annotationId;
        private static String fileName;
        private static bool prompt= true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }
        }
            private static void CleanUpSample(CrmServiceClient service)

            {
                DeleteRequiredRecords(service,prompt);
            }
        
            /// <summary>
            /// Deletes any entity records and files that were created for this sample.
            /// <param name="prompt">Indicates whether to prompt the user 
            /// to delete the records created in this sample.</param>
            /// </summary>
            public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
            {
                bool deleteRecords = true;

                if (prompt)
                {
                    Console.WriteLine("\nDo you want these entity records deleted? (y/n) [y]: ");
                    String answer = Console.ReadLine();

                    deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y") || answer == String.Empty);
                }

                if (deleteRecords)
                {
                    service.Delete(Annotation.EntityLogicalName, annotationId);
                    File.Delete(fileName);
                    Console.WriteLine("Entity records have been deleted.");
                }
            }

        }
    }

