---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates assigning queue items to specific workers"
---

# SpecifyQueueItem

Demonstrates assigning queue items to specific workers

## What this sample does

This sample shows how to:
- Create a queue and add a queue item
- Assign a queue item to a specific worker using PickFromQueueRequest
- Retrieve the current user's information for assignment
- Display the worker's name after successful assignment

Queue items represent work that needs to be completed. This sample demonstrates how to assign specific queue items to designated workers who will be responsible for handling them.

## How this sample works

### Setup

The setup process:
1. Creates a private queue
2. Creates a letter activity record
3. Creates a queue item linking the letter to the queue

### Run

The main demonstration:
1. Retrieves the current user's ID using WhoAmIRequest
2. Retrieves the current user's full name from the systemuser entity
3. Executes PickFromQueueRequest with:
   - QueueItemId: The queue item to assign
   - WorkerId: The ID of the user to assign it to (current user)
4. Displays the worker's name confirming the assignment

### Cleanup

The cleanup process deletes all created records:
- Queue
- Letter activity
- Queue item (automatically deleted when queue or letter is deleted)

## Demonstrates

This sample demonstrates:
- **PickFromQueueRequest**: Assigning queue items to specific workers
- **WhoAmIRequest**: Getting the current user's identity
- **Entity retrieval**: Retrieving user information for display
- **Queue item assignment**: Understanding worker assignment workflow

## Sample Output

```
Connected to Dataverse.

Creating queue and queue item...
Setup complete.

Assigning queue item to worker...
Queue item assigned to John Doe.
Cleaning up...
Deleting 2 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Work with queues](https://learn.microsoft.com/power-apps/developer/data-platform/work-with-queues)
[PickFromQueueRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.pickfromqueuerequest)
