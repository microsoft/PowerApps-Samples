<#
.SYNOPSIS
    Helper functions for column-level security sample demonstrations in Power Platform.

.DESCRIPTION
    This module contains helper functions that support column-level security demonstrations
    in Microsoft Dataverse. It provides functionality for:
    
    - User authentication and context switching between system admin and service principal
    - Creating and managing publishers, solutions, tables, and columns
    - Managing sample data for security demonstrations
    - Setting up security roles and field security profiles
    - Retrieving and displaying data with and without field-level security masking
    - Utility functions for cache management and timing

.NOTES
    File Name    : Helpers.ps1
    Author       : Power Platform Code Sample Development
    Requires     : PowerShell 5.1+, Power Platform modules
    Dependencies : Core.ps1, CommonFunctions.ps1, MetadataOperations.ps1, TableOperations.ps1, Examples.ps1
    
    Global Constants:
    - CUSTOMIZATION_PREFIX: 'sample'
    - TABLE_NAME: 'Example'
    - PUBLISHER_UNIQUE_NAME: 'ColumnLevelSecuritySamplePublisher'
    - SOLUTION_UNIQUE_NAME: 'ColumnLevelSecuritySampleSolution'
    - ROLE_NAME: 'Column-level security sample role'
    - FIELD_SECURITY_PROFILE_NAME: 'Example Field Security Profile'
    
    Global Variables:
    - systemAdminContext, secondUserContext: Azure authentication contexts
    - systemAdminData, secondUserData, secondUserId: User identity information
    - entityStore: Hashtable for storing created entity IDs
    - settings: Configuration object with environment variables
#>

# Common scripts that all PowerShell samples use
. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\CommonFunctions.ps1
. $PSScriptRoot\..\MetadataOperations.ps1
. $PSScriptRoot\..\TableOperations.ps1
# Reusable functions that only depend on the current logged on user.
. $PSScriptRoot\Examples.ps1

$CUSTOMIZATION_PREFIX = 'sample'
$TABLE_NAME = 'Example'
$TABLE_SCHEMA_NAME = "${CUSTOMIZATION_PREFIX}_${TABLE_NAME}"
$TABLE_LOGICAL_NAME = $TABLE_SCHEMA_NAME.ToLower()
$TABLE_PRIMARY_KEY = "${TABLE_LOGICAL_NAME}id"
$TABLE_SET_NAME = "${CUSTOMIZATION_PREFIX}_examples"
$PUBLISHER_UNIQUE_NAME = "ColumnLevelSecuritySamplePublisher" 
$SOLUTION_UNIQUE_NAME = "ColumnLevelSecuritySampleSolution" 
$ROLE_NAME = "Column-level security sample role"
$FIELD_SECURITY_PROFILE_NAME = "Example Field Security Profile" 
$DELETE_CREATED_OBJECTS = $true
$global:systemAdminContext = $null
$global:secondUserContext = $null
$global:systemAdminData = $null
$global:secondUserData = $null
$global:secondUserId = $null
$global:entityStore = @{}


<#
.SYNOPSIS
    Initializes user connections for both system admin and service principal users.

.DESCRIPTION
    This function loads environment variables from a .env file and establishes authenticated 
    connections to both Azure and Dataverse for two different user contexts:
    1. A service principal (second user) for limited operations
    2. A system administrator for full administrative operations
    
    The function caches the authentication contexts globally for later user switching.

.EXAMPLE
    Initialize-Users-Helper
    
    Loads credentials from .env file and establishes connections for both users.

.NOTES
    - Requires a .env file in the script directory with authentication details
    - Sets global variables for user contexts and data
    - Exits with system admin user connected to Dataverse
