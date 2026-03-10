# Sample: ExportDataUsingFetchXmlToAnnotation Custom API

This sample shows how to write a plug-in that supports a custom API 
named `sample_ExportDataUsingFetchXmlToAnnotation`.

The plug-in provides logic for the main operation of the custom API. 
The `sample_ExportDataUsingFetchXmlToAnnotation` custom api retrieves data using 
the provided `FetchXML` input parameter and creates a CSV file. It then creates
an annotation record and returns the `annotationid` as the `AnnotationId` response property.

**NOTE :**
The size of data in CSV file should be under the attachment size limit 
specified in the system settings; otherwise the creation of attachment fails.

## How to run this sample

To run the code found in this sample, you must first create a custom API in your organization.

There are two ways to create the custom api:

1. [Import the managed solution file](#import-the-managed-solution-file)
1. [Create the custom APIs](#create-the-custom-apis)

### Import the managed solution file

The `ExportDataUsingFetchXmlToAnnotation_1_0_0_0_managed.zip` in this folder contains the `sample_ExportDataUsingFetchXmlToAnnotation` custom API that uses this code, and a cleanup API `sample_CleanupExportedDataAnnotations`. You can import this solution file to create the custom API in your organization.  See [Import solutions](https://learn.microsoft.com/power-apps/maker/data-platform/import-update-export-solutions) for instructions.

After you're finished testing, use the `sample_CleanupExportedDataAnnotations` custom API to delete the data created and delete the managed solution to remove the custom API.

`sample_ExportDataUsingFetchXmlToAnnotation` is an unbound custom API. It takes one input parameter `FetchXml`, which is used to fetch the data and returns `AnnotationId` the record ID of the created annotation record that contains the CSV file data.

The `sample_CleanupExportedDataAnnotations` API has no input/output parameters.

### Create the custom APIs

You can also build the plug-in assembly in this project, create the custom API and associate the plug-in step using one of several methods. More information: [Create a custom API](https://learn.microsoft.com/power-apps/developer/data-platform/custom-api#create-a-custom-api)

There are two custom apis in this solution. The following JSON describing these custom APIs was retrieved using Web API. More information: [Retrieve data about custom APIs](https://learn.microsoft.com/power-apps/developer/data-platform/custom-api-tables#retrieve-data-about-custom-apis)

#### sample_ExportDataUsingFetchXmlToAnnotation

```json
{
    "uniquename": "sample_ExportDataUsingFetchXmlToAnnotation",
    "allowedcustomprocessingsteptype@OData.Community.Display.V1.FormattedValue": "None",
    "allowedcustomprocessingsteptype": 0,
    "bindingtype@OData.Community.Display.V1.FormattedValue": "Global",
    "bindingtype": 0,
    "boundentitylogicalname": null,
    "description": "Exports data using the input Fetch Xml to CSV attaches to an annotation record.",
    "displayname": "Export Data Using Fetch XML to Annotation",
    "executeprivilegename": null,
    "isfunction@OData.Community.Display.V1.FormattedValue": "No",
    "isfunction": false,
    "isprivate@OData.Community.Display.V1.FormattedValue": "No",
    "isprivate": false,
    "workflowsdkstepenabled@OData.Community.Display.V1.FormattedValue": "No",
    "workflowsdkstepenabled": false,
    "customapiid": "bd8ffcee-5a38-4d0a-b296-6848c94dd22e",
    "iscustomizable": {
        "Value": true,
        "CanBeChanged": true,
        "ManagedPropertyLogicalName": "iscustomizableanddeletable"
    },
    "CustomAPIRequestParameters": [
        {            
            "uniquename": "FetchXml",
            "name": "Fetch Xml",
            "description": "Fetch XML which is used to fetch all data and export to CSV",
            "displayname": "Fetch Xml",
            "type@OData.Community.Display.V1.FormattedValue": "String",
            "type": 10,
            "logicalentityname": null,
            "isoptional@OData.Community.Display.V1.FormattedValue": "No",
            "isoptional": false
        }
    ],
    "CustomAPIResponseProperties": [
        {
            "uniquename": "AnnotationId",
            "name": "Annotation Id",
            "description": "Id of the created annotation entity record.",
            "displayname": "Annotation Id",
            "type@OData.Community.Display.V1.FormattedValue": "Guid",
            "type": 12,
            "logicalentityname": null
        }
    ],
    "PluginTypeId": {
        "typename": "PowerApps.Samples.ExportDataUsingFetchXmlToAnnotationPlugin",
        "version": "1.0.0.0",
        "name": "PowerApps.Samples.ExportDataUsingFetchXmlToAnnotationPlugin",
        "assemblyname": "ExportDataUsingFetchXmlToAnnotation"
    }
}
```

#### sample_CleanupExportedDataAnnotations

```json
{
    "uniquename": "sample_CleanupExportedDataAnnotations",
    "allowedcustomprocessingsteptype@OData.Community.Display.V1.FormattedValue": "None",
    "allowedcustomprocessingsteptype": 0,
    "bindingtype@OData.Community.Display.V1.FormattedValue": "Global",
    "bindingtype": 0,
    "boundentitylogicalname": null,
    "description": "Clean Up Exported Data Annotations",
    "displayname": "Clean Up Exported Data Annotations",
    "executeprivilegename": null,
    "isfunction@OData.Community.Display.V1.FormattedValue": "No",
    "isfunction": false,
    "isprivate@OData.Community.Display.V1.FormattedValue": "No",
    "isprivate": false,
    "workflowsdkstepenabled@OData.Community.Display.V1.FormattedValue": "No",
    "workflowsdkstepenabled": false,
    "iscustomizable": {
        "Value": true,
        "CanBeChanged": true,
        "ManagedPropertyLogicalName": "iscustomizableanddeletable"
    },
    "CustomAPIRequestParameters": [],
    "CustomAPIResponseProperties": [],
    "PluginTypeId": {
        "typename": "PowerApps.Samples.CleanUpExportedDataAnnotationsPlugin",
        "version": "1.0.0.0",
        "name": "PowerApps.Samples.CleanUpExportedDataAnnotationsPlugin",
        "assemblyname": "ExportDataUsingFetchXmlToAnnotation"
    }
}
```


## How this sample works

You can use either the Web API or the Organization Service using the Dataverse SDK for .NET to use the `sample_ExportDataUsingFetchXmlToAnnotation` custom API.

### SDK for .NET


1. You can use the Organization Service Quick Start sample instructions to create a .NET Console application with C#. See [Quickstart: Execute an Organization service request (C#)](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/quick-start-org-service-console-app)
1. Add the following static method to the program class to create a reusable method for exporting data using FetchXML to Annotation.

   ```csharp
   static Guid ExportDataUsingFetchXmlToAnnotation(IOrganizationService service)
   {
       var req = new OrganizationRequest("sample_ExportDataUsingFetchXmlToAnnotation")
       {
           ["FetchXml"] = @"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
                           <entity name='account'>
                               <attribute name='accountid'/>
                               <attribute name='name'/>  
                           </entity>
                       </fetch>"
       };
   
       var resp = service.Execute(req);
   
       var annotationId = (Guid)resp["AnnotationId"];
   
       return annotationId;
   }
   ```

1. Replace the code that is calling `WhoAmIRequest` with the following code:

   ```csharp
    var annotationId = ExportDataUsingFetchXmlToAnnotation(svc)
   ```

### Web API

To use the `sample_ExportDataUsingFetchXmlToAnnotation` custom API with the Web API, send a `POST` request to the API endpoint. You can use Postman to send this request. More information: [Set up a Postman environment](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/setup-postman-environment)

 **Request**

```http
POST [Organization URI]/api/data/v9.2/sample_ExportDataUsingFetchXmlToAnnotation
Content-Type: application/json
OData-MaxVersion: 4.0
OData-Version: 4.0
Accept: application/json

{
    "FetchXml": "<fetch version='1.0' output-format='xml-platform' mapping='logical'>
                    <entity name='account'>
                        <attribute name='accountid'/>
                        <attribute name='name'/>  
                    </entity>
                </fetch>"
}
```

 **Response**

```http
HTTP/1.1 200 OK
OData-Version: 4.0
Content-Type: application/json; odata.metadata=minimal

{
    "@odata.context": "[Organization URI]/api/data/v9.2/$metadata#Microsoft.Dynamics.CRM.sample_ExportDataUsingFetchXmlToAnnotation",
    "AnnotationId": "c9fdfe63-41e3-ed11-8845-0022480b2800"
}
```

The `AnnotationId` value indicates the record in annotation table.


### Demonstrates

1. How to recursively fetch data from fetch xml.
1. How to create a csv attachment to annotation entity.
1. How to write a plug-in to support a custom API
1. How to use a custom API using the Web API
1. How to use a custom API using the Organization service

## Clean Up

To clean up all the created data, use the `sample_CleanupExportedDataAnnotations` custom API action to delete the created annotation records, and then uninstall the managed solution.

`sample_CleanupExportedDataAnnotations` deletes all annotation records that meet the following criteria:


|Column|Value|
|---------|---------|
|`subject`|`Export Data Using FetchXml To Csv`|
|`filename`|`exportdatausingfetchxml.csv`|


You can use either the Web API or the Dataverse SDK for .NET to use the `sample_CleanupExportedDataAnnotations` custom API.

### SDK for .NET


1. You can use the Organization Service Quick Start sample instructions to create a .NET Console application with C#. See [Quickstart: Execute an Organization service request (C#)](https://learn.microsoft.com/power-apps/developer/data-platform/org-service/quick-start-org-service-console-app)
1. Add the following static method to the program class to create a reusable method for deleting the data created using the `sample_ExportDataUsingFetchXmlToAnnotation` custom api.

   ```csharp
   static void CleanupExportedDataAnnotations(IOrganizationService service)
   {
       var req = new OrganizationRequest("sample_CleanupExportedDataAnnotations")
   
      service.Execute(req);
   }
   ```

1. Replace the code that is calling `WhoAmIRequest` with the following code:

   ```csharp
    CleanupExportedDataAnnotations(svc)
   ```

### Web API

To use the `sample_CleanupExportedDataAnnotations` custom API with the Web API, send a `POST` request to the API endpoint. You can use Postman to send this request. More information: [Set up a Postman environment](https://learn.microsoft.com/power-apps/developer/data-platform/webapi/setup-postman-environment)

 **Request**

```http
POST [Organization URI]/api/data/v9.2/sample_CleanupExportedDataAnnotations
Content-Type: application/json
OData-MaxVersion: 4.0
OData-Version: 4.0
Accept: application/json
```

 **Response**

```http
HTTP/1.1 204 No Content
OData-Version: 4.0
Content-Type: application/json; odata.metadata=minimal
```

---


## See also

[Create and use Custom APIs](https://learn.microsoft.com/powerapps/developer/data-platform/custom-api)<br />
[Write a plug-in](https://learn.microsoft.com/powerapps/developer/common-data-service/write-plug-in)<br />
[Register a plug-in](https://learn.microsoft.com/powerapps/developer/common-data-service/register-plug-in)