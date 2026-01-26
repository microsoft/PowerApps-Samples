. $PSScriptRoot\Core.ps1

<#
.SYNOPSIS
Gets a set of records from a Dataverse table.

.DESCRIPTION
The Get-Records function uses the Invoke-ResilientRestMethod function to send a GET request to the Dataverse API.
It constructs the request URI by appending the entity set name and the query parameters to the base URI.
It also adds the necessary headers to include annotations in the response.

.PARAMETER setName
The name of the entity set to retrieve records from. This parameter is mandatory.

.PARAMETER query
The query parameters to filter, sort, or select the records. This parameter is mandatory.

.PARAMETER maxPageSize
The maximum number of records to retrieve per page. This parameter is optional. If not specified, the server default page size is used.

.PARAMETER strongConsistency
When true, requests Strong Consistency for the query. This parameter is optional. Default is false.

.EXAMPLE
(Get-Records -setName accounts -query '?$select=name&$top=10').value
This example gets the name of the first 10 accounts from Dataverse.


$accountContacts = (Get-Records `
   -setName 'accounts' `
   -query ('({0})/contact_customer_accounts?$select=fullname,jobtitle' `
      -f $accountId)).value

This example uses the query parameter to return a collection of contact records related to an account using the contact_customer_accounts relationship.

.EXAMPLE
$firstPage = Get-Records -setName 'contacts' -query '?$select=fullname' -maxPageSize 50

This example retrieves the first page of contacts with a maximum of 50 records per page.

#>

function Get-Records {
   param (
      [Parameter(Mandatory)]
      [String]
      $setName,
      [Parameter(Mandatory)]
      [String]
      $query,
      [Nullable[int]]
      $maxPageSize,
      [bool]
      $strongConsistency = $false
   )
   $uri = $baseURI + $setName + $query
   # Header for GET operations that have annotations
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $null)

   $preferHeaders = @('odata.include-annotations="*"')
   if ($PSBoundParameters.ContainsKey('maxPageSize')) {
      $preferHeaders = @("odata.maxpagesize=$maxPageSize") + $preferHeaders
   }
   $getHeaders.Add('Prefer', ($preferHeaders -join ','))

   if ($strongConsistency) {
      $getHeaders.Add('Consistency', 'Strong')
   }
   $RetrieveMultipleRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }
   Invoke-ResilientRestMethod $RetrieveMultipleRequest
}

<#
.SYNOPSIS
Retrieves the next page of records using the @odata.nextLink value.

.DESCRIPTION
The Get-NextLink function uses the Invoke-ResilientRestMethod function to send a GET request to the next page URL.
It accepts the @odata.nextLink value from a previous response and optional parameters to control page size and annotations.
This function is designed to work with paginated results from the Dataverse Web API.

.PARAMETER nextLink
The @odata.nextLink value from the previous response. This parameter is mandatory.

.PARAMETER maxPageSize
The maximum number of records to retrieve per page. This parameter is optional. If not specified, the server default page size is used.

.PARAMETER includeAnnotations
Whether to include OData annotations in the response. This parameter is optional. Default is true.

.EXAMPLE
$firstPage = Get-Records -setName 'contacts' -query '?$select=fullname'
if ($firstPage.'@odata.nextLink') {
   $secondPage = Get-NextLink -nextLink $firstPage.'@odata.nextLink' -maxPageSize 50
}

This example retrieves the first page of contacts and then uses Get-NextLink to retrieve the second page with a max page size of 50.

.EXAMPLE
$nextPage = Get-NextLink -nextLink $response.'@odata.nextLink'

This example retrieves the next page using the server's default page size and includes annotations.

#>

function Get-NextLink {
   param (
      [Parameter(Mandatory)]
      [String]
      $nextLink,
      [Nullable[int]]
      $maxPageSize,
      [bool]
      $includeAnnotations = $true
   )

   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $null)

   $preferHeaders = @()
   if ($PSBoundParameters.ContainsKey('maxPageSize')) {
      $preferHeaders += "odata.maxpagesize=$maxPageSize"
   }
   if ($includeAnnotations) {
      $preferHeaders += 'odata.include-annotations="*"'
   }

   if ($preferHeaders.Count -gt 0) {
      $getHeaders.Add('Prefer', ($preferHeaders -join ','))
   }

   $RetrieveNextPageRequest = @{
      Uri     = $nextLink
      Method  = 'Get'
      Headers = $getHeaders
   }

   Invoke-ResilientRestMethod $RetrieveNextPageRequest
}

<#
.SYNOPSIS
Creates a new record in a Dataverse table.

.DESCRIPTION
The New-Record function uses the Invoke-ResilientRestMethod function to send a POST request to the Dataverse Web API. 
It constructs the request URI by appending the entity set name to the base URI. 
It also adds the necessary headers and converts the body hashtable to JSON format. It returns the GUID ID value of the created record.

.PARAMETER setName
The name of the entity set to create a record in. This parameter is mandatory.

.PARAMETER body
A hashtable of attributes and values for the new record. This parameter is mandatory.

.PARAMETER solutionUniqueName
A string representing the unique name of the solution that a new solution component is created in. This parameter is optional.

.PARAMETER strongConsistency
When true, requests Strong Consistency for the operation. This parameter is optional. Default is false.

.EXAMPLE
$contactRafelShillo = @{
   'firstname' = 'Rafel'
   'lastname'  = 'Shillo'
}

$rafelShilloId = New-Record `
   -setName 'contacts' `
   -body $contactRafelShillo

This example creates a new contact record with the firstname 'Rafel' and the lastname 'Shillo'. It returns the GUID ID of the created record.
#>

function New-Record {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $setName,
      [Parameter(Mandatory)] 
      [hashtable]
      $body,
      [string]
      $solutionUniqueName = $null,
      [bool]
      $strongConsistency = $false
   )

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   if($strongConsistency) {
      $postHeaders.Add('Consistency', 'Strong')
   }

   if($solutionUniqueName) {
      $postHeaders.Add('MSCRM.SolutionUniqueName', $solutionUniqueName)
   } 
   
   $CreateRequest = @{
      Uri     = $baseURI + $setName
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body -Depth 5 # 5 should be enough for most cases, the default is 2.

   }
   $rh = Invoke-ResilientRestMethod -request $CreateRequest -returnHeader $true
   $url = $rh['OData-EntityId']
   $selectedString = Select-String -InputObject $url -Pattern '(?<=\().*?(?=\))'
   return [System.Guid]::New($selectedString.Matches.Value.ToString())
}

<#
.SYNOPSIS
Creates a new record in a Dataverse table and returns the created record.

.DESCRIPTION
The Add-Record function uses the Invoke-ResilientRestMethod function to send a POST request to the Dataverse Web API.
It constructs the request URI by appending the entity set name and optional query to the base URI.
It also adds the necessary headers including 'Prefer: return=representation' and converts the body hashtable to JSON format.
It returns the created record with its ETag value, which can be used for optimistic concurrency operations.

.PARAMETER setName
The name of the entity set to create a record in. This parameter is mandatory.

.PARAMETER body
A hashtable of attributes and values for the new record. This parameter is mandatory.

.PARAMETER query
The query parameters to filter or select the returned record properties. This parameter is optional.

.EXAMPLE
$contactData = @{
   'firstname' = 'Rafel'
   'lastname'  = 'Shillo'
}

$retrievedContact = Add-Record `
   -setName 'contacts' `
   -body $contactData `
   -query '?$select=fullname,firstname,lastname'

Write-Host "Contact created with ETag: $($retrievedContact.'@odata.etag')"

This example creates a new contact record and returns it with the specified properties and ETag value.
#>

function Add-Record {
   param (
      [Parameter(Mandatory)]
      [String]
      $setName,
      [Parameter(Mandatory)]
      [hashtable]
      $body,
      [String]
      $query
   )

   $uri = $baseURI + $setName
   if ($query) {
      $uri += if ($query.StartsWith('?')) { $query } else { "?$query" }
   }

   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')

   $preferHeaders = @('return=representation', 'odata.include-annotations="*"')
   $postHeaders.Add('Prefer', ($preferHeaders -join ','))

   $CreateRequest = @{
      Uri     = $uri
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body -Depth 5
   }

   Invoke-ResilientRestMethod $CreateRequest
}

<#
.SYNOPSIS
Synchronizes a record by retrieving it only if it has been modified on the server.

.DESCRIPTION
The Sync-Record function uses conditional GET to refresh a record from the server.
It accepts a record object that must contain @odata.etag and @odata.context properties.
The function extracts the entity set name, columns, and ID from the record's context and uses
the If-None-Match header with the record's ETag value to perform a conditional GET.
If the record has not been modified (status 304), the original record is returned.
If the record has been modified, the updated record with a new ETag is returned.

.PARAMETER record
A PSCustomObject representing the record to synchronize. Must contain '@odata.etag' and '@odata.context' properties. This parameter is mandatory.

.PARAMETER primaryKeyName
The name of the primary key property in the record (e.g., 'accountid', 'contactid'). This parameter is mandatory.

.EXAMPLE
$account = Add-Record `
   -setName 'accounts' `
   -body @{ name = 'Contoso Ltd' } `
   -query '?$select=name,revenue'

# Later, check if the record has been modified
$syncedAccount = Sync-Record -record $account -primaryKeyName 'accountid'

if ($syncedAccount.'@odata.etag' -ne $account.'@odata.etag') {
   Write-Host "Record was modified on the server"
} else {
   Write-Host "Record was not modified"
}

This example creates an account record and then uses Sync-Record to check if it has been modified on the server.
#>

function Sync-Record {
   param (
      [Parameter(Mandatory)]
      [PSCustomObject]
      $record,
      [Parameter(Mandatory)]
      [String]
      $primaryKeyName
   )

   # Validate that record has required properties
   $etag = $record.'@odata.etag'
   $context = $record.'@odata.context'

   if (-not $etag -or -not $context) {
      throw "record parameter must have '@odata.etag' and '@odata.context' properties."
   }

   # Extract entity set name from context (e.g., "#accounts(name,revenue)")
   if ($context -match '#(\w+)\(') {
      $setName = $Matches[1]
   }
   else {
      throw "Cannot extract entity set name from '@odata.context' property."
   }

   # Extract columns from context
   if ($context -match '\(([^)]+)\)') {
      $columns = $Matches[1]
   }
   else {
      throw "Cannot extract columns from '@odata.context' property."
   }

   # Get the record ID using the primary key name
   $id = $record.$primaryKeyName
   if (-not $id) {
      throw "Cannot extract ID from record using primary key name '$primaryKeyName'."
   }

   # Build the query URI
   $query = "?`$select=$columns"
   $uri = $baseURI + $setName + '(' + $id + ')' + $query

   # Set up headers for conditional GET
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $etag)

   $RetrieveRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }

   try {
      $response = Invoke-ResilientRestMethod $RetrieveRequest
      Write-Verbose "Record was modified on the server. Returning updated record."
      return $response
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      $statuscode = [int]$_.Exception.StatusCode
      if ($statuscode -eq 304) {
         # Not Modified - return the original record
         Write-Verbose "Record was not modified on the server. Returning original record."
         return $record
      }
      else {
         throw $_
      }
   }
}

