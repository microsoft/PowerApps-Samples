# Dataverse Web API PowerShell Helper functions

The files in this folder are PowerShell helper functions that Dataverse Web API PowerShell samples use. These samples are separated in the following files:

| File | Description |
|------|-------------|
| [Core.ps1](Core.ps1) | Contains functions that all other functions or samples depend on. |
| [TableOperations.ps1](TableOperations.ps1) | Contains functions that enable performing data operations on table rows. |
| [MetadataOperation.ps1](MetadataOperations.ps1) | Contains functions that enable performing table, column, and relationship schema change operations, as well as solution operations. |

Samples that use these common functions reference them using [dot sourcing](https://learn.microsoft.com/powershell/module/microsoft.powershell.core/about/about_scripts#script-scope-and-dot-sourcing) as demonstrated by the [BasicOperations/BasicOperations.ps1](BasicOperations/BasicOperations.ps1):

```powershell
. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1
```

## Function list

| Group | Function | Description |
|-------|----------|-------------|
| Core | [Connect](#connect-function) | Connects to Dataverse Web API using Azure authentication. |
| Metadata | [Export-Solution](#export-solution-function) | Export a solution from Dataverse. |
| Core | [Invoke-DataverseCommands](#invoke-dataversecommands-function) | Invokes a set of commands against the Dataverse Web API. |
| Core | [Invoke-ResilientRestMethod](#invoke-resilientrestmethod-function) | Invokes a REST method with resilience to handle 429 errors. |
| TableOperations | [Add-ToCollection](#add-tocollection-function) | Adds a record to a collection-valued navigation property of another record. |
| Metadata | [Get-CanBeReferenced](#get-canbereferenced-function) | Check whether a table can be referenced in a relationship. |
| Metadata | [Get-CanBeReferencing](#get-canbereferencing-function) | Check whether a table can be referencing in a relationship. |
| Metadata | [Get-CanManyToMany](#get-canmanytomany-function) | Check if a table can have many-to-many relationships in Dataverse. |
| Metadata | [Get-Column](#get-column-function) | Retrieve a column from a Dataverse table. |
| TableOperations | [Get-ColumnValue](#get-columnvalue-function) | Gets the value of a single property from a Dataverse record. |
| TableOperations | [Get-Record](#get-record-function) | Gets a single record from a Dataverse table by its primary key value. |
| TableOperations | [Get-Records](#get-records-function) | Gets a set of records from a Dataverse table. |
| Metadata | [Get-GlobalOptionSet](#get-globaloptionset-function) | Retrieve a global option set from Dataverse. |
| Metadata | [Get-Relationship](#get-relationship-function) | Retrieve a relationship from Dataverse. |
| Metadata | [Get-Relationships](#get-relationships-function) | Retrieve relationships from Dataverse. |
| Metadata | [Get-Table](#get-table-function) | Get a table definition from Dataverse. |
| Metadata | [Get-TableColumns](#get-tablecolumns-function) | Retrieve the columns of a table in Dataverse. |
| Metadata | [Get-Tables](#get-tables-function) | Gets table definitions from Dataverse. |
| Metadata | [Get-ValidManyToManyTables](#get-validmanytomanytables-function) | Get valid tables for many-to-many relationships in Dataverse. |
| Metadata | [Get-ValidReferencingTables](#get-validreferencingtables-function) | Get valid referencing tables for a specified table in Dataverse. |
| Metadata | [Import-Solution](#import-solution-function) | Import a solution into Dataverse. |
| Metadata | [New-Column](#new-column-function) | Create a new column in a Dataverse table. |
| Metadata | [New-CustomerRelationship](#new-customerrelationship-function) | Create a new customer relationship in Dataverse. |
| Metadata | [New-GlobalOptionSet](#new-globaloptionset-function) | Create a new global option set in Dataverse. |
| Metadata | [New-OptionValue](#new-optionvalue-function) | Create a new option value in a column in a Dataverse table. |
| TableOperations | [New-Record](#new-record-function) | Creates a new record in a Dataverse table. |
| Metadata | [New-Relationship](#new-relationship-function) | Create a new relationship in Dataverse. |
| Metadata | [New-StatusOption](#new-statusoption-function) | Create a new status option in a Dataverse table column. |
| Metadata | [New-Table](#new-table-function) | Create a new Dataverse table. |
| TableOperations | [Remove-FromCollection](#remove-fromcollection-function) | Removes a record from a collection-valued navigation property of another record. |
| Metadata | [Remove-OptionValue](#remove-optionvalue-function) | Remove an option value from a column in a Dataverse table. |
| TableOperations | [Remove-Record](#remove-record-function) | Deletes a record from a Dataverse table. |
| TableOperations | [Set-ColumnValue](#set-columnvalue-function) | Sets the value of a single property for a Dataverse record. |
| Metadata | [Update-Column](#update-column-function) | Update a column in a Dataverse table. |
| Metadata | [Update-OptionValue](#update-optionvalue-function) | Update the value of an option in a column in a table in Dataverse. |
| TableOperations | [Update-Record](#update-record-function) | Updates an existing record in a Dataverse table. |
| CommonFunctions | [Get-WhoAmI](#get-whoami-function) | Gets the current user information from the Dataverse Web API. |

## Core functions

The [Core.ps1](Core.ps1) file contains these variables and functions.

### Variables

Set these variables when debugging, using Fiddler:

| Variable | Type | Description |
|----------|------|-------------|
| `$debug` | bool | Set to `$true` only while debugging with Fiddler. |
| `$proxyUrl` | string | Set this value to the Fiddler proxy URL configured on your computer. The default value is `http://127.0.0.1:8888`. |

These global variables are set by the [Connect function](#connect-function).

| Variable | Type | Description |
|----------|------|-------------|
| `$baseHeaders` | hashtable | Includes the request headers that should be used by all Dataverse Web API calls, including the `Authorization` header with the bearer access token to enable authentication. |
| `$baseURI` | string | The URL to the root of the Dataverse Web API. Many operations can use relative URL references. This value is useful for certain operations where the absolute URL is necessary. |

### Connect function

Connects to Dataverse Web API using Azure authentication.

The `Connect` function uses the `Get-AzAccessToken` cmdlet to obtain an access token for the specified resource URI. This function then sets the global variables `$baseHeaders` and `$baseURI` to be used for subsequent requests to the resource.

#### Connect parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `uri` | string | **Required**. The URL for the Dataverse environment. |

#### Connect returns

This function doesn't return a value.

#### Connect example

```powershell
Connect -uri 'https://yourorg.crm.dynamics.com'
```

### Invoke-DataverseCommands function

Invokes a set of commands against the Dataverse Web API.

The `Invoke-DataverseCommands` function uses the `Invoke-Command` cmdlet to run a script block of commands against the Dataverse Web API. This function handles any errors that may occur from the Dataverse API or the script itself.

[Learn about handling Dataverse Web API errors](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/compose-http-requests-handle-errors#parse-errors-from-the-response).

#### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `$commands` | command block | **Required**. The script block of commands to run against the Dataverse resource. |

#### Invoke-DataverseCommands returns

This function doesn't return a value, but it intercepts and parses errors returned from the Dataverse Web API. Other types of errors are returned without processing.

#### Invoke-DataverseCommands example

This example invokes a script block that gets the first account from Dataverse and updates the name of the first account.

```powershell
Invoke-DataverseCommands {
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
```

### Invoke-ResilientRestMethod function

Invokes a REST method with resilience to handle 429 (Too Many Requests) errors.

The `Invoke-ResilientRestMethod` function uses the `Invoke-RestMethod` cmdlet to send an HTTP request to a RESTful web service. This function handles any 429 errors by retrying the request using the `Retry-After` header value as the retry interval, which Dataverse provides. This function also supports using a proxy if the `$debug` variable is set to true.

[Learn about Dataverse service protection limits](https://learn.microsoft.com/power-apps/developer/data-platform/api-limits)

#### Invoke-ResilientRestMethod parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `request` | hashtable | **Required**. Parameters to pass to the `Invoke-RestMethod` cmdlet. |
| `returnHeader` | bool | Whether to return the response headers instead of the response body. |

#### Invoke-ResilientRestMethod returns

This function doesn't return a value.

#### Invoke-ResilientRestMethod example

See the functions in the [TableOperations.ps1](TableOperations.ps1) file for examples using this function.

## Table Operation functions

The [TableOperations.ps1](TableOperations.ps1) file contains these functions.

### Add-ToCollection function

Adds a record to a collection-valued navigation property of another record.

The `Add-ToCollection` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `POST` request to the Dataverse API. This function constructs the request URI by appending the target entity set name, the target record ID, and the collection name to the base URI. This function also adds the necessary headers and converts the record URI to JSON format. This function creates a reference between the target record and the record to be added to the collection.

[Learn to associate and disassociate table rows](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/associate-disassociate-entities-using-web-api)

#### Add-ToCollection parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `targetSetName` | string | **Required**. The name of the entity set that contains the target record. |
| `targetId` | string | **Required**. The GUID of the target record. |
| `collectionName` | string | **Required**. The name of the collection-valued navigation property of the target record. |
| `setName` | string | **Required**. The name of the entity set that contains the record to be added to the collection. |
| `id` | string | **Required**. The GUID of the record to be added to the collection. |

#### Add-ToCollection returns

This function doesn't return a value.

#### Add-ToCollection example

This example adds the contact with the specified ID to the `contact_customer_accounts` collection of the account with the specified ID.

```powershell
Add-ToCollection `
   -targetSetName 'accounts' `
   -targetId 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84 `
   -collectionName 'contact_customer_accounts' `
   -setName 'contacts' `
   -id 5d68b37f-aae9-4cd6-8b94-37d6439b2f34
```

### Get-ColumnValue function

Gets the value of a single property from a Dataverse record.

The `Get-ColumnValue` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `GET` request to the Dataverse API. This function constructs the request URI by appending the entity set name, the record ID, and the property name to the base URI. This function also adds the necessary headers to avoid caching. It returns the value of the property as a string.

[Learn to retrieve specific properties](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/retrieve-entity-using-web-api#retrieve-specific-properties)

#### Get-ColumnValue parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `setName` | string | **Required**. The name of the entity set to retrieve the record from. |
| `id` | Guid | **Required**. The GUID of the record to retrieve. |
| property | string | **Required**. The name of the property to get the value from. |

#### Get-ColumnValue returns

The value of the property as a string.

#### Get-ColumnValue example

This example gets the `telephone1` value of the contact record with the specified ID.

```powershell
$telephone1 = Get-ColumnValue `
   -setName 'contacts' `
   -id 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84 `
   -property 'telephone1'
```

### Get-Record function

Gets a single record from a Dataverse table by its primary key value.

The `Get-Record` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `GET` request to the Dataverse API. This function constructs the request URI by appending the entity set name, the record ID, and the query parameters to the base URI. This function also adds the necessary headers to include annotations in the response. It returns the record as an object.

[Learn to retrieve a table row](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/retrieve-entity-using-web-api)

#### Get-Record parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `setName` | string | **Required**. The name of the entity set to retrieve the record from. |
| `id` | guid | **Required**. The GUID of the record to retrieve. |
| `query` | string | The query parameters to filter, expand, or select the record properties. |

#### Get-Record returns

This function returns the response containing the record data.

#### Get-Record example

This example gets the `fullname`, `annualincome`, `jobtitle`, and `description` of the contact with the specified ID.

```powershell
$retrievedRafelShillo1 = Get-Record `
   -setName 'contacts' `
   -id $rafelShilloId `
   -query '?$select=fullname,annualincome,jobtitle,description'
```

### Get-Records function

Gets a *set* of records from a Dataverse table.

The `Get-Records` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `GET` request to the Dataverse API. This function constructs the request URI by appending the entity set name and the query parameters to the base URI. This function also adds the necessary headers to include annotations in the response.

[Learn to Query data using the Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query-data-web-api)

#### Get-Records parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `setName` | string | **Required**. The name of the entity set to retrieve records from. |
| `query` | string | **Required**. The query parameters to filter, sort, or select the records. |

#### Get-Records returns

Returns the response that contains properties about the collection of records returned. These properties are useful when paging requests. The array of records matching the request is in the `value` property.

#### Get-Records example

This example gets the name of the first 10 accounts from Dataverse.

```powershell
(Get-Records -setName accounts -query '?$select=name&$top=10').value
```

This example uses the `query` parameter to return a collection of contact records related to an account using the `contact_customer_accounts` relationship.

```powershell
$accountContacts = (Get-Records `
   -setName 'accounts' `
   -query ('({0})/contact_customer_accounts?$select=fullname,jobtitle' `
      -f $accountId)).value
```

### New-Record function

Creates a new record in a Dataverse table.

The `New-Record` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `POST` request to the Dataverse Web API. This function constructs the request URI by appending the entity set name to the base URI. This function also adds the necessary headers and converts the `body` hashtable to JSON format. This funtion returns the GUID ID value of the created record.

[Learn to create records](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/create-entity-web-api)

#### New-Record parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `setName` | string | **Required**. The name of the entity set to create a record in. |
| `body` | hashtable | **Required**. A hashtable of attributes and values for the new record. |

#### New-Record returns

This function returns the GUID value of the record created.

#### New-Record example

This example creates a new contact record with the `firstname` 'Rafel' and the `lastname` 'Shillo'. The sample returns the GUID ID of the created record.

```powershell
$contactRafelShillo = @{
   'firstname' = 'Rafel'
   'lastname'  = 'Shillo'
}

$rafelShilloId = New-Record `
   -setName 'contacts' `
   -body $contactRafelShillo
```

### Remove-FromCollection function

Removes a record from a collection-valued navigation property of another record.

The `Remove-FromCollection` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `DELETE` request to the Dataverse API. This function constructs the request URI by appending the target entity set name, the target record ID, the collection name, and the record ID to the base URI. This function also adds the necessary headers. It deletes the reference between the target record and the record to be removed from the collection.

[Learn to associate and disassociate table rows using the Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/associate-disassociate-entities-using-web-api)

#### Remove-FromCollection parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `targetSetName` | string | **Required**. The name of the entity set that contains the target record. |
| `targetId` | Guid | **Required**. The ID of the target record. |
| `collectionName` | string | **Required**. The name of the collection-valued navigation property of the target record. |
| `id` | Guid | **Required**. The ID of the record to be removed from the collection. |

#### Remove-FromCollection returns

This function doesn't return a value.

#### Remove-FromCollection example

This example removes the contact with the specified ID from the `contact_customer_accounts` collection of the account with the specified ID.

```powershell
Remove-FromCollection `
   -targetSetName 'accounts' `
   -targetId 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84 `
   -collectionName 'contact_customer_accounts' `
   -id 5d68b37f-aae9-4cd6-8b94-37d6439b2f34
```

### Remove-Record function

Deletes a record from a Dataverse table.

The `Remove-Record` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `DELETE` request to the Dataverse API. This function constructs the request URI by appending the entity set name and the record ID to the base URI. This function also adds the necessary headers. This function deletes the record with the specified ID from the table.

[Learn to delete records](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/update-delete-entities-using-web-api#basic-delete)

#### Remove-Record parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `setName` | string | **Required**. The name of the entity set to delete the record from. |
| `id` | Guid | **Required**. The GUID of the record to delete. |

#### Remove-Record returns

This function doesn't return a value.

#### Remove-Record example

This example deletes the account with the specified ID from the Dataverse table.

```powershell
Remove-Record `
   -setName accounts `
   -id 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84
```

### Set-ColumnValue function

Sets the value of a single property for a Dataverse record.

The `Set-ColumnValue` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `PUT` request to the Dataverse API. This function constructs the request URI by appending the entity set name, the record ID, and the property name to the base URI. This function also adds the necessary headers and converts the value to JSON format. This function overwrites the existing value of the property with the new value.

[Learn to update a single property value](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/update-delete-entities-using-web-api#update-a-single-property-value)

#### Set-ColumnValue parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `setName` | string | **Required**. The name of the entity set to update the record in. |
| `id` | string | **Required**. The GUID of the record to update. |
| `property` | string | **Required**. The name of the property to set the value for. |
| `value` | string | **Required**. The new value for the property. |

#### Set-ColumnValue returns

This function doesn't return a value.

#### Set-ColumnValue example

This example sets the `telephone1` column value of the contact with the specified ID to 555-0105.

```powershell
Set-ColumnValue `
   -setName 'contacts' `
   -id 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84 `
   -property 'telephone1' `
   -value '555-0105'
```

### Update-Record function

Updates an existing record in a Dataverse table.

The `Update-Record` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `PATCH` request to the Dataverse API. This function constructs the request URI by appending the entity set name and the record ID to the base URI. This function also adds the necessary headers and converts the body hashtable to JSON format. This function uses the `If-Match` header with a value of `'*'` to prevent creating a new record if the record ID does not exist.

[Learn to update a record](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/update-delete-entities-using-web-api#basic-update)

#### Update-Record parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `setName` | string | **Required**. The name of the entity set to update the record in. |
| `id` | Guid | **Required**. The GUID of the record to update. |
| `body` | hashtable | **Required**. A hashtable of attributes and values for the updated record. |

#### Update-Record returns

This function doesn't return a value.

#### Update-Record example

This example updates the annualincome and jobtitle of the contact with the specified ID.

```powershell
$body = @{
   'annualincome' = 80000
   'jobtitle'     = 'Junior Developer'
}

# Update the record with the data
Update-Record `
   -setName 'contacts' `
   -id 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84`
   -body $body
```

## Common functions

The [CommonFunctions.ps1](CommonFunctions.ps1) file currently contains just one function.

[Learn how to use functions](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/use-web-api-functions).

### Get-WhoAmI function

Gets the current user information from the Dataverse Web API.

The `Get-WhoAmI` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `GET` request to the Dataverse API. This function constructs the request URI by appending the [WhoAmI function](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/whoami?view=dataverse-latest) name to the base URI. This function also adds the necessary headers. This function returns an object that contains the user ID, business unit ID, and organization ID.

#### Get-WhoAmI parameters

This function doesn't have any parameters.

#### Get-WhoAmI returns

This function returns an instance of the [WhoAmIResponse ComplexType](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/whoamiresponse).

#### Get-WhoAmI example

This example gets the current user information from the Dataverse Web API.

```powershell
$WhoIAm = Get-WhoAmI
$myBusinessUnit = $WhoIAm.BusinessUnitId
$myUserId = $WhoIAm.UserId
```

## Metadata Operations functions

The [MetadataOperations.ps1](MetadataOperations.ps1) file contains these functions used in the [MetadataOperations sample](MetadataOperations/README.md).

### Export-Solution function

A function to export a solution from Dataverse.

#### Export-Solution parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `solutionName` | string | The name of the solution to be exported. |
| `managed` | boolean | A boolean value indicating whether the solution is managed. |
| `exportAutoNumberingSettings` | boolean | A boolean value indicating whether to export auto-numbering settings. |
| `exportCalendarSettings` | boolean | A boolean value indicating whether to export calendar settings. |
| `exportCustomizationSettings` | boolean | A boolean value indicating whether to export customization settings. |
| `exportEmailTrackingSettings` | boolean | A boolean value indicating whether to export email tracking settings. |
| `exportGeneralSettings` | boolean | A boolean value indicating whether to export general settings. |
| `exportMarketingSettings` | boolean | A boolean value indicating whether to export marketing settings. |
| `exportOutlookSynchronizationSettings` | boolean | A boolean value indicating whether to export Outlook synchronization settings. |
| `exportRelationshipRoles` | boolean | A boolean value indicating whether to export relationship roles. |
| `exportIsvConfig` | boolean | A boolean value indicating whether to export ISV configuration. |
| `exportSales` | boolean | A boolean value indicating whether to export sales settings. |
| `exportExternalApplications` | boolean | A boolean value indicating whether to export external applications settings. |
| `exportComponentsParams` | hashtable | A hashtable containing additional parameters for exporting components. |

#### Export-Solution returns

The function returns the exported solution as a byte array.

#### Export-Solution example

```powershell
$solutionFile = Export-Solution `
   -solutionName 'mySolution'`
   -managed $true
```

### Get-CanBeReferenced function

A function to check whether a table can be referenced in a relationship.

#### Get-CanBeReferenced parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table to be checked. |

#### Get-CanBeReferenced returns

A boolean value indicating whether the table can be referenced.

#### Get-CanBeReferenced example

```powershell
Get-CanBeReferenced -tableLogicalName "account"
```

### Get-CanBeReferencing function

A function to check whether a table can be referencing in a relationship.

#### Get-CanBeReferencing parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table to be checked. |

#### Get-CanBeReferencing returns

A boolean value indicating whether the table can be referencing.

#### Get-CanBeReferencing example

```powershell
Get-CanBeReferencing -tableLogicalName "account"
```

### Get-CanManyToMany function

A function to check if a table can have many-to-many relationships in Dataverse.

#### Get-CanManyToMany parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table to be checked. |

#### Get-CanManyToMany returns

A boolean value indicating whether the table can have many-to-many relationships.

#### Get-CanManyToMany example

```powershell
Get-CanManyToMany -tableLogicalName "account"
```

### Get-Column function

A function to retrieve a column from a Dataverse table.

#### Get-Column parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table from which the column will be retrieved. |
| `logicalName` | string | The logical name of the column to be retrieved. |
| `type` | string |T he type of the column to be retrieved. This function supports the following types: 'BigInt', 'Boolean', 'DateTime', 'Decimal', 'Double', 'File', 'Image', 'Integer', 'Lookup', 'ManagedProperty', 'Memo', 'Money', 'String', 'EntityName', 'UniqueIdentifier', 'MultiSelectPicklist', 'Picklist', 'State', 'Status'. |
| `query` | string | The query string to be appended to the base URI to form the complete URI for the GET request. |

#### Get-Column returns

Details of the specified column.

#### Get-Column example

```powershell
Get-Column `
   -tableLogicalName 'account' `
   -logicalName 'accountid' `
   -type 'UniqueIdentifier" `
   -query "?`$select=SchemaName,DisplayName,AttributeType"
```

### Get-GlobalOptionSet function

A function to retrieve a global option set from Dataverse.

#### Get-GlobalOptionSet parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `name` | string | The name of the global option set to be retrieved. Either this or the id must be provided. |
| `id` | string | The GUID of the global option set to be retrieved. Either this or the name must be provided. |
| `type` | string | The type of the global option set to be retrieved. It can be 'OptionSet' or 'Boolean'. If this parameter isn't provided, the function doesn't enable expanding the options. |
| `query` | string | An OData query string to filter the global option set to be retrieved. |

#### Get-GlobalOptionSet returns

Returns the global option set if found, or `$null` if not found.
If the server returns an error other than 404, the function will throw an exception.

#### Get-GlobalOptionSet example

```powershell
Get-GlobalOptionSet -name "new_globaloptionset" -type "OptionSet"
```

### Get-Relationship function

A function to retrieve a relationship from Dataverse.

#### Get-Relationship parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `schemaName` | string | The schema name of the relationship to be retrieved. Either this or the id must be provided. |
| `id` | GUID | The GUID of the relationship to be retrieved. Either this or the schema name must be provided. |
| `type` | string | The type of the relationship to be retrieved. It can be 'OneToMany', 'ManyToOne', or 'ManyToMany'. If this parameter is not provided, the function will not enable expanding or selecting type specific properties. |
| `query` | string | An OData query string to filter the relationship to be retrieved. |

#### Get-Relationship returns

The relationship if found.

#### Get-Relationship example

```powershell
Get-Relationship -schemaName "new_account_customer" -type "OneToMany"
```

### Get-Relationships function

A function to retrieve relationships from Dataverse.

#### Get-Relationships parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `query` | string | An OData query string to filter the relationships to be retrieved. |
| `isManyToMany` | boolean | A boolean value indicating whether to retrieve many-to-many relationships. If this parameter is set to true, many-to-many relationships are retrieved; otherwise, one-to-many relationships are retrieved. |

#### Get-Relationships returns

The relationships that match the provided query.

#### Get-Relationships example

```powershell
$relationshipQuery = "?`$filter=SchemaName eq '"
$relationshipQuery += 'sample_BankAccount_Contacts'
$relationshipQuery += "'&`$select=SchemaName"
   
$relationshipQueryResults = (Get-Relationships `
      -query $relationshipQuery `
      -isManyToMany $false).value
```

### Get-Table function

A function to get a table definition from Dataverse.

#### Get-Table parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `logicalName` | string | The logical name of the table to be retrieved. |
| `query` | string | The query string to be appended to the base URI to form the complete URI for the GET request. |

#### Get-Table returns

The definition of the table.

#### Get-Table example

```powershell
$bankAccountTable = Get-Table -logicalName 'new_bankaccount' `
   -query "?`$select=SchemaName,DisplayName,TableType"
```

### Get-TableColumns function

A function to retrieve the columns of a table in Dataverse.

#### Get-TableColumns parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table whose columns are to be retrieved. |
| `query` | string | The query string to be appended to the base URI to form the complete URI for the GET request. |

#### Get-TableColumns returns

The columns of the table that match the query.

#### Get-TableColumns example

```powershell
Get-TableColumns -tableLogicalName 'account' -query "?`$filter=SchemaName eq 'Name'"
```

### Get-Tables function

Gets table definitions from Dataverse.

#### Get-Tables Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `query` | string | **Required**. The query string to be appended to the base URI to form the complete URI for the GET request. |

#### Get-Tables returns

This function returns the response containing the table definitions.

#### Get-Tables example

This example gets the `SchemaName`, `DisplayName`, and `TableType` of the table with the specified schema name.

```powershell
$tableQuery = "?`$filter=SchemaName eq '"
$tableQuery += 'new_BankAccount'
$tableQuery += "'&`$select=SchemaName,DisplayName,TableType"
   
$tableQueryResults = (Get-Tables `
      -query $tableQuery).value
```

### Get-ValidManyToManyTables function

A function to get valid tables for many-to-many relationships in Dataverse.

#### Get-ValidManyToManyTables example

```powershell
Get-ValidManyToManyTables
```

#### Get-ValidManyToManyTables returns

An array of strings, each string being the logical name of a valid table for many-to-many relationships.

### Get-ValidReferencingTables function

A function to get valid referencing tables for a specified table in Dataverse.

#### Get-ValidReferencingTables parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table for which to retrieve valid referencing tables. |

#### Get-ValidReferencingTables returns

An array of strings, each string being the logical name of a valid referencing table.

#### Get-ValidReferencingTables example

```powershell
Get-ValidReferencingTables -tableLogicalName "account"
```

### Import-Solution function

A function to import a solution into Dataverse.

#### Import-Solution parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `customizationFile` | byte array | The solution file to be imported. |
| `overwriteUnmanagedCustomizations` | boolean | A boolean value indicating whether to overwrite unmanaged customizations. |
| `importJobId` | GUID | The GUID of the import job. |
| `publishWorkflows` | boolean | A boolean value indicating whether to publish workflows. |
| `convertToManaged` | boolean | A boolean value indicating whether to convert the solution to managed. |
| `skipProductUpdateDependencies` | boolean | A boolean value indicating whether to skip product update dependencies. |
| `holdingSolution` | boolean | A boolean value indicating whether the solution is a holding solution. |
| `componentParameters` | array of hashtables | An array of hashtables containing additional parameters for importing components. |
| `solutionParameters` | hashtable | A hashtable containing additional parameters for importing the solution. |

#### Import-Solution returns

This function doesn't return a value.

#### Import-Solution example

```powershell
$importJobId = New-Guid
   
Import-Solution `
   -customizationFile ([System.IO.File]::ReadAllBytes("C:\path\to\solution.zip")) `
   -overwriteUnmanagedCustomizations $false `
   -importJobId $importJobId
```

### New-Column function

A function to create a new column in a Dataverse table.

#### New-Column parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table where the new column will be created. |
| `column` | hashtable | A hashtable that represents the new column to be created. It should contain the details of the column. |
| `solutionUniqueName` | string | The unique name of the solution where the table is located. If this parameter is not provided, the column will be created in the table in the default solution. |

#### New-Column returns

The GUID of the newly created column.

#### New-Column example

```powershell
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
```

### New-CustomerRelationship function

A function to create a new customer relationship in Dataverse.

#### New-CustomerRelationship parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `lookup` | hashtable | A hashtable containing the details of the lookup field for the customer relationship. |
| `oneToManyRelationships` | array | An array of hashtables, each containing the details of a one-to-many relationship for the customer relationship. |
| `solutionUniqueName` | string | The unique name of the solution where the customer relationship will be created. If this parameter isn't provided, the customer relationship is created in the default solution. |

#### New-CustomerRelationship returns

A [CreateCustomerRelationshipsResponse complex type](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/createcustomerrelationshipsresponse) which includes the ID values of the lookup column and the relationships created to support it.

#### Example

```powershell
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
```

### New-GlobalOptionSet function

A function to create a new global option set in Dataverse.

#### New-GlobalOptionSet parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `optionSet` | hashtable | A hashtable containing the details of the global option set to be created. |
| `solutionUniqueName` | string | The unique name of the solution where the global option set will be created. If this parameter is not provided, the global option set will be created in the default solution. |

#### New-GlobalOptionSet returns

The GUID ID of the newly created global option set.

#### New-GlobalOptionSet example

```powershell
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
```

### New-OptionValue function

A function to create a new option value in a column in a Dataverse table.

#### New-OptionValue parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table where the column is located. |
| `columnLogicalName` | string | The logical name of the column where the new option value is created.|
| `label` | string | The label for the new option value. |
| `languageCode` | integer | The language code for the label. |
| `value` | varies | The value for the new option. |
| `solutionUniqueName` | string | The unique name of the solution where the table is located. If this parameter is not provided, the new option value is created in the table in the default solution.|

#### New-OptionValue returns

The value of the newly created option.

#### New-OptionValue example

```powershell
New-OptionValue `
   -tableLogicalName 'sample_bankaccount' `
   -columnLogicalName 'sample_picklist' `
   -label 'Echo' `
   -languageCode 1033 `
   -solutionUniqueName 'mysolution'
```

### New-Relationship function

A function to create a new relationship in Dataverse.

#### New-Relationship parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `relationship` | hashtable | A hashtable containing the details of the relationship to be created. |
| `solutionUniqueName` | string | The unique name of the solution where the relationship is created. If this parameter isn't provided, the relationship is created in the default solution.|

#### New-Relationship returns

The GUID of the created relationship.

#### New-Relationship example

```powershell
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
```

### New-StatusOption function

A function to create a new status option in a Dataverse table column.

#### New-StatusOption parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the Dataverse table where the status option is created. |
| `label` | string | The label of the new status option. |
| `languageCode` | integer | The language code for the label and description of the new status option. |
| `stateCode` | integer | The state code of the new status option. |
| `value` | varies | The value of the new status option. If this parameter isn't provided, a value is automatically assigned. |
| `color` | string | The color of the new status option. If this parameter isn't provided, a default color is used. |
| `description` | string | The description of the new status option. |
| `solutionUniqueName` | string | The unique name of the solution where the Dataverse table is located. If this parameter isn't provided, the status option is created in the table in the default solution.|

#### New-StatusOption returns

The value of the new status option.

#### New-StatusOption example

```powershell
New-StatusOption `
   -tableLogicalName "account" `
   -label "New Status" `
   -languageCode 1033 `
   -stateCode 1 `
   -value 100000000 `
   -color "FF0000" `
   -description "This is a new status option" `
   -solutionUniqueName "MySolution"
```

### New-Table function

A function to create a new Dataverse table.

#### New-Table parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `body` | hashtable | The body of the POST request, which should be a hashtable containing the details of the table to be created. |
| `solutionUniqueName` | string | The unique name of the solution where the table is created. If this parameter isn't provided, the table is created in the default solution.|

#### New-Table returns

The GUID ID of the newly created table.

#### New-Table example

```powershell
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

```

### Remove-OptionValue function

A function to remove an option value from a column in a Dataverse table.

#### Remove-OptionValue parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table where the column is located. |
| `columnLogicalName` | string | The logical name of the column where the option value is located. |
| `value` | varies | The value of the option to be removed. |
| `solutionUniqueName` | string | The unique name of the solution where the table is located. If this parameter isn't provided, the option value wis removed from the table in the default solution. |

#### Remove-OptionValue returns

This function doesn't return a value.

#### Remove-OptionValue example

```powershell
Remove-OptionValue `
   -tableLogicalName "account" `
   -columnLogicalName "new_type" `
   -value 3 `
   -solutionUniqueName "MySolution"
```

### Update-Column function

A function to update a column in a Dataverse table.

#### Update-Column parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table where the column will be updated. |
| `column` | PSCustomObject | A PSCustomObject that represents the column to be updated. The column should contain all the properties retrieved from Dataverse, including the LogicalName property. |
| `type` | string | The type of the column to be updated. This function supports the following types: 'BigInt', 'Boolean', 'DateTime', 'Decimal', 'Double', 'File', 'Image', 'Integer', 'Lookup', 'ManagedProperty', 'Memo', 'Money', 'String', 'UniqueIdentifier'. |
| `solutionUniqueName` | string | The unique name of the solution where the table is located. If this parameter isn't provided, the column is updated in the table in the default solution. |
| `mergeLabels` | boolean | A boolean value that indicates whether to merge labels during the update operation. |

#### Update-Column returns

This function doesn't return a value.

#### Update-Column example

```powershell
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
```

### Update-OptionValue function

A function to update the value of an option in a column in a table in Dataverse.

#### Update-OptionValue parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table where the column is located. |
| `columnLogicalName` | string | The logical name of the column where the option is located. |
| `value` | varies | The new value for the option. |
| `label` | string | The new label for the option. |
| `languageCode` | integer | The language code for the label. |
| `mergeLabels` | boolean | A boolean value that indicates whether to merge labels during the update operation. |
| `solutionUniqueName` | string | The unique name of the solution where the table is located. If this parameter isn't provided, the option is updated in the table in the default solution. |

#### Update-OptionValue returns

This function doesn't return a value.

#### Update-OptionValue example

```powershell
Update-OptionValue `
   -tableLogicalName "account" `
   -columnLogicalName "sample_picklist" `
   -value 1 `
   -label "New Label" `
   -languageCode 1033 `
   -mergeLabels $true `
   -solutionUniqueName "MySolution"
```

### Update-OptionsOrder function

A function to update the order of options in a column in a Dataverse table.

#### Update-OptionsOrder parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `tableLogicalName` | string | The logical name of the table where the column is located. |
| `columnLogicalName` | string | The logical name of the column where the options are located. |
| `values` | array of integers | An array of integers representing the new order of the options. |
| `solutionUniqueName` | string | The unique name of the solution where the table is located. If this parameter isn't provided, the order of options is updated in the table in the default solution. |

#### Update-OptionsOrder returns

This function doesn't return a value.

#### Update-OptionsOrder example

```powershell
Update-OptionsOrder `
   -tableLogicalName "account" `
   -columnLogicalName "sample_type" `
   -values @(3, 1, 2) `
   -solutionUniqueName "MySolution"
```

### Update-Table function

A function to update an existing Dataverse table.

#### Update-Table parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `table` | PSCustomObject | A PSCustomObject that represents the table to be updated. This table must include all the properties retrieved from Dataverse.|
| `solutionUniqueName` | string | The unique name of the solution where the table is located. If this parameter isn't provided, the table in the default solution is updated. |
| `mergeLabels` | boolean | A boolean value that indicates whether to merge labels during the update. If this parameter isn't provided, labels aren't merged.|

#### Update-Table returns

This function doesn't return a value.

#### Update-Table example

```powershell
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
```
