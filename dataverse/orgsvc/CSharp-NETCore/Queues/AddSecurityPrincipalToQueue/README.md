---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates adding security principals (teams/users) as queue members"
---

# AddSecurityPrincipalToQueue

Demonstrates adding security principals (teams/users) as queue members

## What this sample does

This sample shows how to:
- Create a queue and a team
- Add a security principal (team) to a queue using AddPrincipalToQueueRequest
- Configure queue membership for teams

Adding security principals to queues makes them members of the queue, allowing them to work with queue items. This differs from sharing access - queue membership provides a direct association between the principal and the queue.

## How this sample works

### Setup

The setup process:
1. Creates a queue
2. Retrieves the default business unit (where parentbusinessunitid is null)
3. Creates a team associated with the default business unit

### Run

The main demonstration:
1. Retrieves the team entity with its name column
2. Executes AddPrincipalToQueueRequest with:
   - Principal: The team entity (not just EntityReference)
   - QueueId: The ID of the queue
3. The team is added as a member of the queue
4. Team members can now work with items in this queue

### Cleanup

The cleanup process deletes all created records:
- Team
- Queue

## Demonstrates

This sample demonstrates:
- **AddPrincipalToQueueRequest**: Adding security principals to queue membership
- **Queue membership**: Understanding the relationship between principals and queues
- **Entity retrieval**: Retrieving full entity records for request parameters
- **Business unit query**: Retrieving the default business unit
- **Team management**: Creating and associating teams with queues

## Sample Output

```
Connected to Dataverse.

Creating queue and team...
Setup complete.

Adding team to queue...
Team added to queue.
Cleaning up...
Deleting 2 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Work with queues](https://learn.microsoft.com/power-apps/developer/data-platform/work-with-queues)
[AddPrincipalToQueueRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.addprincipaltoqueuerequest)
[Queue entity reference](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/queue)
