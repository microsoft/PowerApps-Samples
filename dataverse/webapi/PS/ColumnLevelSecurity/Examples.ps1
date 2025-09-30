. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\CommonFunctions.ps1

# <SetColumnIsSecuredExample>
<#
.SYNOPSIS
    Sets the IsSecured property of a specified column in a Dataverse table.

.DESCRIPTION
    This function retrieves a column definition from a Dataverse table and updates its IsSecured 
    property.
    If the column is already set to the desired value, no action is taken.

.PARAMETER tableLogicalName
    The logical name of the table containing the column.

.PARAMETER logicalName
    The logical name of the column to update.

.PARAMETER type
    The type of the column (e.g., String, Integer, DateTime).

.PARAMETER value
    The boolean value to set for the IsSecured property.

.PARAMETER solutionUniqueName
    The unique name of the solution where the column update should be tracked.

.EXAMPLE
    $setColumnParams = @{
        tableLogicalName = "account"
        logicalName      = "revenue"
        type             = "Money"
        value            = $true
        solutionUniqueName = "MySolution"
    }

    Set-ColumnIsSecured-Example @setColumnParams

    Sets the revenue column in the account table to be secured.

.NOTES
    Requires appropriate permissions to modify table schema in Dataverse.
#>
function Set-ColumnIsSecured-Example {
   param(
      [Parameter(Mandatory)] 
      [string] 
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [string] 
      $logicalName,
      [Parameter(Mandatory)] 
      [string] 
      $type,
      [Parameter(Mandatory)] 
      [bool] 
      $value,
      [Parameter(Mandatory)] 
      [string] 
      $solutionUniqueName
   )

   # Retrieve the column definition
   $getColumnParams = @{
      tableLogicalName = $tableLogicalName
      logicalName      = $logicalName
      type             = $type
   }

   $columnDefinition = Get-Column @getColumnParams
   
   if ($null -eq $columnDefinition) {
      throw "Column $logicalName not found in table $tableLogicalName."
   }
   if ($columnDefinition.IsSecured -eq $value) {
      return
   }
   else {
      # Update the column definition to set IsSecured
      $columnDefinition.IsSecured = $value

      try {

         $updateColumnParams = @{
            tableLogicalName   = $tableLogicalName
            column             = $columnDefinition
            type               = $type
            solutionUniqueName = $solutionUniqueName
            mergeLabels        = $true
         }

         Update-Column @updateColumnParams
      }
      catch {
         throw "Failed to update column $logicalName in table ${$tableLogicalName}: $_.Exception.Message"
      }
   }
}
# </SetColumnIsSecuredExample>
# <AddRoleToUserByNameExample>
<#
.SYNOPSIS
    Adds a security role to a user by role name.

.DESCRIPTION
    This function retrieves a security role by its name and assigns it to the specified user.
    If the user already has the role, no action is taken.

.PARAMETER userid
    The GUID of the user to assign the role to.

.PARAMETER rolename
    The name of the security role to assign.

.EXAMPLE
    Add-RoleToUserByName-Example -userid "12345678-1234-1234-1234-123456789012" -rolename "System Administrator"
    
    Assigns the System Administrator role to the specified user.

.NOTES
    Requires appropriate permissions to manage user roles in Dataverse.
#>
function Add-RoleToUserByName-Example {
   param(
      [Parameter(Mandatory)] 
      [guid] 
      $userid,
      [Parameter(Mandatory)] 
      [string]
      $rolename
   )

   # Retrieve the ID of the role using the name
   $getRecordsParams = @{
      setName           = 'roles'
      query             = "?`$filter=name eq '$rolename'&`$select=roleid"
      strongConsistency = $true
   }

   $results = Get-Records @getRecordsParams

   if ($results.value.length -eq 0) {
      throw "No security role named $rolename exists."
   }

   $roleid = $results.value[0].roleid

   if (!(Get-UserHasRole-Example -userid $userid -roleid $roleid)) {
      # Add the role to the user if they don't already have it
      $addToCollectionParams = @{
         targetSetName     = 'roles'
         targetId          = $roleid
         collectionName    = 'systemuserroles_association'
         setName           = 'systemusers'
         id                = $userid
         strongConsistency = $true
      }

      Add-ToCollection @addToCollectionParams

   }

}
# </AddRoleToUserByNameExample>
# <GetUserHasRoleExample>
<#
.SYNOPSIS
    Checks if a user has a specific security role.

