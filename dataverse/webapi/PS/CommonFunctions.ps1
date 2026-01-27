. $PSScriptRoot\Core.ps1

<#
.SYNOPSIS
Gets the current user information from the Dataverse Web API.

.DESCRIPTION
The Get-WhoAmI function uses the Invoke-ResilientRestMethod function to send a GET request to the Dataverse API. 
It constructs the request URI by appending the WhoAmI function name to the base URI. 
It also adds the necessary headers. It returns an object that contains the user ID, business unit ID, and organization ID.

.EXAMPLE
$WhoIAm = Get-WhoAmI
$myBusinessUnit = $WhoIAm.BusinessUnitId
$myUserId = $WhoIAm.UserId

This example gets the current user information from the Dataverse Web API.
#>

function Get-WhoAmI{

   $WhoAmIRequest = @{
      Uri = $baseURI + 'WhoAmI'
      Method = 'Get'
      Headers = $baseHeaders
   }

   Invoke-ResilientRestMethod $WhoAmIRequest
}

<#
.SYNOPSIS
Formats an address according to country/region-specific requirements.

.DESCRIPTION
The Format-Address function uses the Invoke-ResilientRestMethod function to send a GET request to the Dataverse API.
It constructs the request URI by appending the FormatAddress function name and parameters to the base URI.
It returns a formatted address string according to country/region-specific requirements.

.PARAMETER Line1
The first line of the address. This parameter is mandatory.

.PARAMETER City
The city name. This parameter is mandatory.

.PARAMETER StateOrProvince
The state or province name. This parameter is mandatory.

.PARAMETER PostalCode
The postal code. This parameter is mandatory.

.PARAMETER Country
The country name. This parameter is mandatory.

.EXAMPLE
$formattedAddress = Format-Address `
   -Line1 '123 Maple St.' `
   -City 'Seattle' `
   -StateOrProvince 'WA' `
   -PostalCode '98007' `
   -Country 'USA'

This example formats a US address according to US address formatting requirements.
#>

function Format-Address {
   param (
      [Parameter(Mandatory)]
      [String]
      $Line1,
      [Parameter(Mandatory)]
      [String]
      $City,
      [Parameter(Mandatory)]
      [String]
      $StateOrProvince,
      [Parameter(Mandatory)]
      [String]
      $PostalCode,
      [Parameter(Mandatory)]
      [String]
      $Country
   )

   # Create parameter aliases and assignments
   $aliases = "Line1=@p1,City=@p2,StateOrProvince=@p3,PostalCode=@p4,Country=@p5"
   $values = "&@p1='$([System.Uri]::EscapeDataString($Line1))'&@p2='$([System.Uri]::EscapeDataString($City))'&@p3='$([System.Uri]::EscapeDataString($StateOrProvince))'&@p4='$([System.Uri]::EscapeDataString($PostalCode))'&@p5='$([System.Uri]::EscapeDataString($Country))'"

   $FormatAddressRequest = @{
      Uri     = $baseURI + "FormatAddress($aliases)?$values"
      Method  = 'Get'
      Headers = $baseHeaders
   }

   Invoke-ResilientRestMethod $FormatAddressRequest
}

<#
.SYNOPSIS
Initializes a new record from an existing record based on mapping configuration.

.DESCRIPTION
The Initialize-From function uses the Invoke-ResilientRestMethod function to send a GET request to the Dataverse API.
It constructs the request URI by appending the InitializeFrom function name and parameters to the base URI.
It returns an entity with default values set based on the source record and mapping configuration for the organization.

.PARAMETER SourceSetName
The entity set name of the source record. This parameter is mandatory.

.PARAMETER SourceId
The GUID of the source record. This parameter is mandatory.

.PARAMETER TargetEntityName
The logical name of the target entity. This parameter is mandatory.

.PARAMETER TargetFieldType
The target field type. Valid values are 'ValidForCreate', 'ValidForUpdate', or 'ValidForRead'. Default is 'ValidForCreate'. This parameter is optional.

.EXAMPLE
$initializedAccount = Initialize-From `
   -SourceSetName 'accounts' `
   -SourceId $sourceAccountId `
   -TargetEntityName 'account'

