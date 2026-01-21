# Set to $true only while debugging with Fiddler
$debug = $false
# Set this value to the Fiddler proxy URL configured on your computer
$proxyUrl = 'http://127.0.0.1:8888'

<#
.SYNOPSIS
Connects to Dataverse Web API using Azure authentication.

.DESCRIPTION
The Connect function uses the Get-AzAccessToken cmdlet to obtain an access token for the specified resource URI. 
It then sets the global variables baseHeaders and baseURI to be used for subsequent requests to the resource.

.PARAMETER uri
The resource URI to connect to. This parameter is mandatory.

.EXAMPLE
Connect -uri 'https://yourorg.crm.dynamics.com'
This example connects to Dataverse environment and sets the baseHeaders and baseURI variables.
#>

function Connect {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $uri
   )

   ## Login if not already logged in
   if ($null -eq (Get-AzTenant -ErrorAction SilentlyContinue)) {
      Connect-AzAccount | Out-Null
   }

   # Get an access token
   $secureToken = (Get-AzAccessToken `
      -ResourceUrl $uri `
      -AsSecureString).Token

   # Convert the secure token to a string
   $token = ConvertFrom-SecureString `
      -SecureString $secureToken `
      -AsPlainText

   # Define common set of headers
   $global:baseHeaders = @{
      'Authorization'    = 'Bearer ' + $token
      'Accept'           = 'application/json'
      'OData-MaxVersion' = '4.0'
      'OData-Version'    = '4.0'
   }

   # Set baseURI
   $global:baseURI = $uri + 'api/data/v9.2/'
}


<#
.SYNOPSIS
Invokes a set of commands against the Dataverse Web API.

.DESCRIPTION
The Invoke-DataverseCommands function uses the Invoke-Command cmdlet to run a script block of commands against the Dataverse Web API. 
It handles any errors that may occur from the Dataverse API or the script itself.

.PARAMETER commands
The script block of commands to run against the Dataverse resource. This parameter is mandatory.

.EXAMPLE
Invoke-DataverseCommands -commands {
   # Get first account from Dataverse
   $accounts = (Get-Records `
      -setName 'accounts' `
      -query '?$select=name&$top=1').value

   $oldName = $accounts[0].name
   $newName = 'New Name'

   # Update the first account name to 'New Name'
   Set-ColumnValue `
      -setName 'accounts' `
      -id $accounts[0].accountid `
      -property 'name' `
      -value $newName

   Write-Host "First account name changed from '$oldName' to '$newName'"
}
This example invokes a script block that gets the first account from Dataverse and updates the name of the first account.
#>


function Invoke-DataverseCommands {
   param (
      [Parameter(Mandatory)] 
      $commands
   )
   try {
      Invoke-Command $commands -NoNewScope
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      Write-Host "An error occurred calling Dataverse:" -ForegroundColor Red
      $statuscode = [int]$_.Exception.StatusCode;
      $statusText = $_.Exception.StatusCode
      Write-Host "StatusCode: $statuscode ($statusText)"
      # Replaces escaped characters in the JSON
      [Regex]::Replace($_.ErrorDetails.Message, "\\[Uu]([0-9A-Fa-f]{4})", 
         { [char]::ToString([Convert]::ToInt32($args[0].Groups[1].Value, 16)) } )

   }
   catch {
      Write-Host "An error occurred in the script:" -ForegroundColor Red
      $_
   }
}

<#
.SYNOPSIS
Invokes a REST method with resilience to handle 429 errors.

.DESCRIPTION
The Invoke-ResilientRestMethod function uses the Invoke-RestMethod cmdlet to send an HTTP request to a RESTful web service. 
It handles any 429 errors (Too Many Requests) by retrying the request using the Retry-After header value as the retry interval. 
It also supports using a proxy if the $debug variable is set to true.

.PARAMETER request
A hashtable of parameters to pass to the Invoke-RestMethod cmdlet. This parameter is mandatory.

.PARAMETER returnHeader
A boolean value that indicates whether to return the response headers instead of the response body. The default value is false.

.EXAMPLE
See the functions in the TableOperations.ps1 file for examples of using this function.
#>

function Invoke-ResilientRestMethod {
   param (
      [Parameter(Mandatory)] 
      $request,
      [bool]
      $returnHeader
   )

   if ($debug) {
      $request.Add('Proxy', $proxyUrl)
   }
   try {
      if ($returnHeader) {
         Invoke-RestMethod @request -ResponseHeadersVariable rhv | Out-Null
         return $rhv
      }
      Invoke-RestMethod @request
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      $statuscode = $_.Exception.Response.StatusCode
      # 429 errors only
      if ($statuscode -eq 'TooManyRequests') {
         if (!$request.ContainsKey('MaximumRetryCount')) {
            $request.Add('MaximumRetryCount', 3)
            # Don't need - RetryIntervalSec
            # When the failure code is 429 and the response includes the Retry-After property in its headers, 
            # the cmdlet uses that value for the retry interval, even if RetryIntervalSec is specified
         }
         # Will attempt retry up to 3 times
         if ($returnHeader) {
            Invoke-RestMethod @request -ResponseHeadersVariable rhv | Out-Null
            return $rhv
         }
         Invoke-RestMethod @request
      }
      else {
         throw $_
      }
   }
   catch {
      throw $_
   }
}