.DESCRIPTION
    This function verifies whether a specified user has been assigned a particular security role
    by querying the systemuserrolescollection.

.PARAMETER userid
    The GUID of the user to check.

.PARAMETER roleid
    The GUID of the security role to check for.

.RETURNS
    Boolean value indicating whether the user has the specified role.

.EXAMPLE
    $hasRole = Get-UserHasRole-Example -userid "12345678-1234-1234-1234-123456789012" -roleid "87654321-4321-4321-4321-210987654321"
    
    Checks if the specified user has the specified role.

.NOTES
    Uses strong consistency to ensure accurate results.
#>
function Get-UserHasRole-Example {
   param(
      [Parameter(Mandatory)] 
      [guid] 
      $userid,
      [Parameter(Mandatory)] 
      [guid] 
      $roleid
   )

   $getRecordsParams = @{
      setName           = 'systemuserrolescollection'
      query             = "?`$filter=systemuserid eq $userid and roleid eq $roleid&`$select=systemuserroleid"
      strongConsistency = $true
   }

   $results = Get-Records @getRecordsParams

   return $results.value.length -eq 1
}
# </GetUserHasRoleExample>
# <PrivilegeDepth>
<#
.DESCRIPTION
    Enum defining the privilege depth levels for security roles in Dataverse.
    - Basic: Basic level access
    - Local: Local/Business Unit level access
    - Deep: Deep/Parent-Child Business Unit level access
    - Global: Organization level access
    - RecordFilter: Record-based filtering access
#>
# Define the PrivilegeDepth enum
enum PrivilegeDepth {
   Basic = 0
   Local = 1
   Deep = 2
   Global = 3
   RecordFilter = 4
}
# </PrivilegeDepth>
# <AddPrivilegesToRoleExample>
<#
.SYNOPSIS
    Adds specified privileges to a security role with a defined depth level.

.DESCRIPTION
    This function retrieves privileges by their names and adds them to a specified security role
    with the specified privilege depth level (Basic, Local, Deep, Global, or RecordFilter).

.PARAMETER roleid
    The GUID of the security role to add privileges to.

.PARAMETER depth
    The privilege depth level from the PrivilegeDepth enum.

.PARAMETER privilegeNames
    An array of privilege names to add to the role.

.EXAMPLE
    Add-PrivilegesToRole-Example -roleid "12345678-1234-1234-1234-123456789012" -depth ([PrivilegeDepth]::Global) -privilegeNames @("prvReadAccount", "prvWriteAccount")
    
    Adds read and write account privileges at Global level to the specified role.

.NOTES
    Requires appropriate permissions to modify security roles in Dataverse.
#>
function Add-PrivilegesToRole-Example {
   param(
      [Parameter(Mandatory)] 
      [guid] 
      $roleid,
      [Parameter(Mandatory)] 
      [PrivilegeDepth] 
      $depth,
      [Parameter(Mandatory)] 
      [string[]] 
      $privilegeNames
   )

   $query = @()
   $query += '$select=privilegeid,name'
   $query += '$filter=Microsoft.Dynamics.CRM.In(PropertyName=@p1,PropertyValues=@p2)'
   $query += "@p1='name'"
   $query += '@p2={0}' -f ("['" + ($privileges -join "','") + "']")
   $queryString = $query -join '&'

   # Retrieve the ID of the privilege using the name and type
   $results = $null

   try {
      $getRecordsParams = @{
         setName           = 'privileges'
         query             = "?$queryString"
         strongConsistency = $true
      }

      $results = Get-Records @getRecordsParams
   }
   catch {
      throw "Failed to retrieve privileges: $_.Exception.Message"
   }

   if ($results.value.length -eq 0) {
      throw "No privileges matching privilege names found."
   }

   $rolePrivileges = @()

   $results.value | ForEach-Object {
      $privilegeId = $_.privilegeid
      $privilegeName = $_.name

      $rolePrivileges += [PSCustomObject]@{
         '@odata.type' = 'Microsoft.Dynamics.CRM.RolePrivilege'
         PrivilegeId   = $privilegeId
         Depth         = $depth.ToString()
      }
   }

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')

   $AddPrivilegesRoleRequest = @{
      Uri     = $baseURI + "roles($roleid)" + '/Microsoft.Dynamics.CRM.AddPrivilegesRole'
      Method  = 'Post'
      Headers = $postHeaders
      Body    = @{
         Privileges = $rolePrivileges
      } | ConvertTo-Json -Depth 10
   }

   try {
      Invoke-ResilientRestMethod $AddPrivilegesRoleRequest
   }
   catch {
      throw "Failed to add privileges to role: $_.Exception.Message"
   }
  
}
# </AddPrivilegesToRoleExample>
# <DumpColumnSecurityInfoExample>
<#
.SYNOPSIS
    Exports column security information for all tables to a CSV file.