This example initializes a new account record based on an existing account, using the default TargetFieldType of 'ValidForCreate'.
#>

function Initialize-From {
   param (
      [Parameter(Mandatory)]
      [String]
      $SourceSetName,
      [Parameter(Mandatory)]
      [Guid]
      $SourceId,
      [Parameter(Mandatory)]
      [String]
      $TargetEntityName,
      [String]
      $TargetFieldType = 'ValidForCreate'
   )

   $aliases = "EntityMoniker=@p1,TargetEntityName=@p2,TargetFieldType=@p3"
   $values = "&@p1={'@odata.id':'$SourceSetName($SourceId)'}&@p2='$TargetEntityName'&@p3=Microsoft.Dynamics.CRM.TargetFieldType'$TargetFieldType'"

   $InitializeFromRequest = @{
      Uri     = $baseURI + "InitializeFrom($aliases)?$values"
      Method  = 'Get'
      Headers = $baseHeaders
   }

   Invoke-ResilientRestMethod $InitializeFromRequest
}

<#
.SYNOPSIS
Retrieves detailed information about the current organization.

.DESCRIPTION
The Get-CurrentOrganization function uses the Invoke-ResilientRestMethod function to send a GET request to the Dataverse API.
It constructs the request URI by appending the RetrieveCurrentOrganization function name and parameters to the base URI.
It returns detailed information about the current organization including endpoints, version, and other metadata.

.PARAMETER AccessType
The endpoint access type. Valid values are 'Default', 'WebApplication', 'OrganizationService', or 'OrganizationDataService'. Default is 'Default'. This parameter is optional.

.EXAMPLE
$orgInfo = Get-CurrentOrganization

This example retrieves the current organization information using the default access type.

.EXAMPLE
$orgInfo = Get-CurrentOrganization -AccessType 'WebApplication'

This example retrieves the current organization information with WebApplication endpoint access type.
#>

function Get-CurrentOrganization {
   param (
      [String]
      $AccessType = 'Default'
   )

   $RetrieveCurrentOrganizationRequest = @{
      Uri     = $baseURI + "RetrieveCurrentOrganization(AccessType=@p1)?@p1=Microsoft.Dynamics.CRM.EndpointAccessType'$AccessType'"
      Method  = 'Get'
      Headers = $baseHeaders
   }

   Invoke-ResilientRestMethod $RetrieveCurrentOrganizationRequest
}

<#
.SYNOPSIS
Retrieves the total number of records for specified entities.

.DESCRIPTION
The Get-TotalRecordCount function uses the Invoke-ResilientRestMethod function to send a GET request to the Dataverse API.
It constructs the request URI by appending the RetrieveTotalRecordCount function name and parameters to the base URI.
It returns the total number of records for the specified entities from a snapshot taken within the last 24 hours.

.PARAMETER EntityNames
An array of entity logical names to retrieve record counts for. This parameter is mandatory.

.EXAMPLE
$counts = Get-TotalRecordCount -EntityNames @('account', 'contact')

This example retrieves the total record counts for accounts and contacts.
#>

function Get-TotalRecordCount {
   param (
      [Parameter(Mandatory)]
      [String[]]
      $EntityNames
   )

   $entityNamesJson = ConvertTo-Json $EntityNames -Compress

   $RetrieveTotalRecordCountRequest = @{
      Uri     = $baseURI + "RetrieveTotalRecordCount(EntityNames=@p1)?@p1=$entityNamesJson"
      Method  = 'Get'
      Headers = $baseHeaders
   }

   Invoke-ResilientRestMethod $RetrieveTotalRecordCountRequest
}

<#
.SYNOPSIS
Determines whether a user has the System Administrator security role.

.DESCRIPTION
The Test-SystemAdministrator function uses the Invoke-ResilientRestMethod function to send a GET request to the Dataverse API.
It constructs the request URI by appending the sample_IsSystemAdmin bound function to the systemusers entity.
This is a custom API that must be installed in the environment before use.
It returns a boolean value indicating whether the user has the System Administrator role.

