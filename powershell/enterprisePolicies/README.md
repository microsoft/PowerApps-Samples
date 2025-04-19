# Power Platform Enterprise Policies PowerShell Scripts

These scripts automate managing (create, update, get, delete) Power Platform Enterprise Policies as Azure resources.</br>
In addition, we are providing sample scripts on how to associate these policies with Power Platform environments.</br>
Please note that these scripts are provided under MIT license and its usage is the sole responsibility of the user.

## How to run setup scripts

1. **Install modules script** : This script installs the required modules to run Enterprise Policies scripts.</br>
Script name: InstallPowerAppsCmdlets.ps1</br>
Run the script to import required PowerShell modules.

2. **Setup Azure subscription for Microsoft.PowerPlatform** : This script registers the Azure subscription for Microsoft.PowerPlatform resource provider </br>
and also allow lists the subscription for enterprisePoliciesPreview feature.</br>
Script name : SetupSubscriptionForPowerPlatform.ps1</br>
Run the script to setup Azure subscription for Microsoft.PowerPlatform

## How to run CMK scripts

The CMK scripts are present in folder Cmk at current location

### Create CMK Enterprise policy
1. **Create CMK Enterprise Policy** : This script creates a CMK enterprise policy</br>
Script name : CreateCMKEnterprisePolicy.ps1</br>
Input parameters :
    - subscriptionId : The subscriptionId where CMK enterprise policy needs to be created
    - resourceGroup : The resource group where CMK enterprise policy needs to be created
    - enterprisePolicyName : The name of the CMK enterprise policy resource
    - enterprisePolicyLocation : The Azure geo where CMK enterprise policy needs to be created. Example: unitedstates, europe, australia.</br>
      To get the complete supported locations for enterprise policy, below command can be used:</br>
      ((Get-AzResourceProvider -ProviderNamespace Microsoft.PowerPlatform).ResourceTypes | Where-Object ResourceTypeName -eq enterprisePolicies).Locations
    - keyVaultId : The ARM resource ID of the key vault used for CMK
    - keyName : The name of the key in the key vault used for CMK
    - keyVersion: The version of the key in the key vault used for CMK

Sample Input :</br>
![alt text](./ReadMeImages/CreateCMKEP1.png)</br>

Sample Output : </br>
![alt text](./ReadMeImages/CreateCMKEP2.png)</br>

### Get CMK Enterprise Policy By ResourceId
2. **Get CMK Enterprise Policy By ResourceId** : The script gets a CMK enterprise policy by ARM resourceId</br>
Script name : GetCMKEnterprisePolicyByResourceId.ps1</br>
Input parameter :
    - enterprisePolicyArmId : The ARM resource ID of the CMK Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/GetCMKByResourceId1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetCMKByResourceId2.png)</br>

### Get CMK Enterprise Policies in Subscription
3. **Get CMK Enterprise Policies in Subscription** : The script gets all CMK enterprise policies in an Azure subscription</br>
Script name : GetCMKEnterprisePoliciesInSubscription.ps1</br>
Input parameter :
    - subscriptionId: : The Azure subscription Id

Sample Input :</br>
![alt text](./ReadMeImages/GetCMKInSub1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetCMKInSub2.png)</br>

### Get CMK Enterprise Policies in Resource Group
4. **Get CMK Enterprise Policies in Resource Group** : The script gets all CMK enterprise policies in an Azure resource group</br>
Script name : GetCMKEnterprisePoliciesInResourceGroup.ps1</br>
Input parameters :
    - subscriptionId : The Azure subscription Id
    - resourceGroup : The Azure resource group

Sample Input : </br>
![alt text](./ReadMeImages/GetCMKInResourceGroup1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetCMKInResourceGroup2.png)</br>

### Validate Azure Key Vault
5. **Validate Azure Key Vault** : This script checks if the Key Vault is setup correctly according to the pre-requisites required by the Power Platform CMK Enterprise Policy. For details please follow the setup instructions at https://learn.microsoft.com/power-platform/admin/customer-managed-key#create-encryption-key-and-grant-access</br>
	Following major validations are performed:
    - Soft-delete is enabled for key vault: Please follow the instructions at </br>
      https://learn.microsoft.com/azure/key-vault/general/soft-delete-change to update the soft delete property.
    - Purge protection is enabled for key vault: Please follow the istructions at </br>
	  https://learn.microsoft.com/azure/key-vault/general/key-vault-recovery?tabs=azure-portal to get details about enabling Purge Protection</br>
	- "Key Vault Crypto Service Encryption User" role assignment is present for the given enterprise policy if key vault permission model is Azure role based access control.</br>
    - Access policies of GET, UNWRAPKEY, WRAPKEY are added to the key vault for the given enterprise policy if key vault permission model is vault access policy.</br>
	- Key configured for the given enterprise policy is present, enabled, activated and not expired.</br>
	 

