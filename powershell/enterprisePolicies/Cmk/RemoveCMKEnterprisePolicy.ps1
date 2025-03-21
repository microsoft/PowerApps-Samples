Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

function RemoveCMKEnterprisePolicy
{
     param(
        [Parameter(
            Mandatory=$true,
            HelpMessage="The Policy Id"
        )]
        [string]$policyArmId
    )

    if (-not(Connect-Azure))
    {
        return
    }

    $policy = RemoveEnterprisePolicy $policyArmId
    if ($policy -eq "true")
    {
      Write-Host "Policy removed"  -ForegroundColor Green
      return
    }
    Write-Host "Policy not removed"  -ForegroundColor Red
}
RemoveCMKEnterprisePolicy