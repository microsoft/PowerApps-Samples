if(-not(Get-InstalledModule Az) -and -not(Get-module -ListAvailable Az))
{
    throw "Az module not found. Ensure it is installed by running InstallPowerAppsCmdlets.ps1"
}

Import-Module @("Az.Accounts", "Az.Resources", "Az.KeyVault", "Az.Network")