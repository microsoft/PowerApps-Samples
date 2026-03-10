# Web API Attachment and Annotation Operations sample

This .NET 6.0 sample demonstrates how to perform operations using file data with `ActivityMimeAttachment` (Attachment) and `Annotation` (Note) tables using the Dataverse Web API.

This sample uses the common helper code in the [WebAPIService](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/webapi/CSharp-NETx/WebAPIService) class library project.

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator or system customizer privileges.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `PowerApps-Samples\dataverse\webapi\CSharp-NETx\AttachmentAndAnnotationOperations\AttachmentAndAnnotationOperations.sln` file using Visual Studio 2022.

   This solution contains two projects that include samples:

   - **ActivityMimeAttachmentOperations**: Demonstrates using Attachments.
   - **AnnotationOperations**: Demonstrates using Annotations.

   > [!NOTE]
   > The **WebAPIService** project is included so that each of the other projects can depend on the common helper code provided by the service. The samples use several classes in the `WebAPIService/Messages` folder.

   In **Solution Explorer**, right-click the project you want to run and choose **Set as Startup Project**.

1. In either project, edit the `appsettings.json` file to set the following property values:

   | Property | Instructions |
   |----------|--------------|
   | `Url` | The Url for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` value with the value for your environment. Learn how to find your URL in [View developer resources](https://learn.microsoft.com/power-apps/developer/data-platform/view-download-developer-resources). |
   | `UserPrincipalName` | Replace the placeholder `you@yourorg.onmicrosoft.com` value with the UPN value you use to access the environment. |
   | `Password` | Replace the placeholder `yourPassword` value with the password you use. |

1. Save the `appsettings.json` file.

   > [!NOTE]
   > Both projects refer to the same `appsettings.json` file, so you only need to do this one time to run either project.

1. Press **F5** to run the sample.

## Demonstrates

This sample is a solution  with two projects. See the respective README files for details on each project.

- [Web API Attachment (ActivityMimeAttachment) Operations sample README](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/AttachmentAndAnnotationOperations/ActivityMimeAttachmentOperations/README.md)
- [Web API Annotation (Note) Operations sample README](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/AttachmentAndAnnotationOperations/AnnotationOperations/README.md)

## Utility class

Both projects use a shared `Utility` class to perform common operations. This class contains three static methods: `GetMimeType`, `GetMaxUploadFileSize`, and `SetMaxUploadFileSize`.

### GetMimeType

Based on a [FileInfo](https://learn.microsoft.com/dotnet/api/system.io.fileinfo?view=net-7.0) parameter, this function uses [Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider](https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.staticfiles.fileextensioncontenttypeprovider?view=aspnetcore-7.0) to try to get the mimetype of the file. If this cannot be determined, it returns `application/octet-stream`

### GetMaxUploadFileSize

Using the **WebAPIService** `Service` `service` parameter, this function returns the integer `maxuploadfilesize` value from the `organization` table.

### SetMaxUploadFileSize

Using the **WebAPIService** `Service`  `service` parameter, this function sets the integer `maxuploadfilesize` value from the `organization` table to the value of the integer `maxUploadFileSizeInBytes` parameter.
