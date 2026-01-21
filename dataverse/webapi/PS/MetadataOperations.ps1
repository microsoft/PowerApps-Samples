. $PSScriptRoot\Core.ps1

<#
.SYNOPSIS
   A function to export a solution from Dataverse.

.DESCRIPTION
   This function sends a POST request to a specified URI to export a solution from Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER solutionName
   The name of the solution to be exported.

.PARAMETER managed
   A boolean value indicating whether the solution is managed.

.PARAMETER exportAutoNumberingSettings
   A boolean value indicating whether to export auto-numbering settings.

.PARAMETER exportCalendarSettings
   A boolean value indicating whether to export calendar settings.

.PARAMETER exportCustomizationSettings
   A boolean value indicating whether to export customization settings.

.PARAMETER exportEmailTrackingSettings
   A boolean value indicating whether to export email tracking settings.

.PARAMETER exportGeneralSettings
   A boolean value indicating whether to export general settings.

.PARAMETER exportMarketingSettings
   A boolean value indicating whether to export marketing settings.

.PARAMETER exportOutlookSynchronizationSettings
   A boolean value indicating whether to export Outlook synchronization settings.

.PARAMETER exportRelationshipRoles
   A boolean value indicating whether to export relationship roles.

.PARAMETER exportIsvConfig
   A boolean value indicating whether to export ISV configuration.

.PARAMETER exportSales
   A boolean value indicating whether to export sales settings.

.PARAMETER exportExternalApplications
   A boolean value indicating whether to export external applications settings.

.PARAMETER exportComponentsParams
   A hashtable containing additional parameters for exporting components.

.EXAMPLE
   $solutionFile = Export-Solution `
      -solutionName 'mySolution'`
      -managed $true

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns the exported solution as a byte array.
#>
function Export-Solution {
   param (
      [Parameter(Mandatory)] 
      [string]
      $solutionName,
      [Parameter(Mandatory)] 
      [bool] 
      $managed,
      [bool] 
      $exportAutoNumberingSettings,
      [bool] 
      $exportCalendarSettings,
      [bool] 
      $exportCustomizationSettings,
      [bool] 
      $exportEmailTrackingSettings,
      [bool] 
      $exportGeneralSettings,
      [bool] 
      $exportMarketingSettings,
      [bool] 
      $exportOutlookSynchronizationSettings,
      [bool] 
      $exportRelationshipRoles,
      [bool] 
      $exportIsvConfig,
      [bool] 
      $exportSales,
      [bool] 
      $exportExternalApplications,
      [hashtable] 
      $exportComponentsParams
   )

   $uri = $baseURI + 'ExportSolution'
   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')
   $body = @{
      SolutionName                         = $solutionName
      Managed                              = $managed
      ExportAutoNumberingSettings          = $exportAutoNumberingSettings
      ExportCalendarSettings               = $exportCalendarSettings
      ExportCustomizationSettings          = $exportCustomizationSettings
      ExportEmailTrackingSettings          = $exportEmailTrackingSettings
      ExportGeneralSettings                = $exportGeneralSettings
      ExportMarketingSettings              = $exportMarketingSettings
      ExportOutlookSynchronizationSettings = $exportOutlookSynchronizationSettings
      ExportRelationshipRoles              = $exportRelationshipRoles
      ExportIsvConfig                      = $exportIsvConfig
      ExportSales                          = $exportSales
      ExportExternalApplications           = $exportExternalApplications
   }
   if ($exportComponentsParams) {
      $body.ExportComponentsParams = $exportComponentsParams
   }
   
   $ExportSolutionRequest = @{
      Uri     = $uri
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body -Depth 5 # 5 should be enough for most cases, the default is 2.
   }

   $encodedString = Invoke-ResilientRestMethod -request $ExportSolutionRequest |
   Select-Object -ExpandProperty ExportSolutionFile

   return [System.Convert]::FromBase64String($encodedString)

}
<#
.SYNOPSIS
   A function to check if a table can be referenced in Dataverse.

.DESCRIPTION
   This function sends a POST request to a specified URI to check if a table can be referenced in Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table to be checked.

.EXAMPLE
   Get-CanBeReferenced -tableLogicalName "account"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns a boolean value indicating whether the table can be referenced.
