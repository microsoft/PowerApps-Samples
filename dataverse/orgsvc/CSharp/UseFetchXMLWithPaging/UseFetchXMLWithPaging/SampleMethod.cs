using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        // Define the IDs needed for this sample.
        public static Guid parentAccountId;
        public static Guid[] childAccountIds;
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
        /// This method creates any entity records that this sample requires.
        /// Create a parent account record.
        /// Create 10 child accounts to the parent account record.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Instantiate a account entity record and set its property values.
            // See the Entity Metadata topic in the SDK documentation
            // to determine which attributes must be set for each entity.
            // Create the parent account.
            var parentAccount = new Account
            {
                Name = "Root Test Account",
                EMailAddress1 = "root@root.com"
            };


            parentAccountId = service.Create(parentAccount);

            // Create 10 child accounts.
            childAccountIds = new Guid[10];
            int count = 1;
            while (true)
            {
                var childAccount = new Account
                {
                    Name = "Child Test Account " + count.ToString(),
                    EMailAddress1 = "child" + count.ToString() + "@root.com",
                    EMailAddress2 = "same@root.com",
                    ParentAccountId = new EntityReference(Account.EntityLogicalName, parentAccountId)
                };

                childAccountIds[count - 1] = service.Create(childAccount);

                // Jump out of the loop after creating 10 child accounts.
                if (count == 10)
                    break;
                // Increment the count.
                count++;
            }
            return;
        }

        public static string ExtractNodeValue(XmlNode parentNode, string name)
        {
            XmlNode childNode = parentNode.SelectSingleNode(name);

            if (null == childNode)
            {
                return null;
            }
            return childNode.InnerText;
        }

        public string ExtractAttribute(XmlDocument doc, string name)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;
            var attr = (XmlAttribute)attrs.GetNamedItem(name);
            if (null == attr)
            {
                return null;
            }
            return attr.Value;
        }


        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service ,bool prompt)
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
                // Remove the test parent account.
                service.Delete(Account.EntityLogicalName, parentAccountId);

                // Remove 10 test child accounts.
                int deleteCount = 0;
                while (deleteCount < 10)
                {
                    service.Delete(Account.EntityLogicalName, childAccountIds[deleteCount]);
                    ++deleteCount;
                }

                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
