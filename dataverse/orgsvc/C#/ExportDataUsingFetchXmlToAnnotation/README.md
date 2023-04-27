# Sample: ExportDataUsingFetchXmlToAnnotation Custom API

This sample shows how to write a plug-in that supports a Custom API 
named `sample_ExportDataUsingFetchXmlToAnnotation`. 
You can download the sample from 
[here](https://github.com/Microsoft/PowerApps-Samples/tree/master/cds/orgsvc/C%23/ExportDataUsingFetchXmlToAnnotation).

The plug-in provides logic for the main operation of the Custom API. 
The `sample_ExportDataUsingFetchXmlToAnnotation` custom api retrieves data using 
the provided `FetchXML` input parameter and creates a CSV file. It then creates
an annotation record and returns the annotationid as the `AnnotationId` response property..

**NOTE :**
The size of data in CSV file should be under the attachment size limit 
specified in the system settings; otherwise the creation of attachment will fail.

## How to run this sample

To run the code found in this sample, you must first create a Custom API in your organization.

### Import the managed solution file

The `ExportDataUsingFetchXmlToAnnotation_1_0_0_0_managed.zip` in this folder contains the `sample_ExportDataUsingFetchXmlToAnnotation` Custom API that uses this code, and a cleanup API `sample_CleanupExportedDataAnnotations`. You can simply import this solution file to create the Custom API in your organization.  See [Import solutions](https://docs.microsoft.com/powerapps/maker/data-platform/import-update-export-solutions) for instructions.

After you are finished testing, invoke the clean up Custom API `sample_CleanupExportedDataAnnotations` and delete the managed solution to remove the Custom API.

The Custom API `sample_ExportDataUsingFetchXmlToAnnotation` is an unbound API. It takes one input parameter `FetchXml` which is used to fetch the data and returns `AnnotationId` the record Id of the created annotation record which will have the CSV attached.

The `sample_CleanupExportedDataAnnotations` API has no input/output parameters.

## How this sample works

You can use either the Web API or the Organization Service using the Dataverse .NET Framework SDK assemblies to invoke the Custom API.

### Using Web API
To use the Custom API with the Web API, send a POST request to the API endpoint.

 **Request**

```http

POST [Organization URI]/api/data/v9.2/sample_ExportDataUsingFetchXmlToAnnotation HTTP/1.1
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
```
```
{
    "@odata.context": "https://yourorgname.api.crm.dynamics.com/api/data/v9.2/$metadata#Microsoft.Dynamics.CRM.sample_ExportDataUsingFetchXmlToAnnotation",
    "AnnotationId": "c9fdfe63-41e3-ed11-8845-0022480b2800"
}
```
The `AnnotationId` value indicates the record in annotation table.


### Using Organization Service

1. You can use the Organization Service Quick Start sample instructions to create a .NET Framework Console application with C#. See [Quickstart: Organization service sample (C#)](https://docs.microsoft.com/en-us/powerapps/developer/data-platform/org-service/quick-start-org-service-console-app)
1. Add the following static method to the program class to create a re-usable method for exporting data using FetchXML to Annotation.

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

1. Replace the code that is calling `WhoAmIRequest` with the following:

   ```csharp
    var annotationId = ExportDataUsingFetchXmlToAnnotation(svc)
   ```

### Demonstrate

1. How to recursively fetch data from fetch xml.
1. How to create a csv attachment to annotation entity.
1. How to write a plug-in to support a Custom API
1. How to invoke a Custom API using the Web API
1. How to invoke a Custom API using the Organization service

## Clean Up

To clean up all the created data, invoke the `sample_CleanupExportedDataAnnotations` Custom API to delete the created Annotation records, and then uninstall the managed solution.

## See also

[Create and use Custom APIs](https://docs.microsoft.com/powerapps/developer/data-platform/custom-api)<br />
[Write a plug-in](https://docs.microsoft.com/powerapps/developer/common-data-service/write-plug-in)<br />
[Register a plug-in](https://docs.microsoft.com/powerapps/developer/common-data-service/register-plug-in)