using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using PowerApps.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoalDataForFiscalYear
{
   public partial class SampleProgram
    {
        private static Guid _salesManagerId;
        private static Guid _accountId;
        private static Guid _phoneCallId;
        private static Guid _phoneCall2Id;
        private static Guid _metricId;
        private static Guid _actualId;
        private static Guid _parentGoalId;
        private static Guid _firstChildGoalId;
        private static Guid _secondChildGoalId;
        private static List<Guid> _rollupQueryIds = new List<Guid>();
        private static List<Guid> _salesRepresentativeIds = new List<Guid>();
        private static bool prompt = true;
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
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {

            #region Create or Retrieve the necessary system users

            // Retrieve the ldapPath
            String ldapPath = String.Empty;
            // Retrieve the sales team - 1 sales manager and 2 sales representatives.
            _salesManagerId =
                SystemUserProvider.RetrieveSalesManager(service, ref ldapPath);
            _salesRepresentativeIds =
                SystemUserProvider.RetrieveSalespersons(service, ref ldapPath);

            #endregion

            #region Create PhoneCall record and supporting account

            Account account = new Account
            {
                Name = "Margie's Travel",
                Address1_PostalCode = "99999"
            };
            _accountId = (service.Create(account));
            account.Id = _accountId;

            // Create Guids for PhoneCalls
            _phoneCallId = Guid.NewGuid();
            _phoneCall2Id = Guid.NewGuid();

            // Create ActivityPartys for the phone calls' "From" field.
            ActivityParty activityParty = new ActivityParty()
            {
                PartyId = account.ToEntityReference(),
                ActivityId = new EntityReference
                {
                    Id = _phoneCallId,
                    LogicalName = PhoneCall.EntityLogicalName,
                },
                ParticipationTypeMask = new OptionSetValue(9),
            };

            ActivityParty activityPartyClosed = new ActivityParty()
            {
                PartyId = account.ToEntityReference(),
                ActivityId = new EntityReference
                {
                    Id = _phoneCall2Id,
                    LogicalName = PhoneCall.EntityLogicalName,
                },
                ParticipationTypeMask = new OptionSetValue(9)
            };

            // Create an open phone call.
            PhoneCall phoneCall = new PhoneCall()
            {
                Id = _phoneCallId,
                Subject = "Sample Phone Call",
                DirectionCode = false,
                To = new ActivityParty[] { activityParty },
                OwnerId = new EntityReference("systemuser", _salesRepresentativeIds[0]),
                ActualEnd = DateTime.Now
            };
            service.Create(phoneCall);

            // Close the first phone call.
            SetStateRequest closePhoneCall = new SetStateRequest()
            {
                EntityMoniker = phoneCall.ToEntityReference(),
                State = new OptionSetValue(1),
                Status = new OptionSetValue(4)
            };
            service.Execute(closePhoneCall);

            // Create a second phone call. 
            phoneCall = new PhoneCall()
            {
                Id = _phoneCall2Id,
                Subject = "Sample Phone Call 2",
                DirectionCode = true,
                To = new ActivityParty[] { activityParty },
                OwnerId = new EntityReference("systemuser", _salesRepresentativeIds[1]),
                ActualEnd = DateTime.Now
            };
            service.Create(phoneCall);

            // Close the second phone call.
            closePhoneCall = new SetStateRequest()
            {
                EntityMoniker = phoneCall.ToEntityReference(),
                State = new OptionSetValue(1),
                Status = new OptionSetValue(4)
            };
            service.Execute(closePhoneCall);

            #endregion

        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            // The three system users that were created by this sample will continue to 
            // exist on your system because system users cannot be deleted in Microsoft
            // Dynamics CRM.  They can only be enabled or disabled.

            bool toBeDeleted = true;

            if (prompt)
            {
                // Ask the user if the created entities should be deleted.
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
                if (answer.StartsWith("y") ||
                    answer.StartsWith("Y") ||
                    answer == String.Empty)
                {
                    toBeDeleted = true;
                }
                else
                {
                    toBeDeleted = false;
                }
            }

            if (toBeDeleted)
            {
                // Delete all records created in this sample.
                service.Delete("goal", _firstChildGoalId);
                service.Delete("goal", _secondChildGoalId);
                service.Delete("goal", _parentGoalId);
                service.Delete("goalrollupquery", _rollupQueryIds[0]);
                service.Delete("goalrollupquery", _rollupQueryIds[1]);
                service.Delete("account", _accountId);
                service.Delete("phonecall", _phoneCallId);
                service.Delete("phonecall", _phoneCall2Id);
                service.Delete("rollupfield", _actualId);
                service.Delete("metric", _metricId);

                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }
    }
}
