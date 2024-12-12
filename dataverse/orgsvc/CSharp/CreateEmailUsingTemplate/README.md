# Create an email using a template

This topic shows how to instantiate an email record by using the [InstantiateTemplateRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.instantiatetemplaterequest) message.

## How to run this sample

See [How to run samples](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/README.md) for information about how to run this sample.

## What this sample does

The `InstantiateTemplateRequest` message is intended to be used in a scenario where it contains the parameters that are needed to create an email message from a template (email template).

## How this sample works

In order to simulate the scenario described in [What this sample does](#what-this-sample-does), the sample will do the following:

### Setup

1. Checks for the current version of the org.
2. The `CreateRequiredRecords` method creates any table records that this sample requires.

### Demonstrate

1. The `InstantiateTemplateRequest` method creates an email using the template. 
2. The `XmlSerializer` method serializes the email message to XML and saves to a file.

### Clean up

Display an option to delete the records created in the [Setup](#setup). The deletion is optional in case you want to examine the entities and data created by the sample. You can manually delete the records to achieve the same result.
