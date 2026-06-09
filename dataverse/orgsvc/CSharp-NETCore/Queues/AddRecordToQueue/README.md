---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates adding records to queues and moving between queues"
---

# AddRecordToQueue

Demonstrates adding records to queues and moving records between queues

## What this sample does

This sample shows how to:
- Create multiple queues (source and destination)
- Create an activity record (letter)
- Add a record to a queue using AddToQueueRequest
- Move a record from one queue to another queue

Queues are used to organize work items and activities that need attention. This sample demonstrates the routing of work items between queues.

## How this sample works

### Setup

The setup process:
1. Creates a "Source Queue" (private queue)
2. Creates a "Destination Queue" (private queue)
3. Creates a letter activity record
4. Adds the letter to the Source Queue using AddToQueueRequest

### Run

The main demonstration:
1. Retrieves the queue and letter IDs from the entity store
2. Executes AddToQueueRequest with:
   - SourceQueueId: The source queue containing the letter
   - Target: EntityReference to the letter activity
   - DestinationQueueId: The destination queue to move the letter to
3. The letter is moved from the source queue to the destination queue

### Cleanup

The cleanup process deletes all created records:
- Source queue
- Destination queue
- Letter activity

## Demonstrates

This sample demonstrates:
- **AddToQueueRequest**: Adding records to queues and routing between queues
- **Queue routing**: Moving work items from one queue to another
- **Activity management**: Working with activity entities (letter) in queues
- **EntityReference**: Referencing entities across operations

## Sample Output

```
Connected to Dataverse.

Creating queues and letter...
  Created Source Queue
  Created Destination Queue
  Created letter activity
  Added letter to Source Queue
Setup complete.

Moving letter between queues...
Letter moved to destination queue.
Cleaning up...
Deleting 3 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Work with queues](https://learn.microsoft.com/power-apps/developer/data-platform/work-with-queues)
[AddToQueueRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.addtoqueuerequest)
