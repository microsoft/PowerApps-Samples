# Microsoft.PowerPlatform.UserManagement.Powershell scripts
These samples help customers to perform user management operations across one or multiple environments in the tenant

## what this sample does
This sample helps customers to do the following for - an environment / all environments in a geo / all environments in the tenant.
1. Generate reports of users having a specified role assignment (Ex : System Administrator)
2. Remove role assignments from for a list of users 
3. Add role assignments to a list of users

## How to run this sample

1. Install Visual studio 2017 or higher
2. Install .net framework 4.6.2 or higher
4. Open Microsoft.PowerPlatform.Administration.Powershell.sln and build the solution. 
5. Open powershell in administrator mode 
6. Set Execution Policy to Unrestricted
7. Import Microsoft.PowerPlatform.Administration.Powershell module, as shown below
    - cd Microsoft.PowerPlatform.Administration.Powershell
    - Import-Module .\Microsoft.PowerPlatform.Administration.UserManagement.psm1
8. Run the command of your choice from the following commands
9. Each command will prompt running user to provide credentials to connect to dataverse. Enter user principal name & password.

## Powershell commands 
### 1. To generate reports of role assignments
    -  Get-UsersWithRoleAssignment 
        - roleName : Localized role name in dataverse (Ex : System Administrator)
        - environmentUrl : Url of Environment, if admin wants to get reports from only one environment
        - processAllEnvironments : Generate reports for all environments the admin user has access to
        - geo : Generate reports for environments in given geo - GeoCodes[https://learn.microsoft.com/en-us/power-platform/admin/new-datacenter-regions]. If not specified, processes all environments across all geos. 
        - outputLogsDirectory : Location folder for the logs & reports to be written to.

### 2. To remove role assignments from given list of users
    -  Remove-RoleAssignmentFromUsers
        - roleName : Localized role name in dataverse (Ex : System Administrator)
        - usersFilePath : Path to file containing list of user princiapl names (one per line) 
        - environmentUrl : Url of Environment, if admin wants to get reports from only one environment
        - processAllEnvironments : Generate reports for all environments the admin user has access to
        - geo : Generate reports for environments in given geo - GeoCodes[https://learn.microsoft.com/en-us/power-platform/admin/new-datacenter-regions]. If not specified, processes all environments across all geos. 
        - outputLogsDirectory : Location folder for the logs & reports to be written to.

### 3. To add role assignments for a given list of users 
    -  Add-RoleToUsers
        - roleName : Localized role name in dataverse (Ex : System Administrator)
        - usersFilePath : Path to file containing list of user princiapl names (one per line) 
        - environmentUrl : Url of Environment, if admin wants to get reports from only one environment
        - processAllEnvironments : Generate reports for all environments the admin user has access to
        - geo : Generate reports for environments in given geo - [GeoCodes](https://learn.microsoft.com/en-us/power-platform/admin/new-datacenter-regions). If not specified, processes all environments across all geos. 
        - outputLogsDirectory : Location folder for the logs & reports to be written to.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Security
For reporting security issues Please refer to this [link](https://github.com/microsoft/PowerApps-Samples/blob/master/SECURITY.md)
