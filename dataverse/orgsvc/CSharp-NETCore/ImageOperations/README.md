# Sample: Image Operations using Dataverse SDK for .NET

This .NET 6.0 sample demonstrates how to perform operations with image columns using the Dataverse SDK for .NET.

This sample uses the [Microsoft.PowerPlatform.Dataverse.Client.ServiceClient](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient).

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator or system customizer privileges.

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `PowerApps-Samples\dataverse\orgsvc\CSharp-NETCore\ImageOperations\ImageOperations.sln` file using Visual Studio 2022.
1. Edit the *appsettings.json* file. Set the connection string `Url` and `Username` parameters as appropriate for your test environment.

   The environment Url can be found in the Power Platform admin center. It has the form https://\<environment-name>.crm.dynamics.com.

1. Build the solution, and then run the desired project.

When the sample runs, you'll be prompted in the default browser to select an environment user account and enter a password. To avoid having to do this every time you run a sample, insert a password parameter into the connection string in the appsettings.json file. For example:

```json
{
"ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;Password=mypassword;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
  }
}
```

> **Tip:**
> You can set a user environment variable named DATAVERSE_APPSETTINGS to the file path of the appsettings.json file stored anywhere on your computer. The samples will use that appsettings file if the environment variable exists and is not null. Be sure to log out and back in again after you define the variable for it to take effect. To set an environment variable, go to **Settings > System > About**, select **Advanced system settings**, and then choose **Environment variables**.

## Sample Output

The output of the sample should look something like this:

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
Attempt to download full-size images for all 10 records. 5 should fail.
        Download failed: No FileAttachment records found for imagedescriptorId: a81a473d-3a8d-ed11-81ad-000d3a993550 for image attribute: sample_imagecolumn of account record with id a51a473d-3a8d-ed11-81ad-000d3a993550
        Download failed: No FileAttachment records found for imagedescriptorId: 2a813f43-3a8d-ed11-81ad-000d3a993550 for image attribute: sample_imagecolumn of account record with id 29813f43-3a8d-ed11-81ad-000d3a993550
        Download failed: No FileAttachment records found for imagedescriptorId: 2e813f43-3a8d-ed11-81ad-000d3a993550 for image attribute: sample_imagecolumn of account record with id 2d813f43-3a8d-ed11-81ad-000d3a993550
        Download failed: No FileAttachment records found for imagedescriptorId: 34813f43-3a8d-ed11-81ad-000d3a993550 for image attribute: sample_imagecolumn of account record with id 32813f43-3a8d-ed11-81ad-000d3a993550
        Download failed: No FileAttachment records found for imagedescriptorId: 3d813f43-3a8d-ed11-81ad-000d3a993550 for image attribute: sample_imagecolumn of account record with id 3c813f43-3a8d-ed11-81ad-000d3a993550
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 144x144.png_downloaded.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 144x400.png_downloaded.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 400x144.png_downloaded.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 400x500.png_downloaded.png
        Full-sized file downloaded to DownloadedImages\CanStoreFullImage true 60x80.png_downloaded.png
Deleted the records created for this sample.
Deleting the image column named 'sample_ImageColumn' on the account table ...
Deleted the image column named 'sample_ImageColumn' in the account table.
Sample completed.
```

## Demonstrates

The code for this sample is in the [Program.cs](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/CSharp-NETCore/ImageOperations/Program.cs) file.

The project uses a `Utility` class to perform operations involving creating or retrieving schema data. This class is in the [Utility.cs](https://github.com/microsoft/PowerApps-Samples/blob/master/dataverse/orgsvc/CSharp-NETCore/ImageOperations/Utility.cs) file.

This project performs these operations:

### Create an image column

This sample needs to create a new image column that is the primary image for the account table. It must also return the system to the original state when it's finished. So these steps are performed:

1. Capture the original primary image name using the `Utility.GetTablePrimaryImageName` method.
1. Use the `Utility.CreateImageColumn` method to create a new image column named `sample_ImageColumn` on the account table if it doesn't exist already.

   > **Note:**
   > This image column `CanStoreFullImage` value is false.

1. Use the `Utility.SetTablePrimaryImageName` method to make `sample_ImageColumn` the primary image.

### Create account records with image data

1. The program loops through a list of five filenames that match the names of files in the `Images` folder.
1. For each image, it creates an account record with the `name` set to `CanStoreFullImage false {fileName}` and the file `byte[]` set as the `sample_ImageColumn` value.
1. The program then uses the `Utility.UpdateCanStoreFullImage` method to set the `sample_ImageColumn` definition `CanStoreFullImage` value to true.
1. Again, the program loops through the file names and creates five account records with the same image files set to the `sample_ImageColumn` value. This time the `name` is set to: `CanStoreFullImage true {fileName}`.

In the following program, we can see how the value of the `CanStoreFullImage` property changes what data is available.

### Retrieve the account records

1. The program retrieves the 10 account records created in the previous step, including the image data.
1. For each account record, the image data is downloaded to the `DownloadedImages` folder with the name `{recordName}_retrieved.png`.

   > **Note:**
   > All of these records are thumbnail-sized images.

### Download the account record images

The program uses the static `DownloadFile` method, which encapsulates the use of the
[InitializeFileBlocksDownloadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializefileblocksdownloadrequest)
and [DownloadBlockRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.downloadblockrequest) classes to download the files.

> **Note:**
> It's expected that 5 of the 10 operations will fail because no full-sized images were uploaded while `CanStoreFullImage` was false. Those records created while `CanStoreFullImage` was true succeed.

### Delete the image data

1. The program deletes the image data for each account record by setting the `sample_ImageColumn` value to null and updating the record.
1. The program verifies the deletion by retrieving the records again using the same criteria as before. No value is returned for the `CanStoreFullImage` attribute because it's null.

### Clean up

To leave the system in the state before the sample ran, it does the following:

- Delete the account records
- Set the primary image back to the original
- Delete the image column
