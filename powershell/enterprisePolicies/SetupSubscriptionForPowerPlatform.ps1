# Load thescript
. "$PSScriptRoot\Common\EnterprisePolicyOperations.ps1"

function SetupSubscriptionForPowerPlatform
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$subscriptionId

    )
    
    Write-Host "Logging In..." -ForegroundColor Green
    $connect = AzureLogin
    Write-Host "Logged In" -ForegroundColor Green
    Set-AzContext -Subscription $subscriptionId

    # Register the subscription
    Write-Host "Registering the subscription for Microsoft.PowerPlatform"
    $register = Register-AzResourceProvider -ProviderNamespace Microsoft.PowerPlatform
    if ($null -eq $register -or $null -eq $register.RegistrationState)
    {
        $registerString = $register | ConvertTo-Json
        Write-Host "Registration failed for subscription $subscription $registerString" -ForegroundColor Red
    }

    if ($register.RegistrationState -eq "Registered")
    {
       Write-Host "Subscription registered for Microsoft.PowerPlatform" 
    }
    else
    {
        Write-Host "Registration of the suscription for Microsoft.PowerPlatform started"
        $registrationState = "Registering"
        while (-not ($registrationState -eq "Registered"))
        {
            Write-Host "Polling for registration after 60 seconds."
            start-sleep  -Seconds (60)
            Write-Host "Getting the RegistrationState."
            $resourceProvider = Get-AzResourceProvider -ProviderNamespace Microsoft.PowerPlatform
            $registrationState = $resourceProvider.RegistrationState[0]
        }
        Write-Host "Subscription registered for Microsoft.PowerPlatform" 
    }
    
    #Enable the feature
    Write-Host "Registering the subscription for feature enterprisePoliciesPreview for Microsoft.PowerPlatform"
    $register = Register-AzProviderFeature -FeatureName enterprisePoliciesPreview -ProviderNamespace Microsoft.PowerPlatform
    if ($null -eq $register -or $null -eq $register.RegistrationState)
    {
        $registerString = $register | ConvertTo-Json
        Write-Host "Registration failed for feature enterprisePoliciesPreview for subscription $subscription $registerString" -ForegroundColor Red
    }
    Write-Host "Subscription registered for feature enterprisePoliciesPreview for Microsoft.PowerPlatform"        
}
SetupSubscriptionForPowerPlatform