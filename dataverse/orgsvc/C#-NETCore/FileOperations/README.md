# SDK for .NET File Operations Sample

This .NET 6.0 sample demonstrates how to perform operations with file columns using the Dataverse SDK for .NET.

This sample uses the [Microsoft.PowerPlatform.Dataverse.Client.ServiceClient Class](https://learn.microsoft.com/dotnet/api/microsoft.powerplatform.dataverse.client.serviceclient).

## Prerequisites

- Microsoft Visual Studio 2022
- Access to Dataverse with system administrator or system customizer privileges

## How to run the sample

1. Clone or download the [PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples) repository.
1. Open the `PowerApps-Samples/dataverse/orgsvc/C#-NETCore/FileOperations/FileOperations.sln` file using Visual Studio 2022.
1. Edit the `appsettings.json` file. Set the connection string `Url` and `Username` parameters for your test environment.

   The environment URL can be found in the Power Platform admin center. The URL has the form `https://<environment-name>.crm.dynamics.com`.

1. Build the solution, and then run the desired project.

When the sample runs, you're prompted in your default browser to select an environment user account and enter a password. To avoid repeated authentication every time you run a sample, insert a password parameter into the connection string in the `appsettings.json` file. For example:

```json
{
"ConnectionStrings": {
    "default": "AuthType=OAuth;Url=https://myorg.crm.dynamics.com;Username=someone@myorg.onmicrosoft.com;Password=mypassword;RedirectUri=http://localhost;AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;LoginPrompt=Auto"
  }
}
```

> [!TIP]
> You can set a user environment variable named DATAVERSE_APPSETTINGS to the file path of the `appsettings.json` file stored anywhere on your computer. The samples use that file if the environment variable exists and isn't null. Log out and back in again after you define the variable for it to take affect. To set an environment variable, go to **Settings > System > About**, select **Advanced system settings**, and then choose **Environment variables**.

## Sample Output

You can expect this output for the project:

```cmd
Creating file column named 'sample_FileColumn' on the account table ...
Created file column named 'sample_FileColumn' in the account table.
Created account record with accountid:5fb4f993-7c55-ed11-bba3-000d3a9933c9
Uploading file Files\25mb.pdf ...
Uploaded file Files\25mb.pdf
Downloading file from accounts(5fb4f993-7c55-ed11-bba3-000d3a9933c9)/sample_filecolumn ...
Downloaded the file to E:\GitHub\PowerApps-Samples\dataverse\webapi\C#-NETx\FileOperations\FileOperationsWithActions\bin\Debug\net6.0//downloaded-25mb.pdf.
Deleted the file using FileId.
Deleted the account record.
Deleting the file column named 'sample_filecolumn' on the account table ...
Deleted the file column named 'sample_filecolumn' in the account table.
```

## Demonstrates

The project uses a `Utility` class to perform operations involving creating or retrieving schema data.

The code for this sample is in the [Program.cs](Program.cs) file.

This project performs these operations:

- [Create a file column](#create-a-file-column)
- [Update a file column](#update-a-file-column)
- [Retrieve the file column](#retrieve-the-file-column-maxsizeinkb-value)
- [Create an account record](#create-an-account-record)
- [Upload a file](#upload-a-file)
- [Download the file](#download-the-file)
- [Delete the file](#delete-the-file)

### Create a file column

The `Utility.CreateFileColumn` function creates a file column named `sample_FileColumn` in the account table with a `MaxSizeInKb` value of 10MB.

### Update a file column

The `Utility.UpdateFileColumnMaxSizeInKB` function updates the `MaxSizeInKb` value of the `sample_FileColumn` file column to 100MB.

> [!TIP]
> If you want to create some error scenarios because the file column size is too small, comment out this line.

### Retrieve the file column MaxSizeInKb value

The `Utility.GetFileColumnMaxSizeInKb` retrieves the `MaxSizeInKb` value of the `sample_FileColumn` file column and stores it in a variable named `fileColumnMaxSizeInKb`.

### Create an account record

Before a file can be uploaded to the file column, a record must exist.

### Upload a file

To upload a PDF file named `25MB.pdf` to the `sample_FileColumn` file column on the account record, this sample uses an `UploadFile` static method that accepts parameters to make the following requests:

1. Initializes the upload with the [InitializeFileBlocksUploadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializefileblocksuploadrequest) class.
1. Processes the response with the [InitializeFileBlocksUploadResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.initializefileblocksuploadresponse) class to get the `FileContinuationToken`  used in all the following requests.
1. Tests the size of the file: The `UploadFile` method accepts a parameter named `fileColumnMaxSizeInKb`. If the file is larger than the configured limit of the file column, it throws an error.
1. Splits the file up into 4MB blocks and send each block using the [UploadBlockRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.uploadblockrequest) class. Each instance must include a base64 encoded string to uniquely identify the block. This request has no response value to process.
1. After all the blocks are sent, uses the [CommitFileBlocksUploadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.CommitFileBlocksUploadRequest) class with an array of the base64 encoded string values to finalize the operation.
1. Processes the response with the [CommitFileBlocksUploadResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.CommitFileBlocksUploadResponse) class to get the `FileId` value that's used to delete the file.

### Download the file

To download the PDF file `25MB.pdf` uploaded to the `sample_FileColumn` file column on the account record, this sample uses a `DownloadFile` static method that accepts parameters to make the following requests:

1. If the file was succesfully uploaded, initializes the download with the [InitializeFileBlocksDownloadRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.InitializeFileBlocksDownloadRequest).
1. Processes the response with the [InitializeFileBlocksDownloadResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.InitializeFileBlocksDownloadResponse) class to get the `FileContinuationToken` that's used in the following requests.
1. Instantiates a `List<byte>` variable to capture the portions of the file as it's downloaded.
1. Downloads the file in 4MB blocks with multiple requests using the [DownloadBlockRequest](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.DownloadBlockRequest) class.
1. Processes each response with the [DownloadBlockResponse](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.DownloadBlockResponse) class to get the partial `byte[]` from the `Data` property. Add that portion to the `List<byte>`.
1. After all the blocks are recieved, returns the `List<byte>` as an array.
1. The file is saved to the current directory. You can try opening the file to confirm it was uploaded and downloaded correctly.

### Delete the file

If the file was succesfully uploaded, use the [DeleteFileRequest class](https://learn.microsoft.com/dotnet/api/microsoft.crm.sdk.messages.DeleteFileRequest) to delete the file using the `FileId` value returned by the `CommitFileBlocksUploadResponse`. This request doesn't have any response value to process.

### Clean up

To return the system to the state before the sample ran, the sample does the following:

- Deletes the account record
- Deletes the file column
