---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates sharing queue access with teams using security permissions"
---

# ShareQueue

Demonstrates sharing queue access with teams using security permissions

## What this sample does

This sample shows how to:
- Create a private queue and a team
- Share queue access with a team using GrantAccessRequest
- Configure specific access rights (Read and AppendTo) for the team

Private queues require explicit sharing to grant access to users or teams. This sample demonstrates how to programmatically configure queue sharing and access control.

## How this sample works

### Setup

The setup process:
1. Creates a private queue (queueviewtype = 1)
2. Retrieves the default business unit (where parentbusinessunitid is null)
3. Creates a team associated with the default business unit

### Run

The main demonstration:
1. Executes GrantAccessRequest with:
   - PrincipalAccess containing:
     - Principal: EntityReference to the team
     - AccessMask: ReadAccess | AppendToAccess (combined rights)
   - Target: EntityReference to the queue
2. The team is granted read and append-to access to the queue
3. Team members can now view and add items to the queue

### Cleanup

The cleanup process deletes all created records:
- Team
- Queue

## Demonstrates

This sample demonstrates:
- **GrantAccessRequest**: Sharing record access with security principals
- **PrincipalAccess**: Configuring access rights for principals
- **AccessRights**: Using AccessMask flags (ReadAccess, AppendToAccess)
- **Security model**: Understanding Dataverse record-level security
- **Business unit query**: Retrieving the default business unit

## Sample Output

```
Connected to Dataverse.

Creating queue and team...
Setup complete.

Sharing queue with team...
Queue access granted to team.
Cleaning up...
Deleting 2 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[Work with queues](https://learn.microsoft.com/power-apps/developer/data-platform/work-with-queues)
[GrantAccessRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.grantaccessrequest)
[Security concepts in Dataverse](https://learn.microsoft.com/power-apps/developer/data-platform/security-concepts)
