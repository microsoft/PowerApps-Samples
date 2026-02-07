---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates releasing queue items from workers back to the queue"
---

# ReleaseQueueItems

Demonstrates releasing queue items from workers back to the queue

## What this sample does

This sample shows how to:
- Create a queue and add a queue item
- Assign a queue item to a worker (user)
- Release the queue item from the worker back to the queue using ReleaseToQueueRequest

When a queue item is assigned to a worker, they become responsible for handling it. This sample demonstrates how to release that assignment and return the item to the general queue pool, making it available for other workers to pick up.

## How this sample works

### Setup

The setup process:
1. Creates a private queue
2. Creates a letter activity record
3. Creates a queue item linking the letter to the queue
4. Retrieves the current user's ID using WhoAmIRequest
5. Assigns the queue item to the current user by updating the workerid field

### Run

The main demonstration:
1. Executes ReleaseToQueueRequest with the queue item ID
2. The queue item is released from the worker's assignment
3. The item becomes available in the queue for other workers to pick

### Cleanup

The cleanup process deletes all created records:
- Queue
- Letter activity
- Queue item (automatically deleted when queue or letter is deleted)

## Demonstrates

This sample demonstrates:
- **ReleaseToQueueRequest**: Releasing queue items from worker assignments
- **WhoAmIRequest**: Getting the current user's identity
- **Queue item assignment**: Understanding worker assignment workflow
- **Queue item lifecycle**: Managing work item states (assigned vs. available)

## Sample Output

```
Connected to Dataverse.

Creating queue and queue item...
Setup complete.

Releasing queue item...
Queue item released from worker.
Cleaning up...
Deleting 2 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Work with queues](https://learn.microsoft.com/power-apps/developer/data-platform/work-with-queues)
[ReleaseToQueueRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.releasetoqueuerequest)
