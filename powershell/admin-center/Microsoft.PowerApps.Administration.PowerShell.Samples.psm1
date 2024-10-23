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
                id = "/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/$($environment.EnvironmentName)"
                name = $environment.EnvironmentName
                type = "Microsoft.BusinessAppPlatform/scopes/environments"
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

            # define customer connector for old API
            $TenantPolicyTestDisplayName = "TenantPolicyDemo"
            $EnvironmentPolicyDisplayName = "EnvironmentPolicyDemo"
            $NonBusinessConnectorName = "shared_graph-2dapi-2dtest-5f490bdb62ac165ca8-5fbf5c83266b2adfb5"
            $NonBusinessConnectorId = "/providers/Microsoft.PowerApps/scopes/admin/environments/Default-bde1d79a-4825-4883-a114-b4a801feaf16/apis/shared_graph-2dapi-2dtest-5f490bdb62ac165ca8-5fbf5c83266b2adfb5"
            $NonBusinessConnectorType = "Microsoft.PowerApps/apis"
            $BusinessConnectorName = "shared_graph-2dapi-2dtest-5f490bdb62ac165ca8-5fbf5c83266b2adfb5"
            $BusinessConnectorId = "/providers/Microsoft.PowerApps/scopes/admin/environments/Default-bde1d79a-4825-4883-a114-b4a801feaf16/apis/shared_graph-2dapi-2dtest-5f490bdb62ac165ca8-5fbf5c83266b2adfb5"
            $BusinessConnectorType = "Microsoft.PowerApps/apis"

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

function Add-ConnectorToBusinessDataGroupSample
{
    <#
    .SYNOPSIS
    Sets connector to the business data group of data loss policy.
    .DESCRIPTION
    The code is changed to using new DLP API and set connector to the business data group depending on parameters. 
    .PARAMETER PolicyName
    The PolicyName's identifier.
    .PARAMETER ConnectorName
    The Connector's identifier.
    .EXAMPLE
    Add-ConnectorToBusinessDataGroup -PolicyName e25a94b2-3111-468e-9125-3d3db3938f13 -ConnectorName shared_office365users
    Sets the connector to Confidential group of policyname e25a94b2-3111-468e-9125-3d3db3938f13.
    #> 
    param
    (
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$PolicyName,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$ConnectorName
    )
    process 
    {
        $policy = Get-DlpPolicy -PolicyName $PolicyName
        $confidentialGroup = $policy.connectorGroups | Where-Object { $_.classification -eq 'Confidential' }
        $connectorInConfidential = $confidentialGroup.connectors | where { $_.name -eq $ConnectorName }

        if($connectorInConfidential -ne $null)
        {
            Write-Error "Connector already exists in Confidential group"
            return $null
        }

        $connector = Get-PowerAppConnector -EnvironmentName $policy.environments[0].name -ConnectorName $ConnectorName `
            | %{ New-Object -TypeName PSObject -Property @{ id = $_.connectorId; name = ($_.connectorId -split "/apis/")[1]; type = $_.internal.type } }

        if($connector -eq $null)
        {
            Write-Error "No connector with specified name found"
            return $null
        }

        #Add the connector to the confidential group of policy
        $confidentialGroup.connectors += $connector

        #remove the connector from General group if exist
        $generalGroup = $policy.connectorGroups | Where-Object { $_.classification -eq 'General' }
        $generalConnectorsWithoutProvidedConnector = $generalGroup.connectors | Where-Object { $_.id -ne $connector.id }
        
        if ($generalConnectorsWithoutProvidedConnector -eq $null)
        {
            $generalConnectorsWithoutProvidedConnector =  @()
        }
        
        $generalGroup.connectors = [Array]$generalConnectorsWithoutProvidedConnector

        #Update policy
        Set-DlpPolicy -PolicyName $policy.name -UpdatedPolicy $policy
    }
}

function Remove-ConnectorFromBusinessDataGroupSample
{
     <#
    .SYNOPSIS
    Removes connector from the business data group of data loss policy.
    .DESCRIPTION
    The Remove-ConnectorFromBusinessDataGroup removes connector from the business data group of DLP depending on parameters. 
    .PARAMETER PolicyName
    The PolicyName's identifier.
    .PARAMETER ConnectorName
    The Connector's identifier.
    .EXAMPLE
    Remove-ConnectorFromBusinessDataGroup -PolicyName e25a94b2-3111-468e-9125-3d3db3938f13 -ConnectorName shared_office365users
    Removes the connector from BusinessData group of policyname e25a94b2-3111-468e-9125-3d3db3938f13.
    #> 
    param
    (
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$PolicyName,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$ConnectorName
    )
    process 
    {
        $policy = Get-DlpPolicy -PolicyName $PolicyName
        $generalGroup = $policy.connectorGroups | Where-Object { $_.classification -eq 'General' }
        $connectorInGeneral = $generalGroup.connectors | where { $_.name -eq $ConnectorName }

        if($connectorInGeneral -ne $null)
        {
            Write-Error "Connector already exists in General group"
            return $null
        }

        $connector = Get-PowerAppConnector -EnvironmentName $policy.environments[0].name -ConnectorName $ConnectorName `
            | %{ New-Object -TypeName PSObject -Property @{ id = $_.connectorId; name = ($_.connectorId -split "/apis/")[1]; type = $_.internal.type } }

        if($connector -eq $null)
        {
            Write-Error "No connector with specified name found"
            return $null
        }

        #Add the connector to the General group of the policy
        $generalGroup.connectors += $connector

        #remove the connector from Confidential group of the policy
        $confidentialGroup = $policy.connectorGroups | Where-Object { $_.classification -eq 'Confidential' }
        $confidentialConnectorsWithoutProvidedConnector = $confidentialGroup.connectors | where { $_.id -ne $connector.id }

        if($confidentialConnectorsWithoutProvidedConnector -eq $null)
        {
            $confidentialConnectorsWithoutProvidedConnector =  @()
        }
        
        $confidentialGroup.connectors = [Array]$confidentialConnectorsWithoutProvidedConnector

        #Update policy
        Set-DlpPolicy -PolicyName $policy.name -UpdatedPolicy $policy
    }
}