.PARAMETER SystemUserId
The GUID of the system user to test. This parameter is mandatory.

.EXAMPLE
$isAdmin = Test-SystemAdministrator -SystemUserId $userId

This example checks if the specified user has the System Administrator role.
#>

function Test-SystemAdministrator {
   param (
      [Parameter(Mandatory)]
      [Guid]
      $SystemUserId
   )

   $IsSystemAdminRequest = @{
      Uri     = $baseURI + "systemusers($SystemUserId)/Microsoft.Dynamics.CRM.sample_IsSystemAdmin"
      Method  = 'Get'
      Headers = $baseHeaders
   }

   Invoke-ResilientRestMethod $IsSystemAdminRequest
}

<#
.SYNOPSIS
Retrieves the access rights a principal (user) has to a specific record.

.DESCRIPTION
The Get-PrincipalAccess function uses the Invoke-ResilientRestMethod function to send a GET request to the Dataverse API.
It constructs the request URI by appending the RetrievePrincipalAccess bound function to the systemusers entity.
It returns the access rights the specified user has to the target record.

.PARAMETER SystemUserId
The GUID of the system user (principal) to check access for. This parameter is mandatory.

.PARAMETER TargetSetName
The entity set name of the target record. This parameter is mandatory.

.PARAMETER TargetId
The GUID of the target record. This parameter is mandatory.

.EXAMPLE
$access = Get-PrincipalAccess `
   -SystemUserId $userId `
   -TargetSetName 'accounts' `
   -TargetId $accountId

This example retrieves the access rights the specified user has to the specified account.
#>

function Get-PrincipalAccess {
   param (
      [Parameter(Mandatory)]
      [Guid]
      $SystemUserId,
      [Parameter(Mandatory)]
      [String]
      $TargetSetName,
      [Parameter(Mandatory)]
      [Guid]
      $TargetId
   )

   $targetODataId = "$TargetSetName($TargetId)"
   $encodedTarget = [System.Uri]::EscapeDataString("{'@odata.id':'$targetODataId'}")

   $GetPrincipalAccessRequest = @{
      Uri     = $baseURI + "systemusers($SystemUserId)/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@p1)?@p1=$encodedTarget"
      Method  = 'Get'
      Headers = $baseHeaders
   }

   Invoke-ResilientRestMethod $GetPrincipalAccessRequest
}

<#
.SYNOPSIS
Grants access rights to a principal (user or team) for a specific record.

.DESCRIPTION
The Grant-Access function uses the Invoke-ResilientRestMethod function to send a POST request to the Dataverse API.
It constructs the request to call the GrantAccess action to grant specific access privileges on a record to a principal.
It uses the AccessMask parameter to specify which access rights to grant (e.g., DeleteAccess, ShareAccess).

.PARAMETER TargetSetName
The entity set name of the target record to grant access to. This parameter is mandatory.

.PARAMETER TargetId
The GUID of the target record. This parameter is mandatory.

.PARAMETER TargetEntityName
The logical name of the target entity. This parameter is mandatory.

.PARAMETER TargetPrimaryKey
The name of the primary key column for the target entity (e.g., 'accountid'). This parameter is mandatory.

.PARAMETER PrincipalSetName
The entity set name of the principal (e.g., 'systemusers' or 'teams'). This parameter is mandatory.

.PARAMETER PrincipalId
The GUID of the principal. This parameter is mandatory.

.PARAMETER PrincipalEntityName
The logical name of the principal entity (e.g., 'systemuser' or 'team'). This parameter is mandatory.

.PARAMETER PrincipalPrimaryKey
The name of the primary key column for the principal entity (e.g., 'systemuserid'). This parameter is mandatory.

.PARAMETER AccessMask
The access rights to grant. Valid values include 'ReadAccess', 'WriteAccess', 'DeleteAccess', 'ShareAccess', 'AppendAccess', 'AppendToAccess', 'AssignAccess'. Multiple values can be combined with commas. This parameter is mandatory.

.EXAMPLE
Grant-Access `
   -TargetSetName 'accounts' `
   -TargetId $accountId `
   -TargetEntityName 'account' `
   -TargetPrimaryKey 'accountid' `
   -PrincipalSetName 'systemusers' `
   -PrincipalId $userId `
   -PrincipalEntityName 'systemuser' `
   -PrincipalPrimaryKey 'systemuserid' `
   -AccessMask 'DeleteAccess'

