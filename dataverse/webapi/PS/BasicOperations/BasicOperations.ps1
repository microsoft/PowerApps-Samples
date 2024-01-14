. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1

Connect 'https://yourorg.crm.dynamics.com/' # change this
$deleteCreatedRecords = $true # change this if you want to keep the records created by this sample

$recordsToDelete = [PsObject[]]@()

Invoke-DataverseCommands {

   Write-Host  '--Starting Basic Operations sample--'

   #region Section 1: Basic Create and Update operations
   Write-Host "`n--Section 1 started--`n"

   $contactRafelShillo = @{
      'firstname' = 'Rafel'
      'lastname'  = 'Shillo'
   }

   $rafelShilloId = New-Record `
      -setName 'contacts' `
      -body $contactRafelShillo

   Write-Host "Contact '$($contactRafelShillo.firstname) $($contactRafelShillo.lastname)' created"
   Write-Host "`tContact URI: $baseURI($rafelShilloId)"
   Write-Host "`tContact relative Uri: contacts($rafelShilloId)"

   # To delete later
   $recordsToDelete += [PsObject]@{
      'description' = "Contact '$($contactRafelShillo.firstname) $($contactRafelShillo.lastname)'"
      'setName' = 'contacts'
      'id'      = $rafelShilloId
   }

   # Data to update for Rafel Shillo
   $rafelShilloUpdate1 = @{
      'annualincome' = 80000
      'jobtitle'     = 'Junior Developer'
   }

   # Update the record with the data
   Update-Record `
      -setName 'contacts' `
      -id $rafelShilloId `
      -body $rafelShilloUpdate1

   $message = "Contact '$($contactRafelShillo.firstname) $($contactRafelShillo.lastname)' "
   $message += 'updated with jobtitle and annual income.'
   Write-Host $message

   # Retrieve the created contact
   $retrievedRafelShillo1 = Get-Record `
      -setName 'contacts' `
      -id $rafelShilloId `
      -query '?$select=fullname,annualincome,jobtitle,description'

   Write-Host "`nContact '$($retrievedRafelShillo1.fullname) retrieved."
   Write-Host "`tAnnual income:" `
   ($retrievedRafelShillo1.'annualincome@OData.Community.Display.V1.FormattedValue')
   Write-Host "`tJob title:" `
   ($retrievedRafelShillo1.jobtitle)
   Write-Host "`tDescription:" `  # Description is initialized empty.
      ($retrievedRafelShillo1.description)

   # Modify specific properties and then update contact record.
   $rafelShilloUpdate2 = @{
      'jobtitle'     = 'Senior Developer'
      'annualincome' = 95000
      'description'  = 'Assignment to-be-determined'
   }

   # Update record with new values
   Update-Record `
      -setName 'contacts' `
      -id $rafelShilloId `
      -body $rafelShilloUpdate2

   Write-Host "Contact '$($retrievedRafelShillo1.fullname)' updated"
   Write-Host "`tJob title: $($rafelShilloUpdate2.jobtitle)"
   Write-Host "`tAnnual income: $($rafelShilloUpdate2.annualincome)"
   Write-Host "`tDescription: $($rafelShilloUpdate2.description)"

   #Set single column value
   Set-ColumnValue `
      -setName 'contacts' `
      -id $rafelShilloId `
      -property 'telephone1' `
      -value '555-0105' | Out-Null

   Write-Host "`nContact '$($retrievedRafelShillo1.fullname)' phone number updated."
   
   # Retrieve just the telephone1 column
   $telephone1 = Get-ColumnValue `
      -setName 'contacts' `
      -id $rafelShilloId `
      -property 'telephone1'

   Write-Host "`tContact's telephone number is: $telephone1"

   #endregion Section 1: Basic Create and Update operations

   #region Section 2: Create record associated to another
   #  Demonstrates creation of records and simultaneous association to another, 
   #  existing record. 

   Write-Host "`n--Section 2 started--`n"

   #Create a new account and associate with existing contact in one operation. 
   $accountContoso = @{
      "name"                        = "Contoso Ltd" 
      "telephone1"                  = "555-5555"
      "primarycontactid@odata.bind" = "/contacts($rafelShilloId)"
   };

   $accountContosoId = New-Record -setName 'accounts' -body $accountContoso

   # To delete later
   $recordsToDelete += [PsObject]@{
      'description' = "Account '$($accountContoso.name)'"
      'setName' = 'accounts'
      'id'      = $accountContosoId
   }

   Write-Host "Account '$($accountContoso.name)' created."
   Write-Host "`tAccount URI: accounts($accountContosoId)"

   # Retrieve account name and primary contact info
   $retrievedAccountContoso = Get-Record `
      -setName 'accounts' `
      -id $accountContosoId `
      -query '?$select=name,&$expand=primarycontactid($select=fullname,jobtitle,annualincome)'
   
   Write-Host "`nAccount '$($retrievedAccountContoso.name)' has primary contact" `
      "'$($retrievedAccountContoso.primarycontactid.fullname)':"
   Write-Host "`tJob title: $($retrievedAccountContoso.primarycontactid.jobtitle)"
   Write-Host "`tAnnual income: $($retrievedAccountContoso.primarycontactid.'annualincome@OData.Community.Display.V1.FormattedValue')"
   
   #endregion Section 2: Create record associated to another

   #region Section 3: Create related entities
   # Demonstrates creation of entity instance and related entities in a single operation.
   Write-Host "`n--Section 3 started--`n"
   # Create the following entries in one operation: an account, its 
   #  associated primary contact, and open tasks for that contact.  These 
   #  entity types have the following relationships:
   #     Accounts 
   #        |---[Primary] Contact (N-to-1)
   #               |---Tasks (1-to-N)

   $accountFourthCoffee = @{
      "name"             = "Fourth Coffee"
      "primarycontactid" = @{
         "firstname"     = "Susie"
         "lastname"      = "Curtis"
         "jobtitle"      = "Coffee Master"
         "annualincome"  = 48000
         "Contact_Tasks" = @(
            @{
               "subject"                  = "Sign invoice"
               "description"              = "Invoice #12321"
               "scheduledstart"           = New-Object DateTimeOffset(2024, 4, 19, 3, 0, 0, (New-TimeSpan -Hours 7))
               "scheduledend"             = New-Object DateTimeOffset(2024, 4, 19, 4, 0, 0, (New-TimeSpan -Hours 7))
               "scheduleddurationminutes" = 60
            },
            @{
               "subject"                  = "Setup new display"
               "description"              = "Theme is - Spring is in the air"
               "scheduledstart"           = New-Object DateTimeOffset(2024, 4, 20, 3, 0, 0, (New-TimeSpan -Hours 7))
               "scheduledend"             = New-Object DateTimeOffset(2024, 4, 20, 4, 0, 0, (New-TimeSpan -Hours 7))
               "scheduleddurationminutes" = 60
            }
            @{
               "subject"                  = "Conduct training"
               "description"              = "Train team on making our new blended coffee"
               "scheduledstart"           = New-Object DateTimeOffset(2024, 4, 21, 3, 0, 0, (New-TimeSpan -Hours 7))
               "scheduledend"             = New-Object DateTimeOffset(2024, 4, 21, 4, 0, 0, (New-TimeSpan -Hours 7))
               "scheduleddurationminutes" = 60
            }
         )
      }
   }

   # Create the account record and related records
   $accountFourthCoffeeId = New-Record `
      -setName 'accounts' `
      -body $accountFourthCoffee
  
   # To delete later
   $recordsToDelete += [PsObject]@{
      'description' = "Account '$($accountFourthCoffee.name)'"
      'setName' = 'accounts'
      'id'      = $accountFourthCoffeeId
   }

   Write-Host "Account '$($accountFourthCoffee.name)' created."
   Write-Host "`tAccount URI: accounts($accountFourthCoffeeId)"

   #  Retrieve account, primary contact info, and assigned tasks for contact.
   #  Dataverse only supports querying-by-expansion one level deep, so first query 
   #  account-primary contact.

   $retrievedAccountFourthCoffee = Get-Record `
      -setName 'accounts' `
      -id $accountFourthCoffeeId `
      -query '?$select=name&$expand=primarycontactid($select=fullname,jobtitle,annualincome)'

   Write-Host "`nAccount '$($retrievedAccountFourthCoffee.name)' has primary contact" `
      "'$($retrievedAccountFourthCoffee.primarycontactid.fullname)':"
   Write-Host "`tJob title: $($retrievedAccountFourthCoffee.primarycontactid.jobtitle)"
   Write-Host "`tAnnual income: $($retrievedAccountFourthCoffee.primarycontactid.'annualincome@OData.Community.Display.V1.FormattedValue')"

   $contactSusieCurtisId = $retrievedAccountFourthCoffee.primarycontactid.contactid

   # To delete later
   $recordsToDelete += [PsObject]@{
      'description' = "Contact '$($retrievedAccountFourthCoffee.primarycontactid.fullname)'"
      'setName' = 'contacts'
      'id'      = $contactSusieCurtisId
   }

   # Next retrieve same contact and their assigned tasks.

   $contactSusieCurtis = Get-Record `
      -setName 'contacts' `
      -id $contactSusieCurtisId `
      -query '?$select=fullname&$expand=Contact_Tasks($select=subject,description,scheduledstart,scheduledend)'

   Write-Host "`nContact '$($contactSusieCurtis.fullname)' has the following assigned tasks:"

   foreach ($task in $contactSusieCurtis.Contact_Tasks) {
      Write-Host "`tSubject: $($task.subject)`n" `
      "`t`tDescription: $($task.description)"
      "`t`tStart: $($task.'scheduledstart@OData.Community.Display.V1.FormattedValue')"
      "`t`tEnd: $($task.'scheduledend@OData.Community.Display.V1.FormattedValue')"
   }

   #endregion Section 3: Create related entities

   #region Section 4: Associate and Disassociate entities
   # Demonstrates associating and disassociating existing records.
   Write-Host "`n--Section 4 started--`n"

   #  Add 'Rafel Shillo' to the contact list of 'Fourth Coffee', 
   #   a 1-to-N relationship.

   Add-ToCollection `
      -targetSetName 'accounts' `
      -targetId $accountFourthCoffeeId `
      -collectionName 'contact_customer_accounts' `
      -setName 'contacts' `
      -id $rafelShilloId | Out-Null

   # Retrieve and output all contacts for account 'Fourth Coffee'.

   $fourthCoffeeContacts = Get-Records `
      -setName 'accounts' `
      -query ('({0})/contact_customer_accounts?$select=fullname,jobtitle' `
         -f $accountFourthCoffeeId)

   # Find 'Rafel Shillo' in the list of 'Fourth Coffee' account contacts
   Write-Host "Contact list for account '$($accountFourthCoffee.name)':"
   foreach ($contact in $fourthCoffeeContacts.value) {
      Write-Host "`tName: $($contact.fullname)," `
         "Job title: $($contact.jobtitle)"
   }

   # Remove 'Rafel Shillo' from the contact list of 'Fourth Coffee'

   Remove-FromCollection `
      -targetSetName 'accounts' `
      -targetId $accountFourthCoffeeId `
      -collectionName 'contact_customer_accounts' `
      -id $rafelShilloId | Out-Null

   # Create role and assign it to systemuser using systemuserroles_association

   $WhoIAm = Get-WhoAmI
   $myBusinessUnit = $WhoIAm.BusinessUnitId
   $myUserId = $WhoIAm.UserId

   $exampleSecurityRole = @{
      'businessunitid@odata.bind' = "businessunits($myBusinessUnit)"
      'name'                      = 'Example Security Role'
   }

   $exampleSecurityRoleId = New-Record `
      -setName 'roles' `
      -body $exampleSecurityRole

   # To delete later
   $recordsToDelete += [PsObject]@{
      'description' = "Role '$($exampleSecurityRole.name)'"
      'setName' = 'roles'
      'id'      = $exampleSecurityRoleId
   }

   # Associate role to you via systemuserroles_association.
   Add-ToCollection `
      -targetSetName 'systemusers' `
      -targetId $myUserId `
      -collectionName 'systemuserroles_association' `
      -setName 'roles' `
      -id $exampleSecurityRoleId

   Write-Host "Security Role '$($exampleSecurityRole.name)' associated with your user account."

   # Retrieve the new security role as part via the systemuserroles_association

   $myRoles = Get-records `
      -setName 'systemusers' `
      -query ('({0})/systemuserroles_association?$select=name' -f $myUserId)

   Write-Host "`nSecurity roles associated to my account:"

   foreach ($role in $myRoles.value) {
      Write-Host "`t-$($role.name)"
   }

   # Disassociate role to systemuser via systemuserroles_association.

   Remove-FromCollection `
      -targetSetName 'systemusers' `
      -targetId $myUserId `
      -collectionName 'systemuserroles_association' `
      -id $exampleSecurityRoleId | Out-Null

   #endregion Section 4: Associate and Disassociate entities

   #region Section 5: Delete sample records

   Write-Host "`n--Section 5 started--`n"

   if ($deleteCreatedRecords) {
      Write-Host '--Deleting sample records--'
      foreach ($record in $recordsToDelete) {
         Write-Host "`tDeleting $($record.description)"
         Remove-Record `
            -setName $record['setName'] `
            -id $record['id'] | Out-Null
      }
   }

   #endregion Section 5: Delete sample records

   Write-Host  `n'--Basic Operations sample completed--'

}