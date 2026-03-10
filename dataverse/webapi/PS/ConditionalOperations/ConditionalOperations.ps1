. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1

Connect 'https://yourorg.crm.dynamics.com/' # change this
$deleteCreatedRecords = $true # change this if you want to keep the records created by this sample

$recordsToDelete = [PsObject[]]@()

Invoke-DataverseCommands {

   Write-Host '--Starting Conditional Operations sample--'

   #region Section 0: Create sample records
   Write-Host "`n--Section 0 started--`n"

   $contosoAccount = @{
      'name'        = 'Contoso Ltd'
      'telephone1'  = '555-0000'
      'revenue'     = 5000000
      'description' = 'Parent company of Contoso Pharmaceuticals, etc.'
   }

   # Create and retrieve the account record using Add-Record
   $addRecordParams = @{
      setName = 'accounts'
      body    = $contosoAccount
      query   = '?$select=name,revenue,telephone1,description'
   }
   $retrievedAccount = Add-Record @addRecordParams

   $accountId = $retrievedAccount.accountid

   # To delete later
   $recordsToDelete += [PsObject]@{
      'description' = "Account '$($contosoAccount.name)'"
      'setName'     = 'accounts'
      'id'          = $accountId
   }

   # Store the initial ETag value
   $initialETagValue = $retrievedAccount.'@odata.etag'

   Write-Host "Created and retrieved the initial account, shown below:"
   Write-Host "`tName: $($retrievedAccount.name)"
   Write-Host "`tRevenue: $($retrievedAccount.'revenue@OData.Community.Display.V1.FormattedValue')"
   Write-Host "`tTelephone: $($retrievedAccount.telephone1)"
   Write-Host "`tDescription: $($retrievedAccount.description)"
   Write-Host "`tInitial ETag: $initialETagValue"

   #endregion Section 0: Create sample records

   #region Section 1: Conditional GET
   Write-Host "`n--Section 1 started--`n"
   Write-Host "** Conditional GET demonstration **"

   # Attempt to retrieve the account using conditional GET with current ETag
   Write-Host "`nAttempting to retrieve account using current ETag value: $initialETagValue"

   $syncRecordParams = @{
      record         = $retrievedAccount
      primaryKeyName = 'accountid'
   }
   $syncedAccount = Sync-Record @syncRecordParams

   if ($syncedAccount.'@odata.etag' -eq $initialETagValue) {
      Write-Host "Expected outcome: Entity was not modified so nothing was returned (HTTP 304)."
   }
   else {
      Write-Host "Instance retrieved with updated data."
   }

   # Modify the account by updating the telephone1 field
   $setColumnValueParams = @{
      setName  = 'accounts'
      id       = $accountId
      property = 'telephone1'
      value    = '555-0001'
   }
   Set-ColumnValue @setColumnValueParams | Out-Null

   Write-Host "`nModified account telephone number to 555-0001"

   # Re-attempt to retrieve using conditional GET with the original ETag
   Write-Host "Attempting to retrieve account using original ETag value: $initialETagValue"

   $syncRecordParams = @{
      record         = $retrievedAccount
      primaryKeyName = 'accountid'
   }
   $retrievedAccount = Sync-Record @syncRecordParams

   # Store the updated ETag value
   $updatedETagValue = $retrievedAccount.'@odata.etag'

   Write-Host "Modified account record retrieved using ETag: $initialETagValue"
   Write-Host "Notice the updated ETag value and telephone number:"
   Write-Host "`tName: $($retrievedAccount.name)"
   Write-Host "`tTelephone: $($retrievedAccount.telephone1)"
   Write-Host "`tOriginal ETag: $initialETagValue"
   Write-Host "`tUpdated ETag: $updatedETagValue"

   #endregion Section 1: Conditional GET

   #region Section 2: Optimistic concurrency on delete and update
   Write-Host "`n--Section 2 started--`n"
   Write-Host "** Optimistic concurrency demonstration **"

   # Attempt to delete the account using the original ETag value
   Write-Host "`nAttempting to delete the account using the original ETag value: $initialETagValue"

   try {
      $removeRecordParams = @{
         setName   = 'accounts'
         id        = $accountId
         eTagValue = $initialETagValue
      }
      Remove-Record @removeRecordParams

      # Not expected
      Write-Host "Account deleted"
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      $statuscode = [int]$_.Exception.StatusCode
      if ($statuscode -eq 412) {
         # Expected - Precondition Failed
         Write-Host "Expected Error: The version of the existing record doesn't match the property provided."
         Write-Host "`tAccount not deleted using ETag '$initialETagValue', status code: '412 (PreconditionFailed)'."
      }
      else {
         throw $_
      }
   }

   # Attempt to update the account using the original ETag value
   Write-Host "`nAttempting to update the account using the original ETag value: $initialETagValue"

   $accountUpdate = @{
      'telephone1' = '555-0002'
      'revenue'    = 6000000
   }

   try {
      $updateRecordParams = @{
         setName   = 'accounts'
         id        = $accountId
         body      = $accountUpdate
         eTagValue = $initialETagValue
      }
      Update-Record @updateRecordParams

      # Not expected
      Write-Host "Account updated using original ETag $initialETagValue"
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      $statuscode = [int]$_.Exception.StatusCode
      if ($statuscode -eq 412) {
         # Expected - Precondition Failed
         Write-Host "Expected Error: The version of the existing record doesn't match the property provided."
         Write-Host "`tAccount not updated using ETag '$initialETagValue', status code: '412 (PreconditionFailed)'."
      }
      else {
         throw $_
      }
   }

   # Reattempt update using the current ETag value
   $accountUpdate['telephone1'] = '555-0003'

   Write-Host "`nAttempting to update the account using the current ETag value: $updatedETagValue"

   try {
      $updateRecordParams = @{
         setName   = 'accounts'
         id        = $accountId
         body      = $accountUpdate
         eTagValue = $updatedETagValue
      }
      Update-Record @updateRecordParams

      # Expected
      Write-Host "`nAccount successfully updated using ETag: $updatedETagValue"
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      $statuscode = [int]$_.Exception.StatusCode
      if ($statuscode -eq 412) {
         # Not expected
         Write-Host "Unexpected status code: '412 (PreconditionFailed)'"
      }
      else {
         throw $_
      }
   }

   # Retrieve and output the current account state
   $getRecordParams = @{
      setName = 'accounts'
      id      = $accountId
      query   = '?$select=name,revenue,telephone1,description'
   }
   $finalAccount = Get-Record @getRecordParams

   Write-Host "`nBelow is the final state of the account:"
   Write-Host "`tName: $($finalAccount.name)"
   Write-Host "`tRevenue: $($finalAccount.'revenue@OData.Community.Display.V1.FormattedValue')"
   Write-Host "`tTelephone: $($finalAccount.telephone1)"
   Write-Host "`tDescription: $($finalAccount.description)"
   Write-Host "`tFinal ETag: $($finalAccount.'@odata.etag')"

   #endregion Section 2: Optimistic concurrency on delete and update

   #region Section 3: Delete sample records

   Write-Host "`n--Section 3 started--`n"

   if ($deleteCreatedRecords) {
      Write-Host '--Deleting sample records--'
      foreach ($record in $recordsToDelete) {
         Write-Host "`tDeleting $($record.description)"
         $removeRecordParams = @{
            setName = $record['setName']
            id      = $record['id']
         }
         Remove-Record @removeRecordParams | Out-Null
      }
   }

   #endregion Section 3: Delete sample records

   Write-Host "`n--Conditional Operations sample completed--"

}