Script name : ValidateKeyVaultForCMK.ps1</br>
Input parameters:
- subscriptionId : The Azure subscription Id of the Key Vault
- keyVaultName : The name of the key Vault
- enterprisePolicyArmId : The CMK enterprise policy ARM Id 

Sample Input : </br>
![alt text](./ReadMeImages/ValidateKeyVault1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/ValidateKeyVault2.png)</br>

### Update CMK Enterprise Policy
6. **Update CMK Enterprise Policy** : This script updates a CMK Enterprise Policy. The updates allowed are for keyVaultId, keyName, keyVersion.</br>
If you are changing only some of the allowed parameter values, provide “N/A” when prompted for the parameters that you don’t want to change.</br>
 **If the enterprise policy is associated with one or more environments, the update operation will fail, and the script will return an error.**</br>
Script name : UpdateCMKEnterprisePolicy.ps1</br>
Input parameters :
    - subscriptionId : The Azure subscription Id of the CMK Enterprise Policy
    - resourceGroup : The Azure resource group of the CMK Enterprise Policy
    - enterprisePolicyName : The name of the CMK enterprise policy that needs to be updated
    - keyVaultId : The ARM resource ID of the key vault if it needs to be updated. Provide "N/A" if update is not required for key vault Id
    - keyName: The name of the key if it needs to be updated. Provide "N/A" if update is not required for name of the key
    - keyVersion: The version of the key if it needs to be updated. Provide "N/A" if update is not required for version of the key

Sample Input : </br>
![alt text](./ReadMeImages/UpdateCMKEP1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/UpdateCMKEP2.png)</br>

### Delete CMK Enterprise Policy
7. **Delete CMK Enterprise Policy** : This script deletes the CMK Enterprise Policy for a given policy Id. </br>
**If the CMK enterprise policy is associated with one or more environments, the delete operation will fail, and the script will return an error.**</br>
Script name : RemoveCMKEnterprisePolicy.ps1</br>
Input parameter :
    - policyArmId : The ARM ID of the CMK enterprise policy to be deleted

Sample Input : </br>
![alt text](./ReadMeImages/RemoveCMKEP1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/RemoveCMKEP2.png)</br>

### Set CMK for an environment
8. **Set CMK for an environment** : This script applies a CMK enterprise policy to a given Power Platform environment.</br>
The script adds the environment to the enterprise policy and optionally polls for the operation outcome.</br>
Script name : AddCustomerManagedKeyToEnvironment.ps1</br>
Input parameters :
    - environmentId : The Power Platform environment ID
    - policyArmId : The ARM ID of the CMK Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/AddCMKToEnv1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/AddCMKToEnv2.png)</br>

### Get CMK for an environment
9. **Get CMK for an environment** : This script returns the CMK enterprise policy if applied to a given Power Platform environment.</br>
Script name : GetCMKEnterprisePolicyForEnvironment.ps1</br>
Input parameter :
    - environmentId : The Power Platform environment ID

Sample Input :</br>
![alt text](./ReadMeImages/GetCMKForEnv1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetCMKForEnv2.png)</br>

### Remove CMK from an environment
10. **Remove CMK from an environment** : The script removes the CMK enterprise policy from an environment, </br>
which results on data to be encrypted with a Microsoft managed encryption key.</br>
Script name : RemoveCustomerManagedKeyFromEnvironment.ps1</br>
Input parameters :
    - environmentId : The Power Platform environment ID
    - policyArmId: The ARM ID of the CMK Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/RemoveCMKFromEnv1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/RemoveCMKFromEnv2.png)</br>

## How to run Subnet Injection scripts

The Subnet Injection scripts are present in folder SubnetInjection at current location

### 1. **Setup virtual network for Subnet Injection**
This script adds the subnet delegation for Microsoft.PowerPlatform/enterprisePolicies for a given virtual network and subnet </br>
Script name : New-VnetForSubnetDelegation.ps1</br>
Input parameters :
  - virtualNetworkSubscriptionId : The subscriptionId of the virtual network
  - virtualNetworkName : The name of the virtual network
  - subnetName : The name of the virtual network subnet