#>
function Get-CanBeReferenced {
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
<#
.SYNOPSIS
   A function to check if a table can be referencing in Dataverse.

.DESCRIPTION
   This function sends a POST request to a specified URI to check if a table can be referencing in Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table to be checked.

.EXAMPLE
   Get-CanBeReferencing -tableLogicalName "account"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns a boolean value indicating whether the table can be referencing.
#>
function Get-CanBeReferencing {
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
<#
.SYNOPSIS
   A function to check if a table can have many-to-many relationships in Dataverse.

.DESCRIPTION
   This function sends a POST request to a specified URI to check if a table can have many-to-many relationships in Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table to be checked.

.EXAMPLE
   Get-CanManyToMany -tableLogicalName "account"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns a boolean value indicating whether the table can have many-to-many relationships.
#>
function Get-CanManyToMany {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $tableLogicalName
   )

   $uri = $baseURI + 'CanManyToMany'
   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')
   $body = @{
      EntityName = $tableLogicalName
   }

   $CanManyToManyRequest = @{
      Uri     = $uri
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body
   }

   Invoke-ResilientRestMethod -request $CanManyToManyRequest | 
   Select-Object -ExpandProperty CanManyToMany
}
<#
.SYNOPSIS
   A function to retrieve a column from a Dataverse table.

.DESCRIPTION
   This function sends a GET request to a specified URI to retrieve a column from a Dataverse table. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table from which the column will be retrieved.

.PARAMETER logicalName
   The logical name of the column to be retrieved.

.PARAMETER type
   The type of the column to be retrieved. This function supports the following types: 
   
    - 'BigInt'
    - 'Boolean'
    - 'DateTime'
    - 'Decimal'
    - 'Double'
    - 'File'
    - 'Image'
    - 'Integer'
    - 'Lookup'
    - 'ManagedProperty'
    - 'Memo'
    - 'Money'
    - 'String'
    - 'EntityName'
    - 'UniqueIdentifier'
    - 'MultiSelectPicklist'
    - 'Picklist'
    - 'State'
    - 'Status'

.PARAMETER query
   The query string to be appended to the base URI to form the complete URI for the GET request.

.EXAMPLE
   Get-Column `
      -tableLogicalName 'account' `
      -logicalName 'accountid' `
      -type 'UniqueIdentifier" `
      -query "?`$select=SchemaName,DisplayName,AttributeType"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns the details of the specified column.
#>
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
<#
.SYNOPSIS
   A function to retrieve a global option set from Dataverse.

.DESCRIPTION
   This function sends a GET request to a specified URI to retrieve a global option set from Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER name
   The name of the global option set to be retrieved. Either this or the id must be provided.

.PARAMETER id
   The GUID of the global option set to be retrieved. Either this or the name must be provided.

.PARAMETER type
   The type of the global option set to be retrieved. 
   It can be 'OptionSet' or 'Boolean'. 
   If this parameter is not provided, the function will not enable expanding the options.

.PARAMETER query
   An OData query string to filter the global option set to be retrieved.

.EXAMPLE
   Get-GlobalOptionSet -name "new_globaloptionset" -type "OptionSet"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns the global option set if found, or $null if not found.
   If the server returns an error other than 404, the function will throw an exception.
#>
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
<#
.SYNOPSIS
   A function to retrieve a relationship from Dataverse.

.DESCRIPTION
   This function sends a GET request to a specified URI to retrieve a relationship from Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER schemaName
   The schema name of the relationship to be retrieved. Either this or the id must be provided.

.PARAMETER id
   The GUID of the relationship to be retrieved. Either this or the schema name must be provided.

.PARAMETER type
   The type of the relationship to be retrieved. 
   It can be 'OneToMany', 'ManyToOne', or 'ManyToMany'. 
   If this parameter is not provided, the function will not enable expanding or selecting type properties.

.PARAMETER query
   An OData query string to filter the relationship to be retrieved.

.EXAMPLE
   Get-Relationship -schemaName "new_account_customer" -type "OneToMany"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns the relationship if found.
#>
function Get-Relationship {
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
   }
   elseif ($schemaName) {
      $key = "(SchemaName='$schemaName')"
   }
   else {
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
<#
.SYNOPSIS
   A function to retrieve relationships from Dataverse.

.DESCRIPTION
   This function sends a GET request to a specified URI to retrieve relationships from Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER query
   An OData query string to filter the relationships to be retrieved.

.PARAMETER isManyToMany
   A boolean value indicating whether to retrieve many-to-many relationships. 
   If this parameter is set to true, many-to-many relationships are retrieved; 
   otherwise, one-to-many relationships are retrieved.

.EXAMPLE
   $relationshipQuery = "?`$filter=SchemaName eq '"
   $relationshipQuery += 'sample_BankAccount_Contacts'
   $relationshipQuery += "'&`$select=SchemaName"
   
   $relationshipQueryResults = (Get-Relationships `
         -query $relationshipQuery `
         -isManyToMany $false).value

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns the relationships that match the provided query.
#>
function Get-Relationships {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $query,
      [Parameter(Mandatory)] 
      [bool] 
      $isManyToMany
   )

   $type = $isManyToMany ? 'ManyToManyRelationshipMetadata' : 'OneToManyRelationshipMetadata'

   $uri = $baseURI + 'RelationshipDefinitions/Microsoft.Dynamics.CRM.' + $type + $query
   # Header for GET operations that have annotations
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $null)
   $getHeaders.Add('Consistency', 'Strong')
   $RetrieveMultipleRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }
   Invoke-ResilientRestMethod $RetrieveMultipleRequest
}
<#
.SYNOPSIS
   A function to get a table definition from Dataverse.