.DESCRIPTION
    This function retrieves metadata for all non-private tables and their attributes,
    focusing on column security properties. The results are exported to a CSV file
    containing information about each column's security capabilities.

.PARAMETER filepath
    The directory path where the CSV file will be saved.

.PARAMETER filename
    The name of the CSV file to create. Defaults to 'ColumnSecurityInfo.csv'.

.EXAMPLE
    Dump-ColumnSecurityInfo-Example -filepath "C:\Export" -filename "SecurityReport.csv"
    
    Exports column security information to C:\Export\SecurityReport.csv

.NOTES
    The CSV includes columns for: Column name, Type, IsPrimaryName, IsSecured, 
    CanBeSecuredForCreate, CanBeSecuredForUpdate, and CanBeSecuredForRead.
#>
function Dump-ColumnSecurityInfo-Example {
   param(
      [Parameter(Mandatory)] 
      [string] 
      $filepath,
      [string]
      $filename = 'ColumnSecurityInfo.csv'
   )

   $query = @"
   {
   "Properties": {
      "AllProperties": false,
      "PropertyNames": ["SchemaName","Attributes"]
   },
   "Criteria": {
      "FilterOperator": "And",
      "Conditions": [
         {
            "ConditionOperator": "Equals",
            "PropertyName": "IsPrivate",
            "Value": {
               "Type": "System.Boolean",
               "Value": "false"
            }
         }
      ]
   },
   "AttributeQuery": {
      "Properties": {
         "AllProperties": false,
         "PropertyNames": [
            "SchemaName",
            "AttributeTypeName",
            "IsPrimaryName",
            "IsSecured",
            "CanBeSecuredForCreate",
            "CanBeSecuredForUpdate",
            "CanBeSecuredForRead"
         ]
      },
      "Criteria": {
         "FilterOperator": "And",
         "Conditions": [ 
            {
               "ConditionOperator": "NotEquals",
               "PropertyName": "AttributeTypeName",
               "Value": {
                  "Type": "Microsoft.Xrm.Sdk.Metadata.AttributeTypeDisplayName",
                  "Value": "VirtualType"
               }
            }
         ]
      }
   }
}
"@

   $query = $query -replace '\s+', ' ' # Remove extra whitespace

   $RetrieveMetadataChangesRequest = @{
      Uri     = $baseURI + 'RetrieveMetadataChanges(Query=@p1)?@p1=' + $([System.Web.HttpUtility]::UrlEncode($query))
      Method  = 'Get'
      Headers = $baseHeaders
   }

   try {
      $results = Invoke-ResilientRestMethod $RetrieveMetadataChangesRequest 

      # Convert the results to CSV and save to file
      $results.EntityMetadata.forEach({
            $tableName = $_.SchemaName
            $attributes = $_.Attributes | ForEach-Object {
               [PSCustomObject]@{
                  Column                = "$tableName.$($_.SchemaName)"
                  Type                  = $_.AttributeTypeName.Value
                  IsPrimaryName         = $_.IsPrimaryName
                  IsSecured             = $_.IsSecured
                  CanBeSecuredForCreate = $_.CanBeSecuredForCreate
                  CanBeSecuredForUpdate = $_.CanBeSecuredForUpdate
                  CanBeSecuredForRead   = $_.CanBeSecuredForRead
               }
            }
            $attributes | Export-Csv -Path "$filepath\$filename" -NoTypeInformation -Append
         })
   }
   catch {
      throw "Failed retrieve column security information: $_.Exception.Message"
   }

}
# </DumpColumnSecurityInfoExample>
# <NewFieldSecurityProfileExample>
<#
.SYNOPSIS
    Creates a new field security profile in Dataverse.

.DESCRIPTION
    This function creates a new field security profile with the specified name and description.
    If a profile with the same name already exists, it will be deleted before creating the new one.

