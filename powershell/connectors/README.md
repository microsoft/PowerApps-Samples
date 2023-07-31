# What is this for?
This is a helper script that is used to diagnose issues when setting up an on-premises data gateway that needs to be configured with the Power Automate SAP ERP connector and Kerberos.

# When to use it?
During implementation or production to determine why your Kerberos authentication into SAP from the on-premises data gateway is not working.

# How to use it?
See the `Get-Help .\opdg-check.ps1 -Detailed` for specific parameter information. The example parameters below will need to be replaced with values specific to your environment.
```
.\saperp-opdg-checks.ps1
  -sapServicePrincipalName SAP/DV6
  -sapServicePrincipal ehpsdv6
  -opdgServicePrincipal opdgsapclienttools
  -sapCclDllPath c:\sapcryptolib\sapcrypto.dll
  -sapCclIniPath c:\sapcryptolib\sapcrypto.ini
```

# Prerequisites
1. Powershell (Run as Administrator)
2. [Active Directory Remote Power Shell module](https://go.microsoft.com/fwlink/?linkid=2243545)

# Links
- [Official Power Automate SAP ERP documentation](https://go.microsoft.com/fwlink/?linkid=2243722)
