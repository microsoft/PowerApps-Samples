# SDK for .NET Attachment (ActivityMimeAttachment) Operations sample

This .NET 6.0 sample demonstrates how to perform operations using file data with `ActivityMimeAttachment` (Attachment) table using the Dataverse SDK for .NET.

This project one of two projects included in the `AttachmentAndAnnotationOperations` solution, which provides shared resources to run either sample. See [SDK for .NET Attachment and Annotation Operations sample README](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/CSharp-NETCore/AttachmentAndAnnotationOperations/README.md) for an overview and how to run this sample.

This sample uses the [Microsoft.PowerPlatform.Dataverse.Client.ServiceClient Class](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient).

## Demonstrates

The `ActivityMimeAttachmentOperations` project demonstrates:

- Adding attachments to an email record directly for single use. These attachment records will be deleted when the email is deleted.
- Adding attachments to an email template, then associating the attachment to an email. These attachments will not be deleted when the email is deleted.
- Setting the value of the `activitymimeattachment.body` property directly with a Base64 encoded string value. This is appropriate for small files.
- Using the following Dataverse SDK classes when working with `activitymimeattachment`, especially for large files.

  - [InitializeAttachmentBlocksUploadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeattachmentblocksuploadrequest) and [InitializeAttachmentBlocksUploadResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeattachmentblocksuploadresponse) classes.
  - [UploadBlockRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.uploadblockrequest) and [UploadBlockResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.uploadblockresponse) classes.
  - [CommitAttachmentBlocksUploadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.commitattachmentblocksuploadrequest) and [CommitAttachmentBlocksUploadResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.commitattachmentblocksuploadresponse) classes.
  - [InitializeAttachmentBlocksDownloadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeattachmentblocksdownloadrequest) and [InitializeAttachmentBlocksDownloadResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeattachmentblocksdownloadresponse) classes.
  - [DownloadBlockRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.downloadblockrequest) and [DownloadBlockResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.downloadblockresponse) classes.

The code for this sample is in the [Program.cs](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/CSharp-NETCore/AttachmentAndAnnotationOperations/ActivityMimeAttachmentOperations/Program.cs) file.

## What does this sample do?

This sample uses three files to represent attachments:

|File|Size|Description  |
|---------|---------|---------|
|`WordDoc.docx`|18 KB|A small Word document.|
|`ExcelDoc.xlsx`|75 KB|A small Excel file.|
|`25mb.pdf`|25,265 KB|A large PDF file, which exceeds the default maximum file size of 5 MB.|

To upload the large file, the maximum file upload size must be increased. The `Utility.GetMaxUploadFileSize` function retrieves this value so it can be set back to the original value when the sample completes.

The sample is separated into two regions: **Create single-use attachments** and **Create re-usable attachments**.

### Create single-use attachments

1. Create an email activity.
1. Using only the small files, create two attachments associated to the email, setting the `activitymimeattachment.body` directly.
1. Use `Utility.SetMaxUploadFileSize` to increase the maximum file size for the environment to 131,072,000 Bytes (125 MB).
1. Use the static `UploadAttachment` method to create the attachment and upload the large file in chunks.

   The static `UploadAttachment` method encapsulates the use of these Dataverse SDK classes:

   - [InitializeAttachmentBlocksUploadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeattachmentblocksuploadrequest)
   - [UploadBlockRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.uploadblockrequest)
   - [CommitAttachmentBlocksUploadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.commitattachmentblocksuploadrequest)

1. Retrieve the `activitymimeattachmentid` and `filename` of the attachment records associated with the email using the `email_activity_mime_attachment` relationship.
1. Use that data to download each attachment using the static `DownloadAttachment` method.

   The static `DownloadAttachment` method encapsulates the use of these Dataverse SDK classes:

   - [InitializeAttachmentBlocksDownloadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializeattachmentblocksdownloadrequest)
   - [DownloadBlockRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.downloadblockrequest)

1. Delete the email activity. All the attachments are deleted with the email.

### Create re-usable attachments

1. Create an email template. This provides values to use when creating an `ActivityMimeAttachment` because `ObjectId` and `ObjectTypeCode` are required.
1. Attach all files as attachments using the `UploadAttachment` method described above.
1. Create a new email activity.
1. For each attachment created using the `UploadAttachment` method, create a new attachment associated with the email. Only set the `attachmentid` single-valued navigation property. Do not set the `body`, `filename`, or `mimetype`.
1. Delete the email activity.
1. Retrieve the attachments created using the `UploadAttachment` method to confirm that they still exist.
1. Delete the email template to clean up. This will delete the attachments associated with it

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
                ActivityMimeAttachmentId:8d43eb9a-87a2-ed11-aad1-000d3a9933c9
                FileSizeInBytes: 25870370
Download attached files:
        Downloading filename: WordDoc.docx...
        Saved the attachment to \bin\Debug\net6.0\DownloadedWordDoc.docx.
        Downloading filename: ExcelDoc.xlsx...
        Saved the attachment to \bin\Debug\net6.0\DownloadedExcelDoc.xlsx.
        Downloading filename: 25mb.pdf...
        Saved the attachment to \bin\Debug\net6.0\Downloaded25mb.pdf.

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
