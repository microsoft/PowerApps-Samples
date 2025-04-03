# Sample: Image Operations using Dataverse Web API

This .NET 6.0 sample demonstrates how to perform operations with image columns using the Dataverse Web API.

This sample uses the common helper code in the [WebAPIService class library (CSharp)](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/samples/webapiservice).

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator or system customizer privileges

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `PowerApps-Samples\dataverse\webapi\CSharp-NETx\ImageOperations\ImageOperations.sln` file using Visual Studio 2022.
1. Edit the `appsettings.json` file to set the following property values:

   | Property | Instructions |
   |----------|--------------|
   | `Url` | The URL for your environment. Replace the placeholder `https://yourorg.api.crm.dynamics.com` with the value for your environment. Learn how to find your URL in [View developer resources](../../view-download-developer-resources.md). |
   | `UserPrincipalName` | Replace the placeholder `you@yourorg.onmicrosoft.com` with the UPN value you use to access your environment. |
   | `Password` | Replace the placeholder `yourPassword` with the password you use. |

1. Save the `appsettings.json` file.
1. Press `F5` to run the sample.

## Sample Output

The output of the sample should look similar to this output:

```
Creating image column named 'sample_ImageColumn' on the account table ...
Created image column named 'sample_ImageColumn' in the account table.
Create 5 records while CanStoreFullImage is false.
        Created account: 'CanStoreFullImage false 144x144.png'
        Created account: 'CanStoreFullImage false 144x400.png'
        Created account: 'CanStoreFullImage false 400x144.png'
        Created account: 'CanStoreFullImage false 400x500.png'
        Created account: 'CanStoreFullImage false 60x80.png'
Set the CanStoreFullImage property to True
Create 5 records while CanStoreFullImage is true.
        Created account: 'CanStoreFullImage true 144x144.png'
        Created account: 'CanStoreFullImage true 144x400.png'
        Created account: 'CanStoreFullImage true 400x144.png'
        Created account: 'CanStoreFullImage true 400x500.png'
        Created account: 'CanStoreFullImage true 60x80.png'
Retrieving records with thumbnail images:
        Thumbnail-sized file column data saved to DownloadedImages\CanStoreFullImage false 144x144.png_retrieved.png
        Thumbnail-sized file column data saved to DownloadedImages\CanStoreFullImage false 144x400.png_retrieved.png
        Thumbnail-sized file column data saved to DownloadedImages\CanStoreFullImage false 400x144.png_retrieved.png
        Thumbnail-sized file column data saved to DownloadedImages\CanStoreFullImage false 400x500.png_retrieved.png
        Thumbnail-sized file column data saved to DownloadedImages\CanStoreFullImage false 60x80.png_retrieved.png
        Thumbnail-sized file column data saved to DownloadedImages\CanStoreFullImage true 144x144.png_retrieved.png
        Thumbnail-sized file column data saved to DownloadedImages\CanStoreFullImage true 144x400.png_retrieved.png
        Thumbnail-sized file column data saved to DownloadedImages\CanStoreFullImage true 400x144.png_retrieved.png
        Thumbnail-sized file column data saved to DownloadedImages\CanStoreFullImage true 400x500.png_retrieved.png
        Thumbnail-sized file column data saved to DownloadedImages\CanStoreFullImage true 60x80.png_retrieved.png
Attempt to download full-size images for all 10 records using 3 different methods:
Download full-sized files with actions: 5 should fail
        Download failed: No FileAttachment records found for imagedescriptorId: e40bdcf1-598d-ed11-81ad-000d3a9933c9 for image attribute: sample_imagecolumn of account record with id e30bdcf1-598d-ed11-81ad-000d3a9933c9
        Download failed: No FileAttachment records found for imagedescriptorId: 2258d4f7-598d-ed11-81ad-000d3a9933c9 for image attribute: sample_imagecolumn of account record with id 2158d4f7-598d-ed11-81ad-000d3a9933c9
        Download failed: No FileAttachment records found for imagedescriptorId: 2658d4f7-598d-ed11-81ad-000d3a9933c9 for image attribute: sample_imagecolumn of account record with id 2558d4f7-598d-ed11-81ad-000d3a9933c9
        Download failed: No FileAttachment records found for imagedescriptorId: 2a58d4f7-598d-ed11-81ad-000d3a9933c9 for image attribute: sample_imagecolumn of account record with id 2958d4f7-598d-ed11-81ad-000d3a9933c9
        Download failed: No FileAttachment records found for imagedescriptorId: 2e58d4f7-598d-ed11-81ad-000d3a9933c9 for image attribute: sample_imagecolumn of account record with id 2d58d4f7-598d-ed11-81ad-000d3a9933c9
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 144x144.png_downloaded_with_actions.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 144x400.png_downloaded_with_actions.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 400x144.png_downloaded_with_actions.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 400x500.png_downloaded_with_actions.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 60x80.png_downloaded_with_actions.png
Download full-sized files with chunks: 5 should fail
        No full-sized image data returned because record was created while CanStoreFullImage was false.
        No full-sized image data returned because record was created while CanStoreFullImage was false.
        No full-sized image data returned because record was created while CanStoreFullImage was false.
        No full-sized image data returned because record was created while CanStoreFullImage was false.
        No full-sized image data returned because record was created while CanStoreFullImage was false.
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 144x144.png_downloaded_with_chunks_full-sized.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 144x400.png_downloaded_with_chunks_full-sized.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 400x144.png_downloaded_with_chunks_full-sized.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 400x500.png_downloaded_with_chunks_full-sized.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 60x80.png_downloaded_with_chunks_full-sized.png
Download full-sized files in single requests: 5 should fail
        No full-sized image data returned because record was created while CanStoreFullImage was false.
        No full-sized image data returned because record was created while CanStoreFullImage was false.
        No full-sized image data returned because record was created while CanStoreFullImage was false.
        No full-sized image data returned because record was created while CanStoreFullImage was false.
        No full-sized image data returned because record was created while CanStoreFullImage was false.
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 144x144.png_downloaded_with_stream_full-sized.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 144x400.png_downloaded_with_stream_full-sized.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 400x144.png_downloaded_with_stream_full-sized.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 400x500.png_downloaded_with_stream_full-sized.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 60x80.png_downloaded_with_stream_full-sized.png
Deleting the image data from the columns using 3 different methods:
        CanStoreFullImage false 144x144.png sample_imagecolumn deleted with PATCH
        CanStoreFullImage false 144x400.png sample_imagecolumn deleted with PATCH
        CanStoreFullImage false 400x144.png sample_imagecolumn deleted with PATCH
        CanStoreFullImage false 400x500.png sample_imagecolumn deleted with PUT
        CanStoreFullImage false 60x80.png sample_imagecolumn deleted with PUT
        CanStoreFullImage true 144x144.png sample_imagecolumn deleted with PUT
        CanStoreFullImage true 144x400.png sample_imagecolumn deleted with DELETE
        CanStoreFullImage true 400x144.png sample_imagecolumn deleted with DELETE
        CanStoreFullImage true 400x500.png sample_imagecolumn deleted with DELETE
        CanStoreFullImage true 60x80.png sample_imagecolumn deleted with DELETE
Deleted the account records created for this sample.
Deleting the image column named 'sample_ImageColumn' on the account table ...
Deleted the image column named 'sample_ImageColumn' in the account table.
Sample completed.
```

