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
                    // Create an EntityReference to represent an open case
                    var caseReference = new EntityReference()
                    {
                        LogicalName = Incident.EntityLogicalName,
                        Id = _caseIncidentId
                    };

                    var checkState =
                        new IsValidStateTransitionRequest();

                    // Set the transition request to an open case
                    checkState.Entity = caseReference;

                    // Check to see if a new state of "resolved" and 
                    // a new status of "problem solved" are valid
                    checkState.NewState = IncidentState.Resolved.ToString();
                    checkState.NewStatus = (int)incident_statuscode.ProblemSolved;

                    // Execute the request
                    var checkStateResponse =
                        (IsValidStateTransitionResponse)service.Execute(checkState);

                    // Handle the response
                    if (checkStateResponse.IsValid)
                    {
                        String changeAnswer = "y"; // default to "y" unless prompting for delete
                        if (prompt)
                        {
                            // The case can be closed
                            Console.WriteLine("Validate State Request returned that the case " +
                                              "can be closed.");
                            Console.Write("\nDo you want to change the record state? " +
                                              "(y/n) [y]: ");
                            changeAnswer = Console.ReadLine();
                            Console.WriteLine();
                        }

                        if (changeAnswer.StartsWith("y") || changeAnswer.StartsWith("Y")
                            || changeAnswer == String.Empty)
                        {
                            // Call function to change the incident to the closed state
                            CloseIncident(service, caseReference);
                            // Re-open the incident and change its state
                            SetState(service, caseReference);
                        }
                    }
                    else
                    {
                        // The case cannot be closed
                        Console.WriteLine("Validate State Request returned that the " +
                                          "change is not valid.");
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
