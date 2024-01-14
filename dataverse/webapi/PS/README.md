# Dataverse Web API PowerShell Helper functions

The files in this folder are PowerShell helper functions that Dataverse Web API PowerShell samples use. These samples are separated in the following files:

|File|Description|
|---|---|
|[Core.ps1](Core.ps1)|Contains functions that all other functions or samples depend on.|
|[TableOperations.ps1](TableOperationsre.ps1)|Contains function that enable performing data operations on table rows|
|[CommonFunctions.ps1](CommonFunctions.ps1)|Contains common Dataverse functions|

Samples that use these common functions reference them using [dot sourcing](https://learn.microsoft.com/powershell/module/microsoft.powershell.core/about/about_scripts#script-scope-and-dot-sourcing) as demonstrated by the [BasicOperations/BasicOperations.ps1](BasicOperations/BasicOperations.ps1)

```powershell
. $PSScriptRoot\..\Core.ps1
. $PSScriptRoot\..\TableOperations.ps1
. $PSScriptRoot\..\CommonFunctions.ps1
```

## Core functions

## Table Operation functions

## Common functions
