# Microsoft.PowerPlatform.UserManagement.Powershell scripts

These code samples help customers perform user management operations across one or multiple environments in a tenant.

## What this sample does

This sample helps customers do the following for: an environment, all environments in a geography, all environments in the tenant.

1. Generate reports of users having a specified role assignment (e.g.; System Administrator)
1. Remove role assignments from for a list of users
1. Add role assignments to a list of users

This code can be run by Global / Power Platform Administrator users.

## Requirements

- Powershell 5.1.22 (default for windows 10 & 11) or lower
- .NET Framework 4.6.2 Developer Pack
- Visual Studio 2017 or a newer version

## How to run this sample

1. Install [Visual Studio 2017](https://visualstudio.microsoft.com/downloads/) or higher.
2. Install the .NET Framework 4.6.2 [Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net462).
3. Open the solution file (Microsoft.PowerPlatform.Administration.Powershell.sln) in administrator mode and build the solution.
4. Open PowerShell in administrator mode.
5. Set Execution Policy to *Unrestricted*.
6. Import the Microsoft.PowerPlatform.Administration.Powershell module, as shown below.

    ```powershell
    cd Microsoft.PowerPlatform.Administration.Powershell
    Import-Module .\Microsoft.PowerPlatform.Administration.UserManagement.psm1
    ```

7. Run the command of your choice from the following PowerShell commands (see below).
8. Each command will prompt the user to provide credentials when connecting to Dataverse. Enter a user principal name and password.

## Powershell commands

### Generate reports of role assignments

#### Command: Get-UsersWithRoleAssignment

| Parameter | Description |
|---|---|
|roleName|Localized role name in Dataverse (e.g.; System Administrator).|
|environmentUrl|Url of the environment for when the administrator wants to get reports from only one environment.|
|processAllEnvironments|Generate reports for all environments that the administrator user has access to.|
|geo|Generate reports for environments in a given geography. If not specified, processes all environments across all geographies. Valid Geo codes - [NA, EMEA, APAC, SAM, OCE, JPN, IND, CAN, GBR, FRA, UAE,ZAF, GER, CHE, KOR, NOR, SGP] |
|outputLogsDirectory|Location folder for the logs & reports to be written to.|

### Remove role assignments from given list of users

#### Command: Remove-RoleAssignmentFromUsers

| Parameter | Description |
|---|---|
|roleName|Localized role name in Dataverse (e.g.; System Administrator).|
|usersFilePath|Path to file containing list of user principal names (one per line).|
|environmentUrl|Url of the environment for when the administrator wants to get reports from only one environment.|
|processAllEnvironments|Generate reports for all environments the administrator user has access to.|
|geo|Generate reports for environments in a given geography. If not specified, processes all environments across all geographies. Valid Geo codes - [NA, EMEA, APAC, SAM, OCE, JPN, IND, CAN, GBR, FRA, UAE,ZAF, GER, CHE, KOR, NOR, SGP] |
|outputLogsDirectory|Location folder for the logs & reports to be written to.|

### Add role assignments for a given list of users

#### Command: Add-RoleToUsers

| Parameter | Description |
|---|---|
|roleName|Localized role name in Dataverse (e.g.; System Administrator).|
|usersFilePath|Path to the file containing a list of user principal names (one per line).|
|environmentUrl|Url of the environment for when the administrator wants to get reports from only one environment.|
|processAllEnvironments|Generate reports for all environments the administrator user has access to.|
|geo|Generate reports for environments in a given geography. If not specified, processes all environments across all geographies. Valid Geo codes - [NA, EMEA, APAC, SAM, OCE, JPN, IND, CAN, GBR, FRA, UAE,ZAF, GER, CHE, KOR, NOR, SGP] |
|outputLogsDirectory|Location folder for the logs & reports to be written to.|

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g.; label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Security

For reporting security issues, please refer to this [security](https://github.com/microsoft/PowerApps-Samples/blob/master/SECURITY.md) document.
