using Microsoft.Crm.Sdk.Messages;
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
        // Define the IDs needed for this sample.
        private static Guid _contactId;
        private static Guid _appointmentId;
        private static bool prompt = true;
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
                    #region Set up
                    SetUpSample(service);
                    #endregion Set up
                    #region Demonstrate 
                    // Retrieve the appointment to be validated
                    ColumnSet cols = new ColumnSet("scheduledstart", "scheduledend", "statecode", "statuscode");
                    Appointment retrievedAppointment = (Appointment)service.Retrieve(Appointment.EntityLogicalName,
                                                               _appointmentId, cols);

                    // Use the Validate message
                    ValidateRequest validatedReq = new ValidateRequest();
                    validatedReq.Activities = new EntityCollection();
                    validatedReq.Activities.Entities.Add(retrievedAppointment);
                    validatedReq.Activities.MoreRecords = false;
                    validatedReq.Activities.PagingCookie = "";
                    validatedReq.Activities.EntityName = Appointment.EntityLogicalName;

                    ValidateResponse validateResp = (ValidateResponse)service.Execute(validatedReq);

                    // Verify success
                    if ((validateResp.Result != null) && (validateResp.Result.Length > 0))
                    {
                        Console.WriteLine("Validated the appointment.");
                    }

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                #endregion Demonstrate
                #endregion Sample Code
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