function Add-CustomConnectorToPolicySample
{
    <#
    .SYNOPSIS
    Adds a custom connector to the given group.
    .DESCRIPTION
    The Add-CustomConnectorToPolicySample adds a custom connector to a specific group of a DLP policy depending on parameters.
    .PARAMETER PolicyName
    The PolicyName's identifier.
    .PARAMETER GroupName
    The name of the group to add the connector to, lbi or hbi.
    .PARAMETER ConnectorName
    The Custom Connector's name.
    .PARAMETER ConnectorId
    The Custom Connector's ID.
    .PARAMETER ConnectorType
    The Custom Connector's type.
    .EXAMPLE
    Add-CustomConnectorToPolicySample -EnvironmentName Default-02c201b0-db76-4a6a-b3e1-a69202b479e6 -PolicyName 7b914a18-ad8b-4f15-8da5-3155c77aa70a -ConnectorName BloopBlop -ConnectorId /providers/Microsoft.PowerApps/apis/BloopBlop -ConnectorType Microsoft.PowerApps/apis -GroupName hbi
    Adds the custom connector 'BloopBlop' to BusinessData group of policy name 7b914a18-ad8b-4f15-8da5-3155c77aa70a in environment Default-02c201b0-db76-4a6a-b3e1-a69202b479e6.
    #>
    param
    (
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$PolicyName,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$ConnectorName,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string][ValidateSet("lbi", "hbi")]$GroupName,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$ConnectorId,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$ConnectorType
    )
    process
    {
        $policy = Get-DlpPolicy -PolicyName $PolicyName

        $generalGroup = $policy.connectorGroups | Where-Object { $_.classification -eq 'General' }
        $connectorInGeneral = $generalGroup.connectors | where { $_.id -eq $ConnectorId }

        $confidentialGroup = $policy.connectorGroups | Where-Object { $_.classification -eq 'Confidential' }
        $connectorInConfidential = $confidentialGroup.connectors | where { $_.id -eq $ConnectorId }

        if($connectorInGeneral -eq $null -and $connectorInConfidential -eq $null)
        {
            $customConnector = [pscustomobject]@{
                id = $ConnectorId
                name = $ConnectorName
                type = $ConnectorType
            }

            if ($GroupName -eq "hbi")
            {
                #Add it to the confidential group of the policy
                $confidentialGroup.connectors += $customConnector
            }
            else
            {
                #Add it to the general group of the policy
                $generalGroup.connectors += $customConnector
            }

            #Update policy
            Set-DlpPolicy -PolicyName $policy.name -UpdatedPolicy $policy
        }
        else
        {
            if($connectorInConfidential -ne $null)
            {
                Write-Error "The given connector is already present in the hbi group."
            }
            else
            {
                Write-Error "The given connector is already present in the lbi group."
            }
            return $null
        }
    }
}

function Remove-CustomConnectorFromPolicySample
{
    <#
    .SYNOPSIS
    Deletes a custom connector from the given DLP policy.
    .DESCRIPTION
    The Remove-CustomConnectorFromPolicySample deletes a custom connector from the specific DLP policy. 
    .PARAMETER PolicyName
    The PolicyName's identifier.
    .PARAMETER ConnectorName
    The connector's identifier.
    .EXAMPLE
    Remove-CustomConnectorFromPolicySample -PolicyName 7b914a18-ad8b-4f15-8da5-3155c77aa70a -ConnectorName shared_office365users
    Deletes the custom connector 'shared_office365users' from the DLP policy of policy name 7b914a18-ad8b-4f15-8da5-3155c77aa70a.
    #>
    param
    (
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$PolicyName,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$ConnectorName
    )
    process
    {
        $policy = Get-DlpPolicy -PolicyName $PolicyName

        $generalGroup = $policy.connectorGroups | Where-Object { $_.classification -eq 'General' }
        $connectorInGeneral = $generalGroup.connectors | where { $_.name -eq $ConnectorName }

        $confidentialGroup = $policy.connectorGroups | Where-Object { $_.classification -eq 'Confidential' }
        $connectorInConfidential = $confidentialGroup.connectors | where { $_.name -eq $ConnectorName }

        if($connectorInGeneral -eq $null -and $connectorInConfidential -eq $null)
        {
            Write-Error "The given connector is not in the policy."
            return $null
        }
        else
        {
            if($connectorInGeneral -eq $null)
            {
                #remove the connector from confidential group of policy
                $confidentialConnectorsWithoutProvidedConnector = $confidentialGroup.connectors | where { $_.name -ne $ConnectorName }
                $confidentialGroup.connectors = [Array]$confidentialConnectorsWithoutProvidedConnector

                #Update policy
                Set-DlpPolicy -PolicyName $policy.name -UpdatedPolicy $policy
            }
            else
            {
                #remove the connector from general group of policy
                $generalConnectorsWithoutProvidedConnector = $generalGroup.connectors | Where-Object { $_.name -ne $ConnectorName }
                $generalGroup.connectors = [Array]$generalConnectorsWithoutProvidedConnector

                #Update policy
                Set-DlpPolicy -PolicyName $policy.name -UpdatedPolicy $policy
            }
        }
    }
}

