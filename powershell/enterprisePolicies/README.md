# Power Platform Enterprise Policies PowerShell Scripts

These scripts automate managing (create, update, get, delete) Power Platform Enterprise Policies as Azure resources.</br>
In addition, we are providing sample scripts on how to associate these policies with Power Platform environments.</br>
Please note that these scripts are provided under MIT license and its usage is the sole responsibility of the user.

## How to run setup scripts

1. **Install modules script** : This script installs the required modules to run Enterprise Policies scripts.</br>
Script name: [InstallPowerAppsCmdlets.ps1](./InstallPowerAppsCmdlets.ps1)</br>
    * Run the script to import required PowerShell modules.

2. **Setup Azure subscription for Microsoft.PowerPlatform** : This script registers the Azure subscription for Microsoft.PowerPlatform resource provider
and also allow lists the subscription for enterprisePoliciesPreview feature.</br>
Script name: [SetupSubscriptionForPowerPlatform.ps1](./SetupSubscriptionForPowerPlatform.ps1)</br>
    * Run the script to setup Azure subscription for Microsoft.PowerPlatform resources

## How to run CMK scripts

The CMK scripts are present in folder [Cmk](./Cmk/) at current location

### Create CMK Enterprise policy
1. **Create CMK Enterprise Policy** : This script creates a CMK enterprise policy</br>
Script name : [CreateCMKEnterprisePolicy.ps1](./Cmk/CreateCMKEnterprisePolicy.ps1)</br>
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
Script name : [GetCMKEnterprisePolicyByResourceId.ps1](./Cmk/GetCMKEnterprisePolicyByResourceId.ps1)</br>
Input parameter :
    - enterprisePolicyArmId : The ARM resource ID of the CMK Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/GetCMKByResourceId1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetCMKByResourceId2.png)</br>

### Get CMK Enterprise Policies in Subscription
3. **Get CMK Enterprise Policies in Subscription** : The script gets all CMK enterprise policies in an Azure subscription</br>
Script name : [GetCMKEnterprisePoliciesInSubscription.ps1](./Cmk/GetCMKEnterprisePoliciesInSubscription.ps1)</br>
Input parameter :
    - subscriptionId: : The Azure subscription Id

Sample Input :</br>
![alt text](./ReadMeImages/GetCMKInSub1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetCMKInSub2.png)</br>

### Get CMK Enterprise Policies in Resource Group
4. **Get CMK Enterprise Policies in Resource Group** : The script gets all CMK enterprise policies in an Azure resource group</br>
Script name : [GetCMKEnterprisePoliciesInResourceGroup.ps1](./Cmk/GetCMKEnterprisePoliciesInResourceGroup.ps1)</br>
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
	 

Script name : [ValidateKeyVaultForCMK.ps1](./Cmk/ValidateKeyVaultForCMK.ps1)</br>
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
Script name : [UpdateCMKEnterprisePolicy.ps1](./Cmk/UpdateCMKEnterprisePolicy.ps1)</br>
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
Script name : [RemoveCMKEnterprisePolicy.ps1](./Cmk/RemoveCMKEnterprisePolicy.ps1)</br>
Input parameter :
    - policyArmId : The ARM ID of the CMK enterprise policy to be deleted

Sample Input : </br>
![alt text](./ReadMeImages/RemoveCMKEP1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/RemoveCMKEP2.png)</br>

### Set CMK for an environment
8. **Set CMK for an environment** : This script applies a CMK enterprise policy to a given Power Platform environment.</br>
The script adds the environment to the enterprise policy and optionally polls for the operation outcome.</br>
Script name : [AddCustomerManagedKeyToEnvironment.ps1](./Cmk/AddCustomerManagedKeyToEnvironment.ps1)</br>
Input parameters :
    - environmentId : The Power Platform environment ID
    - policyArmId : The ARM ID of the CMK Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/AddCMKToEnv1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/AddCMKToEnv2.png)</br>

### Get CMK for an environment
9. **Get CMK for an environment** : This script returns the CMK enterprise policy if applied to a given Power Platform environment.</br>
Script name : [GetCMKEnterprisePolicyForEnvironment.ps1](./Cmk/GetCMKEnterprisePolicyForEnvironment.ps1)</br>
Input parameter :
    - environmentId : The Power Platform environment ID

Sample Input :</br>
![alt text](./ReadMeImages/GetCMKForEnv1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetCMKForEnv2.png)</br>

### Remove CMK from an environment
10. **Remove CMK from an environment** : The script removes the CMK enterprise policy from an environment, </br>
which results on data to be encrypted with a Microsoft managed encryption key.</br>
Script name : [RemoveCustomerManagedKeyFromEnvironment.ps1](./Cmk/RemoveCustomerManagedKeyFromEnvironment.ps1)</br>
Input parameters :
    - environmentId : The Power Platform environment ID
    - policyArmId: The ARM ID of the CMK Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/RemoveCMKFromEnv1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/RemoveCMKFromEnv2.png)</br>

