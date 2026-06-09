# C# code samples

The CSharp folder contains code samples that demonstrate various Dataverse features and targets .NET Framework. These samples are considered to be "legacy" as they were written to use older and perhaps now deprecated APIs. Specifically, most of the samples use the [CrmServiceClient class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.tooling.connector.crmserviceclient?view=dataverse-sdk-latest) for authentication, which depends on Azure Active Directory Authentication Library (ADAL), (now deprecated).

The ./CSharp-NETCore folder contains newer and more modern .NET Core versions of many code samples. Those samples use the recommended [ServiceClient class](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient?view=dataverse-sdk-latest) which can target both .NET Framework and .NET Core, and depends on the Microsoft Authentication Library (MSAL).

We are actively upgrading our legacy code samples to the more modern coding style. We recommend you look first in the ./CSharp-NETCore folder for a sample, an then only if an appropriate sample cannot be found, in the CSharp folder.

Plug-in and custom workflow activity samples will continue to be based on .NET Framework 4.6.2 until .NET Framework 4.8 is supported by the Dataverse event framework.