function CustomerConnectorUpdateTests
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentDisplayName,

        [Parameter(Mandatory = $false)]
        [string]$PolicyDisplayName = "Test policy for CustomerConnectorUpdateTests",

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
            Write-Host "`r`nCustomerConnectorUpdateTests started`r`n"

            Write-Host "Change to EnvironmentAdmin"
            $Password = ConvertTo-SecureString $EnvironmentAdminPassword -AsPlainText -Force
            Add-PowerAppsAccount -Endpoint $EndPoint -Username $EnvironmentAdminName -Password $Password
                
            $policy = (Get-DlpPolicy).Value | where { $_.displayName -eq $PolicyDisplayName }
            if ($policy -eq $null)
            {
                $EnvironmentDisplayName = $EnvironmentDisplayName + " for NonAdmin"
                $environment = CreateEnvironmentWithoutCDSDatabase -EnvironmentDisplayName $EnvironmentDisplayName -EnvironmentSku "Production"
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
                $policy = New-DlpPolicy -NewPolicy $newPolicy
                StringsAreEqual -Expect $PolicyDisplayName -Actual $policy.displayName
            }

            # define connector test data
            $BusinessConnectorId = "/providers/Microsoft.PowerApps/apis/shared_msnweather"
            $BusinessConnectorName = "shared_msnweather"
            $BusinessConnectorType = "Microsoft.PowerApps/apis"

            if ((CheckConnectorExist -Policy $policy -Classification "Confidential" -ConnectorName $BusinessConnectorName) -or
                (CheckConnectorExist -Policy $policy -Classification "General" -ConnectorName $BusinessConnectorName))
            {
                # remove the connector from the policy
                $response = Remove-CustomConnectorFromPolicySample -PolicyName $policy.Name -ConnectorName $BusinessConnectorName
                $result = CheckConnectorExist -Policy $response.Internal -Classification "Confidential" -ConnectorName $BusinessConnectorName
                IsFalse -Result $result -Message "The connector is not removed."
                $result = CheckConnectorExist -Policy $response.Internal -Classification "General" -ConnectorName $BusinessConnectorName
                IsFalse -Result $result -Message "The connector is not removed."
            }

            # add a connector to the policy
            $response = Add-CustomConnectorToPolicySample -PolicyName $policy.Name -ConnectorName $BusinessConnectorName -ConnectorId $BusinessConnectorId -ConnectorType $BusinessConnectorType -GroupName hbi
            $result = CheckConnectorExist -Policy $response.Internal -Classification "Confidential" -ConnectorName $BusinessConnectorName
            IsTrue -Result $result -Message "The connector is not in confidential group."

            # remove the connector from the policy
            $response = Remove-CustomConnectorFromPolicySample -PolicyName $policy.Name -ConnectorName $BusinessConnectorName
            $result = CheckConnectorExist -Policy $response.Internal -Classification "Confidential" -ConnectorName $BusinessConnectorName
            IsFalse -Result $result -Message "The connector is not removed."

            # add the connector to confidential group
            $response = Add-ConnectorToBusinessDataGroupSample -PolicyName $policy.Name -ConnectorName $BusinessConnectorName
            $result = CheckConnectorExist -Policy $response.Internal -Classification "Confidential" -ConnectorName $BusinessConnectorName
            IsTrue -Result $result -Message "The connector is not in confidential group."

            # remove the connector from confidential group
            $response = Remove-ConnectorFromBusinessDataGroupSample -PolicyName $policy.Name -ConnectorName $BusinessConnectorName
            $result = CheckConnectorExist -Policy $response.Internal -Classification "Confidential" -ConnectorName $BusinessConnectorName
            IsFalse -Result $result -Message "The connector is not removed."

            Write-Host "Change user back to GlobalAdmin"
            $Password = ConvertTo-SecureString $TenantAdminPassword -AsPlainText -Force
            Add-PowerAppsAccount -Endpoint $EndPoint -Username $TenantAdminName -Password $Password

            Write-Host "`r`nCustomerConnectorUpdateTests completed"
        } catch {
            WriteStack
        }
    }
}

