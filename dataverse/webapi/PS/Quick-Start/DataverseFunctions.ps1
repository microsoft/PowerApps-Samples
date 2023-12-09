<#
   .SYNOPSIS
   Connects to a Dataverse environment

   .DESCRIPTION
   The Connect function uses the Azure CLI to obtain an access token for the specified 
   Dataverse environment. It sets the global variables $environmentUrl and $accessToken with the 
   values of the uri parameter and the token respectively. It also displays the expiration time 
   of the token.

   .PARAMETER uri 
   The url for the Dataverse environment you want to connect to. 
   For example, 'https://contoso.crm.dynamics.com'.

   .EXAMPLE
   PS> Connect -uri 'https://yourorg.crm.dynamics.com/'
   ERROR: Please run 'az login' to setup account.
   WARNING: A web browser has been opened at https://login.microsoftonline.com/organizations/oauth2/v2.0/authorize. Please continue the login in the web browser. If no web browser is available or if the web browser fails to open, use device code flow with `az login --use-device-code`.
   Connected to https://yourorg.crm.dynamics.com/
   Token will expire in 30 minutes.

   .EXAMPLE
   PS> Connect -uri 'https://yourorg.crm.dynamics.com/'
   Connected to https://yourorg.crm.dynamics.com/
   Token will expire in 30 minutes.
#>
function Connect {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $uri
   )

   # Set environment URL
   $global:environmentUrl = $uri

   ## Login if not already logged in
   if ($null -eq (az account tenant list  --only-show-errors)) {
      az login --allow-no-subscriptions | Out-Null
   }
   # Get token
   $token = az account get-access-token --resource=$environmentUrl --output json
   $tokenObj = $token | ConvertFrom-Json
   # Set accessToken
   $global:accessToken = $tokenObj.accessToken
   # Get minutes to token expiration
   $minutesToTokenExpire = (New-TimeSpan -End ([DateTime]$tokenObj.expiresOn)).Minutes
   # Show information
   Write-Output "Connected to $environmentUrl"
   Write-Output "Token will expire in $minutesToTokenExpire minutes."
}

# Define common set of headers
$baseHeaders = @{
   'Authorization'    = "Bearer $accessToken"
   'Accept'           = 'application/json'
   'OData-MaxVersion' = '4.0'
   'OData-Version'    = '4.0'
}

<#
   .SYNOPSIS
   Gets the information about the current user, organization, and business unit.

   .DESCRIPTION
   The Get-WhoAmI function uses the Invoke-RestMethod cmdlet to send a GET request to the Dataverse Web API. 
   It retrieves the information about the current user, organization, and business unit with the 
   WhoAmI function. 
   It returns the user ID, business unit ID, and organization ID as properties.

   .PARAMETER None 
   This function does not take any parameters.

   .OUTPUTS
   Microsoft.Dynamics.CRM.WhoAmIResponse ComplexType 

   .EXAMPLE
   PS> Get-WhoAmI | Format-List -Property BusinessUnitId,UserId,OrganizationId

   BusinessUnitId : 6d8b5aa9-2845-4bd7-ae9b-947ecbeba47c
   UserId         : a7436a58-33af-4e54-8fbe-d73b4050646b
   OrganizationId : e702c361-9c7f-4f26-bc79-5619f2809b17
#>
function Get-WhoAmI {
   $WhoAmIRequest = @{
      Uri     = $environmentUrl + 'api/data/v9.2/WhoAmI'
      Method  = 'Get'
      Headers = $baseHeaders
   }
   Invoke-RestMethod @WhoAmIRequest
}

