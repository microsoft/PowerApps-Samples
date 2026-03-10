. $PSScriptRoot\Examples.ps1
. $PSScriptRoot\Helpers.ps1
. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\CommonFunctions.ps1

function Setup {

   # Initializes user connections for both system admin and service principal users.
   Initialize-Users-Helper

   Write-Host "Setting up the sample..."

   $publisherId = New-Publisher-Helper
   New-Solution-Helper -publisherId $publisherId
   New-SampleTable-Helper   # Create example table
   # Create example columns
   $columnData = @(
      @{
         schemaName  = "${CUSTOMIZATION_PREFIX}_Email"
         displayName = 'Email'
         description = 'A sample column containing email addresses'
      },
      @{
         schemaName  = "${CUSTOMIZATION_PREFIX}_GovernmentId"
         displayName = 'Government ID'
         description = 'A sample column containing government ID values'
      },
      @{
         schemaName  = "${CUSTOMIZATION_PREFIX}_TelephoneNumber"
         displayName = 'Telephone Number'
         description = 'A sample column containing telephone numbers'
      },
      @{
         schemaName  = "${CUSTOMIZATION_PREFIX}_DateOfBirth"
         displayName = 'Date of Birth'
         description = 'A sample column containing dates of birth'
      }
   )

   foreach ($column in $columnData) {
      $params = @{
         schemaName  = $column.schemaName
         displayName = $column.displayName
         description = $column.description
      }
      New-SampleColumn-Helper @params
   }

   # Add data for the columns
   Manage-SampleData-Helper

   # Create a security role with privileges for the sample table
   $securityRoleId = New-SecurityRole-Helper

   # Add the role to the application user
   Manage-RoleForAppUser-Helper -roleId $securityRoleId

   # Create field security profile
   $fieldSecurityProfileParams = @{
      name               = $FIELD_SECURITY_PROFILE_NAME
      description        = 'A field security profile created for the column-level security sample.'
      solutionUniqueName = $SOLUTION_UNIQUE_NAME
   }

   $fspID = New-FieldSecurityProfile-Example @fieldSecurityProfileParams

   # To access and delete later
   $entityStore.$FIELD_SECURITY_PROFILE_NAME = $fspID
   Write-Host "`t☑ Created $FIELD_SECURITY_PROFILE_NAME."

   # Associate the app user with the field security profile
   $ManageFieldSecurityProfileForAppUserParams = @{
      fieldSecurityProfileId   = $fspID
      appUserId                = $secondUserId
      fieldSecurityProfileName = $FIELD_SECURITY_PROFILE_NAME
   }

   Manage-FieldSecurityProfileForAppUser-Helper @ManageFieldSecurityProfileForAppUserParams

   # Pause to let the cache catch up to the changes
   Wait-ForCacheUpdate-Helper
   # Waited 30 seconds for the cache to update....

}