Sample Input :</br>
```powershell
New-VnetForSubnetDelegation.ps1 `
-virtualNetworkSubscriptionId "98159998-fb68-44c3-b7d8-22b6539499a2" `
-virtualNetworkName "demoVirtualNetwork" `
-subnetName "default"
```

Sample Output : </br>
![alt text](./ReadMeImages/SetupVirtualNetwork2.png)</br>

 ### 2. **Create Subnet Injection Enterprise Policy** 
This script creates a Subnet Injection enterprise policy</br>
Script name : New-SubnetInjectionEnterprisePolicy.ps1</br>
Input parameters :
  - subscriptionId : The subscriptionId where Subnet Injection enterprise policy needs to be created
  - resourceGroup : The resource group where Subnet Injection enterprise policy needs to be created
  - enterprisePolicyName : The name of the Subnet Injection enterprise policy resource
  - enterprisePolicyLocation : The Azure geo where Subnet Injection enterprise policy needs to be created. Example: unitedstates, europe, australia.<br/>
      >[NOTE] To get the complete supported locations for enterprise policy, use the command below:</br>
      ((Get-AzResourceProvider -ProviderNamespace Microsoft.PowerPlatform).ResourceTypes | Where-Object ResourceTypeName -eq enterprisePolicies).Locations
  - vnetId1 : The ARM resource ID of the first virtual network used for Subnet Injection
  - subnetName1 : The name of the subnet in the first virtual network that will be used for Subnet Injection
  - vnetId2 : The ARM resource ID of the second virtual network used for Subnet Injection
  - subnetName2 : The name of the subnet in the second virtual network that will be used for Subnet Injection

Sample Input :</br>
```powershell
New-SubnetInjectionEnterprisePolicy.ps1 `
-subscriptionId "98159998-fb68-44c3-b7d8-22b6539499a2" `
-resourceGroup "enterprisePolicy-snet-delegation" `
-enterprisePolicyName "vnetEP1" `
-enterprisePolicyLocation "unitedstates" `
-vnetId1 "/subscriptions/98159998-fb68-44c3-b7d8-22b6539499a2/resourceGroups/enterprisePolicy-snet-delegation/Providers/Microsoft.Network/virtualNetworks/westus-vnet" `
-subnetName1 "wus-delegated-snet" `
-vnetId2 "/subscriptions/98159998-fb68-44c3-b7d8-22b6539499a2/resourceGroups/enterprisePolicy-snet-delegation/Providers/Microsoft.Network/virtualNetworks/eastus-vnet" `
-subnetName "eus-delegated-snet" `
```

Sample Output : </br>
![alt text](./ReadMeImages/CreateSubnetInjectionEnterprisePolicy2.png)</br>

### 3. **Get Subnet Injection Enterprise Policy By ResourceId**
The script gets a Subnet Injection enterprise policy by ARM resourceId</br>
Script name : Get-SubnetInjectionEnterprisePolicyByResourceId.ps1</br>
Input parameter :
  - enterprisePolicyArmId : The ARM resource ID of the Subnet Injection Enterprise Policy

Sample Input :</br>
```powershell
Get-SubnetInjectionEnterprisePolicyByResourceId.ps1 `
-enterprisePolicyArmId "/subscriptions/98159998-fb68-44c3-b7d8-22b6539499a2/resourceGroups/enterprisePolicy-snet-delegation/Providers/Microsoft.PowerPlatform/enterprisePolicies/vnetEP1"
```

Sample Output :</br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePolicyByResourceId2.png)</br>

### 4. **Get Subnet Injection Enterprise Policies in Subscription** 
The script gets all Subnet Injection enterprise policies in an Azure subscription</br>
Script name : Get-SubnetInjectionEnterprisePoliciesInSubscription.ps1</br>
Input parameter :
  - subscriptionId: : The Azure subscription Id

Sample Input :</br>
```powershell
Get-SubnetInjectionEnterprisePoliciesInSubscription.ps1 `
-subscriptionId "98159998-fb68-44c3-b7d8-22b6539499a2"
```

Sample Output :</br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePoliciesInSubscription2.png)</br>

### 5. **Get Subnet Injection Enterprise Policies in Resource Group**
The script gets all Subnet Injection enterprise policies in an Azure resource group</br>
Script name : Get-SubnetInjectionEnterprisePoliciesInResourceGroup.ps1</br>
Input parameters :
  - subscriptionId : The Azure subscription Id
  - resourceGroup : The Azure resource group

Sample Input : </br>
```powershell
Get-SubnetInjectionEnterprisePoliciesInResourceGroup.ps1 `
-subscriptionId "98159998-fb68-44c3-b7d8-22b6539499a2" `
-resourceGroup "enterprisePolicy-snet-delegation"
```

