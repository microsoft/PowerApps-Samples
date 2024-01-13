. $PSScriptRoot\Core.ps1
function Get-Records {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $setName,
      [Parameter(Mandatory)] 
      [String] 
      $query
   )
   $uri = $baseURI + $setName + $query
   # Header for GET operations that have annotations
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $null)
   $getHeaders.Add('Prefer', 'odata.include-annotations="*"')
   $RetrieveMultipleRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }
   Invoke-ResilientRestMethod $RetrieveMultipleRequest
}

function New-Record {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $setName,
      [Parameter(Mandatory)] 
      [hashtable]
      $body
   )
   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   
   $CreateRequest = @{
      Uri     = $baseURI + $setName
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body

   }
   $rh = Invoke-ResilientRestMethod -request $CreateRequest -returnHeader $true
   $url = $rh[1]['OData-EntityId']
   $selectedString = Select-String -InputObject $url -Pattern '(?<=\().*?(?=\))'
   return [System.Guid]::New($selectedString.Matches.Value.ToString())
}

function Get-Record {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $setName,
      [Parameter(Mandatory)] 
      [Guid] 
      $id,
      [String] 
      $query
   )
   $uri = $baseURI + $setName
   $uri = $uri + '(' + $id.Guid + ')' + $query
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $null)
   $getHeaders.Add('Prefer', 'odata.include-annotations="*"')
   $RetrieveRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }
   Invoke-ResilientRestMethod $RetrieveRequest | Select-Object
}

function Get-ColumnValue {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $setName,
      [Parameter(Mandatory)] 
      [Guid] 
      $id,
      [String] 
      $property
   )
   $uri = $baseURI + $setName
   $uri = $uri + '(' + $id.Guid + ')/' + $property
   $headers = $baseHeaders.Clone()
   $headers.Add('If-None-Match', $null)
   $GetColumnValueRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $headers
   }
   $value = Invoke-ResilientRestMethod $GetColumnValueRequest
   return $value.value
}

function Update-Record {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $setName,
      [Parameter(Mandatory)] 
      [Guid] 
      $id,
      [Parameter(Mandatory)] 
      [hashtable]
      $body
   )
   $uri = $baseURI + $setName
   $uri = $uri + '(' + $id.Guid + ')'
   # Header for Update operations
   $updateHeaders = $baseHeaders.Clone()
   $updateHeaders.Add('Content-Type', 'application/json')
   $updateHeaders.Add('If-Match', '*') # Prevent Create
   $UpdateRequest = @{
      Uri     = $uri
      Method  = 'Patch'
      Headers = $updateHeaders
      Body    = ConvertTo-Json $body
   }
   Invoke-ResilientRestMethod $UpdateRequest
}

function Set-ColumnValue {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $setName,
      [Parameter(Mandatory)] 
      [Guid] 
      $id,
      [Parameter(Mandatory)] 
      [string]
      $property,
      [Parameter(Mandatory)] 
      $value
   )
   $uri = $baseURI + $setName
   $uri = $uri + '(' + $id.Guid + ')' + '/' + $property
   $headers = $baseHeaders.Clone()
   $headers.Add('Content-Type', 'application/json')
   $body = @{
      'value' = $value
   }
   $SetColumnValueRequest = @{
      Uri     = $uri
      Method  = 'Put'
      Headers = $headers
      Body    =  ConvertTo-Json $body
   }
   Invoke-ResilientRestMethod $SetColumnValueRequest
}

function Remove-Record {
   param (
      [Parameter(Mandatory)] 
      [String]
      $setName,
      [Parameter(Mandatory)] 
      [Guid] 
      $id
   )
   $uri = $baseURI + $setName
   $uri = $uri + '(' + $id.Guid + ')'
   $DeleteRequest = @{
      Uri     = $uri
      Method  = 'Delete'
      Headers = $baseHeaders
   }
   Invoke-ResilientRestMethod $DeleteRequest
}