#>
function Initialize-Users-Helper {

   # Load the environment variables from the .env file
   $envFilePath = Join-Path -Path $PSScriptRoot -ChildPath ".env"
   if (-Not (Test-Path -Path $envFilePath)) {

      throw "The .env file does not exist at the specified path: $envFilePath"
   }
   $envFileContent = Get-Content -Path $envFilePath
   $envFileContent | ForEach-Object {
      $key, $value = $_ -split '='
      [System.Environment]::SetEnvironmentVariable($key, $value, 'Process')
   }

   $global:settings = [PSCustomObject]@{
      Url                  = [System.Environment]::GetEnvironmentVariable('BASE_URL')
      ClientId             = [System.Environment]::GetEnvironmentVariable('CLIENT_ID')
      ClientSecret         = [System.Environment]::GetEnvironmentVariable('CLIENT_SECRET')
      TenantId             = [System.Environment]::GetEnvironmentVariable('TENANT_ID')
      SysAdminUser         = [System.Environment]::GetEnvironmentVariable('SYS_ADMIN_USER')
      SysAdminUserPassword = [System.Environment]::GetEnvironmentVariable('SYS_ADMIN_PASSWORD')
   }

   # Connect as the second user
   $secureClientSecret = ConvertTo-SecureString $settings.ClientSecret -AsPlainText -Force
   $Credential = New-Object System.Management.Automation.PSCredential($settings.ClientId, $secureClientSecret)
   # Connect to Azure using the service principal
   Connect-AzAccount -ServicePrincipal -TenantId $settings.TenantId -Credential $Credential | Out-Null
   # Cache the context of the second user
   $global:secondUserContext = Get-AzContext

   # Connect to Dataverse using the service principal
   Connect -uri $settings.Url
   # Cache the data of the second user
   $global:secondUserData = Get-WhoAmI
   $global:secondUserId = $secondUserData.UserId

   # Connect as the system admin user

   if ( -not $settings.SysAdminUser -or -not $settings.SysAdminUserPassword) {
      # System admin user credentials are not set in the .env file.
      # Browser window will open to authenticate the system admin user
      Connect-AzAccount | Out-Null
   }
   else {

      # "System admin user credentials are set in the .env file."
      $secureSysAdminPassword = ConvertTo-SecureString $settings.SysAdminUserPassword -AsPlainText -Force
      $SysAdminCredential = New-Object System.Management.Automation.PSCredential($settings.SysAdminUser, $secureSysAdminPassword)
      # Connect to Azure using the system admin user credentials stored in the .env file.
      Connect-AzAccount -Credential $SysAdminCredential | Out-Null
   }
   # Cache the context of the system admin user
   $global:systemAdminContext = Get-AzContext

   # Connect to Dataverse using the system admin user
   Connect -uri $settings.Url
   # Cache the data of the system admin user
   $global:systemAdminData = Get-WhoAmI

   # The function exits with the system admin user connected to Dataverse

}

<#
.SYNOPSIS
    Switches between different user contexts for Dataverse operations.

.DESCRIPTION
    This function allows switching between pre-authenticated user contexts (system admin 
    or service principal) and reconnects to the Dataverse environment with the selected 
    user's credentials.

.PARAMETER user
    The user context to switch to. Valid values are "systemAdmin" or "secondUser".

.EXAMPLE
    Switch-User-Helper -user "systemAdmin"
    
    Switches to the system administrator context.

.EXAMPLE
    Switch-User-Helper -user "secondUser"
    
    Switches to the service principal (second user) context.

.NOTES
    - Requires Initialize-Users-Helper to be called first to cache user contexts
    - Automatically reconnects to Dataverse after switching contexts
#>
function Switch-User-Helper {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $user
   )

   if ($user -eq "systemAdmin") {
      # Set the user context to the system admin account
      Set-AzContext -Context $global:systemAdminContext | Out-Null
   } 
   elseif ($user -eq "secondUser") {
      # Set the user context to the second user account
      Set-AzContext -Context $global:secondUserContext | Out-Null
   }

   # Reconnect to the Dataverse environment
   Connect $settings.Url
}

<#
.SYNOPSIS
    Retrieves the ID of a record based on a unique identifier column value.

.DESCRIPTION
    This function searches for a record in a specified entity set using a column value 
    and returns the record's primary key ID if found. It uses OData query syntax to 
    filter and select specific data.

.PARAMETER entitySetName
    The name of the entity set to search in (e.g., "publishers", "solutions").

.PARAMETER columnLogicalName
    The logical name of the column to filter by.

.PARAMETER uniqueStringValue
    The unique value to search for in the specified column.

.PARAMETER primaryKeyLogicalName
    The logical name of the primary key column to return.

.OUTPUTS
    System.Guid
    Returns the GUID of the found record, or $null if no record is found.

.EXAMPLE
    $publisherId = Get-RecordId-Helper -entitySetName "publishers" -columnLogicalName "uniquename" -uniqueStringValue "MyPublisher" -primaryKeyLogicalName "publisherid"
    
    Searches for a publisher with the unique name "MyPublisher" and returns its ID.

.NOTES
    - Uses strong consistency for reliable data retrieval
    - Returns null if no matching record is found
    - Throws an exception if the query fails
#>
function Get-RecordId-Helper {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $entitySetName,
      [Parameter(Mandatory)] 
      [String] 
      $columnLogicalName,
      [String] 
      $uniqueStringValue,
      [String] 
      $primaryKeyLogicalName
   )

   $records = $null;

   $query = @();
   $query += '$select={0}' -f $primaryKeyLogicalName
   $query += '$filter={0} eq ''{1}''' -f $columnLogicalName, $uniqueStringValue
   $queryString = $query -join "&"
   $queryString = "?" + $queryString


   try {
      $getRecordsParams = @{
         setName           = $entitySetName
         query             = $queryString
         strongConsistency = $true
      }

      $records = Get-Records @getRecordsParams

   }
   catch {
      throw "Failed to retrieve records from ${$entitySetName}: $_.Exception.Message"
   }

   if ($records.value.length -eq 0) {
      return $null
   }
   else {
      $record = $records.value[0]
      $recordId = $record.$primaryKeyLogicalName
      return [System.Guid]::Parse($recordId)
   }
}