function UpdatePolicyEnvironmentsForTeams
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$OnlyEnvironmentsPolicyName,

        [Parameter(Mandatory = $true)]
        [string]$OnlyEnvironmentsPolicyDisplayName,

        [Parameter(Mandatory = $false)]
        [string]$ExceptEnvironmentsPolicyName,

        [Parameter(Mandatory = $false)]
        [string]$ExceptEnvironmentsPolicyDisplayName,

        [Parameter(Mandatory = $false)]
        [string[]]$ExceptionEnvironmentIds
    )

    Write-Host "UpdatePolicyEnvironmentsForTeams starts."

    $onlyEnvironmentsPolicy = Get-DlpPolicy -PolicyName $OnlyEnvironmentsPolicyName
    if ($onlyEnvironmentsPolicy.environmentType -ne "OnlyEnvironments" -or
        $onlyEnvironmentsPolicy.displayName -ne $OnlyEnvironmentsPolicyDisplayName)
    {
        Write-Host "Invalid OnlyEnvironments policy."
        return
    }

    $exceptionEnvironmentsPolicy = $null
    if (-not [string]::IsNullOrWhiteSpace($ExceptEnvironmentsPolicyName) -and
        -not [string]::IsNullOrWhiteSpace($ExceptEnvironmentsPolicyDisplayName))
    {
        $exceptionEnvironmentsPolicy = Get-DlpPolicy -PolicyName $ExceptEnvironmentsPolicyName
        if ($exceptionEnvironmentsPolicy.environmentType -ne "ExceptEnvironments" -or
            $exceptionEnvironmentsPolicy.displayName -ne $ExceptEnvironmentsPolicyDisplayName)
        {
            Write-Host "Invalid ExceptEnvironments policy."
            return        
        }
    }

    # get Teams environments
    $environments = Get-AdminPowerAppEnvironment -EnvironmentSku "Teams" -ApiVersion "2020-06-01"

    $teamEnvironments = @()
    foreach ($env in $environments)
    {
        $item = [pscustomobject]@{
            id = $env.Internal.id
            name = $env.Internal.name
            type = $env.Internal.type
        }
        $teamEnvironments += $item
    }

    if ($teamEnvironments.Count -gt 0)
    {
        $onlyEnvironmentsPolicy.environments = $teamEnvironments
        $response = Set-DlpPolicy -PolicyName $onlyEnvironmentsPolicy.name -UpdatedPolicy $onlyEnvironmentsPolicy

        StringsAreEqual -Expect $OnlyEnvironmentsPolicyName -Actual $response.Internal.name
        Write-Host "OnlyEnvironments policy is updated."
    }
    else
    {
        Write-Host "There is no Teams environment found."
    }

    if ($exceptionEnvironmentsPolicy -ne $null)
    {
        # add teams environment into ExceptEnvironments policy
        foreach ($environment in $teamEnvironments)
        {
            if (($exceptionEnvironmentsPolicy.environments | where {$_.id -eq $environment.Id}) -eq $null)
            {
                # add teams environment
                $exceptionEnvironmentsPolicy.environments += $environment
            }
        }

        if ($ExceptionEnvironmentIds -ne $null)
        {
            foreach ($environmentId in $ExceptionEnvironmentIds)
            {
                $environment = $exceptionEnvironmentsPolicy.environments | where {$_.name -eq $environmentId}
                if ($environment -eq $null)
                {
                    # add the environment from $ExceptionEnvironmentIds into the policy
                    $environment = Get-AdminPowerAppEnvironment -EnvironmentName $environmentId
                    if ($environment.Internal.id -ne $null)
                    {
                        $item = [pscustomobject]@{
                            id = $environment.Internal.id
                            name = $environment.Internal.name
                            type = $environment.Internal.type
                        }
                        $exceptionEnvironmentsPolicy.environments += $item
                    }
                    else
                    {
                        Write-Host "Get environment fails.`r`n$($environment.Internal.Message)"
                    }
                }
            }
        }

        if ($exceptionEnvironmentsPolicy.environments.Count -gt 0)
        {
            $response = Set-DlpPolicy -PolicyName $exceptionEnvironmentsPolicy.name -UpdatedPolicy $exceptionEnvironmentsPolicy

            if ($response.Internal.name -ne $null)
            {
                StringsAreEqual -Expect $ExceptEnvironmentsPolicyName -Actual $response.Internal.name
                Write-Host "ExceptEnvironments policy is updated."
            }
            else
            {
                Write-Host "ExceptEnvironments policy update fails.`r`n$response.Error"
            }
        }
        else
        {
            Write-Host "ExceptEnvironments policy is not updated."
        }
    }

    Write-Host "UpdatePolicyEnvironmentsForTeams completes."
}

function EnableManagedEnvironments
{
    <#
     .SYNOPSIS
     Enables Managed Environments for the given environment.
     .DESCRIPTION
     The EnableManagedEnvironments cmdlet enables Managed Environments for the given environment by updating its governance configuration.
     Use Get-Help EnableManagedEnvironments -Examples for more details.
     .PARAMETER EnvironmentId
     The id (usually a GUID) of the environment.
     .EXAMPLE
     EnableManagedEnvironments -EnvironmentId 8d996ece-8558-4c4e-b459-a51b3beafdb4
     Enables Managed Environments for environment with id 8d996ece-8558-4c4e-b459-a51b3beafdb4.
    #>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentId
    )

    Write-Host "Retrieving environment."

    $environment = Get-AdminPowerAppEnvironment -EnvironmentName $EnvironmentId
    if ($environment -eq $null)
    {
        Write-Host "No environment was found with the given id."
        return
    }

    $governanceConfiguration = $environment.Internal.properties.governanceConfiguration
    $governanceConfiguration = CoalesceGovernanceConfiguration -GovernanceConfiguration $governanceConfiguration
    if ($governanceConfiguration.protectionLevel -eq "Standard")
    {
        Write-Host "The specified environment is already managed."
        return
    }

    $governanceConfiguration.protectionLevel = "Standard"
    
    $response = Set-AdminPowerAppEnvironmentGovernanceConfiguration -EnvironmentName $EnvironmentId -UpdatedGovernanceConfiguration $GovernanceConfiguration
    if ($response.Code -ne 202)
    {
        Write-Host "Failed to enable Managed Environments for the specified environment."
        Write-Host $response.Internal.Message
        return
    }
    
    Write-Host "Enabled Managed Environments for the specified environment."    
}

