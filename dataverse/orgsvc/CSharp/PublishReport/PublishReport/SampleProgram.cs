using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;

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
                    // Define an anonymous type to define the possible values for
                    // report type.
                    var ReportTypeCode = new
                    {
                        ReportingServicesReport = 1,
                        OtherReport = 2,
                        LinkedReport = 3
                    };

                    // Define an anonymous type to define the possible values for
                    // report category.
                    var ReportCategoryCode = new
                    {
                        SalesReports = 1,
                        ServiceReports = 2,
                        MarketingReports = 3,
                        AdministrativeReports = 4
                    };

                    // Define an anonymous type to define the possible values for
                    // report visibility
                    var ReportVisibilityCode = new
                    {
                        ReportsGrid = 1,
                        Form = 2,
                        Grid = 3,
                    };

                    // Instantiate a report object.
                    // See the Entity Metadata topic in the SDK documentation to determine
                    // which attributes must be set for each entity.

                    var sampleReport = new Report
                    {
                        Name = "Sample Report",
                        BodyText = File.ReadAllText("SampleReport.rdl"),
                        FileName = "SampleReport.rdl",
                        LanguageCode = 1033, // US English
                        ReportTypeCode = new OptionSetValue(ReportTypeCode.ReportingServicesReport)
                    };
                    // Create a report record named Sample Report.
                    _reportId = service.Create(sampleReport);


                    // Set the report category.
                    var sampleReportCategory = new ReportCategory
                    {
                        ReportId = new EntityReference(Report.EntityLogicalName, _reportId),
                        CategoryCode = new OptionSetValue(ReportCategoryCode.AdministrativeReports)
                    };
                    _reportCategoryId = service.Create(sampleReportCategory);

                    // Define which entity this report uses.
                    var reportEntity = new ReportEntity
                    {
                        ReportId = new EntityReference(Report.EntityLogicalName, _reportId),
                        ObjectTypeCode = Account.EntityLogicalName
                    };
                    _reportEntityId = service.Create(reportEntity);


                    // Set the report visibility.
                    var rv = new ReportVisibility
                    {
                        ReportId = new EntityReference(Report.EntityLogicalName, _reportId),
                        VisibilityCode = new OptionSetValue(ReportVisibilityCode.Form)
                    };
                    _reportVisibilityId1 = service.Create(rv);

                    rv = new ReportVisibility
                    {
                        ReportId = new EntityReference(Report.EntityLogicalName, _reportId),
                        VisibilityCode = new OptionSetValue(ReportVisibilityCode.Grid)
                    };
                    _reportVisibilityId2 = service.Create(rv);

                    rv = new ReportVisibility
                    {
                        ReportId = new EntityReference(Report.EntityLogicalName, _reportId),
                        VisibilityCode = new OptionSetValue(ReportVisibilityCode.ReportsGrid)
                    };
                    _reportVisibilityId3 = service.Create(rv);

                    Console.WriteLine("{0} published in Common Data Service.", sampleReport.Name);
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