<#
.SYNOPSIS
    Creates a new publisher or returns the ID of an existing one.

.DESCRIPTION
    This function checks if a publisher with the predefined unique name already exists.
    If it doesn't exist, creates a new publisher with the specified customization prefix
    and other required properties for the column-level security sample.

.OUTPUTS
    System.Guid
    Returns the GUID of the publisher (either existing or newly created).

.EXAMPLE
    $publisherId = New-Publisher-Helper
    
    Creates the sample publisher or returns the existing one's ID.

.NOTES
    - Uses global constant PUBLISHER_UNIQUE_NAME for the publisher's unique name
    - Sets customization prefix and option value prefix for the sample
    - Stores the publisher ID in the global entityStore hashtable
    - Displays colored output messages for user feedback
#>
function New-Publisher-Helper {

   $getPublisherParams = @{
      entitySetName         = "publishers"
      columnLogicalName     = "uniquename"
      uniqueStringValue     = $PUBLISHER_UNIQUE_NAME
      primaryKeyLogicalName = "publisherid"
   }

   $publisherId = Get-RecordId-Helper @getPublisherParams

   if ($publisherId -ne $null) {
      Write-Host "`t☑ Publisher $PUBLISHER_UNIQUE_NAME already exists." -ForegroundColor Yellow
      return $publisherId
   }
   else {

      Write-Host "`tCreating $PUBLISHER_UNIQUE_NAME publisher..."
      # Create a new publisher
      $publisher = @{
         uniquename                     = $PUBLISHER_UNIQUE_NAME
         friendlyname                   = "Column-level security sample publisher"
         customizationprefix            = $CUSTOMIZATION_PREFIX
         customizationoptionvalueprefix = 72700
         description                    = "This publisher was created from sample code"
      }

      try {

         $newRecordParams = @{
            setName = "publishers"
            body    = $publisher
         }

         $publisherId = New-Record @newRecordParams

         $entityStore.$PUBLISHER_UNIQUE_NAME = $publisherId
         Write-Host "`t☑ $PUBLISHER_UNIQUE_NAME publisher created."
         return $publisherId
      }
      catch {
         throw "Failed to create publisher: $_.Exception.Message"
      }
   }
}

<#
.SYNOPSIS
    Creates a new solution or verifies an existing one.

.DESCRIPTION
    This function checks if a solution with the predefined unique name already exists.
    If it doesn't exist, creates a new solution associated with the specified publisher
    for the column-level security sample.

.PARAMETER publisherId
    The GUID of the publisher to associate with the solution.

.EXAMPLE
    New-Solution-Helper -publisherId $publisherId
    
    Creates the sample solution or verifies the existing one.

.NOTES
    - Uses global constant SOLUTION_UNIQUE_NAME for the solution's unique name
    - Associates the solution with the specified publisher
    - Sets the solution version to 1.0.0.0
    - Stores the solution ID in the global entityStore hashtable
    - Displays colored output messages for user feedback
#>
function New-Solution-Helper {
   param (
      [String] 
      $publisherId
   )

   # Check if the solution already exists

   $getRecordParams = @{
      entitySetName         = "solutions"
      columnLogicalName     = "uniquename"
      uniqueStringValue     = $SOLUTION_UNIQUE_NAME
      primaryKeyLogicalName = "solutionid"
   }

   $solutionId = Get-RecordId-Helper @getRecordParams

   if ($solutionId -ne $null) {
      Write-Host "`t☑ Solution $SOLUTION_UNIQUE_NAME already exists." -ForegroundColor Yellow

   }
   else {

      Write-Host "`tCreating $SOLUTION_UNIQUE_NAME solution..."

      # Create a new solution
      $solution = @{
         uniquename               = $SOLUTION_UNIQUE_NAME
         friendlyname             = "Column-Level Security Sample Solution"
         'publisherid@odata.bind' = '/publishers(' + $publisherId + ')'
         version                  = "1.0.0.0"
      }

      try {
         $newRecordParams = @{
            setName = "solutions"
            body    = $solution
         }

         $solutionId = New-Record @newRecordParams

         $entityStore.$SOLUTION_UNIQUE_NAME = $solutionId
         Write-Host "`t☑ $SOLUTION_UNIQUE_NAME solution created."
      }
      catch {
         throw "Failed to create solution: $_.Exception.Message"
      }
   }
}

<#
.SYNOPSIS
    Creates a new sample table for demonstrating column-level security.