function DisableManagedEnvironments
{
    <#
     .SYNOPSIS
     Disables Managed Environments for the given environment.
     .DESCRIPTION
     The DisableManagedEnvironments cmdlet enables Managed Environments for the given environment by updating its governance configuration.
     Use Get-Help DisableManagedEnvironments -Examples for more details.
     .PARAMETER EnvironmentId
     The id (usually a GUID) of the environment.
     .EXAMPLE
     DisableManagedEnvironments -EnvironmentId 8d996ece-8558-4c4e-b459-a51b3beafdb4
     Disables Managed Environments for environment with id 8d996ece-8558-4c4e-b459-a51b3beafdb4.
    #>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentId
    )

    Write-Host "Retrieving environment."

    $environment = Get-AdminPowerAppEnvironment -EnvironmentName $EnvironmentId
    if ($environment -eq $null)
    {
        Write-Host "No environment was found with the given id."
        return
    }

    $governanceConfiguration = $environment.Internal.properties.governanceConfiguration
    $governanceConfiguration = CoalesceGovernanceConfiguration -GovernanceConfiguration $governanceConfiguration
    if ($governanceConfiguration.protectionLevel -ne "Standard")
    {
        Write-Host "The specified environment is not managed."
        return
    }

    $governanceConfiguration.protectionLevel = "Basic"
    
    $response = Set-AdminPowerAppEnvironmentGovernanceConfiguration -EnvironmentName $EnvironmentId -UpdatedGovernanceConfiguration $GovernanceConfiguration
    if ($response.Code -ne 202)
    {
        Write-Host "Failed to disable Managed Environments for the specified environment."
        Write-Host $response.Internal.Message
        return
    }
    
    Write-Host "Disabled Managed Environments for the specified environment."    
}

function IncludeInsightsForManagedEnvironmentsInWeeklyEmailDigest
{
    <#
     .SYNOPSIS
     Includes insights for the specified Managed environment from weekly email digest.
     .DESCRIPTION
     The IncludeInsightsForManagedEnvironmentsInWeeklyEmailDigest cmdlet includes insights for the specified Managed environment from weekly email digest.
     Use Get-Help IncludeInsightsForManagedEnvironmentsInWeeklyEmailDigest -Examples for more details.
     .PARAMETER EnvironmentId
     The id (usually a GUID) of the environment.
     .EXAMPLE
     IncludeInsightsForManagedEnvironmentsInWeeklyEmailDigest -EnvironmentId 8d996ece-8558-4c4e-b459-a51b3beafdb4
     Includes insights for Managed environment with id 8d996ece-8558-4c4e-b459-a51b3beafdb4 in the weekly email digest.
    #>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentId
    )

    Write-Host "Retrieving environment."

    $environment = Get-AdminPowerAppEnvironment -EnvironmentName $EnvironmentId
    if ($environment -eq $null)
    {
        Write-Host "No environment was found with the given id."
        return
    }

    $governanceConfiguration = $environment.Internal.properties.governanceConfiguration
    $governanceConfiguration = CoalesceGovernanceConfiguration -GovernanceConfiguration $governanceConfiguration
    if ($governanceConfiguration.protectionLevel -ne "Standard")
    {
        Write-Host "The specified environment is not managed."
        return
    }

    if ($governanceConfiguration.settings.extendedSettings.excludeEnvironmentFromAnalysis -ne "True")
    {
        Write-Host "The specified environment is already included in weekly email digest."
        return
    }
    
    $governanceConfiguration.settings.extendedSettings.excludeEnvironmentFromAnalysis = "false"

    $response = Set-AdminPowerAppEnvironmentGovernanceConfiguration -EnvironmentName $EnvironmentId -UpdatedGovernanceConfiguration $GovernanceConfiguration
    if ($response.Code -ne 202)
    {
        Write-Host "Failed to include insights for the specified environment in weekly email digest."
        Write-Host $response.Internal.Message
        return
    }
    
    Write-Host "Included insights for the specified environment in weekly email digest."    
}

function ExcludeInsightsForManagedEnvironmentsInWeeklyEmailDigest
{
    <#
     .SYNOPSIS
     Excludes insights for the specified Managed environment from weekly email digest.
     .DESCRIPTION
     The ExcludeInsightsForManagedEnvironmentsInWeeklyEmailDigest cmdlet excludes insights for the specified Managed environment from weekly email digest.
     Use Get-Help ExcludeInsightsForManagedEnvironmentsInWeeklyEmailDigest -Examples for more details.
     .PARAMETER EnvironmentId
     The id (usually a GUID) of the environment.
     .EXAMPLE
     ExcludeInsightsForManagedEnvironmentsInWeeklyEmailDigest -EnvironmentId 8d996ece-8558-4c4e-b459-a51b3beafdb4
     Excludes insights for Managed environment with id 8d996ece-8558-4c4e-b459-a51b3beafdb4 in the weekly email digest.
    #>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentId
    )

    Write-Host "Retrieving environment."

    $environment = Get-AdminPowerAppEnvironment -EnvironmentName $EnvironmentId
    if ($environment -eq $null)
    {
        Write-Host "No environment was found with the given id."
        return
    }

    $governanceConfiguration = $environment.Internal.properties.governanceConfiguration
    $governanceConfiguration = CoalesceGovernanceConfiguration -GovernanceConfiguration $governanceConfiguration
    if ($governanceConfiguration.protectionLevel -ne "Standard")
    {
        Write-Host "The specified environment is not managed."
        return
    }

    if ($governanceConfiguration.settings.extendedSettings.excludeEnvironmentFromAnalysis -eq "True")
    {
        Write-Host "The specified environment is already excluded from weekly email digest."
        return
    }
    
    $governanceConfiguration.settings.extendedSettings.excludeEnvironmentFromAnalysis = "true"

    $response = Set-AdminPowerAppEnvironmentGovernanceConfiguration -EnvironmentName $EnvironmentId -UpdatedGovernanceConfiguration $GovernanceConfiguration
    if ($response.Code -ne 202)
    {
        Write-Host "Failed to exclude insights for the specified environment in weekly email digest."
        Write-Host $response.Internal.Message
        return
    }
    
    Write-Host "Excluded insights for the specified environment in weekly email digest."    
}

