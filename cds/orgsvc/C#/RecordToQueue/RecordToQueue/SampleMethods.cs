using Microsoft.Crm.Sdk.Messages;
//using Microsoft.Crm.Sdk.Samples;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
  public partial  class SampleProgram
    {

        private static Guid _sourceQueueId;
        private static Guid _destinationQueueId;
        private static Guid _letterId;
        private static Guid _userId;
        
        private static bool prompt= true;

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
        /// Create source queue and destination queue.
        /// Create a letter  entity.
        /// Add letter  entity to source queue.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            var QueueViewType = new
            {
                Public = 0,
                Private = 1
            };
            //Create new queues and store their returned GUIDs in variables for later use.
            Queue sourceQueue = new Queue
            {
                Name = "Source Queue",
                Description = "This is an example queue.",
                QueueViewType = new OptionSetValue(QueueViewType.Private)
            };

            _sourceQueueId = service.Create(sourceQueue);
            Console.WriteLine("Created {0}", sourceQueue.Name);

            Queue destinationQueue = new Queue
            {
                Name = "Destination Queue",
                Description = "This is an example queue.",
                QueueViewType = new OptionSetValue(QueueViewType.Private)
            };

            _destinationQueueId = service.Create(destinationQueue);
            Console.WriteLine("Created {0}", destinationQueue.Name);


            // Create a letter  entity.
            Letter newLetter = new Letter
            {
                Description = "Example Letter"
            };

            _letterId = service.Create(newLetter);
            Console.WriteLine("Created {0}", newLetter.Description);

            // Use AddToQueue message to add an  entity into a queue, which will associate
            // the letter with the first queue.
            AddToQueueRequest addToSourceQueue = new AddToQueueRequest
            {
                DestinationQueueId = _sourceQueueId,
                Target = new EntityReference(Letter.EntityLogicalName, _letterId)
            };

            service.Execute(addToSourceQueue);
            Console.WriteLine("Added letter record to {0}", sourceQueue.Name);

            // Retrieve/create a user record for assigning the queue item to the user's
            // queue.
            _userId = SystemUserProvider.RetrieveSalesManager(service);
            
            
                
                  

            return;
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
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
                service.Delete(Letter.EntityLogicalName, _letterId);
                service.Delete(Queue.EntityLogicalName, _sourceQueueId);
                service.Delete(Queue.EntityLogicalName, _destinationQueueId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }

    }
}
