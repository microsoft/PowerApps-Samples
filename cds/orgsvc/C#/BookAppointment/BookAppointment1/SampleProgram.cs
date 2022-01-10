using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        // Define the IDs needed for this sample.
        public static Guid _appointmentId = Guid.Empty;
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

                    // Get the current user information
                    var userRequest = new WhoAmIRequest();
                    var userResponse = (WhoAmIResponse)service.Execute(userRequest);

                    // Create the ActivityParty instance.
                    var party = new ActivityParty
                    {
                        PartyId = new EntityReference(SystemUser.EntityLogicalName, userResponse.UserId)
                    };

                    // Create the appointment instance.
                    var appointment = new Appointment
                    {
                        Subject = "Test Appointment",
                        Description = "Test Appointment created using the BookRequest Message.",
                        ScheduledStart = DateTime.Now.AddHours(1),
                        ScheduledEnd = DateTime.Now.AddHours(2),
                        Location = "Office",
                        RequiredAttendees = new ActivityParty[] { party },
                        Organizer = new ActivityParty[] { party }
                    };

                    // Use the Book request message.
                    var book = new BookRequest
                    {
                        Target = appointment
                    };
                    var booked = (BookResponse)service.Execute(book);
                    _appointmentId = booked.ValidationResult.ActivityId;

                    // Verify that the appointment has been scheduled.
                    if (_appointmentId != Guid.Empty)
                    {
                        Console.WriteLine("Succesfully booked {0}.", appointment.Subject);
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
    