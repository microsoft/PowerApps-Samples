using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
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
                    // Query for an an existing report: Account Overview. This is a default report in Microsoft Dynamics CRM.				    
                    QueryByAttribute reportQuery = new QueryByAttribute(Report.EntityLogicalName);
                    reportQuery.AddAttributeValue("name", "Account Overview");
                    reportQuery.ColumnSet = new ColumnSet("reportid");

                    // Get the report.
                    EntityCollection retrieveReports = service.RetrieveMultiple(reportQuery);

                    // Convert retrieved Entity to a report
                    Report retrievedReport = (Report)retrieveReports.Entities[0];
                    Console.WriteLine("Retrieved the 'Account Overview' report.");

                    // Use the Download Report Definition message.
                    var reportHistoryRequest = new GetReportHistoryLimitRequest
                    {
                        ReportId = retrievedReport.ReportId.Value
                    };

                    var reportHistoryResponse = (GetReportHistoryLimitResponse)service.Execute(reportHistoryRequest);

                    // Access the history limit data
                    int historyLimit = reportHistoryResponse.HistoryLimit;

                    Console.WriteLine("The report history limit is {0}.", historyLimit);

                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
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
