# C# code samples

The CSharp folder contains C# code samples that target .NET Framework and demonstrate various Dataverse capabilities. These samples are considered to be "legacy" as they were written to use older and perhaps now deprecated APIs. Specifically, most of the samples use the [CrmServiceClient class](https://learn.microsoft.com/dotnet/api/microsoft.xrm.tooling.connector.crmserviceclient?view=dataverse-sdk-latest) for authentication, which depends on the deprecated Azure Active Directory Authentication Library (ADAL).

The ./CSharp-NETCore folder contains newer and more modern .NET Core versions of many code samples. Those samples use the recommended [ServiceClient class](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient?view=dataverse-sdk-latest) which can target both .NET Framework and .NET Core, and depends on the Microsoft Authentication Library (MSAL).

> [!NOTE]
> The modern and legacy code samples both use the same Dataverse SDK API calls. The difference is in the code design, client class used, and authentication library.

We are actively upgrading our legacy code samples to the more modern coding style. We recommend you look first in the ./CSharp-NETCore folder for a sample, an then only if an appropriate sample cannot be found, in the CSharp folder. We will be removing the legacy code samples as we upgrade the samples to the modern style.

For information about supported versions of .NET Framework and .NET Core see [Supported customizations for Dataverse](https://learn.microsoft.com/power-apps/developer/data-platform/supported-customizations).