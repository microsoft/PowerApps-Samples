---
languages:
- csharp
products:
- power-platform
- power-apps
page_type: sample
description: "This sample demonstrates how to work with Dataverse column-level security features using the SDK for .NET."
---

# SDK for .NET column-level security sample

This .NET 8 sample demonstrates how to:

- Discover which columns can be secured in a Dataverse environment
- Discover which columns are currently secured
- Secure columns in a Dataverse environment
- Grant read or write access to selected fields to individual users
- Modify access to secured fields for individual users
- Revoke access to selected fields for individual users
- Provide read and write access to specific groups of users
- Enable masking of secured columns
- Retrieve unmasked values for secured columns

This sample uses the [Microsoft.PowerPlatform.Dataverse.Client.ServiceClient Class](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient).

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator privileges.
- An application user account with **Basic User** access. See [Configure users](#configure-users) for instructions about how to create this user.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `PowerApps-Samples\dataverse\orgsvc\CSharp-NETCore\Column-level-security\column-level-security.sln` file using Visual Studio 2022.
1. Edit the `appsettings.json` file. 

   - Replace the connection string `<ADD your-org>` placeholders as appropriate for your test environment.
   - Replace the `<ADD CLIENTID>` and `<ADD SECRET>` placeholders using the information from the instructions found in [Configure users](#configure-users)

1. Build the solution, and then press F5 to run the sample.

   When the sample runs, you will be prompted in the default browser to select an environment user account and enter a password. To avoid having to do this every time you run a sample, insert a `Password` parameter into the connection string in the `appsettings.json` file. For example:

   ```json
   {
   "ConnectionStrings": {
      "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;Password=mypassword;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
      }
   }
   ```

   >**Tip**
   > You can set a user environment variable named `DATAVERSE_APPSETTINGS` to the file path of the `appsettings.json` file stored anywhere on your computer. The samples will use that appsettings file if the environment variable exists and is not null. Be sure to log out and back in again after you define the variable for it to take affect. To set an environment variable, go to **Settings > System > About**, select **Advanced system settings**, and then choose **Environment variables**.


## Configure users

 This sample requires two user accounts:
 
 - The person running the program must have the system administrator security role
 - A second application user must not have the system administrator security role.

Verifying the behavior of column-level security configuration changes requires a user who does not have the system administrator security role. People with the system administrator security role are not effected by column-level security - they are always able to view the data.

This sample requires you create an application user and configure it to run as that the second user. Create a new authentication profile if necessary and use PAC CLI `pac auth select` command to indicate it as the active account.

Create an application user with the PAC CLI [pac admin create-service-principal](https://learn.microsoft.com/power-platform/developer/cli/reference/admin#pac-admin-create-service-principal) command. When you execute this command, you need to make sure that the active PAC CLI authentication profile is the environment you want to run the sample against.

> **IMPORTANT**
> When you run the `pac admin create-service-principal` command, you need the ID of your environment and you must specify the optional `role` parameter set for `'Basic User'`. Otherwise the default behavior is to use the system administrator security role, which defeats the purpose of the application user in this sample.

This is an example showing how to create the application user:

```PowerShell
PowerShell 7.5.0
PS C:\Users\you> pac admin create-service-principal `
>> --environment 00aa00aa-bb11-cc22-dd33-44ee44ee44ee `
>> --name 'field level security sample application user' `
>> --role 'Basic User'
>>
Connected as you@yourorg.onmicrosoft.com
Creating Entra ID Application 'field level security sample application user'... Done
Creating Entra ID Service Principal... Done

Connected to... <Your Environment Name>
Registering Application '00001111-aaaa-2222-bbbb-3333cccc4444' with Dataverse... Done
Creating Dataverse system user and assigning role... Done

Application Name         field level security sample application user
Tenant Id                aaaabbbb-0000-cccc-1111-dddd2222eeee
Application Id           00001111-aaaa-2222-bbbb-3333cccc4444
Service Principal Id     aaaaaaaa-bbbb-cccc-1111-222222222222
Client Secret            Aa1Bb~2Cc3.-Dd4Ee5Ff6Gg7Hh8Ii9_Jj0Kk1Ll2
Client Secret Expiration 3/27/2026 10:37:27 PM +00:00
System User Id           11bb11bb-cc22-dd33-ee44-55ff55ff55ff
```

After you create the application user, update the `appsettings.json` file. For the `ConnectionStrings` configured there:

1. Update the `default` connection string to replace the `<ADD your-org>` with the value for the environment you want to connect to.
1. Update the `applicationuser` connection string to replace the `<ADD your-org>` to the same value. Also, replace the `<ADD CLIENTID>` and `<ADD SECRET>` values to use the `Application Id` and `Client Secret` values for the application user you created.

## Demonstrates

The code for this sample is in the following files:

|File|Description|
|---------|---------|
|`Program.cs`|Controls the flow of the sample. Contains definition of [Setup](#setup), [Run](#run), and [Cleanup](#cleanup) static methods and calls those static methods from the `Main` method when the program runs. |
|`Examples.cs`|Contains 10 static methods that demonstrate operations related to column-level security operations. These methods may appear as code snippets in documentation.|
|`Helpers.cs`|Contains static methods used by the sample to manage setting up and running the sample. These methods are not the focus of this sample.|

This sample follows the common pattern described in [Sample code design](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/CSharp-NETCore/README-code-design.md). It is designed to be resilient when errors occur so you should be able to run the sample again if it failed previously.

### Manage cached configuration changes

This sample demonstrates changes that depend on changes made to configuration data. Normally these configuration changes are not used immediately after they are changed. Dataverse caches this information for best performance. Throughout this sample, the [ServiceClient.ForceServerMetadataCacheConsistency Property](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient.forceservermetadatacacheconsistency?view=dataverse-sdk-latest) is set to specify strong consistency. This shouldn't be a normal practice because there is some performance impact.

This sample also pauses for 30 seconds at several points to allow the cache to catch up with changes. This is recommended to improve resiliency when you automate testing immediately following configuration changes.

### Setup

The `Setup` static method does the following:

1. Create a solution publisher named `ColumnLevelSecuritySamplePublisher` with customization prefix of `sample` if it doesn't exist.
1. Create a solution named `ColumnLevelSecuritySampleSolution` if it doesn't exist.  All subsequent items created are created in the context of this solution.
1. Create a table named `sample_Example` if it doesn't exist.
1. Create 4 string columns in the `sample_Example` table if they don't exist. The table names are:
   
   - `sample_Email`
   - `sample_GovernmentId`
   - `sample_TelephoneNumber`
   - `sample_DateOfBirth`

1. Remove any existing sample data in the `sample_Example` table.
1. Add three rows of sample data with information in each column of the `sample_Example` table.
1. Create a new security role named `Column-level security sample role`.
1. Add privileges for the `sample_Example` table to the security role
1. Associate the user to the security role.
1. Create a [Field Security Profile ](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/fieldsecurityprofile) record named `Example Field Security Profile` that will be used in the [Manage access to secure column data to groups](#manage-access-to-secure-column-data-to-groups) section of the sample.
1. Associate the application user to the field security profile.
1. Wait 30 seconds for the cache to catch up with the new objects created.


### Run

The `Run` static method does the following:

#### Retrieve information about columns

1. Use the `Examples.DumpColumnSecurityInfo` method to download a CSV file with data about which columns in the system can be secured.
1. Use the `Examples.GetSecuredColumnList` method to retrieve and show a list of environment columns that are already secured.

#### Secure columns

1. Demonstrate that the application user can retrieve data from all the columns in the `sample_Example` table.
1. Use the `Examples.SetColumnIsSecured` method to secure the 4 columns
1. Demonstrate that the application user can no longer retrieve data from the secured columns in the `sample_Example` table.

#### Grant access to secure column data to individuals

1. Use the `Examples.GrantColumnAccess` method to grant the application users read access to specific record field values by creating a [Field Sharing (PrincipalObjectAttributeAccess)](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/principalobjectattributeaccess) record.
1. Demonstrate that the application user can now retrieve data from specific secured record fields in the `sample_Example` table.
1. Demonstrate that the application user is not allowed to write data to the secured columns.
1. Use the `Examples.ModifyColumnAccess` method to grant write access to a specific record field.
1. Demonstrate that the application user is now allowed to write data to the specific record field.
1. Use the `Examples.RevokeColumnAccess` method to delete the `PrincipalObjectAttributeAccess` records that gave the application user access to the secured columns.

#### Manage access to secure column data to groups

1. Add field permissions to the `Example Field Security Profile` record that was created in `Setup` by creating [Field Permission (FieldPermission)](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/fieldpermission) records
1. Demonstrate that the application user can view only the secured columns specified in the field permission records.
1. Demonstrate that the application user is not allowed to write data to the specific record field not enabled with field permissions.

#### Masking

1. Retrieve ID values for existing masking rules. Create new  [Secured Masking Column (AttributeMaskingRule)](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/attributemaskingrule) records to specify masking rules for columns of the `sample_Example` table.
1. Update the `canreadunmasked` column values of the [Field Permission (FieldPermission)](https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/fieldpermission) records created earlier.
1. Wait 30 seconds for the cache to catch up with the new objects created.
1. Demonstrate that the application user can now retrieve data with masked values.
1. Demonstrate that the application user can now retrieve unmasked values with [RetrieveMultipleRequest class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrievemultiplerequest) when using the `UnMaskedData` optional parameter.
1. Demonstrate that the application user can now retrieve unmasked values with [RetrieveRequest class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.messages.retrieverequest) when using the `UnMaskedData` optional parameter.

#### Export solution

Use an exported solution to test the functionality of the sample configurations outside of this sample.

1. Export the solution created with all the configurations as an unmanaged solution.
1. Export the solution created with all the configurations as a managed solution


### Cleanup

When the `SampleSettings.DeleteCreatedObjects` setting in `appsettings.json` is `true`, the `Cleanup` method will try to delete all components created during `Setup` or `Run`. The goal is to return the enviroment to the original state. If you don't want the items to be deleted, you can change the setting to `false`.

> **NOTE**
> The sample will only try to delete records that were created when it ran. If the sample fails due to an error before reaching the clean up phase, those items will not be deleted by the sample the next time it runs. To remove the items, go to the solution named `Column-Level Security Sample Solution` and manually delete the items you find there.

## Sample output

The output of the sample should look something like this:

```console
Column-level security sample started...

Setting up the sample:

        Creating ColumnLevelSecuritySamplePublisher publisher...
        ☑ ColumnLevelSecuritySamplePublisher created.
        Creating ColumnLevelSecuritySampleSolution solution...
        ☑ ColumnLevelSecuritySampleSolution created.
        Creating sample_Example table...
        ☑ sample_Example table created.
        Creating sample_Email column...
        ☑ Created sample_Email column
        Creating sample_GovernmentId column...
        ☑ Created sample_GovernmentId column
        Creating sample_TelephoneNumber column...
        ☑ Created sample_TelephoneNumber column
        Creating sample_DateOfBirth column...
        ☑ Created sample_DateOfBirth column
        ☑ Created 3 sample records.
        Creating Column-level security sample role security role...
        ☑ Column-level security sample role created.
        ☑ Privileges added to Column-level security sample role.
        Associating Column-level security sample role to application user...
        ☑ Associated Column-level security sample role to application user.
        ☑ Verified that the application user has all privileges for the 3 sample records created.
        ☑ Created Example Field Security Profile
        ☑ Associated Example Field Security Profile to application user.

Waited 30 seconds for the cache to update....

Running the sample:

        ☑ Dumped column security information into this file:
        \bin\Debug\net8.0\ColumnSecurityInfo.csv

        These are the secured columns in this environment:
        -account.opendeals
        -account.openrevenue
        -systemuser.isallowedbyipfirewall
        -systemuser.sharepointemailaddress

        Before columns are secured, the application user can see this data:
         -----------------------------------------------------------------------------------------------------
         | Name            | Email                        | Government ID | Telephone Number | Date of Birth |
         -----------------------------------------------------------------------------------------------------
         | Jayden Phillips | jayden@adatum.com            | 166-67-5353   | (736) 555-9012   | 3/25/1974     |
         -----------------------------------------------------------------------------------------------------
         | Benjamin Stuart | benjamin@adventure-works.com | 211-16-7508   | (195) 555-7901   | 6/18/1984     |
         -----------------------------------------------------------------------------------------------------
         | Avery Howard    | avery@alpineskihouse.com     | 346-20-1720   | (152) 555-5591   | 9/4/1994      |
         -----------------------------------------------------------------------------------------------------

        Securing columns...☑ Email,☑ Government ID,☑ Telephone Number, and ☑ Date of Birth.

        After columns are secured, the application user can see this data:
         ------------------------------------------------------------------------------
         | Name            | Email | Government ID | Telephone Number | Date of Birth |
         ------------------------------------------------------------------------------
         | Jayden Phillips |       |               |                  |               |
         ------------------------------------------------------------------------------
         | Benjamin Stuart |       |               |                  |               |
         ------------------------------------------------------------------------------
         | Avery Howard    |       |               |                  |               |
         ------------------------------------------------------------------------------


        After granting access to selected fields, the application user can see this data:
         ------------------------------------------------------------------------------------------
         | Name            | Email             | Government ID | Telephone Number | Date of Birth |
         ------------------------------------------------------------------------------------------
         | Jayden Phillips | jayden@adatum.com |               |                  |               |
         ------------------------------------------------------------------------------------------
         | Benjamin Stuart |                   | 211-16-7508   |                  |               |
         ------------------------------------------------------------------------------------------
         | Avery Howard    |                   |               | (152) 555-5591   |               |
         ------------------------------------------------------------------------------------------

        Demonstrate error when attempting update without update access:
        Try to update the Email column for the Jayden Phillips record
        ☑ Expected error:
        Caller user with Id 11bb11bb-cc22-dd33-ee44-55ff55ff55ff does not have update permissions to a Email secured field on entity Example table. The requested operation could not be completed.

        Demonstrate success when attempting update with update access:
        Grant write access to the email column for Jayden Phillips record:
        Try to update the Email column for the Jayden Phillips record again:
        ☑ Successfully updated record.

        Revoking access to selected fields...
        After access to selected fields is revoked, the application user can not see any data:
        The Example Field Security Profile was created during Setup and the application user was associated with it.
        Add field permissions to the Example Field Security Profile
        New field permissions:
         ------------------------------------------------
         | Column           | Can Read    | Can Update  |
         ------------------------------------------------
         | Email            | Allowed     | Allowed     |
         ------------------------------------------------
         | Government ID    | Not Allowed | Not Allowed |
         ------------------------------------------------
         | Telephone Number | Allowed     | Allowed     |
         ------------------------------------------------
         | Date of Birth    | Allowed     | Not Allowed |
         ------------------------------------------------


Waited 30 seconds for the cache to update....

        The Government ID column now appears null for all rows.
         -----------------------------------------------------------------------------------------------------
         | Name            | Email                        | Government ID | Telephone Number | Date of Birth |
         -----------------------------------------------------------------------------------------------------
         | Jayden Phillips | jaydenp@adatum.com           |               | (736) 555-9012   | 3/25/1974     |
         -----------------------------------------------------------------------------------------------------
         | Benjamin Stuart | benjamin@adventure-works.com |               | (195) 555-7901   | 6/18/1984     |
         -----------------------------------------------------------------------------------------------------
         | Avery Howard    | avery@alpineskihouse.com     |               | (152) 555-5591   | 9/4/1994      |
         -----------------------------------------------------------------------------------------------------

        Attempt to update Date of Birth column data without access:
        ☑ Expected error:
        Caller user with Id 11bb11bb-cc22-dd33-ee44-55ff55ff55ff does not have update permissions to a Date of Birth secured field on entity Example table. The requested operation could not be completed.

        Demonstrate how to enable masking

        This tables shows the fieldpermission changes made to enable displaying masked data.
         -----------------------------------------------------------------
         | Column           | Can Read | Can Read Unmasked | Can Update  |
         -----------------------------------------------------------------
         | Email            | Allowed  | All records       | Allowed     |
         -----------------------------------------------------------------
         | Government ID    | Allowed  | One record        | Not Allowed |
         -----------------------------------------------------------------
         | Telephone Number | Allowed  | Not Allowed       | Allowed     |
         -----------------------------------------------------------------
         | Date of Birth    | Allowed  | All records       | Not Allowed |
         -----------------------------------------------------------------


Waited 30 seconds for the cache to update....

        Now masked values are returned:
         -----------------------------------------------------------------------------------------------------
         | Name            | Email                        | Government ID | Telephone Number | Date of Birth |
         -----------------------------------------------------------------------------------------------------
         | Jayden Phillips | *******@******.com           | ***-**-5353   | (736) 555-9012   | 3/25/****     |
         -----------------------------------------------------------------------------------------------------
         | Benjamin Stuart | ********@***************.com | ***-**-7508   | (195) 555-7901   | 6/18/****     |
         -----------------------------------------------------------------------------------------------------
         | Avery Howard    | *****@**************.com     | ***-**-1720   | (152) 555-5591   | 9/4/****      |
         -----------------------------------------------------------------------------------------------------

        The unmasked values for Email and Date of Birth can be retrieved for all records
         -----------------------------------------------------------------------------------------------------
         | Name            | Email                        | Government ID | Telephone Number | Date of Birth |
         -----------------------------------------------------------------------------------------------------
         | Jayden Phillips | jaydenp@adatum.com           | ***-**-5353   | (736) 555-9012   | 3/25/1974     |
         -----------------------------------------------------------------------------------------------------
         | Benjamin Stuart | benjamin@adventure-works.com | ***-**-7508   | (195) 555-7901   | 6/18/1984     |
         -----------------------------------------------------------------------------------------------------
         | Avery Howard    | avery@alpineskihouse.com     | ***-**-1720   | (152) 555-5591   | 9/4/1994      |
         -----------------------------------------------------------------------------------------------------

        The unmasked values for Government ID can only be retrieved individually:
                -Name: Jayden Phillips  Government ID: 166-67-5353
                -Name: Benjamin Stuart  Government ID: 211-16-7508
                -Name: Avery Howard     Government ID: 346-20-1720

        Exporting unmanaged solution...
        ☑ Exported unmanaged solution to this file:
        \bin\Debug\net8.0\ColumnLevelSecuritySampleSolution.zip


        Exporting managed solution...
        ☑ Exported managed solution to this file:
        \bin\Debug\net8.0\ColumnLevelSecuritySampleSolution_managed.zip


Cleaning up the sample:

        ☑ Column-level security sample role deleted.
        ☑ Example Field Security Profile deleted.
        ☑ sample_Example table deleted.
        ☑ ColumnLevelSecuritySampleSolution deleted.
        ☑ ColumnLevelSecuritySamplePublisher deleted.

Column-level security sample completed.
This sample ran for 4 minutes and 24 seconds
```