.DESCRIPTION
    This function checks if the sample table already exists and creates it if it doesn't.
    The table includes a primary name attribute and is configured for user ownership.
    It's designed specifically for column-level security demonstrations.

.PARAMETER solutionId
    The GUID of the solution to add the table to.

.EXAMPLE
    New-SampleTable-Helper -solutionId $solutionId
    
    Creates the sample table for column-level security demonstrations.

.NOTES
    - Uses global constants for table naming and schema
    - Creates a UserOwned table with activities and notes disabled
    - Includes a primary name attribute with 100 character limit
    - Adds the table to the specified solution
    - Displays colored output messages for user feedback
#>
function New-SampleTable-Helper {
   param (
      [String] 
      $solutionId
   )   # Check if the table already exists
   $getRecordParams = @{
      entitySetName         = "EntityDefinitions"
      columnLogicalName     = "LogicalName"
      uniqueStringValue     = $TABLE_LOGICAL_NAME
      primaryKeyLogicalName = "MetadataId"
   }
   $tableId = Get-RecordId-Helper @getRecordParams

   if ($tableId -ne $null) {

      Write-Host "`t☑ Table $TABLE_SCHEMA_NAME already exists." -ForegroundColor Yellow
   }
   else {
      Write-Host "`tCreating $TABLE_SCHEMA_NAME table..."

      # Create a new table
      $table = @{
         '@odata.type'         = "Microsoft.Dynamics.CRM.EntityMetadata"
         SchemaName            = $TABLE_SCHEMA_NAME
         DisplayName           = @{
            '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
            LocalizedLabels = @(
               @{
                  '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                  Label         = "Example table"
                  LanguageCode  = 1033 # English (United States)
               }
            )
         }
         DisplayCollectionName = @{
            '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
            LocalizedLabels = @(
               @{
                  '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                  Label         = "Example table records"
                  LanguageCode  = 1033 # English (United States)
               }
            )
         }
         Description           = @{
            '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
            LocalizedLabels = @(
               @{
                  '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                  Label         = "This is an example table created from sample code."
                  LanguageCode  = 1033 # English (United States)
               }
            )
         }
         HasActivities         = $false
         HasNotes              = $false
         OwnershipType         = 'UserOwned'
         PrimaryNameAttribute  = $($CUSTOMIZATION_PREFIX + '_name')
         Attributes            = @(
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.StringAttributeMetadata'
               IsPrimaryName = $true
               SchemaName    = $($CUSTOMIZATION_PREFIX + '_Name')
               RequiredLevel = @{
                  Value = 'None'
               }
               DisplayName   = @{
                  '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
                  LocalizedLabels = @(
                     @{
                        '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                        Label         = 'Name'
                        LanguageCode  = 1033 # English (United States)
                     }
                  )
               }
               Description   = @{
                  '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
                  LocalizedLabels = @(
                     @{
                        '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                        Label         = "The primary attribute for the $TABLE_SCHEMA_NAME table"
                        LanguageCode  = 1033 # English (United States)
                     }
                  )
               }
               MaxLength     = 100
            }
         )
      }

      try {

         $newTableParams = @{
            body               = $table
            solutionUniqueName = $SOLUTION_UNIQUE_NAME
         }

         $tableId = New-Table @newTableParams


         Write-Host "`t☑ $TABLE_SCHEMA_NAME table created."
      }
      catch {
         throw "Failed to create $TABLE_SCHEMA_NAME table: $_.Exception.Message"
      } 
   }
}

<#
.SYNOPSIS
    Creates a new string column in the sample table.

.DESCRIPTION
    This function checks if a column with the specified schema name already exists in 
    the sample table. If it doesn't exist, creates a new string column with the provided
    display name and description. If it exists, ensures the column is not secured initially.

.PARAMETER schemaName
    The schema name for the new column (e.g., "sample_Email").

.PARAMETER displayName
    The display name for the column that users will see.

.PARAMETER description
    A description explaining the purpose of the column.

.EXAMPLE
    New-SampleColumn-Helper -schemaName "sample_Email" -displayName "Email Address" -description "The user's email address"
    
    Creates a new email column in the sample table.

.NOTES
    - Creates string columns with a maximum length of 100 characters
    - Sets required level to None by default
    - Ensures existing columns are not secured at the start of the sample
    - Adds the column to the predefined solution
    - Uses English (United States) language code 1033 for labels
