using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    #region Sample Code
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Query using the paging cookie.
                    // Define the paging attributes.
                    // The number of records per page to retrieve.
                    int queryCount = 3;

                    // Initialize the page number.
                    int pageNumber = 1;

                    // Initialize the number of records.
                    int recordCount = 0;

                    // Define the condition expression for retrieving records.

                    var pagecondition = new ConditionExpression();
                    pagecondition.AttributeName = "parentaccountid";
                    pagecondition.Operator = ConditionOperator.Equal;
                    pagecondition.Values.Add(parentAccountId);

                    // Define the order expression to retrieve the records.
                    var order = new OrderExpression();
                    order.AttributeName = "name";
                    order.OrderType = OrderType.Ascending;

                    // Create the query expression and add condition.
                    var pagequery = new QueryExpression();
                    pagequery.EntityName = "account";
                    pagequery.Criteria.AddCondition(pagecondition);
                    pagequery.Orders.Add(order);
                    pagequery.ColumnSet.AddColumns("name", "emailaddress1");

                    // Assign the pageinfo properties to the query expression.
                    pagequery.PageInfo = new PagingInfo();
                    pagequery.PageInfo.Count = queryCount;
                    pagequery.PageInfo.PageNumber = pageNumber;

                    // The current paging cookie. When retrieving the first page, 
                    // pagingCookie should be null.
                    pagequery.PageInfo.PagingCookie = null;
                    Console.WriteLine("Retrieving sample account records in pages...\n");
                    Console.WriteLine("#\tAccount Name\t\tEmail Address");

                    while (true)
                    {
                        // Retrieve the page.
                        EntityCollection results = service.RetrieveMultiple(pagequery);
                        if (results.Entities != null)
                        {
                            // Retrieve all records from the result set.
                            foreach (Account acct in results.Entities)
                            {
                                Console.WriteLine("{0}.\t{1}\t{2}", ++recordCount, acct.Name,
                                                   acct.EMailAddress1);
                            }
                        }

                        // Check for more records, if it returns true.
                        if (results.MoreRecords)
                        {
                            Console.WriteLine("\n****************\nPage number {0}\n****************", pagequery.PageInfo.PageNumber);
                            Console.WriteLine("#\tAccount Name\t\tEmail Address");

                            // Increment the page number to retrieve the next page.
                            pagequery.PageInfo.PageNumber++;

                            // Set the paging cookie to the paging cookie returned from current results.
                            pagequery.PageInfo.PagingCookie = results.PagingCookie;
                        }
                        else
                        {
                            // If no more records are in the result nodes, exit the loop.
                            break;
                        }
                    }


                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                #endregion Demonstrate
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
            #endregion Sample Code
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
