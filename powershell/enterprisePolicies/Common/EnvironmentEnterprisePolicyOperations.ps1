# Load the environment script
. "$PSScriptRoot\EnvironmentOperations.ps1"

# Load the environment script
. "$PSScriptRoot\EnterprisePolicyOperations.ps1"

function Login($endpoint) {

    $logIn = $false

    # Login - only needs to be run once per session
    if ($null -eq $global:currentSession.userId) {
        $logIn = $true
    }

    if (($null -eq $global:currentSession.expiresOn) -or (get-date $global:currentSession.expiresOn) -lt (Get-Date)) {
        $logIn = $true
    }

    $envSearch = $env + "*"

    if ($global:currentSession.bapEndpoint -notlike $envSearch) {
        $logIn = $true
    }

    if ($logIn) {
        $result = Add-PowerAppsAccount -Endpoint $endpoint
        echo $result
    }

    $environment = "AzureCloud"
    if (($endpoint -eq "usgovhigh") -or ($endpoint -eq "dod")) {
        $environment = "AzureUSGovernment"
    }
    elseif ($endpoint -eq "china") {
        $environment = "AzureChinaCloud"
    }
    $connect = Connect-AzAccount -Environment $environment

    if ($null -eq $connect)
    {
        Write-Host "Error connecting to Azure Account `n" -ForegroundColor Red
        return $false
    }
    return $true
}

function LinkPolicyToEnv 
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateSet("cmk","vnet", "identity")]
        [ValidateNotNullOrEmpty()]
        [String]$policyType,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$environmentId,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$policyArmId,

        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod", "usgovhigh", "dod", "china")]
        [String]$endpoint

    )

    if (![bool]$endpoint) {
        $endpoint = "prod"
    }

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = Login $endpoint
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green

    #Validate Environment
    $env = GetEnvironment $environmentId

    if ($env -eq $null) 
    {
        return
    }
    Write-Host "Environment reterieved `n" -ForegroundColor Green

    #Validate Enterprise Policy
    $policySystemId = GetEnterprisePolicySystemId $policyArmId
    if ($null -eq $policySystemId)
    {
        return
    }
    Write-Host "Enterprise Policy reterieved `n" -ForegroundColor Green


    $linkResult = LinkEnterprisePolicy $env $policyType $policySystemId

    $linkResultString = $linkResult | ConvertTo-Json

    if ($null -eq $linkResult -or $linkResult.StatusCode -ne "202")
    {
        Write-Host "Linking of $policyType policy did not start for environement $environmentId"
        Write-Host "Error: $linkResultString"
        return 
    }

    # Don't do polling for identity ep linking since its not a long running operation
    if ($policyType -eq "identity")
    {
        Write-Host "Start linking of identity enterprise policy, Reponse received from link request: $linkResultString"
        return 
    }

    Write-Host "Linking of $policyType policy started for environement $environmentId"
    $Headers = $linkResult.Headers

    Write-Host "Do you want to poll the linking operation (y/n)"
    $poll = Read-Host

    if ("n" -eq $poll)
    {
        return
    }

    # Poll the operation every retry-after seconds
    $operationLocation = $headers.'operation-location'
    $retryAfter = $headers.'Retry-After'
    Write-Host "Polling the link operation every $retryAfter seconds."

    PollLinkUnlinkOperation $operationLocation $retryAfter
}