<#
   .SYNOPSIS
   Returns a collection of records from a table that match a query

   .DESCRIPTION
   The Get-Records function returns a collection of records from the table with the 
   matching -setName parameter filtered by the -query parameter

   .PARAMETER setName 
   The table set name (EntityMetadata.EntitySetName property) of the records to retrieve. 
   For example, 'accounts' or 'contacts'.

   .PARAMETER query 
   The ODATA query to select properties, expand navigation properties, and filter results. 
   For example, '?$select=name,address1_city&$filter=address1_city eq ''Seattle''&$top=10'.

   .OUTPUTS
   Collection of table records 

   .EXAMPLE 
   Get-Records -setName 'accounts' -query '?$select=name,address1_city&$filter=address1_city eq ''Seattle''&$top=10'

   .EXAMPLE
   PS> Get-Records accounts '?$select=name&$top=3'

   @odata.context                                        : https://yourorg.crm.dynamics.com/api/data/v9.2/$metadata#accounts(name)
   @Microsoft.Dynamics.CRM.totalrecordcount              : -1
   @Microsoft.Dynamics.CRM.totalrecordcountlimitexceeded : False
   @Microsoft.Dynamics.CRM.globalmetadataversion         : 103348253
   value                                                 : {@{@odata.etag=W/"102872096"; name=Adatum Corporation;
                                                         accountid=4b757ff7-9c85-ee11-8179-000d3a9933c9}, @{@odata.etag=W/"103323309";
                                                         name=Adventure Works Cycles; accountid=2ada33e7-ef8b-ee11-8179-000d3a9933c9},
                                                         @{@odata.etag=W/"103323312"; name=Alpine Ski House;
                                                         accountid=2eda33e7-ef8b-ee11-8179-000d3a9933c9}}

   .EXAMPLE
   PS> (Get-Records accounts '?$select=name&$top=3').value

   @odata.etag   name                   accountid
   -----------   ----                   ---------
   W/"102872096" Adatum Corporation     4b757ff7-9c85-ee11-8179-000d3a9933c9
   W/"103323309" Adventure Works Cycles 2ada33e7-ef8b-ee11-8179-000d3a9933c9
   W/"103323312" Alpine Ski House       2eda33e7-ef8b-ee11-8179-000d3a9933c9


   .EXAMPLE
   PS> (Get-Records accounts '?$select=name&$top=3').value | Format-Table -Property name,accountid

   name                   accountid
   ----                   ---------
   Adatum Corporation     4b757ff7-9c85-ee11-8179-000d3a9933c9
   Adventure Works Cycles 2ada33e7-ef8b-ee11-8179-000d3a9933c9
   Alpine Ski House       2eda33e7-ef8b-ee11-8179-000d3a9933c9
#>
function Get-Records {
   param (
      [Parameter(Mandatory)] 
      [String] 
      $setName,
      [Parameter(Mandatory)] 
      [String] 
      $query
   )
   $uri = $environmentUrl + 'api/data/v9.2/' + $setName + $query

   # Header for GET operations that have annotations
   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $null)
   $getHeaders.Add('Prefer', 'odata.include-annotations="*"')

   $RetrieveMultipleRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }
   Invoke-RestMethod @RetrieveMultipleRequest
}

<# 
   .SYNOPSIS 
   Creates a record in a table.

   .DESCRIPTION 
   The New-Record function uses the Invoke-RestMethod cmdlet to send a POST request to the Dataverse Web API. 
   It creates a new record in the specified table set with the properties and values provided in the body parameter. 
   It returns the GUID of the created record.

   .PARAMETER setName 
   The table set name (EntityMetadata.EntitySetName property) of the record to create. 
   For example, 'accounts' or 'contacts'.

   .PARAMETER body 
   A hashtable that contains the properties and values of the record to create. 
   For example, @{name = 'Contoso'; address1_city = 'Seattle'}.

   .EXAMPLE 
   New-Record -setName 'accounts' -body @{name = 'Contoso'; address1_city = 'Seattle'}

   This example creates a new account record with the name 'Contoso' and the city 'Seattle'. 
   It returns the GUID of the created record. 
#>
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
      Uri     = $environmentUrl + 'api/data/v9.2/' + $setName
      Method  = 'Post'
      Headers = $postHeaders
      Body    = ConvertTo-Json $body
   }
   Invoke-RestMethod @CreateRequest  -ResponseHeadersVariable rh
   $url = $rh['OData-EntityId']
   $selectedString = Select-String -InputObject $url -Pattern '(?<=\().*?(?=\))'
   return [System.Guid]::New($selectedString.Matches.Value.ToString())
}

