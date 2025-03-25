<#
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS. MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED. YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
#>

Import-Module "$PSScriptRoot\..\Common\EnterprisePolicies" -Force

function GetAndValidateKeyVaultProperties($keyVaultName)
{
    Write-Host "Getting KeyVault $keyVaultName" -ForegroundColor Green `n
    $keyVault = Get-AzKeyVault -VaultName $keyVaultName
    $keyVaultString = $keyVault | ConvertTo-Json
    if ($keyVault -eq $nul -or $null -eq $keyVault.VaultName)
    {
        Write-Host "Could not retrieve vault $keyVaultName  $keyVaultString. Please check if key vault exists and accessible" -ForegroundColor Red 
        return $null
    }

     #validate soft-delete
    if ($null -eq $keyVault.EnableSoftDelete -or  $keyVault.EnableSoftDelete.Equals("False"))
    {
       Write-Host "Soft delete not enabled for keyVault $keyVaultName. Please enable it as per the instruction at https://learn.microsoft.com/azure/key-vault/general/soft-delete-change " -ForegroundColor Red
       return $null
    }

    #validate purge-protection
    if ($null -eq $keyVault.EnablePurgeProtection -or $keyVault.EnablePurgeProtection.Equals("False"))
    {
       Write-Host "Purge protection not enabled for keyVault $keyVaultName. Please enable it as per the instruction at https://learn.microsoft.com/azure/key-vault/general/soft-delete-overview#permitted-purge" -ForegroundColor Red
       return $null
    }
    Write-Host "KeyVault $keyVaultName reterieved with soft delete and purge protection enabled" -ForegroundColor Green `n

    return $keyVault
}

