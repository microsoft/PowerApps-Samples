# Sample functions

function AllEnvironmentsPolicyTests
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentDisplayName,

        [Parameter(Mandatory = $false)]
        [string]$PolicyDisplayName = "Test policy for AllEnvironments"
    )
    process 
    {
        try
        {
            Write-Host "`r`nAllEnvironmentsPolicyTests started`r`n"

            CleanV1TestPolicies
        
            Write-Host "Create a new policy for AllEnvironments"
            $response = New-DlpPolicy -DisplayName $PolicyDisplayName -EnvironmentType "AllEnvironments"
            StringsAreEqual -Expect $PolicyDisplayName -Actual $response.displayName
        
            Write-Host "Add connector groups"
            $response.connectorGroups = CreateConnectorGroups
            $response = Set-DlpPolicy -PolicyName $response.name -UpdatedPolicy $response
            IsNotNull($response.Internal.connectorGroups | Where-Object { $_.classification -eq 'General' })
            IsNotNull($response.Internal.connectorGroups | Where-Object { $_.classification -eq 'Confidential' })
            IsNotNull($response.Internal.connectorGroups | Where-Object { $_.classification -eq 'Blocked' })
        
            ListGetUpdateRemovePolicy -PolicyDisplayName $PolicyDisplayName -EnvironmentType "AllEnvironments"

            Write-Host "Add the same connector to all groups test"
            $newPolicy = [pscustomobject]@{
                displayName = $PolicyDisplayName
                defaultConnectorsClassification = "General"
                connectorGroups = CreateConnectorGroupsForDuplicatedConnector
                environmentType = $EnvironmentType
                environments = @()
                etag = $null
                }

            $response = New-DlpPolicy -NewPolicy $newPolicy
            StringsAreEqual -Expect "Bad Request" -Actual $response.StatusDescription

            Write-Host "`r`nAllEnvironmentsPolicyTests completed"
        } catch {
            WriteStack
        }
    }
}

function ChangeOnlyToExceptEnvironmentsPolicyTests
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentDisplayName,

        [Parameter(Mandatory = $false)]
        [string]$PolicyDisplayName = "Test policy for OnlyEnvironments"
    )
    process 
    {
        try
        {
            Write-Host "`r`nChangeOnlyToExceptEnvironmentsPolicyTests started`r`n"

            # remove the test environments
            CleanTestEnvironments -EnvironmentDisplayName $EnvironmentDisplayName

            CleanV1TestPolicies
        
            $environment = CreateEnvironmentWithoutCDSDatabase -EnvironmentDisplayName $EnvironmentDisplayName
            if($environment -eq $null)
            {
                throw "CreateEnvironment failed."
            }

            $newPolicy = CreatePolicyObject -EnvironmentType "OnlyEnvironments" -PolicyDisplayName $PolicyDisplayName
            $environment = [pscustomobject]@{
                id = "/providers/admin/environment"
                name = $environment.EnvironmentName
                type = "/providers/dummyEnvironments"
            }

            $newPolicy.environments += $environment
        
            Write-Host "Create a new policy for OnlyEnvironments"
            $response = New-DlpPolicy -NewPolicy $newPolicy
            StringsAreEqual -Expect $PolicyDisplayName -Actual $response.displayName

            $ExceptEnvironmentsPolicyDisplayName = "ExceptEnvironments policy"
            $newPolicy = CreatePolicyObject -EnvironmentType "ExceptEnvironments" -PolicyDisplayName $ExceptEnvironmentsPolicyDisplayName
            $environment = [pscustomobject]@{
                id = "/providers/admin/environment"
                name = $environment.EnvironmentName
                type = "/providers/dummyEnvironments"
            }

            $newPolicy.environments += $environment
        
            Write-Host "Create a new policy for ExceptEnvironments by using the same environment for OnlyEnvironments"
            $exceptEnvironmentsResponse = New-DlpPolicy -NewPolicy $newPolicy
            StringsAreEqual -Expect "Bad Request" -Actual $exceptEnvironmentsResponse.StatusDescription

            Write-Host "Change from EnvironmentType from Only to Except"
            $updatedPolicy = [pscustomobject]$response
            $updatedPolicy.environmentType = "ExceptEnvironments"
            $response = Set-DlpPolicy -PolicyName $updatedPolicy.name -UpdatedPolicy $updatedPolicy
            StringsAreEqual -Expect "ExceptEnvironments" -Actual $response.Internal.environmentType

            Write-Host "Remove the test policy"
            $response = CheckHttpResponse(Remove-DlpPolicy -PolicyName $updatedPolicy.name)
            StringsAreEqual -Expect "OK" -Actual $response.Description

            #Change EnvironmentType from Only to Except
            Write-Host "`r`nChangeOnlyToExceptEnvironmentsPolicyTests completed"
        } catch {
            WriteStack
        }            
    }
}

function SingleEnvironmentPolicyTests
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentDisplayName,

        [Parameter(Mandatory = $false)]
        [string]$PolicyDisplayName = "Test policy for SingleEnvironment",

        [Parameter(Mandatory = $false)]
        [string]$EndPoint,

        [Parameter(Mandatory = $false)]
        [string]$TenantAdminName,

        [Parameter(Mandatory = $false)]
        [string]$TenantAdminPassword,

        [Parameter(Mandatory = $false)]
        [string]$EnvironmentAdminName,

        [Parameter(Mandatory = $false)]
        [string]$EnvironmentAdminPassword
    )
    process 
    {
        try
        {
            Write-Host "`r`nSingleEnvironmentPolicyTests started`r`n"

            # remove the test environments
            CleanTestEnvironments -EnvironmentDisplayName $EnvironmentDisplayName

            # remove the test policies
            CleanV1TestPolicies
        
            Write-Host "Change to a user EnvironmentAdmin"
            $Password = ConvertTo-SecureString $EnvironmentAdminPassword -AsPlainText -Force
            Add-PowerAppsAccount -Endpoint $EndPoint -Username $EnvironmentAdminName -Password $Password

            $EnvironmentDisplayName = $EnvironmentDisplayName + " for NonAdmin"
            $environment = CreateEnvironmentWithoutCDSDatabase -EnvironmentDisplayName $EnvironmentDisplayName
            if($environment -eq $null)
            {
                throw "CreateEnvironment failed."
            }

            $newPolicy = CreatePolicyObject -EnvironmentType "SingleEnvironment" -PolicyDisplayName $PolicyDisplayName
            $environment = [pscustomobject]@{
                id = "/providers/admin/environment"
                name = $environment.EnvironmentName
                type = "/providers/dummyEnvironments"
            }

            $newPolicy.environments += $environment
        
            Write-Host "Create a new policy for SingleEnvironment"
            $response = New-DlpPolicy -NewPolicy $newPolicy
            StringsAreEqual -Expect $PolicyDisplayName -Actual $response.displayName

            ListGetUpdateRemovePolicy -PolicyDisplayName $PolicyDisplayName -EnvironmentType "SingleEnvironment"

            Write-Host "Change user back to GlobalAdmin"
            $Password = ConvertTo-SecureString $TenantAdminPassword -AsPlainText -Force
            Add-PowerAppsAccount -Endpoint $EndPoint -Username $TenantAdminName -Password $Password

            Write-Host "`r`nSingleEnvironmentPolicyTests completed"
        } catch {
            WriteStack
        }
    }
}

