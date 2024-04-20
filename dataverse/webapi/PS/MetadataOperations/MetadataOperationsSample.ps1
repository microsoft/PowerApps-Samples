. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1
. $PSScriptRoot\..\MetadataOperations.ps1

# Change this to the URL of your Dataverse environment
Connect 'https://yourorg.crm.dynamics.com/' 

# Change this if you want to keep the records created by this sample
$deleteCreatedRecords = $true 
$recordsToDelete = @()
$publisherId = $null
$languageCode = 1033

Invoke-DataverseCommands { 

   #region Section 0: Create Publisher and Solution

   $publisherData = @{
      'uniquename'                     = 'examplepublisher'
      'friendlyname'                   = 'Example Publisher'
      'description'                    = 'An example publisher for samples'
      'customizationprefix'            = 'sample'
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
         'id'      = $publisherId 
      }
      $recordsToDelete += $publisherRecordToDelete

   }
   else {
      # Example Publisher already exists
      Write-Host "$($publisherQueryResults[0].friendlyname) already exists"
      $publisherId = $publisherQueryResults[0].publisherid
   }

   $solutionData = @{
      'uniquename'             = 'metadataexamplesolution'
      'friendlyname'           = 'Metadata Example Solution'
      'description'            = 'An example solution for metadata samples'
      'version'                = '1.0.0.0'
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
         'id'      = $solutionId 
      }
      $recordsToDelete += $solutionRecordToDelete
   }
   else {
      # Example Solution already exists
      Write-Host "$($solutionQueryResults[0].friendlyname) already exists"
      $solutionId = $solutionQueryResults[0].solutionid
   }
   #endregion Section 0: Create Publisher and Solution

   #region Section 1: Create, Retrieve and Update Table
   
   # Definition of new 'sample_BankAccount' table to create
   $bankAccountTableData = @{
      '@odata.type'           = "Microsoft.Dynamics.CRM.EntityMetadata"
      'SchemaName'            = "$($publisherData.customizationprefix)_BankAccount"
      'DisplayName'           = @{
         '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
         'LocalizedLabels' = @(
            @{
               '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
               'Label'        = 'Bank Account'
               'LanguageCode' = $languageCode
            }
         )
      }
      'DisplayCollectionName' = @{
         '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
         'LocalizedLabels' = @(
            @{
               '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
               'Label'        = 'Bank Accounts'
               'LanguageCode' = $languageCode
            }
         )
      }
      'Description'           = @{
         '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
         'LocalizedLabels' = @(
            @{
               '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
               'Label'        = 'A table to store information about customer bank accounts'
               'LanguageCode' = $languageCode
            }
         )
      }
      'HasActivities'         = $false
      'HasNotes'              = $false
      'OwnershipType'         = 'UserOwned'
      'PrimaryNameAttribute'  = "$($publisherData.customizationprefix)_name"
      'Attributes'            = @(
         @{
            '@odata.type'   = 'Microsoft.Dynamics.CRM.StringAttributeMetadata'
            'IsPrimaryName' = $true
            'SchemaName'    = "$($publisherData.customizationprefix)_Name"
            'RequiredLevel' = @{
               'Value' = 'ApplicationRequired'
            }
            'DisplayName'   = @{
               '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
               'LocalizedLabels' = @(
                  @{
                     '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                     'Label'        = 'Name'
                     'LanguageCode' = $languageCode
                  }
               )
            }
            'Description'   = @{
               '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
               'LocalizedLabels' = @(
                  @{
                     '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                     'Label'        = 'The name of the bank account'
                     'LanguageCode' = $languageCode
                  }
               )
            }
            'MaxLength'     = 100
         }
      )
   }

   # Check if the table already exists
   $tableQuery = "?`$filter=SchemaName eq "
   $tableQuery += "'$($bankAccountTableData.SchemaName)' "
   $tableQuery += "&`$select=SchemaName,DisplayName"
   
   $tableQueryResults = (Get-Tables `
         -query $tableQuery).value

   if ($tableQueryResults.Length -eq 0) {
      # Create the table if it doesn't exist
      $tableId = New-Table `
         -body $bankAccountTableData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Example Table created successfully'
      $tableToDelete = @{ 
         'setName' = 'EntityDefinitions'
         'id'      = $tableId 
      }
      $recordsToDelete += $tableToDelete
   }
   else {
      # Example table already exists
      Write-Host "$($tableQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $tableId = $tableQueryResults[0].MetadataId
   }

   # Retrieve the table to update it
   $table = Get-Table `
      -logicalName ($bankAccountTableData.SchemaName.ToLower())

   Write-Host "Retrieved $($table.DisplayName.UserLocalizedLabel.Label) table."

   # Update the table
   $table.HasActivities = $true
   $table.Description = @{
      '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
      'LocalizedLabels' = @(
         @{
            '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
            'Label'        = 'Contains information about customer bank accounts'
            'LanguageCode' = $languageCode
         }
      )
   }
   # Send the request to update the table
   # TODO: Uncomment this later...
   # Update-Table `
   #    -table $table `
   #    -solutionUniqueName $solutionData.uniquename `
   #    -mergeLabels $true

   # Write-Host "$($table.DisplayName.UserLocalizedLabel.Label) table updated successfully"

   #endregion Section 1: Create, Retrieve and Update Table

   #region Section 2: Create, Retrieve and Update Columns

   $tableAttributesPath = "EntityDefinitions(LogicalName='"
   $tableAttributesPath += "$($bankAccountTableData.SchemaName.ToLower())')"
   $tableAttributesPath += "/Attributes"

   #region Boolean

   $boolColumnData = @{
      '@odata.type'   = 'Microsoft.Dynamics.CRM.BooleanAttributeMetadata'
      'SchemaName'    = "$($publisherData.customizationprefix)_Boolean"
      'DefaultValue'  = $false
      'RequiredLevel' = @{
         'Value' = 'None'
      }
      'DisplayName'   = @{
         '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
         'LocalizedLabels' = @(
            @{
               '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
               'Label'        = 'Sample Boolean'
               'LanguageCode' = $languageCode
            }
         )
      }
      'Description'   = @{
         '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
         'LocalizedLabels' = @(
            @{
               '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
               'Label'        = 'Sample Boolean column description'
               'LanguageCode' = $languageCode
            }
         )
      }
      'OptionSet'     = @{
         '@odata.type' = 'Microsoft.Dynamics.CRM.BooleanOptionSetMetadata'
         'TrueOption'  = @{
            'Value' = 1
            'Label' = @{
               '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
               'LocalizedLabels' = @(
                  @{
                     '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                     'Label'        = 'True'
                     'LanguageCode' = $languageCode
                  }
               )
            }
         }
         'FalseOption' = @{
            'Value' = 0
            'Label' = @{
               '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
               'LocalizedLabels' = @(
                  @{
                     '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
                     'Label'        = 'False'
                     'LanguageCode' = $languageCode
                  }
               )
            }
         }
      }
   }

   # Check if the column already exists
   $boolColumnQuery = "?`$filter=SchemaName eq "
   $boolColumnQuery += "'$($boolColumnData.SchemaName)'"
   $boolColumnQuery += "&`$select=SchemaName,DisplayName"
   
   $boolColumnQueryResults = Get-TableColumns `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -query $boolColumnQuery

   if ($boolColumnQueryResults.Length -eq 0) {
      # Create the column if it doesn't exist
      $boolColumnId = New-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -column $boolColumnData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Example Boolean column created successfully'

      $boolColumnToDelete = @{ 
         'setName' = $tableAttributesPath
         'id'      = $boolColumnId 
      }
      $recordsToDelete += $boolColumnToDelete
   }
   else {
      # Example bool column already exists
      Write-Host "$($boolColumnQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $boolColumnId = $boolColumnQueryResults[0].MetadataId
   }

   $retrievedBooleanColumn1 = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($boolColumnData.SchemaName.ToLower()) `
      -type 'Boolean' `
      -query "?`$expand=OptionSet" # So options will be returned

   $trueOption = $retrievedBooleanColumn1.OptionSet.TrueOption;
   $falseOption = $retrievedBooleanColumn1.OptionSet.FalseOption;
   
   Write-Host "Original Option Labels:"
   Write-Host " True Option Label: $($trueOption.Label.UserLocalizedLabel.Label)"
   Write-Host " False Option Label: $($falseOption.Label.UserLocalizedLabel.Label)"

   # Update the column
   $retrievedBooleanColumn1.DisplayName = @{
      '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
      'LocalizedLabels' = @(
         @{
            '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
            'Label'        = 'Sample Boolean Column Updated'
            'LanguageCode' = $languageCode
         }
      )
   }
   $retrievedBooleanColumn1.Description = @{
      '@odata.type'     = 'Microsoft.Dynamics.CRM.Label'
      'LocalizedLabels' = @(
         @{
            '@odata.type'  = 'Microsoft.Dynamics.CRM.LocalizedLabel'
            'Label'        = 'Sample Boolean column description updated'
            'LanguageCode' = $languageCode
         }
      )
   }
   $retrievedBooleanColumn1.RequiredLevel = @{
      'Value' = 'ApplicationRequired'
   }

   # TODO: Uncomment this later...
   # Update-Column `
   #    -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
   #    -column $retrievedBooleanColumn1 `
   #    -type 'Boolean' `
   #    -solutionUniqueName $solutionData.uniquename `
   #    -mergeLabels $true

   Write-Host "Sample Boolean Column updated successfully"

   #region Update option values

   # Update the True Option Label
   # TODO: Uncomment this later...
   # Update-OptionValue `
   #    -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
   #    -columnLogicalName ($boolColumnData.SchemaName.ToLower()) `
   #    -value 1 `
   #    -label 'Up' `
   #    -languageCode $languageCode `
   #    -solutionUniqueName ($solutionData.uniquename) `
   #    -mergeLabels $true
   
   # Update the False Option Label
   # TODO: Uncomment this later...
   # Update-OptionValue `
   #    -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
   #    -columnLogicalName ($boolColumnData.SchemaName.ToLower()) `
   #    -value 0 `
   #    -label 'Down' `
   #    -languageCode $languageCode `
   #    -solutionUniqueName ($solutionData.uniquename) `
   #    -mergeLabels $true

   Write-Host "Option values updated successfully"

   $retrievedBooleanColumn2 = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($boolColumnData.SchemaName.ToLower()) `
      -type 'Boolean' `
      -query "?`$expand=OptionSet" # So options will be returned

   $trueOption = $retrievedBooleanColumn2.OptionSet.TrueOption;
   $falseOption = $retrievedBooleanColumn2.OptionSet.FalseOption;
   
   Write-Host "Updated Option Labels:"
   Write-Host " True Option Label: $($trueOption.Label.UserLocalizedLabel.Label)"
   Write-Host " False Option Label: $($falseOption.Label.UserLocalizedLabel.Label)"

   #endregion Update option values

   #endregion Boolean

   #region DateTime
   $dateTimeColumnData = @{
      '@odata.type'    = 'Microsoft.Dynamics.CRM.DateTimeAttributeMetadata'
      SchemaName       = "$($publisherData.customizationprefix)_DateTime"
      RequiredLevel    = @{
         Value = 'None'
      }
      DisplayName      = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample DateTime'
               LanguageCode = $languageCode
            }
         )
      }
      Description      = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample DateTime column description'
               LanguageCode = $languageCode
            }
         )
      }
      DateTimeBehavior = @{
         Value = 'DateOnly'
      }
      Format           = 'DateOnly'
      ImeMode          = 'Disabled'
   }

   # Check if the column already exists
   $dateTimeColumnQuery = "?`$filter=SchemaName eq "
   $dateTimeColumnQuery += "'$($dateTimeColumnData.SchemaName)'"
   $dateTimeColumnQuery += "&`$select=SchemaName,DisplayName"
   
   $dateTimeColumnQueryResults = Get-TableColumns `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -query $dateTimeColumnQuery

   if ($dateTimeColumnQueryResults.Length -eq 0) {
      # Create the column if it doesn't exist
      $dateTimeColumnId = New-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -column $dateTimeColumnData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Example DateTime column created successfully'

      $dateTimeColumnToDelete = @{ 
         'setName' = $tableAttributesPath
         'id'      = $dateTimeColumnId 
      }
      $recordsToDelete += $dateTimeColumnToDelete
   }
   else {
      # Example DateTime column already exists
      Write-Host "$($dateTimeColumnQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $dateTimeColumnId = $dateTimeColumnQueryResults[0].MetadataId
   }

   # Retrieve the dateTime column
   $dateTimeColumn = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($dateTimeColumnData.SchemaName.ToLower()) `
      -type 'DateTime' `
      -query "?`$select=SchemaName,DisplayName,DateTimeBehavior,Format,ImeMode"
   
   Write-Host "Retrieved $($dateTimeColumn.DisplayName.UserLocalizedLabel.Label) column."
   Write-Host " DateTimeBehavior: $($dateTimeColumn.DateTimeBehavior.Value)"
   Write-Host " Format: $($dateTimeColumn.Format)"

   #endregion DateTime

   #region Decimal
   $decimalColumnData = @{
      '@odata.type' = 'Microsoft.Dynamics.CRM.DecimalAttributeMetadata'
      SchemaName    = "$($publisherData.customizationprefix)_Decimal"
      RequiredLevel = @{
         Value = 'None'
      }
      DisplayName   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Decimal'
               LanguageCode = $languageCode
            }
         )
      }
      Description   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Decimal column description'
               LanguageCode = $languageCode
            }
         )
      }
      MaxValue      = 100
      MinValue      = 0
      Precision     = 1
   }

   # Check if the column already exists
   $decimalColumnQuery = "?`$filter=SchemaName eq "
   $decimalColumnQuery += "'$($decimalColumnData.SchemaName)'"
   $decimalColumnQuery += "&`$select=SchemaName,DisplayName"

   $decimalColumnQueryResults = Get-TableColumns `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -query $decimalColumnQuery

   if ($decimalColumnQueryResults.Length -eq 0) {
      # Create the column if it doesn't exist
      $decimalColumnId = New-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -column $decimalColumnData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Example Decimal column created successfully'

      $decimalColumnToDelete = @{ 
         'setName' = $tableAttributesPath
         'id'      = $decimalColumnId 
      }
      $recordsToDelete += $decimalColumnToDelete
   }
   else {
      # Example Decimal column already exists
      Write-Host "$($decimalColumnQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $decimalColumnId = $decimalColumnQueryResults[0].MetadataId
   }

   # Retrieve the decimal column
   $decimalColumn = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($decimalColumnData.SchemaName.ToLower()) `
      -type 'Decimal' `
      -query "?`$select=SchemaName,DisplayName,MaxValue,MinValue,Precision"

   Write-Host "Retrieved $($decimalColumn.DisplayName.UserLocalizedLabel.Label) column."
   Write-Host " MaxValue: $($decimalColumn.MaxValue)"
   Write-Host " MinValue: $($decimalColumn.MinValue)"
   Write-Host " Precision: $($decimalColumn.Precision)"

   #endregion Decimal

   #region Integer
   $integerColumnData = @{
      '@odata.type' = 'Microsoft.Dynamics.CRM.IntegerAttributeMetadata'
      SchemaName    = "$($publisherData.customizationprefix)_Integer"
      RequiredLevel = @{
         Value = 'None'
      }
      DisplayName   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Integer'
               LanguageCode = $languageCode
            }
         )
      }
      Description   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Integer column description'
               LanguageCode = $languageCode
            }
         )
      }
      MaxValue      = 100
      MinValue      = 0
      Format        = 'None'
   }

   # Check if the column already exists
   $integerColumnQuery = "?`$filter=SchemaName eq "
   $integerColumnQuery += "'$($integerColumnData.SchemaName)'"
   $integerColumnQuery += "&`$select=SchemaName,DisplayName"

   $integerColumnQueryResults = Get-TableColumns `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -query $integerColumnQuery
   
   if ($integerColumnQueryResults.Length -eq 0) {
      # Create the column if it doesn't exist
      $integerColumnId = New-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -column $integerColumnData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Example Integer column created successfully'

      $integerColumnToDelete = @{ 
         'setName' = $tableAttributesPath
         'id'      = $integerColumnId 
      }
      $recordsToDelete += $integerColumnToDelete
   }
   else {
      # Example Integer column already exists
      Write-Host "$($integerColumnQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $integerColumnId = $integerColumnQueryResults[0].MetadataId
   }

   # Retrieve the integer column
   $integerColumn = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($integerColumnData.SchemaName.ToLower()) `
      -type 'Integer' `
      -query "?`$select=SchemaName,DisplayName,MaxValue,MinValue,Format"
   
   Write-Host "Retrieved $($integerColumn.DisplayName.UserLocalizedLabel.Label) column."
   Write-Host " MaxValue: $($integerColumn.MaxValue)"
   Write-Host " MinValue: $($integerColumn.MinValue)"
   Write-Host " Format: $($integerColumn.Format)"

   #endregion Integer

   #region Memo

   $memoColumnData = @{
      '@odata.type' = 'Microsoft.Dynamics.CRM.MemoAttributeMetadata'
      SchemaName    = "$($publisherData.customizationprefix)_Memo"
      RequiredLevel = @{
         Value = 'None'
      }
      DisplayName   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Memo'
               LanguageCode = $languageCode
            }
         )
      }
      Description   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Memo column description'
               LanguageCode = $languageCode
            }
         )
      }
      Format        = 'Text'
      ImeMode       = 'Disabled'
      MaxLength     = 500
   }

   # Check if the column already exists
   $memoColumnQuery = "?`$filter=SchemaName eq "
   $memoColumnQuery += "'$($memoColumnData.SchemaName)'"
   $memoColumnQuery += "&`$select=SchemaName,DisplayName"

   $memoColumnQueryResults = Get-TableColumns `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -query $memoColumnQuery

   if ($memoColumnQueryResults.Length -eq 0) {
      # Create the column if it doesn't exist
      $memoColumnId = New-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -column $memoColumnData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Example Memo column created successfully'

      $memoColumnToDelete = @{ 
         'setName' = $tableAttributesPath
         'id'      = $memoColumnId 
      }
      $recordsToDelete += $memoColumnToDelete
   }
   else {
      # Example Memo column already exists
      Write-Host "$($memoColumnQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $memoColumnId = $memoColumnQueryResults[0].MetadataId
   }

   # Retrieve the memo column
   $memoColumn = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($memoColumnData.SchemaName.ToLower()) `
      -type 'Memo' `
      -query "?`$select=SchemaName,DisplayName,Format,ImeMode,MaxLength"

   Write-Host "Retrieved $($memoColumn.DisplayName.UserLocalizedLabel.Label) column."
   Write-Host " Format: $($memoColumn.Format)"
   Write-Host " ImeMode: $($memoColumn.ImeMode)"
   Write-Host " MaxLength: $($memoColumn.MaxLength)"

   #endregion Memo

   #region Money

   $moneyColumnData = @{
      '@odata.type'   = 'Microsoft.Dynamics.CRM.MoneyAttributeMetadata'
      SchemaName      = "$($publisherData.customizationprefix)_Money"
      RequiredLevel   = @{
         Value = 'None'
      }
      DisplayName     = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Money'
               LanguageCode = $languageCode
            }
         )
      }
      Description     = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Money column description'
               LanguageCode = $languageCode
            }
         )
      }
      MaxValue        = 1000.00
      MinValue        = 0.00
      Precision       = 1
      PrecisionSource = 1
      ImeMode         = 'Disabled'
   }

   # Check if the column already exists
   $moneyColumnQuery = "?`$filter=SchemaName eq "
   $moneyColumnQuery += "'$($moneyColumnData.SchemaName)'"
   $moneyColumnQuery += "&`$select=SchemaName,DisplayName"

   $moneyColumnQueryResults = Get-TableColumns `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -query $moneyColumnQuery

   if ($moneyColumnQueryResults.Length -eq 0) {
      # Create the column if it doesn't exist
      $moneyColumnId = New-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -column $moneyColumnData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Example Money column created successfully'

      $moneyColumnToDelete = @{ 
         'setName' = $tableAttributesPath
         'id'      = $moneyColumnId 
      }
      $recordsToDelete += $moneyColumnToDelete
   }
   else {
      # Example Money column already exists
      Write-Host "$($moneyColumnQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $moneyColumnId = $moneyColumnQueryResults[0].MetadataId
   }

   # Retrieve the money column
   $moneyColumn = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($moneyColumnData.SchemaName.ToLower()) `
      -type 'Money' `
      -query "?`$select=SchemaName,DisplayName,MaxValue,MinValue,Precision,PrecisionSource,ImeMode"

   Write-Host "Retrieved $($moneyColumn.DisplayName.UserLocalizedLabel.Label) column."
   Write-Host " MaxValue: $($moneyColumn.MaxValue)"
   Write-Host " MinValue: $($moneyColumn.MinValue)"
   Write-Host " Precision: $($moneyColumn.Precision)"
   Write-Host " PrecisionSource: $($moneyColumn.PrecisionSource)"
   Write-Host " ImeMode: $($moneyColumn.ImeMode)"


   #endregion Money

   #region Picklist

   $picklistColumnData = @{
      '@odata.type' = 'Microsoft.Dynamics.CRM.PicklistAttributeMetadata'
      SchemaName    = "$($publisherData.customizationprefix)_Picklist"
      RequiredLevel = @{
         Value = 'None'
      }
      DisplayName   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Choice'
               LanguageCode = $languageCode
            }
         )
      }
      Description   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Choice column description'
               LanguageCode = $languageCode
            }
         )
      }
      OptionSet     = @{
         '@odata.type' = 'Microsoft.Dynamics.CRM.OptionSetMetadata'
         OptionSetType = 'Picklist'
         IsGlobal      = $false
         Options       = @(
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.OptionMetadata'
               Label         = @{
                  LocalizedLabels = @(
                     @{
                        Label        = 'Bravo'
                        LanguageCode = $languageCode
                     }
                  )
               }
               Value         = [int]([string]$publisherData.customizationoptionvalueprefix + '0000')
            },
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.OptionMetadata'
               Label         = @{
                  LocalizedLabels = @(
                     @{
                        Label        = 'Delta'
                        LanguageCode = $languageCode
                     }
                  )
               }
               Value         = [int]([string]$publisherData.customizationoptionvalueprefix + '0001')
            },
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.OptionMetadata'
               Label         = @{
                  LocalizedLabels = @(
                     @{
                        Label        = 'Alpha'
                        LanguageCode = $languageCode
                     }
                  )
               }
               Value         = [int]([string]$publisherData.customizationoptionvalueprefix + '0002')
            },
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.OptionMetadata'
               Label         = @{
                  LocalizedLabels = @(
                     @{
                        Label        = 'Charlie'
                        LanguageCode = $languageCode
                     }
                  )
               }
               Value         = [int]([string]$publisherData.customizationoptionvalueprefix + '0003')
            },
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.OptionMetadata'
               Label         = @{
                  LocalizedLabels = @(
                     @{
                        Label        = 'Foxtrot'
                        LanguageCode = $languageCode
                     }
                  )
               }
               Value         = [int]([string]$publisherData.customizationoptionvalueprefix + '0004')
            }
         )
      }
   }

   # Check if the column already exists
   $picklistColumnQuery = "?`$filter=SchemaName eq "
   $picklistColumnQuery += "'$($picklistColumnData.SchemaName)'"
   $picklistColumnQuery += "&`$select=SchemaName,DisplayName"

   $picklistColumnQueryResults = Get-TableColumns `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -query $picklistColumnQuery

   if ($picklistColumnQueryResults.Length -eq 0) {
      # Create the column if it doesn't exist
      $picklistColumnId = New-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -column $picklistColumnData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Example Picklist column created successfully'

      $picklistColumnToDelete = @{ 
         'setName' = $tableAttributesPath
         'id'      = $picklistColumnId 
      }
      $recordsToDelete += $picklistColumnToDelete
   }
   else {
      # Example Picklist column already exists
      Write-Host "$($picklistColumnQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $picklistColumnId = $picklistColumnQueryResults[0].MetadataId
   }

   # Retrieve the picklist column with OptionSet
   $picklistColumnV1 = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($picklistColumnData.SchemaName.ToLower()) `
      -type 'Picklist' `
      -query "?`$select=SchemaName,DisplayName&`$expand=OptionSet"

   Write-Host "Retrieved $($picklistColumnV1.DisplayName.UserLocalizedLabel.Label) column."

   Write-Host 'Retrieved Choice column options:'
   foreach ($option in $picklistColumnV1.OptionSet.Options) {
      Write-Host " Value: $($option.Value) Label: $($option.Label.UserLocalizedLabel.Label)"
   }

   #region Add an option to the local optionset if it doesn't exist

   $echoOptionValue = [int]([string]$publisherData.customizationoptionvalueprefix + '0005')
   $echoOptionExists = $picklistColumnV1.OptionSet.Options | 
   Where-Object { $_.Value -eq $echoOptionValue } | 
   Measure-Object | 
   ForEach-Object { $_.Count -gt 0 }
   

   if (-not $echoOptionExists) {
      # Add an option to the local optionset

      New-OptionValue `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -columnLogicalName ($picklistColumnData.SchemaName.ToLower()) `
         -label 'Echo' `
         -languageCode $languageCode `
         -value ([int]([string]$publisherData.customizationoptionvalueprefix + '0005')) `
         -solutionUniqueName $solutionData.uniquename | Out-Null
      # Setting Out-Null to suppress the output of the value 
      # returned by the New-OptionValue function because we are providing
      # the value to use. If not provided, the system will generate a value
      # and the New-OptionValue function function will return it.
      Write-Host 'Echo option added to the local optionset.'

      # Retrieve the picklist column again with OptionSet and new option
      $picklistColumnV2 = Get-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -logicalName ($picklistColumnData.SchemaName.ToLower()) `
         -type 'Picklist' `
         -query "?`$select=SchemaName,DisplayName&`$expand=OptionSet"

      Write-Host "Retrieved $($picklistColumnV2.DisplayName.UserLocalizedLabel.Label) column again."

      Write-Host 'Retrieved Choice column options:'
      foreach ($option in $picklistColumnV2.OptionSet.Options) {
         Write-Host " Value: $($option.Value) Label: $($option.Label.UserLocalizedLabel.Label)"
      }
   }
   #endregion Add an option to the local optionset

   #region Re-order choice column options

   # Retrieve the picklist column again with OptionSet and new option
   $picklistColumnV3 = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($picklistColumnData.SchemaName.ToLower()) `
      -type 'Picklist' `
      -query "?`$select=SchemaName,DisplayName&`$expand=OptionSet"

   $retrievedChoiceOptions = $picklistColumnV3.OptionSet.Options
   $retrievedChoiceOptions = $retrievedChoiceOptions | 
   Sort-Object -Property @{ Expression = { $_.Label.UserLocalizedLabel.Label } }
   $newOrder = @()
   $retrievedChoiceOptions | ForEach-Object {
      $newOrder += $_.Value
   }

   # Update the order of the options
   Update-OptionsOrder `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -columnLogicalName ($picklistColumnData.SchemaName.ToLower()) `
      -values $newOrder `
      -solutionUniqueName $solutionData.uniquename

   Write-Host 'Choice column options re-ordered.'

   # Retrieve the picklist column again with OptionSet and new option
   $picklistColumnV4 = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($picklistColumnData.SchemaName.ToLower()) `
      -type 'Picklist' `
      -query "?`$select=SchemaName,DisplayName&`$expand=OptionSet"

   Write-Host "Retrieved $($picklistColumnV4.DisplayName.UserLocalizedLabel.Label) column again."

   Write-Host 'Retrieved Choice column options with new order:'
   foreach ($option in $picklistColumnV4.OptionSet.Options) {
      Write-Host " Value: $($option.Value) Label: $($option.Label.UserLocalizedLabel.Label)"
   }
   #endregion Re-order choice column options

   #region Delete local option value

   $foxTrotOptionValue = [int]([string]$publisherData.customizationoptionvalueprefix + '0004')
   $foxTrotOptionExists = $picklistColumnV4.OptionSet.Options | 
   Where-Object { $_.Value -eq $foxTrotOptionValue } | 
   Measure-Object | 
   ForEach-Object { $_.Count -gt 0 }

   if ($foxTrotOptionExists) {
      # Delete the option from the local optionset
      Remove-OptionValue `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -columnLogicalName ($picklistColumnData.SchemaName.ToLower()) `
         -value $foxTrotOptionValue `
         -solutionUniqueName $solutionData.uniquename

      Write-Host 'Foxtrot option deleted from the local optionset.'
   }

   #endregion Delete local option value
   #endregion Picklist

   #region MultiSelectPicklist

   $multiSelectPicklistColumnData = @{
      '@odata.type' = 'Microsoft.Dynamics.CRM.MultiSelectPicklistAttributeMetadata'
      SchemaName    = "$($publisherData.customizationprefix)_MultiSelectChoice"
      RequiredLevel = @{
         Value = 'None'
      }
      DisplayName   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample MultiSelect Choice'
               LanguageCode = $languageCode
            }
         )
      }
      Description   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample MultiSelect Choice column description'
               LanguageCode = $languageCode
            }
         )
      }
      OptionSet     = @{
         '@odata.type' = 'Microsoft.Dynamics.CRM.OptionSetMetadata'
         OptionSetType = 'Picklist'
         IsGlobal      = $false
         Options       = @(
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.OptionMetadata'
               Label         = @{
                  LocalizedLabels = @(
                     @{
                        Label        = 'Appetizer'
                        LanguageCode = $languageCode
                     }
                  )
               }
               Value         = [int]([string]$publisherData.customizationoptionvalueprefix + '0000')
            },
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.OptionMetadata'
               Label         = @{
                  LocalizedLabels = @(
                     @{
                        Label        = 'Entree'
                        LanguageCode = $languageCode
                     }
                  )
               }
               Value         = [int]([string]$publisherData.customizationoptionvalueprefix + '0001')
            },
            @{
               '@odata.type' = 'Microsoft.Dynamics.CRM.OptionMetadata'
               Label         = @{
                  LocalizedLabels = @(
                     @{
                        Label        = 'Dessert'
                        LanguageCode = $languageCode
                     }
                  )
               }
               Value         = [int]([string]$publisherData.customizationoptionvalueprefix + '0002')
            }
         )
      }
   }

   # Check if the column already exists
   $multiSelectPicklistColumnQuery = "?`$filter=SchemaName eq "
   $multiSelectPicklistColumnQuery += "'$($multiSelectPicklistColumnData.SchemaName)'"
   $multiSelectPicklistColumnQuery += "&`$select=SchemaName,DisplayName"

   $multiSelectPicklistColumnQueryResults = Get-TableColumns `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -query $multiSelectPicklistColumnQuery

   if ($multiSelectPicklistColumnQueryResults.Length -eq 0) {
      # Create the column if it doesn't exist
      $multiSelectPicklistColumnId = New-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -column $multiSelectPicklistColumnData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Example MultiSelect Choice column created successfully'

      $multiSelectPicklistColumnToDelete = @{ 
         'setName' = $tableAttributesPath
         'id'      = $multiSelectPicklistColumnId 
      }
      $recordsToDelete += $multiSelectPicklistColumnToDelete
   }
   else {
      # Example MultiSelectPicklist column already exists
      Write-Host "$($multiSelectPicklistColumnQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $multiSelectPicklistColumnId = $multiSelectPicklistColumnQueryResults[0].MetadataId
   }

   # Retrieve the MultiSelectPicklist column with OptionSet
   $multiSelectPicklistColumn = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($multiSelectPicklistColumnData.SchemaName.ToLower()) `
      -type 'MultiSelectPicklist' `
      -query "?`$select=SchemaName,DisplayName&`$expand=OptionSet"

   Write-Host "Retrieved $($multiSelectPicklistColumn.DisplayName.UserLocalizedLabel.Label) column."

   Write-Host 'Retrieved MultiSelect Choice column options:'
   foreach ($option in $multiSelectPicklistColumn.OptionSet.Options) {
      Write-Host " Value: $($option.Value) Label: $($option.Label.UserLocalizedLabel.Label)"
   }
   #endregion MultiSelectPicklist

   #region BigInt

   $bigIntColumnData = @{
      '@odata.type' = 'Microsoft.Dynamics.CRM.BigIntAttributeMetadata'
      SchemaName    = "$($publisherData.customizationprefix)_BigInt"
      RequiredLevel = @{
         Value = 'None'
      }
      DisplayName   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample BigInt'
               LanguageCode = $languageCode
            }
         )
      }
      Description   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample BigInt column description'
               LanguageCode = $languageCode
            }
         )
      }
   }

   # Check if the column already exists
   $bigIntColumnQuery = "?`$filter=SchemaName eq "
   $bigIntColumnQuery += "'$($bigIntColumnData.SchemaName)'"
   $bigIntColumnQuery += "&`$select=SchemaName,DisplayName"

   $bigIntColumnQueryResults = Get-TableColumns `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -query $bigIntColumnQuery

   if ($bigIntColumnQueryResults.Length -eq 0) {
      # Create the column if it doesn't exist
      $bigIntColumnId = New-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -column $bigIntColumnData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Example BigInt column created successfully'

      $bigIntColumnToDelete = @{ 
         'setName' = $tableAttributesPath
         'id'      = $bigIntColumnId 
      }
      $recordsToDelete += $bigIntColumnToDelete
   }
   else {
      # Example BigInt column already exists
      Write-Host "$($bigIntColumnQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $bigIntColumnId = $bigIntColumnQueryResults[0].MetadataId
   }

   # Retrieve the BigInt column
   $bigIntColumn = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($bigIntColumnData.SchemaName.ToLower()) `
      -type 'BigInt' `
      -query "?`$select=SchemaName,DisplayName,MaxValue,MinValue"

   Write-Host "Retrieved $($bigIntColumn.DisplayName.UserLocalizedLabel.Label) column."   
   Write-Host " MaxValue: $($bigIntColumn.MaxValue)"
   Write-Host " MinValue: $($bigIntColumn.MinValue)"

   #endregion BigInt

   #region InsertStatusValue

   # Retrieve the status column with OptionSet
   $statusColumnV1 = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName 'statuscode' `
      -type 'Status' `
      -query "?`$select=SchemaName,DisplayName&`$expand=OptionSet"


   $frozenOptionExists = $statusColumnV1.OptionSet.Options | 
   Where-Object { $_.Label.UserLocalizedLabel.Label -eq 'Frozen' }
   Measure-Object | 
   ForEach-Object { $_.Count -gt 0 } | Out-Null

   if (!$frozenOptionExists) {
   
      # Add a new status value to the status column
      $frozenStatusValue = New-StatusOption `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -label 'Frozen' `
         -languageCode $languageCode `
         -stateCode 1 `
         -solutionUniqueName $solutionData.uniquename
      Write-Host 'Frozen status added to the status column.'
      Write-Host "With the value of: $frozenStatusValue"

      # Retrieve the status column again with OptionSet and new option
      $statusColumnV2 = Get-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -logicalName 'statuscode' `
         -type 'Status' `
         -query "?`$select=SchemaName,DisplayName&`$expand=OptionSet"

      Write-Host "Retrieved $($statusColumnV2.DisplayName.UserLocalizedLabel.Label) column again."

      Write-Host 'Retrieved status column options:'
      foreach ($option in $statusColumnV2.OptionSet.Options) {
         Write-Host " Value: $($option.Value) Label: $($option.Label.UserLocalizedLabel.Label) State: $($option.State)"
      }

   }
   else {
      Write-Host "'Frozen' status option already exists"
   }

   #endregion InsertStatusValue

   #endregion Section 2: Create, Retrieve and Update Columns

   #region Section 3: Create and use Global OptionSet

   $colorsGlobalOptionSetData = @{
      '@odata.type' = 'Microsoft.Dynamics.CRM.OptionSetMetadata'
      Name          = "$($publisherData.customizationprefix)_colors"
      DisplayName   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Colors'
               LanguageCode = $languageCode
            }
         )
      }
      Description   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Color Choice description'
               LanguageCode = $languageCode
            }
         )
      }
      IsGlobal      = $true
      Options       = @(
         @{
            # '@odata.type' = 'Microsoft.Dynamics.CRM.OptionMetadata'
            Label = @{
               LocalizedLabels = @(
                  @{
                     Label        = 'Red'
                     LanguageCode = $languageCode
                  }
               )
            }
            Value = [int]([string]$publisherData.customizationoptionvalueprefix + '0000')
         },
         @{
            # '@odata.type' = 'Microsoft.Dynamics.CRM.OptionMetadata'
            Label = @{
               LocalizedLabels = @(
                  @{
                     Label        = 'Yellow'
                     LanguageCode = $languageCode
                  }
               )
            }
            Value = [int]([string]$publisherData.customizationoptionvalueprefix + '0001')
         },
         @{
            # '@odata.type' = 'Microsoft.Dynamics.CRM.OptionMetadata'
            Label = @{
               LocalizedLabels = @(
                  @{
                     Label        = 'Green'
                     LanguageCode = $languageCode
                  }
               )
            }
            Value = [int]([string]$publisherData.customizationoptionvalueprefix + '0002')
         }
      )
   }

   # Check if the global optionset already exists
   # Get-GlobalOptionSet returns $null if not found

   $colorsGlobalOptionSet = Get-GlobalOptionSet `
      -name $colorsGlobalOptionSetData.Name `
      -type 'OptionSet' `
      -query "?`$select=Name,DisplayName,Options"

   if ($null -eq $colorsGlobalOptionSet) {
      # Create the global optionset if it doesn't exist
      $colorsGlobalOptionSetId = New-GlobalOptionSet `
         -optionSet $colorsGlobalOptionSetData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Colors global optionset created successfully'

      $colorsGlobalOptionSetToDelete = @{ 
         'setName' = 'GlobalOptionSetDefinitions'
         'id'      = $colorsGlobalOptionSetId 
      }
      $recordsToDelete += $colorsGlobalOptionSetToDelete

      # Retrieve the global optionset
      $colorsGlobalOptionSet = Get-GlobalOptionSet `
         -type 'OptionSet' `
         -id $colorsGlobalOptionSetId `
         -query "?`$select=Name,DisplayName,Options"

      Write-Host "Retrieved $($colorsGlobalOptionSet.DisplayName.UserLocalizedLabel.Label) global optionset."
   }
   else {
      # Colors global optionset already exists
      Write-Host "$($colorsGlobalOptionSet.DisplayName.UserLocalizedLabel.Label) global optionset already exists"
      $colorsGlobalOptionSetId = $colorsGlobalOptionSet.MetadataId
   }

   Write-Host 'Retrieved global optionset options:'
   foreach ($option in $colorsGlobalOptionSet.Options) {
      Write-Host " Value: $($option.Value) Label: $($option.Label.UserLocalizedLabel.Label)"
   }

   # Create a column that uses the global optionset
   $colorColumnData = @{
      '@odata.type'                = 'Microsoft.Dynamics.CRM.PicklistAttributeMetadata'
      SchemaName                   = "$($publisherData.customizationprefix)_Colors"
      RequiredLevel                = @{
         Value = 'None'
      }
      DisplayName                  = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Colors Choice'
               LanguageCode = $languageCode
            }
         )
      }
      Description                  = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Colors Choice column description'
               LanguageCode = $languageCode
            }
         )
      }
      'GlobalOptionSet@odata.bind' = "/GlobalOptionSetDefinitions($colorsGlobalOptionSetId)"
   }

   # Check if the column already exists
   $colorColumnQuery = "?`$filter=SchemaName eq "
   $colorColumnQuery += "'$($colorColumnData.SchemaName)'"
   $colorColumnQuery += "&`$select=SchemaName,DisplayName"

   $colorColumnQueryResults = Get-TableColumns `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -query $colorColumnQuery

   if ($colorColumnQueryResults.Length -eq 0) {
      # Create the column if it doesn't exist
      $colorColumnId = New-Column `
         -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
         -column $colorColumnData `
         -solutionUniqueName $solutionData.uniquename
            
      Write-Host 'Example Colors Choice column created successfully'

      $colorColumnToDelete = @{ 
         'setName' = $tableAttributesPath
         'id'      = $colorColumnId 
      }
      $recordsToDelete += $colorColumnToDelete
   }
   else {
      # Example Colors Choice column already exists
      Write-Host "$($colorColumnQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $colorColumnId = $colorColumnQueryResults[0].MetadataId
   }

   # Retrieve the color column with GlobalOptionSet
   $colorColumn = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($colorColumnData.SchemaName.ToLower()) `
      -type 'Picklist' `
      -query "?`$select=SchemaName,DisplayName&`$expand=GlobalOptionSet"

   Write-Host "Retrieved $($colorColumn.DisplayName.UserLocalizedLabel.Label) column."
   
   Write-Host 'Retrieved Choice column options:'
   foreach ($option in $colorColumn.GlobalOptionSet.Options) {
      Write-Host " Value: $($option.Value) Label: $($option.Label.UserLocalizedLabel.Label)"
   }

   #endregion Section 3: Create and use Global OptionSet

   #region Section 4: Create Customer Relationship

   $customerLookupData = @{
      # '@odata.type' = 'Microsoft.Dynamics.CRM.LookupAttributeMetadata'
      SchemaName    = "$($publisherData.customizationprefix)_CustomerId"
      RequiredLevel = @{
         Value = 'None'
      }
      DisplayName   = @{
         LocalizedLabels = @(
            @{
               Label        = 'Sample Bank Account owner'
               LanguageCode = $languageCode
            }
         )
      }
      Description   = @{
         LocalizedLabels = @(
            @{
               Label        = 'The owner of the bank account'
               LanguageCode = $languageCode
            }
         )
      }
      Targets       = @('account', 'contact')
   }

   $customerRelationships = @(
      @{
         SchemaName        = "$($publisherData.customizationprefix)_BankAccount_Customer_Account"
         ReferencedEntity  = 'account'
         ReferencingEntity = $bankAccountTableData.SchemaName.ToLower()
         RelationshipType  = 'OneToManyRelationship'
      },
      @{
         SchemaName        = "$($publisherData.customizationprefix)_BankAccount_Customer_Contact"
         ReferencedEntity  = 'contact'
         ReferencingEntity = $bankAccountTableData.SchemaName.ToLower()
         RelationshipType  = 'OneToManyRelationship'
      }
   )

   # Check if the column already exists
   $customerLookupQuery = "?`$filter=SchemaName eq "
   $customerLookupQuery += "'$($customerLookupData.SchemaName)'"
   $customerLookupQuery += "&`$select=SchemaName,DisplayName"

   $customerLookupQueryResults = Get-TableColumns `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -query $customerLookupQuery

   if ($customerLookupQueryResults.Length -eq 0) {
      # Create the column if it doesn't exist
      $response = New-CustomerRelationship `
         -lookup $customerLookupData `
         -oneToManyRelationships $customerRelationships `
         -solutionUniqueName $solutionData.uniquename
   
      Write-Host 'Customer relationship created successfully'
   
      $customerLookupRelationshipIds = $response.RelationshipIds
      $customerLookupId = $response.AttributeId
            
      Write-Host 'Example Customer Lookup column created successfully'

      $customerLookupToDelete = @{ 
         'setName' = $tableAttributesPath
         'id'      = $customerLookupId 
      }
      $recordsToDelete += $customerLookupToDelete

 
   }
   else {
      # Example Customer Lookup column already exists
      Write-Host "$($customerLookupQueryResults[0].DisplayName.UserLocalizedLabel.Label) table already exists"
      $customerLookupId = $customerLookupQueryResults[0].MetadataId
   }

   $retrievedCustomerLookup = Get-Column `
      -tableLogicalName ($bankAccountTableData.SchemaName.ToLower()) `
      -logicalName ($customerLookupData.SchemaName.ToLower()) `
      -type 'Lookup' `
      -query "?`$select=SchemaName,DisplayName,Targets" 

   Write-Host "Retrieved $($retrievedCustomerLookup.DisplayName.UserLocalizedLabel.Label) column Targets:"
   foreach ($target in $retrievedCustomerLookup.Targets) {
      Write-Host " $target"
   }

   # $customerLookupRelationshipIds are set when the relationship is created.
   if ($customerLookupRelationshipIds) {
      Write-Host 'Retrieved Customer relationship IDs:'
      foreach ($relationshipId in $customerLookupRelationshipIds) {
         $relationship = Get-Relationship `
            -id $relationshipId `
            -query "?`$select=SchemaName"
         Write-Host " $($relationship.SchemaName)"
      }
   }

   #endregion Section 4: Create Customer Relationship

   #region Section 5: Create and retrieve a one-to-many relationship

   #region Validate 1:N relationship eligibility
   $canBeReferenced = Get-CanBeReferenced `
      -tableLogicalName $table.SchemaName.ToLower()

   $message = "`nThe $($table.DisplayName.UserLocalizedLabel.Label) table"
   $message += ($canBeReferenced) ? " is" : " is not"
   $message += " eligible to be a primary table in a one-to-many relationship."
   Write-Host $message

   $canBeReferencing = Get-CanBeReferencing `
      -tableLogicalName $table.SchemaName.ToLower()

   $message = "`nThe $($table.DisplayName.UserLocalizedLabel.Label) table"
   $message += ($canBeReferencing) ? " is" : " is not"
   $message += " eligible to be a related table in a one-to-many relationship."
   Write-Host $message
   
   
   #endregion Validate 1:N relationship eligibility

   #endregion Section 5: Create and retrieve a one-to-many relationship

   #region Section 6: Create and retrieve a many-to-one relationship
   #endregion Section 6: Create and retrieve a many-to-one relationship

   #region Section 7: Create and retrieve a many-to-many relationship
   #endregion Section 7: Create and retrieve a many-to-many relationship

   #region Section 8: Export managed solution
   #endregion Section 8: Export managed solution

   #region Section 9: Delete sample records
   if ($deleteCreatedRecords -and ($recordsToDelete.Length -gt 0)) {
      Write-Host 'Deleting sample records...'

      # In the reverse order of creation, delete the records created by this sample
      for ($i = $recordsToDelete.Length - 1; $i -ge 0; $i--) {
         $recordToDelete = $recordsToDelete[$i]
         Remove-Record -setName $recordToDelete.setName -id $recordToDelete.id | Out-Null
         Write-Host "$($recordToDelete.setName) record with ID: $($recordToDelete.id) deleted"
      }
   }
   #endregion Section 9: Delete sample records

   #region Section 10: Import and Delete managed solution
   #endregion Section 10: Import and Delete managed solution

}