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
- NuGet version 4.7 or higher

## How to run this sample

1. Install [NuGet](https://www.nuget.org/downloads) 
2. Clone the [Powerapps-Samples repo](https://github.com/microsoft/PowerApps-Samples.git).
3. Install [Visual Studio 2017](https://visualstudio.microsoft.com/downloads/) or higher.
4. Install the .NET Framework 4.6.2 [Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/net462).
5. Navigate to the powershell/UserManagement/Microsoft.PowerPlatform.Administration.UserManagement folder.
6. Open the solution file (Microsoft.PowerPlatform.Administration.Powershell.sln) in administrator mode and [build](https://learn.microsoft.com/visualstudio/ide/building-and-cleaning-projects-and-solutions-in-visual-studio?view=vs-2022) the solution.
7. If you notice any errors, check if the path to the directory is too long and try to copy the Microsoft.PowerPlatform.Administration.UserManagement into a shorter path ( Ex: C:\) and try building again.
8. Open PowerShell in administrator mode.
9. Set Execution Policy to *Unrestricted*.
10. Import the Microsoft.PowerPlatform.Administration.UserManagement.psm1 module, as shown below.

    ```powershell
    cd Microsoft.PowerPlatform.Administration.Powershell
    Import-Module .\Microsoft.PowerPlatform.Administration.UserManagement.psm1
    ```

11. Run the command of your choice from the following PowerShell commands (see below).
12. Each command will prompt the user to provide credentials when connecting to Dataverse. Enter a user principal name and password.

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

### Assign all user records from the source user to the target user

#### Command: Add-BulkRecordsToUsers

| Parameter | Description |
|---|---|
|usersFilePath|Path to file containing list of user principal names (source and target user principals separated by commas)|
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
