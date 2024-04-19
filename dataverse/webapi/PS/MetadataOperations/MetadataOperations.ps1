. $PSScriptRoot\..\Core.ps1

function Get-Tables {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $query
   )
   $uri = $baseURI + 'EntityDefinitions' + $query
   # Header for GET operations that have annotations
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $null)
   # $getHeaders.Add('Prefer', 'odata.include-annotations="*"')
   $getHeaders.Add('Consistency', 'Strong')
   $RetrieveMultipleRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }
   Invoke-ResilientRestMethod $RetrieveMultipleRequest
}

function Get-Table {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $logicalName,
      [String] 
      $query
   )
   $uri = $baseURI + "EntityDefinitions(LogicalName='$logicalName')" + $query
   # Header for GET operations that have annotations
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $null)
   # $getHeaders.Add('Prefer', 'odata.include-annotations="*"')
   $getHeaders.Add('Consistency', 'Strong')
   $RetrieveRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }
   Invoke-ResilientRestMethod $RetrieveRequest
}

function New-Table {
   param (
      [Parameter(Mandatory)] 
      [hashtable]
      $body,
      [String] 
      $solutionUniqueName
   )

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')
   if ($solutionUniqueName -ne $null) {
      $postHeaders.Add('MSCRM.SolutionUniqueName', $solutionUniqueName)
   }
   
   $CreateRequest = @{
      Uri     = $baseURI + 'EntityDefinitions'
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body -Depth 5 # 5 should be enough for most cases, the default is 2.

   }
   $rh = Invoke-ResilientRestMethod -request $CreateRequest -returnHeader $true
   $url = $rh['OData-EntityId']
   $selectedString = $url | Select-String `
      -Pattern '[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}' `
      -AllMatches | % { $_.Matches }
   return [System.Guid]::New($selectedString.Value.ToString())
}

function Update-Table {
   param (
      [Parameter(Mandatory)] 
      [PSCustomObject]
      $table,
      [String] 
      $solutionUniqueName,
      [bool]
      $mergeLabels
   
   )
   if ($null -eq $table.LogicalName) {
      throw 'The table must include all the properties retrieved from Dataverse, including the LogicalName property.'
   }

   $uri = $baseURI + "EntityDefinitions(LogicalName='$($table.LogicalName)')"
   # Header for Update operations
   $updateHeaders = $baseHeaders.Clone()
   $updateHeaders.Add('Content-Type', 'application/json')
   $updateHeaders.Add('Consistency', 'Strong')
   if ($solutionUniqueName -ne $null) {
      $updateHeaders.Add('MSCRM.SolutionUniqueName', $solutionUniqueName)
   }
   if ($mergeLabels) {
      $updateHeaders.Add('MSCRM.MergeLabels ', $true)
   }
   $UpdateRequest = @{
      Uri     = $uri
      Method  = 'Put'
      Headers = $updateHeaders
      Body    = ConvertTo-Json $table `
         -Depth 5 
      # 5 should be enough for most cases, the default is 2.
   }
   Invoke-ResilientRestMethod $UpdateRequest | Out-Null
}

function Get-TableColumns {
   param (
      [Parameter(Mandatory)]
      [String]
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [String]
      $query
   )
   $pathToColumns = "EntityDefinitions(LogicalName='$tableLogicalName')/Attributes"

   $uri = $baseURI + $pathToColumns + $query
   # Header for GET operations that have annotations
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $null)
   # $getHeaders.Add('Prefer', 'odata.include-annotations="*"')
   $getHeaders.Add('Consistency', 'Strong')
   $RetrieveMultipleRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }
   Invoke-ResilientRestMethod $RetrieveMultipleRequest
}

function New-Column {
   param (
      [Parameter(Mandatory)] 
      [string]
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [hashtable]
      $column,
      [String] 
      $solutionUniqueName
   )

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')
   if ($solutionUniqueName -ne $null) {
      $postHeaders.Add('MSCRM.SolutionUniqueName', $solutionUniqueName)
   }
   
   $CreateRequest = @{
      Uri     = $baseURI + "EntityDefinitions(LogicalName='$tableLogicalName')/Attributes"
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $column `
         -Depth 10 # 5 should be enough for most cases, the default is 2.

   }
   $rh = Invoke-ResilientRestMethod -request $CreateRequest -returnHeader $true
   $url = $rh['OData-EntityId']
   $selectedString = Select-String -InputObject $url -Pattern '[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}'
   return [System.Guid]::New($selectedString.Matches.Value.ToString())
}