function GetAndValidateEnterprisePolicyForKeyVault($enterprisePolicyArmId, $keyVault)
{
    Write-Host "Getting CMK enterprise policy" -ForegroundColor Green `n
    $cmkPolicy = Get-EnterprisePolicy -PolicyArmId $enterprisePolicyArmId
    $cmkPolicyString = $cmkPolicy | ConvertTo-Json
    if ($null -eq $cmkPolicy.ResourceId)
    {
       
        Write-Host "Could not reterieve CMK Policy $enterprisePolicyArmId  $cmkPolicyString" -ForegroundColor Red
        return $null
    }

    #validate enterprise policy is of kind encryption
    if ($cmkPolicy.Kind -ne "Encryption")
    {
       
        Write-Host "Enterprise Policy retrieved for $enterprisePolicyArmId  is not of Kind Encryption. Enterprise Policy = $cmkPolicyString" -ForegroundColor Red
        return $null
    }

    #validate enterprise policy is having SystemAssigned identity
    if ($null -eq $cmkPolicy.Identity -or $cmkPolicy.Identity.Type -ne "SystemAssigned")
    {
       
        Write-Host "Enterprise Policy retrieved for $enterprisePolicyArmId is not having SystemAssigned identity. Enterprise Policy = $cmkPolicyString" -ForegroundColor Red
        return $null
    }

    #validate enterprise policy key vault configuration is same as given keyVault
    $epKeyVaultConfig = $cmkPolicy.Properties.Encryption.KeyVault
    if ($epKeyVaultConfig.Id -ne $keyVault.ResourceId)
    {
       
        Write-Host "Enterprise Policy retrieved for $enterprisePolicyArmId is not having same key vault config as $keyVaultName. Enterprise Policy = $cmkPolicyString" -ForegroundColor Red
        return $null
    }
   
    #check if key vault has vault access policy
    if ($null -ne $keyVault.AccessPolicies)
    {
        #validate CMK enterprise policy identity has Get, UnwrapKey and WrapKey access permission for key vault
        $accessPolicies = $keyVault.AccessPolicies
        $epAccessPolicy = $accessPolicies | Where-Object {$_.ObjectId -in $cmkPolicy.Identity.PrincipalId}
        if ("Get" -notin $epAccessPolicy.PermissionsToKeys)
        {
            Write-Host "Get access not present for Enterprise Policy $enterprisePolicyArmId in keyVault $keyVaultName" -ForegroundColor Red
            return $null
        }
        if ("UnwrapKey" -notin $epAccessPolicy.PermissionsToKeys)
        {
            Write-Host "UnwrapKey access not present for Enterprise Policy $enterprisePolicyArmId in keyVault $keyVaultName" -ForegroundColor Red
            return $null
        }
        if ("WrapKey" -notin $epAccessPolicy.PermissionsToKeys)
        {
            Write-Host "WrapKey access not present for Enterprise Policy $enterprisePolicyArmId in keyVault $keyVaultName" -ForegroundColor Red
            return $null
        }
        Write-Host "Enterprise policy $enterprisePolicyArmId retrieved and is valid for $keyvaultName with Get, UnwrapKey and WrapKey access" -ForegroundColor Green `n
    }
    else
    {
        #validate if CMK enterprise policy identity has "Key Vault Crypto Service Encryption User" role assignment
        $epRoleAssignment = Get-AzRoleAssignment -Scope $keyVault.ResourceId -ObjectId $cmkPolicy.Identity.PrincipalId -RoleDefinitionName "Key Vault Crypto Service Encryption User"
        if ($null -eq $epRoleAssignment)
        {
            Write-Host "Enterprise policy $enterprisePolicyArmId identity is not assigned 'Key Vault Crypto Service Encryption User' role" -ForegroundColor Red
            return $null
        }
        Write-Host "Enterprise policy $enterprisePolicyArmId retrieved and is valid for $keyvaultName with 'Key Vault Crypto Service Encryption User' role" -ForegroundColor Green `n

    }

    return $cmkPolicy
}

function GetAndValidateEnterprisePolicyKey($epKeyVaultConfig, $keyVaultName)
{
    #validate Key configured in enterprise policy
    $keyName = $epKeyVaultConfig.Key.Name
    $keyVersion = $epKeyVaultConfig.Key.version

    Write-Host "Validating enterprise policy $enterprisePolicyArmId key $keyName in $keyVaultName" -ForegroundColor Green `n
    $key = $null
    #get the specific key version if it is present in enterprise policy
    if ($null -ne $keyVersion)
    {
        $key = Get-AzKeyVaultKey -VaultName $keyVaultName -keyName $keyName -Version $keyVersion
    }
    else
    {
        $key = Get-AzKeyVaultKey -VaultName $keyVaultName -keyName $keyName
    }
    $keyString = $key | ConvertTo-Json
    if ($null -eq $key -or $null -eq $key.Id)
    {
        Write-Host "Key $keyName not reterieved from $keyVaultName  $keyString" -ForegroundColor Red
        return $null
    }
    #validate if key is enabled
    if ($key.Enabled -ne "True")
    {
        Write-Host "Key $keyName is not enabled" -ForegroundColor Red
        return $null
    }

    #validate if key is valid
    [datetime]$current = Get-Date
    $currentDateInUTC = $current.ToUniversalTime()
    if($null -ne $key.NotBefore)
    {
        [datetime]$notBefore = Get-Date $key.NotBefore
        if ($notBefore -ge $currentDateInUTC)
        {
            Write-Host "Key $keyName is not activated. Activation Date $notBefore" -ForegroundColor Red
            return $null
        }
    }

    if($null -ne $key.Expires)
    {
        [datetime]$expires = Get-Date $key.Expires
        if ($expires -le $currentDateInUTC)
        {
            Write-Host "Key $keyName is expired. Expiry Date $expires" -ForegroundColor Red
            return $null
        }
    }

    Write-Host "Key $keyName for enterprise policy $enterprisePolicyArmId is enabled, activated and not expired" -ForegroundColor Green `n

    return $key
}

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
    
    if (-not(Connect-Azure))
    {
        return
    }
    Set-AzContext -Subscription $subscriptionId | Out-Null

    #validate key vault
    $keyVault = GetAndValidateKeyVaultProperties -keyVaultName $keyVaultName
    if ($null -eq $keyVault)
    {
        return
    }

    #validate enterprise policy
    $cmkPolicy = GetAndValidateEnterprisePolicyForKeyVault -enterprisePolicyArmId $enterprisePolicyArmId -keyVault $keyVault
    if ($null -eq $cmkPolicy)
    {
        return
    }

    #validate key
    $key = GetAndValidateEnterprisePolicyKey -epKeyVaultConfig $cmkPolicy.Properties.Encryption.KeyVault -keyVaultName $keyVaultName
}
ValidateKeyVaultForCMK