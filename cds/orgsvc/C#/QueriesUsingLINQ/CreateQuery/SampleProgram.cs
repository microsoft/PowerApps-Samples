using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerApps.Samples
{
    public partial class CreateALinqQuery
    {
        private static Dictionary<Guid, String> _recordIds = new Dictionary<Guid, String>();

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

                    ServiceContext svcContext = new ServiceContext(service);
                    // Loop through all accounts using the IQueryable interface
                    // on the ServiceContext object
                    //<snippetCreateALinqQuery1>
                    var accounts = from a in svcContext.AccountSet
                                   select new Account
                                   {
                                       Name = a.Name,
                                       Address1_County = a.Address1_County
                                   };
                    System.Console.WriteLine("All accounts");
                    System.Console.WriteLine("========================");
                    foreach (var a in accounts)
                    {
                        System.Console.WriteLine(a.Name + " " + a.Address1_County);
                    }
                    //</snippetCreateALinqQuery1>
                    System.Console.WriteLine();
                    System.Console.WriteLine("<End of Listing>");
                    System.Console.WriteLine();
                    //OUTPUT:
                    //List all accounts in CRM
                    //========================
                    //Fourth Coffee
                    //School of Fine Art Lake County
                    //Tailspin Toys King County
                    //Woodgrove Bank
                    //Contoso, Ltd. Saint Louis County

                    //<End of Listing>
                    System.Console.WriteLine();



                    // Retrieve all accounts owned by the user who has read access rights
                    // to the accounts and where the last name of the user is not Cannon.
                    //<snippetCreateALinqQuery2>
                    var queryAccounts = from a in svcContext.AccountSet
                                        join owner in svcContext.SystemUserSet
                                          on a.OwnerId.Id equals owner.SystemUserId
                                        where owner.LastName != "Cannon"
                                        select new Account
                                        {
                                            Name = a.Name,
                                            Address1_City = a.Address1_City
                                        };
                    System.Console.WriteLine("Accounts not owned by user with last name of 'Cannon'");
                    System.Console.WriteLine("================================================");
                    foreach (var a in queryAccounts)
                    {
                        System.Console.WriteLine(a.Name + " " + a.Address1_County);
                    }
                    //</snippetCreateALinqQuery2>
                    System.Console.WriteLine();
                    System.Console.WriteLine("<End of Listing>");
                    System.Console.WriteLine();
                    //OUTPUT:
                    //Accounts not owned by user w/ last name 'Cannon'
                    //================================================
                    //Fourth Coffee
                    //School of Fine Art
                    //Tailspin Toys
                    //Woodgrove Bank
                    //Contoso, Ltd.

                    //<End of Listing>
                    System.Console.WriteLine();



                    // Return a count of all accounts which have a county specified
                    // in their address.
                    //<snippetCreateALinqQuery3>
                    int accountsWithCounty = (from a in svcContext.AccountSet
                                              where (a.Address1_County != null)
                                              select new Account
                                              {
                                                  Name = a.Name,
                                                  Address1_City = a.Address1_City
                                              }).ToArray().Count();
                    System.Console.Write("Number of accounts with a county specified: ");
                    System.Console.WriteLine(accountsWithCounty);
                    //</snippetCreateALinqQuery3>
                    System.Console.WriteLine();
                    //OUTPUT:
                    //Number of accounts with a county specified: 3
                    System.Console.WriteLine();



                    // Return a count of states in which we have an account. This
                    // uses the 'distinct' keyword which counts a state only one time.
                    //<snippetCreateALinqQuery4>
                    int statesWithAccounts = (from a in svcContext.AccountSet
                                              where (a.Address1_StateOrProvince != null)
                                              select a.Address1_StateOrProvince)
                                              .Distinct().ToArray().Count();
                    System.Console.Write("Number of unique states that contain accounts: ");
                    System.Console.WriteLine(statesWithAccounts);
                    //</snippetCreateALinqQuery4>
                    System.Console.WriteLine();
                    //OUTPUT:
                    //Number of unique states that contain accounts: 3
                    System.Console.WriteLine();



                    // Return contacts where the city equals Redmond AND the first
                    // name is Joe OR John.
                    //<snippetCreateALinqQuery5>
                    var queryContacts = from c in svcContext.ContactSet
                                        where (c.Address1_City == "Redmond") &&
                                              (c.FirstName.Equals("Joe") ||
                                               c.FirstName.Equals("John"))
                                        select new Contact
                                        {
                                            FirstName = c.FirstName,
                                            LastName = c.LastName,
                                            Address1_City = c.Address1_City
                                        };
                    System.Console.WriteLine("Contacts in city of Redmond named Joe OR John");
                    System.Console.WriteLine("=====================================");
                    foreach (var c in queryContacts)
                    {
                        System.Console.WriteLine(c.FirstName + " " +
                            c.LastName + ", " + c.Address1_City);
                    }
                    //</snippetCreateALinqQuery5>
                    System.Console.WriteLine();
                    System.Console.WriteLine("<End of Listing>");
                    System.Console.WriteLine();
                    //OUTPUT:
                    //Contacts in Redmond named Joe OR John
                    //=====================================
                    //Joe  Redmond
                    //John  Redmond
                    //Joe  Redmond

                    //<End of Listing>

                    System.Console.WriteLine();
                    #endregion Demonstrate

                    CleanUpSample(service);
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