function CreateListMultiplePoliciesTests
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentDisplayName
    )
    process 
    {
        try
        {
            Write-Host "`r`nCreateListMultiplePoliciesTests started`r`n"

            CleanV1TestPolicies
        
            $AllEnvironmentsPolicyDisplayName = "AllEnvironments policy"
            $newPolicy = CreatePolicyObject -EnvironmentType "AllEnvironments" -PolicyDisplayName $AllEnvironmentsPolicyDisplayName
            Write-Host "Create a new policy for AllEnvironments"
            $response = New-DlpPolicy -NewPolicy $newPolicy
            StringsAreEqual -Expect $AllEnvironmentsPolicyDisplayName -Actual $response.displayName
        
            $OnlyEnvironmentsPolicyDisplayName = "OnlyEnvironments policy"
            $newPolicy = CreatePolicyObject -EnvironmentType "OnlyEnvironments" -PolicyDisplayName $OnlyEnvironmentsPolicyDisplayName
            $environmentName = New-Guid
            $environment = [pscustomobject]@{
                id = "/providers/admin/environment"
                name = $environmentName
                type = "/providers/dummyEnvironments"
            }

            $newPolicy.environments += $environment
        
            Write-Host "Create a new policy for OnlyEnvironments"
            $response = New-DlpPolicy -NewPolicy $newPolicy
            StringsAreEqual -Expect $OnlyEnvironmentsPolicyDisplayName -Actual $response.displayName
        
            $ExceptEnvironmentsPolicyDisplayName = "ExceptEnvironments policy"
            $newPolicy = CreatePolicyObject -EnvironmentType "ExceptEnvironments" -PolicyDisplayName $ExceptEnvironmentsPolicyDisplayName
            $environmentName = New-Guid
            $environment = [pscustomobject]@{
                id = "/providers/admin/environment"
                name = $environmentName
                type = "/providers/dummyEnvironments"
            }

            $newPolicy.environments += $environment
        
            Write-Host "Create a new policy for ExceptEnvironments"
            $response = New-DlpPolicy -NewPolicy $newPolicy
            StringsAreEqual -Expect $ExceptEnvironmentsPolicyDisplayName -Actual $response.displayName
        
            $policies = Get-DlpPolicy

            Write-Host "Check and remove all created policies"

            foreach ($policy in $Policies.Value)
            {
                if ($policy.displayName -eq $AllEnvironmentsPolicyDisplayName -or
                    $policy.displayName -eq $OnlyEnvironmentsPolicyDisplayName -or
                    $policy.displayName -eq $ExceptEnvironmentsPolicyDisplayName)
                {
                    $response = CheckHttpResponse(Remove-DlpPolicy -PolicyName $policy.name)
                    StringsAreEqual -Expect "OK" -Actual $response.Description
                }
                else
                {
                    throw "CreateListMultiplePoliciesTests fails ($policy.displayName)"
                }
            }

            Write-Host "`r`nCreateListMultiplePoliciesTests completed"
        } catch {
            WriteStack
        }
    }
}

function MoveConnectorCrossGroupsTests
{
    param
    (
        [Parameter(Mandatory = $false)]
        [string]$PolicyDisplayName = "Test policy for moving connectors"
    )
    process 
    {
        try
        {
            Write-Host "`r`nMoveConnectorCrossGroupsTests started`r`n"
        
            CleanV1TestPolicies

            $newPolicy = CreatePolicyObject -EnvironmentType "AllEnvironments" -PolicyDisplayName $PolicyDisplayName
            $response = New-DlpPolicy -NewPolicy $newPolicy
            StringsAreEqual -Expect $PolicyDisplayName -Actual $response.displayName
            $confidentialGroup = $response.connectorGroups | Where-Object { $_.classification -eq 'Confidential' }
            $blockedGroup = $response.connectorGroups | Where-Object { $_.classification -eq 'Blocked' }
            IntAreEqual -Expect 3 -Actual $confidentialGroup.connectors.Count
            IntAreEqual -Expect 1 -Actual $blockedGroup.connectors.Count

            Write-Host "Move a connector from Confidential group to Blocked group."
            $connector = $confidentialGroup.connectors[0]
        
            # remove Confidential group from connectorGroups
            $response.connectorGroups = $response.connectorGroups -ne $confidentialGroup

            # remove Blocked group from connectorGroups
            $response.connectorGroups = $response.connectorGroups -ne $blockedGroup

            # remove the connector from Confidential group
            $confidentialConnectors = $confidentialGroup.connectors -ne $connector
            $confidentialGroup.connectors = $confidentialConnectors

            # add the connector to Blocked group
            $blockedGroup.connectors += $connector

            # add Confidential group back
            $response.connectorGroups += $confidentialGroup
            # add blocked group back
            $response.connectorGroups += $blockedGroup

            # update policy
            $response = Set-DlpPolicy -PolicyName $response.name -UpdatedPolicy $response
            $confidentialGroup = $response.Internal.connectorGroups | Where-Object { $_.classification -eq 'Confidential' }
            $blockedGroup = $response.Internal.connectorGroups | Where-Object { $_.classification -eq 'Blocked' }

            # Confidential group number of connectors changed from 3 to 2
            IntAreEqual -Expect 2 -Actual $confidentialGroup.connectors.Count
            # Blocked group number of connectors changed from 1 to 2
            IntAreEqual -Expect 2 -Actual $blockedGroup.connectors.Count

            Write-Host "`r`nMoveConnectorCrossGroupsTests completed"
        } catch {
            WriteStack
        }
    }
}

