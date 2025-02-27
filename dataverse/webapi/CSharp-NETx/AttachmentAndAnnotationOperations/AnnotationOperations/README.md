# Web API Annotation (Note) Operations sample

This .NET 6.0 sample demonstrates how to perform operations using file data with `Annotation` (Note) table using the Dataverse Web API.

This project one of two projects included in the `AttachmentAndAnnotationOperations` solution, which provides shared resources to run either sample. See [Web API Attachment and Annotation Operations sample README](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/AttachmentAndAnnotationOperations/README.md) for an overview and how to run this sample.

> [!NOTE] 
> This sample uses the common helper code in the [WebAPIService](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/webapi/CSharp-NETx/WebAPIService) class library project.

## Demonstrates

The `AnnotationOperations` project demonstrates:

- Creating an note associated with an account record.
- Setting the the `annotation.documentbody` property with the Base64 encoded string value for the file.
- Retrieving the note and saving the file.
- Updating the note to refer to a different file.
- Using the following Dataverse Web API actions to work with notes, especially for large files.

  - [InitializeAnnotationBlocksUpload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/initializeannotationblocksupload)
  - [UploadBlock Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/uploadblock)
  - [CommitAnnotationBlocksUpload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/commitannotationblocksupload)
  - [InitializeAnnotationBlocksDownload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/initializeannotationblocksdownload)
  - [DownloadBlock Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/downloadblock)

- Downloading the `annotation.documentbody` value and saving the file.

This sample uses classes defined for these actions within the [WebAPIService project Messages folder](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages) so that they can be re-used. These classes inherit from the .NET [HttpRequestMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) and [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0) classes. The classes have constructors that accept the relevant variables to compose the requests to send or deserialize the responses into standard properties.

The code for this sample is in the [Program.cs](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/AttachmentAndAnnotationOperations/AnnotationOperations/Program.cs) file.

## What does this sample do?

This sample uses three files to represent attachments with notes:

| File | Size | Description |
|------|------|-------------|
| `WordDoc.docx` | 18 KB | A small Word document. |
| `ExcelDoc.xlsx` | 75 KB | A small Excel file. |
| `25mb.pdf` | 25,265 KB | A large PDF file, which exceeds the default maximum file size of 5 MB. |

To upload the large file, the maximum file upload size must be increased. The `Utility.GetMaxUploadFileSize` function retrieves this value so it can be set back to the original value when the sample completes.

The sample performs the following operations:

1. Creates an account record to associate to the notes.
1. Creates a note associated to the account, setting the `annotation.documentbody` to the Base64 encoded string value of the `WordDoc.docx` file.
1. Retrieves the note with the `documentbody`, `mimetype`, and `filename` properties.
1. Converts the `documentbody` to `byte[]` and save the file.
1. Updates the note, changing the file to `ExcelDoc.xlsx` by setting the `documentbody`, `mimetype`, and `filename` properties.
1. Retrieves the note and save the file again.
1. Uses `Utility.SetMaxUploadFileSize` to increase the maximum file size for the environment to 131,072,000 Bytes (125 MB).
1. Uses the static `UploadNote` method to update the note using the large `25mb.pdf` file and upload it in chunks.

   The static `UploadNote` method encapsulates the use of these Web API actions:

   - [InitializeAnnotationBlocksUpload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/initializeannotationblocksupload)
   - [UploadBlock Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/uploadblock)
   - [CommitAnnotationBlocksUpload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/CommitAnnotationBlocksUpload)

1. Downloads the note file using the static `DownloadNote` method.

   The static `DownloadNote` method encapsulates the use of these Web API actions:

   - [InitializeAnnotationBlocksDownload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/initializeannotationblocksdownload)
   - [DownloadBlock Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/downloadblock)

1. Downloads the large `25mb.pdf` file individually using `GET /annotations(<annotationid>)/documentbody/$value`. This GET call returns a Base64 encoded string value that can be converted into `byte[]` and saved.

   Conversion uses the [WebAPIService/Messages/DownloadAnnotationFileRequest](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/DownloadAnnotationFileRequest.cs) and [WebAPIService/Messages/DownloadAnnotationFileResponse](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/DownloadAnnotationFileResponse.cs) classes.

1. Deletes the account record. All the notes associated with it are deleted as well.

## Clean up

Use the static `Utility.SetMaxUploadFileSize` method to set the maximum file upload size to the original setting.

## Sample Output

The output of the sample should look something like this:

```
Current MaxUploadFileSize: 5242880
Created an account record to associate notes with.
Created note with attached Word document.
        Retrieved note with attached Word document.
        Saved the Word document to \bin\Debug\net6.0\DownloadedWordDoc.docx.
Updated note with attached Excel document.
        Retrieved note with attached Excel document.
        Saved the Excel document to \bin\Debug\net6.0\DownloadedExcelDoc.xlsx.
Updated MaxUploadFileSize to: 131072000
Uploading 25mb.pdf...
Uploaded 25mb.pdf
        AnnotationId: 2da8aaf5-3ca0-ed11-aad1-000d3a993550
        FileSizeInBytes: 25870370
        Saved the PDF document to \bin\Debug\net6.0\Downloaded25mb.pdf.
        Saved the PDF document to \bin\Debug\net6.0\DownloadedAgain25mb.pdf.
Deleted the account record.
Current MaxUploadFileSize: 5242880
```
