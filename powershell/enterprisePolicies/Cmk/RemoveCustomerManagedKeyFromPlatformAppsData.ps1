param(
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [String]$policyArmId,

    [Parameter(Mandatory=$false)]
    [ValidateSet("tip1", "tip2", "prod")]
    [String]$endpoint = "prod"
)

Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

if (-not(Connect-Azure))
{
    return
}

UnLinkPolicyFromPlatformAppsData -policyType cmk -policyArmId $policyArmId -endpoint $endpoint    