<#
.SYNOPSIS
Gets a single record from a Dataverse table by its primary key value.

.DESCRIPTION
The Get-Record function uses the Invoke-ResilientRestMethod function to send a GET request to the Dataverse API. 
It constructs the request URI by appending the entity set name, the record ID, and the query parameters to the base URI. 
It also adds the necessary headers to include annotations in the response. It returns the record as an object.

.PARAMETER setName
The name of the entity set to retrieve the record from. This parameter is mandatory.

.PARAMETER id
The GUID of the record to retrieve. This parameter is mandatory.

.PARAMETER query
The query parameters to filter, expand, or select the record properties. This parameter is optional.

.EXAMPLE
   $retrievedRafelShillo1 = Get-Record `
      -setName 'contacts' `
      -id $rafelShilloId `
      -query '?$select=fullname,annualincome,jobtitle,description'

This example gets the fullname, annualincome, jobtitle, and description of the contact with the specified ID.
#>

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

<#
.SYNOPSIS
Gets the value of a single property from a Dataverse record.

.DESCRIPTION
The Get-ColumnValue function uses the Invoke-ResilientRestMethod function to send a GET request to the Dataverse API. 
It constructs the request URI by appending the entity set name, the record ID, and the property name to the base URI. 
It also adds the necessary headers to avoid caching. It returns the value of the property as a string.

.PARAMETER setName
The name of the entity set to retrieve the record from. This parameter is mandatory.

.PARAMETER id
The GUID of the record to retrieve. This parameter is mandatory.

.PARAMETER property
The name of the property to get the value from. This parameter is mandatory.

.EXAMPLE
$telephone1 = Get-ColumnValue `
   -setName 'contacts' `
   -id 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84 `
   -property 'telephone1'