function SetManagedEnvironmentSolutionCheckerEnforcementLevel
{
    <#
     .SYNOPSIS
     Sets solution checker enforcement for the specified Managed environment.
     .DESCRIPTION
     The SetManagedEnvironmentSolutionCheckerEnforcementLevel cmdlet sets solution checker enforcement for the specified Managed environment.
     Use Get-Help SetManagedEnvironmentSolutionCheckerEnforcementLevel -Examples for more details.
     .PARAMETER EnvironmentId
     The id (usually a GUID) of the environment.
     .PARAMETER Level
     The enforcement level (none, warn, block).
     .PARAMETER RuleExclusions
     Optional. The rule exclusions for solution checker enforcement.
     .EXAMPLE
     SetManagedEnvironmentSolutionCheckerEnforcementLevel -EnvironmentId 8d996ece-8558-4c4e-b459-a51b3beafdb4 -Level block
     Sets solution checker enforcement for Managed environment with id 8d996ece-8558-4c4e-b459-a51b3beafdb4 to the "block" level.
    #>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentId,

        [Parameter(Mandatory = $true)]
        [string][ValidateSet("none", "warn", "block")]$Level,

        [string]$RuleExclusions
    )

    Write-Host "Retrieving environment."

    $environment = Get-AdminPowerAppEnvironment -EnvironmentName $EnvironmentId
    if ($environment -eq $null)
    {
        Write-Host "No environment was found with the given id."
        return
    }

    $governanceConfiguration = $environment.Internal.properties.governanceConfiguration
    $governanceConfiguration = CoalesceGovernanceConfiguration -GovernanceConfiguration $governanceConfiguration
    if ($governanceConfiguration.protectionLevel -ne "Standard")
    {
        Write-Host "The specified environment is not managed."
        return
    }
    
    $governanceConfiguration.settings.extendedSettings | Add-Member -MemberType NoteProperty -Name 'solutionCheckerMode' -Value $Level -Force

    if ($PSBoundParameters.ContainsKey('RuleExclusions'))
    {
        $governanceConfiguration.settings.extendedSettings | Add-Member -MemberType NoteProperty -Name 'solutionCheckerRuleOverrides' -Value $RuleExclusions -Force
    }
    
    $response = Set-AdminPowerAppEnvironmentGovernanceConfiguration -EnvironmentName $EnvironmentId -UpdatedGovernanceConfiguration $GovernanceConfiguration
    if ($response.Code -ne 202)
    {
        Write-Host "Failed to set solution checker enforcement for the specified environment."
        Write-Host $response.Internal.Message
        return
    }
    
    Write-Host "Set solution checker enforcement for the specified environment."    
}

function SetManagedEnvironmentMakerOnboardingMarkdownContent
{
    <#
     .SYNOPSIS
     Sets markdown content for maker onboarding for the specified Managed environment.
     .DESCRIPTION
     The SetManagedEnvironmentMakerOnboardingMarkdownContent cmdlet sets markdown content for maker onboarding for the specified Managed environment.
     Use Get-Help SetManagedEnvironmentMakerOnboardingMarkdownContent -Examples for more details.
     .PARAMETER EnvironmentId
     The id (usually a GUID) of the environment.
     .PARAMETER Markdown
     The maker content Markdown.
     .EXAMPLE
     SetManagedEnvironmentMakerOnboardingMarkdownContent -EnvironmentId 8d996ece-8558-4c4e-b459-a51b3beafdb4 -Markdown "## Welcome to Power Apps
### Let's get started"

     Sets Maker onboarding markdown content for Managed environment with id 8d996ece-8558-4c4e-b459-a51b3beafdb4 to
        ## Welcome to NR Power Apps
        ### Let's get started
    #>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentId,

        [Parameter(Mandatory = $true)]
        [string]$Markdown
    )

    Write-Host "Retrieving environment."

    $environment = Get-AdminPowerAppEnvironment -EnvironmentName $EnvironmentId
    if ($environment -eq $null)
    {
        Write-Host "No environment was found with the given id."
        return
    }

    $governanceConfiguration = $environment.Internal.properties.governanceConfiguration
    $governanceConfiguration = CoalesceGovernanceConfiguration -GovernanceConfiguration $governanceConfiguration
    if ($governanceConfiguration.protectionLevel -ne "Standard")
    {
        Write-Host "The specified environment is not managed."
        return
    }

    $makerOnboardingChangeTimestamp = (Get-Date).ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'")

    $governanceConfiguration.settings.extendedSettings | Add-Member -MemberType NoteProperty -Name 'makerOnboardingMarkdown' -Value $Markdown -Force
    $governanceConfiguration.settings.extendedSettings | Add-Member -MemberType NoteProperty -Name 'makerOnboardingTimestamp' -Value $makerOnboardingChangeTimestamp -Force

    $response = Set-AdminPowerAppEnvironmentGovernanceConfiguration -EnvironmentName $EnvironmentId -UpdatedGovernanceConfiguration $GovernanceConfiguration
    if ($response.Code -ne 202)
    {
        Write-Host "Failed to set markdown content for maker onboarding for the specified environment."
        Write-Host $response.Internal.Message
        return
    }
    
    Write-Host "Set markdown content for maker onboarding for the specified environment."    
}

