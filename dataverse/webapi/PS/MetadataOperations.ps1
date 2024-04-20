. $PSScriptRoot\Core.ps1

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
   Invoke-ResilientRestMethod $RetrieveMultipleRequest | 
   Select-Object -ExpandProperty value
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
   if ($null -eq $column.'@odata.type') {
      # The @odata.type property is required for this operation.
      # It is not returned when the column is retrieved from Dataverse.
      # Add it here.
      $column | Add-Member `
         -MemberType NoteProperty `
         -Name '@odata.type' `
         -Value ('Microsoft.Dynamics.CRM.' + $typeName)
   }

   if ($null -ne $column.'@odata.context') {
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

function Update-OptionValue {
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
      EntityLogicalName    = $tableLogicalName
      AttributeLogicalName = $columnLogicalName
      Value                = $value
      Label                = @{
         LocalizedLabels = @(
            @{
               Label        = $label
               LanguageCode = $languageCode
            }
         )
      }
      MergeLabels          = $mergeLabels
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

function New-OptionValue {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [string]
      $columnLogicalName,
      [Parameter(Mandatory)] 
      [String] 
      $label,
      [Parameter(Mandatory)] 
      [int] 
      $languageCode,
      [int] 
      $value,
      [String] 
      $solutionUniqueName
   )

   $body = @{
      EntityLogicalName    = $tableLogicalName
      AttributeLogicalName = $columnLogicalName
      Value                = $value
      Label                = @{
         LocalizedLabels = @(
            @{
               Label        = $label
               LanguageCode = $languageCode
            }
         )
      }
      SolutionUniqueName   = $solutionUniqueName
   }

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')

   $CreateOptionValueRequest = @{
      Uri     = $baseURI + 'InsertOptionValue'
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body `
         -Depth 5 # 5 should be enough for most cases, the default is 2.

   }
   Invoke-ResilientRestMethod -request $CreateOptionValueRequest | 
   Select-Object -ExpandProperty NewOptionValue
}

function Update-OptionsOrder {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [string]
      $columnLogicalName,
      [Parameter(Mandatory)] 
      [int[]] 
      $values,
      [String] 
      $solutionUniqueName
   )

   $body = @{
      EntityLogicalName    = $tableLogicalName
      AttributeLogicalName = $columnLogicalName
      Values               = $values
   }
   if ($null -ne $solutionUniqueName) {
      $body.SolutionUniqueName = $solutionUniqueName
   }

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')
   
   $UpdateOptionsOrderRequest = @{
      Uri     = $baseURI + 'OrderOption'
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body `
         -Depth 5 # 5 should be enough for most cases, the default is 2.

   }
   Invoke-ResilientRestMethod -request $UpdateOptionsOrderRequest | Out-Null
}

function Remove-OptionValue {
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
      [String] 
      $solutionUniqueName
   )
   
   $body = @{
      EntityLogicalName    = $tableLogicalName
      AttributeLogicalName = $columnLogicalName
      Value                = $value
   }
   if ($null -ne $solutionUniqueName) {
      $body.SolutionUniqueName = $solutionUniqueName
   }
   
   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')

   $DeleteOptionValueRequest = @{
      Uri     = $baseURI + 'DeleteOptionValue'
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body `
         -Depth 5 # 5 should be enough for most cases, the default is 2.

   }
   Invoke-ResilientRestMethod -request $DeleteOptionValueRequest | Out-Null
}

function New-StatusOption {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $tableLogicalName,
      [Parameter(Mandatory)] 
      [String] 
      $label,
      [Parameter(Mandatory)] 
      [int] 
      $languageCode,
      [Parameter(Mandatory)] 
      [int] 
      $stateCode,
      [Nullable[System.Int32]]
      $value,
      [string] 
      $color,
      [string] 
      $description,
      [String] 
      $solutionUniqueName
   )

   $body = @{
      EntityLogicalName    = $tableLogicalName
      AttributeLogicalName = 'statuscode'
      StateCode            = $stateCode
      Label                = @{
         LocalizedLabels = @(
            @{
               Label        = $label
               LanguageCode = $languageCode
            }
         )
      }
   }
   if ($null -ne $value) {
      $body.Value = $value
   }
   if ($color) {
      $body.Color = $color
   }
   if ($description) {
      $body.Description = @{
         LocalizedLabels = @(
            @{
               Label        = $description
               LanguageCode = $languageCode
            }
         )
      }
   }
   if ($solutionUniqueName) {
      $body.SolutionUniqueName = $solutionUniqueName
   }

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')

   $CreateStatusOptionRequest = @{
      Uri     = $baseURI + 'InsertStatusValue'
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body `
         -Depth 5 # 5 should be enough for most cases, the default is 2.

   }
   Invoke-ResilientRestMethod -request $CreateStatusOptionRequest | 
   Select-Object -ExpandProperty NewOptionValue
}

function New-GlobalOptionSet {
   param (
      [Parameter(Mandatory)] 
      [hashtable]
      $optionSet,
      [string] 
      $solutionUniqueName
   )

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')
   if ($solutionUniqueName) {
      $postHeaders.Add('MSCRM.SolutionUniqueName', $solutionUniqueName)
   }
   
   $CreateRequest = @{
      Uri     = $baseURI + 'GlobalOptionSetDefinitions'
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $optionSet `
         -Depth 5 # 5 should be enough for most cases, the default is 2.

   }
   $rh = Invoke-ResilientRestMethod `
      -request $CreateRequest `
      -returnHeader $true
   $url = $rh['OData-EntityId']
   $selectedString = Select-String `
      -InputObject $url `
      -Pattern '[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}'
   return [System.Guid]::New($selectedString.Matches.Value.ToString())
}

