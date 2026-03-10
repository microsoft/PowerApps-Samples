---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to perform common data operations using the Dataverse Web API."
---

# Web API Basic Operations sample

This .NET 6.0 sample demonstrates how to perform common data operations using the Dataverse Web API.

This sample uses the common helper code in the [WebAPIService](../WebAPIService) class library project.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with privileges to perform data operations.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the [BasicOperations.sln](BasicOperations.sln) file using Visual Studio 2022.
1. Edit the [appsettings.json](../appsettings.json) file to set the following property values.

   | Property | Instructions |
   |----------|--------------|
   | `Url` | The Url for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. Learn how to find your URL in [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources). |
   | `UserPrincipalName` | Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment. |
   | `Password` | Replace the placeholder `yourPassword` value with the password you use. |

1. Save the `appsettings.json` file.
1. Press `F5` to run the sample.

## Demonstrates

This sample has five sections.

### Section 1: Basic Create and Update operations

Operations:

- Create a contact record.
- Update the contact record.
- Retrieve the contact record.
- Update a single property of the contact record.
- Retrieve a single property of the contact record.

### Section 2: Create record associated to another

Operations: Associate a new record to an existing one.

### Section 3: Create related entities

Operations: Create the following entries in one operation: an account, its associated primary contact, and open tasks for that contact.

Entity types have the following relationships:

```
Accounts
    |---[Primary] Contact (N-to-1)
        |---Tasks (1-to-N)
```

### Section 4: Associate and Disassociate entities

Operations:

- Add a contact to the account `contact_customer_accounts` collection.
- Remove a contact from the account `contact_customer_accounts` collection.
- Associate a security role to a user using the `systemuserroles_association` collection.
- Remove a security role for a user using the `systemuserroles_association` collection.

### Section 5: Delete sample entities

Operations: A reference to each record created in this sample was added to a list as it was created. This section loops through that list and deletes each record.

## Clean up

By default this sample deletes all the records created in it. To view created records after the sample is complete, change the `deleteCreatedRecords` variable to `false` and you're prompted to delete the records if desired.
