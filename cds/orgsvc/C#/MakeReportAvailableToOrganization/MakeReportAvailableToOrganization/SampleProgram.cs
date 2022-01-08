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
                    // Retrieve existing personal report.
                    var existingPersonalReport =
                        (Report)service.Retrieve(Report.EntityLogicalName,
                        _reportId,
                        new ColumnSet("ispersonal"));

                    // Set IsPersonal property to false. 
                    existingPersonalReport.IsPersonal = false;

                    // Make the report available to the organization.
                    service.Update(existingPersonalReport);

                    // Retrieve the report and verify that the report is available to the organization
                    ColumnSet Cols1 = new ColumnSet("ispersonal");
                    var retrieveAvailableReport =
                        (Report)service.Retrieve(Report.EntityLogicalName,
                        _reportId, Cols1);
                    if (retrieveAvailableReport.IsPersonal.Value == false)
                    {
                        Console.WriteLine("Report is available to the organization.");
                    }

                    // Now make the retrieved report unavailable to the organization
                    retrieveAvailableReport.IsPersonal = true;
                    service.Update(retrieveAvailableReport);

                    // Retrieve the report, and verify that the report is unavailable to the organization
                    ColumnSet Cols2 = new ColumnSet("ispersonal");
                    var retrieveUnavailableReport =
                        (Report)service.Retrieve(Report.EntityLogicalName,
                        _reportId, Cols2);
                    if (retrieveUnavailableReport.IsPersonal.Value == true)
                    {
                        Console.WriteLine("Report is unavailable to the organization.");
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
