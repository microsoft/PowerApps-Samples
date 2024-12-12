# Send an email using a template

This sample shows how to send an email message by using a template using the [SendEmailFromTemplateRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.sendemailfromtemplaterequest) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `SendEmailFromTemplateRequest` message is intended to be used in a scenario where it contains data that is needed to send an email message using a template.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. Creates a contact record to send an email to (To: column).

### Demonstrate

1. The `ActivityParty` creates the `From:`  and `To:` activity party for the email.
2. Creates an email message.
3. The `QueryExpression` queries to get one of the email template of type `Contact`.
4. The `SendEmailFromTemplateRequest` sends an email message by using a template.

### Clean up

Display an option to delete the sample data that is created in [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
