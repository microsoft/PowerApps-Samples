---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates creating and deleting a queue"
---

# DeleteQueue

Demonstrates creating and deleting a queue

## What this sample does

This sample shows how to:
- Create a queue record
- Delete a queue record using the Delete method

This demonstrates the basic lifecycle of queue management, showing both creation and deletion operations.

## How this sample works

### Setup

No setup is required for this sample.

### Run

The main demonstration:
1. Creates a queue entity with name "Example Queue"
2. Displays the created queue's ID
3. Indicates the queue will be deleted during cleanup

### Cleanup

The cleanup process deletes the created queue record using the standard Delete method.

## Demonstrates

This sample demonstrates:
- **Entity creation**: Creating queue records
- **Entity deletion**: Using service.Delete() to remove queue records
- **Queue lifecycle management**: Basic CRUD operations for queues

## Sample Output

```
Connected to Dataverse.

Creating a queue...
Created queue: Example Queue (ID: a1234567-89ab-cdef-0123-456789abcdef)
Queue will be deleted during cleanup.
Cleaning up...
Deleting 1 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Work with queues](https://learn.microsoft.com/power-apps/developer/data-platform/work-with-queues)
[Queue entity reference](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/queue)
