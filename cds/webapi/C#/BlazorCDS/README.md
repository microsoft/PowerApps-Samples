---
languages:
- csharp
products:
- dotnet, blazor web assembly
- common data service
page_type: sample
description: "This sample shows how to retrieve Microsoft Dataverse environment instances using the Global Discovery Service, and then retrieves business account information using the Web API in Dataverse."
---
# Blazor web assembly sample

This sample application is based on the code and information provided in [Tutorial: Create an ASP.NET Core Blazor WebAssembly App using Dataverse](https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/walkthrough-blazor-webassembly-single-tenant).

## Demonstrate

1. Uses the Global Discovery Service to obtain your subscribed environments.
1. The `Environment` dropdown will populate environment instances retrieved from the Global Discovery Service.
1. The `Fetch Account` page retrieves accounts entity instances and displays them in a table.

## Add multi-tenancy (optional)

Define your Azure Active Directory (AD) app registration to authenticate with account entity records in any organizational directory - any Azure AD directory (multi-tenant).

## Clean up

No clean-up is required.

## Demo

See it in action at:
[https://blazorcds.mohsinonxrm.com/](https://blazorcds.mohsinonxrm.com/)