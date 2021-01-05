using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerApps.Samples
{
    public partial class LateBoundQuery
    {
        private static List<Guid> _accountIds = new List<Guid>();
        private static List<Guid> _contactIds = new List<Guid>();
        private static List<Guid> _leadIds = new List<Guid>();

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
                    OrganizationServiceContext orgContext =
                        new OrganizationServiceContext(service);

                    // Retrieve records with Skip/Take record paging. Setting a page size
                    // can help you manage your Skip and Take calls, since Skip must be
                    // passed a multiple of Take's parameter value.
                    //<snippetUseLinqWithDotNetDataServicesDE2>
                    int pageSize = 5;

                    var accountsByPage = from a in orgContext.CreateQuery("account")
                                         select a["name"];
                    System.Console.WriteLine("Skip 10 accounts, then Take 5 accounts");
                    System.Console.WriteLine("======================================");
                    foreach (var name in accountsByPage.Skip(2 * pageSize).Take(pageSize))
                    {
                        System.Console.WriteLine(name);
                    }
                    //</snippetUseLinqWithDotNetDataServicesDE2>
                    System.Console.WriteLine();
                    System.Console.WriteLine("<End of Listing>");
                    System.Console.WriteLine();
                    //OUTPUT:
                    //Skip 10 accounts, then Take 5 accounts
                    //======================================
                    //Fourth Coffee 6
                    //Fourth Coffee 7
                    //Fourth Coffee 8
                    //Fourth Coffee 9
                    //Fourth Coffee 10

                    //<End of Listing>

                    // Use orderBy to order items retrieved.
                    //<snippetUseLinqWithDotNetDataServicesDE3>
                    var orderedAccounts = from a in orgContext.CreateQuery("account")
                                          orderby a["name"]
                                          select a["name"];
                    System.Console.WriteLine("Display accounts ordered by name");
                    System.Console.WriteLine("================================");
                    foreach (var name in orderedAccounts)
                    {
                        System.Console.WriteLine(name);
                    }
                    //</snippetUseLinqWithDotNetDataServicesDE3>
                    System.Console.WriteLine();
                    System.Console.WriteLine("<End of Listing>");
                    System.Console.WriteLine();
                    //OUTPUT:
                    //Display accounts ordered by name
                    //================================
                    //A. Datum Corporation
                    //Adventure Works
                    //Coho Vineyard
                    //Fabrikam
                    //Fourth Coffee 1
                    //Fourth Coffee 10
                    //Fourth Coffee 2
                    //Fourth Coffee 3
                    //Fourth Coffee 4
                    //Fourth Coffee 5
                    //Fourth Coffee 6
                    //Fourth Coffee 7
                    //Fourth Coffee 8
                    //Fourth Coffee 9
                    //Humongous Insurance

                    //<End of Listing>


                    // Filter multiple entities using LINQ.
                    //<snippetUseLinqWithDotNetDataServicesDE4>
                    var query = from c in orgContext.CreateQuery("contact")
                                join a in orgContext.CreateQuery("account")
                                    on c["contactid"] equals a["primarycontactid"]
                                where (String)c["lastname"] == "Wilcox" ||
                                    (String)c["lastname"] == "Andrews"
                                where ((String)a["address1_telephone1"]).Contains("(206)")
                                    || ((String)a["address1_telephone1"]).Contains("(425)")
                                select new
                                {
                                    Contact = new
                                    {
                                        FirstName = c["firstname"],
                                        LastName = c["lastname"]
                                    },
                                    Account = new
                                    {
                                        Address1_Telephone1 = a["address1_telephone1"]
                                    }
                                };

                    Console.WriteLine("Join account and contact");
                    Console.WriteLine("List all records matching specified parameters");
                    Console.WriteLine("Contact name: Wilcox or Andrews");
                    Console.WriteLine("Account area code: 206 or 425");
                    Console.WriteLine("==============================================");
                    foreach (var record in query)
                    {
                        Console.WriteLine("Contact Name: {0} {1}",
                            record.Contact.FirstName, record.Contact.LastName);
                        Console.WriteLine("Account Phone: {0}",
                            record.Account.Address1_Telephone1);
                    }
                    //</snippetUseLinqWithDotNetDataServicesDE4>
                    Console.WriteLine("<End of Listing>");
                    Console.WriteLine();
                    //OUTPUT:
                    //Join account and contact
                    //List all records matching specified parameters
                    //Contact name: Wilcox or Andrews
                    //Account area code: 206 or 425
                    //==============================================
                    //Contact Name: Ben Andrews
                    //Account Phone: (206)555-5555
                    //Contact Name: Ben Andrews
                    //Account Phone: (425)555-5555
                    //Contact Name: Colin Wilcox
                    //Account Phone: (425)555-5555
                    //<End of Listing>

                    // Build a complex query with LINQ. This query includes multiple
                    // JOINs and a complex WHERE statement.
                    //<snippetUseLinqWithDotNetDataServicesDE5>
                    var complexQuery = from c in orgContext.CreateQuery("contact")
                                       join a in orgContext.CreateQuery("account")
                                            on c["contactid"] equals a["primarycontactid"]
                                       join l in orgContext.CreateQuery("lead")
                                            on a["originatingleadid"] equals l["leadid"]
                                       where (String)c["lastname"] == "Wilcox" ||
                                            (String)c["lastname"] == "Andrews"
                                       where ((String)a["address1_telephone1"]).Contains("(206)")
                                           || ((String)a["address1_telephone1"]).Contains("(425)")
                                       select new
                                       {
                                           Contact = new
                                           {
                                               FirstName = c["firstname"],
                                               LastName = c["lastname"]
                                           },
                                           Account = new
                                           {
                                               Address1_Telephone1 = a["address1_telephone1"]
                                           },
                                           Lead = new
                                           {
                                               LeadId = l["leadid"]
                                           }
                                       };

                    Console.WriteLine("Join account, contact and lead");
                    Console.WriteLine("List all records matching specified parameters");
                    Console.WriteLine("Contact name: Wilcox or Andrews");
                    Console.WriteLine("Account area code: 206 or 425");
                    Console.WriteLine("==============================================");
                    foreach (var record in complexQuery)
                    {
                        Console.WriteLine("Lead ID: {0}",
                            record.Lead.LeadId);
                        Console.WriteLine("Contact Name: {0} {1}",
                            record.Contact.FirstName, record.Contact.LastName);
                        Console.WriteLine("Account Phone: {0}",
                            record.Account.Address1_Telephone1);
                    }
                    //</snippetUseLinqWithDotNetDataServicesDE5>
                    Console.WriteLine("<End of Listing>");
                    Console.WriteLine();
                    //OUTPUT:
                    //Join account, contact and lead
                    //List all records matching specified parameters
                    //Contact name: Wilcox or Andrews
                    //Account area code: 206 or 425
                    //==============================================
                    //Lead ID: 78d5df14-64a3-e011-aea3-00155dba3818
                    //Contact Name: Colin Wilcox
                    //Account Phone: (425)555-5555
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
