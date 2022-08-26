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
        // Define the IDs needed for this sample.        
        private static Guid _accountId;
        private static Guid _templateId;
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

            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create an account.
            var account = new Account
            {
                Name = "Fourth Coffee",
            };
            _accountId = service.Create(account);
            Console.WriteLine("Created a sample account: {0}.", account.Name);

            // Define the body and subject of the email template in XML format.
            string bodyXml =
               "<?xml version=\"1.0\" ?>"
               + "<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">"
               + "<xsl:output method=\"text\" indent=\"no\"/><xsl:template match=\"/data\">"
               + "<![CDATA["
               + "This message is to notify you that a new account has been created."
               + "]]></xsl:template></xsl:stylesheet>";

            string subjectXml =
               "<?xml version=\"1.0\" ?>"
               + "<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">"
               + "<xsl:output method=\"text\" indent=\"no\"/><xsl:template match=\"/data\">"
               + "<![CDATA[New account notification]]></xsl:template></xsl:stylesheet>";

            string presentationXml =
               "<template><text><![CDATA["
               + "This message is to notify you that a new account has been created."
               + "]]></text></template>";

            string subjectPresentationXml =
               "<template><text><![CDATA[New account notification]]></text></template>";

            // Create an e-mail template.
            var template = new Template
            {
                Title = "Sample E-mail Template for Account",
                Body = bodyXml,
                Subject = subjectXml,
                PresentationXml = presentationXml,
                SubjectPresentationXml = subjectPresentationXml,
                TemplateTypeCode = Account.EntityLogicalName,
                LanguageCode = 1033, // For US English.
                IsPersonal = false
            };

            _templateId = service.Create(template);
            Console.WriteLine("Created {0}.", template.Title);
        }


        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool toBeDeleted = true;

            if (prompt)
            {
                // Ask the user if the created entities should be deleted.
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
                if (answer.StartsWith("y") ||
                    answer.StartsWith("Y") ||
                    answer == String.Empty)
                {
                    toBeDeleted = true;
                }
                else
                {
                    toBeDeleted = false;
                }
            }

            if (toBeDeleted)
            {
                // Delete all records created in this sample.
                service.Delete(Template.EntityLogicalName, _templateId);
                service.Delete(Account.EntityLogicalName, _accountId); ;

                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }

    }
}