# Setup is invoked before Run
function Run {

   #region Determine whether a column can be secured

   # Creates a CSV file with data about columns that are secured
   # or are limited in how they can be secured.  Any column not included can be secured.

   Write-Host "`nRunning the sample:"

   $parameters = @{
      filePath = "$PSScriptRoot\ExportedFiles\"
      fileName = "ColumnSecurityInfo.csv"
   }

   Dump-ColumnSecurityInfo-Example @parameters

   Write-Host "`t☑ Dumped column security information into this file:"
   Write-Host "`t\ExportedFiles\$($parameters.fileName)"

   #endregion Determine whether a column can be secured

   #region Discover which columns are already secured 

   try {

      # Because this portion is run by the system admin user, 
      # we can use the AdminOnly example function.
      # Otherwise use Get-SecuredColumnList-Example function.

      $values = Get-SecuredColumnList-AdminOnly-Example

      Write-Host "`n`r`tThese are the secured columns in this environment:"
      
      if ($values.Count -eq 0) { 
         Write-Host "`tNo columns are secured." -ForegroundColor Yellow
      }
      else {
         foreach ($value in $values) {
            Write-Host "`t-$value"
         }
      }
   }
   catch {
      Write-Host "`t☒ Error retrieving secured columns: $_" -ForegroundColor Red
   }

   #endregion Discover which columns are already secured 

   #region Secure a column

   Write-Host "`n`rBefore columns are secured, the application user can see this data:"

   #Switch to the second user
   Switch-User-Helper -user 'secondUser'
   #Show the data in the columns
   Write-ExampleRows-Helper

   # Name            Email                        Government ID Telephone Number Date of Birth
   # ----            -----                        ------------- ---------------- -------------
   # Jayden Phillips jayden@adatum.com            166-67-5353   (736) 555-9012   3/25/1974
   # Benjamin Stuart benjamin@adventure-works.com 211-16-7508   (195) 555-7901   6/18/1984
   # Avery Howard    avery@alpineskihouse.com     346-20-1720   (152) 555-5591   9/4/1994

   #Switch to the system admin user
   Switch-User-Helper -user 'systemAdmin'

   Write-Host "Securing columns..." -NoNewline

   $setColumnSecuredParams = @{
      tableLogicalName   = $TABLE_LOGICAL_NAME
      logicalName        = 'sample_email'
      type               = 'String'
      value              = $true
      solutionUniqueName = $SOLUTION_UNIQUE_NAME
   }
   Set-ColumnIsSecured-Example @setColumnSecuredParams

   Write-Host "☑ Email," -NoNewline

   $setColumnSecuredParams.logicalName = 'sample_governmentid'

   Set-ColumnIsSecured-Example @setColumnSecuredParams

   Write-Host "☑ Government ID," -NoNewline

   $setColumnSecuredParams.logicalName = 'sample_telephonenumber'

   Set-ColumnIsSecured-Example @setColumnSecuredParams

   Write-Host "☑ Telephone Number," -NoNewline
      
   $setColumnSecuredParams.logicalName = 'sample_dateofbirth'

   Set-ColumnIsSecured-Example @setColumnSecuredParams

   Write-Host " and ☑ Date of Birth."

   Write-Host "`nAfter columns are secured, the application user can see this data:"

   #Switch to the second user
   Switch-User-Helper -user 'secondUser'
   #show the data in the columns
   Write-ExampleRows-Helper

   # Name            Email Government ID Telephone Number Date of Birth
   # ----            ----- ------------- ---------------- -------------
   # Jayden Phillips
   # Benjamin Stuart
   # Avery Howard


   #endregion Secure a column

   #Switch to the systemAdmin user
   Switch-User-Helper -user 'systemAdmin'

   #region Manage read access to secured column

   # Retrieve the three records
   $records = Get-ExampleRows-Helper
   if ($records.Count -eq 0) {
      Write-Host "`t☒ No records found to grant access to." -ForegroundColor Red
      return
   }

   $jaydenPhillipsId = $records[0].$TABLE_PRIMARY_KEY
   $benjaminStuartId = $records[1].$TABLE_PRIMARY_KEY
   $averyHowardId = $records[2].$TABLE_PRIMARY_KEY


   $grantColumnAccessParams = @{
      recordId          = $jaydenPhillipsId
      columnLogicalName = 'sample_email'
      tableLogicalName  = $TABLE_LOGICAL_NAME
      principalId       = $secondUserId
      principalType     = 'systemuser'
      readAccess        = $true
      updateAccess      = $false
   }

   Grant-ColumnAccess-Example @grantColumnAccessParams

   $grantColumnAccessParams.recordId = $benjaminStuartId
   $grantColumnAccessParams.columnLogicalName = 'sample_governmentid'
   Grant-ColumnAccess-Example @grantColumnAccessParams

   $grantColumnAccessParams.recordId = $averyHowardId
   $grantColumnAccessParams.columnLogicalName = 'sample_telephonenumber'
   Grant-ColumnAccess-Example @grantColumnAccessParams

   #Switch to the second user
   Switch-User-Helper -user 'secondUser'

   Write-Host "`nAfter granting access to selected fields, the application user can see this data:"

   # show the data in the columns
   Write-ExampleRows-Helper

   # Name            Email             Government ID Telephone Number Date of Birth
   # ----            -----             ------------- ---------------- -------------
   # Jayden Phillips jayden@adatum.com
   # Benjamin Stuart                   211-16-7508
   # Avery Howard                                    (152) 555-5591

   

      
   #endregion Manage read access to secured column

   #region Manage write access to secured column

   Write-Host "`n`r`tDemonstrate error when attempting update without update access:"
   Write-Host "`tTry to update the Email column for the Jayden Phillips record:"

   $updateRecordParams = @{
      setName = $TABLE_SET_NAME
      id      = $jaydenPhillipsId
      body    = @{
         sample_email = 'jaydenp@adatum.com'
      }
   }

   try {
      Update-Record @updateRecordParams | Out-Null

      Write-Host "`t☒ Successfully updated record, but expected an error." -ForegroundColor Red
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      Write-Host "`t ☑ Expected error: $_" -ForegroundColor Yellow

      # {
      #   "error": {
      #     "code": "0x8004f507",
      #     "message": "Caller user with Id <secondUserId> does not have update permissions to a Email secured field on entity Example table. The requested operation could not be completed."
      #   }
      # }

   }
   catch {
      Write-Host "`t☒ Unexpected Error updating record: $_" -ForegroundColor Red
   }

   Write-Host "`n`r`tDemonstrate success when attempting update with update access:"
   Write-Host "`tGrant write access to the email column for Jayden Phillips record:"

   #switch back to the systemAdmin user to grant write access
   Switch-User-Helper -user 'systemAdmin'
   
   $modifyColumnAccessParams = @{
      recordId          = $jaydenPhillipsId
      columnLogicalName = 'sample_email'
      tableLogicalName  = $TABLE_LOGICAL_NAME
      principalId       = $secondUserId
      principalType     = 'systemuser'
      readAccess        = $true
      updateAccess      = $true
   }

   Modify-ColumnAccess-Example @modifyColumnAccessParams

   Write-Host "`tTry to update the Email column for the Jayden Phillips record again:"

   #switch back to the secondUser user
   Switch-User-Helper -user 'secondUser'
   
   try {
      Update-Record @updateRecordParams | Out-Null
      Write-Host "`t ☑ Successfully updated record."
   }
   catch {
      Write-Host "`t☒ Unexpected Error updating record: $_" -ForegroundColor Red
   }

   #endregion Manage write access to secured column

   #region Remove access to fields
   Write-Host "`n`r`tRevoking access to selected fields..."
   
   #switch back to the systemAdmin user
   Switch-User-Helper -user 'systemAdmin'

   $revokeColumnAccessParams = @{
      recordId          = $jaydenPhillipsId
      columnLogicalName = 'sample_email'
      tableLogicalName  = $TABLE_LOGICAL_NAME
      principalId       = $secondUserId
      principalType     = 'systemuser'
      readAccess        = $true
      updateAccess      = $true
   }

   Revoke-ColumnAccess-Example @revokeColumnAccessParams | Out-Null

   $revokeColumnAccessParams.recordId = $benjaminStuartId
   $revokeColumnAccessParams.columnLogicalName = 'sample_governmentid'

   Revoke-ColumnAccess-Example @revokeColumnAccessParams | Out-Null

   $revokeColumnAccessParams.recordId = $averyHowardId
   $revokeColumnAccessParams.columnLogicalName = 'sample_telephonenumber'

   Revoke-ColumnAccess-Example @revokeColumnAccessParams | Out-Null

   Write-Host "`tAfter access to selected fields is revoked, the application user can't see any data."


   #endregion Remove access to fields

   #region Provide access to specific groups

   Write-Host "`n`r`tThe $FIELD_SECURITY_PROFILE_NAME was created during Setup and the application user was associated with it."
   Write-Host "`n`r`tAdd field permissions to the $FIELD_SECURITY_PROFILE_NAME .."

   # Field security profile was created and associated with the App User in Setup.
   $fspID = $entityStore.$FIELD_SECURITY_PROFILE_NAME
   if (-not $fspID) {
      throw "Field security profile ID not found in entity store."
   }

   # Define related field permissions
   # You can't set canreadunmasked column values until you create
   # attributemaskingrule records for each column. This will be demonstrated later.

   $fieldPermissions = @(
      @{
         attributelogicalname = 'sample_email'
         canRead              = 4 # Allowed
         canUpdate            = 4 # Allowed
      },
      @{
         attributelogicalname = 'sample_governmentid'
         canRead              = 0 # Not allowed
         canUpdate            = 0 # Not allowed
      },
      @{
         attributelogicalname = 'sample_telephonenumber'
         canRead              = 4 # Allowed
         canUpdate            = 4 # Allowed
      },
      @{
         attributelogicalname = 'sample_dateofbirth'
         canRead              = 4 # Allowed
         canUpdate            = 0 # Not allowed
      }
   )

   # We need these later when configuring masking
   $fieldPermissionIds = @{}

   $fieldPermissions.ForEach({

         $newFieldPermissionParams = @{
            setName            = 'fieldpermissions'
            body               = @{
               'fieldsecurityprofileid@odata.bind' = "fieldsecurityprofiles($fspID)"
               attributelogicalname                = $_.attributelogicalname
               cancreate                           = 4 # Allowed
               canread                             = $_.canRead
               canreadunmasked                     = 0 # Not allowed, will be set later
               canupdate                           = $_.canUpdate
               entityname                          = $TABLE_LOGICAL_NAME
            }
            solutionUniqueName = $SOLUTION_UNIQUE_NAME
            strongConsistency  = $true
         }

         # Create the field permission
         $fieldPermissionId = New-Record @newFieldPermissionParams

         # Store the ID for later use
         $fieldPermissionIds[$_.attributelogicalname] = $fieldPermissionId

      })
   
   Write-Host "`n`r`tNew field permissions:"

   Write-Host "`tColumn           Can Read    Can Update"
   Write-Host "`t---------------- ----------- ----------"
   Write-Host "`tEmail            Allowed     Allowed"
   Write-Host "`tGovernment ID    Not Allowed Not Allowed"
   Write-Host "`tTelephone Number Allowed     Allowed"
   Write-Host "`tDate of Birth    Allowed     Not Allowed"


   Wait-ForCacheUpdate-Helper
   # Waited 30 seconds for the cache to update....


   #endregion Provide access to specific groups

   #region Show change due to field security profile

   #region Show Read access limited

   #switch back to the secondUser user
   Switch-User-Helper -user 'secondUser'

   Write-Host "`n`rThe Government ID column now appears null for all rows."

   Write-ExampleRows-Helper

   # Name            Email                        Government ID Telephone Number Date of Birth
   # ----            -----                        ------------- ---------------- -------------
   # Jayden Phillips jaydenp@adatum.com                         (736) 555-9012   3/25/1974
   # Benjamin Stuart benjamin@adventure-works.com               (195) 555-7901   6/18/1984
   # Avery Howard    avery@alpineskihouse.com                   (152) 555-5591   9/4/1994

   #endregion Show Read access limited

   #region Show Write access limited

   Write-Host "`n`r`tAttempt to update Date of Birth column data without access:"

   $updateRecordParams = @{
      setName = $TABLE_SET_NAME
      id      = $jaydenPhillipsId
      body    = @{
         sample_dateofbirth = '1/1/2000'
      }
   }

   try {
      Update-Record @updateRecordParams | Out-Null
   }
   catch [Microsoft.PowerShell.Commands.HttpResponseException] {
      Write-Host "`t ☑ Expected error: $_" -ForegroundColor Yellow

      # {
      #   "error": {
      #     "code": "0x8004f507",
      #     "message": "Caller user with Id <secondUserID> does not have update permissions to a Date of Birth secured field on entity Example table. The requested operation could not be completed."
      #   }
      # }

   }
   catch {
      Write-Host "`t☒ Unexpected Error updating record: $_" -ForegroundColor Red
   }
   

   #endregion Show Write access limited

   #endregion Show change due to field security profile

   #region Masking

   #switch back to the systemAdmin user
   Switch-User-Helper -user 'systemAdmin'

   Write-Host "`n`r`tDemonstrate how to enable masking"

   #region Create attribute masking rules

   $attributeMaskingRuleList = @()

   # Retrieve ID values for existing masking rules

   # Email_HideName
   $getRecordIdParams = @{
      entitySetName         = 'maskingrules'
      columnLogicalName     = 'name'
      uniqueStringValue     = 'Email_HideName'
      primaryKeyLogicalName = 'maskingruleid'
   }

   $email_HideNameMaskingRuleId = Get-RecordId-Helper @getRecordIdParams

   if (-not $email_HideNameMaskingRuleId) {
      throw "Masking rule 'Email_HideName' not found."
   }
   else {

      $attributeMaskingRuleList += @{
         attributelogicalname       = 'sample_email'
         entityname                 = $TABLE_LOGICAL_NAME         
         uniquename                 = 'sample_example_sample_email'
         'MaskingRuleId@odata.bind' = "maskingrules($email_HideNameMaskingRuleId)"
      }
   }

   # SocialSecurityNumber_ShowLastFourDigits
   $getRecordIdParams = @{
      entitySetName         = 'maskingrules'
      columnLogicalName     = 'name'
      uniqueStringValue     = 'SocialSecurityNumber_ShowLastFourDigits'
      primaryKeyLogicalName = 'maskingruleid'
   }

   $SSN_ShowLastFourDigitsRuleId = Get-RecordId-Helper @getRecordIdParams

   if (-not $SSN_ShowLastFourDigitsRuleId) {
      throw "Masking rule 'SocialSecurityNumber_ShowLastFourDigits' not found."
   }
   else {

      $attributeMaskingRuleList += @{
         attributelogicalname       = 'sample_governmentid'
         entityname                 = $TABLE_LOGICAL_NAME
         uniquename                 = 'sample_example_sample_governmentid'
         'MaskingRuleId@odata.bind' = "maskingrules($SSN_ShowLastFourDigitsRuleId)"
      }
   }

   # Date_Slash
   $getRecordIdParams = @{
      entitySetName         = 'maskingrules'
      columnLogicalName     = 'name'
      uniqueStringValue     = 'Date_Slash'
      primaryKeyLogicalName = 'maskingruleid'
   }

   $date_SlashID = Get-RecordId-Helper @getRecordIdParams

   if (-not $date_SlashID) {
      throw "Masking rule 'Date_Slash' not found."
   }
   else {

      $attributeMaskingRuleList += @{
         attributelogicalname       = 'sample_dateofbirth'
         entityname                 = $TABLE_LOGICAL_NAME
         uniquename                 = 'sample_example_sample_dateofbirth'
         'MaskingRuleId@odata.bind' = "maskingrules($date_SlashID)"
      }
   }

   $attributeMaskingRuleList | ForEach-Object {

      $newAttributeMaskingRuleParams = @{
         setName            = 'attributemaskingrules'
         body               = $_
         solutionUniqueName = $SOLUTION_UNIQUE_NAME
         strongConsistency  = $true
      }

      # Create the attribute masking rule

      try {
         New-Record @newAttributeMaskingRuleParams | Out-Null
      }
      catch {
         throw "Error creating attribute masking rule for $($_.attributelogicalname): $_"
      }
   }



   #endregion Create attribute masking rules

   #region Update fieldpermissions canreadunmasked values


   # These fieldpermission IDs values were cached when they were created earlier

   $emailfieldPermissionId = $fieldPermissionIds['sample_email']
   $governmentidfieldPermissionId = $fieldPermissionIds['sample_governmentid']
   $dateofbirthfieldPermissionId = $fieldPermissionIds['sample_dateofbirth']

   # Although these records were created with canread values
   # When canreadunmasked is set, you must also include a canread value!

   $fieldPermissionUpdateList = @(
      @{
         fieldpermissionid = $emailfieldPermissionId
         canreadunmasked   = 3 # All records
         canread           = 4 # Allowed
      },
      @{
         fieldpermissionid = $governmentidfieldPermissionId
         canreadunmasked   = 1 # One record
         canread           = 4 # Allowed
      },
      @{
         fieldpermissionid = $dateofbirthfieldPermissionId
         canreadunmasked   = 3 # All records
         canread           = 4 # Allowed
      }
   )

   $fieldPermissionUpdateList.ForEach({
         # Slowing this down to avoid errors
         # Pause to let the cache catch up to the changes
         Start-Sleep -Seconds 5

         $updateFieldPermissionParams = @{
            setName            = 'fieldpermissions'
            id                 = $_.fieldpermissionid
            body               = @{
               canreadunmasked = $_.canreadunmasked
               canread         = $_.canread
            }
            solutionUniqueName = $SOLUTION_UNIQUE_NAME
            strongConsistency  = $true
         }

         try {
            Update-Record @updateFieldPermissionParams | Out-Null
         }
         catch {
            throw "Error updating field permission for $($_.fieldpermissionid): $_"
         }
      })
   
   Write-Host "`n`r`tThis table shows the fieldpermission changes made to enable displaying masked data.`n`r"

   Write-Host "`tColumn           Can Read    Can Read Unmasked Can Update"
   Write-Host "`t---------------- ----------- ----------------- ----------"
   Write-Host "`tEmail            Allowed     All records       Allowed"
   Write-Host "`tGovernment ID    Not Allowed One record        Not Allowed"
   Write-Host "`tTelephone Number Allowed     Not Allowed       Allowed"
   Write-Host "`tDate of Birth    Allowed     All records       Not Allowed"


   #endregion Update fieldpermissions canreadunmasked 
   
   Write-Host "`n`rNow masked values are returned:"

   #switch back to the secondUser user
   Switch-User-Helper -user 'secondUser'

   # show the data in the columns
   Write-ExampleRows-Helper

   # Name            Email                        Government ID Telephone Number Date of Birth
   # ----            -----                        ------------- ---------------- -------------
   # Jayden Phillips *******@******.com           ***-**-5353   (736) 555-9012   3/25/****
   # Benjamin Stuart ********@***************.com ***-**-7508   (195) 555-7901   6/18/****
   # Avery Howard    *****@**************.com     ***-**-1720   (152) 555-5591   9/4/****

   #endregion Masking

   #region Show unmasked values

   Write-Host "`nThe unmasked values for Email and Date of Birth can be retrieved for all records:"

   # Pause to let the cache catch up to the changes
   Wait-ForCacheUpdate-Helper

   # Show the unmasked data in the columns
   Write-ExampleRows-Unmasked-Helper

   Write-Host "The unmasked values for Government ID can only be retrieved individually:`n"

   $recordIDs = @($jaydenPhillipsId, $benjaminStuartId, $averyHowardId)

   $UnMaskedDataQuery = '?$select=sample_name,sample_governmentid&UnMaskedData=true'

   foreach ($recordId in $recordIDs) {
      $getRecordParams = @{
         setName = $TABLE_SET_NAME
         id      = $recordId
         query   = $UnMaskedDataQuery
      }
      $record = Get-Record @getRecordParams

      Write-Host "`t-Name: $($record.sample_name) Government ID: $($record.sample_governmentid)"

      #   -Name: Jayden Phillips Government ID: 166-67-5353
      #   -Name: Benjamin Stuart Government ID: 211-16-7508
      #   -Name: Avery Howard Government ID: 346-20-1720
   }
   
   #endregion Show unmasked values

   #region Export solution

   ## Change to System administrator user to export the solution
   Switch-User-Helper -user 'systemAdmin'

   #region Export unmanaged

   Write-Host "`n`r`tExporting unmanaged solution..."

   $exportSolutionParams = @{
      solutionName = $SOLUTION_UNIQUE_NAME
      managed      = $false
   }

   $unmanagedSolutionFile = Export-Solution @exportSolutionParams

   # Save the solution file to the current directory
   $saveSolutionFilePath = ".\ExportedFiles\$($SOLUTION_UNIQUE_NAME).zip"

   [IO.File]::WriteAllBytes($saveSolutionFilePath, $unmanagedSolutionFile)

   Write-Host "`t☑ Unmanaged solution exported to $saveSolutionFilePath"

   #endregion Export unmanaged

   #region Export managed 

   #endregion Export managed 

   Write-Host "`n`r`tExporting managed solution..."

   $exportSolutionParams.managed = $true

   $managedSolutionFile = Export-Solution @exportSolutionParams

   # Save the solution file to the current directory
   $saveSolutionFilePath = ".\ExportedFiles\$($SOLUTION_UNIQUE_NAME)_managed.zip"

   [IO.File]::WriteAllBytes($saveSolutionFilePath, $managedSolutionFile)

   Write-Host "`t☑ Managed solution exported to $saveSolutionFilePath"

   #endregion Export solution
}
function Cleanup {

   if ($DELETE_CREATED_OBJECTS) {
      Write-Host "`n`rCleaning up the sample..."


      # Remove the field security profile
      $fspID = $entityStore.$FIELD_SECURITY_PROFILE_NAME
      if ($fspID) {
         Remove-Record -setName 'fieldsecurityprofiles' -id $fspID
         Write-Host "`t☑ Removed field security profile with ID $fspID."
      }
      else {
         Write-Host "`t☑ Field security profile with ID $fspID not found." -ForegroundColor Yellow
      }

      # Remove the security role
      $roleID = $entityStore.$ROLE_NAME
      if ($roleID) { 
         Remove-Record -setName 'roles' -id $roleID
         Write-Host "`t☑ Removed security role with ID $roleID."
      }
      else {
         Write-Host "`t☑ Security role with ID $roleID not found." -ForegroundColor Yellow
      }

      # Remove the sample table

      Write-Host "`tRemoving the $TABLE_LOGICAL_NAME table..."
      Remove-Table -tableLogicalName $TABLE_LOGICAL_NAME
      Write-Host "`t☑ Removed the $TABLE_LOGICAL_NAME table."

      # Delete the solution

      $solutionID = $entityStore.$SOLUTION_UNIQUE_NAME
      if ($solutionID) {

         Remove-Record -setName 'solutions' -id $solutionID
         Write-Host "`t☑ Removed solution with ID $solutionID."
      }
      else {
         Write-Host "`t☑ Solution with ID $solutionID not found." -ForegroundColor Yellow
      }


      # Delete the publisher
      $publisherID = $entityStore.$PUBLISHER_UNIQUE_NAME
      if ($publisherID) {

         Remove-Record -setName 'publishers' -id $publisherID
         Write-Host "`t☑ Removed publisher with ID $publisherID."
      }
      else {
         Write-Host "`t☑ Publisher with ID $publisherID not found." -ForegroundColor Yellow
      }

   }
   else {
      Write-Host "Cleanup skipped. Created objects will remain in the environment." -ForegroundColor Yellow
   }


}

# Call the functions in order to run the sample
Write-Host "Column-level security sample started..." 
Setup
Run
Cleanup
Write-Host "`nColumn-level security sample completed." 