.DESCRIPTION
   This function sends a GET request to a specified URI to retrieve table data. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER logicalName
   The logical name of the table to be retrieved.

.PARAMETER query
   The query string to be appended to the base URI to form the complete URI for the GET request.

.EXAMPLE
      $bankAccountTable = Get-Table -logicalName 'new_bankaccount' `
         -query "?`$select=SchemaName,DisplayName,TableType"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
#>
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
<#
.SYNOPSIS
   A function to retrieve the columns of a table in Dataverse.

.DESCRIPTION
   This function sends a GET request to a specified URI to retrieve the columns (attributes) of a Dataverse table. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table whose columns are to be retrieved.

.PARAMETER query
   The query string to be appended to the base URI to form the complete URI for the GET request.

.EXAMPLE
   Get-TableColumns -tableLogicalName 'account' -query "?`$filter=SchemaName eq 'Name'"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns the columns of the specified table.
#>
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
<#
.SYNOPSIS
   A function to get table definitions from Dataverse.

.DESCRIPTION
   This function sends a GET request to a specified URI to retrieve table data. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER query
   The query string to be appended to the base URI to form the complete URI for the GET request.

.EXAMPLE
   $tableQuery = "?`$filter=SchemaName eq '"
   $tableQuery += 'new_BankAccount'
   $tableQuery += "'&`$select=SchemaName,DisplayName,TableType"
   
   $tableQueryResults = (Get-Tables `
         -query $tableQuery).value

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
#>
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
   $getHeaders.Add('Consistency', 'Strong')
   $RetrieveMultipleRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }
   Invoke-ResilientRestMethod $RetrieveMultipleRequest
}
<#
.SYNOPSIS
   A function to get valid tables for many-to-many relationships in Dataverse.

.DESCRIPTION
   This function sends a GET request to a specified URI to retrieve valid tables for many-to-many relationships in Dataverse. 
   It uses resilient REST method to handle potential network issues.

.EXAMPLE
   Get-ValidManyToManyTables

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns an array of strings, each string being the logical name of a valid table for many-to-many relationships.
#>
function Get-ValidManyToManyTables {

   $uri = $baseURI + 'GetValidManyToMany'
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('Consistency', 'Strong')

   $GetValidManyToManyEntitiesRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }

   Invoke-ResilientRestMethod -request $GetValidManyToManyEntitiesRequest | 
   Select-Object -ExpandProperty EntityNames
}
<#
.SYNOPSIS
   A function to get valid referencing tables for a specified table in Dataverse.

.DESCRIPTION
   This function sends a GET request to a specified URI to retrieve valid referencing tables for a specified table in Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table for which to retrieve valid referencing tables.

.EXAMPLE
   Get-ValidReferencingTables -tableLogicalName "account"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns an array of strings, each string being the logical name of a valid referencing table.
#>
function Get-ValidReferencingTables {
   param(
      [Parameter(Mandatory)]
      [String]
      $tableLogicalName
   )

   $uri = $baseURI + "GetValidReferencingEntities(ReferencedEntityName='$tableLogicalName')"
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('Consistency', 'Strong')

   $GetValidReferencingEntitiesRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }

   Invoke-ResilientRestMethod -request $GetValidReferencingEntitiesRequest | 
   Select-Object -ExpandProperty EntityNames
}
<#
.SYNOPSIS
   A function to import a solution into Dataverse.

.DESCRIPTION
   This function sends a POST request to a specified URI to import a solution into Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER customizationFile
   The solution file to be imported, as a byte array.

.PARAMETER overwriteUnmanagedCustomizations
   A boolean value indicating whether to overwrite unmanaged customizations.

.PARAMETER importJobId
   The GUID of the import job.

.PARAMETER publishWorkflows
   A boolean value indicating whether to publish workflows.

.PARAMETER convertToManaged
   A boolean value indicating whether to convert the solution to managed.

.PARAMETER skipProductUpdateDependencies
   A boolean value indicating whether to skip product update dependencies.

.PARAMETER holdingSolution
   A boolean value indicating whether the solution is a holding solution.

.PARAMETER componentParameters
   An array of hashtables containing additional parameters for importing components.

.PARAMETER solutionParameters
   A hashtable containing additional parameters for importing the solution.

.EXAMPLE
   $importJobId = New-Guid

   Import-Solution `
      -customizationFile ([System.IO.File]::ReadAllBytes("C:\path\to\solution.zip")) `
      -overwriteUnmanagedCustomizations $false `
      -importJobId $importJobId

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function does not return a value.
#>
function Import-Solution {
   param (
      [Parameter(Mandatory)] 
      [byte[]]
      $customizationFile,
      [Parameter(Mandatory)] 
      [bool] 
      $overwriteUnmanagedCustomizations,
      [Parameter(Mandatory)]
      [guid]
      $importJobId,
      [bool] 
      $publishWorkflows,
      [bool] 
      $convertToManaged,
      [bool] 
      $skipProductUpdateDependencies,
      [bool] 
      $holdingSolution,
      [hashtable[]] 
      $componentParameters,
      [hashtable] 
      $solutionParameters
   )

   $uri = $baseURI + 'ImportSolution'
   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   $postHeaders.Add('Consistency', 'Strong')
   $body = @{
      CustomizationFile                = [System.Convert]::ToBase64String($customizationFile)
      OverwriteUnmanagedCustomizations = $overwriteUnmanagedCustomizations
      PublishWorkflows                 = $publishWorkflows
      ImportJobId                      = $importJobId
      ConvertToManaged                 = $convertToManaged
      SkipProductUpdateDependencies    = $skipProductUpdateDependencies
      HoldingSolution                  = $holdingSolution
   }

   if ($componentParameters) {
      $body.ComponentParameters = $componentParameters
   }
   if ($solutionParameters) {
      $body.SolutionParameters = $solutionParameters
   }

   $ImportSolutionRequest = @{
      Uri     = $uri
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body -Depth 5 # 5 should be enough for most cases, the default is 2.
   }

   Invoke-ResilientRestMethod -request $ImportSolutionRequest
}
<#
.SYNOPSIS
   A function to create a new column in a Dataverse table.

.DESCRIPTION
   This function sends a POST request to a specified URI to create a new column in a Dataverse table. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table where the new column will be created.

.PARAMETER column
   A hashtable that represents the new column to be created. It should contain the details of the column.

.PARAMETER solutionUniqueName
   The unique name of the solution where the table is located. If this parameter is not provided, the column will be created in the table in the default solution.

.EXAMPLE
$boolColumnData = @{
      '@odata.type' = 'Microsoft.Dynamics.CRM.BooleanAttributeMetadata'
      SchemaName    = "sample_Boolean"
      DefaultValue  = $false
      RequiredLevel = @{
         Value = 'None'
      }
      DisplayName   = @{
         '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
         LocalizedLabels = @(
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
               Label         = 'Sample Boolean'
               LanguageCode  = $languageCode
            }
         )
      }
      Description   = @{
         '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
         LocalizedLabels = @(
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
               Label         = 'Sample Boolean column description'
               LanguageCode  = $languageCode
            }
         )
      }
      OptionSet     = @{
         '@odata.type' = 'Microsoft.Dynamics.CRM.BooleanOptionSetMetadata'
         TrueOption    = @{
            Value = 1
            Label = @{
               '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
               LocalizedLabels = @(
                  @{
                     '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                     Label         = 'True'
                     LanguageCode  = $languageCode
                  }
               )
            }
         }
         FalseOption   = @{
            Value = 0
            Label = @{
               '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
               LocalizedLabels = @(
                  @{
                     '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                     Label         = 'False'
                     LanguageCode  = $languageCode
                  }
               )
            }
         }
      }
   }

   # Create the column
   $boolColumnId = New-Column `
      -tableLogicalName 'sample_bankaccount' `
      -column $boolColumnData `
      -solutionUniqueName 'mysolution'

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns the GUID of the newly created column.
#>
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
<#
.SYNOPSIS
   A function to create a new customer relationship in Dataverse.

.DESCRIPTION
   This function sends a POST request to a specified URI to create a new customer relationship in Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER lookup
   A hashtable containing the details of the lookup field for the customer relationship.

.PARAMETER oneToManyRelationships
   An array of hashtables, each containing the details of a one-to-many relationship for the customer relationship.

.PARAMETER solutionUniqueName
   The unique name of the solution where the customer relationship will be created. If this parameter is not provided, the customer relationship will be created in the default solution.

.EXAMPLE

$customerLookupData = @{
      SchemaName    = "sample_CustomerId"
      RequiredLevel = @{
         Value = 'None'
      }
      DisplayName   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Bank Account owner'
               LanguageCode = 1033
            }
         )
      }
      Description   = @{
         LocalizedLabels = @(
            @{
               Label        = 'The owner of the bank account'
               LanguageCode = 1033
            }
         )
      }
      Targets       = @('account', 'contact')
   }

   $customerRelationships = @(
      @{
         SchemaName        = "sample_BankAccount_Customer_Account"
         ReferencedEntity  = 'account'
         ReferencingEntity = 'sample_bankaccount'
         RelationshipType  = 'OneToManyRelationship'
      },
      @{
         SchemaName        = "sample_BankAccount_Customer_Contact"
         ReferencedEntity  = 'contact'
         ReferencingEntity = 'sample_bankaccount'
         RelationshipType  = 'OneToManyRelationship'
      }
   )

   $response = New-CustomerRelationship `
   -lookup $customerLookupData `
   -oneToManyRelationships $customerRelationships `
   -solutionUniqueName $solutionData.uniquename
   
   
   $customerLookupRelationshipIds = $response.RelationshipIds
   $customerLookupId = $response.AttributeId

