# Load thescript
. "$PSScriptRoot\..\Common\EnterprisePolicyOperations.ps1"

function ValidateKeyVaultForCMK
{
    param(
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$subscriptionId,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$keyVaultName,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]$enterprisePolicyArmId

    )
    
    Write-Host "Logging In..." -ForegroundColor Green
    AzureLogin
    Write-Host "Logged In" -ForegroundColor Green
    $setSubscription = Set-AzContext -Subscription $subscriptionId

    Write-Host "Getting KeyVault" -ForegroundColor Green
    $keyVault = Get-AzKeyVault -VaultName $keyVaultName
    $keyVaultString = $keyVault | ConvertTo-Json
    if ($null -eq $keyVault -or $null -eq $keyVault.VaultName)
    {
        Write-Host "Could not reterieve vault $keyVaultName  keyVaultString" -ForegroundColor Red
        return
    }
    Write-Host "KeyVault reterieved" -ForegroundColor Green

    Write-Host "Getting CMK enterprise policy" -ForegroundColor Green
    $cmkPolicy = GetEnterprisePolicy $enterprisePolicyArmId
    $cmkPolicyString = $cmkPolicy | ConvertTo-Json
    if ($cmkPolicy.ResourceId -eq $null)
    {
       
        Write-Host "Could not reterieve CMK Policy $enterprisePolicyArmId  $cmkPolicyString" -ForegroundColor Red
        return 
    }
    Write-Host "Enterprise Policy reterieved" -ForegroundColor Green

    #validate soft-delete
    Write-Host "Validating soft-delete for key vault $keyVaultName" -ForegroundColor Green
    if ($keyVault.EnableSoftDelete -eq $null -or  $keyVault.EnableSoftDelete.Equals("False"))
    {
       Write-Host "Soft delete not enabled for keyVault $keyVaultName. Please enable it as per instruction at https://docs.microsoft.com/en-us/azure/key-vault/general/soft-delete-change "
       return
    }
    Write-Host "Soft delete enabled for keyVault $keyVaultName"

    #validate purge-protection
    Write-Host "Validating purge protection for key vault $keyVaultName" -ForegroundColor Green
    if ($keyVault.EnablePurgeProtection -eq $null -or $keyVault.EnablePurgeProtection.Equals("False"))
    {
       Write-Host "Purge protection not enabled for keyVault $keyVaultName. Enabling purge protection " 
       Update-AzKeyVault -VaultName $keyVaultName -ResourceGroup $keyVault.ResourceGroupName -EnablePurgeProtection
    }
    Write-Host "Purge Protection enabled for keyVault $keyVaultName"

    Write-Host "Adding permission Get, UnwrapKey and WrapKey for enterprise policy $enterprisePolicyArmId " -ForegroundColor Green
    $epServicePrincipal = Get-AzADServicePrincipal -SearchString $cmkPolicy.Name
    if ($null -eq $epServicePrincipal.Id)
    {
        Write-Host "Service Principal not found for enterprise policy $enterprisePolicyArmId" -ForegroundColor Red
        return
    }
    $epObjectId = $epServicePrincipal.Id
    try
    {
        Set-AzKeyVaultAccessPolicy -VaultName $keyVaultName -ObjectId $epObjectId -PermissionsToKeys Get,UnwrapKey, WrapKey
        Write-Host "Added permission Get, UnwrapKey and WrapKey for enterprise policy $enterprisePolicyArmId " -ForegroundColor Green
    }
    catch
    {
         Write-Host "Error is adding permission Get, UnwrapKey and WrapKey for enterprise policy $enterprisePolicyArmId " -ForegroundColor Green
    }  
        
}
ValidateKeyVaultForCMK