.PARAMETER name
    The name of the field security profile to create.

.PARAMETER description
    The description of the field security profile.

.PARAMETER solutionUniqueName
    The unique name of the solution where the profile should be tracked.

.RETURNS
    The GUID of the newly created field security profile.

.EXAMPLE
    $profileId = New-FieldSecurityProfile-Example -name "HR Profile" -description "Profile for HR department" -solutionUniqueName "MySolution"
    
    Creates a new field security profile for HR department use.

.NOTES
    Requires appropriate permissions to create field security profiles in Dataverse.
#>
function New-FieldSecurityProfile-Example {
   param(
      [Parameter(Mandatory)] 
      [string] 
      $name,
      [Parameter(Mandatory)] 
      [string] 
      $description,
      [Parameter(Mandatory)] 
      [string] 
      $solutionUniqueName
   )

   # Check if the field security profile already exists
   $recordIdParams = @{
      entitySetName         = 'fieldsecurityprofiles'
      columnLogicalName     = 'name'
      uniqueStringValue     = $name
      primaryKeyLogicalName = 'fieldsecurityprofileid'
   }

   $fieldSecurityProfileId = Get-RecordId-Helper @recordIdParams


   if ($null -ne $fieldSecurityProfileId) {
      # Delete if it already exists

      $removeRecordParams = @{
         setName           = 'fieldsecurityprofiles'
         id                = $fieldSecurityProfileId
         strongConsistency = $true
      }
      
      Remove-Record @removeRecordParams | Out-Null
   }

   $newId = $null
   
   try {

      # Create new field security profile

      $newRecordParams = @{
         setName            = 'fieldsecurityprofiles'
         body               = @{
            name        = $name
            description = $description
         }
         solutionUniqueName = $solutionUniqueName
      }
      $newId = New-Record @newRecordParams
   }
   catch {
      throw "Failed to create field security profile: $_.Exception.Message"
   }

   return [guid]$newId
}
# </NewFieldSecurityProfileExample>
# <GetSecuredColumnListExample>
<#
.SYNOPSIS
    Retrieves a list of all secured columns in the Dataverse environment.

.DESCRIPTION
    This function queries the system administrator field security profile (which contains
    references to all secured columns) to retrieve a comprehensive list of secured columns
    in the format "tablename.columnname".

.RETURNS
    A sorted array of strings representing secured columns in "table.column" format.

.EXAMPLE
    $securedColumns = Get-SecuredColumnList-AdminOnly-Example
    Write-Host "Secured columns: $($securedColumns -join ', ')"
    
    Retrieves and displays all secured columns in the environment.

.NOTES
    Requires system administrator privileges to read field security profiles.
    Uses the fixed GUID '572329c1-a042-4e22-be47-367c6374ea45' which represents
    the system administrator field security profile.
#>

