# Load thescript
. "$PSScriptRoot\..\Common\EnvironmentEnterprisePolicyOperations.ps1"

function GetSubnetInjectionEnterprisePolicyForEnvironment
{
    param(
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
    
    GetEnterprisePolicyForEnvironment -policyType vnet -environmentId $environmentId -endpoint $endpoint
}
GetSubnetInjectionEnterprisePolicyForEnvironment