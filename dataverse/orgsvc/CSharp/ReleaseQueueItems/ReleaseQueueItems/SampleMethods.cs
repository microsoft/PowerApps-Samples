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
        private static Guid _queueItemId;
        private static Guid _letterId;
        private static Guid _queueId;
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
        /// This method creates any entity records that this sample requires.
        /// Create a queue record.
        /// Create a letter record.
        /// Create a queue item for queue record.
        /// Retrieves new owner's details. 
        /// Update the queue item record to assign it to new owner.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create a private queue instance and set its property values.
            Queue newQueue = new Queue
            {
                Name = "Example Queue.",
                Description = "This is an example queue.",
                QueueViewType = new OptionSetValue((int)QueueQueueViewType.Private)
            };

            // Create a new queue and store its returned GUID in a variable 
            // for later use.
            _queueId = service.Create(newQueue);

            Console.WriteLine("Created {0}.", newQueue.Name);

            Letter newLetter = new Letter
            {
                Description = "Example Letter"
            };

            _letterId = service.Create(newLetter);

            Console.WriteLine("Created {0}.", newLetter.Description);

            // Create a new instance of a queueitem and initialize its 
            // properties.
            QueueItem item = new QueueItem
            {
                QueueId = new EntityReference(Queue.EntityLogicalName, _queueId),
                ObjectId = new EntityReference(Letter.EntityLogicalName, _letterId)
            };

            // Create the queueitem on the server, which will associate 
            // the letter with the queue.
            _queueItemId = service.Create(item);

            Console.WriteLine("Created the letter queue item for the queue.");

            // Retrieve the user information.
            WhoAmIRequest whoAmIRequest = new WhoAmIRequest();
            WhoAmIResponse whoAmIResponse = (WhoAmIResponse)service.Execute(
                whoAmIRequest);

            ColumnSet columnSet = new ColumnSet("fullname");
            SystemUser currentUser = (SystemUser)service.Retrieve(
                SystemUser.EntityLogicalName,
                whoAmIResponse.UserId, columnSet);
            String currentUserName = currentUser.FullName;

            // Create an instance of an existing queueitem in order to specify 
            // the user that will be working on it.
            QueueItem queueItem = new QueueItem
            {
                QueueItemId = _queueItemId,
                WorkerId = new EntityReference(SystemUser.EntityLogicalName,
                    whoAmIResponse.UserId)
            };

            service.Update(queueItem);

            Console.WriteLine("The letter queue item is queued for new owner {0}.",
                currentUserName);

            return;
        }
        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user to 
        /// delete the records created in this sample.</param>
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
                service.Delete(QueueItem.EntityLogicalName, _queueItemId);
                service.Delete(Letter.EntityLogicalName, _letterId);
                service.Delete(Queue.EntityLogicalName, _queueId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }
    }
}