#>
function New-SampleColumn-Helper {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $schemaName,
      [Parameter(Mandatory)] 
      [String] 
      $displayName,
      [Parameter(Mandatory)] 
      [String] 
      $description
   )   # Check if the column already exists
   $getColumnParams = @{
      entitySetName         = "EntityDefinitions(LogicalName='${TABLE_LOGICAL_NAME}')/Attributes"
      columnLogicalName     = "SchemaName"
      uniqueStringValue     = $schemaName
      primaryKeyLogicalName = "MetadataId"
   }
   $columnId = Get-RecordId-Helper @getColumnParams

   if ($columnId -ne $null) {
      Write-Host "`t☑ Column $schemaName already exists." -ForegroundColor Yellow

      # Make sure the column is not secured at the start of the sample
      $setColumnSecuredParams = @{
         tableLogicalName   = $TABLE_LOGICAL_NAME
         logicalName        = $schemaName.ToLower()
         type               = 'String'
         value              = $false
         solutionUniqueName = $SOLUTION_UNIQUE_NAME
      }
      Set-ColumnIsSecured-Example @setColumnSecuredParams
   }
   else {
      Write-Host "`tCreating $schemaName column..."

      # Create a new column
      $column = @{
         '@odata.type' = 'Microsoft.Dynamics.CRM.StringAttributeMetadata'
         SchemaName    = $schemaName
         DisplayName   = @{
            '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
            LocalizedLabels = @(
               @{
                  '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                  Label         = $displayName
                  LanguageCode  = 1033 # English (United States)
               }
            )
         }
         Description   = @{
            '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
            LocalizedLabels = @(
               @{
                  '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                  Label         = $description
                  LanguageCode  = 1033 # English (United States)
               }
            )
         }
         RequiredLevel = @{
            Value = 'None'
         }
         MaxLength     = 100
      }
      
      try {
         $newColumnParams = @{
            tableLogicalName   = $TABLE_LOGICAL_NAME
            column             = $column
            solutionUniqueName = $SOLUTION_UNIQUE_NAME
         }
         New-Column @newColumnParams | Out-Null

         Write-Host "`t☑ $schemaName column created." 
      }
      catch {
         throw "Failed to create column ${$schemaName}: $_.Exception.Message"
      }
   }
}

<#
.SYNOPSIS
    Manages sample data in the example table for column-level security demonstrations.

.DESCRIPTION
    This function removes any existing records from the sample table and creates fresh 
    sample data with three example records. Each record contains personal information 
    including names, emails, government IDs, phone numbers, and dates of birth.

.EXAMPLE
    Manage-SampleData-Helper
    
    Clears existing data and creates three sample records.

.NOTES
    - Removes all existing records before creating new ones
    - Creates three predefined sample records with realistic data
    - Uses strong consistency for reliable data operations
    - Sample data includes sensitive information suitable for security demonstrations
    - All records are created in a single batch operation
#>
function Manage-SampleData-Helper {   
   # Remove any existing records in the sample_example table
   $getRecordsParams = @{
      setName           = $TABLE_SET_NAME
      query             = '?$select=sample_exampleid'
      strongConsistency = $true
   }
   $existingRecords = Get-Records @getRecordsParams

   foreach ($record in $existingRecords.value) {
      $recordId = $record.sample_exampleid
      $removeRecordParams = @{
         setName           = $TABLE_SET_NAME
         id                = $recordId
         strongConsistency = $true
      }
      Remove-Record @removeRecordParams | Out-Null
   } 
   
   # Add sample data to the sample_example table
   $records = @(
      @{
         '@odata.type'                             = "Microsoft.Dynamics.CRM.$TABLE_LOGICAL_NAME"
         "${CUSTOMIZATION_PREFIX}_name"            = 'Jayden Phillips'
         "${CUSTOMIZATION_PREFIX}_email"           = 'jayden@adatum.com' 
         "${CUSTOMIZATION_PREFIX}_governmentid"    = '166-67-5353'
         "${CUSTOMIZATION_PREFIX}_telephonenumber" = '(736) 555-9012'
         "${CUSTOMIZATION_PREFIX}_dateofbirth"     = '3/25/1974'
      },
      @{
         '@odata.type'                             = "Microsoft.Dynamics.CRM.$TABLE_LOGICAL_NAME"
         "${CUSTOMIZATION_PREFIX}_name"            = 'Benjamin Stuart'
         "${CUSTOMIZATION_PREFIX}_email"           = 'benjamin@adventure-works.com' 
         "${CUSTOMIZATION_PREFIX}_governmentid"    = '211-16-7508'
         "${CUSTOMIZATION_PREFIX}_telephonenumber" = '(195) 555-7901'
         "${CUSTOMIZATION_PREFIX}_dateofbirth"     = '6/18/1984'
      },
      @{
         '@odata.type'                             = "Microsoft.Dynamics.CRM.$TABLE_LOGICAL_NAME"
         "${CUSTOMIZATION_PREFIX}_name"            = 'Avery Howard'
         "${CUSTOMIZATION_PREFIX}_email"           = 'avery@alpineskihouse.com' 
         "${CUSTOMIZATION_PREFIX}_governmentid"    = '346-20-1720'
         "${CUSTOMIZATION_PREFIX}_telephonenumber" = '(152) 555-5591'
         "${CUSTOMIZATION_PREFIX}_dateofbirth"     = '9/4/1994'
      }
   )
   try {
      $createMultipleRecordsParams = @{
         setName           = $TABLE_SET_NAME
         targets           = $records
         strongConsistency = $true
      }
      Create-MultipleRecords @createMultipleRecordsParams | Out-Null

      Write-Host "`t☑ Created 3 sample records."
   }
   catch {
      throw "Failed to create sample data: $_.Exception.Message"
   }

}

