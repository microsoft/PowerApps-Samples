#Requires -Version 7.4.0
#Requires -Modules @{ ModuleName="Az"; ModuleVersion="11.1.0" }

# Set to $true only while debugging with Fiddler
$debug = $false
# Set this value to the Fiddler proxy URL configured on your computer
$proxyUrl = 'http://127.0.0.1:8888'

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
   $token = (Get-AzAccessToken -ResourceUrl $uri).Token

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

function Invoke-DataverseCommands {
   param (
      [Parameter(Mandatory)] 
      $commands
   )
   try {
      Invoke-Command $commands
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      Write-Host "An error occurred calling Dataverse:" -ForegroundColor Red
      $statuscode = [int]$_.Exception.StatusCode;
      $statusText = $_.Exception.StatusCode
      Write-Host "StatusCode: $statuscode ($statusText)"
      # Replaces escaped characters in the JSON
      [Regex]::Replace($_.ErrorDetails.Message, "\\[Uu]([0-9A-Fa-f]{4})", 
         {[char]::ToString([Convert]::ToInt32($args[0].Groups[1].Value, 16))} )

   }
   catch {
      Write-Host "An error occurred in the script:" -ForegroundColor Red
      $_
   }
}


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
      Invoke-RestMethod @request -ResponseHeadersVariable rhv
      if ($returnHeader) {
         return $rhv
      }
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
         Invoke-RestMethod @request -ResponseHeadersVariable rhv
         if ($returnHeader) {
            return $rhv
         }
      }
      else {
         throw $_
      }
   }
   catch {
      throw $_
   }
}