function OldAPIToNewAPICompatibilityTests
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentDisplayName
    )
    process 
    {
        try
        {
            Write-Host "`r`nOldAPIToNewAPICompatibilityTests started`r`n"

            # remove the test environments
            CleanTestEnvironments -EnvironmentDisplayName $EnvironmentDisplayName
            
            CleanV1TestPolicies
        
            $AllEnvironmentsPolicyDisplayName = "AllEnvironments policy"
            $newPolicy = CreatePolicyObject -EnvironmentType "AllEnvironments" -PolicyDisplayName $AllEnvironmentsPolicyDisplayName
            Write-Host "Create a new policy for AllEnvironments"
            $newCreatedPolicy = New-DlpPolicy -NewPolicy $newPolicy
            StringsAreEqual -Expect $AllEnvironmentsPolicyDisplayName -Actual $newCreatedPolicy.displayName

            # define test data for old API
            $TenantPolicyTestDisplayName = "TenantPolicyDemo"
            $EnvironmentPolicyDisplayName = "EnvironmentPolicyDemo"
            $NonBusinessConnectorName = "Amazon Redshift"
            $NonBusinessConnectorId = "/providers/Microsoft.PowerApps/apis/shared_amazonredshift"
            $NonBusinessConnectorType = "Microsoft.PowerApps/apis"
            $BusinessConnectorName = "Dropbox"
            $BusinessConnectorId = "/providers/Microsoft.PowerApps/apis/shared_dropbox"

            [Parameter(Mandatory = $false)]
            [string]$BusinessConnectorType = "Microsoft.PowerApps/apis"

            # create test tenant policy
            Write-Host "Create an old tenant policy"
            $tenantPolicy = New-AdminDlpPolicy -DisplayName $TenantPolicyTestDisplayName
            StringsAreEqual -Expect $TenantPolicyTestDisplayName -Actual $tenantPolicy.DisplayName

            <# Update test tenant policy #>
            $tenantPolicyResponse = Set-AdminDlpPolicy -PolicyName $tenantPolicy.PolicyName -SetNonBusinessDataGroupState "Block"
            StringsAreEqual -Expect "Block" -Actual $tenantPolicyResponse.Internal.properties.definition.rules.apiGroupRule.actions.blockAction.type
            $apiGroupRule = ConvertTo-Json $tenantPolicyResponse.Internal.properties.definition.rules.apiGroupRule
            Write-Verbose "Test tenant policy apiGroupRule added:`r`n$apiGroupRule"

            # Add customer connector to BusinessData group of tenant policy
            $response = CheckHttpResponse(Add-CustomConnectorToPolicy -PolicyName $tenantPolicy.PolicyName -ConnectorName $BusinessConnectorName -ConnectorId $BusinessConnectorId -ConnectorType $BusinessConnectorType -GroupName hbi)
            $content = ConvertFrom-Json $response.Internal.Content
            StringsAreEqual -Expect $BusinessConnectorName -Actual $content.properties.definition.apiGroups.hbi.apis[0].name

            # Add customer connector to Non-BusinessData group of tenant policy
            $response = CheckHttpResponse(Add-CustomConnectorToPolicy -PolicyName $tenantPolicy.PolicyName -ConnectorName $NonBusinessConnectorName -ConnectorId $NonBusinessConnectorId -ConnectorType $NonBusinessConnectorType -GroupName lbi)
            $content = ConvertFrom-Json $response.Internal.Content
            StringsAreEqual -Expect $NonBusinessConnectorName -Actual $content.properties.definition.apiGroups.lbi.apis[0].name

            $updatedTenantPolicy = $content

            # create test environment policy
            Write-Host "Create an old environment policy"
            $environment = CreateEnvironmentWithoutCDSDatabase -EnvironmentDisplayName $EnvironmentDisplayName
            if($environment -eq $null)
            {
                throw "CreateEnvironment failed."
            }

            $envirnmentPolicy = New-AdminDlpPolicy -DisplayName $EnvironmentPolicyDisplayName -EnvironmentName $environment.EnvironmentName
            StringsAreEqual -Expect $EnvironmentPolicyDisplayName -Actual $envirnmentPolicy.DisplayName

            <# update test environment policy #>
            $envirnmentPolicyResponse = Set-AdminDlpPolicy -PolicyName $envirnmentPolicy.PolicyName -EnvironmentName $environment.EnvironmentName -SetNonBusinessDataGroupState "Block"
            StringsAreEqual -Expect "Block" -Actual $envirnmentPolicyResponse.Internal.properties.definition.rules.apiGroupRule.actions.blockAction.type
            $apiGroupRule = ConvertTo-Json $envirnmentPolicyResponse.Internal.properties.definition.rules.apiGroupRule
            Write-Verbose "Test environment policy apiGroupRule added:`r`n$apiGroupRule"

            # Add customer connector to BusinessData group of environment policy
            $response = CheckHttpResponse(Add-CustomConnectorToPolicy -PolicyName $envirnmentPolicy.PolicyName -EnvironmentName $environment.EnvironmentName -ConnectorName $BusinessConnectorName -ConnectorId $BusinessConnectorId -ConnectorType $BusinessConnectorType -GroupName hbi)
            $content = ConvertFrom-Json $response.Internal.Content
            StringsAreEqual -Expect $BusinessConnectorName -Actual $content.properties.definition.apiGroups.hbi.apis[0].name

            # Add customer connector to Non-BusinessData group of environment policy
            $response = CheckHttpResponse(Add-CustomConnectorToPolicy -PolicyName $envirnmentPolicy.PolicyName -EnvironmentName $environment.EnvironmentName -ConnectorName $NonBusinessConnectorName -ConnectorId $NonBusinessConnectorId -ConnectorType $NonBusinessConnectorType -GroupName lbi)
            $content = ConvertFrom-Json $response.Internal.Content
            StringsAreEqual -Expect $NonBusinessConnectorName -Actual $content.properties.definition.apiGroups.lbi.apis[0].name

            $updatedEnvirnmentPolicy = $content

            # using new API to list and remove policies
            $policies = Get-DlpPolicy

            Write-Host "Check and remove all created policies"

            foreach ($policy in $Policies.Value)
            {
                $generalGroup = $policy.connectorGroups | Where-Object { $_.classification -eq 'General' }
                $confidentialGroup = $policy.connectorGroups | Where-Object { $_.classification -eq 'Confidential' }
                $blockedGroup = $policy.connectorGroups | Where-Object { $_.classification -eq 'Blocked' }

                if ($policy.displayName -eq $TenantPolicyTestDisplayName)
                {
                    StringsAreEqual -Expect $updatedTenantPolicy.properties.CreatedBy.displayName -Actual $policy.CreatedBy.displayName
                    StringsAreEqual -Expect $updatedTenantPolicy.properties.createdTime -Actual $policy.createdTime
                    StringsAreEqual -Expect $updatedTenantPolicy.properties.lastModifiedBy.displayName -Actual $policy.lastModifiedBy.displayName
                    StringsAreEqual -Expect $updatedTenantPolicy.properties.lastModifiedTime -Actual $policy.lastModifiedTime

                    # connector group check, lbi connector moves to blocked group, hbi connector moves to general group
                    StringsAreEqual -Expect $updatedTenantPolicy.properties.definition.apiGroups.lbi.apis[0].id -Actual $blockedGroup.connectors[0].id
                    StringsAreEqual -Expect $updatedTenantPolicy.properties.definition.apiGroups.lbi.apis[0].name -Actual $blockedGroup.connectors[0].name
                    StringsAreEqual -Expect $updatedTenantPolicy.properties.definition.apiGroups.lbi.apis[0].type -Actual $blockedGroup.connectors[0].type
                    StringsAreEqual -Expect $updatedTenantPolicy.properties.definition.apiGroups.hbi.apis[0].id -Actual $generalGroup.connectors[0].id
                    StringsAreEqual -Expect $updatedTenantPolicy.properties.definition.apiGroups.hbi.apis[0].name -Actual $generalGroup.connectors[0].name
                    StringsAreEqual -Expect $updatedTenantPolicy.properties.definition.apiGroups.hbi.apis[0].type -Actual $generalGroup.connectors[0].type

                    StringsAreEqual -Expect "Microsoft.BusinessAppPlatform/scopes/apiPolicies" -Actual $tenantPolicy.Type
                    StringsAreEqual -Expect "AllEnvironments" -Actual $policy.environmentType

                    # update policy displayName
                    $updatedDisplayName = "Updated old API test policy"
                    $policy.displayName = $updatedDisplayName
                    $response = Set-DlpPolicy -PolicyName $policy.name -UpdatedPolicy $policy
                    StringsAreEqual -Expect $updatedDisplayName -Actual $response.Internal.displayName
                }
                elseif ($policy.displayName -eq $AllEnvironmentsPolicyDisplayName)
                {
                    StringsAreEqual -Expect $newCreatedPolicy.CreatedBy.displayName -Actual $policy.CreatedBy.displayName
                    StringsAreEqual -Expect $newCreatedPolicy.createdTime -Actual $policy.createdTime
                    StringsAreEqual -Expect $newCreatedPolicy.lastModifiedBy.displayName -Actual $policy.lastModifiedBy.displayName
                    StringsAreEqual -Expect $newCreatedPolicy.lastModifiedTime -Actual $policy.lastModifiedTime
                    StringsAreEqual -Expect $newCreatedPolicy.etag -Actual $policy.etag

                    $newCreatedGeneralGroup = $newCreatedPolicy.connectorGroups | Where-Object { $_.classification -eq 'General' }
                    $newCreatedConfidentialGroup = $newCreatedPolicy.connectorGroups | Where-Object { $_.classification -eq 'Confidential' }
                    $newCreatedBlockedGroup = $newCreatedPolicy.connectorGroups | Where-Object { $_.classification -eq 'Blocked' }
                    StringsAreEqual -Expect $newCreatedGeneralGroup.connectors[0].id -Actual $generalGroup.connectors[0].id
                    StringsAreEqual -Expect $newCreatedConfidentialGroup.connectors[0].id -Actual $confidentialGroup.connectors[0].id
                    StringsAreEqual -Expect $newCreatedConfidentialGroup.connectors[1].id -Actual $confidentialGroup.connectors[1].id
                    StringsAreEqual -Expect $newCreatedBlockedGroup.connectors[0].id -Actual $blockedGroup.connectors[0].id

                    $upatedDisplayName = "Updated new API test policy"
                    Write-Host "Upate policy to $upatedDisplayName"
                    $updatedPolicy = [pscustomobject]$policy
                    $updatedPolicy.displayName = $upatedDisplayName
                    $response = Set-DlpPolicy -PolicyName $updatedPolicy.name -UpdatedPolicy $updatedPolicy
                    StringsAreEqual -Expect $upatedDisplayName -Actual $response.Internal.displayName
                }
                elseif ($policy.displayName -eq $EnvironmentPolicyDisplayName)
                {
                    StringsAreEqual -Expect $updatedEnvirnmentPolicy.properties.CreatedBy.displayName -Actual $policy.CreatedBy.displayName
                    StringsAreEqual -Expect $updatedEnvirnmentPolicy.properties.createdTime -Actual $policy.createdTime
                    StringsAreEqual -Expect $updatedEnvirnmentPolicy.properties.lastModifiedBy.displayName -Actual $policy.lastModifiedBy.displayName
                    StringsAreEqual -Expect $updatedEnvirnmentPolicy.properties.lastModifiedTime -Actual $policy.lastModifiedTime
                    StringsAreEqual -Expect $envirnmentPolicy.Environments[0].name -Actual $policy.Environments[0].name
                    StringsAreEqual -Expect $envirnmentPolicy.Environments[0].id -Actual $policy.Environments[0].id
                    StringsAreEqual -Expect $envirnmentPolicy.Environments[0].type -Actual $policy.Environments[0].type

                    # connector group check, lbi connector moves to blocked group, hbi connector moves to general group
                    StringsAreEqual -Expect "Blocked" -Actual $blockedGroup.classification
                    StringsAreEqual -Expect $updatedEnvirnmentPolicy.properties.definition.apiGroups.lbi.apis[0].id -Actual $blockedGroup.connectors[0].id
                    StringsAreEqual -Expect $updatedEnvirnmentPolicy.properties.definition.apiGroups.lbi.apis[0].name -Actual $blockedGroup.connectors[0].name
                    StringsAreEqual -Expect $updatedEnvirnmentPolicy.properties.definition.apiGroups.lbi.apis[0].type -Actual $blockedGroup.connectors[0].type
                    StringsAreEqual -Expect "General" -Actual $generalGroup.classification
                    StringsAreEqual -Expect $updatedEnvirnmentPolicy.properties.definition.apiGroups.hbi.apis[0].id -Actual $generalGroup.connectors[0].id
                    StringsAreEqual -Expect $updatedEnvirnmentPolicy.properties.definition.apiGroups.hbi.apis[0].name -Actual $generalGroup.connectors[0].name
                    StringsAreEqual -Expect $updatedEnvirnmentPolicy.properties.definition.apiGroups.hbi.apis[0].type -Actual $generalGroup.connectors[0].type

                    StringsAreEqual -Expect "Microsoft.BusinessAppPlatform/scopes/environments/apiPolicies" -Actual $envirnmentPolicy.Type
                    StringsAreEqual -Expect "SingleEnvironment" -Actual $policy.environmentType

                    # update policy displayName
                    $updatedDisplayName = "Updated old API environment test policy"
                    $policy.displayName = $updatedDisplayName
                    $response = Set-DlpPolicy -PolicyName $policy.name -UpdatedPolicy $policy
                    StringsAreEqual -Expect $updatedDisplayName -Actual $response.Internal.displayName
                }
                else
                {
                    throw "CreateListMultiplePoliciesTests fails ($policy.displayName)"
                }
            }

            CleanV1TestPolicies

            Write-Host "`r`nOldAPIToNewAPICompatibilityTests completed"
        } catch {
            WriteStack
        }
    }
}