function UnLinkPolicyFromEnv 
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateSet("cmk","vnet", "identity")]
        [ValidateNotNullOrEmpty()]
        [String]$policyType,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$environmentId,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$policyArmId,

        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod", "usgovhigh", "dod", "china")]
        [String]$endpoint

    )

    if (![bool]$endpoint) {
        $endpoint = "prod"
    }

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = Login $endpoint
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green

    #Validate Environment
    $env = GetEnvironment $environmentId

    if ($env -eq $null) 
    {
        return
    }
    Write-Host "Environment reterieved `n" -ForegroundColor Green

    $epPropertyName = switch ( $policyType )
    {
        "cmk" { "CustomerManagedKeys" }
        "vnet" { "VNets" }
        "identity" { "Identity" }
    }
    
    if ($null -eq $env.properties.enterprisePolicies -or $null -eq $env.properties.enterprisePolicies.$epPropertyName)
    {
        Write-Host "No enterprise policy present to remove for environement $environmentId"
        return
    }

    if ($policyArmId -ine $env.properties.enterprisePolicies.$epPropertyName.id)
    {
        Write-Host "Given policyArmId $policyArmId not matching with $policyType policy ArmId for environement $environmentId"
        return 
    }


    #Validate Enterprise Policy
    $policySystemId = GetEnterprisePolicySystemId $policyArmId
    if ($null -eq $policySystemId)
    {
        return
    }
    Write-Host "Enterprise Policy reterieved `n" -ForegroundColor Green


    $unLinkResult = UnLinkEnterprisePolicy $env $policyType $policySystemId

    $unLinkResultString = $UnLinkResult | ConvertTo-Json

    if ($null -eq $unLinkResult -or $unLinkResult.StatusCode -ne "202")
    {
        Write-Host "Unlinking of $policyType policy did not start for environement $environmentId"
        Write-Host "Error: $unLinkResultString"
        return 
    }

    # Don't do polling for identity ep unlinking since its not a long running operation
    if ($policyType -eq "identity")
    {
        Write-Host "Start unlinking of identity enterprise policy, Reponse received from link request: $unLinkResultString"
        return 
    }

    Write-Host "Unlinking of $policyType policy started for environement $environmentId"
    $headers = $unlinkResult.Headers

    Write-Host "Do you want to poll the unlink operation (y/n)"
    $poll = Read-Host

    if ("n" -eq $poll)
    {
        return
    }

    # Poll the operation every retry-after seconds
    $operationLocation = $headers.'operation-location'
    $retryAfter = $headers.'Retry-After'
    Write-Host "Polling the unlink operation every $retryAfter seconds."

    PollLinkUnlinkOperation $operationLocation $retryAfter
    
}

function SwapPolicyForEnv 
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateSet("cmk","vnet", "identity")]
        [ValidateNotNullOrEmpty()]
        [String]$policyType,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$environmentId,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$policyArmId,

        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod", "usgovhigh", "dod", "china")]
        [String]$endpoint

    )

    if (![bool]$endpoint) {
        $endpoint = "prod"
    }

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = Login $endpoint
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green

    #Validate Environment
    $env = GetEnvironment $environmentId

    if ($env -eq $null) 
    {
        return
    }
    Write-Host "Environment reterieved `n" -ForegroundColor Green

    $epPropertyName = switch ( $policyType )
    {
        "cmk" { "CustomerManagedKeys" }
        "vnet" { "VNets" }
        "identity" { "Identity" }
    }
    
    if ($null -eq $env.properties.enterprisePolicies -or $null -eq $env.properties.enterprisePolicies.$epPropertyName)
    {
        Write-Host "No enterprise policy of $policyType present to swap for environement $environmentId"
        return
    }

    #Validate Enterprise Policy
    $policySystemId = GetEnterprisePolicySystemId $policyArmId
    if ($null -eq $policySystemId)
    {
        return
    }
    Write-Host "Enterprise Policy reterieved `n" -ForegroundColor Green


    $swapResult = LinkEnterprisePolicy $env $policyType $policySystemId

    $swapResultString = $swapResult | ConvertTo-Json

    if ($null -eq $swapResult -or $swapResult.StatusCode -ne "202")
    {
        Write-Host "Swapping of $policyType policy did not start for environement $environmentId"
        Write-Host "Error: $swapResultString"
        return 
    }

    # Not do polling for identity ep swapping since its not a long running operation
    if ($policyType -eq "identity")
    {
        Write-Host "Start swapping of identity enterprise policy, Reponse received from link request: $swapResultString"
        return 
    }

    Write-Host "Swapping of $policyType policy started for environement $environmentId"
    $headers = $swapResult.Headers

    Write-Host "Do you want to poll the swapping operation (y/n)"
    $poll = Read-Host

    if ("n" -eq $poll)
    {
        return
    }

    # Poll the operation every retry-after seconds
    $operationLocation = $headers.'operation-location'
    $retryAfter = $headers.'Retry-After'
    Write-Host "Polling the swap operation every $retryAfter seconds."

    PollLinkUnlinkOperation $operationLocation $retryAfter
    
}