function Get-GlobalOptionSet {
   param (
      [String] 
      $name,
      [guid] 
      $id,
      [string]
      $type,
      [String] 
      $query
   )

   $key = ''
   if ($id) {
      $key = "($id)"
   }
   elseif ($name) {
      $key = "(Name='$name')"
   }
   else {
      throw 'Either the name or the id of the global option set must be provided.'
   }

   $typeName = switch ($type) {
      'OptionSet' { '/Microsoft.Dynamics.CRM.OptionSetMetadata' }
      'Boolean' { '/Microsoft.Dynamics.CRM.BooleanOptionSetMetadata' }
      Default {
         ''
         # If the type isn't set the function will not enable expanding the options.
      }
   }


   $uri = $baseURI + 'GlobalOptionSetDefinitions' + $key + $typeName + $query
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('Consistency', 'Strong')
   $RetrieveRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }

   try {
      Invoke-ResilientRestMethod $RetrieveRequest
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      $statuscode = $_.Exception.Response.StatusCode
      # 404 errors only
      if ($statuscode -eq 'NotFound') {
         # Return $null if the global option set is not found
         return $null
      }
      else {
         throw $_
      }
   }
   catch {
      throw $_
   }
}

function New-CustomerRelationship{
 param(
      [Parameter(Mandatory)]
      [hashtable]
      $lookup,
      [Parameter(Mandatory)]
      [hashtable[]]
      $oneToManyRelationships,
      [String]
      $solutionUniqueName
 )
   $body = @{
      Lookup = $lookup
      OneToManyRelationships = $oneToManyRelationships
   }
   if($solutionUniqueName){
      $body.SolutionUniqueName = $solutionUniqueName
   }

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')
   
   $CreateRequest = @{
      Uri     = $baseURI + 'CreateCustomerRelationships'
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body `
         -Depth 5 # 5 should be enough for most cases, the default is 2.
   }
  Invoke-ResilientRestMethod -request $CreateRequest 

}

function Get-Relationship{
   param(
      [String]
      $schemaName,
      [guid]
      $id,
      [String]
      $type,
      [String]
      $query
   )

   $typeName = switch ($type) {
      'OneToMany' { '/Microsoft.Dynamics.CRM.OneToManyRelationshipMetadata' }
      'ManyToOne' { '/Microsoft.Dynamics.CRM.ManyToOneRelationshipMetadata' }
      'ManyToMany' { '/Microsoft.Dynamics.CRM.ManyToManyRelationshipMetadata' }
      Default {
         ''
         # If the type isn't set the function will not enable 
         # expanding or selecting type specific properties.
      }
   }

   $key = ''
   if ($id) {
      $key = "($id)"
   } elseif ($schemaName) {
      $key = "(SchemaName='$schemaName')"
   } else {
      throw 'Either the schemaName or the id of the relationship must be provided.'
   }

   $uri = $baseURI + 'RelationshipDefinitions' + $key + $typeName + $query
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

function Get-CanBeReferenced{
   param(
      [Parameter(Mandatory)]
      [String]
      $tableLogicalName
   )

   $uri = $baseURI + 'CanBeReferenced'
   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')
   $body = @{
      EntityName = $tableLogicalName
   }

   $CanBeReferencedRequest = @{
      Uri     = $uri
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body
   }

   Invoke-ResilientRestMethod -request $CanBeReferencedRequest | 
   Select-Object -ExpandProperty CanBeReferenced
}

function Get-CanBeReferencing{
   param(
      [Parameter(Mandatory)]
      [String]
      $tableLogicalName
   )

   $uri = $baseURI + 'CanBeReferencing'
   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')
   $body = @{
      EntityName = $tableLogicalName
   }

   $CanBeReferencingRequest = @{
      Uri     = $uri
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body
   }

   Invoke-ResilientRestMethod -request $CanBeReferencingRequest | 
   Select-Object -ExpandProperty CanBeReferencing
}


