. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1

Connect 'https://yourorg.crm.dynamics.com/' # change this
$deleteCreatedRecords = $true # change this if you want to keep the records created by this sample

$recordsToDelete = [PsObject[]]@()

Invoke-DataverseCommands {

   Write-Host  '--Starting Query Data sample--'

   #region Section 0: Create records to query

   $contosoAccount = @{
      name                      = 'Contoso, Ltd. (sample)'
      Account_Tasks             = @(
         @{
            subject     = 'Task 1 for Contoso, Ltd.'
            description = 'Task 1 for Contoso, Ltd. description'
         },
         @{
            subject     = 'Task 2 for Contoso, Ltd.'
            description = 'Task 2 for Contoso, Ltd. description'
         },
         @{
            subject     = 'Task 3 for Contoso, Ltd.'
            description = 'Task 3 for Contoso, Ltd. description'
         }
      )
      primarycontactid          = @{
         firstname     = 'Yvonne'
         lastname      = 'McKay (sample)'
         jobtitle      = 'Coffee Master'
         annualincome  = 45000
         Contact_Tasks = @(
            @{
               subject               = 'Task 1 for Yvonne McKay'
               description           = 'Task 1 for Yvonne McKay description'
               actualdurationminutes = 5
            },
            @{
               subject               = 'Task 2 for Yvonne McKay'
               description           = 'Task 2 for Yvonne McKay description'
               actualdurationminutes = 5
            },
            @{
               subject               = 'Task 3 for Yvonne McKay'
               description           = 'Task 3 for Yvonne McKay description'
               actualdurationminutes = 5
            }
         )
      }
      contact_customer_accounts = @(
         @{
            firstname     = 'Susanna'
            lastname      = 'Stubberod (sample)'
            jobtitle      = 'Senior Purchaser'
            annualincome  = 52000
            Contact_Tasks = @(
               @{
                  subject               = 'Task 1 for Susanna Stubberod'
                  description           = 'Task 1 for Susanna Stubberod description'
                  actualdurationminutes = 3
               },
               @{
                  subject               = 'Task 2 for Susanna Stubberod'
                  description           = 'Task 2 for Susanna Stubberod description'
                  actualdurationminutes = 3
               },
               @{
                  subject               = 'Task 3 for Susanna Stubberod'
                  description           = 'Task 3 for Susanna Stubberod description'
                  actualdurationminutes = 3
               }
            )
         },
         @{
            firstname     = 'Nancy'
            lastname      = 'Anderson (sample)'
            jobtitle      = 'Activities Manager'
            annualincome  = 55500
            Contact_Tasks = @(
               @{
                  subject               = 'Task 1 for Nancy Anderson'
                  description           = 'Task 1 for Nancy Anderson description'
                  actualdurationminutes = 4
               },
               @{
                  subject               = 'Task 2 for Nancy Anderson'
                  description           = 'Task 2 for Nancy Anderson description'
                  actualdurationminutes = 4
               },
               @{
                  subject               = 'Task 3 for Nancy Anderson'
                  description           = 'Task 3 for Nancy Anderson description'
                  actualdurationminutes = 4
               }
            )  
         },
         @{
            firstname     = 'Maria'
            lastname      = 'Cambell (sample)'
            jobtitle      = 'Accounts Manager'
            annualincome  = 31000
            Contact_Tasks = @(
               @{
                  subject               = 'Task 1 for Maria Cambell'
                  description           = 'Task 1 for Maria Cambell description'
                  actualdurationminutes = 5
               },
               @{
                  subject               = 'Task 2 for Maria Cambell'
                  description           = 'Task 2 for Maria Cambell description'
                  actualdurationminutes = 5
               },
               @{
                  subject               = 'Task 3 for Maria Cambell'
                  description           = 'Task 3 for Maria Cambell description'
                  actualdurationminutes = 5
               }
            )
         },
         @{
            firstname     = 'Scott'
            lastname      = 'Konersmann (sample)'
            jobtitle      = 'Accounts Manager'
            annualincome  = 38000
            Contact_Tasks = @(
               @{
                  subject               = 'Task 1 for Scott Konersmann'
                  description           = 'Task 1 for Scott Konersmann description'
                  actualdurationminutes = 6
               },
               @{
                  subject               = 'Task 2 for Scott Konersmann'
                  description           = 'Task 2 for Scott Konersmann description'
                  actualdurationminutes = 6
               },
               @{
                  subject               = 'Task 3 for Scott Konersmann'
                  description           = 'Task 3 for Scott Konersmann description'
                  actualdurationminutes = 6
               }
            )
         },
         @{
            firstname     = 'Robert'
            lastname      = 'Lyon (sample)'
            jobtitle      = 'Senior Technician'
            annualincome  = 78000
            Contact_Tasks = @(
               @{
                  subject               = 'Task 1 for Robert Lyon'
                  description           = 'Task 1 for Robert Lyon description'
                  actualdurationminutes = 7
               },
               @{
                  subject               = 'Task 2 for Robert Lyon'
                  description           = 'Task 2 for Robert Lyon description'
                  actualdurationminutes = 7
               },
               @{
                  subject               = 'Task 3 for Robert Lyon'
                  description           = 'Task 3 for Robert Lyon description'
                  actualdurationminutes = 7
               }
            )
         },
         @{
            firstname     = 'Paul'
            lastname      = 'Cannon (sample)'
            jobtitle      = 'Ski Instructor'
            annualincome  = 68500
            Contact_Tasks = @(
               @{
                  subject               = 'Task 1 for Paul Cannon'
                  description           = 'Task 1 for Paul Cannon description'
                  actualdurationminutes = 8
               },
               @{
                  subject               = 'Task 2 for Paul Cannon'
                  description           = 'Task 2 for Paul Cannon description'
                  actualdurationminutes = 8
               },
               @{
                  subject               = 'Task 3 for Paul Cannon'
                  description           = 'Task 3 for Paul Cannon description'
                  actualdurationminutes = 8
               }
            )
         },
         @{
            firstname     = 'Rene'
            lastname      = 'Valdes (sample)'
            jobtitle      = 'Data Analyst III'
            annualincome  = 86000
            Contact_Tasks = @(
               @{
                  subject               = 'Task 1 for Rene Valdes'
                  description           = 'Task 1 for Rene Valdes description'
                  actualdurationminutes = 9
               },
               @{
                  subject               = 'Task 2 for Rene Valdes'
                  description           = 'Task 2 for Rene Valdes description'
                  actualdurationminutes = 9
               },
               @{
                  subject               = 'Task 3 for Rene Valdes'
                  description           = 'Task 3 for Rene Valdes description'
                  actualdurationminutes = 9
               }
            )
         },
         @{
            firstname     = 'Jim'
            lastname      = 'Glynn (sample)'
            jobtitle      = 'Senior International Sales Manager'
            annualincome  = 81400
            Contact_Tasks = @(
               @{
                  subject               = 'Task 1 for Jim Glynn'
                  description           = 'Task 1 for Jim Glynn description'
                  actualdurationminutes = 10
               },
               @{
                  subject               = 'Task 2 for Jim Glynn'
                  description           = 'Task 2 for Jim Glynn description'
                  actualdurationminutes = 10
               },
               @{
                  subject               = 'Task 3 for Jim Glynn'
                  description           = 'Task 3 for Jim Glynn description'
                  actualdurationminutes = 10
               }
            )
         }
      )
   }

   $createContosoAccountParams = @{
      setName = 'accounts'
      body    = $contosoAccount
   }

   $contosoAccountId = New-Record @createContosoAccountParams
   # To delete later
   $recordsToDelete += [PsObject]@{
      'description' = "Account '$($contosoAccount.name)'"
      'setName'     = 'accounts'
      'id'          = $contosoAccountId
   }

   $retrieveYvonneParams = @{
      setName = 'accounts'
      id      = $contosoAccountId
      query   = '?$select=accountid&$expand=primarycontactid($select=contactid,fullname)'
   }

   $yvonneContactId = (Get-Record @retrieveYvonneParams).primarycontactid.contactid
   # To delete later
   $recordsToDelete += [PsObject]@{
      'description' = "Contact 'Yvonne McKay (sample)'"
      'setName'     = 'contacts'
      'id'          = $yvonneContactId
   }

   #endregion Section 0: Create records to query

   #region Section 1: Select specific properties

   $contactColumns = @('fullname', 'jobtitle', 'annualincome')
   $selectQuery = '?$select=' + ($contactColumns -join ',')
   $retrieveYvonneParams = @{      
      setName = 'contacts'
      id      = $yvonneContactId
      query   = $selectQuery
   }
   $yvonneContact = Get-Record @retrieveYvonneParams

   Write-Host `n"Contact basic info:"
   Write-host `t"Name: $($yvonneContact.fullname)"
   Write-host `t"Job Title: $($yvonneContact.jobtitle)"
   Write-host `t"Annual Income (unformatted): $($yvonneContact.annualincome)"
   Write-host `t"Annual Income (formatted): $($yvonneContact.'annualincome@OData.Community.Display.V1.FormattedValue')"
   
   #endregion Section 1: Select specific properties

   #region Section 2: Use query functions

   
   $selectPart = '$select=' + ($contactColumns -join ',')
   # OData filter expressions
   $filterExpr = @(
      "contains(fullname,'(sample)')",
      "_parentcustomerid_value eq $contosoAccountId"
   ) -join ' and '
   $filterPart = '$filter=' + [System.Uri]::EscapeDataString($filterExpr)

   $query = '?' + $selectPart + '&' + $filterPart

   $retrieveContactsWhereFullNameContainsSampleParams = @{
      setName = 'contacts'
      query   = $query
   }

   $contacts = Get-Records @retrieveContactsWhereFullNameContainsSampleParams

   Write-Host `n"Contoso contacts where Full Name contains '(sample)':"
   
   $contacts.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.'annualincome@OData.Community.Display.V1.FormattedValue' } }


   $selectPart = '$select=' + ($contactColumns -join ',')
   # OData filter expressions
   $filterExpr = @(
      "Microsoft.Dynamics.CRM.LastXHours(PropertyName=@p1,PropertyValue=@p2)",
      "_parentcustomerid_value eq $contosoAccountId"
   ) -join ' and '
   $filterParameters = "&@p1='createdon'&@p2='1'"
   $filterPart = '$filter=' + [System.Uri]::EscapeDataString($filterExpr) + $filterParameters

   $query = '?' + $selectPart + '&' + $filterPart

   $retrieveContactsCreatedInTheLastHourParams = @{
      setName = 'contacts'
      query   = $query
   }

   $contacts = Get-Records @retrieveContactsCreatedInTheLastHourParams

   Write-Host `n"Contoso contacts created in the last hour:"
   
   $contacts.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.'annualincome@OData.Community.Display.V1.FormattedValue' } }



   #endregion Section 2: Use query functions

   #region Section 3: Ordering and aliases
   #endregion Section 3: Ordering and aliases

   #region Section 4: Limit and count results
   #endregion Section 4: Limit and count results

   #region Section 5: Pagination
   #endregion Section 5: Pagination

   #region Section 6: Expand results
   #endregion Section 6: Expand results

   #region Section 7: Aggregate results
   #endregion Section 7: Aggregate results

   #region Section 8: FetchXML queries
   #endregion Section 8: FetchXML queries

   #region Section 9: Use predefined queries
   #endregion Section 9: Use predefined queries

   #region Section 10: Delete sample records

   if ($deleteCreatedRecords) {
      Write-Host  `n'Deleting sample records...'

      foreach ($record in $recordsToDelete) {

         Write-Host "`tDeleting $($record.description)"
         Remove-Record -setName $record.SetName -id $record.Id
      }

      Write-Host 'Sample records deleted.'
   }

   #endregion Section 10: Delete sample records
   

   Write-Host  `n'--Query Data sample completed--'

}