function NewAPIToOldAPICompatibilityTests
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentDisplayName
    )
    process 
    {
        try
        {
            Write-Host "`r`nNewAPIToOldAPICompatibilityTests started`r`n"

            # remove the test environments
            CleanTestEnvironments -EnvironmentDisplayName $EnvironmentDisplayName

            CleanV1TestPolicies
        
            $AllEnvironmentsPolicyDisplayName = "AllEnvironments policy"
            $newPolicy = CreatePolicyObject -EnvironmentType "AllEnvironments" -PolicyDisplayName $AllEnvironmentsPolicyDisplayName
            Write-Host "Create a new policy for AllEnvironments"
            $allEnvironmentsResponse = New-DlpPolicy -NewPolicy $newPolicy
            StringsAreEqual -Expect $AllEnvironmentsPolicyDisplayName -Actual $allEnvironmentsResponse.displayName
        
            # create test environment
            Write-Host "Create a test environment"
            $environment = CreateEnvironmentWithoutCDSDatabase -EnvironmentDisplayName $EnvironmentDisplayName
            if($environment -eq $null)
            {
                throw "CreateEnvironment failed."
            }
        
            $OnlyEnvironmentsPolicyDisplayName = "OnlyEnvironments policy"
            $newPolicy = CreatePolicyObject -EnvironmentType "OnlyEnvironments" -PolicyDisplayName $OnlyEnvironmentsPolicyDisplayName
            $environment = [pscustomobject]@{
                id = "/providers/admin/environment"
                name = $environment.EnvironmentName
                type = "/providers/dummyEnvironments"
            }

            $newPolicy.environments += $environment
        
            Write-Host "Create a new policy for OnlyEnvironments"
            $onlyEnvironmentsResponse = New-DlpPolicy -NewPolicy $newPolicy
            StringsAreEqual -Expect $OnlyEnvironmentsPolicyDisplayName -Actual $onlyEnvironmentsResponse.displayName

            # using old API to list and remove policies
            $policies = Get-AdminDlpPolicy

            Write-Host "Check and remove all created policies"

            foreach ($policy in $Policies)
            {
                if ($policy.displayName -eq $AllEnvironmentsPolicyDisplayName)
                {
                    PolicyCheck -NewPolicy $allEnvironmentsResponse -OldPolicy $policy -EnvironmentType "AllEnvironments"

                    # update test tenant policy
                    $policyResponse = Set-AdminDlpPolicy -PolicyName $policy.PolicyName -SetNonBusinessDataGroupState "Block"
                    StringsAreEqual -Expect "Block" -Actual $policyResponse.Internal.properties.definition.rules.apiGroupRule.actions.blockAction.type
                }
                elseif ($policy.displayName -eq $OnlyEnvironmentsPolicyDisplayName)
                {
                    PolicyCheck -NewPolicy $onlyEnvironmentsResponse -OldPolicy $policy -EnvironmentType "OnlyEnvironments"
                    StringsAreEqual -Expect $onlyEnvironmentsResponse.Environments[0].id -Actual $policy.Environments[0].id
                    StringsAreEqual -Expect $onlyEnvironmentsResponse.Environments[0].name -Actual $policy.Environments[0].name
                    StringsAreEqual -Expect $onlyEnvironmentsResponse.Environments[0].type -Actual $policy.Environments[0].type
                    StringsAreEqual -Expect $onlyEnvironmentsResponse.Environments[0].id -Actual $policy.Constraints.environmentFilter1.parameters.environments[0].id
                    StringsAreEqual -Expect $onlyEnvironmentsResponse.Environments[0].name -Actual $policy.Constraints.environmentFilter1.parameters.environments[0].name
                    StringsAreEqual -Expect $onlyEnvironmentsResponse.Environments[0].type -Actual $policy.Constraints.environmentFilter1.parameters.environments[0].type
                }
                else
                {
                    throw "NewAPIToOldAPICompatibilityTests fails ($policy.properties.displayName)"
                }
            }

            # use old API to remove new policies
            CleanTestPolicies

            Write-Host "`r`nOldAPIToNewAPICompatibilityTests completed"
        } catch {
            WriteStack
        }
    }
}

