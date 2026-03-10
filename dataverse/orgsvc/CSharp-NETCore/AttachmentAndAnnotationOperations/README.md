# SDK for .NET Attachment and Annotation Operations sample

This .NET 6.0 sample demonstrates how to perform operations using file data with `ActivityMimeAttachment` (Attachment) and `Annotation` (Note) tables using the Dataverse SDK for .NET.

This sample uses the [Microsoft.PowerPlatform.Dataverse.Client.ServiceClient](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient) class.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator or system customizer privileges.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `PowerApps-Samples\dataverse\orgsvc\CSharp-NETCore\AttachmentAndAnnotationOperations\AttachmentAndAnnotationOperations.sln` file using Visual Studio 2022.

   This solution contains two projects that include samples:

   - **ActivityMimeAttachmentOperations**: Demonstrates using attachments.
   - **AnnotationOperations**: Demonstrates using annotations.

   In **Solution Explorer**, right-click the project you want to run and choose **Set as Startup Project**.

1. In either project, edit the *appsettings.json* file. Set the connection string `Url` and `Username` parameters as appropriate for your test environment.

   The environment URL can be found in the Power Platform admin center. It has the form `https://<environment-name>.crm.dynamics.com`.

1. Build the solution, and then run the desired project.

When the sample runs, you are prompted in a browser to select an environment user account and enter a password. To avoid doing this every time you run a sample, insert a password parameter into the connection string in the `appsettings.json` file. For example:

```json
{
"ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;Password=mypassword;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
  }
}
```

> [!Tip]
> You can set a user environment variable named `DATAVERSE_APPSETTINGS` to the file path of the `appsettings.json` file stored anywhere on your computer. The samples use that appsettings file if the environment variable exists and isn't null. Be sure to log out and back in again after you define the variable for it to take affect. You can manually set an environment variable by typing `env` in your system search bar, then choose **Environment variables** in **System properties** window.

## Demonstrates

This sample is a solution  with two projects. See the respective README files for details on each project.

- [SDK for .NET Attachment (ActivityMimeAttachment) Operations sample README](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/CSharp-NETCore/AttachmentAndAnnotationOperations/ActivityMimeAttachmentOperations/README.md)
- [SDK for .NET Annotation (Note) Operations sample README](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/CSharp-NETCore/AttachmentAndAnnotationOperations/AnnotationOperations/README.md)

## Utility class

Both projects use a shared `Utility` class to perform common operations. This class contains three static methods:

### GetMimeType

Based on a [FileInfo](https://learn.microsoft.com/dotnet/api/system.io.fileinfo?view=net-7.0) parameter, this function uses [Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider](https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.staticfiles.fileextensioncontenttypeprovider?view=aspnetcore-7.0) to try to get the mimetype of the file. If this cannot be determined, it returns `application/octet-stream`

### GetMaxUploadFileSize

Using the [IOrganizationService](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice) `service` parameter, this function returns the integer `maxuploadfilesize` value from the `organization` table.

### SetMaxUploadFileSize

Using the [IOrganizationService](https://learn.microsoft.com/dotnet/api/microsoft.xrm.sdk.iorganizationservice) `service` parameter, this function sets the integer `maxuploadfilesize` value from the `organization` table to the value of the integer `maxUploadFileSizeInBytes` parameter.
