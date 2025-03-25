param(
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [String]$policyArmId,

    [Parameter(Mandatory=$false)]
    [BAPEndpoint]$Endpoint = "prod"
)

$ErrorActionPreference = "Stop"

Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

if (-not(Connect-Azure))
{
    return
}

LinkPolicyToPlatformAppsData -policyType cmk  -policyArmId $policyArmId  -endpoint $endpoint  