## Demonstrates

The code for this sample is in the [Program.cs](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse//webapi/CSharp-NETx/ImageOperations/Program.cs) file.

The project uses a `Utility` class to perform operations like *creating* or *retrieving* schema data. This class is in the [Utility.cs](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/webapi/CSharp-NETx/ImageOperations/Utility.cs) file.

This project performs these operations:

- [Create an image column](#creates-an-image-column)
- [Create account records with image data](#creates-account-records-with-image-data)
- [Retrieve the account records](#retrieves-the-account-records)
- [Download the account record images](#downloads-the-account-record-images)
- [Delete the image data](#deletes-the-image-data)
- [Clean up](#cleans-up)

### Creates an image column

This sample creates a new image column as the primary image for the account table. The original state is restored when the sample is finished. At the beginning, this sample performs:

1. Capture the original primary image name using the `Utility.GetTablePrimaryImageName` method.
1. Use the `Utility.CreateImageColumn` method to create a new image column named `sample_ImageColumn` on the account table if it doesn't exist already.

   > [!NOTE]
   > This image column `CanStoreFullImage` value is false.

1. Use the `Utility.SetTablePrimaryImageName` method to make the new `sample_ImageColumn` the primary image column.

### Creates account records with image data

1. The program loops through a list of five filenames that match the names of files in the `Images` folder.
1. For each image, the program creates an account record with the `name` set to `CanStoreFullImage false {fileName}` and the file `byte[]` is set as the `sample_ImageColumn` value.
1. The program then uses the `Utility.UpdateCanStoreFullImage` method to set the `sample_ImageColumn` column definition `CanStoreFullImage` value to true.
1. The program loops through the file names again and creates five account records with the same image files set to the `sample_ImageColumn` value. This time the `name` is `CanStoreFullImage true {fileName}`.

### Retrieves the account records

1. The code retrieves the 10 account records created in the previous step, including the image data.
1. For each account record, the image data is downloaded to the `DownloadedImages` folder with the name `{recordName}_retrieved.png`.

   > [!NOTE]
   > All records are thumbnail-sized images.

### Downloads the account record images

This program uses three different methods to download image files.

> [!NOTE]
> In each case, five of the ten operations fail because no full-sized images are uploaded while `CanStoreFullImage` is false. Records created while `CanStoreFullImage` is true succeed.

#### Downloads with Actions

The code uses the static `DownloadImageWithActions` method, which encapsulates the use of the
[InitializeFileBlocksDownload](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/initializefileblocksdownload) and  
[DownloadBlock](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/downloadblock) actions to download the images as described in
[Use Dataverse messages to download a file](https://learn.microsoft.com/power-apps/developer/data-platform/file-column-data?tabs=webapi#use-dataverse-messages-to-download-a-file).

> [!NOTE]
> These operations fail when there's no full-sized image to download. The error message is: `No FileAttachment records found for imagedescriptorId: <guid> for image attribute: sample_imagecolumn of account record with id <guid>`.

#### Downloads with Chunks

The code uses the static `DownloadImageWithChunks` method, which demonstrates how to download images as described in
[Download the file in chunks using Web API](https://learn.microsoft.com/power-apps/developer/data-platform/file-column-data#download-the-file-in-chunks-using-web-api).

> [!NOTE]
> These operations don't fail when there's no full-sized image to download, they simply return `204 No Content`.

#### Downloads with Stream

The code uses the static `DownloadImageWithStream` method, which demonstrates how to download images as described in
[Download a file in a single request using Web API](https://learn.microsoft.com/power-apps/developer/data-platform/file-column-data#download-a-file-in-a-single-request-using-web-api).

> [!NOTE]
> These operations don't fail when there's no full-sized image to download, they simply return `204 No Content`.

### Deletes the image data

1. The program uses three different methods to demonstrate deleting image values, using `PATCH`, `PUT`, and `DELETE`.
1. The program verifies the records are deleted by retrieving the records again using the same criteria as before. The value for the image column is null.

### Cleans up

To return the sample to its previous state, the program does the following actions:

- Deletes the account records.
- Sets the account table primary image back to the original value.
- Deletes the image column.
