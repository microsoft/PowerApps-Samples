# Sample: Send an email

This sample shows how to send an email using [SendEmailRequest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.crm.sdk.messages.sendemailrequest?view=dynamics-general-ce-9) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/cds/README.md) for information about how to run this sample.

## What this sample does

The `SendEmailRequest` message is intended to be used in a scenario where it contains data that is needed to send an email message.

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
1. The `Contact` method creates a contact to send an email to `(To: field)`.
1. The `WhoAmIRequest` method gets the current user information to send the email `(From: field)`.
1. The `ActivityParty`method creates  `To` and `From` activity party for the email.
1. The `Email` method creates an email message.

### Demonstrate

The `SendEmailRequest` method sends an email message created in the [Setup](#setup).

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
