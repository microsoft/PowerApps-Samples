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
        private static List<Account> _accounts = new List<Account>();
        private static SavedQuery savedQuery;
        private static UserQuery userQuery;
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
            #region Create Accounts to query over

            Console.WriteLine("  Creating some sample accounts");

            var account = new Account()
            {
                Name = "Coho Vineyard"
            };
            account.Id = service.Create(account);
            _accounts.Add(account);
            Console.WriteLine("    Created Account {0}", account.Name);

            account = new Account()
            {
                Name = "Coho Winery"
            };
            account.Id = service.Create(account);
            _accounts.Add(account);
            Console.WriteLine("    Created Account {0}", account.Name);

            account = new Account()
            {
                Name = "Coho Vineyard &amp; Winery"
            };
            account.Id = service.Create(account);
            _accounts.Add(account);
            Console.WriteLine("    Created Account {0}", account.Name);

            #endregion

            #region Create a Saved Query

            Console.WriteLine("  Creating a Saved Query that retrieves all Account ids");

            savedQuery = new SavedQuery()
            {
                Name = "Fetch all Account ids",
                ReturnedTypeCode = Account.EntityLogicalName,
                FetchXml = @"
                    <fetch mapping='logical'>
                        <entity name='account'>
                            <attribute name='name' />
                        </entity>
                    </fetch>",
                QueryType = 0,

            };
            savedQuery.Id = service.Create(savedQuery);

            #endregion

            #region Create a User Query

            Console.WriteLine(
                "  Creating a User Query that retrieves all Account ids for Accounts with name 'Coho Winery'");

           userQuery = new UserQuery()
            {
                Name = "Fetch Coho Winery",
                ReturnedTypeCode = Account.EntityLogicalName,
                FetchXml = @"
                    <fetch mapping='logical'>
                        <entity name='account'>
                            <attribute name='name' />
                            <filter>
                                <condition attribute='name' operator='eq' value='Coho Winery' />
                            </filter>
                        </entity>
                    </fetch>",
                QueryType = 0
            };
            userQuery.Id = service.Create(userQuery);

            #endregion
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service ,bool prompt)
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
                service.Delete(SavedQuery.EntityLogicalName,
                    savedQuery.Id);

                service.Delete(UserQuery.EntityLogicalName,
                    userQuery.Id);

                foreach (Account a in _accounts)
                    service.Delete(Account.EntityLogicalName, a.Id);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

        private static void PrintResults(CrmServiceClient service ,String response)
        {
            // Using XmlReader to format output
            var output = new StringBuilder();
            using (XmlReader reader = XmlReader.Create(new StringReader(response)))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true
                };
                using (XmlWriter writer = XmlWriter.Create(output, settings))
                {
                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                writer.WriteStartElement(reader.Name);
                                break;
                            case XmlNodeType.Text:
                                writer.WriteString(reader.Value);
                                break;
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                break;
                            case XmlNodeType.Comment:
                                writer.WriteComment(reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                writer.WriteFullEndElement();
                                break;
                        }
                    }
                }
            }

            Console.WriteLine("  Result of query:\r\n {0}", output.ToString());
            Console.WriteLine();
        }

    }
}
