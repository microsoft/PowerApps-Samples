# Create an email using a template

This topic shows how to instantiate an email record by using the [InstantiateTemplateRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.instantiatetemplaterequest) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `InstantiateTemplateRequest` message is intended to be used in a scenario where it contains the parameters that are needed to create an email message from a template (email template).

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Creates an account record to be used with the email template.
2. Creates an email template for accounts with predefined body and subject XML content.
   - The template includes XSL transformations for the body and subject
   - The template is configured for accounts and US English (language code 1033)

### Demonstrate

1. Uses the `InstantiateTemplateRequest` message to create an email message using the template.
   - Specifies the template ID, target account ID, and object type (account)
2. Serializes the email message response to XML and saves it to a file named "email-message.xml".

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