function SetManagedEnvironmentMakerOnboardingLearnMoreUrl
{
    <#
     .SYNOPSIS
     Sets Learn more URL for maker onboarding for the specified Managed environment.
     .DESCRIPTION
     The SetManagedEnvironmentMakerOnboardingLearnMoreUrl cmdlet Sets Learn more URL for maker onboarding for the specified Managed environment.
     Use Get-Help SetManagedEnvironmentMakerOnboardingLearnMoreUrl -Examples for more details.
     .PARAMETER EnvironmentId
     The id (usually a GUID) of the environment.
     .PARAMETER LearnMoreUrl
     The maker onboarding learn more URL.
     .EXAMPLE
     SetManagedEnvironmentMakerOnboardingLearnMoreUrl -EnvironmentId 8d996ece-8558-4c4e-b459-a51b3beafdb4 -LearnMoreUrl "www.microsoft.com"
     Sets Learn more URL for maker onboarding for Managed environment with id 8d996ece-8558-4c4e-b459-a51b3beafdb4 to "www.microsoft.com"
    #>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentId,

        [Parameter(Mandatory = $true)]
        [string]$LearnMoreUrl
    )

    Write-Host "Retrieving environment."

    $environment = Get-AdminPowerAppEnvironment -EnvironmentName $EnvironmentId
    if ($environment -eq $null)
    {
        Write-Host "No environment was found with the given id."
        return
    }

    $governanceConfiguration = $environment.Internal.properties.governanceConfiguration
    $governanceConfiguration = CoalesceGovernanceConfiguration -GovernanceConfiguration $governanceConfiguration
    if ($governanceConfiguration.protectionLevel -ne "Standard")
    {
        Write-Host "The specified environment is not managed."
        return
    }
    
    $makerOnboardingChangeTimestamp = (Get-Date).ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'")

    $governanceConfiguration.settings.extendedSettings | Add-Member -MemberType NoteProperty -Name 'makerOnboardingUrl' -Value $LearnMoreUrl -Force
    $governanceConfiguration.settings.extendedSettings | Add-Member -MemberType NoteProperty -Name 'makerOnboardingTimestamp' -Value $makerOnboardingChangeTimestamp -Force

    $response = Set-AdminPowerAppEnvironmentGovernanceConfiguration -EnvironmentName $EnvironmentId -UpdatedGovernanceConfiguration $GovernanceConfiguration
    if ($response.Code -ne 202)
    {
        Write-Host "Failed to set Learn more URL for maker onboarding for the specified environment."
        Write-Host $response.Internal.Message
        return
    }
    
    Write-Host "Set Learn more URL for maker onboarding for the specified environment."    
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

function IncludeInsightsForManagedEnvironmentsInPPACHomePageCards
{
    <#
     .SYNOPSIS
     Include insights for the specified Managed environment from PPAC homepage insights cards.
     .DESCRIPTION
     The IncludeInsightsForManagedEnvironmentsInPPACHomePageCards cmdlet includes insights for the specified Managed environment from PPAC homepage insights cards.
     Use Get-Help IncludeInsightsForManagedEnvironmentsInPPACHomePageCards -Examples for more details.
     .PARAMETER EnvironmentId
     The id (usually a GUID) of the environment.
     .EXAMPLE
     IncludeInsightsForManagedEnvironmentsInPPACHomePageCards -EnvironmentId 8d996ece-8558-4c4e-b459-a51b3beafdb4
     Includes insights for Managed environment with id 8d996ece-8558-4c4e-b459-a51b3beafdb4 in PPAC homepage insights cards.
    #>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentId
    )

    Write-Host "Retrieving environment."

    $environment = Get-AdminPowerAppEnvironment -EnvironmentName $EnvironmentId
    if ($environment -eq $null)
    {
        Write-Host "No environment was found with the given id."
        return
    }

    $governanceConfiguration = $environment.Internal.properties.governanceConfiguration
    $governanceConfiguration = CoalesceGovernanceConfiguration -GovernanceConfiguration $governanceConfiguration
    if ($governanceConfiguration.protectionLevel -ne "Standard")
    {
        Write-Host "The specified environment is not managed."
        return
    }

    if ($governanceConfiguration.settings.extendedSettings.includeOnHomepageInsights -eq "True")
    {
        Write-Host "The specified environment is already included in PPAC homepage insights cards."
        return
    }
    
    $governanceConfiguration.settings.extendedSettings | Add-Member -MemberType NoteProperty -Name 'includeOnHomepageInsights' -Value "true" -Force

    $response = Set-AdminPowerAppEnvironmentGovernanceConfiguration -EnvironmentName $EnvironmentId -UpdatedGovernanceConfiguration $GovernanceConfiguration

    if ($response.Code -ne 202)
    {
        Write-Host "Failed to include insights for the specified environment in PPAC homepage insights cards."
        Write-Host $response.Internal.Message
        return
    }
    
    Write-Host "Included insights for the specified environment in PPAC homepage insights cards."    
}

