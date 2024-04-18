. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1

Connect 'https://yourorg.crm.dynamics.com/' # change this
$deleteCreatedRecords = $true # change this if you want to keep the records created by this sample

$recordsToDelete = @()
$publisherId = $null

Invoke-DataverseCommands { 

   #region Section 0: Create Publisher and Solution

   $publisherData = @{
      'uniquename' = 'examplepublisher'
      'friendlyname' = 'Example Publisher'
      'description' = 'An example publisher for samples'
      'customizationprefix' = 'sample'
      'customizationoptionvalueprefix' = 72700
   } 

   # Check if the publisher already exists
   $publisherQuery = "?`$filter=uniquename eq "
   $publisherQuery += "'$($publisherData.uniquename)' "
   $publisherQuery += "and customizationprefix eq "
   $publisherQuery += "'$($publisherData.customizationprefix)' "
   $publisherQuery += "and customizationoptionvalueprefix eq "
   $publisherQuery += "$($publisherData.customizationoptionvalueprefix)"
   $publisherQuery += "&`$select=friendlyname"

   $publisherQueryResults = (Get-Records `
      -setName 'publishers' `
      -query $publisherQuery).value
   
   if ($publisherQueryResults.Length -eq 0) {
      # Create the publisher if it doesn't exist
      $publisherId = New-Record `
         -setName 'publishers' `
         -body $publisherData
      
      Write-Host 'Example Publisher created successfully'
      $publisherRecordToDelete = @{ 
         'setName' = 'publishers'
         'id' = $publisherId 
      }
      $recordsToDelete += $publisherRecordToDelete

   } else {
      # Example Publisher already exists
      Write-Host "$($publisherQueryResults[0].friendlyname) already exists"
      $publisherId = $publisherQueryResults[0].publisherid
   }

   $solutionData = @{
      'uniquename' = 'examplesolution'
      'friendlyname' = 'Example Solution'
      'description' = 'An example solution for samples'
      'version' = '1.0.0.0'
      'publisherid@odata.bind' = "/publishers($publisherId)"
   }

   # Check if the solution already exists
   $solutionQuery = "?`$filter=uniquename eq "
   $solutionQuery += "'$($solutionData.uniquename)' "
   $solutionQuery += "and _publisherid_value eq $publisherId"
   $solutionQuery += "&`$select=friendlyname"

   $solutionQueryResults = (Get-Records `
      -setName 'solutions' `
      -query $solutionQuery).value
   
   if ($solutionQueryResults.Length -eq 0) {
      # Create the solution if it doesn't exist
      $solutionId = New-Record `
         -setName 'solutions' `
         -body $solutionData
      
      Write-Host 'Example Solution created successfully'
      # Must be deleted before publisher, so add it to the beginning of the array
      $solutionRecordToDelete = @{ 
         'setName' = 'solutions'
         'id' = $solutionId 
      }
      if($publisherRecordToDelete){
         $recordsToDelete = $solutionRecordToDelete, $publisherRecordToDelete
      } else {
         $recordsToDelete += $solutionRecordToDelete
      }

   } else {
      # Example Solution already exists
      Write-Host "$($solutionQueryResults[0].friendlyname) already exists"
      $solutionId = $solutionQueryResults[0].solutionid
   }





   #endregion Section 0: Create Publisher and Solution

   #region Section 1: Create, Retrieve and Update Table
   #endregion Section 1: Create, Retrieve and Update Table

   #region Section 2: Create, Retrieve and Update Columns
   #endregion Section 2: Create, Retrieve and Update Columns

   #region Section 3: Create and use Global OptionSet
   #endregion Section 3: Create and use Global OptionSet

   #region Section 4: Create Customer Relationship
   #endregion Section 4: Create Customer Relationship

   #region Section 5: Create and retrieve a one-to-many relationship
   #endregion Section 5: Create and retrieve a one-to-many relationship

   #region Section 6: Create and retrieve a many-to-one relationship
   #endregion Section 6: Create and retrieve a many-to-one relationship

   #region Section 7: Create and retrieve a many-to-many relationship
   #endregion Section 7: Create and retrieve a many-to-many relationship

   #region Section 8: Export managed solution
   #endregion Section 8: Export managed solution

   #region Section 9: Delete sample records
   if($deleteCreatedRecords) {
      $recordsToDelete | ForEach-Object {
         Remove-Record -setName $_.setName -id $_.id | Out-Null
         Write-Host "$($_.setName) record with ID: $($_.id) deleted"
      }
   }
   #endregion Section 9: Delete sample records

   #region Section 10: Import and Delete managed solution
   #endregion Section 10: Import and Delete managed solution

}