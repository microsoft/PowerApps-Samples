---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to ..."
---

# How to associate a user with a security role

Learn how to associate a system user with a security role.

Related article(s):

- [Query data using QueryExpression](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/queryexpression/overview)
- [Role-based security roles](https://learn.microsoft.com/power-platform/admin/database-security)
- [IOrganizationService.Associate Method](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice.associate?view=dataverse-sdk-latest)

## About the sample code

|Sample|Description|Build target|
|---|---|---|
|[AssociateSecurityRoleToUser](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/CSharp-NETCore/Security/AssociateSecurityRoleToUser) | Demonstrates associating a user with a security role.|.NET 9|

The code samples demonstrates how to associate a system user with a security role. Specifically, the samples demonstrates how to:

1. Connect to Dataverse using a [connection string](https://learn.microsoft.com/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect) that defines required connection information
1. Query for a security role using its name attribute.
1. Associate the logged on user with that security role.

Additional information can be found in [README-code-design](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/orgsvc/CSharp-NETCore/README-code-design.md) file.

## How to build and run the code sample(s)

1. Clone the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Locate the sample folder.
1. Open the solution file (*.sln) in Visual Studio.
1. Edit the project's appsettings.json file and set the `Url`value as appropriate for your Dataverse test environment.
1. Build and run the project [F5].
1. You will be prompted in a browser window for account logon credentials to the target environment.

## Expected program output

For a successful run, the program's console output should look similar to the following example.
Otherwise, any errors or exceptions will be displayed.

```console
Discovering who you are...done.
Associating your system user record with role 'Basic User'..done.

Use the Power Platform admin center to see that you now have
the 'Basic User' role. Afterwards, remove the role if desired.
Press any key to undo environment data changes.
```

If you get a "duplicate key" exception, it is probably because the Basic User role was already associated with your system user account. In that case, you can removed the Basic User role from your account using the Power Platfor admin center before running the program.
