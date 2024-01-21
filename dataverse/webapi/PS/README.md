# Dataverse Web API PowerShell Helper functions

The files in this folder are PowerShell helper functions that Dataverse Web API PowerShell samples use. These samples are separated in the following files:

|File|Description|
|---|---|
|[Core.ps1](Core.ps1)|Contains functions that all other functions or samples depend on.|
|[TableOperations.ps1](TableOperationsre.ps1)|Contains functions that enable performing data operations on table rows|
|[CommonFunctions.ps1](CommonFunctions.ps1)|Contains common Dataverse functions|

Samples that use these common functions reference them using [dot sourcing](https://learn.microsoft.com/powershell/module/microsoft.powershell.core/about/about_scripts#script-scope-and-dot-sourcing) as demonstrated by the [BasicOperations/BasicOperations.ps1](BasicOperations/BasicOperations.ps1):

```powershell
. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1
```

## Function list

|Group|Function|Description|
|---|---|---|
|Core|[Connect](#connect-function)|Connects to Dataverse Web API using Azure authentication.|
|Core|[Invoke-DataverseCommands](#invoke-dataversecommands-function)|Invokes a set of commands against the Dataverse Web API.|
|Core|[Invoke-ResilientRestMethod](#invoke-resilientrestmethod-function)|Invokes a REST method with resilience to handle 429 errors.|
|TableOperations|[Add-ToCollection](#add-tocollection-function)|Adds a record to a collection-valued navigation property of another record.|
|TableOperations|[Get-ColumnValue](#get-columnvalue-function)|Gets the value of a single property from a Dataverse record.|
|TableOperations|[Get-Record](#get-record-function)|Gets a single record from a Dataverse table by its primary key value.|
|TableOperations|[Get-Records](#get-records-function)|Gets a set of records from a Dataverse table|
|TableOperations|[New-Record](#new-record-function)|Creates a new record in a Dataverse table.|
|TableOperations|[Remove-FromCollection](#remove-fromcollection-function)|Removes a record from a collection-valued navigation property of another record.|
|TableOperations|[Remove-Record](#remove-record-function)|Deletes a record from a Dataverse table.|
|TableOperations|[Set-ColumnValue](#set-columnvalue-function)|Sets the value of a single property for a Dataverse record.|
|TableOperations|[Update-Record](#update-record-function)|Updates an existing record in a Dataverse table.|
|CommonFunctions|[Get-WhoAmI](#get-whoami-function)|Gets the current user information from the Dataverse Web API.|

## Core functions

The [Core.ps1](Core.ps1) file contains these variables and functions.

### Variables

Set these variables when debugging using Fiddler:

|Variable|Type|Description|
|---|---|---|
|`$debug`|bool|Set to `$true` only while debugging with Fiddler|
|`$proxyUrl`|string|Set this value to the Fiddler proxy URL configured on your computer. The default value is `http://127.0.0.1:8888`.|

These global variables are set by the [Connect function](#connect-function).

|Variable|Type|Description|
|---|---|---|
|`$baseHeaders`|hashtable|Includes the request headers that should be used by all Dataverse Web API calls, including the `Authorization` header with the bearer access token to enable authentication.|
|`$baseURI`|string|The URL to the root of the Dataverse Web API. Many operations can use relative URL references. This value is useful for certain operations where the absolute URL is necessary.|

### Connect function

Connects to Dataverse Web API using Azure authentication.

The `Connect` function uses the `Get-AzAccessToken` cmdlet to obtain an access token for the specified resource URI. 
It then sets the global variables `$baseHeaders` and `$baseURI` to be used for subsequent requests to the resource.

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`uri`|string|**Required**. The URL for the Dataverse environment. |


#### Returns

This function doesn't return a value.

#### Example

```powershell
Connect -uri 'https://yourorg.crm.dynamics.com'
```

### Invoke-DataverseCommands function

Invokes a set of commands against the Dataverse Web API.

The `Invoke-DataverseCommands` function uses the `Invoke-Command` cmdlet to run a script block of commands against the Dataverse Web API.
It handles any errors that may occur from the Dataverse API or the script itself.

[Learn about handling Dataverse Web API errors](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/compose-http-requests-handle-errors#parse-errors-from-the-response)

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`$commands`|command block|**Required**. The script block of commands to run against the Dataverse resource.|


#### Returns

This function doesn't return a value, but it intercepts and parses errors returned from the Dataverse Web API. Other types of errors are returned without processing.

#### Example

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

The `Invoke-ResilientRestMethod` function uses the `Invoke-RestMethod` cmdlet to send an HTTP request to a RESTful web service. 
It handles any 429 errors by retrying the request using the `Retry-After` header value as the retry interval, which Dataverse provides.
It also supports using a proxy if the `$debug` variable is set to true.

[Learn about Dataverse service protection limits](https://learn.microsoft.com/power-apps/developer/data-platform/api-limits)

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`request`|hashtable|**Required**. Parameters to pass to the `Invoke-RestMethod` cmdlet.|
|`returnHeader`|bool|Whether to return the response headers instead of the response body.|


#### Returns

This function doesn't return a value.

#### Example

See the functions in the [TableOperations.ps1](TableOperations.ps1) file for examples using this function.


## Table Operation functions

The [TableOperations.ps1](TableOperations.ps1) file contains these functions.

### Add-ToCollection function

Adds a record to a collection-valued navigation property of another record.

The `Add-ToCollection` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `POST` request to the Dataverse API.
It constructs the request URI by appending the target entity set name, the target record ID, and the collection name to the base URI.
It also adds the necessary headers and converts the record URI to JSON format.
It creates a reference between the target record and the record to be added to the collection.

[Learn to associate and disassociate table rows](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/associate-disassociate-entities-using-web-api)

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`targetSetName`|string|**Required**. The name of the entity set that contains the target record. |
|`targetId`|string|**Required**. The GUID of the target record.|
|`collectionName`|string|**Required**. The name of the collection-valued navigation property of the target record.|
|`setName`|string|**Required**. The name of the entity set that contains the record to be added to the collection.|
|`id`|string|**Required**. The GUID of the record to be added to the collection.|


#### Returns

This function doesn't return a value.

#### Example

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

The `Get-ColumnValue` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `GET` request to the Dataverse API.
It constructs the request URI by appending the entity set name, the record ID, and the property name to the base URI.
It also adds the necessary headers to avoid caching. It returns the value of the property as a string.

[Learn to retrieve specific properties](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/retrieve-entity-using-web-api#retrieve-specific-properties)

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`setName`|string|**Required**. The name of the entity set to retrieve the record from. |
|`id`|Guid|**Required**. The GUID of the record to retrieve.|
|property|string|**Required**. The name of the property to get the value from.|


#### Returns

The value of the property as a string.

#### Example

This example gets the `telephone1` value of the contact record with the specified ID.

```powershell
$telephone1 = Get-ColumnValue `
   -setName 'contacts' `
   -id 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84 `
   -property 'telephone1'
```

### Get-Record function

Gets a single record from a Dataverse table by its primary key value.

The `Get-Record` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `GET` request to the Dataverse API. 
It constructs the request URI by appending the entity set name, the record ID, and the query parameters to the base URI. 
It also adds the necessary headers to include annotations in the response. It returns the record as an object.

[Learn to retrieve a table row](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/retrieve-entity-using-web-api)

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`setName`|string|**Required**.The name of the entity set to retrieve the record from.|
|`id`|guid|**Required**. The GUID of the record to retrieve.|
|`query`|string|The query parameters to filter, expand, or select the record properties.|

#### Returns

This function returns the response containing the record data.

#### Example

This example gets the `fullname`, `annualincome`, `jobtitle`, and `description` of the contact with the specified ID.

```powershell
$retrievedRafelShillo1 = Get-Record `
   -setName 'contacts' `
   -id $rafelShilloId `
   -query '?$select=fullname,annualincome,jobtitle,description'
```

### Get-Records function

Gets a set of records from a Dataverse table.

The `Get-Records` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `GET` request to the Dataverse API.
It constructs the request URI by appending the entity set name and the query parameters to the base URI.
It also adds the necessary headers to include annotations in the response.

[Learn to Query data using the Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query-data-web-api)

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`setName`|string|**Required**.The name of the entity set to retrieve records from.|
|`query`|string|**Required**.The query parameters to filter, sort, or select the records.|

#### Returns

Returns the response that contains properties about the collection of records returned. These properties are useful when paging requests. The array of records matching the request is in the `value` property.

#### Example

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

The `New-Record` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `POST` request to the Dataverse Web API.
It constructs the request URI by appending the entity set name to the base URI.
It also adds the necessary headers and converts the `body` hashtable to JSON format.
It returns the GUID ID value of the created record.

[Learn to create records](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/create-entity-web-api)

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`setName`|string|**Required**. The name of the entity set to create a record in.|
|`body`|hashtable|**Required**. A hashtable of attributes and values for the new record.|


#### Returns

This function returns the GUID value of the record created.

#### Example

This example creates a new contact record with the `firstname` 'Rafel' and the `lastname` 'Shillo'. It returns the GUID ID of the created record.

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

The `Remove-FromCollection` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `DELETE` request to the Dataverse API.
It constructs the request URI by appending the target entity set name, the target record ID, the collection name, and the record ID to the base URI.
It also adds the necessary headers. It deletes the reference between the target record and the record to be removed from the collection.

[Learn to associate and disassociate table rows using the Web API](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/associate-disassociate-entities-using-web-api)

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`targetSetName`|string|**Required**. The name of the entity set that contains the target record. |
|`targetId`|Guid|**Required**. The ID of the target record. |
|`collectionName`|string|**Required**. The name of the collection-valued navigation property of the target record.|
|`id`|Guid|**Required**. The ID of the record to be removed from the collection.|

#### Returns

This function doesn't return a value.

#### Example

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

The `Remove-Record` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `DELETE` request to the Dataverse API.
It constructs the request URI by appending the entity set name and the record ID to the base URI.
It also adds the necessary headers. It deletes the record with the specified ID from the table.

[Learn to delete records](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/update-delete-entities-using-web-api#basic-delete)

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`setName`|string|**Required**. The name of the entity set to delete the record from.|
|`id`|Guid|**Required**. The GUID of the record to delete.|

#### Returns

This function doesn't return a value.

#### Example

This example deletes the account with the specified ID from the Dataverse table.

```powershell
Remove-Record `
   -setName accounts `
   -id 9ec0b0ec-d6c3-4b8d-bd75-435723b49f84
```

### Set-ColumnValue function

Sets the value of a single property for a Dataverse record.

The `Set-ColumnValue` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `PUT` request to the Dataverse API.
It constructs the request URI by appending the entity set name, the record ID, and the property name to the base URI.
It also adds the necessary headers and converts the value to JSON format.
It overwrites the existing value of the property with the new value.

[Learn to update a single property value](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/update-delete-entities-using-web-api#update-a-single-property-value)

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`setName`|string|**Required**. The name of the entity set to update the record in.|
|`id`|string|**Required**. The GUID of the record to update.|
|`property`|string|**Required**. The name of the property to set the value for.|
|`value`|string|**Required**. The new value for the property.|

#### Returns

This function doesn't return a value.

#### Example

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

The `Update-Record` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `PATCH` request to the Dataverse API.
It constructs the request URI by appending the entity set name and the record ID to the base URI.
It also adds the necessary headers and converts the body hashtable to JSON format.
It uses the `If-Match` header with a value of `'*'` to prevent creating a new record if the record ID does not exist.

[Learn to update a record](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/update-delete-entities-using-web-api#basic-update)

#### Parameters

|Parameter|Type|Description|
|---|---|---|
|`setName`|string|**Required**. The name of the entity set to update the record in.|
|`id`|Guid|**Required**. The GUID of the record to update.|
|`body`|hashtable|**Required**. A hashtable of attributes and values for the updated record.|


#### Returns

This function doesn't return a value.

#### Example

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

[Learn how to use functions](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/use-web-api-functions)

### Get-WhoAmI function

Gets the current user information from the Dataverse Web API.

The `Get-WhoAmI` function uses the [Invoke-ResilientRestMethod function](#invoke-resilientrestmethod-function) to send a `GET` request to the Dataverse API.
It constructs the request URI by appending the [WhoAmI function](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/whoami?view=dataverse-latest) name to the base URI.
It also adds the necessary headers. It returns an object that contains the user ID, business unit ID, and organization ID.

#### Parameters

This function doesn't have any parameters.

#### Returns

This function returns an instance of the [WhoAmIResponse ComplexType](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/whoamiresponse)

#### Example

This example gets the current user information from the Dataverse Web API.

```powershell
$WhoIAm = Get-WhoAmI
$myBusinessUnit = $WhoIAm.BusinessUnitId
$myUserId = $WhoIAm.UserId
```
