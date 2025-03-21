Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

function GetCMKEnterprisePolicyForEnvironment
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$environmentId,

        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod")]
        [String]$endpoint = "prod"

    )
    
    if (-not(Connect-Azure))
    {
        return
    }

    GetEnterprisePolicyForEnvironment -policyType cmk -environmentId $environmentId -endpoint $endpoint
}
GetCMKEnterprisePolicyForEnvironment