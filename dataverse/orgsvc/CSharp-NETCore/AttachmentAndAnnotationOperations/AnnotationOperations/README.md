# SDK for .NET Annotation (Note) Operations sample

This .NET 6.0 sample demonstrates how to perform operations using file data with `Annotation` (Note) table using the Dataverse SDK for .NET.

This project one of two projects included in the `AttachmentAndAnnotationOperations` solution, which provides shared resources to run either sample. See [SDK for .NET Attachment and Annotation Operations sample README](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/CSharp-NETCore/AttachmentAndAnnotationOperations/README.md) for an overview and how to run this sample.

This sample uses the [Microsoft.PowerPlatform.Dataverse.Client.ServiceClient Class](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient).

## Demonstrates

The `AnnotationOperations` project demonstrates:

- Creating an note associated with an account record.
- Setting the the `annotation.documentbody` property with the Base64 encoded string value for the file.
- Retrieving the note and saving the file.
- Updating the note to refer to a different file.
- Using the following Dataverse SDK for .NET classes to work with notes, especially for large files.

  - [InitializeAnnotationBlocksUploadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeannotationblocksuploadrequest) and [InitializeAnnotationBlocksUploadResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeannotationblocksuploadresponse) classes.
  - [UploadBlockRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.uploadblockrequest) and [UploadBlockResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.uploadblockresponse) classes.
  - [CommitAnnotationBlocksUploadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.commitannotationblocksuploadrequest) and [CommitAnnotationBlocksUploadResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.commitannotationblocksuploadresponse) classes.
  - [InitializeAnnotationBlocksDownloadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeannotationblocksdownloadrequest) and [InitializeAnnotationBlocksDownloadResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeannotationblocksdownloadresponse) classes.
  - [DownloadBlockRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.downloadblockrequest) and [DownloadBlockResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.downloadblockresponse) classes.

The code for this sample is in the [Program.cs](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/CSharp-NETCore/AttachmentAndAnnotationOperations/AnnotationOperations/Program.cs) file.

## What does this sample do?

This sample uses three files to represent annotations with notes:

|File|Size|Description  |
|---------|---------|---------|
|`WordDoc.docx`|18 KB|A small Word document.|
|`ExcelDoc.xlsx`|75 KB|A small Excel file.|
|`25mb.pdf`|25,265 KB|A large PDF file, which exceeds the default maximum file size of 5 MB.|

To upload the large file, the maximum file upload size must be increased. The `Utility.GetMaxUploadFileSize` function retrieves this value so it can be set back to the original value when the sample completes.

The sample performs the following operations:

1. Create an account record to associate the notes to.
1. Create a note associated to the account, setting the `annotation.documentbody` to the Base64 encoded string value of the `WordDoc.docx` file.
1. Retrieve the note with the `documentbody`, `mimetype`, and `filename` properties.
1. Convert the `documentbody` to `byte[]` and save the file.
1. Update the note, changing the file to `ExcelDoc.xlsx` by setting the `documentbody`, `mimetype`, and `filename` properties.
1. Retrieve the note and save the file again.
1. Use `Utility.SetMaxUploadFileSize` to increase the maximum file size for the environment to 131,072,000 Bytes (125 MB).
1. Use the static `UploadNote` method to update the note using the large `25mb.pdf` file and upload it in chunks.

   The static `UploadNote` method encapsulates the use of these Dataverse SDK classes:

   - [InitializeAnnotationBlocksUploadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeannotationblocksuploadrequest)
   - [UploadBlockRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.uploadblockrequest)
   - [CommitAnnotationBlocksUploadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.commitannotationblocksuploadrequest)

1. Download the note file using the static `DownloadNote` method.

   The static `DownloadNote` method encapsulates the use of these Dataverse SDK classes:

   - [InitializeAnnotationBlocksDownloadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeannotationblocksdownloadrequest)
   - [DownloadBlockRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.downloadblockrequest)

1. Delete the account record. All the notes associated with it are deleted as well.

## Clean up

Use static `Utility.SetMaxUploadFileSize` method to set the maximum file upload size to the original setting.

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
                AnnotationId: d1b0db4c-89a2-ed11-aad1-000d3a9933c9
                FileSizeInBytes: 25870370
        Saved the PDF document to \bin\Debug\net6.0\Downloaded25mb.pdf.
Deleted the account record.
Current MaxUploadFileSize: 5242880
```