function DLPPolicyConnectorActionControlCrud
{
    param
    (
        [Parameter(Mandatory = $false)]
        [string]$TenantPolicyTestDisplayName = "TenantPolicyDemo"
    )
    process 
    {
        $connectorId = "/providers/Microsoft.PowerApps/apis/shared_msnweather"
        $connectorName = "shared_msnweather"
        $desiredActionBehavior = "Block"
        $desiredDefaultBehavior = "Allow"
        $tenantId = $global:currentSession.tenantId;

        Write-Host "Get connector shared_msnweather actions"
        $connectorActions = Get-AdminPowerAppConnectorAction -ConnectorName $connectorName
        $connectorAction = $connectorActions[0]

        Write-Host "Get all policies"
        $policies = Get-DlpPolicy
        if ($policies -ne $null -and $policies.value -ne $null)
        {
            foreach ($policy in $policies.value)
            {
                if ($policy.displayName -eq $TenantPolicyTestDisplayName)
                {
                    $tenantPolicy = $policy
                    break
                }
            }
        }

        if ($tenantPolicy -eq $null)
        {
            Write-Host "Create test tenant policy."
            $tenantPolicy = New-DlpPolicy -DisplayName $TenantPolicyTestDisplayName -EnvironmentType "AllEnvironments"
        }

        Write-Host "Get connector configuration."
        $policyConnectorConfigurations = Get-PowerAppDlpPolicyConnectorConfigurations  -TenantId $tenantId -PolicyName $tenantPolicy.Name

        $connectorConfigurationsAlreadyExists = $false
        if ($policyConnectorConfigurations -ne $null)
        {
            $connectorConfigurationsAlreadyExists = $true
        }
        else
        {
            $policyConnectorConfigurations = New-Object -TypeName PSObject
        }

        if ($policyConnectorConfigurations.connectorActionConfigurations -eq $null)
        {
            $policyConnectorConfigurations | Add-Member -PassThru -MemberType NoteProperty -Name connectorActionConfigurations -Value @()
        }

        Write-Host "Loop through policy connector action configurations and find the connector based on connector Id."
        $msnWeatherConnectorActionConfigurations = $null
        foreach ($connectorConfiguration in $policyConnectorConfigurations.connectorActionConfigurations)
        {
            if ($connectorConfiguration.connectorId -eq $connectorId)
            {
                $msnWeatherConnectorActionConfigurations = $connectorConfiguration
                break
            }
        }

        Write-Host "If the connector action configuration does not exist, add the connector action configuration."
        if ($msnWeatherConnectorActionConfigurations -eq $null)
        {
            $msnWeatherConnectorActionConfigurations = [pscustomobject]@{  
                connectorId = $connectorId
                actionRules = @()
                defaultConnectorActionRuleBehavior = $desiredDefaultBehavior
            }

            $policyConnectorConfigurations.connectorActionConfigurations += $msnWeatherConnectorActionConfigurations
        }
 

        Write-Host "Loop through policy connector action configurations action rules and find the action rule based on connector action."
        $msnWeatherConnectorActionRule = $null
        foreach ($actionRule in $msnWeatherConnectorActionConfigurations.actionRules)
        {
            if ($actionRule.ActionId -eq $connectorAction.Id)
            {
                $msnWeatherConnectorActionRule = $actionRule
                break
            }
        }
         
        Write-Host "If the action rule does not exist, add the action rule."
        if ($msnWeatherConnectorActionRule -eq $null)
        {
            $msnWeatherConnectorActionRule = [pscustomobject]@{  
                ActionId = $connectorAction.Id
                behavior = $desiredActionBehavior
            }

            $msnWeatherConnectorActionConfigurations.actionRules += $msnWeatherConnectorActionRule
        }

        if ($connectorConfigurationsAlreadyExists)
        {
            Write-Host "Update the policy connector configurations."
            Set-PowerAppDlpPolicyConnectorConfigurations -PolicyName $tenantPolicy.Name -UpdatedConnectorConfigurations $policyConnectorConfigurations -TenantId $tenantId | Out-Null
            $removeConnectorConfigration = $true
        }
        else
        {
            Write-Host "Create a new dlp policy connector configurations."
            New-PowerAppDlpPolicyConnectorConfigurations -NewDlpPolicyConnectorConfigurations $policyConnectorConfigurations -TenantId $tenantId -PolicyName $tenantPolicy.Name | Out-Null
            $removeConnectorConfigration = $false
        }

        if ($removeConnectorConfigration)
        {
            Write-Host "Remove the policy connector configurations."
            Remove-PowerAppDlpPolicyConnectorConfigurations -TenantId $tenantId -PolicyName $tenantPolicy.Name | Out-Null
        }
    }
}

