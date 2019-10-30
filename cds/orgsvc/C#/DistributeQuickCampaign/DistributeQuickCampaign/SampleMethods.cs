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
        private static Guid[] _accountIdArray;
        static Email _templateEmailActivity;
        static Letter _templateLetterActivity;
        private static Guid _newListId;
        private static Guid _currentUser;
        private static Guid _qcBOId;
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
        /// This method creates a Quick Campaign for a set of accounts selected by a query
        /// </summary>
        /// <param name="activityEntity">
        /// An object that indicates activity type for the quick campaign and 
        /// contains values for each of activity that will be created
        /// </param>
        /// <param name="query">
        /// A query that provides a list of accounts for which 
        /// the quick campaign is distributed.
        /// </param>
        /// <param name="ownershipOption">
        /// Specifies who will own the activities created by the Quick Campaign
        ///	The PropagationOwnershipOptions enum is used to specify value for this parameter
        ///	</param>
        /// <param name="isPropagate">
        /// Specifies whether the operation is to be executed. 
        /// This input is often 'true' for Quick Campaign
        /// </param>        
        /// <returns></returns>
        public static Guid CreateAndRetrieveQuickCampaignForQueryExpression(CrmServiceClient service, Entity emailActivityEntity,
            QueryExpression query, PropagationOwnershipOptions ownershipOption, bool isPropagate)
        {

            // create the bulkoperation
            var request = new PropagateByExpressionRequest()
            {
                Activity = emailActivityEntity,
                ExecuteImmediately = false, // Default value.
                FriendlyName = "Query Based Quick Campaign",
                OwnershipOptions = ownershipOption,
                QueryExpression = query,
                Owner = new EntityReference("systemuser", _currentUser),
                PostWorkflowEvent = true,
                SendEmail = false,
                TemplateId = Guid.Empty
            };

            var response =
                (PropagateByExpressionResponse)service.Execute(request);

            Guid bulkOpId = response.BulkOperationId;
            System.Console.WriteLine(
                "Quick Campaign with following name has been created. "
                + "Please verify manually: \n"
                + request.FriendlyName + "\nPress enter to continue....");
            System.Console.ReadLine();
            return bulkOpId;

        }

        /// <summary>
        /// This method creates Quick Campaign for a given Marketing List and retruns the 
        /// Guid of the Quich Campaign which is modelled as bulk operation in CRM
        /// </summary>
        /// <param name="activityEntity">
        /// An object that indicates activity type for the quick campaign and 
        /// contains values for each activity that will be created
        /// </param>
        /// <param name="marketingListId">
        /// The ID of the marketing list to which quick campaign is distributed
        /// </param>
        /// <param name="ownershipOption">
        /// Specifies who will own the activities created by the Quick Campaign
        ///	The PropagationOwnershipOptions enum is used to specify value for this parameter
        ///	</param>
        /// <param name="isPropagate">
        /// Specifies whether the operation is to be executed. 
        /// This input is often 'true' for Quick Campaign
        /// </param>				
        public static Guid CreateAndRetrieveQuickCampaignForMarketingList(CrmServiceClient service,
            Entity letterActivityEntity,
            Guid marketingListId,
            PropagationOwnershipOptions ownershipOption,
            bool isPropagate)
        {
            //Create the request object from input parameters
            var request = new CreateActivitiesListRequest()
            {
                Activity = letterActivityEntity,
                ListId = marketingListId,
                OwnershipOptions = ownershipOption,
                Propagate = isPropagate,
                TemplateId = Guid.Empty,
                FriendlyName = "Quick Campaign for My List",
                Owner = new EntityReference("systemuser", _currentUser),
                PostWorkflowEvent = true
            };

            //Execute the request
            var response =
                (CreateActivitiesListResponse)service.Execute(request);

            //On executing the request a BulkOperation record would be created in CRM. 
            //If isPropagate is true, a corresponding Async job is also created which runs and creates the required activities 
            //The response has BulkOperationId. This is the Id of the bulkoperation that mimics QuickCampaign in CRM
            Guid BOId = response.BulkOperationId;
            System.Console.WriteLine(
                "Quick Campaign with following name has been created. "
                + "Please verify manually: \n"
                + request.FriendlyName
                + "\nPress enter to continue....");
            System.Console.ReadLine();

            return BOId;

        }

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Create a new queue instance.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            var whoRequest = new WhoAmIRequest();
            _currentUser = ((WhoAmIResponse)service.Execute(whoRequest)).UserId;

            //Create an activity objects which will act like a template during QC distrbution. 
            //The activities created by QC will create activities with content that this activity has
            _templateEmailActivity = new Email()
            {
                Subject = "qcCreatedEmailActivity"
            };

            _templateLetterActivity = new Letter()
            {
                Subject = "qcCreatedLetterActivity"
            };

            // Create accounts on which we want to run QC
            _accountIdArray = new Guid[5];
            for (int i = 0; i < 5; i++)
            {
                var acct = new Account()
                {
                    Name = "Account For Quick Campaign " + i.ToString()
                };
                _accountIdArray[i] = service.Create(acct);
                Console.WriteLine("Created {0}.", acct.Name);
            }
        }

        /// <summary>
        /// Deletes/Reverts the record that was created/changed for this sample.
        /// <param name="prompt">Indicates whether to prompt the user to delete 
        /// the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
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
                for (int i = 0; i < _accountIdArray.Length; i++)
                    service.Delete(Account.EntityLogicalName, _accountIdArray[i]);
                service.Delete(List.EntityLogicalName, _newListId);
                service.Delete(BulkOperation.EntityLogicalName, _qcBOId);
                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }
    }
}