.NOTES
   The function returns a 
   [CreateCustomerRelationshipsResponse complex type](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/createcustomerrelationshipsresponse) 
   which includes the ID values of the lookup column and the relationships created to support it.
#>
function New-CustomerRelationship {
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
      Lookup                 = $lookup
      OneToManyRelationships = $oneToManyRelationships
   }
   if ($solutionUniqueName) {
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
<#
.SYNOPSIS
   A function to create a new global option set in Dataverse.

.DESCRIPTION
   This function sends a POST request to a specified URI to create a new global option set in Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER optionSet
   A hashtable containing the details of the global option set to be created.

.PARAMETER solutionUniqueName
   The unique name of the solution where the global option set will be created. If this parameter is not provided, the global option set will be created in the default solution.

.EXAMPLE
   $colorsGlobalOptionSetData = @{
      '@odata.type' = 'Microsoft.Dynamics.CRM.OptionSetMetadata'
      Name          = "sample_colors"
      DisplayName   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Colors'
               LanguageCode = 1033
            }
         )
      }
      Description   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Color Choice description'
               LanguageCode = 1033
            }
         )
      }
      IsGlobal      = $true
      Options       = @(
         @{
            Label = @{
               LocalizedLabels = @(
                  @{
                     Label        = 'Red'
                     LanguageCode = 1033
                  }
               )
            }
            Value = 100000000
         },
         @{
            Label = @{
               LocalizedLabels = @(
                  @{
                     Label        = 'Yellow'
                     LanguageCode = 1033
                  }
               )
            }
            Value = 100000001
         },
         @{
            Label = @{
               LocalizedLabels = @(
                  @{
                     Label        = 'Green'
                     LanguageCode = 1033
                  }
               )
            }
            Value = 100000002
         }
      )
   }
   New-GlobalOptionSet `
      -optionSet $colorsGlobalOptionSetData `
      -solutionUniqueName "MySolution"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns the GUID of the newly created global option set.