This example gets the telephone1 value of the contact record with the specified ID.
#>

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

<#
.SYNOPSIS
Updates an existing record in a Dataverse table.

.DESCRIPTION
The Update-Record function uses the Invoke-ResilientRestMethod function to send a PATCH request to the Dataverse API.
It constructs the request URI by appending the entity set name and the record ID to the base URI.
It also adds the necessary headers and converts the body hashtable to JSON format.
It uses the If-Match header to prevent creating a new record if the record ID does not exist.
When an eTagValue is provided, it enables optimistic concurrency by only updating the record if it matches the specified ETag.

.PARAMETER setName
The name of the entity set to update the record in. This parameter is mandatory.

.PARAMETER id
The GUID of the record to update. This parameter is mandatory.

.PARAMETER body
A hashtable of attributes and values for the updated record. This parameter is mandatory.

.PARAMETER eTagValue
The ETag value to use for optimistic concurrency. When specified, the update will only succeed if the record's current ETag matches this value. This parameter is optional.

.PARAMETER solutionUniqueName
A string representing the unique name of the solution that a new solution component is created in. This parameter is optional.

.PARAMETER strongConsistency
When true, requests Strong Consistency for the operation. This parameter is optional. Default is false.

.EXAMPLE
$body = @{
   'annualincome' = 80000
   'jobtitle'     = 'Junior Developer'
}