function ExcludeInsightsForManagedEnvironmentsInPPACHomePageCards
{
    <#
     .SYNOPSIS
     Excludes insights for the specified Managed environment from PPAC homepage insights cards.
     .DESCRIPTION
     The ExcludeInsightsForManagedEnvironmentsInPPACHomePageCards cmdlet excludes insights for the specified Managed environment from PPAC homepage insights cards.
     Use Get-Help ExcludeInsightsForManagedEnvironmentsInPPACHomePageCards -Examples for more details.
     .PARAMETER EnvironmentId
     The id (usually a GUID) of the environment.
     .EXAMPLE
     ExcludeInsightsForManagedEnvironmentsInPPACHomePageCards -EnvironmentId 8d996ece-8558-4c4e-b459-a51b3beafdb4
     Excludes insights for Managed environment with id 8d996ece-8558-4c4e-b459-a51b3beafdb4 in PPAC homepage insights cards.
    #>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$EnvironmentId
    )

    Write-Host "Retrieving environment."

    $environment = Get-AdminPowerAppEnvironment -EnvironmentName $EnvironmentId
    if ($environment -eq $null)
    {
        Write-Host "No environment was found with the given id."
        return
    }

    $governanceConfiguration = $environment.Internal.properties.governanceConfiguration
    $governanceConfiguration = CoalesceGovernanceConfiguration -GovernanceConfiguration $governanceConfiguration
    if ($governanceConfiguration.protectionLevel -ne "Standard")
    {
        Write-Host "The specified environment is not managed."
        return
    }

    if ($governanceConfiguration.settings.extendedSettings.includeOnHomepageInsights -eq "False")
    {
        Write-Host "The specified environment is already excluded from PPAC homepage insights cards."
        return
    }
    
    $governanceConfiguration.settings.extendedSettings | Add-Member -MemberType NoteProperty -Name 'includeOnHomepageInsights' -Value "false" -Force

    $response = Set-AdminPowerAppEnvironmentGovernanceConfiguration -EnvironmentName $EnvironmentId -UpdatedGovernanceConfiguration $GovernanceConfiguration
    if ($response.Code -ne 202)
    {
        Write-Host "Failed to exclude insights for the specified environment in PPAC homepage cards."
        Write-Host $response.Internal.Message
        return
    }
    
    Write-Host "Excluded insights for the specified environment in PPAC homepage cards."    
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
        [string]$EnvironmentSku = "Production"
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

function CoalesceGovernanceConfiguration
{
    <#
     .SYNOPSIS
     Internal helper method. Coalesces the given governance configuration object by initializing it if it is null.
     .DESCRIPTION
     The CoalesceGovernanceConfiguration cmdlet returns a non-null copy of the given governance configuration object.
     Use Get-Help CoalesceGovernanceConfiguration -Examples for more details.
     .PARAMETER GovernanceConfiguration
     The governance configuration property of an environment.
     .EXAMPLE
     $GovernanceConfiguration = $null
     CoalesceGovernanceConfiguration -GovernanceConfiguration $GovernanceConfiguration
     Returns a governance configuration object with protectionLevel set to "Basic" and empty settings.
     .EXAMPLE
     $GovernanceConfiguration = [pscustomobject]@{
          protectionLevel = "Basic"
         settings = [pscustomobject]@{
             extendedSettings = @{}
         }
     }
     CoalesceGovernanceConfiguration -GovernanceConfiguration $GovernanceConfiguration
     Returns the provided governance configuration object as is.
    #>
    param
    (
        [Parameter(Mandatory = $true)]
        [object]$GovernanceConfiguration
    )
    
    if ($GovernanceConfiguration -eq $null -or $GovernanceConfiguration.protectionLevel -eq $null)
    {
        $GovernanceConfiguration = [pscustomobject]@{
            protectionLevel = "Basic"
            settings = [pscustomobject]@{
                extendedSettings = @{}
            }
        }
    }
    
    if ($GovernanceConfiguration.settings -eq $null -or $GovernanceConfiguration.settings.extendedSettings -eq $null)
    {
        $GovernanceConfiguration = [pscustomobject]@{
            protectionLevel = $GovernanceConfiguration.protectionLevel
            settings = [pscustomobject]@{
                extendedSettings = @{}
            }
        }
    }
    
    return $GovernanceConfiguration
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

function IsFalse
{
    param
    (
        [Parameter(Mandatory = $true)]
        [bool]$Result,

        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    if ($Result)
    {
        throw $Message
    }
}

function IsTrue
{
    param
    (
        [Parameter(Mandatory = $true)]
        [bool]$Result,

        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    if (-Not $Result)
    {
        throw $Message
    }
}

function CheckConnectorExist
{
    param
    (
        [Parameter(Mandatory = $true)]
        [object]$Policy,

        [Parameter(Mandatory = $true)]
        [string]$Classification,

        [Parameter(Mandatory = $true)]
        [string]$ConnectorName
    )

    $group = $Policy.connectorGroups | Where-Object { $_.classification -eq $Classification }
    $connector = $group.connectors | where { $_.name -eq $ConnectorName }

    if ($connector -eq $null)
    {
        # the connector is not in the connector group
        return $false
    }

    return $true
}

function WriteStack
{
    Write-Host ('=' * 70) -ForegroundColor red
    Write-Host $_.Exception.Message -ForegroundColor red
    Write-Host $_.ScriptStackTrace -ForegroundColor red
    Write-Host ('=' * 70) -ForegroundColor red
}