This example grants DeleteAccess rights to the specified user for the specified account.
#>

function Grant-Access {
   param (
      [Parameter(Mandatory)]
      [String]
      $TargetSetName,
      [Parameter(Mandatory)]
      [Guid]
      $TargetId,
      [Parameter(Mandatory)]
      [String]
      $TargetEntityName,
      [Parameter(Mandatory)]
      [String]
      $TargetPrimaryKey,
      [Parameter(Mandatory)]
      [String]
      $PrincipalSetName,
      [Parameter(Mandatory)]
      [Guid]
      $PrincipalId,
      [Parameter(Mandatory)]
      [String]
      $PrincipalEntityName,
      [Parameter(Mandatory)]
      [String]
      $PrincipalPrimaryKey,
      [Parameter(Mandatory)]
      [String]
      $AccessMask
   )

   $body = @{
      Target          = @{
         $TargetPrimaryKey = $TargetId
         '@odata.type'     = "Microsoft.Dynamics.CRM.$TargetEntityName"
      }
      PrincipalAccess = @{
         Principal     = @{
            $PrincipalPrimaryKey = $PrincipalId
            '@odata.type'        = "Microsoft.Dynamics.CRM.$PrincipalEntityName"
         }
         AccessMask    = $AccessMask
      }
   }

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')

   $GrantAccessRequest = @{
      Uri     = $baseURI + 'GrantAccess'
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body -Depth 5
   }

   Invoke-ResilientRestMethod $GrantAccessRequest
}

<#
.SYNOPSIS
Adds privileges to a security role.

.DESCRIPTION
The Add-RolePrivilege function uses the Invoke-ResilientRestMethod function to send a POST request to the Dataverse API.
It constructs the request to call the AddPrivilegesRole bound action to add privileges to an existing security role.
Each privilege must specify a depth (Basic, Local, Deep, or Global) and the privilege GUID.

.PARAMETER RoleId
The GUID of the security role to add privileges to. This parameter is mandatory.

.PARAMETER Privileges
An array of hashtables containing privilege information. Each hashtable must have PrivilegeId (Guid), Depth (String), BusinessUnitId (Guid), and optionally PrivilegeName (String). This parameter is mandatory.

.EXAMPLE
$privileges = @(
   @{
      PrivilegeId = [Guid]'e479f919-8e17-42f4-94a6-3e5e4b1a3bae'
      Depth = 'Basic'
      BusinessUnitId = $businessUnitId
      PrivilegeName = 'prvCreateAccount'
   },
   @{
      PrivilegeId = [Guid]'0f1a4a0f-0e17-42f4-94a6-3e5e4b1a3bae'
      Depth = 'Basic'
      BusinessUnitId = $businessUnitId
      PrivilegeName = 'prvReadAccount'
   }
)

Add-RolePrivilege -RoleId $roleId -Privileges $privileges

This example adds the prvCreateAccount and prvReadAccount privileges to the specified role.
#>

function Add-RolePrivilege {
   param (
      [Parameter(Mandatory)]
      [Guid]
      $RoleId,
      [Parameter(Mandatory)]
      [hashtable[]]
      $Privileges
   )

   $body = @{
      Privileges = $Privileges
   }

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')

   $AddPrivilegesRoleRequest = @{
      Uri     = $baseURI + "roles($RoleId)/Microsoft.Dynamics.CRM.AddPrivilegesRole"
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body -Depth 5
   }

   Invoke-ResilientRestMethod $AddPrivilegesRoleRequest
}