using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerApps.Samples
{
    public partial class RetrieveMultiple
    {
        private static Guid _accountId;
        private static List<Guid> _contactIdList = new List<Guid>();

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
                    // Create the ServiceContext object that will generate
                    // the IQueryable collections for LINQ calls.
                    ServiceContext svcContext =
                        new ServiceContext(service);

                    #region SQL Query Translated to LINQ
                    // Build the following SQL query using QueryExpression:
                    //
                    //		SELECT contact.fullname, contact.address1_telephone1
                    //		FROM contact
                    //			LEFT OUTER JOIN account
                    //				ON contact.parentcustomerid = account.accountid
                    //				AND
                    //				account.name = 'Litware, Inc.'
                    //		WHERE (contact.address1_stateorprovince = 'WA'
                    //		AND
                    //			contact.address1_city in ('Redmond', 'Bellevue', 'Kirkland', 'Seattle')
                    //		AND 
                    //			contact.address1_telephone1 like '(206)%'
                    //			OR
                    //			contact.address1_telephone1 like '(425)%'
                    //		AND
                    //			contact.emailaddress1 Not NULL
                    //			   )
                    var contacts = (from c in svcContext.ContactSet
                                    join a in svcContext.AccountSet on c.ParentCustomerId.Id equals a.AccountId
                                    where (a.Name == "Litware, Inc.")
                                    where (c.Address1_StateOrProvince == "WA"
                                    && (c.Address1_Telephone1.StartsWith("(206)") ||
                                        c.Address1_Telephone1.StartsWith("(425)"))
                                    && (c.Address1_City == "Redmond" ||
                                        c.Address1_City == "Bellevue" ||
                                        c.Address1_City == "Kirkland" ||
                                        c.Address1_City == "Seattle")
                                    && (c.EMailAddress1 != null && c.EMailAddress1 != ""))
                                    select new Contact
                                    {
                                        ContactId = c.ContactId,
                                        FirstName = c.FirstName,
                                        LastName = c.LastName,
                                        Address1_Telephone1 = c.Address1_Telephone1
                                    });

                    // Display the results.
                    Console.WriteLine("List all contacts matching specified parameters");
                    Console.WriteLine("===============================================");
                    foreach (Contact contact in contacts)
                    {
                        Console.WriteLine("Contact ID: {0}", contact.Id);
                        Console.WriteLine("Contact Name: {0} {1}", contact.FirstName, contact.LastName );
                        Console.WriteLine("Contact Phone: {0}", contact.Address1_Telephone1);
                    }
                    Console.WriteLine("<End of Listing>");
                    Console.WriteLine();
                    #endregion

                    //OUTPUT:
                    //List all contacts matching specified parameters
                    //===============================================
                    //Contact ID: a263e139-63a3-e011-aea3-00155dba3818
                    //Contact Name: Ben Andrews
                    //Contact Phone: (206)555-5555
                    //Contact ID: a463e139-63a3-e011-aea3-00155dba3818
                    //Contact Name: Colin Wilcox
                    //Contact Phone: (425)555-5555
                    //<End of Listing>

                    #endregion Demonstrate

                    DeleteRequiredRecords(service, true);
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
