using Microsoft.Crm.Sdk.Messages;
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
                    //////////////////////////////////////////////
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate

                    // Create the request
                    var validateRequest = new ValidateSavedQueryRequest()
                    {
                        FetchXml = savedQuery.FetchXml,
                        QueryType = savedQuery.QueryType.Value
                    };

                    // Send the request
                    Console.WriteLine("  Validating Saved Query");
                    try
                    {
                        // executing the request will throw an exception if the fetch xml is invalid
                        var validateResponse = (ValidateSavedQueryResponse)service.Execute(validateRequest);
                        Console.WriteLine("  Saved Query validated successfully");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("  Invalid Saved Query");
                        throw;
                    }

                    #endregion

                    #region Execute saved query

                    // Create the request
                    var executeSavedQueryRequest = new ExecuteByIdSavedQueryRequest()
                    {
                        EntityId = savedQuery.Id
                    };

                    // Execute the request
                    Console.WriteLine("  Executing Saved Query");
                    var executeSavedQueryResponse =
                        (ExecuteByIdSavedQueryResponse)service.Execute(executeSavedQueryRequest);

                    // Check results
                    if (String.IsNullOrEmpty(executeSavedQueryResponse.String))
                        throw new Exception("Saved Query did not return any results");

                    PrintResults(service,executeSavedQueryResponse.String);
                    #endregion

                    #region Execute user query

                    // Create the request
                    var executeUserQuery = new ExecuteByIdUserQueryRequest()
                    {
                        EntityId = userQuery.ToEntityReference()
                    };

                    // Send the request
                    Console.WriteLine("  Executing User Query");
                    var executeUserQueryResponse =
                        (ExecuteByIdUserQueryResponse)service.Execute(executeUserQuery);
                    if (String.IsNullOrEmpty(executeUserQueryResponse.String))
                        throw new Exception("User Query did not return any results");

                    // validate results
                    PrintResults(service, executeUserQueryResponse.String);

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
