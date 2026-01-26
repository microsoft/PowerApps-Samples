. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1
. $PSScriptRoot\..\MetadataOperations.ps1

Connect 'https://yourorg.crm.dynamics.com/' # change this
$deleteCreatedRecords = $true # Set to false to keep sample records after execution

$recordsToDelete = [PsObject[]]@()

Invoke-DataverseCommands {

   Write-Host '--Starting Functions and Actions sample--'

   try {

      #region Setup: Create sample records

      Write-Host "`nCreating sample records..."

      # Create original account record
      $originalAccount = @{
         accountcategorycode         = 1 # Preferred Customer
         address1_addresstypecode    = 3 # Primary
         address1_city               = 'Redmond'
         address1_country            = 'USA'
         address1_line1              = '123 Maple St.'
         address1_name               = 'Corporate Headquarters'
         address1_postalcode         = '98000'
         address1_shippingmethodcode = 4 # UPS
         address1_stateorprovince    = 'WA'
         address1_telephone1         = '555-1234'
         customertypecode            = 3 # Customer
         description                 = 'Contoso is a business consulting company.'
         emailaddress1               = 'info@contoso.com'
         industrycode                = 7 # Consulting
         name                        = 'Contoso Consulting'
         numberofemployees           = 150
         ownershipcode               = 2 # Private
         preferredcontactmethodcode  = 2 # Email
         telephone1                  = '(425) 555-1234'
      }

      $contosoAccountId = New-Record -setName 'accounts' -body $originalAccount
      $recordsToDelete += [PsObject]@{
         'description' = "Account 'Contoso Consulting'"
         'setName'     = 'accounts'
         'id'          = $contosoAccountId
      }

      Write-Host "Created account 'Contoso Consulting' with ID: $contosoAccountId"

      # Create account to share
      $accountToShare = @{
         name = 'Account to Share'
      }

      $accountToShareId = New-Record -setName 'accounts' -body $accountToShare
      $recordsToDelete += [PsObject]@{
         'description' = "Account 'Account to Share'"
         'setName'     = 'accounts'
         'id'          = $accountToShareId
      }

      Write-Host "Created account 'Account to Share' with ID: $accountToShareId"

      #endregion Setup: Create sample records

      #region Section 1: Unbound Function WhoAmI

      Write-Host "`n-- Section 1: Unbound Function WhoAmI --"

      $whoAmI = Get-WhoAmI

      Write-Host "`nWhoAmI function returns the current user's information:"
      Write-Host "`tBusinessUnitId: $($whoAmI.BusinessUnitId)"
      Write-Host "`tUserId: $($whoAmI.UserId)"
      Write-Host "`tOrganizationId: $($whoAmI.OrganizationId)"

      #endregion Section 1: Unbound Function WhoAmI

      #region Section 2: Unbound Function FormatAddress

      Write-Host "`n-- Section 2: Unbound Function FormatAddress --"

      $message = "`nFormatAddress builds the full address according to" +
      " country/regional format specific requirements."   

      Write-Host $message

      # Format US address
      Write-Host "`nUSA Formatted Address:"
      $usAddressParams = @{
         Line1           = '123 Maple St.'
         City            = 'Seattle'
         StateOrProvince = 'WA'
         PostalCode      = '98007'
         Country         = 'USA'
      }
      $usAddress = Format-Address @usAddressParams

      Write-Host $usAddress.Address

      # Format Japan address
      Write-Host "`nJAPAN Formatted Address:"
      $japanAddressParams = @{
         Line1           = '1-2-3 Sakura'
         City            = 'Nagoya'
         StateOrProvince = 'Aichi'
         PostalCode      = '455-2345'
         Country         = 'JAPAN'
      }
      $japanAddress = Format-Address @japanAddressParams

      Write-Host $japanAddress.Address

      #endregion Section 2: Unbound Function FormatAddress

      #region Section 3: Unbound Function InitializeFrom

      Write-Host "`n-- Section 3: Unbound Function InitializeFrom --"

      $message = "`nInitializeFrom returns an entity with default values" + 
      " set based on mapping configuration."

      Write-Host $message

      $initializeParams = @{
         SourceSetName    = 'accounts'
         SourceId         = $contosoAccountId
         TargetEntityName = 'account'
      }
      $initializedAccount = Initialize-From @initializeParams

      Write-Host "`nNew data based on original record:"
      # Display properties returned readability

      foreach ($property in $initializedAccount.PSObject.Properties) {

         if (-not $property.Name.StartsWith('@')) {
            Write-Host "`t$($property.Name) : $($property.Value)"
         }         
      }


      $propertiesToShow = @('name', 'parentaccountid@odata.bind')
      foreach ($prop in $propertiesToShow) {
         if ($initializedAccount.PSObject.Properties[$prop]) {
            Write-Host "`t$prop : $($initializedAccount.$prop)"
         }
      }

      # Create a new account using the initialized values
      Write-Host "`nCreating new account based on initialized data..."

      # Convert to hashtable for New-Record
      $newAccountBody = @{}
      foreach ($property in $initializedAccount.PSObject.Properties) {
         if (-not $property.Name.StartsWith('@')) {
            $newAccountBody[$property.Name] = $property.Value
         }
      }

      # Modify the initialized account with new values
      $newAccountBody.name = 'Contoso Consulting Chicago Branch'
      $newAccountBody.address1_city = 'Chicago'
      $newAccountBody.address1_line1 = '456 Elm St.'
      $newAccountBody.address1_name = 'Chicago Branch Office'
      $newAccountBody.address1_postalcode = '60007'
      $newAccountBody.address1_stateorprovince = 'IL'
      $newAccountBody.address1_telephone1 = '(312) 555-3456'
      $newAccountBody.numberofemployees = 12

      $newAccountId = New-Record -setName 'accounts' -body $newAccountBody
      $recordsToDelete += [PsObject]@{
         'description' = "Account 'Contoso Consulting Chicago Branch'"
         'setName'     = 'accounts'
         'id'          = $newAccountId
      }

      Write-Host "Created new account: 'Contoso Consulting Chicago Branch' with ID: $newAccountId"

      #endregion Section 3: Unbound Function InitializeFrom

      #region Section 4: Unbound Function RetrieveCurrentOrganization

      Write-Host "`n-- Section 4: Unbound Function RetrieveCurrentOrganization --"

      $orgInfo = Get-CurrentOrganization

      $message = "`nRetrieveCurrentOrganization function returns the" +
      " current organization information:"

      Write-Host $message
      Write-Host "`tOrganizationId: $($orgInfo.Detail.OrganizationId)"
      Write-Host "`tFriendlyName: $($orgInfo.Detail.FriendlyName)"
      Write-Host "`tOrganizationVersion: $($orgInfo.Detail.OrganizationVersion)"
      Write-Host "`tEnvironmentId: $($orgInfo.Detail.EnvironmentId)"
      Write-Host "`tUrlName: $($orgInfo.Detail.UrlName)"
      Write-Host "`tUniqueName: $($orgInfo.Detail.UniqueName)"

      #endregion Section 4: Unbound Function RetrieveCurrentOrganization

      #region Section 5: Unbound Function RetrieveTotalRecordCount

      Write-Host "`n-- Section 5: Unbound Function RetrieveTotalRecordCount --"

      $recordCounts = Get-TotalRecordCount -EntityNames @('account', 'contact')

      $message = "`nThe number of records for each table according to" +
      " RetrieveTotalRecordCount:"
      Write-Host $message

      $keys = $recordCounts.EntityRecordCountCollection.Keys
      $values = $recordCounts.EntityRecordCountCollection.Values

      for ($i = 0; $i -lt $keys.Count; $i++) {
         Write-Host "`t$($keys[$i]): $($values[$i])"
      }

      #endregion Section 5: Unbound Function RetrieveTotalRecordCount

      #region Section 6: Bound Function IsSystemAdmin

      Write-Host "`n-- Section 6: Bound Function IsSystemAdmin --"

      $message = "`nThe sample_IsSystemAdmin function is a custom API that" +
      " checks if the user has the system administrator role."
      Write-Host $message
      $message = "This function is bound to the system user table and" +
      " returns a boolean value."
      Write-Host $message
      $message = "NOTE: This function requires the IsSystemAdminFunction" +
      " solution to be installed."
      Write-Host $message
      $message = "If not installed, this section will attempt to install it."
      Write-Host $message

      # Check if the IsSystemAdminFunction solution is installed
      $solutionCheckParams = @{
         setName = 'solutions'
         query   = "?`$select=solutionid&`$filter=uniquename eq 'IsSystemAdminFunction'"
      }
      $solutionCheck = Get-Records @solutionCheckParams

      if ($solutionCheck.value.Count -eq 0) {
         
         Write-Host "`nIsSystemAdminFunction solution is not installed."
         Write-Host "Attempting to import solution..."

         $zipUrl = "https://github.com/microsoft/PowerApps-Samples/" +
         "blob/master/dataverse/orgsvc/CSharp/" +
         "IsSystemAdminCustomAPI/IsSystemAdminFunction_1_0_0_0_managed.zip"

         try {
            # Load base64-encoded managed solution content from local file
            . (Join-Path $PSScriptRoot 'IsSystemAdminFunction_1_0_0_0_managed.ps1')

            if (-not $customizationFile) {
               throw "Customization file base64 data not found in " +
               "IsSystemAdminFunction_1_0_0_0_managed.ps1."
            }

            # Convert base64 string to byte array
            $solutionBytes = [System.Convert]::FromBase64String($customizationFile)

            # Create import job id
            $importJobId = [guid]::NewGuid()

            Write-Host "Importing IsSystemAdminFunction managed solution..."
            $importParams = @{
               customizationFile                = $solutionBytes
               overwriteUnmanagedCustomizations = $false
               importJobId                      = $importJobId
            }
            Import-Solution @importParams

            Write-Host "Managed solution import requested. Verifying installation..."

            # Re-check for the solution by unique name
            $postImportCheck = Get-Records `
               -setName 'solutions' `
               -query "?`$select=solutionid&`$filter=uniquename eq 'IsSystemAdminFunction'"

            if ($postImportCheck.value.Count -eq 0) {
               Write-Host "Solution import did not confirm installation yet." `
                  -ForegroundColor Yellow
               Write-Host "Skipping this section for now." -ForegroundColor Yellow
               Write-Host "If needed, manually install from:" -ForegroundColor Yellow
               
               Write-Host $zipUrl -ForegroundColor Yellow
               # Skip section if not present
            }
            else {
               Write-Host "IsSystemAdminFunction solution installed."
               Write-Host "Proceeding with tests..."

               # Get top 10 enabled interactive users
               $userQuery = "?`$select=systemuserid,fullname&" +
               "`$filter=not startswith(fullname,'%23') and " +
               "accessmode eq 0 and isdisabled eq false&`$top=10"
               $users = Get-Records -setName 'systemusers' -query $userQuery

               $message = "`nTop 10 users and whether they have" +
               " System Administrator role:"
               Write-Host $message

               foreach ($user in $users.value) {
                  try {
                     $result = Test-SystemAdministrator `
                        -SystemUserId $user.systemuserid
                     $hasRole = if ($result.HasRole) { 'HAS' } `
                        else { 'does not have' }

                     $message = "`t$($user.fullname) $hasRole the " +
                     "System Administrator role."
                     Write-Host $message
                  }
                  catch {
                     Write-Host "`tFailed to check $($user.fullname): $_" `
                        -ForegroundColor Yellow
                  }
               }
            }
         }
         catch {
            Write-Host "Failed to import IsSystemAdminFunction solution: $_" `
               -ForegroundColor Red
            Write-Host "You can install it manually from:" -ForegroundColor Yellow
            Write-Host $zipUrl -ForegroundColor Yellow
         }
      }
      else {
         Write-Host "`nIsSystemAdminFunction solution is installed."
         Write-Host "Testing users..."

         # Get top 10 enabled interactive users
         $userQuery = "?`$select=systemuserid,fullname&" +
         "`$filter=not startswith(fullname,'%23') and " +
         "accessmode eq 0 and isdisabled eq false&`$top=10"
         $users = Get-Records -setName 'systemusers' -query $userQuery

         $message = "`nTop 10 users and whether they have" +
         " System Administrator role:" 
         Write-Host $message

         foreach ($user in $users.value) {
            try {
               $result = Test-SystemAdministrator `
                  -SystemUserId $user.systemuserid
               $hasRole = if ($result.HasRole) { 'HAS' } `
                  else { 'does not have' }

               $message = "`t$($user.fullname) $hasRole the " +
               "System Administrator role."
               Write-Host $message
            }
            catch {
               Write-Host "`tFailed to check $($user.fullname): $_" `
                  -ForegroundColor Yellow
            }
         }
      }

      #endregion Section 6: Bound Function IsSystemAdmin

      #region Section 7: Unbound Action GrantAccess

      Write-Host "`n-- Section 7: Unbound Action GrantAccess --"

      Write-Host "`nGrantAccess is an action used to share a record with another user."

      # Get an enabled user other than current user
      $otherUserQuery = "?`$select=systemuserid,fullname&" +
      "`$filter=systemuserid ne $($whoAmI.UserId) and " +
      "isdisabled eq false and accessmode eq 0 and " +
      "not startswith(fullname,'%23')&`$top=1"
      $otherUsers = Get-Records -setName 'systemusers' -query $otherUserQuery

      if ($otherUsers.value.Count -eq 0) {
         $message = "`nNo other enabled interactive users found in the" +
         " system. Cannot demonstrate the GrantAccess action."

         Write-Host $message
      }
      else {
         $otherUser = $otherUsers.value[0]
         Write-Host "`nTesting user: $($otherUser.fullname)"

         # Check current access rights
         $principalAccessParams = @{
            SystemUserId  = $otherUser.systemuserid
            TargetSetName = 'accounts'
            TargetId      = $accountToShareId
         }
         $currentAccess = Get-PrincipalAccess @principalAccessParams

         $message = "`n$($otherUser.fullname) has the following access" +
         " rights to the account record: $($currentAccess.AccessRights)"

         Write-Host $message

         # Check if user has DeleteAccess
         $hasDeleteAccess = $currentAccess.AccessRights -like '*DeleteAccess*'

         if ($hasDeleteAccess) {

            $message = "`n$($otherUser.fullname) already has DeleteAccess" +
            " rights to the account record."

            Write-Host $message
         }
         else {
            $message = "`n$($otherUser.fullname) does not have DeleteAccess" +
            " rights to the account record."
            Write-Host $message
            Write-Host "`nGranting DeleteAccess rights..."

            # Grant DeleteAccess
            $grantAccessParams = @{
               TargetSetName       = 'accounts'
               TargetId            = $accountToShareId
               TargetEntityName    = 'account'
               TargetPrimaryKey    = 'accountid'
               PrincipalSetName    = 'systemusers'
               PrincipalId         = $otherUser.systemuserid
               PrincipalEntityName = 'systemuser'
               PrincipalPrimaryKey = 'systemuserid'
               AccessMask          = 'DeleteAccess'
            }
            Grant-Access @grantAccessParams

            $message = "Granted DeleteAccess rights to $($otherUser.fullname)" +
            " for the account record."

            Write-Host $message

            # Verify the access was granted
            $updatedAccess = Get-PrincipalAccess @principalAccessParams

            if ($updatedAccess.AccessRights -like '*DeleteAccess*') {
               $message = "$($otherUser.fullname) DeleteAccess rights to the" +
               " account record is confirmed."

               Write-Host $message
            }
            else {
               $message = "$($otherUser.fullname) still does not have " +
               "DeleteAccess rights to the account record."
               Write-Host $message -ForegroundColor Yellow
            }
         }
      }

      #endregion Section 7: Unbound Action GrantAccess

      #region Section 8: Bound Action AddPrivilegesRole

      Write-Host "`n-- Section 8: Bound Action AddPrivilegesRole --"

      Write-Host "`nAddPrivilegesRole adds a set of existing privileges to an existing role."

      # Create a security role
      $roleBody = @{
         'businessunitid@odata.bind' = "businessunits($($whoAmI.BusinessUnitId))"
         name                        = 'Test Role'
      }

      $roleId = New-Record -setName 'roles' -body $roleBody
      $recordsToDelete += [PsObject]@{
         'description' = "Role 'Test Role'"
         'setName'     = 'roles'
         'id'          = $roleId
      }

      Write-Host "`nCreated a security role named 'Test Role'."

      # Get current privileges
      $rolePrivileges = Get-Records `
         -setName "roles($roleId)/roleprivileges_association" `
         -query '?$select=name'

      Write-Host "`nNumber of privileges in new role: $($rolePrivileges.value.Count)"
      foreach ($privilege in $rolePrivileges.value) {
         Write-Host "`t$($privilege.name)"
      }

      # Get the prvCreateAccount and prvReadAccount privileges
      $privQuery = "?`$select=privilegeid,name&`$filter=name eq " +
      "'prvCreateAccount' or name eq 'prvReadAccount'"
      $privileges = Get-Records -setName 'privileges' -query $privQuery

      $message = "`nAdding 'prvCreateAccount' and 'prvReadAccount'" +
      " privileges to the role..."
      Write-Host $message

      # Prepare the privileges array
      $rolePrivilegesArray = @()
      foreach ($privilege in $privileges.value) {
         $rolePrivilegesArray += @{
            BusinessUnitId = $whoAmI.BusinessUnitId
            Depth          = 'Basic'
            PrivilegeId    = $privilege.privilegeid
            PrivilegeName  = $privilege.name
         }
      }

      # Add the privileges to the role
      $addRolePrivilegeParams = @{
         RoleId     = $roleId
         Privileges = $rolePrivilegesArray
      }
      Add-RolePrivilege @addRolePrivilegeParams

      $message = "Added the 'prvCreateAccount' and 'prvReadAccount'" +
      " privileges to the 'Test Role' security role."
      Write-Host $message

      # Get updated privileges
      $updatedRolePrivileges = Get-Records `
         -setName "roles($roleId)/roleprivileges_association" `
         -query '?$select=name'

      Write-Host "`nNumber of privileges after: $($updatedRolePrivileges.value.Count)"
      foreach ($privilege in $updatedRolePrivileges.value) {
         Write-Host "`t$($privilege.name)"
      }

      #endregion Section 8: Bound Action AddPrivilegesRole

   }
   catch {
      Write-Host "`nAn error occurred: $_" -ForegroundColor Red
      # Re-throw to preserve error details
      throw
   }
   finally {
      #region Cleanup: Delete sample records

      if ($deleteCreatedRecords -and $recordsToDelete.Count -gt 0) {
         Write-Host "`n-- Section 9: Delete sample records --"
         Write-Host 'Deleting sample records...'

         # Delete in reverse order to handle dependencies
         [array]::Reverse($recordsToDelete)

         foreach ($record in $recordsToDelete) {
            try {
               Write-Host "`tDeleting $($record.description)"
               Remove-Record -setName $record.setName -id $record.id
            }
            catch {
               Write-Host "`tFailed to delete $($record.description): $_" -ForegroundColor Yellow
            }
         }

         Write-Host 'Sample record cleanup completed.'
      }
      elseif (-not $deleteCreatedRecords -and $recordsToDelete.Count -gt 0) {

         $message = "`nSample records were not deleted. Set " +
         "`$deleteCreatedRecords to `$true to enable automatic cleanup."
         Write-Host $message
      }

      #endregion Cleanup: Delete sample records
   }

   Write-Host "`n--Functions and Actions sample completed--"
}