function Get-SecuredColumnList-Example {
   param(
      [Parameter(Mandatory)] 
      [string] 
      $filepath,
      [string]
      $filename = 'SecuredColumns.csv'
   )

   $query = @"
{
   "Properties": {
      "AllProperties": false,
      "PropertyNames": ["SchemaName","Attributes"]
   },
   "Criteria": {
      "FilterOperator": "And",
      "Conditions": []
   },
   "AttributeQuery": {
      "Properties": {
         "AllProperties": false,
         "PropertyNames": [
            "SchemaName", "IsSecured"
         ]
      },
      "Criteria": {
         "FilterOperator": "And",
         "Conditions": [
            {
               "ConditionOperator": "Equals",
               "PropertyName": "IsSecured",
               "Value": {
                  "Type": "System.Boolean",
                  "Value": "true"
               }
            }
         ]
      }
   }
}
"@

   $query = $query -replace '\s+', ' ' # Remove extra whitespace

   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('Consistency', 'Strong')

   $RetrieveMetadataChangesRequest = @{
      Uri     = $baseURI + 'RetrieveMetadataChanges(Query=@p1)?@p1=' + $([System.Web.HttpUtility]::UrlEncode($query))
      Method  = 'Get'
      Headers = $getHeaders
   }

   try {
      $results = Invoke-ResilientRestMethod $RetrieveMetadataChangesRequest 

      # Convert the results to CSV and save to file
      $results.EntityMetadata.forEach({
            $tableName = $_.SchemaName
            $attributes = $_.Attributes | ForEach-Object {
               [PSCustomObject]@{
                  Table  = $tableName
                  Column = $_.SchemaName
               }
            }
            $attributes | Export-Csv -Path "$filepath\$filename" -NoTypeInformation -Append
         })
   }
   catch {
      throw "Failed retrieve table and column information: $_.Exception.Message"
   }
}
# </GetSecuredColumnListExample>
# <GetSecuredColumnListAdminOnlyExample>
function Get-SecuredColumnList-AdminOnly-Example {

   # Field security profile with ID '572329c1-a042-4e22-be47-367c6374ea45' 
   # manages access for system administrators. It always contains
   # references to each secured column

   $getRecordParams = @{
      setName = 'fieldsecurityprofiles(572329c1-a042-4e22-be47-367c6374ea45)/lk_fieldpermission_fieldsecurityprofileid'
      query   = '?$select=entityname,attributelogicalname&$count=true'
   }

   try {
      $fieldPermisions = Get-Records @getRecordParams

      $values = $fieldPermisions.value | ForEach-Object {
         "{0}.{1}" -f $_.entityname, $_.attributelogicalname
      }

      $sortedValues = $values | Sort-Object

   }
   # Typically, only system administrators can read field security profiles
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      throw "Failed to retrieve secured columns: $_.Exception.Message"
   }

   catch {
      throw "An error occurred in the Get-SecuredColumnList-AdminOnly-Example function: $_.Exception.Message"
   }

   return $sortedValues

}
# </GetSecuredColumnListAdminOnlyExample>
# <GrantColumnAccessExample>
<#
.SYNOPSIS
    Grants column-level access permissions to a user or team for a specific record.

.DESCRIPTION
    This function creates a PrincipalObjectAttributeAccess (POAA) record to grant
    read and/or update access to a specific column on a specific record for a user or team.

.PARAMETER recordId
    The GUID of the record for which access is being granted.

.PARAMETER columnLogicalName
    The logical name of the column to grant access to.

.PARAMETER tableLogicalName
    The logical name of the table containing the column.

.PARAMETER principalId
    The GUID of the user or team to grant access to.

.PARAMETER principalType
    The type of principal - either 'systemuser' or 'team'.

.PARAMETER readAccess
    Boolean indicating whether to grant read access to the column.

.PARAMETER updateAccess
    Boolean indicating whether to grant update access to the column.

.EXAMPLE
    Grant-ColumnAccess-Example -recordId "12345678-1234-1234-1234-123456789012" -columnLogicalName "salary" -tableLogicalName "employee" -principalId "87654321-4321-4321-4321-210987654321" -principalType "systemuser" -readAccess $true -updateAccess $false
    
    Grants read-only access to the salary column for a specific employee record.

.NOTES
    Creates a PrincipalObjectAttributeAccess record. If access has already been granted,
    the operation will fail with an AttributeSharingCreateDuplicate error.