## How to run Subnet Injection scripts

The Subnet Injection scripts are present in folder [SubnetInjection](./SubnetInjection/) at current location

### 1. **Setup virtual network for Subnet Injection**
This script adds the subnet delegation for `Microsoft.PowerPlatform/enterprisePolicies` Azure service to a given virtual network and subnet </br>
Script name : [SetupVnetForSubnetDelegation.ps1](./SubnetInjection/SetupVnetForSubnetDelegation.ps1)</br>
Input parameters :
- virtualNetworkSubscriptionId : The subscriptionId of the virtual network
- virtualNetworkName : The name of the virtual network
- subnetName : The name of the virtual network's subnet

**NOTE**: this can also be achieved through Azure portal, more documentation on subnet delegation [here](https://learn.microsoft.com/en-us/azure/virtual-network/manage-subnet-delegation?tabs=manage-subnet-delegation-portal#delegate-a-subnet-to-an-azure-service)

Sample Input :</br>
![alt text](./ReadMeImages/SetupVirtualNetwork1.png)</br>

Sample Output : </br>
![alt text](./ReadMeImages/SetupVirtualNetwork2.png)</br>

 ### 2. **Create Subnet Injection Enterprise Policy** 
This script creates a Subnet Injection enterprise policy</br>
Script name : [CreateSubnetInjectionEnterprisePolicy.ps1](./SubnetInjection/CreateSubnetInjectionEnterprisePolicy.ps1)</br>
Input parameters :
- subscriptionId : The subscriptionId where Subnet Injection enterprise policy needs to be created
- resourceGroup : The resource group where Subnet Injection enterprise policy needs to be created
- enterprisePolicyName : Designate a name for the Subnet Injection enterprise policy
- enterprisePolicyLocation : The Azure geo where Subnet Injection enterprise policy needs to be created.
    * Example: unitedstates, europe, australia, uk</br>
    * To get the complete list of supported geos for enterprise policy, use the following command:</br>
        ```powershell
        ((Get-AzResourceProvider -ProviderNamespace Microsoft.PowerPlatform).ResourceTypes | Where-Object ResourceTypeName -eq enterprisePolicies).Locations
        ```
- primaryVnetId : The ARM resource ID of the primary virtual network to be used for Subnet Injection
- primarySubnetName : The name of the subnet in the primary virtual network to be used for Subnet Injection
- secondaryVnetId : The ARM resource ID of the secondary virtual network to be used for Subnet Injection
    * can put `N/A` for geo's with only 1 supported region, must be provided for geos with 2+ supported regions
- secondarySubnetName : The name of the subnet in the secondary virtual network to be used for Subnet Injection
    * can put `N/A` for geo's with only 1 supported region, must be provided for geos with 2+ supported regions

**NOTE**:
* :exclamation: If there are more than 1 supported regions for the geo outlined in the [list of supported regions](https://learn.microsoft.com/en-us/power-platform/admin/vnet-support-overview#supported-regions), the primary and secondary VNet must have been created in ***different*** regions in the geo
* :exclamation: To delete a Subnet Injection enterprise policy:
    * ["Remove Subnet Injection from an environment"](#9-remove-subnet-injection-from-an-environment) for **ALL** associated environments, the following remove command should error and call out if there are environments still associated
    * Run the following command (see the "Get Subnet Injection Enterprise Policy" scripts if needed to find the ARM Resource ID):
        ```powershell
        Remove-AzResource -ResourceId $policyArmId -Force
        ```

Sample Input :</br>
![alt text](./ReadMeImages/CreateSubnetInjectionEnterprisePolicy1.png)</br>

Sample Output : </br>
![alt text](./ReadMeImages/CreateSubnetInjectionEnterprisePolicy2.png)</br>

### 3. **Get Subnet Injection Enterprise Policy By ResourceId**
This script gets a Subnet Injection enterprise policy by ARM resourceId</br>
Script name : [GetSubnetInjectionEnterprisePolicyByResourceId.ps1](./SubnetInjection/GetSubnetInjectionEnterprisePolicyByResourceId.ps1)</br>
Input parameter :
- enterprisePolicyArmId : The ARM resource ID of the Subnet Injection Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePolicyByResourceId1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePolicyByResourceId2.png)</br>

### 4. **Get Subnet Injection Enterprise Policies in Subscription** 
This script gets all Subnet Injection enterprise policies in an Azure subscription</br>
Script name : [GetSubnetInjectionEnterprisePoliciesInSubscription.ps1](./SubnetInjection/GetSubnetInjectionEnterprisePoliciesInSubscription.ps1)</br>
Input parameter :
- subscriptionId: : The Azure subscription Id

Sample Input :</br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePoliciesInSubscription1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePoliciesInSubscription2.png)</br>

### 5. **Get Subnet Injection Enterprise Policies in Resource Group**
This script gets all Subnet Injection enterprise policies in an Azure resource group</br>
Script name : [GetSubnetInjectionEnterprisePoliciesInResourceGroup.ps1](./SubnetInjection/GetSubnetInjectionEnterprisePoliciesInResourceGroup.ps1)</br>
Input parameters :
- subscriptionId : The Azure subscription Id
- resourceGroup : The Azure resource group

Sample Input : </br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePoliciesInResourceGroup1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePoliciesInResourceGroup2.png)</br>

