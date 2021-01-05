using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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
                    /////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    //  Create query using QueryByAttribute.
                    var querybyattribute = new QueryByAttribute("account");
                    querybyattribute.ColumnSet = new ColumnSet("name", "address1_city", "emailaddress1");

                    //  Attribute to query.
                    querybyattribute.Attributes.AddRange("address1_city");

                    //  Value of queried attribute to return.
                    querybyattribute.Values.AddRange("Redmond");

                    //  Query passed to service proxy.
                    EntityCollection retrieved = service.RetrieveMultiple(querybyattribute);

                    System.Console.WriteLine("Query Using QueryByAttribute");
                    System.Console.WriteLine("===============================");

                    //  Iterate through returned collection.
                    foreach (var c in retrieved.Entities)
                    {
                        System.Console.WriteLine("Name: " + c.Attributes["name"]);

                        if (c.Attributes.Contains("address1_city"))
                            System.Console.WriteLine("Address: " + c.Attributes["address1_city"]);

                        if (c.Attributes.Contains("emailaddress1"))
                            System.Console.WriteLine("E-mail: " + c.Attributes["emailaddress1"]);
                    }
                    System.Console.WriteLine("===============================");

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
