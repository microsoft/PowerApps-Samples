# Web API Attachment (ActivityMimeAttachment) Operations sample

This .NET 6.0 sample demonstrates how to perform operations using file data with `ActivityMimeAttachment` (Attachment) table using the Dataverse Web API.

This project one of two projects included in the `AttachmentAndAnnotationOperations` solution, which provides shared resources to run either sample. See [Web API Attachment and Annotation Operations sample README](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/AttachmentAndAnnotationOperations/README.md) for an overview and how to run this sample.

> [!NOTE]
> This sample uses the common helper code in the [WebAPIService](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/webapi/CSharp-NETx/WebAPIService) class library project.

## Demonstrates

The `ActivityMimeAttachmentOperations` project demonstrates:

- Adding attachments to an email record directly for single use. These attachment records are be deleted when the email is deleted.
- Adding attachments to an email template, then associating the attachment to an email. These attachments aren't deleted when the email is deleted.
- Setting the value of the `activitymimeattachment.body` property directly with a Base64 encoded string value. This is appropriate for small files.
- Using the following Dataverse Web API actions when working with `activitymimeattachment`, especially for large files.

  - [InitializeAttachmentBlocksUpload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/initializeannotationblocksupload)
  - [UploadBlock Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/uploadblock)
  - [CommitAttachmentBlocksUpload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/commitattachmentblocksupload)
  - [InitializeAttachmentBlocksDownload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/initializeattachmentblocksdownload)
  - [DownloadBlock Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/downloadblock)

- Downloading the `activitymimeattachment.body` value and saving the file.

This sample uses classes defined for these actions within the [WebAPIService project Messages folder](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages) so that they can be re-used. These classes inherit from the .NET [HttpRequestMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) and [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0) classes. These classes have constructors that accept the relevant variables to compose the requests to send, or to deserialize the responses into standard properties.

The code for this sample is in the [Program.cs](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/AttachmentAndAnnotationOperations/ActivityMimeAttachmentOperations/Program.cs) file.

## What does this sample do?

This sample uses three files to represent attachments.

| File | Size | Description |
|------|------|-------------|
| `WordDoc.docx` | 18 KB | A small Word document. |
| `ExcelDoc.xlsx` | 75 KB | A small Excel file. |
| `25mb.pdf` | 25,265 KB | A large PDF file, which exceeds the default maximum file size of 5 MB. |

To upload the large file, the maximum file upload size must be increased. The `Utility.GetMaxUploadFileSize` function retrieves this value so it can be set back to the original value when the sample completes.

The sample is separated into two regions: **Create single-use attachments** and **Create re-usable attachments**.

### Create single-use attachments

1. Create an email activity.
1. Using only the small files, create two attachments associated to the email, setting the `activitymimeattachment.body` directly.
1. Use `Utility.SetMaxUploadFileSize` to increase the maximum file size for the environment to 131,072,000 Bytes (125 MB).
1. Use the static `UploadAttachment` method to create the attachment and upload the large file in chunks.

   The static `UploadAttachment` method encapsulates the use of these Web API actions:

   - [InitializeAttachmentBlocksUpload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/initializeannotationblocksupload)
   - [UploadBlock Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/uploadblock)
   - [CommitAttachmentBlocksUpload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/CommitAttachmentBlocksUpload)

1. Retrieve the `activitymimeattachmentid` and `filename` of the attachment records associated with the email using the `activity_pointer_activity_mime_attachment` collection-valued navigation property.
1. Use that data to download each attachment using the static `DownloadAttachment` method.

   The static `DownloadAttachment` method encapsulates the use of these Web API actions:

   - [InitializeAttachmentBlocksDownload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/InitializeAttachmentBlocksDownload)
   - [DownloadBlock Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/DownloadBlock)

1. Download the large `25mb.pdf` file individually using `GET /activitymimeattachments(<activitymimeattachmentid>)/body/$value`. This returns a Base64 encoded string value that can be converted into `byte[]` and saved.

   This is done using the [WebAPIService/Messages/DownloadAttachmentFileRequest](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/DownloadAttachmentFileRequest.cs) and [WebAPIService/Messages/DownloadAttachmentFileResponse](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/webapi/CSharp-NETx/WebAPIService/Messages/DownloadAttachmentFileResponse.cs) classes.

1. Delete the email activity. All the attachments are deleted with the email.

### Create re-usable attachments

1. Create an email template. This provides values to use when creating an `ActivityMimeAttachment` because `ObjectId` and `ObjectTypeCode` are required.
1. Attach all files as attachments using the `UploadAttachment` method described above.
1. Create a new email activity.
1. For each attachment created using the `UploadAttachment` method, create a new attachment associated with the email. Only set the `attachmentid` single-valued navigation property. Do not set the `body`, `filename`, or `mimetype`.
1. Delete the email activity.
1. Retrieve the attachments created using the `UploadAttachment` method to confirm that they still exist.
1. Delete the email template to clean up. This deletes the attachments associated with the sample.

## Clean up

Use static `Utility.SetMaxUploadFileSize` method to set the maximum file upload size to the original setting.

## Sample Output

The output of the sample should look something like this:

```
Current MaxUploadFileSize: 5242880
Start: Create single-use attachments
Created an email activity.
Created two e-mail attachments with small files for the e-mail activity.
Updated MaxUploadFileSize to: 131072000
Adding 25mb.pdf...
        Uploaded 25mb.pdf as attachment.
                ActivityMimeAttachmentId:6c842bed-34a0-ed11-aad1-000d3a993550
                FileSizeInBytes: 25870370
Download attached files:
        Downloading filename: WordDoc.docx...
        Saved the attachment to \bin\Debug\net6.0\DownloadedWordDoc.docx.
        Downloading filename: ExcelDoc.xlsx...
        Saved the attachment to \bin\Debug\net6.0\DownloadedExcelDoc.xlsx.
        Downloading filename: 25mb.pdf...
        Saved the attachment to \bin\Debug\net6.0\Downloaded25mb.pdf.
Saved the attachment to \bin\Debug\net6.0\DownloadedAgain25mb.pdf.

Start: Create re-usable attachments
Created an email template.
        Added WordDoc.docx to the email template.
        Added ExcelDoc.xlsx to the email template.
        Added 25mb.pdf to the email template.
Added all files as attachment to email template.
Created a second email activity.
        Attached WordDoc.docx to the second email
        Attached ExcelDoc.xlsx to the second email
        Attached 25mb.pdf to the second email
Deleted the second email.
        Attachment for WordDoc.docx still exists.
        Attachment for ExcelDoc.xlsx still exists.
        Attachment for 25mb.pdf still exists.
Current MaxUploadFileSize: 5242880
```