### 6. **Update Subnet Injection Enterprise Policy**
This script updates a Subnet Injection Enterprise Policy. The updates allowed are for primary/secondary virtual network Id and/or primary/secondary subnet name.</br>
If you are changing only some of the allowed parameter values, provide “N/A” when prompted for the parameters that you don’t want to change.</br>
 **If the enterprise policy is associated with one or more environments, the update operation will fail, and the script will return an error.**</br>
Script name : [UpdateSubnetInjectionEnterprisePolicy.ps1](./SubnetInjection/UpdateSubnetInjectionEnterprisePolicy.ps1)</br>
Input parameters :
- subscriptionId : The Azure subscription Id of the Subnet Injection Enterprise Policy
- resourceGroup : The Azure resource group of the Subnet Injection Enterprise Policy
- enterprisePolicyName : The name of the Subnet Injection enterprise policy that needs to be updated
- primaryVnetId : The ARM resource ID of the primary virtual network if it needs to be updated. Provide "N/A" if update is not required for the primary virtual network Id
- primarySubnetName: The name of the subnet in the primary virtual network if it needs to be updated. Provide "N/A" if update is not required for name of the subnet in the primary virtual network
- secondaryVnetId : The ARM resource ID of the secondary virtual network if it needs to be updated. Provide "N/A" if update is not required for the secondary virtual network Id
- secondarySubnetName: The name of the subnet in the secondary virtual network if it needs to be updated. Provide "N/A" if update is not required for name of the subnet in the secondary virtual network

Sample Input : </br>
![alt text](./ReadMeImages/UpdateSubnetInjectionEnterprisePolicy1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/UpdateSubnetInjectionEnterprisePolicy2.png)</br>

### 7. **Set Subnet Injection for an environment**
This script applies a Subnet Injection enterprise policy to a given Power Platform environment.</br>
The script adds the environment to the enterprise policy and optionally polls for the operation outcome.</br>
Script name : [NewSubnetInjection.ps1](./SubnetInjection/NewSubnetInjection.ps1)</br>
Input parameters :
- environmentId : The Power Platform environment ID
- policyArmId : The ARM ID of the Subnet Injection Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/NewSubnetInjection1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/NewSubnetInjection2.png)</br>

### 8. **Get Subnet Injection for an environment**
This script returns the Subnet Injection enterprise policy if applied to a given Power Platform environment.</br>
Script name : [GetSubnetInjectionEnterprisePolicyForEnvironment.ps1](./SubnetInjection/GetSubnetInjectionEnterprisePolicyForEnvironment.ps1)</br>
Input parameter :
- environmentId : The Power Platform environment ID

Sample Input :</br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePolicyForEnvironment1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/GetSubnetInjectionEnterprisePolicyForEnvironment2.png)</br>

### 9. **Remove Subnet Injection from an environment**
This script removes the Subnet Injection enterprise policy from an environment, </br>
Script name : [RevertSubnetInjection.ps1](./SubnetInjection/RevertSubnetInjection.ps1)</br>
Input parameters :
- environmentId : The Power Platform environment ID
- policyArmId: The ARM ID of the Subnet Injection Enterprise Policy

Sample Input :</br>
![alt text](./ReadMeImages/RevertSubnetInjection1.png)</br>

Sample Output :</br>
![alt text](./ReadMeImages/RevertSubnetInjection2.png)</br>

## FAQ

### General FAQ

#### Unable to add/remove EP to/from environment due to "Error getting environment"
* StatusCode: 404 
* ErrorMessage contains: *The environment '\<guid\>' could not be found in the tenant...*
* **Solution**: Ensure the user has the `Power Platform Administrator` (or equivalent) role

### Subnet Injection FAQ

#### Unable to delete VNet / Unable to modify subnet
* ErrorCode: `InUseSubnetCannotBeDeleted` or `SubnetMissingRequiredDelegation`
* ErrorMessage contains: *.../serviceAssociationLinks/PowerPlatformServiceLink...*
* **Solution**: delete the Subnet Injection enterprise policy first, see the notes section in ["Create subnet injection enterprise policy"](#2-create-subnet-injection-enterprise-policy)