# Update the record with the data
Update-Record `
   -setName 'contacts' `
   -id 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84`
   -body $body

This example updates the annualincome and jobtitle of the contact with the specified ID.

.EXAMPLE
# Update with optimistic concurrency
Update-Record `
   -setName 'accounts' `
   -id $accountId `
   -body @{ revenue = 6000000 } `
   -eTagValue $currentETag

This example updates the account only if the current ETag matches the specified value, preventing updates to records that have been modified by others.
#>


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
      $body,
      [String]
      $eTagValue,
      [string]
      $solutionUniqueName = $null,
      [bool]
      $strongConsistency = $false

   )
   $uri = $baseURI + $setName
   $uri = $uri + '(' + $id.Guid + ')'
   # Header for Update operations
   $updateHeaders = $baseHeaders.Clone()
   $updateHeaders.Add('Content-Type', 'application/json')
   if($strongConsistency) {
      $updateHeaders.Add('Consistency', 'Strong')
   }

   # Use provided ETag for optimistic concurrency, otherwise use '*' to prevent create
   $ifMatchValue = if ($eTagValue) { $eTagValue } else { '*' }
   $updateHeaders.Add('If-Match', $ifMatchValue)
   $UpdateRequest = @{
      Uri     = $uri
      Method  = 'Patch'
      Headers = $updateHeaders
      Body    = ConvertTo-Json $body
   }
   Invoke-ResilientRestMethod $UpdateRequest
}

<#
.SYNOPSIS
Sets the value of a single property for a Dataverse record.

.DESCRIPTION
The Set-ColumnValue function uses the Invoke-ResilientRestMethod function to send a PUT request to the Dataverse API. 
It constructs the request URI by appending the entity set name, the record ID, and the property name to the base URI. 
It also adds the necessary headers and converts the value to JSON format. 
It overwrites the existing value of the property with the new value.

.PARAMETER setName
The name of the entity set to update the record in. This parameter is mandatory.

.PARAMETER id
The GUID of the record to update. This parameter is mandatory.

.PARAMETER property
The name of the property to set the value for. This parameter is mandatory.

.PARAMETER value
The new value for the property. This parameter is mandatory.

.EXAMPLE
Set-ColumnValue `
   -setName 'contacts' `
   -id 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84 `
   -property 'telephone1' `
   -value '555-0105'

This example sets the telephone1 column value of the contact with the specified ID to 555-0105.
#>


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
      Body    = ConvertTo-Json $body
   }
   Invoke-ResilientRestMethod $SetColumnValueRequest
}

<#
.SYNOPSIS
Adds a record to a collection-valued navigation property of another record.

.DESCRIPTION
The Add-ToCollection function uses the Invoke-ResilientRestMethod function to send a POST request to the Dataverse API. 
It constructs the request URI by appending the target entity set name, the target record ID, and the collection name to the base URI. 
It also adds the necessary headers and converts the record URI to JSON format. 
It creates a reference between the target record and the record to be added to the collection.

.PARAMETER targetSetName
The name of the entity set that contains the target record. This parameter is mandatory.

.PARAMETER targetId
The GUID of the target record. This parameter is mandatory.

.PARAMETER collectionName
The name of the collection-valued navigation property of the target record. This parameter is mandatory.

.PARAMETER setName
The name of the entity set that contains the record to be added to the collection. This parameter is mandatory.

.PARAMETER id
The GUID of the record to be added to the collection. This parameter is mandatory.

.PARAMETER strongConsistency
When true, requests Strong Consistency for the operation. This parameter is optional. Default is false.

.EXAMPLE
Add-ToCollection `
   -targetSetName 'accounts' `
   -targetId 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84 `
   -collectionName 'contact_customer_accounts' `
   -setName 'contacts' `
   -id 5d68b37f-aae9-4cd6-8b94-37d6439b2f34

