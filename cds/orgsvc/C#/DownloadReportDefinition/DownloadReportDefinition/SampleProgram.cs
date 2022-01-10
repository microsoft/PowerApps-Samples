using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;
using System.Xml;

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
                    var reportQuery = new QueryByAttribute(Report.EntityLogicalName);
                    reportQuery.AddAttributeValue("name", "Activities");
                    reportQuery.ColumnSet = new ColumnSet("reportid");

                    // Get the report.
                    EntityCollection retrieveReports = service.RetrieveMultiple(reportQuery);

                    // Convert retrieved Entity to a report
                    var retrievedReport = (Report)retrieveReports.Entities[0];
                    Console.WriteLine("Retrieved the 'Activities' report.");

                    // Use the Download Report Definition message.
                    var rdlRequest = new DownloadReportDefinitionRequest
                    {
                        ReportId = retrievedReport.ReportId.Value
                    };

                    var rdlResponse = (DownloadReportDefinitionResponse)service.Execute(rdlRequest);

                    // Get the current directory path.
                    _currentDirectoryPath = Directory.GetCurrentDirectory();

                    // Access the xml data and save to disk
                    var reportDefinitionFile = new XmlTextWriter(_currentDirectoryPath + "\\NewReport.rdl", System.Text.Encoding.UTF8);
                    reportDefinitionFile.WriteRaw(rdlResponse.BodyText);
                    reportDefinitionFile.Close();

                    if (File.Exists(_currentDirectoryPath + "\\NewReport.rdl"))
                    {
                        Console.WriteLine("Downloaded the report definition (NewReport.rdl) to '{0}'.", _currentDirectoryPath.ToString());
                    }
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