#>
function Grant-ColumnAccess-Example {
   param(
      [Parameter(Mandatory)] 
      [guid] 
      $recordId,
      [Parameter(Mandatory)] 
      [string] 
      $columnLogicalName,
      [Parameter(Mandatory)] 
      [string] 
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [guid] 
      $principalId,
      [Parameter(Mandatory)] 
      [string] 
      $principalType,
      [Parameter(Mandatory)] 
      [bool] 
      $readAccess,
      [Parameter(Mandatory)] 
      [bool] 
      $updateAccess
   )

   if ($principalType -ne 'systemuser' -and $principalType -ne 'team') {
      throw "Invalid principal type: $principalType. Only 'systemuser' or 'team' are allowed."
   }

   $principalTypeSetName = if ($principalType -eq 'systemuser') { 'systemusers' } else { 'teams' }

   $columnId = $null
   $tableSetName = $null
   
   try {
      # Get the ObjectTypeCode for the table and column ID
      $params = @{
         tableLogicalName  = $tableLogicalName
         columnLogicalName = $columnLogicalName
      }
      $metadata = Get-TableSetNameAndColumnId-Example @params
   
      $tableSetName = $metadata.SetName
      $columnId = $metadata.ColumnId
   }
   catch {
      throw "Failed to retrieve table and column information: $_.Exception.Message"
   }

   $poaaRecord = @{
      '@odata.type'                           = 'Microsoft.Dynamics.CRM.principalobjectattributeaccess'
      attributeid                             = $columnId
      "objectid_$tableLogicalName@odata.bind" = "/$tableSetName($recordId)"
      "principalid_$principalType@odata.bind" = "/$principalTypeSetName($principalId)"
      readaccess                              = $readAccess
      updateaccess                            = $updateAccess
   }

   $newRecordParams = @{
      setName           = 'principalobjectattributeaccessset'
      body              = $poaaRecord
      strongConsistency = $true
   }

   try {

      # Create the PrincipalObjectAttributeAccess record
      New-Record @newRecordParams | Out-Null
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {

      # 0x8004F50B
      # -2147158773	Name: AttributeSharingCreateDuplicate
      # Message: Attribute has already been shared.

      # if (ex.Detail.ErrorCode.Equals(-2147158773))
      # {
      #     throw new Exception("The column has already been shared");
      # }

      throw "Dataverse error in Grant-ColumnAccess-Example:  $_.Exception.Message"
   }
   catch {
      throw "Error in Grant-ColumnAccess-Example: $_.Exception.Message"
   }

}
# </GrantColumnAccessExample>
# <ModifyColumnAccessExample>
<#
.SYNOPSIS
    Modifies existing column-level access permissions for a user or team on a specific record.

.DESCRIPTION
    This function updates an existing PrincipalObjectAttributeAccess (POAA) record to change
    the read and/or update access permissions for a specific column on a specific record.

.PARAMETER recordId
    The GUID of the record for which access is being modified.

.PARAMETER columnLogicalName
    The logical name of the column to modify access for.

.PARAMETER tableLogicalName
    The logical name of the table containing the column.

.PARAMETER principalId
    The GUID of the user or team whose access is being modified.

.PARAMETER principalType
    The type of principal - either 'systemuser' or 'team'.

.PARAMETER readAccess
    Boolean indicating the new read access permission.

.PARAMETER updateAccess
    Boolean indicating the new update access permission.

.EXAMPLE
    Modify-ColumnAccess-Example -recordId "12345678-1234-1234-1234-123456789012" -columnLogicalName "salary" -tableLogicalName "employee" -principalId "87654321-4321-4321-4321-210987654321" -principalType "systemuser" -readAccess $true -updateAccess $true
    
    Updates the access to grant both read and update permissions for the salary column.

.NOTES
    Only updates the POAA record if the permissions are actually changing.
    If no matching POAA record exists, the function will throw an error.
#>
function Modify-ColumnAccess-Example {
   param(
      [Parameter(Mandatory)] 
      [guid] 
      $recordId,
      [Parameter(Mandatory)] 
      [string] 
      $columnLogicalName,
      [Parameter(Mandatory)] 
      [string] 
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [guid] 
      $principalId,
      [Parameter(Mandatory)] 
      [string] 
      $principalType,
      [Parameter(Mandatory)] 
      [bool] 
      $readAccess,
      [Parameter(Mandatory)] 
      [bool] 
      $updateAccess
   )

   if ($principalType -ne 'systemuser' -and $principalType -ne 'team') {
      throw "Invalid principal type: $principalType. Only 'systemuser' or 'team' are allowed."
   }

   $principalTypeSetName = if ($principalType -eq 'systemuser') { 'systemusers' } else { 'teams' }

   $columnId = $null
   $tableSetName = $null
   
   try {
      # Get the ObjectTypeCode for the table and column ID
      $params = @{
         tableLogicalName  = $tableLogicalName
         columnLogicalName = $columnLogicalName
      }
      $metadata = Get-TableSetNameAndColumnId-Example @params
   
      $tableSetName = $metadata.SetName
      $columnId = $metadata.ColumnId
   }
   catch {
      throw "Failed to retrieve table and column information: $_.Exception.Message"
   }   $filters = @(
      "attributeid eq $columnId",
      "_principalid_value eq $principalId",
      "_objectid_value eq $recordId"
   )

   $getRecordsParams = @{
      setName           = 'principalobjectattributeaccessset'
      query             = "?`$filter=$($filters -join ' and ')&`$select=readaccess,updateaccess"
      strongConsistency = $true
   }
   $results = $null

   try {
      $results = Get-Records @getRecordsParams
   }
   catch {
      throw "Failed to retrieve principalobjectattributeaccess record: $_.Exception.Message"
   }

   if ($results.value.Count -eq 0) {
      throw "No matching PrincipalObjectAttributeAccess record found."
   }
   elseif ($results.value.Count -gt 1) {
      throw "Multiple PrincipalObjectAttributeAccess records found. Please ensure unique access."
   }

   $poaaRecord = $results.value[0]
   $currentReadAccess = $poaaRecord.readaccess
   $currentUpdateAccess = $poaaRecord.updateaccess

   $propertiesToUpdate = @{}
   if ($currentReadAccess -ne $readAccess) {
      $propertiesToUpdate['readaccess'] = $readAccess
   }
   if ($currentUpdateAccess -ne $updateAccess) {
      $propertiesToUpdate['updateaccess'] = $updateAccess
   }
   if ($propertiesToUpdate.Count -eq 0) {
      # Don't update if nothing there is nothing to change
      return
   }
   $updateRecordParams = @{
      setName            = 'principalobjectattributeaccessset'
      id                 = $poaaRecord.principalobjectattributeaccessid
      body               = $propertiesToUpdate
      solutionUniqueName = $SOLUTION_UNIQUE_NAME
      strongConsistency  = $true
   }
   try {
      Update-Record @updateRecordParams | Out-Null
   }
   catch {
      throw "Failed to update PrincipalObjectAttributeAccess record: $_.Exception.Message"
   }
}
# </ModifyColumnAccessExample>
# <RevokeColumnAccessExample>
<#
.SYNOPSIS
    Revokes column-level access permissions for a user or team on a specific record.

.DESCRIPTION
    This function removes an existing PrincipalObjectAttributeAccess (POAA) record to revoke
    all access permissions for a specific column on a specific record from a user or team.

.PARAMETER recordId
    The GUID of the record for which access is being revoked.

.PARAMETER columnLogicalName
    The logical name of the column to revoke access from.

.PARAMETER tableLogicalName
    The logical name of the table containing the column.

.PARAMETER principalId
    The GUID of the user or team whose access is being revoked.

.PARAMETER principalType
    The type of principal - either 'systemuser' or 'team'.

.PARAMETER readAccess
    Boolean parameter (currently not used in the deletion logic).

.PARAMETER updateAccess
    Boolean parameter (currently not used in the deletion logic).

.EXAMPLE
    Revoke-ColumnAccess-Example -recordId "12345678-1234-1234-1234-123456789012" -columnLogicalName "salary" -tableLogicalName "employee" -principalId "87654321-4321-4321-4321-210987654321" -principalType "systemuser" -readAccess $false -updateAccess $false
    
    Revokes all access to the salary column for the specified user on the specified record.

.NOTES
    Completely removes the POAA record, revoking all access permissions.
    If no matching POAA record exists, the function will throw an error.
#>
function Revoke-ColumnAccess-Example {
   param(
      [Parameter(Mandatory)] 
      [guid] 
      $recordId,
      [Parameter(Mandatory)] 
      [string] 
      $columnLogicalName,
      [Parameter(Mandatory)] 
      [string] 
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [guid] 
      $principalId,
      [Parameter(Mandatory)] 
      [string] 
      $principalType,
      [Parameter(Mandatory)] 
      [bool] 
      $readAccess,
      [Parameter(Mandatory)] 
      [bool] 
      $updateAccess
   )

   if ($principalType -ne 'systemuser' -and $principalType -ne 'team') {
      throw "Invalid principal type: $principalType. Only 'systemuser' or 'team' are allowed."
   }

   $principalTypeSetName = if ($principalType -eq 'systemuser') { 'systemusers' } else { 'teams' }

   $columnId = $null
   $tableSetName = $null
   
   try {
      # Get the ObjectTypeCode for the table and column ID
      $params = @{
         tableLogicalName  = $tableLogicalName
         columnLogicalName = $columnLogicalName
      }
      $metadata = Get-TableSetNameAndColumnId-Example @params
   
      $tableSetName = $metadata.SetName
      $columnId = $metadata.ColumnId
   }
   catch {
      throw "Failed to retrieve table and column information: $_.Exception.Message"
   }   $filters = @(
      "attributeid eq $columnId",
      "_principalid_value eq $principalId",
      "_objectid_value eq $recordId"
   )

   $getRecordsParams = @{
      setName = 'principalobjectattributeaccessset'
      query   = "?`$filter=$($filters -join ' and ')&`$select=readaccess,updateaccess"
   }
   $results = $null

   try {
      $results = Get-Records @getRecordsParams
   }
   catch {
      throw "Failed to retrieve principalobjectattributeaccess record: $_.Exception.Message"
   }

   if ($results.value.Count -eq 0) {
      throw "No matching PrincipalObjectAttributeAccess record found."
   }
   elseif ($results.value.Count -gt 1) {
      throw "Multiple PrincipalObjectAttributeAccess records found. Please ensure unique access."
   }

   $poaaRecord = $results.value[0]

   $deleteRecordParams = @{
      setName           = 'principalobjectattributeaccessset'
      id                = $poaaRecord.principalobjectattributeaccessid
      strongConsistency = $true
   }
   try {
      Remove-Record @deleteRecordParams | Out-Null
   }
   catch {
      throw "Failed to delete PrincipalObjectAttributeAccess record: $_.Exception.Message"
   }
}
# </RevokeColumnAccessExample>
# <GetTableSetNameAndColumnIdExample>
<#
.SYNOPSIS
    Retrieves table entity set name and column metadata ID for a specified table and column.

.DESCRIPTION
    This function queries Dataverse metadata to retrieve the EntitySetName for a table
    and the MetadataId for a specific column within that table. This information is
    required for column-level security operations.

.PARAMETER tableLogicalName
    The logical name of the table to query.

.PARAMETER columnLogicalName
    The logical name of the column within the table.

.RETURNS
    A hashtable containing 'SetName' (EntitySetName) and 'ColumnId' (MetadataId).

.EXAMPLE
    $metadata = Get-TableSetNameAndColumnId-Example -tableLogicalName "account" -columnLogicalName "revenue"
    Write-Host "Set Name: $($metadata.SetName), Column ID: $($metadata.ColumnId)"
    
    Retrieves metadata information for the revenue column in the account table.

.NOTES
    Uses strong consistency to ensure accurate metadata retrieval.
    Throws an error if the table or column is not found.
#>
function Get-TableSetNameAndColumnId-Example {
   param(
      [Parameter(Mandatory)] 
      [string] 
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [string] 
      $columnLogicalName
   )

   $query = @"
{
   "Properties": {
      "AllProperties": false,
      "PropertyNames": ["EntitySetName","Attributes"]
   },
   "Criteria": {
      "FilterOperator": "And",
      "Conditions": [
         {
            "ConditionOperator": "Equals",
            "PropertyName": "LogicalName",
            "Value": {
               "Type": "System.String",
               "Value": "$tableLogicalName"
            }
         }
      ]
   },
   "AttributeQuery": {
      "Properties": {
         "AllProperties": false,
         "PropertyNames": [
            "MetadataId"
         ]
      },
      "Criteria": {
         "FilterOperator": "And",
         "Conditions": [
            {
               "ConditionOperator": "Equals",
               "PropertyName": "LogicalName",
               "Value": {
                  "Type": "System.String",
                  "Value": "$columnLogicalName"
               }
            }
         ]
      }
   }
}
"@

   $query = $query -replace '\s+', ' ' # Remove extra whitespace

   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('Consistency', 'Strong')

   $RetrieveMetadataChangesRequest = @{
      Uri     = $baseURI + 'RetrieveMetadataChanges(Query=@p1)?@p1=' + $([System.Web.HttpUtility]::UrlEncode($query))
      Method  = 'Get'
      Headers = $getHeaders
   }

   try {
      $results = Invoke-ResilientRestMethod $RetrieveMetadataChangesRequest 

      if ($results.EntityMetadata.Count -eq 0) {
         throw "Table $tableLogicalName not found."
      }
      $tableMetadata = $results.EntityMetadata[0]
      $tableSetName = $tableMetadata.EntitySetName
      $columnMetadata = $tableMetadata.Attributes | Where-Object { $_.LogicalName -eq $columnLogicalName }
      if ($null -eq $columnMetadata) {
         throw "Column $columnLogicalName not found in table $tableLogicalName."
      }
      $columnId = $columnMetadata.MetadataId
      return @{
         SetName  = $tableSetName
         ColumnId = $columnId
      }
   }
   catch {
      throw "Failed retrieve table and column information: $_.Exception.Message"
   }
}
# </GetTableSetNameAndColumnIdExample>
