Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

function RemoveCustomerManagedKeyFromEnvironment 
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$environmentId,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$policyArmId,

        [Parameter(Mandatory=$false)]
        [ValidateSet("tip1", "tip2", "prod")]
        [String]$endpoint = "prod"

    )

    if (-not(Connect-Azure))
    {
        return
    }

    UnLinkPolicyFromEnv -policyType cmk -environmentId $environmentId -policyArmId $policyArmId -endpoint $endpoint    
}
RemoveCustomerManagedKeyFromEnvironment