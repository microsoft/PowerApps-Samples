. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1
. $PSScriptRoot\..\MetadataOperations.ps1

# Change this to the URL of your Dataverse environment
Connect 'https://yourorg.crm.dynamics.com/'

# Change this if you want to keep the records created by this sample
$deleteCreatedRecords = $true
# $recordsToDelete contains references to all records created by this sample
# that will be deleted if $deleteCreatedRecords is $true
$recordsToDelete = @()
$publisherId = $null
$languageCode = 1033
$managedSolutionExported = $false

Invoke-DataverseCommands {

   #region Section 0: Create Publisher and Solution
   # This sample uses the same publisher as MetadataOperationsSample.ps1

   $publisherData = @{
      uniquename                     = 'examplepublisher'
      friendlyname                   = 'Example Publisher'
      description                    = 'An example publisher for samples'
      customizationprefix            = 'sample'
      customizationoptionvalueprefix = 72700
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
      $recordsToDelete += @{
         setName = 'publishers'
         id      = $publisherId
      }
   }
   else {
      Write-Host "$($publisherQueryResults[0].friendlyname) already exists"
      $publisherId = $publisherQueryResults[0].publisherid
   }

   $solutionData = @{
      uniquename               = 'polymorphiclookupexamplesolution'
      friendlyname             = 'Polymorphic Lookup Example Solution'
      description              = 'An example solution for polymorphic lookup samples'
      version                  = '1.0.0.0'
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
      $solutionId = New-Record `
         -setName 'solutions' `
         -body $solutionData

      Write-Host "$($solutionData.friendlyname) created successfully"
      $recordsToDelete += @{
         setName = 'solutions'
         id      = $solutionId
      }
   }
   else {
      Write-Host "$($solutionQueryResults[0].friendlyname) already exists"
      $solutionId = $solutionQueryResults[0].solutionid
   }

   #endregion Section 0: Create Publisher and Solution

   #region Section 1: Create Referenced Tables
   # The polymorphic lookup attribute will be on sample_Media.
   # It can reference records in sample_Book, sample_Audio, or sample_Video.
   #
   # Data model (from https://learn.microsoft.com/power-apps/developer/data-platform/webapi/multitable-lookup):
   #
   #  sample_Book:  sample_name (PK name), sample_callnumber
   #  sample_Audio: sample_name (PK name), sample_audioformat
   #  sample_Video: sample_name (PK name), sample_videoformat
   #  sample_Media: sample_name (PK name), sample_MediaPolymorphicLookup -> one of Book/Audio/Video

   #region Create sample_Book table

   $bookTableData = @{
      '@odata.type'         = 'Microsoft.Dynamics.CRM.EntityMetadata'
      SchemaName            = "$($publisherData.customizationprefix)_Book"
      DisplayName           = New-Label -label 'Book' -languageCode $languageCode
      DisplayCollectionName = New-Label -label 'Books' -languageCode $languageCode
      Description           = New-Label -label 'A table to store books in the media library' -languageCode $languageCode
      HasActivities         = $false
      HasNotes              = $false
      OwnershipType         = 'UserOwned'
      PrimaryNameAttribute  = "$($publisherData.customizationprefix)_name"
      Attributes            = @(
         (New-PrimaryNameAttribute `
            -prefix $publisherData.customizationprefix `
            -description 'The name (title) of the book' `
            -languageCode $languageCode),
         @{
            '@odata.type' = 'Microsoft.Dynamics.CRM.StringAttributeMetadata'
            SchemaName    = "$($publisherData.customizationprefix)_CallNumber"
            RequiredLevel = @{ Value = 'None' }
            DisplayName   = New-Label -label 'Call Number' -languageCode $languageCode
            Description   = New-Label -label 'The library call number for the book' -languageCode $languageCode
            MaxLength     = 50
         }
      )
   }

   $tableQuery = "?`$filter=SchemaName eq '$($bookTableData.SchemaName)'&`$select=SchemaName,MetadataId"
   $tableQueryResults = (Get-Tables -query $tableQuery).value

   if ($tableQueryResults.Length -eq 0) {
      $bookTableId = New-Table `
         -body $bookTableData `
         -solutionUniqueName $solutionData.uniquename

      Write-Host "Book table created successfully"
      $recordsToDelete += @{ setName = 'EntityDefinitions'; id = $bookTableId }
   }
   else {
      Write-Host "Book table already exists"
      $bookTableId = $tableQueryResults[0].MetadataId
   }

   #endregion Create sample_Book table

   #region Create sample_Audio table

   $audioTableData = @{
      '@odata.type'         = 'Microsoft.Dynamics.CRM.EntityMetadata'
      SchemaName            = "$($publisherData.customizationprefix)_Audio"
      DisplayName           = New-Label -label 'Audio' -languageCode $languageCode
      DisplayCollectionName = New-Label -label 'Audio' -languageCode $languageCode
      Description           = New-Label -label 'A table to store audio recordings in the media library' -languageCode $languageCode
      HasActivities         = $false
      HasNotes              = $false
      OwnershipType         = 'UserOwned'
      PrimaryNameAttribute  = "$($publisherData.customizationprefix)_name"
      Attributes            = @(
         (New-PrimaryNameAttribute `
            -prefix $publisherData.customizationprefix `
            -description 'The name (title) of the audio recording' `
            -languageCode $languageCode),
         @{
            '@odata.type' = 'Microsoft.Dynamics.CRM.StringAttributeMetadata'
            SchemaName    = "$($publisherData.customizationprefix)_AudioFormat"
            RequiredLevel = @{ Value = 'None' }
            DisplayName   = New-Label -label 'Audio Format' -languageCode $languageCode
            Description   = New-Label -label 'The format of the audio recording (e.g. mp3, wma)' -languageCode $languageCode
            MaxLength     = 20
         }
      )
   }

   $tableQuery = "?`$filter=SchemaName eq '$($audioTableData.SchemaName)'&`$select=SchemaName,MetadataId"
   $tableQueryResults = (Get-Tables -query $tableQuery).value

   if ($tableQueryResults.Length -eq 0) {
      $audioTableId = New-Table `
         -body $audioTableData `
         -solutionUniqueName $solutionData.uniquename

      Write-Host "Audio table created successfully"
      $recordsToDelete += @{ setName = 'EntityDefinitions'; id = $audioTableId }
   }
   else {
      Write-Host "Audio table already exists"
      $audioTableId = $tableQueryResults[0].MetadataId
   }

   #endregion Create sample_Audio table

   #region Create sample_Video table

   $videoTableData = @{
      '@odata.type'         = 'Microsoft.Dynamics.CRM.EntityMetadata'
      SchemaName            = "$($publisherData.customizationprefix)_Video"
      DisplayName           = New-Label -label 'Video' -languageCode $languageCode
      DisplayCollectionName = New-Label -label 'Videos' -languageCode $languageCode
      Description           = New-Label -label 'A table to store videos in the media library' -languageCode $languageCode
      HasActivities         = $false
      HasNotes              = $false
      OwnershipType         = 'UserOwned'
      PrimaryNameAttribute  = "$($publisherData.customizationprefix)_name"
      Attributes            = @(
         (New-PrimaryNameAttribute `
            -prefix $publisherData.customizationprefix `
            -description 'The name (title) of the video' `
            -languageCode $languageCode),
         @{
            '@odata.type' = 'Microsoft.Dynamics.CRM.StringAttributeMetadata'
            SchemaName    = "$($publisherData.customizationprefix)_VideoFormat"
            RequiredLevel = @{ Value = 'None' }
            DisplayName   = New-Label -label 'Video Format' -languageCode $languageCode
            Description   = New-Label -label 'The format of the video (e.g. wmv, avi)' -languageCode $languageCode
            MaxLength     = 20
         }
      )
   }

   $tableQuery = "?`$filter=SchemaName eq '$($videoTableData.SchemaName)'&`$select=SchemaName,MetadataId"
   $tableQueryResults = (Get-Tables -query $tableQuery).value

   if ($tableQueryResults.Length -eq 0) {
      $videoTableId = New-Table `
         -body $videoTableData `
         -solutionUniqueName $solutionData.uniquename

      Write-Host "Video table created successfully"
      $recordsToDelete += @{ setName = 'EntityDefinitions'; id = $videoTableId }
   }
   else {
      Write-Host "Video table already exists"
      $videoTableId = $tableQueryResults[0].MetadataId
   }

   #endregion Create sample_Video table

   #endregion Section 1: Create Referenced Tables

   #region Section 2: Create Referencing Table (sample_Media)
   # This table will hold the polymorphic lookup attribute that can point to
   # a record in sample_Book, sample_Audio, or sample_Video.

   $mediaTableData = @{
      '@odata.type'         = 'Microsoft.Dynamics.CRM.EntityMetadata'
      SchemaName            = "$($publisherData.customizationprefix)_Media"
      DisplayName           = New-Label -label 'Media' -languageCode $languageCode
      DisplayCollectionName = New-Label -label 'Media' -languageCode $languageCode
      Description           = New-Label -label 'A catalog table that references media items via a polymorphic lookup' -languageCode $languageCode
      HasActivities         = $false
      HasNotes              = $false
      OwnershipType         = 'UserOwned'
      PrimaryNameAttribute  = "$($publisherData.customizationprefix)_name"
      Attributes            = @(
         New-PrimaryNameAttribute `
            -prefix $publisherData.customizationprefix `
            -description 'The name of the media catalog entry' `
            -languageCode $languageCode
      )
   }

   $tableQuery = "?`$filter=SchemaName eq '$($mediaTableData.SchemaName)'&`$select=SchemaName,MetadataId"
   $tableQueryResults = (Get-Tables -query $tableQuery).value

   if ($tableQueryResults.Length -eq 0) {
      $mediaTableId = New-Table `
         -body $mediaTableData `
         -solutionUniqueName $solutionData.uniquename

      Write-Host "Media table created successfully"
      $recordsToDelete += @{ setName = 'EntityDefinitions'; id = $mediaTableId }
   }
   else {
      Write-Host "Media table already exists"
      $mediaTableId = $tableQueryResults[0].MetadataId
   }

   #endregion Section 2: Create Referencing Table

   #region Section 3: Create Polymorphic Lookup Attribute
   # Use the CreatePolymorphicLookupAttribute action to create a single lookup
   # attribute on sample_Media that can reference sample_Book, sample_Audio, or sample_Video.
   # See: https://learn.microsoft.com/power-apps/developer/data-platform/webapi/multitable-lookup

   $polymorphicLookupSchemaName = "$($publisherData.customizationprefix)_MediaPolymorphicLookup"

   # The relationship schema names follow the convention:
   # {ReferencingEntity}_{ReferencedEntity}
   $relBookSchemaName  = "$($publisherData.customizationprefix)_media_$($publisherData.customizationprefix)_book"
   $relAudioSchemaName = "$($publisherData.customizationprefix)_media_$($publisherData.customizationprefix)_audio"
   $relVideoSchemaName = "$($publisherData.customizationprefix)_media_$($publisherData.customizationprefix)_video"

   # Check if the relationship already exists to determine whether the polymorphic lookup was already created
   $relExistsQuery = "?`$filter=SchemaName eq '$relBookSchemaName'&`$select=SchemaName,MetadataId"
   $relExistsResults = (Get-Relationships -query $relExistsQuery -isManyToMany $false).value

   if ($relExistsResults.Length -eq 0) {

      $relationships = @(
         @{
            SchemaName        = $relBookSchemaName
            ReferencedEntity  = $bookTableData.SchemaName.ToLower()
            ReferencingEntity = $mediaTableData.SchemaName.ToLower()
         },
         @{
            SchemaName           = $relAudioSchemaName
            ReferencedEntity     = $audioTableData.SchemaName.ToLower()
            ReferencingEntity    = $mediaTableData.SchemaName.ToLower()
            CascadeConfiguration = @{
               Assign   = 'NoCascade'
               Delete   = 'RemoveLink'
               Merge    = 'NoCascade'
               Reparent = 'NoCascade'
               Share    = 'NoCascade'
               Unshare  = 'NoCascade'
            }
         },
         @{
            SchemaName           = $relVideoSchemaName
            ReferencedEntity     = $videoTableData.SchemaName.ToLower()
            ReferencingEntity    = $mediaTableData.SchemaName.ToLower()
            CascadeConfiguration = @{
               Assign   = 'NoCascade'
               Delete   = 'RemoveLink'
               Merge    = 'NoCascade'
               Reparent = 'NoCascade'
               Share    = 'NoCascade'
               Unshare  = 'NoCascade'
            }
         }
      )

      $lookup = @{
         '@odata.type'     = 'Microsoft.Dynamics.CRM.ComplexLookupAttributeMetadata'
         AttributeType     = 'Lookup'
         AttributeTypeName = @{ Value = 'LookupType' }
         SchemaName        = $polymorphicLookupSchemaName
         DisplayName       = New-Label -label 'Media' -languageCode $languageCode
         Description       = New-Label `
            -label       'Polymorphic lookup that can reference a Book, Audio, or Video record' `
            -languageCode $languageCode
      }

      $polymorphicResult = New-PolymorphicLookupColumn `
         -oneToManyRelationships $relationships `
         -lookup $lookup `
         -solutionUniqueName $solutionData.uniquename

      Write-Host "Polymorphic lookup attribute '$polymorphicLookupSchemaName' created successfully"
      Write-Host "  Attribute ID: $($polymorphicResult.AttributeId)"
      Write-Host "  Relationship IDs: $($polymorphicResult.RelationshipIds -join ', ')"
   }
   else {
      Write-Host "Polymorphic lookup '$polymorphicLookupSchemaName' already exists"
   }

   # Note: The polymorphic lookup attribute and its relationships are on sample_Media.
   # They will be deleted automatically when the sample_Media table is deleted in Section 7.

   # Retrieve the ReferencingEntityNavigationPropertyName for each relationship.
   # This is the name to use when setting the polymorphic lookup value via @odata.bind.
   # See: https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-navigation-properties
   $bookNavProp = (Get-Relationships `
      -query "?`$filter=SchemaName eq '$relBookSchemaName'&`$select=ReferencingEntityNavigationPropertyName" `
      -isManyToMany $false).value[0].ReferencingEntityNavigationPropertyName
   $audioNavProp = (Get-Relationships `
      -query "?`$filter=SchemaName eq '$relAudioSchemaName'&`$select=ReferencingEntityNavigationPropertyName" `
      -isManyToMany $false).value[0].ReferencingEntityNavigationPropertyName
   $videoNavProp = (Get-Relationships `
      -query "?`$filter=SchemaName eq '$relVideoSchemaName'&`$select=ReferencingEntityNavigationPropertyName" `
      -isManyToMany $false).value[0].ReferencingEntityNavigationPropertyName

   Write-Host "Navigation property names:"
   Write-Host "  Book:  $bookNavProp"
   Write-Host "  Audio: $audioNavProp"
   Write-Host "  Video: $videoNavProp"

   #endregion Section 3: Create Polymorphic Lookup Attribute

   #region Section 4: Create Sample Data Records
   # Demonstrate the polymorphic lookup by creating records in each referenced table,
   # then creating sample_Media records that point to them.
   #
   # Expected data model (from the documentation example):
   #
   #  sample_Book:  Content1 (1ww-3452), Content2 (a4e-87hw)
   #  sample_Audio: Content1 (mp4), Content3 (wma)
   #  sample_Video: Content3 (wmv), Content2 (avi)
   #
   #  sample_Media: MediaObjectOne  -> Book:Content1
   #                MediaObjectTwo  -> Audio:Content1
   #                MediaObjectThree -> Video:Content3
   #                MediaObjectFour  -> Audio:Content3

   # Retrieve entity set names so we can use them for data operations.
   # By default Dataverse generates entity set names as the plural of the logical name.
   $bookTable  = Get-Table -logicalName ($bookTableData.SchemaName.ToLower())  -query '?$select=EntitySetName'
   $audioTable = Get-Table -logicalName ($audioTableData.SchemaName.ToLower()) -query '?$select=EntitySetName'
   $videoTable = Get-Table -logicalName ($videoTableData.SchemaName.ToLower()) -query '?$select=EntitySetName'
   $mediaTable = Get-Table -logicalName ($mediaTableData.SchemaName.ToLower()) -query '?$select=EntitySetName'

   $bookSetName  = $bookTable.EntitySetName
   $audioSetName = $audioTable.EntitySetName
   $videoSetName = $videoTable.EntitySetName
   $mediaSetName = $mediaTable.EntitySetName

   Write-Host "`nEntity set names:"
   Write-Host "  Book:  $bookSetName"
   Write-Host "  Audio: $audioSetName"
   Write-Host "  Video: $videoSetName"
   Write-Host "  Media: $mediaSetName"

   # Create Book records
   $book1Id = New-Record -setName $bookSetName -body @{
      "$($publisherData.customizationprefix)_name"       = 'Content1'
      "$($publisherData.customizationprefix)_callnumber" = '1ww-3452'
   }
   Write-Host "Created Book record: Content1 (1ww-3452) - ID: $book1Id"
   $recordsToDelete += @{ setName = $bookSetName; id = $book1Id }

   $book2Id = New-Record -setName $bookSetName -body @{
      "$($publisherData.customizationprefix)_name"       = 'Content2'
      "$($publisherData.customizationprefix)_callnumber" = 'a4e-87hw'
   }
   Write-Host "Created Book record: Content2 (a4e-87hw) - ID: $book2Id"
   $recordsToDelete += @{ setName = $bookSetName; id = $book2Id }

   # Create Audio records
   $audio1Id = New-Record -setName $audioSetName -body @{
      "$($publisherData.customizationprefix)_name"        = 'Content1'
      "$($publisherData.customizationprefix)_audioformat" = 'mp4'
   }
   Write-Host "Created Audio record: Content1 (mp4) - ID: $audio1Id"
   $recordsToDelete += @{ setName = $audioSetName; id = $audio1Id }

   $audio2Id = New-Record -setName $audioSetName -body @{
      "$($publisherData.customizationprefix)_name"        = 'Content2'
      "$($publisherData.customizationprefix)_audioformat" = 'wma'
   }
   Write-Host "Created Audio record: Content2 (wma) - ID: $audio2Id"
   $recordsToDelete += @{ setName = $audioSetName; id = $audio2Id }

   # Create Video records
   $video1Id = New-Record -setName $videoSetName -body @{
      "$($publisherData.customizationprefix)_name"        = 'Content3'
      "$($publisherData.customizationprefix)_videoformat" = 'wmv'
   }
   Write-Host "Created Video record: Content3 (wmv) - ID: $video1Id"
   $recordsToDelete += @{ setName = $videoSetName; id = $video1Id }

   $video2Id = New-Record -setName $videoSetName -body @{
      "$($publisherData.customizationprefix)_name"        = 'Content2'
      "$($publisherData.customizationprefix)_videoformat" = 'avi'
   }
   Write-Host "Created Video record: Content2 (avi) - ID: $video2Id"
   $recordsToDelete += @{ setName = $videoSetName; id = $video2Id }

   # Create Media records using the polymorphic lookup.
   # The @odata.bind key uses the ReferencingEntityNavigationPropertyName retrieved
   # from each relationship definition — not a derived or assumed name.
   # See: https://learn.microsoft.com/power-apps/developer/data-platform/webapi/web-api-navigation-properties
   #
   # Each Media record references exactly one record from one of the three tables.

   $media1Id = New-Record -setName $mediaSetName -body @{
      "$($publisherData.customizationprefix)_name" = 'Media Object One'
      "${bookNavProp}@odata.bind"                  = "/$bookSetName($book1Id)"
   }
   Write-Host "Created Media record: MediaObjectOne -> Book:First Book - ID: $media1Id"
   $recordsToDelete += @{ setName = $mediaSetName; id = $media1Id }

   $media2Id = New-Record -setName $mediaSetName -body @{
      "$($publisherData.customizationprefix)_name" = 'Media Object Two'
      "${audioNavProp}@odata.bind"                 = "/$audioSetName($audio1Id)"
   }
   Write-Host "Created Media record: MediaObjectTwo -> Audio:First Audio - ID: $media2Id"
   $recordsToDelete += @{ setName = $mediaSetName; id = $media2Id }

   $media3Id = New-Record -setName $mediaSetName -body @{
      "$($publisherData.customizationprefix)_name" = 'Media Object Three'
      "${videoNavProp}@odata.bind"                 = "/$videoSetName($video1Id)"
   }
   Write-Host "Created Media record: MediaObjectThree -> Video:First Video - ID: $media3Id"
   $recordsToDelete += @{ setName = $mediaSetName; id = $media3Id }

   $media4Id = New-Record -setName $mediaSetName -body @{
      "$($publisherData.customizationprefix)_name" = 'Media Object Four'
      "${audioNavProp}@odata.bind"                 = "/$audioSetName($audio2Id)"
   }
   Write-Host "Created Media record: MediaObjectFour -> Audio:Second Audio - ID: $media4Id"
   $recordsToDelete += @{ setName = $mediaSetName; id = $media4Id }

   #endregion Section 4: Create Sample Data Records

   #region Section 5: Retrieve Sample Data
   # Query the Media table to show each record's polymorphic lookup value.
   # Include annotations to see the entity type of the referenced record.

   Write-Host "`n-- Retrieving Media records with polymorphic lookup values --"

   $mediaQuery = "?`$select=$($publisherData.customizationprefix)_name"
   $mediaQuery += ",_$($polymorphicLookupSchemaName.ToLower())_value"
   $mediaQuery += "&`$filter=$($publisherData.customizationprefix)_name ne null"
   $mediaQuery += "&`$top=10"

   $mediaResults = (Get-Records `
         -setName $mediaSetName `
         -query $mediaQuery).value

   Write-Host "`nMedia catalog entries:"
   foreach ($media in $mediaResults) {
      $mediaName     = $media."$($publisherData.customizationprefix)_name"
      $lookupId      = $media."_$($polymorphicLookupSchemaName.ToLower())_value"
      $lookupName    = $media."_$($polymorphicLookupSchemaName.ToLower())_value@OData.Community.Display.V1.FormattedValue"
      $lookupType    = $media."_$($polymorphicLookupSchemaName.ToLower())_value@Microsoft.Dynamics.CRM.lookuplogicalname"

      Write-Host "  $mediaName -> [$lookupType] $lookupName (ID: $lookupId)"
   }

   # Demonstrate that a lookup on Media with name 'Content1' retrieves records
   # from both Book and Audio tables.
   Write-Host "`nDemonstrating cross-table lookup: querying Media records where the referenced item is named 'Content1'"

   $content1BookQuery = "?`$select=$($publisherData.customizationprefix)_name"
   $content1BookQuery += "&`$filter=_$($polymorphicLookupSchemaName.ToLower())_value eq $book1Id"

   $content1AudioQuery = "?`$select=$($publisherData.customizationprefix)_name"
   $content1AudioQuery += "&`$filter=_$($polymorphicLookupSchemaName.ToLower())_value eq $audio1Id"

   $content1BookResults  = (Get-Records -setName $mediaSetName -query $content1BookQuery).value
   $content1AudioResults = (Get-Records -setName $mediaSetName -query $content1AudioQuery).value

   Write-Host "  Media records referencing Book 'Content1':"
   foreach ($r in $content1BookResults) {
      Write-Host "    - $($r."$($publisherData.customizationprefix)_name")"
   }

   Write-Host "  Media records referencing Audio 'Content1':"
   foreach ($r in $content1AudioResults) {
      Write-Host "    - $($r."$($publisherData.customizationprefix)_name")"
   }

   #endregion Section 5: Retrieve Sample Data

   #region Section 6: Export Managed Solution

   $solutionFile = Export-Solution -solutionName $solutionData.uniquename -managed $true
   $saveSolutionFilePath = "$(Get-location)\PolymorphicLookup\$($solutionData.uniquename).zip"
   [IO.File]::WriteAllBytes($saveSolutionFilePath, $solutionFile)
   Write-Host "Managed solution exported to $saveSolutionFilePath"
   $managedSolutionExported = $true

   #endregion Section 6: Export Managed Solution

   #region Section 7: Delete Sample Records
   # Records are deleted in the reverse order they were created.
   # Deleting the sample_Media table will also delete the polymorphic lookup
   # attribute (sample_MediaPolymorphicLookup) and its relationships.
   # Deleting the referenced tables removes all book, audio, and video records.

   if ($deleteCreatedRecords -and ($recordsToDelete.Length -gt 0)) {
      Write-Host "`nDeleting sample records..."

      for ($i = $recordsToDelete.Length - 1; $i -ge 0; $i--) {
         $recordToDelete = $recordsToDelete[$i]
         Remove-Record `
            -setName $recordToDelete.setName `
            -id $recordToDelete.id `
            -strongConsistency $true | Out-Null
         Write-Host "$($recordToDelete.setName) record with ID: $($recordToDelete.id) deleted."
      }
   }

   #endregion Section 7: Delete Sample Records

   #region Section 8: Import and Delete Managed Solution

   if ($deleteCreatedRecords -and $managedSolutionExported) {
      $importJobId = New-Guid
      Import-Solution `
         -customizationFile ([System.IO.File]::ReadAllBytes($saveSolutionFilePath)) `
         -overwriteUnmanagedCustomizations $false `
         -importJobId $importJobId
      Write-Host "Managed solution imported."

      $solutionQuery = "?`$filter=uniquename eq '$($solutionData.uniquename)'&`$select=solutionid"
      $solutionQueryResults = (Get-Records -setName 'solutions' -query $solutionQuery).value

      if ($solutionQueryResults.Length -eq 1) {
         $solutionId = $solutionQueryResults[0].solutionid
         Remove-Record -setName 'solutions' -id $solutionId | Out-Null
         Write-Host "Managed solution deleted."
      }
   }

   #endregion Section 8: Import and Delete Managed Solution
}
