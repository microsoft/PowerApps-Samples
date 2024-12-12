# Web API File Operations with actions sample

This document describes the how the `FileOperationsWithActions` project implements the common operations described in [Web API File Operations sample](../README.md). See that document for an overview and how to run this sample.

## Demonstrates

The `FileOperationsWithActions` project demonstrates the use of the following Dataverse Web API actions when working with file data:

- [InitializeFileBlocksUpload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/initializefileblocksupload)
- [UploadBlock Action Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/uploadblock)
- [CommitFileBlocksUpload  Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/commitfileblocksupload)
- [InitializeFileBlocksDownload Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/initializefileblocksdownload)
- [DownloadBlock Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/downloadblock)
- [DeleteFile Action](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/reference/deletefile)

This sample uses classes defined within the [WebAPIService project Messages folder](../../WebAPIService/Messages/) so that they can be re-used. These classes inherit from the .NET [HttpRequestMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httprequestmessage?view=net-6.0) and [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage?view=net-6.0) classes. These classes have constructors that accept the relevant variables to compose the requests to send, or to deserialize the responses into standard properties.

The code for this sample is in the [Program.cs](Program.cs) file.

## Upload File

To upload a PDF file named `25MB.pdf` to the `sample_FileColumn` file column on the account record, this sample uses an `UploadFile` static method that accepts all the parameters needed to make the following requests:

1. Initialize the upload with the [InitializeFileBlocksUploadRequest class](../../WebAPIService/Messages/InitializeFileBlocksUploadRequest.cs)
1. Process the response with the [InitializeFileBlocksUploadResponse class](../../WebAPIService/Messages/InitializeFileBlocksUploadResponse.cs) to get the `FileContinuationToken` that will be used in all the following requests.
1. Split the file up into 4MB block and send each block using the [UploadBlockRequest class](../../WebAPIService/Messages/UploadBlockRequest.cs). Each instance must include a base64 encoded string to uniquely identify the block. This request doesn't have any response value to process.
1. After all the blocks are sent, use the [CommitFileBlocksUploadRequest class](../../WebAPIService/Messages/CommitFileBlocksUploadRequest.cs) with an array of the base64 encoded string values to finalize the operation.
1. Process the response with the [CommitFileBlocksUploadResponse class](../../WebAPIService/Messages/CommitFileBlocksUploadResponse.cs) to get the `FileId` value that will be used to delete the file.

## Download File

To download the PDF file named `25MB.pdf` that was just uploaded to the `sample_FileColumn` file column on the account record, this sample uses an `DownloadFile` static method that accepts all the parameters needed to make the following requests:

1. If the file was succesfully uploaded, initialize the download with the [InitializeFileBlocksDownloadRequest class](../../WebAPIService/Messages/InitializeFileBlocksDownloadRequest.cs)
1. Process the response with the [InitializeFileBlocksDownloadResponse class](../../WebAPIService/Messages/InitializeFileBlocksDownloadResponse.cs) to get the `FileContinuationToken` that will be used in all the following requests.
1. Instantiate a `List<byte>` variable to capture the portions of the file as it is downloaded.
1. Download the file in 4MB blocks with multiple requests using the [DownloadBlockRequest class](../../WebAPIService/Messages/DownloadBlockRequest.cs). 
1. Process each response with the [DownloadBlockResponse class](../../WebAPIService/Messages/DownloadBlockResponse.cs) to get the partial `byte[]` from the `Data` property. Add that portion to the `List<byte>`.
1. After all the blocks are recieved, return the `List<byte>` as an array.

## Delete File

If the file was succesfully uploaded, use the [DeleteFileRequest class](../../WebAPIService/Messages/DeleteFileRequest.cs) to delete the file using the `FileId` value returned by the `CommitFileBlocksUploadResponse`. This request doesn't have any response value to process.