<#
.SYNOPSIS
    Creates a new security role with privileges for the sample table.

.DESCRIPTION
    This function checks if the sample security role already exists and creates it if needed.
    The role is configured with full privileges for the sample table and is associated with
    the second user's business unit.

.OUTPUTS
    System.Guid
    Returns the GUID of the security role (either existing or newly created).

.EXAMPLE
    $roleId = New-SecurityRole-Helper
    
    Creates the sample security role or returns the existing one's ID.

.NOTES
    - Creates role in the second user's business unit for proper scope
    - Grants full privileges (Create, Read, Write, Delete, Share, Append, AppendTo, Assign)
    - Uses global privilege depth for maximum access
    - Stores the role ID in the global entityStore hashtable
    - Some output messages are commented out for cleaner execution
#>
function New-SecurityRole-Helper {

   # Check if the security role already exists
   $getRoleParams = @{
      entitySetName         = 'roles'
      columnLogicalName     = 'name'
      uniqueStringValue     = $ROLE_NAME
      primaryKeyLogicalName = 'roleid'
   }
   $roleId = Get-RecordId-Helper @getRoleParams

   if ($null -ne $roleId) {

      Write-Host "`t☑ Security role '$ROLE_NAME' already exists." -ForegroundColor Yellow
   }
   else {
      Write-Host "`tCreating '$ROLE_NAME' security role..."

      # Create a new security role
      $role = @{
         '@odata.type'               = 'Microsoft.Dynamics.CRM.role'
         name                        = $ROLE_NAME
         description                 = 'This role was created from sample code'
         'businessunitid@odata.bind' = "/businessunits($($secondUserData.BusinessUnitId))"
      }
         
      try {
         $newRecordParams = @{
            setName            = 'roles'
            body               = $role
            solutionUniqueName = $SOLUTION_UNIQUE_NAME
         }
         $newRoleId = New-Record @newRecordParams

         $entityStore.$ROLE_NAME = $newRoleId

         Write-Host "`t☑ $ROLE_NAME created."

         $roleId = $newRoleId
         
      }
      catch {
         throw "Failed to create security role: $_.Exception.Message"
      }
      
      $privileges = @(
         "prvAppendTo$TABLE_SCHEMA_NAME",
         "prvAppend$TABLE_SCHEMA_NAME",
         "prvCreate$TABLE_SCHEMA_NAME",
         "prvRead$TABLE_SCHEMA_NAME",
         "prvShare$TABLE_SCHEMA_NAME",
         "prvDelete$TABLE_SCHEMA_NAME",
         "prvAssign$TABLE_SCHEMA_NAME",
         "prvWrite$TABLE_SCHEMA_NAME"
      )      
      
      try {
         $addPrivilegesParams = @{
            roleId         = $roleId
            depth          = ([PrivilegeDepth]::Global)
            privilegeNames = $privileges
         }
         Add-PrivilegesToRole-Example @addPrivilegesParams | Out-Null

         Write-Host "`t☑ Privileges added to '$ROLE_NAME'."

      }
      catch {
         throw "Failed to add privileges to security role: $_.Exception.Message"
      }
   }

   return [guid]$roleId
   
}

<#
.SYNOPSIS
    Manages security role assignment for the application user.

.DESCRIPTION
    This function checks if the second user (service principal) already has the sample 
    security role assigned. If not, it assigns the role to enable proper access to the 
    sample table for column-level security demonstrations.

.PARAMETER roleId
    The GUID of the security role to assign to the user.

.EXAMPLE
    Manage-RoleForAppUser-Helper -roleId $roleId
    
    Assigns the sample security role to the application user if not already assigned.

.NOTES
    - Uses the global second user data for role assignment
    - Checks existing role assignments before adding new ones
    - Uses role name for assignment operations
    - Displays colored output messages for user feedback
#>
function Manage-RoleForAppUser-Helper {
   param (
      [Parameter(Mandatory)] 
      [guid] 
      $roleId
   )   # Check if the user already has the role
   $getUserHasRoleParams = @{
      userid = $global:secondUserData.UserId
      roleid = $roleId
   }
   $userHasRole = Get-UserHasRole-Example @getUserHasRoleParams

   if ($userHasRole) {
      Write-Host "`t☑ User already has the role." -ForegroundColor Yellow
   }
   else {
      Write-Host "`tAssociating $ROLE_NAME to application user..."
      
      try {
         $addRoleParams = @{
            userid   = $global:secondUserData.UserId
            rolename = $ROLE_NAME
         }
         Add-RoleToUserByName-Example @addRoleParams | Out-Null

         Write-Host "`t☑ Associated $ROLE_NAME to application user"
      }
      catch {
         throw "Failed to add role to user: $_.Exception.Message"
      }
   }
}

