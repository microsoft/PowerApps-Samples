# Load thescript
. "$PSScriptRoot\..\Common\EnvironmentEnterprisePolicyOperations.ps1"

function GetCMKEnterprisePolicyForEnvironment
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$environmentId,

        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod")]
        [String]$endpoint

    )

    if (![bool]$endpoint) {
        $endpoint = "prod"
    }
    
    GetEnterprisePolicyForEnvironment -policyType cmk -environmentId $environmentId -endpoint $endpoint
}
GetCMKEnterprisePolicyForEnvironment