Sample Output :</br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePoliciesInResourceGroup2.png)</br>

### 6. **Update Subnet Injection Enterprise Policy**
This script updates a Subnet Injection Enterprise Policy. The updates allowed are for either of the virtual network Ids and subnet names.</br>
If you are changing only some of the allowed parameter values, provide “N/A” when prompted for the parameters that you don’t want to change.</br>
 **If the enterprise policy is associated with one or more environments, the update operation will fail, and the script will return an error.**</br>
Script name : Update-SubnetInjectionEnterprisePolicy.ps1</br>
Input parameters :
  - subscriptionId : The Azure subscription Id of the Subnet Injection Enterprise Policy
  - resourceGroup : The Azure resource group of the Subnet Injection Enterprise Policy
  - enterprisePolicyName : The name of the Subnet Injection enterprise policy that needs to be updated
  - vnetId1 : The ARM resource ID of the first virtual network if it needs to be updated. Provide "N/A" if update is not required for the first virtual network Id
  - subnetName1 : The name of the subnet in the first virtual network if it needs to be updated. Provide "N/A" if update is not required for name of the subnet in the first virtual network
  - vnetId2 : The ARM resource ID of the second virtual network if it needs to be updated. Provide "N/A" if update is not required for the second virtual network Id
  - subnetName2 : The name of the subnet in the second virtual network if it needs to be updated. Provide "N/A" if update is not required for name of the subnet in the second virtual network

Sample Input : </br>
```powershell
Update-SubnetInjectionEnterprisePolicy.ps1 `
-subscriptionId "98159998-fb68-44c3-b7d8-22b6539499a2" `
-resourceGroup "enterprisePolicy-snet-delegation" `
-enterprisePolicyName "vnetEP1" `
-vnetId1 "N/A" `
-subnetName1 "N/A" `
-vnetId2 "/subscriptions/98159998-fb68-44c3-b7d8-22b6539499a2/resourceGroups/enterprisePolicy-snet-delegation/Providers/Microsoft.Network/virtualNetworks/eastus-vnet" `
-subnetName2 "new-eus-delegated-snet"
```

Sample Output :</br>
![alt text](./ReadMeImages/UpdateSubnetInjectionEnterprisePolicy2.png)</br>

### 7. **Set Subnet Injection for an environment**
This script applies a Subnet Injection enterprise policy to a given Power Platform environment.</br>
The script adds the environment to the enterprise policy and optionally polls for the operation outcome.</br>
Script name : New-SubnetInjection.ps1</br>
Input parameters :
  - environmentId : The Power Platform environment ID
  - policyArmId : The ARM ID of the Subnet Injection Enterprise Policy

Sample Input :</br>
```powershell
New-SubnetInjection.ps1 `
-environmentId "03ec85eb-f8f3-4f26-9d8e-683479431def" `
-policyArmId "/subscriptions/98159998-fb68-44c3-b7d8-22b6539499a2/resourceGroups/enterprisePolicy-snet-delegation/Providers/Microsoft.PowerPlatform/enterprisePolicies/vnetEP1"
```

Sample Output :</br>
![alt text](./ReadMeImages/NewSubnetInjection2.png)</br>

### 8. **Get Subnet Injection for an environment**
This script returns the Subnet Injection enterprise policy if applied to a given Power Platform environment.</br>
Script name : Get-SubnetInjectionEnterprisePolicyForEnvironment.ps1</br>
Input parameter :
  - environmentId : The Power Platform environment ID

Sample Input :</br>
```powershell
Get-SubnetInjectionEnterprisePolicyForEnvironment.ps1 `
-environmentId "03ec85eb-f8f3-4f26-9d8e-683479431def"
```

Sample Output :</br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePolicyForEnvironment2.png)</br>

### 9. **Remove Subnet Injection from an environment**
The script removes the Subnet Injection enterprise policy from an environment, </br>
Script name : Remove-SubnetInjection.ps1</br>
Input parameters :
  - environmentId : The Power Platform environment ID
  - policyArmId: The ARM ID of the Subnet Injection Enterprise Policy

Sample Input :</br>
```powershell
Remove-SubnetInjection.ps1 `
-environmentId "03ec85eb-f8f3-4f26-9d8e-683479431def" `
-policyArmId "/subscriptions/98159998-fb68-44c3-b7d8-22b6539499a2/resourceGroups/enterprisePolicy-snet-delegation/Providers/Microsoft.PowerPlatform/enterprisePolicies/vnetEP1"
```

Sample Output :</br>
![alt text](./ReadMeImages/RevertSubnetInjection2.png)</br>