function Get-Column {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [String] 
      $logicalName,
      [Parameter(Mandatory)] 
      [String] 
      $type,
      [String] 
      $query
   )
   $typeName = switch ($type) {
      'BigInt' { 'BigIntAttributeMetadata' }
      'Boolean' { 'BooleanAttributeMetadata' }
      'DateTime' { 'DateTimeAttributeMetadata' }
      'Decimal' { 'DecimalAttributeMetadata' }
      'Double' { 'DoubleAttributeMetadata' }
      'File' { 'FileAttributeMetadata' }
      'Image' { 'ImageAttributeMetadata' }
      'Integer' { 'IntegerAttributeMetadata' }
      'Lookup' { 'LookupAttributeMetadata' }
      'ManagedProperty' { 'ManagedPropertyAttributeMetadata' }
      'Memo' { 'MemoAttributeMetadata' }
      'Money' { 'MoneyAttributeMetadata' }
      'String' { 'StringAttributeMetadata' }
      'EntityName' { 'EntityNameAttributeMetadata' }
      'UniqueIdentifier' { 'UniqueIdentifierAttributeMetadata' }
      'MultiSelectPicklist' { 'MultiSelectPicklistAttributeMetadata' }
      'Picklist' { 'PicklistAttributeMetadata' }
      'State' { 'StateAttributeMetadata' }
      'Status' { 'StatusAttributeMetadata' }
      Default {
         throw "The type '$type' is not supported."
      }
   }

   $pathToColumn = "EntityDefinitions(LogicalName='$tableLogicalName')"
   $pathToColumn += "/Attributes(LogicalName='$logicalName')/Microsoft.Dynamics.CRM.$typeName"
   $uri = $baseURI + $pathToColumn + $query
   # Header for GET operations that have annotations
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $null)
   $getHeaders.Add('Consistency', 'Strong')
   $RetrieveRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }
   Invoke-ResilientRestMethod $RetrieveRequest
}

function Update-Column {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [PSCustomObject]
      $column,
      [Parameter(Mandatory)] 
      [String] 
      $type,
      [String] 
      $solutionUniqueName,
      [bool]
      $mergeLabels
   )
   if ($null -eq $column.LogicalName) {
      throw 'The column must include all the properties retrieved from Dataverse, including the LogicalName property.'
   }

   $typeName = switch ($type) {
      'BigInt' { 'BigIntAttributeMetadata' }
      'Boolean' { 'BooleanAttributeMetadata' }
      'DateTime' { 'DateTimeAttributeMetadata' }
      'Decimal' { 'DecimalAttributeMetadata' }
      'Double' { 'DoubleAttributeMetadata' }
      'File' { 'FileAttributeMetadata' }
      'Image' { 'ImageAttributeMetadata' }
      'Integer' { 'IntegerAttributeMetadata' }
      'Lookup' { 'LookupAttributeMetadata' }
      'ManagedProperty' { 'ManagedPropertyAttributeMetadata' }
      'Memo' { 'MemoAttributeMetadata' }
      'Money' { 'MoneyAttributeMetadata' }
      'String' { 'StringAttributeMetadata' }
      'UniqueIdentifier' { 'UniqueIdentifierAttributeMetadata' }
      Default {
         throw "The type '$type' is not supported."
      }
   }
   if($null -eq $column.'@odata.type'){
      # The @odata.type property is required for this operation.
      # It is not returned when the column is retrieved from Dataverse.
      # Add it here.
      $column | Add-Member `
         -MemberType NoteProperty `
         -Name '@odata.type' `
         -Value ('Microsoft.Dynamics.CRM.' + $typeName)
   }

   if($null -ne $column.'@odata.context'){
      # Including the @odata.context property in the request body will cause the request to fail. 
      # Remove it before sending the request.
      $column.PSObject.Properties.Remove('@odata.context')
   }

   $pathToColumn = "EntityDefinitions(LogicalName='$tableLogicalName')"
   $pathToColumn += "/Attributes(LogicalName='$($column.LogicalName)')"
   $uri = $baseURI + $pathToColumn
   # Header for Update operations
   $updateHeaders = $baseHeaders.Clone()
   $updateHeaders.Add('Content-Type', 'application/json')
   $updateHeaders.Add('Consistency', 'Strong')
   if ($solutionUniqueName -ne $null) {
      $updateHeaders.Add('MSCRM.SolutionUniqueName', $solutionUniqueName)
   }
   if ($mergeLabels) {
      $updateHeaders.Add('MSCRM.MergeLabels', $true)
   }
   $UpdateRequest = @{
      Uri     = $uri
      Method  = 'Put'
      Headers = $updateHeaders
      Body    = ConvertTo-Json $column `
         -Depth 5 
      # 5 should be enough for most cases, the default is 2.
   }
   Invoke-ResilientRestMethod $UpdateRequest | Out-Null
}

function Update-OptionValue{
   param (
      [Parameter(Mandatory)] 
      [String] 
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [string]
      $columnLogicalName,
      [Parameter(Mandatory)] 
      [int] 
      $value,
      [Parameter(Mandatory)] 
      [String] 
      $label,
      [Parameter(Mandatory)] 
      [int] 
      $languageCode,
      [bool]
      $mergeLabels = $false,
      [String] 
      $solutionUniqueName
   )

   $body = @{
      EntityLogicalName = $tableLogicalName
      AttributeLogicalName = $columnLogicalName
      Value = $value
      Label = @{
         LocalizedLabels = @(
            @{
               Label = $label
               LanguageCode = $languageCode
            }
         )
      }
      MergeLabels = $mergeLabels
   }

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')
   if ($solutionUniqueName -ne $null) {
      $postHeaders.Add('MSCRM.SolutionUniqueName', $solutionUniqueName)
   }
   
   $UpdateOptionValueRequest = @{
      Uri     = $baseURI + 'UpdateOptionValue'
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body `
         -Depth 5 # 5 should be enough for most cases, the default is 2.

   }
   Invoke-ResilientRestMethod -request $UpdateOptionValueRequest | Out-Null
}