#>
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
<#
.SYNOPSIS
   A function to create a new option value in a column in a Dataverse table.

.DESCRIPTION
   This function sends a POST request to a specified URI to create a new option value in a column in a Dataverse table. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table where the column is located.

.PARAMETER columnLogicalName
   The logical name of the column where the new option value will be created.

.PARAMETER label
   The label for the new option value.

.PARAMETER languageCode
   The language code for the label.

.PARAMETER value
   The value for the new option.

.PARAMETER solutionUniqueName
   The unique name of the solution where the table is located. If this parameter is not provided, the new option value will be created in the table in the default solution.

.EXAMPLE
   New-OptionValue `
   -tableLogicalName 'sample_bankaccount' `
   -columnLogicalName 'sample_picklist' `
   -label 'Echo' `
   -languageCode 1033 `
   -solutionUniqueName 'mysolution'

.RETURN
   The function returns the value of the newly created option.

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
#>
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
<#
.SYNOPSIS
   A function to create a new relationship in Dataverse.

.DESCRIPTION
   This function sends a POST request to a specified URI to create a new relationship in Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER relationship
   A hashtable containing the details of the relationship to be created.

.PARAMETER solutionUniqueName
   The unique name of the solution where the relationship will be created. If this parameter is not provided, the relationship will be created in the default solution.

.EXAMPLE
   $oneToManyRelationshipData = @{
      '@odata.type'               = 'Microsoft.Dynamics.CRM.OneToManyRelationshipMetadata'
      SchemaName                  = 'sample_BankAccount_Contacts'
      ReferencedAttribute         = 'sample_bankaccountid'
      ReferencedEntity            = 'sample_bankaccount'
      ReferencingEntity           = 'contact'
      Lookup                      = @{
         SchemaName  = 'sample_BankAccountId'
         DisplayName = @{
            LocalizedLabels = @(
               @{
                  Label        = 'Bank Account'
                  LanguageCode = 1033
               }
            )
         }
         Description = @{
            LocalizedLabels = @(
               @{
                  Label        = 'The bank account this contact has access to.'
                  LanguageCode = 1033
               }
            )
         }
      }
      AssociatedMenuConfiguration = @{
         Behavior = 'UseLabel'
         Group    = 'Details'
         Label    = @{
            LocalizedLabels = @(
               @{
                  Label        = 'Cardholders'
                  LanguageCode = 1033
               }
            )
         }
         Order    = 10000
      }
      CascadeConfiguration        = @{
         Assign     = 'NoCascade'
         Share      = 'NoCascade'
         Unshare    = 'NoCascade'
         RollupView = 'NoCascade'
         Reparent   = 'NoCascade'
         Delete     = 'RemoveLink'
         Merge      = 'NoCascade'
      }

   }
   New-Relationship `
      -relationship $oneToManyRelationshipData `
      -solutionUniqueName 'MySolution'

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns the GUID of the created relationship.
#>
function New-Relationship {
   param (
      [Parameter(Mandatory)] 
      [hashtable]
      $relationship,
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
      Uri     = $baseURI + 'RelationshipDefinitions'
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $relationship -Depth 5 # 5 should be enough for most cases, the default is 2.

   }
   $rh = Invoke-ResilientRestMethod -request $CreateRequest -returnHeader $true
   $url = $rh['OData-EntityId']
   $selectedString = $url | Select-String `
      -Pattern '[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}' `
      -AllMatches | % { $_.Matches }
   return [System.Guid]::New($selectedString.Value.ToString())
}
<#
.SYNOPSIS
   A function to create a new status option in a Dataverse table column.

.DESCRIPTION
   This function sends a POST request to a specified URI to create a new status option in a Dataverse table. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the Dataverse table where the status option will be created.

.PARAMETER label
   The label of the new status option.

.PARAMETER languageCode
   The language code for the label and description of the new status option.

.PARAMETER stateCode
   The state code of the new status option.

.PARAMETER value
   The value of the new status option. If this parameter is not provided, a value will be automatically assigned.

.PARAMETER color
   The color of the new status option. If this parameter is not provided, a default color will be used.

.PARAMETER description
   The description of the new status option.

.PARAMETER solutionUniqueName
   The unique name of the solution where the Dataverse table is located. If this parameter is not provided, the status option will be created in the table in the default solution.

.EXAMPLE
   New-StatusOption `
      -tableLogicalName "account" `
      -label "New Status" `
      -languageCode 1033 `
      -stateCode 1 `
      -value 100000000 `
      -color "FF0000" `
      -description "This is a new status option" `
      -solutionUniqueName "MySolution"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns the value of the new status option.
#>
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
<#
.SYNOPSIS
   A function to create a new table Dataverse.

.DESCRIPTION
   This function sends a POST request to a specified URI to create a new Dataverse table. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER body
   The body of the POST request, which should be a hashtable containing the details of the table to be created.

.PARAMETER solutionUniqueName
   The unique name of the solution where the table will be created. If this parameter is not provided, the table will be created in the default solution.

.EXAMPLE
   $tableDetails = @{
      '@odata.type'         = "Microsoft.Dynamics.CRM.EntityMetadata"
      SchemaName            = "new_BankAccount"
      DisplayName           = @{
         '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
         LocalizedLabels = @(
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
               Label         = 'Bank Account'
               LanguageCode  = $languageCode
            }
         )
      }
      DisplayCollectionName = @{
         '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
         LocalizedLabels = @(
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
               Label         = 'Bank Accounts'
               LanguageCode  = $languageCode
            }
         )
      }
      Description           = @{
         '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
         LocalizedLabels = @(
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
               Label         = 'A table to store information about customer bank accounts'
               LanguageCode  = $languageCode
            }
         )
      }
      HasActivities         = $false
      HasNotes              = $false
      OwnershipType         = 'UserOwned'
      PrimaryNameAttribute  = "new_name"
      Attributes            = @(
         @{
            '@odata.type' = 'Microsoft.Dynamics.CRM.StringAttributeMetadata'
            IsPrimaryName = $true
            SchemaName    = "new_Name"
            RequiredLevel = @{
               Value = 'ApplicationRequired'
            }
            DisplayName   = @{
               '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
               LocalizedLabels = @(
                  @{
                     '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                     Label         = 'Name'
                     LanguageCode  = $languageCode
                  }
               )
            }
            Description   = @{
               '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
               LocalizedLabels = @(
                  @{
                     '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                     Label         = 'The name of the bank account'
                     LanguageCode  = $languageCode
                  }
               )
            }
            MaxLength     = 100
         }
      )
   }
   New-Table -body $tableDetails -solutionUniqueName "MySolution"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function returns the GUID of the newly created table.
#>
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

<#
.SYNOPSIS
   A function to remove a table from Dataverse.

.DESCRIPTION
   This function sends a DELETE request to a specified URI to remove a table from Dataverse. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table to be removed.

.EXAMPLE
   Remove-Table -tableLogicalName "new_bankaccount"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function does not return any value.
   WARNING: This operation is irreversible. Once a table is deleted, all data and metadata associated with it will be permanently lost.
#>
function Remove-Table {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $tableLogicalName
   )

   $deleteHeaders = $baseHeaders.Clone()
   $deleteHeaders.Add('Consistency', 'Strong')

   $DeleteRequest = @{
      Uri     = $baseURI + "EntityDefinitions(LogicalName='$tableLogicalName')"
      Method  = 'Delete'
      Headers = $deleteHeaders
   }
   Invoke-ResilientRestMethod -request $DeleteRequest | Out-Null
}
<#
.SYNOPSIS
   A function to remove an option value from a column in a Datverse table.

.DESCRIPTION
   This function sends a POST request to a specified URI to remove an option value from a column in a Dataverse table. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table where the column is located.

.PARAMETER columnLogicalName
   The logical name of the column where the option value is located.

.PARAMETER value
   The value of the option to be removed.

.PARAMETER solutionUniqueName
   The unique name of the solution where the table is located. 
   If this parameter is not provided, the option value will be removed from the table in the default solution.

.EXAMPLE
   Remove-OptionValue `
      -tableLogicalName "account" `
      -columnLogicalName "new_type" `
      -value 3 `
      -solutionUniqueName "MySolution"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function does not return any value.
#>
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
<#
.SYNOPSIS
   A function to update a column in a Dataverse table.

.DESCRIPTION
   This function sends a PUT request to a specified URI to update a column in a Dataverse table. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table where the column will be updated.

.PARAMETER column
   A PSCustomObject that represents the column to be updated. It should contain all the properties retrieved from Dataverse, including the LogicalName property.

.PARAMETER type
   The type of the column to be updated. 
   This function supports the following types: 
   'BigInt'
   'Boolean'
   'DateTime'
   'Decimal'
   'Double'
   'File'
   'Image'
   'Integer'
   'Lookup'
   'ManagedProperty'
   'Memo'
   'Money'
   'String'
   'UniqueIdentifier'

.PARAMETER solutionUniqueName
   The unique name of the solution where the table is located. 
   If this parameter is not provided, the column will be updated in the table in the default solution.

.PARAMETER mergeLabels
   A boolean value that indicates whether to merge labels during the update operation.

.EXAMPLE
   $retrievedBooleanColumn1 = Get-Column `
      -tableLogicalName 'sample_bankaccount' `
      -logicalName 'sample_boolean' `
      -type 'Boolean' 

   # Update the column
   $retrievedBooleanColumn1.DisplayName = @{
      '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
      LocalizedLabels = @(
         @{
            '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
            Label         = 'Sample Boolean Column Updated'
            LanguageCode  = 1033
         }
      )
   }
   $retrievedBooleanColumn1.Description = @{
      '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
      LocalizedLabels = @(
         @{
            '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
            Label         = 'Sample Boolean column description updated'
            LanguageCode  = $languageCode
         }
      )
   }
   $retrievedBooleanColumn1.RequiredLevel = @{
      Value = 'ApplicationRequired'
   }


   Update-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -column $retrievedBooleanColumn1 `
      -type 'Boolean' `
      -solutionUniqueName 'mysolution' `
      -mergeLabels $true
   

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function does not return any value.
#>
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
<#
.SYNOPSIS
   A function to update the value of an option in a column in a table in Dataverse.
.DESCRIPTION
   This function sends a POST request to a specified URI to update the value of an option in a column in a Dataverse table. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table where the column is located.

.PARAMETER columnLogicalName
   The logical name of the column where the option is located.

.PARAMETER value
   The new value for the option.

.PARAMETER label
   The new label for the option.

.PARAMETER languageCode
   The language code for the label.

.PARAMETER mergeLabels
   A boolean value that indicates whether to merge labels during the update operation.

.PARAMETER solutionUniqueName
   The unique name of the solution where the table is located. 
   If this parameter is not provided, the option will be updated in the table in the default solution.

.EXAMPLE
   Update-OptionValue `
      -tableLogicalName "account" `
      -columnLogicalName "sample_picklist" `
      -value 1 `
      -label "New Label" `
      -languageCode 1033 `
      -mergeLabels $true `
      -solutionUniqueName "MySolution"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function does not return any value.