This example adds the contact with the specified ID to the contact_customer_accounts collection of the account with the specified ID.
#>


function Add-ToCollection {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $targetSetName,
      [Parameter(Mandatory)] 
      [Guid] 
      $targetId,
      [Parameter(Mandatory)] 
      [string]
      $collectionName,
      [Parameter(Mandatory)] 
      [String] 
      $setName,
      [Parameter(Mandatory)] 
      [Guid] 
      $id,
      [bool] 
      $strongConsistency = $false
   )
   $uri = '{0}{1}({2})/{3}/$ref' `
      -f $baseURI, $targetSetName, $targetId, $collectionName

   $headers = $baseHeaders.Clone()
   $headers.Add('Content-Type', 'application/json')
   if ($strongConsistency) {
      $headers.Add('Consistency', 'Strong')
   }

   # Must use absolute URI
   $recordUri = '{0}{1}({2})' `
      -f $baseURI, $setName, $id

   $body = @{
      '@odata.id' = $recordUri
   }
   $AssociateRequest = @{
      Uri     = $uri
      Method  = 'Post'
      Headers = $headers
      Body    = ConvertTo-Json $body
   }
   Invoke-ResilientRestMethod $AssociateRequest
}

<#
.SYNOPSIS
Removes a record from a collection-valued navigation property of another record.

.DESCRIPTION
The Remove-FromCollection function uses the Invoke-ResilientRestMethod function to send a DELETE request to the Dataverse API. 
It constructs the request URI by appending the target entity set name, the target record ID, the collection name, and the record ID to the base URI. 
It also adds the necessary headers. It deletes the reference between the target record and the record to be removed from the collection.

.PARAMETER targetSetName
The name of the entity set that contains the target record. This parameter is mandatory.

.PARAMETER targetId
The GUID of the target record. This parameter is mandatory.

.PARAMETER collectionName
The name of the collection-valued navigation property of the target record. This parameter is mandatory.

.PARAMETER id
The GUID of the record to be removed from the collection. This parameter is mandatory.

.EXAMPLE
Remove-FromCollection `
   -targetSetName 'accounts' `
   -targetId 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84 `
   -collectionName 'contact_customer_accounts' `
   -id 5d68b37f-aae9-4cd6-8b94-37d6439b2f34
This example removes the contact with the specified ID from the contact_customer_accounts collection of the account with the specified ID.
#>


