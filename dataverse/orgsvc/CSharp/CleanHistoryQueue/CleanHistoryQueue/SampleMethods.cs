using Microsoft.Crm.Sdk.Messages;
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
        // Define the IDs needed for this sample.
        private static Guid _queueId;
        private static Guid _phoneCallId;
        private static Guid _queueItemId;
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
        /// Create a queue instance. 
        /// Create a phone call activity instance.
        /// Add phone call entity instance in to queue.
        /// Mark phone call entity instance status as completed.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // Create a queue instance and set its property values.
            var newQueue = new Queue()
            {
                Name = "Example Queue",
                Description = "This is an example queue.",
                QueueViewType = new OptionSetValue((int)QueueQueueViewType.Private)
            };

            _queueId = service.Create(newQueue);
            Console.WriteLine("Created {0}.", newQueue.Name);

            // Create a phone call activity instance.
            var newPhoneCall = new PhoneCall
            {
                Description = "Example Phone Call"
            };

            _phoneCallId = service.Create(newPhoneCall);
            Console.WriteLine("Created {0}.", newPhoneCall.Description);

            // Create a new instance of a queueitem and initialize its 
            // properties.
            var item = new QueueItem
            {
                QueueId = new EntityReference(Queue.EntityLogicalName, _queueId),
                ObjectId = new EntityReference(PhoneCall.EntityLogicalName, _phoneCallId)
            };

            // Create the queueitem on the server, which will associate 
            // the phone call activity with the queue.
            _queueItemId = service.Create(item);

            Console.WriteLine("Added phone call entity instance to {0}", newQueue.Name);

            // Mark the phone call as completed.
            var setStatePhoneCall = new SetStateRequest
            {
                EntityMoniker = new EntityReference(PhoneCall.EntityLogicalName,
                    _phoneCallId),
                State = new OptionSetValue((int)PhoneCallState.Completed),
                Status = new OptionSetValue(-1)
            };

            service.Execute(setStatePhoneCall);
            Console.WriteLine("PhoneCall entity instance has been marked as completed.");

            return;
        }
        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
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
                service.Delete(PhoneCall.EntityLogicalName, _phoneCallId);
                service.Delete(Queue.EntityLogicalName, _queueId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
