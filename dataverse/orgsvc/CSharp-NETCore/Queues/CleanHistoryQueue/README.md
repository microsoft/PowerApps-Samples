---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates removing completed items from queue history"
---

# CleanHistoryQueue

Demonstrates removing completed items from queue history

## What this sample does

This sample shows how to:
- Create a queue and add an activity (phone call)
- Mark an activity as completed using SetStateRequest
- Remove the completed activity from the queue using RemoveFromQueueRequest

Queues can accumulate completed work items over time. This sample demonstrates how to clean up queue history by removing completed items, helping maintain queue organization and performance.

## How this sample works

### Setup

The setup process:
1. Creates a private queue
2. Creates a phone call activity record
3. Marks the phone call as completed using SetStateRequest:
   - State: Completed (1)
   - Status: Made (2)
4. Creates a queue item linking the completed phone call to the queue

### Run

The main demonstration:
1. Executes RemoveFromQueueRequest with the queue item ID
2. The completed phone call is removed from the queue
3. The queue history is cleaned

### Cleanup

The cleanup process deletes all created records:
- Queue
- Phone call activity

## Demonstrates

This sample demonstrates:
- **RemoveFromQueueRequest**: Removing items from queues
- **SetStateRequest**: Changing activity state to completed
- **Queue maintenance**: Managing queue history and completed items
- **Activity lifecycle**: Understanding activity state transitions

## Sample Output

```
Connected to Dataverse.

Creating queue and phone call...
Setup complete.

Removing completed item from queue...
Completed item removed from queue.
Cleaning up...
Deleting 2 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Work with queues](https://learn.microsoft.com/power-apps/developer/data-platform/work-with-queues)
[RemoveFromQueueRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.removefromqueuerequest)
[SetStateRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.setstaterequest)