function DLPPolicyConnectorEndpointControlCrud
{
    param
    (
        [Parameter(Mandatory = $false)]
        [string]$TenantPolicyTestDisplayName = "TenantPolicyDemo"
    )
    process 
    {
        $connectorId = "/providers/Microsoft.PowerApps/apis/shared_sql"
        $connectorName = "shared_sql"
        $initialEndpoint = "www.a.*.com"
        $updatedEndPoint = "www.b.*.com"

        $tenantId = $global:currentSession.tenantId;

        Write-Host "Get all policies"
        $policies = Get-DlpPolicy
        if ($policies -ne $null -and $policies.value -ne $null)
        {
            foreach ($policy in $policies.value)
            {
                if ($policy.displayName -eq $TenantPolicyTestDisplayName)
                {
                    $tenantPolicy = $policy
                    break
                }
            }
        }

        if ($tenantPolicy -eq $null)
        {
            Write-Host "Create test tenant policy."
            $tenantPolicy = New-DlpPolicy -DisplayName $TenantPolicyTestDisplayName -EnvironmentType "AllEnvironments"
        }

        Write-Host "Get connector configuration."
        $policyConnectorConfigurations = Get-PowerAppDlpPolicyConnectorConfigurations  -TenantId $tenantId -PolicyName $tenantPolicy.Name

        $connectorConfigurationsAlreadyExists = $false
        if ($policyConnectorConfigurations -ne $null)
        {
            $connectorConfigurationsAlreadyExists = $true
        }
        else
        {
            $policyConnectorConfigurations = New-Object -TypeName PSObject
        }

        if ($policyConnectorConfigurations.endpointConfigurations -eq $null)
        {
            $policyConnectorConfigurations | Add-Member -PassThru -MemberType NoteProperty -Name endpointConfigurations -Value @()
        }

        Write-Host "Loop through policy connector endpoint configurations and find the connector configuration based on connector Id."
        $sqlConnectorEndpointConfigurations = $null
        foreach ($connectorConfiguration in $policyConnectorConfigurations.endpointConfigurations)
        {
            if ($connectorConfiguration.connectorId -eq $connectorId)
            {
                $sqlConnectorEndpointConfigurations = $connectorConfiguration
                break
            }
        }

        Write-Host "If the connector endpoint configuration does not exist, add the connector endpoint configuration."
        if ($sqlConnectorEndpointConfigurations -eq $null)
        {
            $sqlConnectorEndpointConfigurations = [pscustomobject]@{  
                connectorId = $connectorId
                endpointRules = @()
            }

            $policyConnectorConfigurations.endpointConfigurations += $sqlConnectorEndpointConfigurations
        } 

        Write-Host "Loop through policy connector endpoint configurations endpoint rules and find the endpoint rule based on the endpoint."
        $endpointUpdated = $false
        foreach ($endpointRule in $sqlConnectorEndpointConfigurations.endpointRules)
        {
            if ($endpointRule.endPoint -eq $initialEndpoint)
            {
                # Update the endpoint rule in the 3nd run.
                $endpointRule.endPoint = $updatedEndPoint
                $endpointUpdated = $true
                break
            }
        }
         
        Write-Host "If the endpoint rule does not exist, add the endpoint rule."
        if ($sqlConnectorEndpointConfigurations.endpointRules.Count -eq 0)
        {
            # Add the last endpoint rule in the first run.
            $lastEndpointRule = [pscustomobject]@{
                order = 1
                behavior = "Deny"
                endPoint = "*"
            }

            $sqlConnectorEndpointConfigurations.endpointRules += $lastEndpointRule
        }
        else
        {
            # Increase the last endpoint rule order
            $lastOrder = $sqlConnectorEndpointConfigurations.endpointRules[$sqlConnectorEndpointConfigurations.endpointRules.Count - 1].order
            $sqlConnectorEndpointConfigurations.endpointRules[$sqlConnectorEndpointConfigurations.endpointRules.Count - 1].order = $lastOrder + 1

            # Add a new endpoint rule
            $newEndPointRule = [pscustomobject]@{
                order = $lastOrder
                behavior = "Allow"
                endPoint = $initialEndpoint
            }

            $sqlConnectorEndpointConfigurations.endpointRules += $newEndPointRule
            
            # Sort endpoint rules by order in ascending
            $sqlConnectorEndpointConfigurations.endpointRules = $sqlConnectorEndpointConfigurations.endpointRules | Sort-Object -Property order

            # After the 3nd run, there are 3 endpoint rules.
            #[DBG]: PS C:\>> $sqlConnectorEndpointConfigurations.endpointRules
            #
            #order behavior endPoint   
            #----- -------- --------   
            #    1 Allow    www.b.*.com
            #    2 Allow    www.a.*.com
            #    3 Deny     *        
        }

        $removeConnectorConfigration = $false
        if ($connectorConfigurationsAlreadyExists)
        {
            Write-Host "Update the policy connector configurations."
            Set-PowerAppDlpPolicyConnectorConfigurations -PolicyName $tenantPolicy.Name -UpdatedConnectorConfigurations $policyConnectorConfigurations -TenantId $tenantId | Out-Null

            if ($endpointUpdated)
            {
                $removeConnectorConfigration = $true
            }
        }
        else
        {
            Write-Host "Create a new dlp policy connector configurations."
            New-PowerAppDlpPolicyConnectorConfigurations -NewDlpPolicyConnectorConfigurations $policyConnectorConfigurations -TenantId $tenantId -PolicyName $tenantPolicy.Name | Out-Null
        }

        if ($removeConnectorConfigration)
        {
            Write-Host "Remove the policy connector configurations."
            Remove-PowerAppDlpPolicyConnectorConfigurations -TenantId $tenantId -PolicyName $tenantPolicy.Name | Out-Null
        }
    }
}

#internal, helper function
function CheckHttpResponse
{
    param
    (
        [Parameter(Mandatory = $true)]
        [object]$ResponseObject
    )

    if ($ResponseObject -ne $null -and
         ($ResponseObject.Code -eq 200 -or
          $ResponseObject.Code -eq 201 -or
          $ResponseObject.Code -eq 202 -or
          $ResponseObject.Code -eq 204))
    {
        return $ResponseObject
    }
    else
    {
        Write-Error "Test failed.`r`n$ResponseObject"
        throw
    }
}

