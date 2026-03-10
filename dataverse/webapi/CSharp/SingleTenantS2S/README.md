# A single tenant server-to-server sample

| **C#** | **.NET Framework 4.8** |

This sample shows how to authenticate an Application User with the Dataverse web service and invoke a Web API.
It demonstrates web service authentication and message invocation for use in a server-to-server scenario.

An Application User is a virtual user with no associated Dataverse license. More information:
[Manage application users in the Power Platform admin center](https://learn.microsoft.com/power-platform/admin/manage-application-users#create-an-application-user)

## How to run this sample

1. Clone the samples repo so that you have a local copy.
1. Open the local sample solution in Visual Studio 2022.
1. Edit the App.config file according to the TODO comments in the \<appsettings\> section of the file.
1. Press F5 to run the sample.
1. When the sample is finished, after the console output is displayed, press any key to exit the program.

Note: Make sure your test environment has at least one active account in the Account table.

## What this sample does

This code sample retrieves account data from the Dataverse environment and outputs it to the console.
Below is listed some example output displaying the top three account nanes.

```json
  {
  "@odata.context": "https://myorg.api.crm.dynamics.com/api/data/v9.2/$metadata#accounts(name)",
  "@Microsoft.Dynamics.CRM.totalrecordcount": -1,
  "@Microsoft.Dynamics.CRM.totalrecordcountlimitexceeded": false,
  "@Microsoft.Dynamics.CRM.globalmetadataversion": "81011296",
  "value": [
    {
      "@odata.etag": "W/\"80649578\"",
      "name": "Litware, Inc. (sample)",
      "accountid": "78914942-34cb-ed11-b596-0022481d68cd"
    },
    {
      "@odata.etag": "W/\"80649580\"",
      "name": "Adventure Works (sample)",
      "accountid": "7a914942-34cb-ed11-b596-0022481d68cd"
    },
    {
      "@odata.etag": "W/\"80649582\"",
      "name": "Fabrikam, Inc. (sample)",
      "accountid": "7c914942-34cb-ed11-b596-0022481d68cd"
    }
  ]
}
```

## How this sample works

This program uses an app registration in Microsodft Azure, specified in the App.config file, 
to authenticate the Application User and invoke a Web API request. Use of a client secret associated
with the app registration allows for a non-interactive authentication flow which can be used in a
server-to-server scenario.

### Setup

Setup includes creating an Azure app registration with client secret, an Application User, and assigning that user a role (that can read accounts) in your environment.

Follow the instructions in the topic [Use single-tenant server-to-server authentication](https://learn.microsoft.com/power-apps/developer/data-platform/use-single-tenant-server-server-authentication)

### Demonstrate

- Use of the Microsoft Authentication Library (MSAL) for web service authentication
- Use of an Application User and app registration with client secret for authentication
- Web client configuration
- Web API bound function invocation and web service response parsing

### Clean up

In Power Apps, de-activate the Application User in your environment. In the Azure portal, delete the app registration.

More information: [Activate or deactivate an application user](https://learn.microsoft.com/power-platform/admin/manage-application-users#activate-or-deactivate-an-application-user)
