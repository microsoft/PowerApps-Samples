using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.ServiceModel;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        // Define the IDs needed for this sample.
        private static Guid _accountId;
        private static Guid _myUserId;
        private static Guid _otherUserId;

        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("9.0.0.0")))
            {
                //The environment version is lower than version 9.0.0.0
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
        /// Creates the email activity.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            var userRequest = new WhoAmIRequest();
            WhoAmIResponse user = (WhoAmIResponse)service.Execute(userRequest);

            // Current user.
            _myUserId = user.UserId;

            // Query to retrieve other users.
            var querySystemUser = new QueryExpression
            {
                EntityName = SystemUser.EntityLogicalName,
                ColumnSet = new ColumnSet(new String[] { "systemuserid", "fullname" }),
                Criteria = new FilterExpression()
            };

            querySystemUser.Criteria.AddCondition("businessunitid",
                ConditionOperator.Equal, user.BusinessUnitId);
            querySystemUser.Criteria.AddCondition("systemuserid",
                ConditionOperator.NotEqual, _myUserId);
            // Excluding SYSTEM user.
            querySystemUser.Criteria.AddCondition("lastname",
                ConditionOperator.NotEqual, "SYSTEM");
            // Excluding INTEGRATION user.
            querySystemUser.Criteria.AddCondition("lastname",
                ConditionOperator.NotEqual, "INTEGRATION");

            DataCollection<Entity> otherUsers = service.RetrieveMultiple(
                querySystemUser).Entities;

            int count = service.RetrieveMultiple(querySystemUser).Entities.Count;
            if (count > 0)
            {
                _otherUserId = (Guid)otherUsers[count - 1].Attributes["systemuserid"];

                Console.WriteLine("Retrieved new owner {0} for assignment.",
                    otherUsers[count - 1].Attributes["fullname"]);
            }
            else
            {
                throw new FaultException(
                    "No other user found in the current business unit for assignment.");
            }

            // Create an Account record 
            Account newAccount = new Account
            {
                Name = "Example Account"
            };

            _accountId = service.Create(newAccount);
            Console.WriteLine("Created {0}", newAccount.Name);

            return;
        }


        /// <summary>
        /// Deletes the custom entity record that was created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the entity created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                service.Delete(Account.EntityLogicalName, _accountId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
