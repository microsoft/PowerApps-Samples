param(
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [String]$policyArmId,

    [Parameter(Mandatory=$false)]
    [ValidateSet("tip1", "tip2", "prod")]
    [String]$endpoint

)

# Load thescript
. "$PSScriptRoot\..\Common\EnvironmentEnterprisePolicyOperations.ps1"

if (![bool]$endpoint) {
    $endpoint = "prod"
}

UnLinkPolicyFromPlatformAppsData -policyType cmk -policyArmId $policyArmId -endpoint $endpoint    
