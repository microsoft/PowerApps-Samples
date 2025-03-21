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

LinkPolicyToPlatformAppsData -policyType cmk  -policyArmId $policyArmId  -endpoint $endpoint  