#>
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
<#
.SYNOPSIS
   A function to update the order of options in a column in a Dataverse table.

.DESCRIPTION
   This function sends a POST request to a specified URI to update the order of options in a column in a Dataverse table. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER tableLogicalName
   The logical name of the table where the column is located.

.PARAMETER columnLogicalName
   The logical name of the column where the options are located.

.PARAMETER values
   An array of integers representing the new order of the options.

.PARAMETER solutionUniqueName
   The unique name of the solution where the table is located. If this parameter is not provided, the order of options will be updated in the table in the default solution.

.EXAMPLE
   Update-OptionsOrder `
      -tableLogicalName "account" `
      -columnLogicalName "sample_type" `
      -values @(3, 1, 2) `
      -solutionUniqueName "MySolution"

.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
   The function does not return any value.
#>
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
<#
.SYNOPSIS
   A function to update an existing Dataverse table.

.DESCRIPTION
   This function sends a PUT request to a specified URI to update an existing Dataverse table. 
   It uses resilient REST method to handle potential network issues.

.PARAMETER table
   A PSCustomObject that represents the table to be updated. It must include all the properties retrieved from Dataverse.

.PARAMETER solutionUniqueName
   The unique name of the solution where the table is located. If this parameter is not provided, the table in the default solution will be updated.

.PARAMETER mergeLabels
   A boolean value that indicates whether to merge labels during the update. If this parameter is not provided, labels will not be merged.

.EXAMPLE
# Retrieve the table to update it
   $bankAccountTable = Get-Table `
      -logicalName 'new_bankaccount'
   # No query so all properties will be returned


      # Update the table
      $bankAccountTable.HasActivities = $true
      $bankAccountTable.Description = @{
         '@odata.type'   = 'Microsoft.Dynamics.CRM.Label'
         LocalizedLabels = @(
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.LocalizedLabel'
               Label         = 'Contains information about customer bank accounts'
               LanguageCode  = $languageCode
            }
         )
      }
      # Send the request to update the table
      Update-Table `
         -table $bankAccountTable `
         -solutionUniqueName 'mysolution' `
         -mergeLabels $true


.NOTES
   The function requires a global variable $baseURI and $baseHeaders to be set before it is called.
   The function also calls another function Invoke-ResilientRestMethod which is not defined in this snippet.
#>
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
      $updateHeaders.Add('MSCRM.MergeLabels', $true)
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

























