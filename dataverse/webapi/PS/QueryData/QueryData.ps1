. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1

Connect 'https://yourorg.crm.dynamics.com/' # change this
$deleteCreatedRecords = $true # Set to false to keep sample records after execution

$recordsToDelete = [PsObject[]]@()

Invoke-DataverseCommands {

   Write-Host  '--Starting Query Data sample--'

   try {

   #region Setup: Create records to query

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

   #endregion Setup: Create records to query

   #region Run: Demonstrate query operations

   #region Section 1: Select specific properties

   $contactColumns = @('fullname', 'jobtitle', 'annualincome')
   # Formatted value annotation for annualincome
   $formattedIncome = 'annualincome@OData.Community.Display.V1.FormattedValue'
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
   Write-host `t"Annual Income (formatted): $($yvonneContact.$formattedIncome)"
   
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
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }


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
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }


   Write-Host `n'-- Use Operators --'

   $selectPart = '$select=' + ($contactColumns -join ',')
   # Using operators: contains and greater than
   $filterExpr = @(
      "contains(fullname,'(sample)')",
      "annualincome gt 55000",
      "_parentcustomerid_value eq $contosoAccountId"
   ) -join ' and '
   $filterPart = '$filter=' + [System.Uri]::EscapeDataString($filterExpr)

   $query = '?' + $selectPart + '&' + $filterPart

   $retrieveContactsWithOperatorsParams = @{
      setName = 'contacts'
      query   = $query
   }

   $contacts = Get-Records @retrieveContactsWithOperatorsParams

   Write-Host `n"Contacts with '(sample)' in name and income above `$55,000:"

   $contacts.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }


   Write-Host `n'-- Set Precedence --'

   $selectPart = '$select=' + ($contactColumns -join ',')
   # Using parentheses to control precedence
   $filterExpr = @(
      "contains(fullname,'(sample)')",
      "(contains(jobtitle,'senior') or annualincome gt 55000)",
      "_parentcustomerid_value eq $contosoAccountId"
   ) -join ' and '
   $filterPart = '$filter=' + [System.Uri]::EscapeDataString($filterExpr)

   $query = '?' + $selectPart + '&' + $filterPart

   $retrieveContactsWithPrecedenceParams = @{
      setName = 'contacts'
      query   = $query
   }

   $contacts = Get-Records @retrieveContactsWithPrecedenceParams

   $message = "Contacts with '(sample)' in name AND " +
      "(senior jobtitle OR income above `$55,000):"
   Write-Host `n$message

   $contacts.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }


   #endregion Section 2: Use query functions

   #region Section 3: Ordering and aliases

   Write-Host `n'-- Order Results --'

   $selectPart = '$select=' + ($contactColumns -join ',')
   $filterExpr = @(
      "contains(fullname,'(sample)')",
      "_parentcustomerid_value eq $contosoAccountId"
   ) -join ' and '
   $filterPart = '$filter=' + [System.Uri]::EscapeDataString($filterExpr)
   $orderPart = '$orderby=jobtitle asc, annualincome desc'

   $query = '?' + $selectPart + '&' + $filterPart + '&' + $orderPart

   $retrieveOrderedContactsParams = @{
      setName = 'contacts'
      query   = $query
   }

   $contacts = Get-Records @retrieveOrderedContactsParams

   Write-Host `n"Contacts ordered by jobtitle (Ascending) and annualincome (Descending):"

   $contacts.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }


   Write-Host `n'-- Parameterized Aliases --'

   # Parameterized aliases can be used as parameters in a query
   $selectPart = '$select=' + ($contactColumns -join ',')
   $filterPart = "`$filter=contains(@p1,'(sample)') and @p2 eq @p3"
   $orderPart = '$orderby=@p4 asc, @p5 desc'
   $aliasPart = "&@p1=fullname&@p2=_parentcustomerid_value" +
   "&@p3=$contosoAccountId&@p4=jobtitle&@p5=annualincome"

   $query = '?' + $selectPart + '&' + $filterPart + '&' + $orderPart + $aliasPart

   $retrieveOrderedContactsWithAliasParams = @{
      setName = 'contacts'
      query   = $query
   }

   $contacts = Get-Records @retrieveOrderedContactsWithAliasParams

   $message = "Contacts ordered by jobtitle (Ascending) and " +
   "annualincome (Descending) using parameter aliases:"
   Write-Host `n$message

   $contacts.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }

   #endregion Section 3: Ordering and aliases

   #region Section 4: Limit and count results

   Write-Host `n'-- Top Results --'

   $selectPart = '$select=' + ($contactColumns -join ',')
   $filterExpr = @(
      "contains(fullname,'(sample)')",
      "_parentcustomerid_value eq $contosoAccountId"
   ) -join ' and '
   $filterPart = '$filter=' + [System.Uri]::EscapeDataString($filterExpr)
   $topPart = '$top=5'

   $query = '?' + $selectPart + '&' + $filterPart + '&' + $topPart

   $retrieveTop5ContactsParams = @{
      setName = 'contacts'
      query   = $query
   }

   $contacts = Get-Records @retrieveTop5ContactsParams

   Write-Host `n"Contacts top 5 results:"

   $contacts.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }


   Write-Host `n'-- Result Count --'

   # Get a count of a collection without the data
   $getCountParams = @{
      setName = 'contacts'
      query   = '/$count'
   }

   $contactCount = Get-Records @getCountParams

   Write-Host `n"The contacts collection has $contactCount contacts."


   # Get a count along with the data by including $count=true
   $selectPart = '$select=' + ($contactColumns -join ',')
   $filterExpr = @(
      "contains(fullname,'(sample)')",
      "(contains(jobtitle,'senior') or contains(jobtitle,'manager'))",
      "_parentcustomerid_value eq $contosoAccountId"
   ) -join ' and '
   $filterPart = '$filter=' + [System.Uri]::EscapeDataString($filterExpr)
   $countPart = '$count=true'

   $query = '?' + $selectPart + '&' + $filterPart + '&' + $countPart

   $retrieveContactsWithCountParams = @{
      setName = 'contacts'
      query   = $query
   }

   $contacts = Get-Records @retrieveContactsWithCountParams

   Write-Host `n"$($contacts.'@odata.count') Contacts with 'senior' or 'manager' in job title:"

   $contacts.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }

   #endregion Section 4: Limit and count results

   #region Section 5: Pagination

   Write-Host `n'-- Pagination --'

   $selectPart = '$select=' + ($contactColumns -join ',')
   $filterExpr = @(
      "contains(fullname,'(sample)')",
      "_parentcustomerid_value eq $contosoAccountId"
   ) -join ' and '
   $filterPart = '$filter=' + [System.Uri]::EscapeDataString($filterExpr)
   $countPart = '$count=true'

   $query = '?' + $selectPart + '&' + $filterPart + '&' + $countPart

   $retrieveFirstPageParams = @{
      setName     = 'contacts'
      query       = $query
      maxPageSize = 4
   }

   $firstPage = Get-Records @retrieveFirstPageParams

   Write-Host `n"Contacts total: $($firstPage.'@odata.count')    Contacts per page: 4"
   Write-Host "Page 1 of 2:"

   $firstPage.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }


   # Get the next page using the nextLink
   if ($firstPage.'@odata.nextLink') {
      $secondPage = Get-NextLink -nextLink $firstPage.'@odata.nextLink' -maxPageSize 4

      Write-Host `n"Page 2 of 2:"

      $secondPage.value |
      Format-Table -AutoSize `
      @{ Label = 'Full Name'; Expression = { $_.fullname } },
      @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
      @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }
   }

   #endregion Section 5: Pagination

   #region Section 6: Expand results

   Write-Host `n'-- Expanding Results --'

   # 1) Expand using the 'primarycontactid' single-valued navigation property of account Contoso
   $retrieveAccountWithPrimaryContactParams = @{
      setName = 'accounts'
      id      = $contosoAccountId
      query   = "?`$select=name&`$expand=primarycontactid(`$select=$($contactColumns -join ','))"
   }

   $account = Get-Record @retrieveAccountWithPrimaryContactParams

   Write-Host `n"Account $($account.name) has the following primary contact person:"
   Write-Host "`tFullname: $($account.primarycontactid.fullname)"
   Write-Host "`tJobtitle: $($account.primarycontactid.jobtitle)"
   Write-Host "`tAnnualincome: $($account.primarycontactid.annualincome)"


   # 2) Expand using the 'account_primary_contact' partner property
   $contactSelect = "`$select=$($contactColumns -join ',')"
   $retrieveContactWithAccountsParams = @{
      setName = 'contacts'
      id      = $yvonneContactId
      query   = "?$contactSelect&`$expand=account_primary_contact(`$select=name)"
   }

   $contact = Get-Record @retrieveContactWithAccountsParams

   Write-Host `n"Contact '$($contact.fullname)' is the primary contact for the following accounts:"

   foreach ($account in $contact.account_primary_contact) {
      Write-Host "`t$($account.name)"
   }


   # 3) Expand using the collection-valued 'contact_customer_accounts' navigation property
   $contactSelect = "`$select=$($contactColumns -join ',')"
   $retrieveAccountContactsParams = @{
      setName = 'accounts'
      id      = $contosoAccountId
      query   = "?`$select=name&`$expand=contact_customer_accounts($contactSelect)"
   }

   $account = Get-Record @retrieveAccountContactsParams

   Write-Host `n"Account '$($account.name)' has the following contact customers:"

   $account.contact_customer_accounts |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }


   # 4) Expand using multiple navigation property types in a single request
   Write-Host `n'-- Expanding multiple property types in one request --'

   $contactSelect = "`$select=$($contactColumns -join ',')"
   $expands = @(
      "primarycontactid($contactSelect)",
      "contact_customer_accounts($contactSelect)",
      "Account_Tasks(`$select=subject,description)"
   )
   $retrieveAccountMultipleExpandsParams = @{
      setName = 'accounts'
      id      = $contosoAccountId
      query   = "?`$select=name&`$expand=$($expands -join ',')"
   }

   $account = Get-Record @retrieveAccountMultipleExpandsParams

   Write-Host `n"Account $($account.name) has the following primary contact person:"
   Write-Host "`tFullname: $($account.primarycontactid.fullname)"
   Write-Host "`tJobtitle: $($account.primarycontactid.jobtitle)"
   Write-Host "`tAnnualincome: $($account.primarycontactid.annualincome)"

   Write-Host `n"Account '$($account.name)' has the following contact customers:"

   $account.contact_customer_accounts |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }

   Write-Host `n"Account '$($account.name)' has the following tasks:"

   foreach ($task in $account.Account_Tasks) {
      Write-Host "`t$($task.subject)"
   }


   # 5) Nested expands of single-valued navigation properties
   $selectPart = '$select=subject'
   $filterExpr = "regardingobjectid_contact_task/_accountid_value eq $contosoAccountId"
   $filterPart = '$filter=' + [System.Uri]::EscapeDataString($filterExpr)
   $expandPart = '$expand=regardingobjectid_contact_task(' +
   '$select=fullname;$expand=parentcustomerid_account(' +
   '$select=name;$expand=createdby($select=fullname)))'

   $query = '?' + $selectPart + '&' + $filterPart + '&' + $expandPart

   $retrieveNestedExpandsParams = @{
      setName = 'tasks'
      query   = $query
   }

   $tasks = Get-Records @retrieveNestedExpandsParams

   Write-Host `n"Expanded values from Task:"

   # Display header
   $col1Width = -30
   $col2Width = -30
   $col3Width = -25
   $col4Width = -20

   $formatStr = "`t|{0,$col1Width}|{1,$col2Width}|{2,$col3Width}|{3,$col4Width}"
   Write-Host ($formatStr -f 'Subject', 'Contact', 'Account', 'Account CreatedBy')
   Write-Host ($formatStr -f ('-' * [Math]::Abs($col1Width)),
      ('-' * [Math]::Abs($col2Width)),
      ('-' * [Math]::Abs($col3Width)),
      ('-' * [Math]::Abs($col4Width)))

   foreach ($task in $tasks.value) {
      $subject = $task.subject
      $contactName = $task.regardingobjectid_contact_task.fullname
      $accountName = $task.regardingobjectid_contact_task.parentcustomerid_account.name
      $createdBy = $task.regardingobjectid_contact_task.parentcustomerid_account.createdby.fullname

      Write-Host ($formatStr -f $subject, $contactName, $accountName, $createdBy)
   }


   # 6) Nested $expand having both single-valued and collection-valued navigation properties
   $selectPart = '$select=name,accountid'
   $filterExpr = "accountid eq $contosoAccountId"
   $filterPart = '$filter=' + [System.Uri]::EscapeDataString($filterExpr)
   $expandPart = '$expand=Account_Tasks($select=subject,description),' +
   'contact_customer_accounts($select=fullname;' +
   '$expand=owninguser($select=fullname,systemuserid))'

   $query = '?' + $selectPart + '&' + $filterPart + '&' + $expandPart

   $retrieveNestedExpandMultipleParams = @{
      setName = 'accounts'
      query   = $query
   }

   $accounts = Get-Records @retrieveNestedExpandMultipleParams

   Write-Host `n"Expanded values from Accounts:"

   foreach ($account in $accounts.value) {
      Write-Host "`nAccount: $($account.name)"

      Write-Host ("`n`t|{0,-30}|" -f 'Account Task')
      Write-Host ("`t|{0,-30}|" -f ('-' * 30))

      foreach ($task in $account.Account_Tasks) {
         Write-Host ("`t|{0,-30}|" -f $task.subject)
      }

      Write-Host ("`n`t|{0,-30}|{1,-30}|" -f 'Contact', 'System User')
      Write-Host ("`t|{0,-30}|{1,-30}|" -f ('-' * 30), ('-' * 30))

      foreach ($contact in $account.contact_customer_accounts) {
         Write-Host ("`t|{0,-30}|{1,-30}|" -f $contact.fullname, $contact.owninguser.fullname)
      }
   }

   #endregion Section 6: Expand results

   #region Section 7: Aggregate results

   Write-Host `n'-- Aggregate Results --'

   # Get aggregated salary information about Contacts working for Contoso
   $aggregates = @(
      'annualincome with average as average',
      'annualincome with sum as total',
      'annualincome with min as minimum',
      'annualincome with max as maximum'
   )
   $applyPart = '$apply=aggregate(' + ($aggregates -join ',') + ')'

   $query = '?' + $applyPart

   $retrieveAggregatesParams = @{
      setName = "accounts($contosoAccountId)/contact_customer_accounts"
      query   = $query
   }

   $aggregates = Get-Records @retrieveAggregatesParams

   Write-Host `n"Aggregated Annual Income information for Contoso contacts:"
   $avgFormatted = 'average@OData.Community.Display.V1.FormattedValue'
   $totalFormatted = 'total@OData.Community.Display.V1.FormattedValue'
   $minFormatted = 'minimum@OData.Community.Display.V1.FormattedValue'
   $maxFormatted = 'maximum@OData.Community.Display.V1.FormattedValue'
   Write-Host "`tAverage income: $($aggregates.value[0].$avgFormatted)"
   Write-Host "`tTotal income: $($aggregates.value[0].$totalFormatted)"
   Write-Host "`tMinimum income: $($aggregates.value[0].$minFormatted)"
   Write-Host "`tMaximum income: $($aggregates.value[0].$maxFormatted)"

   #endregion Section 7: Aggregate results

   #region Section 8: FetchXML queries

   Write-Host `n'-- FetchXML --'

   # Use FetchXML to query for all contacts whose fullname contains '(sample)'
   $fetchXml = @"
<fetch>
   <entity name='contact'>
      <attribute name='fullname' />
      <attribute name='jobtitle' />
      <attribute name='annualincome' />
      <order descending='true' attribute='fullname' />
      <filter type='and'>
         <condition value='%(sample)%' attribute='fullname' operator='like' />
         <condition value='$contosoAccountId' attribute='parentcustomerid' operator='eq' />
      </filter>
   </entity>
</fetch>
"@

   # URL encode the FetchXML
   $encodedFetchXml = [System.Uri]::EscapeDataString($fetchXml)

   $retrieveWithFetchXmlParams = @{
      setName = 'contacts'
      query   = "?fetchXml=$encodedFetchXml"
   }

   $contacts = Get-Records @retrieveWithFetchXmlParams

   Write-Host `n"Contacts Fetched by fullname containing '(sample)':"

   $contacts.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }


   # Simple FetchXML Paging
   Write-Host `n'-- Simple Paging --'

   $fetchXmlPage2 = @"
<fetch count='4' page='2'>
   <entity name='contact'>
      <attribute name='fullname' />
      <attribute name='jobtitle' />
      <attribute name='annualincome' />
      <order descending='true' attribute='fullname' />
      <filter type='and'>
         <condition value='%(sample)%' attribute='fullname' operator='like' />
         <condition value='$contosoAccountId' attribute='parentcustomerid' operator='eq' />
      </filter>
   </entity>
</fetch>
"@

   $encodedFetchXmlPage2 = [System.Uri]::EscapeDataString($fetchXmlPage2)

   $retrieveWithFetchXmlPage2Params = @{
      setName = 'contacts'
      query   = "?fetchXml=$encodedFetchXmlPage2"
   }

   $contactsPage2 = Get-Records @retrieveWithFetchXmlPage2Params

   Write-Host `n"Contacts Fetched by fullname containing '(sample)' - Page 2:"

   $contactsPage2.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }


   # FetchXML paging with paging cookie
   Write-Host `n'-- Paging with PagingCookie --'

   $page = 1
   $count = 3

   $fetchXmlPaging = @"
<fetch count='$count' page='$page'>
   <entity name='contact'>
      <attribute name='fullname' />
      <attribute name='jobtitle' />
      <attribute name='annualincome' />
      <order descending='true' attribute='fullname' />
      <filter type='and'>
         <condition value='%(sample)%' attribute='fullname' operator='like' />
         <condition value='$contosoAccountId' attribute='parentcustomerid' operator='eq' />
      </filter>
   </entity>
</fetch>
"@

   $encodedFetchXmlPaging = [System.Uri]::EscapeDataString($fetchXmlPaging)

   $retrieveWithFetchXmlPagingParams = @{
      setName = 'contacts'
      query   = "?fetchXml=$encodedFetchXmlPaging"
   }

   $contactsPaging = Get-Records @retrieveWithFetchXmlPagingParams

   Write-Host `n"Paging with fetchxml cookie - Page ${page}:"

   $contactsPaging.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }

   # Loop through subsequent pages using the paging cookie
   while ($contactsPaging.'@Microsoft.Dynamics.CRM.morerecords' -eq $true) {
      $page++

      # Get the paging cookie from the response
      $pagingCookieXml = $contactsPaging.'@Microsoft.Dynamics.CRM.fetchxmlpagingcookie'

      # Parse the cookie XML and extract the pagingcookie attribute value
      [xml]$cookieDoc = $pagingCookieXml
      $pagingCookie = $cookieDoc.cookie.pagingcookie

      # Double URL decode the paging cookie attribute value
      $decodedOnce = [System.Uri]::UnescapeDataString($pagingCookie)
      $decodedPagingCookie = [System.Uri]::UnescapeDataString($decodedOnce)

      # Load the FetchXML as XML and update the page and paging-cookie attributes
      [xml]$fetchXmlDoc = $fetchXmlPaging
      $fetchXmlDoc.fetch.SetAttribute('page', $page)
      $fetchXmlDoc.fetch.SetAttribute('paging-cookie', $decodedPagingCookie)

      # Convert back to string
      $fetchXmlPaging = $fetchXmlDoc.OuterXml

      $encodedFetchXmlPaging = [System.Uri]::EscapeDataString($fetchXmlPaging)

      $retrieveWithFetchXmlPagingParams = @{
         setName = 'contacts'
         query   = "?fetchXml=$encodedFetchXmlPaging"
      }

      $contactsPaging = Get-Records @retrieveWithFetchXmlPagingParams

      Write-Host `n"Paging with fetchxml cookie - Page ${page}:"

      $contactsPaging.value |
      Format-Table -AutoSize `
      @{ Label = 'Full Name'; Expression = { $_.fullname } },
      @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
      @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }
   }

   #endregion Section 8: FetchXML queries

   #region Section 9: Use predefined queries

   Write-Host `n'-- Saved Query --'

   # Get the 'Active Accounts' Saved Query Id
   $getSavedQueryParams = @{
      setName = 'savedqueries'
      query   = "?`$select=name,savedqueryid&`$filter=name eq 'Active Accounts'"
   }

   $savedQuery = Get-Records @getSavedQueryParams

   if ($savedQuery.value.Count -gt 0) {
      $activeAccountsSavedQueryId = $savedQuery.value[0].savedqueryid

      # Get 3 accounts using the saved query
      $retrieveSavedQueryResultsParams = @{
         setName     = 'accounts'
         query       = "?savedQuery=$activeAccountsSavedQueryId"
         maxPageSize = 3
      }

      $activeAccounts = Get-Records @retrieveSavedQueryResultsParams

      Write-Host `n"Active Accounts:"

      $lineNum = 0
      $formattedPrimaryContact = '_primarycontactid_value@OData.Community.Display.V1.FormattedValue'
      foreach ($account in $activeAccounts.value) {
         $lineNum++
         $name = if ($account.name) { $account.name } else { 'NULL' }
         $primaryContact = if ($account.$formattedPrimaryContact) {
            $account.$formattedPrimaryContact
         }
         else { 'NULL' }
         $telephone = if ($account.telephone1) { $account.telephone1 } else { 'NULL' }

         Write-Host "`t$lineNum) $name, $primaryContact, $telephone"
      }
   }


   Write-Host `n'-- User Query --'

   # Create a user query
   $userQuery = @{
      name             = 'My User Query'
      description      = 'User query to display contact info.'
      querytype        = 0
      returnedtypecode = 'contact'
      fetchxml         = @"
<fetch>
   <entity name='contact'>
      <attribute name='fullname' />
      <attribute name='contactid' />
      <attribute name='jobtitle' />
      <attribute name='annualincome' />
      <order descending='false' attribute='fullname' />
      <filter type='and'>
         <condition value='%(sample)%' attribute='fullname' operator='like' />
         <condition value='%Manager%' attribute='jobtitle' operator='like' />
         <condition value='55000' attribute='annualincome' operator='gt' />
      </filter>
   </entity>
</fetch>
"@
   }

   $createUserQueryParams = @{
      setName = 'userqueries'
      body    = $userQuery
   }

   $myUserQueryId = New-Record @createUserQueryParams

   # To delete later
   $recordsToDelete += [PsObject]@{
      'description' = "User Query '$($userQuery.name)'"
      'setName'     = 'userqueries'
      'id'          = $myUserQueryId
   }

   Write-Host `n"User Query created with ID: $myUserQueryId"

   # Use the query to return results
   $retrieveUserQueryResultsParams = @{
      setName     = 'contacts'
      query       = "?userQuery=$myUserQueryId"
      maxPageSize = 3
   }

   $userQueryResults = Get-Records @retrieveUserQueryResultsParams

   Write-Host `n"Contacts Fetched by My User Query:"

   $userQueryResults.value |
   Format-Table -AutoSize `
   @{ Label = 'Full Name'; Expression = { $_.fullname } },
   @{ Label = 'Job Title'; Expression = { $_.jobtitle } },
   @{ Label = 'Annual Income'; Expression = { $_.$formattedIncome } }

   #endregion Section 9: Use predefined queries

   #endregion Run: Demonstrate query operations

   }
   catch {
      Write-Host "`nAn error occurred: $_" -ForegroundColor Red
      # Re-throw to preserve error details
      throw
   }
   finally {
      #region Cleanup: Delete sample records

      if ($deleteCreatedRecords -and $recordsToDelete.Count -gt 0) {
         Write-Host  `n'Deleting sample records...'

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
         Write-Host  `n'Sample records were not deleted. Set $deleteCreatedRecords to $true to enable automatic cleanup.'
      }

      #endregion Cleanup: Delete sample records
   }

   Write-Host  `n'--Query Data sample completed--'

}