using Microsoft.Xrm.Sdk;
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
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }
            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Create a new recurring appointment.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create a contact
            Contact newContact = new Contact
            {
                FirstName = "Lisa",
                LastName = "Andrews",
                EMailAddress1 = "lisa@contoso.com"
            };
            _contactId = service.Create(newContact);
            Console.WriteLine("Created contact: {0}.", newContact.FirstName + " " + newContact.LastName);

            // Create ab activity party
            ActivityParty requiredAttendee = new ActivityParty
            {
                PartyId = new EntityReference(Contact.EntityLogicalName, _contactId)
            };

            // Create an appointment
            Appointment newAppointment = new Appointment
            {
                Subject = "Test Appointment",
                Description = "Test Appointment",
                ScheduledStart = DateTime.Now.AddHours(1),
                ScheduledEnd = DateTime.Now.AddHours(2),
                Location = "Office",
                RequiredAttendees = new ActivityParty[] { requiredAttendee }
            };
            _appointmentId = service.Create(newAppointment);
            Console.WriteLine("Created {0}.", newAppointment.Subject);
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user to delete 
        /// the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records to be deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                service.Delete(Appointment.EntityLogicalName, _appointmentId);
                service.Delete(Contact.EntityLogicalName, _contactId);
                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