function GetEnterprisePolicyForEnvironment 
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateSet("cmk","vnet", "identity")]
        [ValidateNotNullOrEmpty()]
        [String]$policyType,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$environmentId,

        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod", "usgovhigh", "dod", "china")]
        [String]$endpoint

    )

    if (![bool]$endpoint) {
        $endpoint = "prod"
    }

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = Login $endpoint
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green

    #Validate Environment
    $env = GetEnvironment $environmentId

    if ($env -eq $null) 
    {
        return
    }
    Write-Host "Environment reterieved `n" -ForegroundColor Green

    $epPropertyName = switch ( $policyType )
    {
        "cmk" { "CustomerManagedKeys" }
        "vnet" { "VNets" }
        "identity" {"Identity"}
    }
    
    if ($null -eq $env.properties.enterprisePolicies -or $null -eq $env.properties.enterprisePolicies.$epPropertyName)
    {
        Write-Host "No enterprise policy present of $policyType in environement $environmentId"
        return
    }

    Write-Host "Enterprise Policy of type $policyType reterived for environment $environmentId `n" -ForegroundColor Green
    $policyArmId = $env.properties.enterprisePolicies.$epPropertyName.id
    Write-Host "Enterprise Policy Arm Id $policyArmId"
}

function LinkPolicyToPlatformAppsData 
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateSet("cmk","vnet", "identity")]
        [ValidateNotNullOrEmpty()]
        [String]$policyType,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$policyArmId,

        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod", "usgovhigh", "dod", "china")]
        [String]$endpoint

    )

    if (![bool]$endpoint) {
        $endpoint = "prod"
    }

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = Login $endpoint
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green

     #Validate PlatformApps enrollment
    $platformAppsStatus = GetPlatformApps

    if ($platformAppsStatus -eq $null -or $platformAppsStatus.enrollmentState -ne "Enrolled") 
    {
        Write-Host "PlatformApps not enrolled"
        return
    }
    Write-Host "PlatformApps enrolled `n" -ForegroundColor Green

    #Validate Enterprise Policy
    $policySystemId = GetEnterprisePolicySystemId $policyArmId
    if ($null -eq $policySystemId)
    {
        return
    }
    Write-Host "Enterprise Policy reterieved `n" -ForegroundColor Green


    $linkResult = LinkEnterprisePolicyToPlatformAppsData $policyType $policySystemId

    $linkResultString = $linkResult | ConvertTo-Json

    if ($null -eq $linkResult -or $linkResult.StatusCode -ne "202")
    {
        Write-Host "Linking of $policyType policy did not start for platformapps"
        Write-Host "Error: $linkResultString"
        return 
    }

    Write-Host "Linking of $policyType policy started for platformapps"
}


function UnLinkPolicyFromPlatformAppsData 
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateSet("cmk","vnet", "identity")]
        [ValidateNotNullOrEmpty()]
        [String]$policyType,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$policyArmId,

        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod", "usgovhigh", "dod", "china")]
        [String]$endpoint

    )

    if (![bool]$endpoint) {
        $endpoint = "prod"
    }

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = Login $endpoint
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green

    $epPropertyName = switch ( $policyType )
    {
        "cmk" { "CustomerManagedKeys" }
        "vnet" { "VNets" }
        "identity" { "Identity" }
    }

     #Validate PlatformApps enrollment
    $platformAppsStatus = GetPlatformApps

    if ($platformAppsStatus -eq $null -or $platformAppsStatus.enrollmentState -ne "Enrolled") 
    {
        Write-Host "PlatformApps not enrolled"
        return
    }
    Write-Host "PlatformApps enrolled `n" -ForegroundColor Green
  
    if ($null -eq $platformAppsStatus.enterprisePolicies -or $null -eq $platformAppsStatus.enterprisePolicies.$epPropertyName)
    {
        Write-Host "No enterprise policy present of type $policyType to remove from PlatformApps"
        return
    }

    if ($policyArmId -ine $platformAppsStatus.enterprisePolicies.$epPropertyName.id)
    {
        Write-Host "Given policyArmId $policyArmId not matching with $policyType policy ArmId for PlatformApps"
        return 
    }
    
    #Validate Enterprise Policy
    $policySystemId = GetEnterprisePolicySystemId $policyArmId
    if ($null -eq $policySystemId)
    {
        return
    }
    Write-Host "Enterprise Policy reterieved `n" -ForegroundColor Green


    $unLinkResult = UnLinkEnterprisePolicyForPlatformAppsData $policyType $policySystemId

    $unLinkResultString = $UnLinkResult | ConvertTo-Json

    if ($null -eq $unLinkResult -or $unLinkResult.StatusCode -ne "202")
    {
        Write-Host "Unlinking of $policyType policy did not start for PlatformApps"
        Write-Host "Error: $unLinkResultString"
        return 
    }

    Write-Host "Unlinking of $policyType policy started for PlatformApps"
    
}
