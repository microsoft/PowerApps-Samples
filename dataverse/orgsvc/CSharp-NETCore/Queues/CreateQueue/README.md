---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates how to create a queue with various configuration options"
---

# CreateQueue

Demonstrates how to create a queue with various configuration options

## What this sample does

This sample shows how to:
- Create a queue entity with configuration properties
- Set queue properties including email delivery methods, filtering methods, and view type
- Configure a private queue that can be used for work item routing

Queues in Dataverse are used to organize and prioritize work items, enabling teams to manage activities, cases, and other records that require action.

## How this sample works

### Setup

No setup is required for this sample.

### Run

The main demonstration:
1. Creates a queue entity with the following properties:
   - Name: "Example Queue"
   - Description: "This is an example queue."
   - Incoming email delivery method: None (0)
   - Incoming email filtering method: All Email Messages (0)
   - Outgoing email delivery method: None (0)
   - Queue view type: Private (1)

2. Displays the created queue information including ID and configuration settings

### Cleanup

The cleanup process deletes the created queue record.

## Demonstrates

This sample demonstrates:
- **Entity creation**: Creating queue records using late-bound syntax
- **OptionSetValue**: Setting option set values for queue configuration properties
- **Queue configuration**: Understanding queue property options including:
  - Email delivery methods (None, Email Router, Forward Mailbox)
  - Email filtering methods (All Messages, Responses Only, From Leads/Contacts/Accounts)
  - Queue view types (Public, Private)

## Sample Output

```
Connected to Dataverse.

Creating a queue...
Created queue: Example Queue (ID: a1234567-89ab-cdef-0123-456789abcdef)
  Description: This is an example queue.
  Incoming Email Delivery Method: None (0)
  Incoming Email Filtering Method: All Email Messages (0)
  Outgoing Email Delivery Method: None (0)
  Queue View Type: Private (1)

Queue creation complete.
Cleaning up...
Deleting 1 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Work with queues](https://learn.microsoft.com/power-apps/developer/data-platform/work-with-queues)
[Queue entity reference](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/queue)
