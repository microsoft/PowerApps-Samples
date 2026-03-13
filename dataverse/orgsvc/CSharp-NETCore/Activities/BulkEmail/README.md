---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "Demonstrates how to send bulk email and monitor the async operation results."
---

# Send Bulk Email and Monitor Results

Demonstrates how to send bulk email using the SendBulkMailRequest message and monitor the results by retrieving records from the asyncoperation table.

## What this sample does

The `SendBulkMailRequest` message is used to send bulk emails to multiple recipients. This sample shows how to:
- Create test contact records
- Send bulk email to the contacts using a template
- Monitor the async operation to track when the email send operation completes

## How this sample works

### Setup

The sample creates two contact records with email addresses to serve as recipients for the bulk email.

### Run

The sample demonstrates the following steps:

1. Gets the current user to use as the email sender (using WhoAmIRequest)
2. Sets a tracking ID for the bulk email request
3. Creates a QueryExpression to retrieve the contacts for the email list
4. Executes SendBulkMailRequest with:
   - The query for recipients
   - The sender reference
   - A built-in email template ID
   - The tracking ID for monitoring
5. Monitors the async operation by polling the asyncoperation table every second for up to 60 seconds
6. Reports when the operation completes successfully

### Cleanup

The sample deletes the created contact records by default.

## Demonstrates

- Using `SendBulkMailRequest` to send emails to multiple recipients
- Using a template for bulk email
- Monitoring async operations by polling the asyncoperation table
- Using QueryByAttribute to retrieve async operations by tracking ID
- Checking async operation state to determine completion

## Sample Output

```
Connected to Dataverse.

Creating contact records...
Contact 1 created.
Contact 2 created.

Creating and sending bulk email...
Sent bulk email.

Starting monitoring process...
Retrieved bulk email async operation.
Checking operation's state for 60 seconds.

Operation completed successfully.

When the bulk email operation has completed, all sent emails will
have a status of 'Pending Send' and will be picked up by your email router.

Deleting 2 created record(s)...
Records deleted.

Press any key to exit.
```

## See also

[SendBulkMailRequest Class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.sendbulkmailrequest)
[Email activity entities](https://learn.microsoft.com/power-apps/developer/data-platform/email-activity-entities)
[Asynchronous operation states](https://learn.microsoft.com/power-apps/developer/data-platform/asynchronous-service)
