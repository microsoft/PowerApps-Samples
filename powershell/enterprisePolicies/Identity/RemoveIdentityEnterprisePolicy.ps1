# Load the environment script
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function RemoveIdentityEnterprisePolicy
{
     param(
        [Parameter(
            Mandatory=$true,
            HelpMessage="The Policy Id"
        )]
        [string]$policyArmId
    )

    Write-Host "Logging In..." -ForegroundColor Green
    $connect = AzureLogin
    if ($false -eq $connect)
    {
        return
    }

    Write-Host "Logged In..." -ForegroundColor Green

    $policy = RemoveEnterprisePolicy $policyArmId
    if ($policy -eq "true")
    {
      Write-Host "Policy removed"  -ForegroundColor Green
      return
    }
    Write-Host "Policy not removed"  -ForegroundColor Red
}
RemoveIdentityEnterprisePolicy