function CleanTestPolicies
{
    param
    (
    )

    $policies = Get-AdminDlpPolicy

    # clean test policies
    foreach ($policy in $Policies)
    {
        if ($policy.type -eq "Microsoft.BusinessAppPlatform/scopes/apiPolicies")
        {
            Write-Verbose "Remove test tenant policy: $($policy.PolicyName)"
            $ignoreResponse = CheckHttpResponse(Remove-AdminDlpPolicy -PolicyName $policy.PolicyName)
        }
        elseif ($policy.Environments -ne $null)
        {
            $environments = $policy.Environments
            foreach($environment in $environments)
            {
                Write-Verbose "Remove test environment policy: $($policy.PolicyName)"
                $ignoreResponse = CheckHttpResponse(Remove-AdminDlpPolicy -PolicyName $policy.PolicyName -EnvironmentName $environment.name)
            }
        }
    }
}

function CleanV1TestPolicies
{
    param
    (
    )

    $policies = Get-DlpPolicy

    Write-Host "Remove all existing policies"

    # clean test policies
    foreach ($policy in $Policies.Value)
    {
        $response = CheckHttpResponse(Remove-DlpPolicy -PolicyName $policy.name)
        StringsAreEqual -Expect "OK" -Actual $response.Description
    }
}

function CleanTestEnvironments
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentDisplayName
    )

    Write-Host "CleanTestEnvironments will take few seconds"

    $environments = Get-AdminPowerAppEnvironment -Filter "$EnvironmentDisplayName*"
    if($environments -ne $null)
    {
        foreach($environment in $environments)
        {
            if ($environment.CommonDataServiceDatabaseProvisioningState -eq "Succeeded")
            {
                Write-Verbose "Remove-AdminPowerAppEnvironment: $($environment.DisplayName)"
                $deleteEnvironmentResponse = CheckHttpResponse(Remove-AdminPowerAppEnvironment -EnvironmentName $environment.EnvironmentName)
                $deleteStatusUrl = $deleteEnvironmentResponse.Headers['Location']

                $currentTime = Get-Date -format HH:mm:ss
                $nextTime = Get-Date -format HH:mm:ss
                $TimeDiff = New-TimeSpan $currentTime $nextTime
                $timeoutInSeconds = 300
                $statusCode = $deleteEnvironmentResponse.Code
        
                #Wait until the environment has been deleted, there is an error, or we hit a timeout
                while($statusCode -ne 200 -and $statusCode -ne 499 -and $statusCode -ne 500 -and ($TimeDiff.TotalSeconds -lt $timeoutInSeconds))
                {
                    Start-Sleep -s 5
                    $deleteEnvironmentResponse = InvokeApiNoParseContent -Method GET -Route $deleteStatusUrl -ApiVersion "2018-01-01"
                    $statusCode = $deleteEnvironmentResponse.StatusCode
                    $nextTime = Get-Date -format HH:mm:ss
                    $TimeDiff = New-TimeSpan $currentTime $nextTime
                }

                if ($statusCode -eq 500)
                {
                    Write-Error "Remove-AdminPowerAppEnvironment failed."
                    throw
                }

                if ($statusCode -eq 499)
                {
                    Write-Host "Remove-AdminPowerAppEnvironment: work around Power Shell 499 error."
                }
            }
        }
    }
}

function GetNewEnvironment
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentDisplayName
    )

    $environments = Get-AdminPowerAppEnvironment -Filter "$EnvironmentDisplayName*"
    if($environments -ne $null)
    {
        foreach($environment in $environments)
        {
            if ($environment.CommonDataServiceDatabaseProvisioningState -eq "Succeeded")
            {
                return $environment
            }
        }
    }
}

function CreateEnvironmentWithoutCDSDatabase
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentDisplayName,

        [Parameter(Mandatory = $false)]
        [string]$LocationName = "unitedstates",

        [Parameter(Mandatory = $false)]
        [string]$EnvironmentSku = "Trial"
    )

    Write-Host "CreateEnvironmentWithoutCDSDatabase: $EnvironmentDisplayName"
    $environment = New-AdminPowerAppEnvironment -DisplayName $EnvironmentDisplayName -Location $LocationName -EnvironmentSku $EnvironmentSku

    if ($environment.EnvironmentName -eq $null)
    {
        Write-Host "CreateEnvironmentWithoutCDSDatabase failed."
        throw
    }

    return $environment
}

function CreatePolicyObject
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentType,
        [Parameter(Mandatory = $true)]
        [string]$PolicyDisplayName
    )

    $newPolicy = [pscustomobject]@{
        displayName = $PolicyDisplayName
        defaultConnectorsClassification = "General"
        connectorGroups = CreateConnectorGroups
        environmentType = $EnvironmentType
        environments = @()
        etag = $null
    }

    return $newPolicy
}

function CreateConnectorGroups
{
    param
    (
    )

    $blockedConnector1 = [pscustomobject]@{
        id = "ef8e6e0e-472a-4477-bb74-9b491ab94c46"
        name = "Dummy Blocked Connector"
        type = "/providers/dummyConnector"
    }

    $blockedConnectors = @()
    $blockedConnectors += $blockedConnector1
    $blockedConnectorGroup = [pscustomobject]@{
        classification = "Blocked"
        connectors = $blockedConnectors
    }

    $confidentialConnector1 = [pscustomobject]@{
        id = "Http"
        name = "HttpConnector"
        type = "Microsoft.PowerApps/apis"
    }

    $confidentialConnector2 = [pscustomobject]@{
        id = "5fa15129-8d3f-497c-884b-c3436b0219ae"
        name = "Dummy Connector"
        type = "/providers/dummyConnector"
    }

    $confidentialConnector3 = [pscustomobject]@{
        id = "6dc1e59c-d85f-461a-85ed-d67cc1b5c936"
        name = "Dummy Connector"
        type = "/providers/dummyConnector"
    }

    $confidentialConnectors = @()
    $confidentialConnectors += $confidentialConnector1
    $confidentialConnectors += $confidentialConnector2
    $confidentialConnectors += $confidentialConnector3
    $confidentialConnectorGroup = [pscustomobject]@{
        classification = "Confidential"
        connectors = $confidentialConnectors
    }

    $generalConnector1 = [pscustomobject]@{
        id = "0204a6cf-f911-47dc-8006-ab8a46efdf85"
        name = "Dummy General Connector"
        type = "/providers/dummyConnector"
    }

    $generalConnectors = @()
    $generalConnectors += $generalConnector1
    $generalConnectorGroup = [pscustomobject]@{
        classification = "General"
        connectors = $generalConnectors
    }

    $connectorGroups = @()
    $connectorGroups += $generalConnectorGroup
    $connectorGroups += $confidentialConnectorGroup
    $connectorGroups += $blockedConnectorGroup

    return $connectorGroups
}

