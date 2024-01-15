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