<# 
   .SYNOPSIS 
   Retrieves a record from a table set using the primary key.

   .DESCRIPTION 
   The Get-Record function uses the Invoke-RestMethod cmdlet to send a GET request to the Dataverse Web API. 
   It retrieves a record from the specified table set using the GUID primary key value provided in the id parameter. 
   It optionally accepts an ODATA query parameter to select properties, filter results and expand navigation properties. 
   It returns the record as a PowerShell object.

   .PARAMETER setName 
   The table set name (EntityMetadata.EntitySetName property) of the record to retrieve. 
   For example, 'accounts' or 'contacts'.

   .PARAMETER id 
   The GUID primary key value of the record to retrieve. 
   For example, '00000000-0000-0000-0000-000000000000'.

   .PARAMETER query 
   The ODATA query to select properties, filter results and expand navigation properties. 
   For example, '?$select=name,address1_city&$expand=primarycontactid($select=fullname)'.

   .EXAMPLE 
   Get-Record -setName 'accounts' -id '00000000-0000-0000-0000-000000000000'

   This example retrieves the account record with the specified GUID and returns all the properties.

   .EXAMPLE 
   Get-Record -setName 'accounts' -id '00000000-0000-0000-0000-000000000000' -query '?$select=name,address1_city&$expand=primarycontactid($select=fullname)'

   This example retrieves the account record with the specified GUID and returns only the name and city properties, as well as the full name of the primary contact. 

   .EXAMPLE
   Get-Record `
   -setName  accounts `
   -id $newAccountID.Guid '?$select=name,accountcategorycode' |
   Format-List -Property name,
   accountid,
   accountcategorycode,
   accountcategorycode@OData.Community.Display.V1.FormattedValue

   This example retrieves the account record with the specified GUID and returns only the name, 
   accountid, accountcategorycode, and the formatted value for the account category code.

   name                                                          : Example Account
   accountid                                                     : f20412c1-1592-ee11-be37-000d3a993550
   accountcategorycode                                           : 1
   accountcategorycode@OData.Community.Display.V1.FormattedValue : Preferred Customer

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

   $uri = $environmentUrl + 'api/data/v9.2/' + $setName
   $uri = $uri + '(' + $id.Guid + ')' + $query

   $getHeaders = $baseHeaders.Clone()
   $getHeaders.Add('If-None-Match', $null)
   $getHeaders.Add('Prefer', 'odata.include-annotations="*"')

   $RetrieveRequest = @{
      Uri     = $uri
      Method  = 'Get'
      Headers = $getHeaders
   }
   Invoke-RestMethod @RetrieveRequest
}