<#
.SYNOPSIS
    Associates a field security profile with an application user.

.DESCRIPTION
    This function checks if a field security profile is already associated with the 
    specified application user. If not, it creates the association to enable field-level 
    security permissions for the user.

.PARAMETER fieldSecurityProfileId
    The GUID of the field security profile to associate.

.PARAMETER appUserId
    The GUID of the application user to associate with the profile.

.PARAMETER fieldSecurityProfileName
    The display name of the field security profile for user feedback messages.

.EXAMPLE
    Manage-FieldSecurityProfileForAppUser-Helper -fieldSecurityProfileId $profileId -appUserId $userId -fieldSecurityProfileName "Example Profile"
    
    Associates the field security profile with the specified user.

.NOTES
    - Queries existing associations before creating new ones
    - Uses strong consistency for reliable association checks
    - Provides detailed feedback with checkmark symbols for successful operations
    - Uses the systemuserprofiles_association relationship for the connection
#>
function Manage-FieldSecurityProfileForAppUser-Helper {
   param (
      [Parameter(Mandatory)] 
      [guid] 
      $fieldSecurityProfileId,
      [Parameter(Mandatory)] 
      [guid] 
      $appUserId,
      [Parameter(Mandatory)] 
      [string] 
      $fieldSecurityProfileName
   )

   # Detect whether the field security profile is already associated with the app user

   $getRecordsParams = @{
      setName           = "systemusers($($appUserId))/systemuserprofiles_association"
      query             = "?`$filter=fieldsecurityprofileid eq $($fieldSecurityProfileId)&`$select=fieldsecurityprofileid"
      strongConsistency = $true
   }

   $records = Get-Records @getRecordsParams

   if ($records.value.length -gt 0) {
      Write-Host "`t☑ Application user is already associated with the '$fieldSecurityProfileName'." -ForegroundColor Yellow
   }
   else {
      Write-Host "`tAssociating field security profile '$fieldSecurityProfileName' with app user..."
      
      try {
         $addToCollectionParams = @{
            targetSetName     = "systemusers"
            targetId          = $appUserId
            collectionName    = "systemuserprofiles_association"
            setName           = "fieldsecurityprofiles"
            id                = $fieldSecurityProfileId
            strongConsistency = $true
         }
         Add-ToCollection @addToCollectionParams | Out-Null

         Write-Host "`t☑ Associated '$fieldSecurityProfileName' to application user."
      }
      catch {
         throw "Failed to associate field security profile with app user: $_.Exception.Message"
      }
   }
}

<#
.SYNOPSIS
    Retrieves all records from the sample table with standard field visibility.

.DESCRIPTION
    This function queries the sample table and returns all records with their key fields.
    The data returned will respect field-level security settings, potentially masking 
    sensitive information based on the current user's permissions.

.OUTPUTS
    Array
    Returns an array of records with sample data fields.

.EXAMPLE
    $records = Get-ExampleRows-Helper
    
    Retrieves all sample records with potential field masking applied.

.NOTES
    - Uses strong consistency for reliable data retrieval
    - Orders results by name in descending order
    - Selects specific fields: name, email, government ID, telephone, and date of birth
    - Respects field-level security settings for the current user context
#>
function Get-ExampleRows-Helper {

   # Retrieve the records from the sample_example table
   $getRecordsParams = @{
      setName           = $TABLE_SET_NAME
      query             = '?$select=sample_name,sample_email,sample_governmentid,sample_telephonenumber,sample_dateofbirth&$orderby=sample_name desc'
      strongConsistency = $true
   }
   $records = Get-Records @getRecordsParams

   return $records.value
}

<#
.SYNOPSIS
    Retrieves all records from the sample table with unmasked field values.

.DESCRIPTION
    This function queries the sample table and returns all records with their actual 
    field values, bypassing field-level security masking by using the UnMaskedData 
    parameter. This is useful for comparing masked vs. unmasked data.

.OUTPUTS
    Array
    Returns an array of records with unmasked sample data fields.

.EXAMPLE
    $unmaskedRecords = Get-ExampleRows-Unmasked-Helper
    
    Retrieves all sample records with actual (unmasked) field values.

.NOTES
    - Uses the UnMaskedData=true parameter to bypass field-level security
    - Orders results by name in descending order
    - Selects the same fields as the standard helper for comparison purposes
    - Requires appropriate permissions to access unmasked data
    - Uses strong consistency and OData annotations for complete data retrieval
