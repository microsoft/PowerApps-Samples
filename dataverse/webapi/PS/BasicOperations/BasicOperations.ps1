. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1

Connect 'https://yourorg.crm.dynamics.com/' # change this
$deleteCreatedRecords = $true # change this if you want to keep the records created by this sample

$recordsToDelete = @()

Invoke-DataverseCommands {

   Write-Host  '--Starting Basic Operations sample--'

   #region Section 1: Basic Create and Update operations

   $contactRafelShillo = @{
      'firstname' = 'Rafel'
      'lastname'  = 'Shillo'
   }

   $rafelShilloId = New-Record `
      -setName 'contacts' `
      -body $contactRafelShillo

   Write-Host '--Created contact with ID:' $rafelShilloId

   # To delete later
   $recordsToDelete += @{
      'setName' = 'contacts'
      'id'      = $rafelShilloId
   }

   $rafelShilloUpdate1 = @{
      'annualincome' = 80000
      'jobtitle'     = 'Senior Software Engineer'
   }

   Update-Record `
      -setName 'contacts' `
      -id $rafelShilloId `
      -body $rafelShilloUpdate1

   Write-Host 'Contact' `
   ($contactRafelShillo.firstname) `
   ($contactRafelShillo.lastname) `
      "updated with jobtitle and annual income.`n"

   # Retrieve the created contact
   $retrievedRafelShillo1 = Get-Record `
      -setName 'contacts' `
      -id $rafelShilloId `
      -query '?$select=fullname,annualincome,jobtitle,description'

   Write-Host 'Contact' `
   ($retrievedRafelShillo1.fullname) `
      "retrieved."
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

   Write-Host "`nContact" `
   ($retrievedRafelShillo1.fullname) `
      "updated."
   Write-Host "`tAnnual income:" `
   ($rafelShilloUpdate2.annualincome)
   Write-Host "`tJob title:" `
   ($rafelShilloUpdate2.jobtitle)
   Write-Host "`tDescription:" `
   ($rafelShilloUpdate2.description)

   # Update record with new values
   Update-Record `
      -setName 'contacts' `
      -id $rafelShilloId `
      -body $rafelShilloUpdate2

   #Set single column value
   Set-ColumnValue `
      -setName 'contacts' `
      -id $rafelShilloId `
      -property 'telephone1' `
      -value '555-0105'

   Write-Host "`nContact" `
   ($retrievedRafelShillo1.fullname) `
      "phone number updated."
   
   # Retrieve just the telephone1 column
   $telephone1 = Get-ColumnValue `
      -setName 'contacts' `
      -id $rafelShilloId `
      -property 'telephone1'

   Write-Host "`nContacts telephone number is:" `
      $telephone1

   #region Section 5: Delete sample entities

   if ($deleteCreatedRecords) {
      Write-Host '--Deleting sample records--'
      foreach ($record in $recordsToDelete) {
         Remove-Record `
            -setName $record['setName'] `
            -id $record['id']
      }
   }

}