<# 
   .SYNOPSIS 
   Updates a record in a table using the primary key.

   .DESCRIPTION 
   The Update-Record function uses the Invoke-RestMethod cmdlet to send a PATCH request to the Dataverse Web API. 
   It updates an existing record in the specified table using the GUID primary key value provided in the id parameter. 
   It accepts a hashtable that contains the values to update in the body parameter.

   .PARAMETER setName 
   The table set name (EntityMetadata.EntitySetName property) of the record to update. For example, 'accounts' or 'contacts'.

   .PARAMETER id 
   The GUID primary key value of the record to update. For example, '00000000-0000-0000-0000-000000000000'.

   .PARAMETER body 
   A hashtable that contains the values to update in the record. For example, @{name = 'Contoso'; address1_city = 'Seattle'}.

   .EXAMPLE 
   Update-Record -setName 'accounts' -id '00000000-0000-0000-0000-000000000000' -body @{name = 'Contoso'; address1_city = 'Seattle'}

   This example updates the account record with the specified GUID and changes the name to 'Contoso' and the city to 'Seattle'. 

   .EXAMPLE
   $updateAccountData = @{
      name                = 'Contoso';
      accountcategorycode = 2; #Standard
   }
   Update-Record `
   -setName accounts `
   -id $newAccountID.Guid `
   -body $updateAccountData

   This example updates the account record with the specified GUID and changes the name to 'Contoso' and the accountcategorycode to 2 (Standard). 

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
      $body
   )
   $uri = $environmentUrl + 'api/data/v9.2/' + $setName
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
   Invoke-RestMethod @UpdateRequest
}

<# 

   .SYNOPSIS 
   Deletes a record from a table using the primary key.

   .DESCRIPTION 
   The Remove-Record function uses the Invoke-RestMethod cmdlet to send a DELETE request to the Dataverse Web API. 
   It deletes an existing record from the specified table using the GUID primary key value provided in the id parameter.

   .PARAMETER setName 
   The table set name (EntityMetadata.EntitySetName property) of the record to delete. For example, 'accounts' or 'contacts'.

   .PARAMETER id 
   The GUID primary key value of the record to delete. For example, '00000000-0000-0000-0000-000000000000'.

   .EXAMPLE 
   Remove-Record -setName 'accounts' -id '00000000-0000-0000-0000-000000000000'

   This example deletes the account record with the specified GUID. 

#>
function Remove-Record {
   param (
      [Parameter(Mandatory)] 
      [String]
      $setName,
      [Parameter(Mandatory)] 
      [Guid] 
      $id
   )
   $uri = $environmentUrl + 'api/data/v9.2/' + $setName
   $uri = $uri + '(' + $id.Guid + ')'

   $DeleteRequest = @{
      Uri     = $uri
      Method  = 'Delete'
      Headers = $baseHeaders
   }
   Invoke-RestMethod @DeleteRequest
}

<# 
   .SYNOPSIS 
   Gets the error details from a Dataverse Web API HTTP response exception.

   .DESCRIPTION 
   The Get-ErrorDetails function takes an exception object from the pipeline and extracts the status code, error code, and error message from it. 
   It assumes that the exception object is of type Microsoft.PowerShell.Commands.HttpResponseException and that the error details are in JSON format. 
   It returns a custom object with the status code, error code, and error message as properties.

   .PARAMETER Exception 
   The exception object from the pipeline. 
   It should be of type Microsoft.PowerShell.Commands.HttpResponseException and have the ErrorDetails property populated with a JSON string.

   .EXAMPLE 
   Connect -uri 'https://yourorg.crm.dynamics.com/'
   try {
      (Get-Records `
      -setName accounts `
      -query '?$select=names&$top=3').value | 
      Format-Table -Property name, accountid
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      Get-ErrorDetails | Format-List 
   } 

   Connected to https://yourorg.crm.dynamics.com/
   Token will expire in 30 minutes.

   statuscode : BadRequest
   code       : 0x0
   message    : Could not find a property named 'names' on type 'Microsoft.Dynamics.CRM.account'.
#>
function Get-ErrorDetails {
   try {
      $statuscode = $_.Exception.StatusCode
      $code = $null
      $message = $null
      if ((!$null -eq $_.ErrorDetails.Message) -and (Test-Json $_.ErrorDetails.Message) ) {
         $json = $_.ErrorDetails.Message | ConvertFrom-Json
         $code = $json.error.code
         $message = $json.error.message
      }
      return [PSCustomObject]@{
         statuscode = $statuscode
         code       = $code
         message    = $message
      }
   }
   catch {
      throw $_
   }
}


<#
.SYNOPSIS
Invokes a set of commands that may include Dataverse functions in this file.

.DESCRIPTION
The Invoke-DataverseCommands function takes a mandatory parameter $commands, 
which is a script block that contains one or more commands that may include the Dataverse functions in this file. 
The function uses the Invoke-Command cmdlet to execute the commands remotely on the Dataverse server. 
If the execution succeeds, the function returns the output of the commands. 
If the execution fails, the function handles the error and displays the error details.

.PARAMETER commands
A script block that contains one or more commands 
The commands can use the other functions defined in this file.

.EXAMPLE
Connect -uri 'https://yourorg.crm.dynamics.com/'

Invoke-DataverseCommands {

   Get-WhoAmI | Format-List
   # Retrieve Records
   (Get-Records `
      -setName connectors `
      -query '?$top=1').value

}
#>
function Invoke-DataverseCommands{
   param (
      [Parameter(Mandatory)] 
      $commands
   )

   try {

      Invoke-Command $commands

   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {

      Write-Host "An error occurred calling Dataverse:" -ForegroundColor Red
      Get-ErrorDetails | Format-List 

   }
   catch {

      Write-Host "An error occurred in the script:" -ForegroundColor Red
      Write-Host $_
   }
}