#>
function Get-ExampleRows-Unmasked-Helper {

   $columns = @(
      'sample_name',
      'sample_email',
      'sample_governmentid',
      'sample_telephonenumber',
      'sample_dateofbirth'
   )
   $select = "`$select=" + ($columns -join ',')
   $orderby = '$orderby=sample_name desc'
   $unmaskedData = 'UnMaskedData=true'

   $query = '?' + $select + '&' + $orderby + '&' + $unmaskedData

   $getRecordParams = @{
      setName           = $TABLE_SET_NAME
      query             = $query
      strongConsistency = $true
   }

   $records = Get-Records @getRecordParams

   return $records.value
}

<#
.SYNOPSIS
    Displays sample table records in a formatted table with standard field visibility.

.DESCRIPTION
    This function retrieves sample records using Get-ExampleRows-Helper and formats them 
    into a readable table with user-friendly column headers. The displayed data will 
    respect field-level security settings and may show masked values.

.EXAMPLE
    Write-ExampleRows-Helper
    
    Displays the sample records in a formatted table with potential field masking.

.NOTES
    - Creates user-friendly column headers (Name, Email, Government ID, etc.)
    - Uses Format-Table with AutoSize and Wrap for optimal display
    - Shows data as it appears to the current user (may include masked values)
    - Useful for demonstrating the effect of field-level security settings
#>
function Write-ExampleRows-Helper {
   # Retrieve the records from the sample_example table
   $data = Get-ExampleRows-Helper

   $data | Select-Object @{Name = 'Name'; Expression = { $_.sample_name } },
   @{Name = 'Email'; Expression = { $_.sample_email } },
   @{Name = 'Government ID'; Expression = { $_.sample_governmentid } },
   @{Name = 'Telephone Number'; Expression = { $_.sample_telephonenumber } },
   @{Name = 'Date of Birth'; Expression = { $_.sample_dateofbirth } }
   | Format-Table -AutoSize -Wrap
   
}

<#
.SYNOPSIS
    Displays sample table records in a formatted table with unmasked field values.

.DESCRIPTION
    This function retrieves unmasked sample records using Get-ExampleRows-Unmasked-Helper 
    and formats them into a readable table with user-friendly column headers. The displayed 
    data shows actual field values, bypassing field-level security masking.

.EXAMPLE
    Write-ExampleRows-Unmasked-Helper
    
    Displays the sample records in a formatted table with actual (unmasked) field values.

.NOTES
    - Creates the same user-friendly column headers as the standard helper
    - Uses Format-Table with AutoSize and Wrap for optimal display
    - Shows actual data values regardless of field-level security settings
    - Useful for comparing with masked data to demonstrate security effects
    - Requires appropriate permissions to access unmasked data
#>
function Write-ExampleRows-Unmasked-Helper {
   # Retrieve the records from the sample_example table
   $data = Get-ExampleRows-Unmasked-Helper

   $data | Select-Object @{Name = 'Name'; Expression = { $_.sample_name } },
   @{Name = 'Email'; Expression = { $_.sample_email } },
   @{Name = 'Government ID'; Expression = { $_.sample_governmentid } },
   @{Name = 'Telephone Number'; Expression = { $_.sample_telephonenumber } },
   @{Name = 'Date of Birth'; Expression = { $_.sample_dateofbirth } }
   | Format-Table -AutoSize -Wrap
   
}

<#
.SYNOPSIS
    Waits for a specified number of seconds to allow cache updates to propagate.

.DESCRIPTION
    This function provides a visual countdown timer to wait for Dataverse cache updates 
    to propagate. This is particularly important after making metadata changes like 
    enabling field-level security, as these changes need time to take effect.

.PARAMETER waitSeconds
    The number of seconds to wait. Defaults to 30 seconds if not specified.

.EXAMPLE
    Wait-ForCacheUpdate-Helper
    
    Waits for the default 30 seconds with a countdown display.

.EXAMPLE
    Wait-ForCacheUpdate-Helper -waitSeconds 60
    
    Waits for 60 seconds with a countdown display.

.NOTES
    - Displays a real-time countdown with carriage return for dynamic updates
    - Essential after metadata changes in Dataverse
    - Provides user feedback during wait periods
    - Uses Start-Sleep for accurate timing
#>
function Wait-ForCacheUpdate-Helper {
   param (
      [int] 
      $waitSeconds = 30
   )

   Write-Host ""
   for ($i = $waitSeconds; $i -ge 0; $i--) {
      Write-Host "`rWaiting $i seconds for the cache to update..." -NoNewline
      Start-Sleep -Seconds 1
   }
   Write-Host "`rWaited $waitSeconds seconds for the cache to update." -NoNewline
   Write-Host ""
}



