Import-Module (Join-Path (Split-Path $script:MyInvocation.MyCommand.Path) "Microsoft.PowerApps.Administration.PowerShell.Samples.psm1") -Force

#
# Scenario tests
#

function RunTests
{
    param
    (
        [Parameter(Mandatory = $false)]
        [string]$EnvironmentDisplayName = "Test Environment",

        [Parameter(Mandatory = $false)]
        [string]$EndPoint = "prod",

        [Parameter(Mandatory = $false)]
        [string]$TenantAdminName = "tenant admin account",

        [Parameter(Mandatory = $false)]
        [string]$TenantAdminPassword = "tenant admin password",

        [Parameter(Mandatory = $false)]
        [string]$EnvironmentAdminName = "environment admin account",

        [Parameter(Mandatory = $false)]
        [string]$EnvironmentAdminPassword = "environment admin password"
    )

    <# login with user name and password #>
    $Password = ConvertTo-SecureString $TenantAdminPassword -AsPlainText -Force
    Add-PowerAppsAccount -Endpoint $EndPoint -Username $TenantAdminName -Password $Password

    $StartTime = Get-Date
    Write-Host "`r`n`r`nTests started at $StartTime.`r`nThe tests will run about 5 minutes.`r`n"
    
    # 1. Clean test policies
    # 2. Create an empty test policy for ALLEnvironments with policy dispay name dng environment type
    # 3. Add test connectors.
    # 4. Add the same connector to all groups test.
    # 5. Get all policies
    # 6. Update test policy for ALLEnvironments
    # 7. Remove policy AllEnvironments
    AllEnvironmentsPolicyTests -EnvironmentDisplayName $EnvironmentDisplayName

    # 1. Clean test policies
    # 2. Create test policy for OnlyEnvironments
    # 3. Change EnvironmentType from OnlyEnvironments to ExceptEnvironments
    # 4. Remove the test policy
    ChangeOnlyToExceptEnvironmentsPolicyTests -EnvironmentDisplayName $EnvironmentDisplayName
    
    # 1. Clean test policies
    # 2. Change to a user who is not GlobalAdmin
    # 3. Create test policy for SingleEnvironment
    # 4. Get all policies
    # 5. Update test policy for SingleEnvironment
    # 6. Remove policy for SingleEnvironment
    # 7. Change user back to GlobalAdmin
    SingleEnvironmentPolicyTests  `
        -EnvironmentDisplayName $EnvironmentDisplayName `
        -EndPoint $EndPoint `
        -TenantAdminName $TenantAdminName `
        -TenantAdminPassword $TenantAdminPassword `
        -EnvironmentAdminName $EnvironmentAdminName `
        -EnvironmentAdminPassword $EnvironmentAdminPassword
    
    # 1. Clean test policies
    # 2. Create a test policy for AllEnvironments
    # 3. Create a test policy for OnlyEnvironments
    # 4. Create a test policy for ExeptEnvironments
    # 5. Get all policies
    # 6. Check each policy and remove the policy if matched
    CreateListMultiplePoliciesTests -EnvironmentDisplayName $EnvironmentDisplayName
    
    # 1. Clean test policies
    # 2. Create a new test policy for AllEnvironments
    # 3. Remove a connector from Confidential group
    # 4. Add the connector to Blocked group
    # 5. Update policy
    MoveConnectorCrossGroupsTests
        
    # 1. Clean test policies
    # 2. Create a new test policy for AllEnvironments
    # 3. Create an old test tenant policy
    # 5. Get all policies with new API
    # 6. Check each policy and remove the policy with new API if matched
    OldAPIToNewAPICompatibilityTests -EnvironmentDisplayName $EnvironmentDisplayName
        
    # 1. Clean test policies
    # 2. Create a new AllEnvironments test policy
    # 3. Create an new OnlyEnvironments test tenant policy
    # 4. Create an new ExceptEnvironments test tenant policy
    # 5. Get all policies with old API
    # 6. Check each policy and remove the policy with old API if matched
    NewAPIToOldAPICompatibilityTests -EnvironmentDisplayName $EnvironmentDisplayName
    
    # 1. Get connector shared_msnweather actions.
    # 2. Get dlp policies
    # 3. Create tenant policy if not exist.
    # 4. Get policy connector configuration.
    # 5. Create a new policy connector configurations if not exist.
    # 6. Create a connector actions configuration if not exist
    # 7. Loop through policy connector action configurations and find the connector based on connector Id.
    # 8. If the connector action configuration does not exist, add the connector action configuration.
    # 9. Loop through policy connector action configurations action rules and find the action rule based on connector action.
    #10. If the action rule does not exist, add the action rule.
    #11. Create/Update the policy connector configuration.
    #12. Remove the policy connector configuration if it already exist.
    DLPPolicyConnectorActionControlCrud
    
    # 1. Get connector shared_sql actions.
    # 2. Get dlp policies
    # 3. Create tenant policy if not exist.
    # 4. Get policy connector configuration.
    # 5. Create a new policy connector configurations if not exist.
    # 6. Create a connector endpoint configuration if not exist
    # 7. Loop through policy connector endpoint configurations and find the connector based on connector Id.
    # 8. If the connector endpoint configuration does not exist, add the connector endpoint configuration.
    # 9. Loop through policy connector endpoint configurations endpoint rules and find the endpoint rule based on the endpoint.
    #10. If the endpoint rule found, update the endpoint rule.
    #11. If there is no endpoint rule exist, add the last endpoint rule.
    #12. If there is endpoint rule exist, add a new endpoint rule, and re-sort endpoint rules.
    #13. Create/Update the policy connector configuration.
    #14. Remove the policy connector configuration if it already exist.
    DLPPolicyConnectorEndpointControlCrud
    
    # 1. Change to EnvironmentAdmin
    # 2. Create an environment if not exist
    # 3. Call Add-CustomConnectorToPolicySample
    # 4. Call Remove-CustomConnectorToPolicySample
    # 5. Call Add-ConnectorToBusinessDataGroupSample
    # 6. Call Remove-ConnectorToBusinessDataGroupSample
    CustomerConnectorUpdateTests  `
        -EnvironmentDisplayName $EnvironmentDisplayName `
        -EndPoint $EndPoint `
        -TenantAdminName $TenantAdminName `
        -TenantAdminPassword $TenantAdminPassword `
        -EnvironmentAdminName $EnvironmentAdminName `
        -EnvironmentAdminPassword $EnvironmentAdminPassword

    <# Read environment Ids from file .\environmentIds.txt:
        a337c8e0-01bb-42a0-8a96-c979b8e988a0
        f4554dd6-7c3f-4c46-9a9b-b51af94f36e7
    #>
    [string[]]$environmentIds = Get-Content -Path .\environmentIds.txt

    # 1. Get Teams environments
    # 2. Get the specified policy
    # 3. Relace environments for OnlyEnvironments type
    # 4. Add environments to ExceptEnvironments policy
    UpdatePolicyEnvironmentsForTeams  `
        -OnlyEnvironmentsPolicyName "9d903089-f712-4877-8e99-f6c96bd615b7" `
        -OnlyEnvironmentsPolicyDisplayName "Policy test for Teams" `
        -ExceptEnvironmentsPolicyName "2ab49607-12fc-4b7d-8ee7-d21576561081" `
        -ExceptEnvironmentsPolicyDisplayName "Exception policy test for teams" `
        -ExceptionEnvironmentIds $environmentIds

    $EndTime = Get-Date
    $TimeSpan = New-TimeSpan -Start $StartTime -End $EndTime
    Write-Host "`r`n`r`nAll tests completed at $EndTime.`r`nTotal running time: $TimeSpan`r`n"
}

RunTests