function CreateConnectorGroupsForDuplicatedConnector
{
    param
    (
    )

    $blockedConnector1 = [pscustomobject]@{
        id = "ef8e6e0e-472a-4477-bb74-9b491ab94c46"
        name = "Dummy Blocked Connector"
        type = "/providers/dummyConnector"
    }

    $blockedConnectors = @()
    $blockedConnectors += $blockedConnector1
    $blockedConnectorGroup = [pscustomobject]@{
        classification = "Blocked"
        connectors = $blockedConnectors
    }

    $confidentialConnectors = @()
    $confidentialConnectors += $blockedConnector1
    $confidentialConnectorGroup = [pscustomobject]@{
        classification = "Confidential"
        connectors = $confidentialConnectors
    }

    $generalConnectors = @()
    $generalConnectors += $blockedConnector1
    $generalConnectorGroup = [pscustomobject]@{
        classification = "General"
        connectors = $generalConnectors
    }

    $connectorGroups = @()
    $connectorGroups += $generalConnectorGroup
    $connectorGroups += $confidentialConnectorGroup
    $connectorGroups += $blockedConnectorGroup

    return $connectorGroups
}

function ListGetUpdateRemovePolicy
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$PolicyDisplayName,

        [Parameter(Mandatory = $true)]
        [string]$EnvironmentType
    )

    Write-Host "Get all policies"
    $policies = Get-DlpPolicy
    StringsAreEqual -Expect $PolicyDisplayName -Actual $policies.Value[0].displayName
    IntAreEqual -Expect 1 -Actual $policies.value.Count
    
    Write-Host "Get a policy by name"
    $policy = Get-DlpPolicy -PolicyName $policies.Value[0].name
    StringsAreEqual -Expect $PolicyDisplayName -Actual $policy.displayName
    
    $upatedDisplayName1 = "Updated test policy for $EnvironmentType"
    Write-Host "Upate policy to $upatedDisplayName1"
    $updatedPolicy1 = [pscustomobject]$policies.Value[0]
    $updatedPolicy1.displayName = $upatedDisplayName1
    $response = Set-DlpPolicy -PolicyName $updatedPolicy1.name -UpdatedPolicy $updatedPolicy1
    StringsAreEqual -Expect $upatedDisplayName1 -Actual $response.Internal.displayName
        
    Write-Host "Remove $upatedDisplayName1"
    $response = CheckHttpResponse(Remove-DlpPolicy -PolicyName $updatedPolicy1.name)
    StringsAreEqual -Expect "OK" -Actual $response.Description
}

function PolicyCheck
{
    param
    (
        [Parameter(Mandatory = $true)]
        [pscustomobject]$NewPolicy,

        [Parameter(Mandatory = $true)]
        [pscustomobject]$OldPolicy,

        [Parameter(Mandatory = $true)]
        [string]$EnvironmentType
    )

    StringsAreEqual -Expect $NewPolicy.CreatedBy.displayName -Actual $OldPolicy.CreatedBy.displayName
    StringsAreEqual -Expect $NewPolicy.createdTime -Actual $OldPolicy.createdTime
    StringsAreEqual -Expect $NewPolicy.lastModifiedBy.displayName -Actual $OldPolicy.lastModifiedBy.displayName
    StringsAreEqual -Expect $NewPolicy.lastModifiedTime -Actual $OldPolicy.lastModifiedTime

    $generalGroup = $NewPolicy.connectorGroups | Where-Object { $_.classification -eq 'General' }
    $confidentialGroup = $NewPolicy.connectorGroups | Where-Object { $_.classification -eq 'Confidential' }
    $blockedGroup = $NewPolicy.connectorGroups | Where-Object { $_.classification -eq 'Blocked' }

    # connector group check
    StringsAreEqual -Expect "General" -Actual $generalGroup.classification
    StringsAreEqual -Expect $generalGroup.connectors[0].id -Actual $OldPolicy.NonBusinessDataGroup[0].id
    StringsAreEqual -Expect $generalGroup.connectors[0].name -Actual $OldPolicy.NonBusinessDataGroup[0].name
    StringsAreEqual -Expect $generalGroup.connectors[0].type -Actual $OldPolicy.NonBusinessDataGroup[0].type
    StringsAreEqual -Expect "Confidential" -Actual $confidentialGroup.classification
    StringsAreEqual -Expect $confidentialGroup.connectors[0].id -Actual $OldPolicy.BusinessDataGroup[0].id
    StringsAreEqual -Expect $confidentialGroup.connectors[0].name -Actual $OldPolicy.BusinessDataGroup[0].name
    StringsAreEqual -Expect $confidentialGroup.connectors[0].type -Actual $OldPolicy.BusinessDataGroup[0].type
    StringsAreEqual -Expect $confidentialGroup.connectors[1].id -Actual $OldPolicy.BusinessDataGroup[1].id
    StringsAreEqual -Expect $confidentialGroup.connectors[1].name -Actual $OldPolicy.BusinessDataGroup[1].name
    StringsAreEqual -Expect $confidentialGroup.connectors[1].type -Actual $OldPolicy.BusinessDataGroup[1].type
    StringsAreEqual -Expect $confidentialGroup.connectors[2].id -Actual $OldPolicy.BusinessDataGroup[2].id
    StringsAreEqual -Expect $confidentialGroup.connectors[2].name -Actual $OldPolicy.BusinessDataGroup[2].name
    StringsAreEqual -Expect $confidentialGroup.connectors[2].type -Actual $OldPolicy.BusinessDataGroup[2].type
    StringsAreEqual -Expect "Blocked" -Actual $blockedGroup.classification
    StringsAreEqual -Expect $blockedGroup.connectors[0].id -Actual $OldPolicy.BlockedGroup[0].id
    StringsAreEqual -Expect $blockedGroup.connectors[0].name -Actual $OldPolicy.BlockedGroup[0].name
    StringsAreEqual -Expect $blockedGroup.connectors[0].type -Actual $OldPolicy.BlockedGroup[0].type

    StringsAreEqual -Expect "Microsoft.BusinessAppPlatform/scopes/apiPolicies" -Actual $OldPolicy.Type
    StringsAreEqual -Expect $EnvironmentType -Actual $NewPolicy.environmentType
}

function StringsAreEqual
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$Expect,

        [Parameter(Mandatory = $true)]
        [string]$Actual
    )

    if($Expect -eq $null)
    {
        $errorMessage = "Expect string is null."
    }
    elseif($Actual -eq $null)
    {
        $errorMessage = "Actual string is null."
    }
    elseif ($Expect -ne $Actual)
    {
        $errorMessage = "Actual string not equal to expect string."
    }
    else
    {
        return
    }

    throw $errorMessage
}

function IntAreEqual
{
    param
    (
        [Parameter(Mandatory = $true)]
        [Int]$Expect,

        [Parameter(Mandatory = $true)]
        [Int]$Actual
    )

    if ($Expect -ne $Actual)
    {
        throw $"Actual ($Actual) not equal to expect ($Expect)."
    }
}

function IsNotNull
{
    param
    (
        [Parameter(Mandatory = $true)]
        [object]$Object
    )

    if ($Object -eq $null)
    {
        throw $"Input objectis null."
    }
}

function WriteStack
{
    Write-Host ('=' * 70) -ForegroundColor red
    Write-Host $_.Exception.Message -ForegroundColor red
    Write-Host $_.ScriptStackTrace -ForegroundColor red
    Write-Host ('=' * 70) -ForegroundColor red
}