function Remove-FromCollection {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $targetSetName,
      [Parameter(Mandatory)] 
      [Guid] 
      $targetId,
      [Parameter(Mandatory)] 
      [string]
      $collectionName,
      [Parameter(Mandatory)] 
      [Guid] 
      $id
   )
   $uri = '{0}{1}({2})/{3}({4})/$ref' `
      -f $baseURI, $targetSetName, $targetId, $collectionName, $id

   $DisassociateRequest = @{
      Uri     = $uri
      Method  = 'Delete'
      Headers = $baseHeaders
   }
   Invoke-ResilientRestMethod $DisassociateRequest
}

<#
.SYNOPSIS
Deletes a record from a Dataverse table.

.DESCRIPTION
The Remove-Record function uses the Invoke-ResilientRestMethod function to send a DELETE request to the Dataverse API.
It constructs the request URI by appending the entity set name and the record ID to the base URI.
It also adds the necessary headers. It deletes the record with the specified ID from the table.
When an eTagValue is provided, it enables optimistic concurrency by only deleting the record if it matches the specified ETag.

.PARAMETER setName
The name of the entity set to delete the record from. This parameter is mandatory.

.PARAMETER id
The GUID of the record to delete. This parameter is mandatory.

.PARAMETER eTagValue
The ETag value to use for optimistic concurrency. When specified, the delete will only succeed if the record's current ETag matches this value. This parameter is optional.

.PARAMETER strongConsistency
When true, requests Strong Consistency for the operation. This parameter is optional. Default is false.

.EXAMPLE
Remove-Record `
   -setName accounts `
   -id 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84

This example deletes the account with the specified ID from the Dataverse table.

.EXAMPLE
Remove-Record `
   -setName accounts `
   -id $accountId `
   -eTagValue $currentETag

This example deletes the account only if the current ETag matches the specified value, preventing deletion of records that have been modified by others.
#>

function Remove-Record {
   param (
      [Parameter(Mandatory)]
      [String]
      $setName,
      [Parameter(Mandatory)]
      [Guid]
      $id,
      [String]
      $eTagValue,
      [bool]
      $strongConsistency = $false
   )
   $uri = $baseURI + $setName
   $uri = $uri + '(' + $id.Guid + ')'
   $deleteHeaders = $baseHeaders.Clone()
   if ($strongConsistency) {
      $deleteHeaders.Add('Consistency', 'Strong')
   }

   # Add If-Match header when eTagValue is provided for optimistic concurrency
   if ($eTagValue) {
      $deleteHeaders.Add('If-Match', $eTagValue)
   }

   $DeleteRequest = @{
      Uri     = $uri
      Method  = 'Delete'
      Headers = $deleteHeaders
   }
   Invoke-ResilientRestMethod $DeleteRequest
}

<#
.SYNOPSIS
Creates multiple records in a Dataverse table.

.DESCRIPTION
The Create-MultipleRecords function uses the Invoke-ResilientRestMethod function to send a POST request to the Dataverse API.
It constructs the request URI by appending the entity set name and the CreateMultiple action to the base URI.
It also adds the necessary headers and converts the body hashtable array to JSON format. It creates multiple records 
in a single request and returns the GUID IDs of the created records.

.PARAMETER setName
The name of the entity set to create records in. This parameter is mandatory.

.PARAMETER targets
An array of hashtables, each representing attributes and values for a new record. This parameter is mandatory.

.PARAMETER strongConsistency
When true, requests Strong Consistency for the operation. This parameter is optional. Default is false.

.EXAMPLE
$contacts = @(
   @{ 
      '@odata.type'   = 'Microsoft.Dynamics.CRM.contact'
      firstname = 'John'
      lastname = 'Doe' 
   },
   @{ 
      '@odata.type'   = 'Microsoft.Dynamics.CRM.contact'
      firstname = 'Jane'
      lastname = 'Smith' 
    }
)

$ids = Create-MultipleRecords `
   -setName 'contacts' `
   -targets $contacts

This example creates two new contact records with the specified attributes.
#>

function Create-MultipleRecords {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $setName,
      [Parameter(Mandatory)] 
      [hashtable[]] 
      $targets,
      [bool] 
      $strongConsistency = $false
   )
   $body = @{
      'Targets' = $targets
   }

   $uri = $baseURI + $setName + '/Microsoft.Dynamics.CRM.CreateMultiple'
   $postHeaders = $baseHeaders.Clone()
   $postHeaders.Add('Content-Type', 'application/json')
   if($strongConsistency) {
      $postHeaders.Add('Consistency', 'Strong')
   }

   $CreateMultipleRequest = @{
      Uri     = $uri
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body -Depth 5 # 5 should be enough for most cases, the default is 2.
   }
   $response = Invoke-ResilientRestMethod -request $CreateMultipleRequest
   return $response.Ids
}